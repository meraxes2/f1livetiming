/*
 *  f1livetiming - Part of the Live Timing Library for .NET
 *  Copyright (C) 2009 Liam Lowey
 *  
 *      http://livetiming.turnitin.co.uk/
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  you may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at 
 *  
 *      http://www.apache.org/licenses/LICENSE-2.0 
 *  
 *  Unless required by applicable law or agreed to in writing, software 
 *  distributed under the License is distributed on an "AS IS" BASIS, 
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 *  See the License for the specific language governing permissions and 
 *  limitations under the License. 
 */

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Patterns.Command;
using Common.Utils.Threading;
using F1.Runtime;
using F1.Exceptions;
using log4net;

namespace F1.Network
{
    /// <summary>
    /// This driver is used to connect to the live server and pull the data. It's implementation relies
    /// on the proactor async pattern implemented by .NET sockets to send/receive data. However calls
    /// into the socket are all serialised to prevent race conditions through the command queue based
    /// thread. 
    /// </summary>
    class AsyncConnectionDriver : SimpleThreadedQueueBase, IDriver, IDisposable
    {
        #region Configuration Constants
        const string HOST = "live-timing.formula1.com";
        const int PORT = 4321;
        const int BLOB_SIZE = 256;
        const int SEND_INTERVAL = 1000;
        #endregion

        private readonly ILog _log = LogManager.GetLogger("AsyncConnectionDriver");

        #region Internal data
        private readonly Socket _incoming;
        private readonly Runtime.Runtime _runtime;
        private readonly MemoryStream _memStream;
        private int _refreshRate = SEND_INTERVAL;
        private readonly Timer _timer;
        #endregion

        public AsyncConnectionDriver(Runtime.Runtime runtime, MemoryStream memStream) 
            : base(false)
        {
            try
            {
                _memStream = memStream;
                _runtime = runtime;

                _runtime.Driver = this;

                IPHostEntry e = Dns.GetHostEntry(HOST);

                _incoming = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _log.InfoFormat("Resolved {0} to {1}.", HOST, e.AddressList[0].ToString());

                _log.Info("Connecting...");

                _incoming.Connect(e.AddressList, PORT);

                _incoming.ReceiveTimeout = 10000; // 10 seconds because we may not always be receiving data

                //  Queue the first read and write
                CmdQueue.Push(CommandFactory.MakeCommand(DoNextRead, new byte[BLOB_SIZE]));
                CmdQueue.Push(CommandFactory.MakeCommand(DoPing));

                Start();

                //  We've sent the first request so schedule timer to start one interval later
                _timer = new Timer(TimerPingCallback, null, _refreshRate, _refreshRate);
                
                _log.Info("Connected.");
                
            }
            catch (SocketException e)
            {
                throw new ConnectionException(e);
            }
        }


        #region Internal writer thread

        private void TimerPingCallback(object state)
        {
            // It's time to do another ping
            CmdQueue.Push(CommandFactory.MakeCommand(DoPing));
        }


        private void DoPing()
        {
            byte[] reqPacket = { 0x10 };
            _incoming.BeginSend(reqPacket, 0, 1, SocketFlags.None, new AsyncCallback(OnWriteData), null);
        }


        private void OnWriteData(IAsyncResult res)
        {
            // This method exists purely to perform the remainder of the proactor
            // pattern implemented for .NET socket Async behaviour when writing.
            _incoming.EndSend(res); // Block on this call until data received.
        }

        #endregion
        

        #region Internal reader thread
        
        private void DoNextRead(byte[] blob)
        {
            _incoming.BeginReceive(blob, 0, blob.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), blob);
        }


        private void DoProcessData(byte[] blob, int dataLength)
        {
            long oldPosition = _memStream.Position;

            if (oldPosition == _memStream.Length)
            {
                //  We're at the edge of the stream, so to prevent a backlog
                // of data building up, start overwriting from the beginning.
                _memStream.Seek(0, SeekOrigin.Begin);
                _memStream.SetLength(0);
                oldPosition = 0;
            }
            else
            {
                //  There is some data still left to be read, so to avoid overwriting
                // it we must append this block to the end of the stream.
                _memStream.Seek(0, SeekOrigin.End);
            }


            //  Push new data onto the end of our stream.
            _memStream.Write(blob, 0, dataLength);


            //  Restore the read head position after the write so that the reads
            // start from the correct position.
            _memStream.Seek(oldPosition, SeekOrigin.Begin);


            //  Process new data
            try
            {
                while (_runtime.HandleRead())
                {
                }
            }
            catch(Exception e)
            {
                _log.Warn("HandleRead() - " + e.Message);

                // Error while processing the data, so we abort and discard the remainder
                // of read/write requests pending.
                Stop(JoinMethod.DontJoin, true); // Don't join this thread to this thread.
            }
        }


        private void OnReceiveData(IAsyncResult res)
        {
            int data = _incoming.EndReceive(res);

            byte[] blob = res.AsyncState as byte[];

            if (data > 0)
            {
                //  We received data so push it back onto the queue for processing
                CmdQueue.Push(CommandFactory.MakeCommand(DoProcessData, blob, data));
            }
            

            //  Queue up next read with new blob (assume that processing of data happens
            // prior to queueing the next read so we can recycle the blob).
            CmdQueue.Push(CommandFactory.MakeCommand(DoNextRead, blob));
        }

        #endregion
        

        #region Implementation of IDriver

        private void DoSetRefresh(int refreshRate)
        {
            _refreshRate = refreshRate * 1000;

            // it's fine to change the due time because we generally only
            // up the refresh rate.
            _timer.Change(_refreshRate, _refreshRate);
        }

        public void SetRefresh(int refreshRate)
        {
            CmdQueue.PushUrgent(CommandFactory.MakeCommand(DoSetRefresh, refreshRate));
        }

        public void Terminate()
        {
            _log.Warn("Terminate() - Exiting AsyncConnection.");

            // don't join because we maybe calling back from our own thread.
            // discard messages so we don't queue new socket requests after calling Dispose.
            Stop(JoinMethod.DontJoin, true); 
        }

        public void UpdateCurrentKeyFrame(int currentFrame)
        {
            
        }

        #endregion


        #region IDisposable Members
        public void Dispose()
        {
            Stop(JoinMethod.Join, true);
        }
        #endregion
    }
}

/*
 *  f1livetiming - Part of the Live Timing Library for .NET
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

using F1.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Networking;
using Common.Utils.Threading;
using Common.Patterns.Command;
using System.Threading;
using Windows.Storage.Streams;

namespace F1.Network
{
    public class W8ConnectionDriver : SimpleThreadedQueueBase, IDriver, IDisposable
    {
        #region Configuration Constants
        const string HOST = "live-timing.formula1.com";
        const int PORT = 4321;
        const int BLOB_SIZE = 256;
        const int SEND_INTERVAL = 1000;
        #endregion

        #region Internal data
        //private readonly Socket _incoming;
        private readonly StreamSocket _socket;
        private readonly Runtime.Runtime _runtime;
        private readonly MemoryStream _memStream;
        private int _refreshRate = SEND_INTERVAL;
        private Timer _timer;
        private byte[] _blob = new byte[BLOB_SIZE];
        #endregion

        public W8ConnectionDriver(Runtime.Runtime runtime, MemoryStream memStream)
            : base(false)
        {
            _memStream = memStream;
            _runtime = runtime;

            _runtime.Driver = this;
           
            _socket = new StreamSocket();
            _socket.ConnectAsync(new HostName(HOST), PORT.ToString()).AsTask().Wait();
            OnConnected();
        }

        private void OnConnected()
        {
            //  Queue the first read and write
            CmdQueue.Push(CommandFactory.MakeCommand(DoNextRead, _blob));
            CmdQueue.Push(CommandFactory.MakeCommand(DoPing));

            Start();

            //  We've sent the first request so schedule timer to start one interval later
            _timer = new Timer(TimerPingCallback, null, _refreshRate, _refreshRate);
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

            //Task.Run(()=>
            //    {
            DataWriter writer = new DataWriter(_socket.OutputStream);
            writer.WriteBytes(reqPacket);
            writer.StoreAsync().AsTask().Wait();
            writer.DetachStream();
            writer.Dispose();
            //}
        }

        #endregion

        #region Internal reader thread

        private void DoNextRead(byte[] blob)
        {
            DataReader reader = new DataReader(_socket.InputStream);
            reader.InputStreamOptions = InputStreamOptions.Partial;

            var task = Task.Run(() => 
            {
                try
                {
                    reader.LoadAsync((uint)blob.Length).AsTask().Wait();
                    uint bytesRead = 0;
                    while(reader.UnconsumedBufferLength > 0)
                    {
                        blob[bytesRead++] = reader.ReadByte();
                    }

                    if(bytesRead > 0)
                    { 
                        OnReceive(blob, bytesRead);
                    }
                }
                catch(Exception)
                {
                }
                finally
                {
                    reader.DetachStream();
                    reader.Dispose();                    
                }
            });
        }

        private void OnReceive(byte[] blob, uint bytesRead)
        {
            if(bytesRead > 0)
            {
                //  We received data so push it back onto the queue for processing
                CmdQueue.Push(CommandFactory.MakeCommand(DoProcessData, blob, (int)bytesRead));
            }

            //  Queue up next read with new blob (assume that processing of data happens
            // prior to queueing the next read so we can recycle the blob).
            CmdQueue.Push(CommandFactory.MakeCommand(DoNextRead, _blob));
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
            catch (Exception)
            {
                //_log.Warn("HandleRead() - " + e.Message);

                // Error while processing the data, so we abort and discard the remainder
                // of read/write requests pending.
                Stop(JoinMethod.DontJoin, true); // Don't join this thread to this thread.
            }
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
            //_log.Warn("Terminate() - Exiting AsyncConnection.");

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

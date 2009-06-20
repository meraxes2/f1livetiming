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
using Common.Patterns.Command;
using F1.Messages;
using F1.Messages.System;
using F1.Network;
using F1.Runtime;
using Common.Utils.Threading;
using F1.Simulator;
using log4net;

namespace F1
{
    public class LiveTimingSimulator : SimpleThreadedQueueBase, ILiveTimingApp
    {
        public event LiveTimingMessageHandlerDelegate SystemMessageHandler;
        public event LiveTimingMessageHandlerDelegate CarMessageHandler;

        #region Internal Data
        private const int MEMSTREAM_SIZE = 1024;

        private Runtime.Runtime _runtime;
        private IDisposable _connection;
        private MessageDispatcherImpl _handler;

        private readonly object _onceOnlyLock = new object();

        private readonly ILog _log = LogManager.GetLogger("LiveTimingSimulator");
        #endregion


        public LiveTimingSimulator( string keyFramePath, string liveDataFile, string username, string password, string authKeyFile, bool createThread )
            : base(false)
        {
            _log.Info("Building live timing simulator...");

            BuildSimulator(keyFramePath, liveDataFile, username, password, authKeyFile);

            if(createThread)
            {
                Start();
            }
        }

        #region ILiveTimingApp Members

        public new void Run()
        {
            lock(_onceOnlyLock)
            {
                base.Run();
            }
        }

        public void Stop( bool discard )
        {
            base.Stop( JoinMethod.DontJoin, discard );
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Stop(JoinMethod.Join, false);
            _connection.Dispose();
            _connection = null;
            _runtime = null;
        }

        #endregion

        #region Internal message handling

        /// <summary>
        /// This method is run by the calling thread of Run so as not to interupt the
        /// message processing pump.
        /// </summary>
        /// <param name="msg"></param>
        private void DoDispatchMessage(IMessage msg)
        {
            if (msg.Type == Enums.SystemPacketType.CarType)
            {
                CarMessageHandler.Invoke(msg);
            }
            else
            {
                if (msg is EndOfSession)
                {
                    // Tell the thread to stop blocking and exit after it's processed the remainder of messages.
                    Stop(JoinMethod.DontJoin, false);
                }

                SystemMessageHandler.Invoke(msg);
            }
        }


        private class MessageDispatcherImpl : IMessageDispatch
        {
            private readonly LiveTimingSimulator _lt;

            public MessageDispatcherImpl(LiveTimingSimulator lt)
            {
                _lt = lt;
            }

            public void Dispatch(IMessage msg)
            {
                _lt.CmdQueue.Push(CommandFactory.MakeCommand(_lt.DoDispatchMessage, msg));
            }
        }

        #endregion

        #region Simulator builder

        private void BuildSimulator(string keyFramePath, string liveDataFile, string username, string password, string authKeyFile)
        {
            IKeyFrame kf = new Simulator.KeyFrame(keyFramePath);
            
            IAuthKey ak;

            if( string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) )
            {
                ak = new AuthorizationKey(authKeyFile);
            }
            else
            {
                //  We can still use this in the sim, albeit a bit cheeky :)
                ak = new F1.Protocol.AuthorizationKey(username, password);
            }

            _handler = new MessageDispatcherImpl(this);

            MemoryStream memStream = new MemoryStream(MEMSTREAM_SIZE);

            _runtime = new Runtime.Runtime(memStream, ak, kf, _handler);

            //  Create network component that drives the Runtime with data.
            CreateDriver(liveDataFile, kf, memStream);
        }


        private void CreateDriver(string liveDataPath, IKeyFrame keyFrameService, MemoryStream memStream)
        {
            if (string.IsNullOrEmpty(liveDataPath))
            {
                _connection = new KeyFrameDriver(keyFrameService, _runtime);
            }
            else
            {
                _connection = new FileCapDriver(liveDataPath, memStream, _runtime);
            }
        }

        #endregion
    }
}

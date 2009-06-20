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
using Common.Utils.Threading;
using F1.Messages.System;
using F1.Runtime;
using F1.Protocol;
using F1.Messages;
using F1.Network;
using F1.Exceptions;
using log4net;
using KeyFrame=F1.Protocol.KeyFrame;

namespace F1
{
    /// <summary>
    /// This simple object is the API into the live timing library. You should create one which either
    /// creates its own internal thread to deliver messages on, or you should call the blocking Run()
    /// method. All events are delivered by the thread calling Run() and is seperate from the network
    /// threads, so feel free to use the event handlers' context as you need all it will do is block
    /// other messages' arival until your call exits.
    /// </summary>
    public class LiveTiming : SimpleThreadedQueueBase, ILiveTimingApp
    {
        public event LiveTimingMessageHandlerDelegate SystemMessageHandler;
        public event LiveTimingMessageHandlerDelegate CarMessageHandler;
        

        #region Internal Data
        private const int MEMSTREAM_SIZE = 1024;

        private Runtime.Runtime _runtime;
        private IDisposable _connection;
        private MessageDispatcherImpl _handler;
        
        private readonly object _onceOnlyLock = new object();
        
        private readonly ILog _log = LogManager.GetLogger("LiveTiming");
        #endregion


        public LiveTiming( string username, string password, bool createThread )
            : base(false)
        {
            _log.Info("Logging into LiveTiming server...");

            //  Log in to the live timing server
            IAuthKey authKeyService = Login(username, password);

            //  Set up the Runtime object for business logic to the live timing.
            InitRuntime(authKeyService);

            if (createThread)
            {
                // The caller has promised not to call Run, so we must.
                Start();
            }
        }


        public void Dispose()
        {
            Stop(JoinMethod.Join, false);
            _connection.Dispose();
            _connection = null;
            _runtime = null;
        }


        /// <summary>
        /// Expose Run method for outside to call. If when constructed createThread was
        /// set to true, then this method will already be called by a new thread and 
        /// so shouldn't be called by the application.
        /// </summary>
        public new void Run()
        {
            lock (_onceOnlyLock) // Stop more than one thread from running this method.
            {
                base.Run();
            }
        }


        public void Stop(bool discard)
        {
            Stop(JoinMethod.DontJoin, discard);
        }


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
                if(msg is EndOfSession)
                {
                    // Tell the thread to stop blocking and exit after it's processed the remainder of messages.
                    Stop(JoinMethod.DontJoin, false);
                }

                SystemMessageHandler.Invoke(msg);
            }
        }

        
        private class MessageDispatcherImpl : IMessageDispatch
        {
            private readonly LiveTiming _lt;

            public MessageDispatcherImpl(LiveTiming lt)
            {
                _lt = lt;
            }

            public void Dispatch(IMessage msg)
            {
                _lt.CmdQueue.Push(CommandFactory.MakeCommand(_lt.DoDispatchMessage, msg));
            }
        }

        #endregion


        #region Internal init
        private IAuthKey Login(string username, string password)
        {
            try
            {
                return new AuthorizationKey(username, password);
            }
            catch (AuthorizationException e)
            {
                _log.Error("Could not log in to server. Reason: " + e.Message);
                throw;
            }
        }


        private void InitRuntime(IAuthKey authKeyService)
        {
            _log.Info("Connecting to live stream....");

            IKeyFrame keyFrameService = new KeyFrame();

            _handler = new MessageDispatcherImpl(this);

            MemoryStream memStream  = new MemoryStream(MEMSTREAM_SIZE);

            _runtime = new Runtime.Runtime(memStream, authKeyService, keyFrameService, _handler);

            //  Create network component that drives the Runtime with data.
            CreateDriver(keyFrameService, memStream);
        }


        private void CreateDriver(IKeyFrame keyFrameService, MemoryStream memStream)
        {
            try
            {
                _connection = new AsyncConnectionDriver(_runtime, memStream);
            }
            catch (ConnectionException)
            {
                _connection = new KeyFrameDriver(keyFrameService, _runtime);
            }
        }
        #endregion
    }
}

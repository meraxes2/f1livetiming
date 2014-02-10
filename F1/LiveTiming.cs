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
//using log4net;
using KeyFrame=F1.Protocol.KeyFrame;

namespace F1
{
    /// <summary>
    /// <para>This simple object is the API into the live timing library. You should create one which either
    /// creates its own internal thread to deliver messages on, or you should call the blocking Run()
    /// method. All events are delivered by the thread calling Run() and is seperate from the network
    /// threads, so feel free to use the event handlers' context as you need all it will do is block
    /// other messages' arival until your call exits.</para>
    /// <para>
    /// It is important to know that when establishing a connection it will attempt to connect to
    /// the live server on port 4321 however this often blocked (by firewalls). When it can not
    /// perform the connection it will 'fall back' to a keyframe method, where by a new frame
    /// is requested every 15 seconds. However this means data will only be delivered in bursts and
    /// not adhoc as in live streams. Application provider should make users aware of this.</para>
    /// <example>
    /// For an example see <see cref="ILiveTimingApp"/>.
    /// </example>
    /// </summary>
    public class LiveTiming : SimpleThreadedQueueBase, ILiveTimingApp
    {
        public event LiveTimingMessageHandlerDelegate SystemMessageHandler;
        public event LiveTimingMessageHandlerDelegate CarMessageHandler;
        public event LiveTimingMessageHandlerDelegate ControlMessageHandler;
        

        #region Internal Data
        private const int MEMSTREAM_SIZE = 1024;

        private Runtime.Runtime _runtime = null;
        private IDisposable _connection = null;
        private MessageDispatcherImpl _handler = null;
        private string _username = "";
        private string _password = "";
        
        private readonly object _onceOnlyLock = new object();
        
        //private readonly ILog _log = LogManager.GetLogger("LiveTiming");
        #endregion

        /// <summary>
        /// To create a live timing application you must have already registered at formula1.com for a username
        /// and password to access their live timing feed.
        /// </summary>
        /// <param name="username">Genuine formula1.com username for receiving authentication.</param>
        /// <param name="password">Genuine formula1.com password for receiving authentication.</param>
        /// <param name="createThread">After construction a child thread will be created, and Run will execute.</param>
        public LiveTiming( string username, string password, bool createThread )
            : base(false)
        {
            _username = username;
            _password = password;

            base.CmdQueue.Push(CommandFactory.MakeCommand(InitRuntime));

            if(createThread)
            {
                // The caller has promised not to call Run, so we must.
                Start();
            }
        }

        public void StartThread()
        {
            Start();
        }


        public void Dispose()
        {
            Stop(JoinMethod.Join, false);
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
            _runtime = null;
        }


        public F1.Enums.EventType CurrentEventType { get; private set; }

        #region ILiveTimingApp Members

        /// <summary>
        /// See <see cref="ILiveTimingApp.Run"/>
        /// </summary>
        public new void Run()
        {
            lock (_onceOnlyLock) // Stop more than one thread from running this method.
            {
                base.Run();
            }
        }

        /// <summary>
        /// See <see cref="ILiveTimingApp.Stop"/>
        /// </summary>
        public void Stop(bool discard)
        {
            Stop(JoinMethod.DontJoin, discard);
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
            else if (msg.Type == Enums.SystemPacketType.ControlType)
            {
                if (ControlMessageHandler != null)
                {
                    ControlMessageHandler.Invoke(msg);
                }
            }
            else
            {
                if (msg is EndOfSession)
                {
                    // Tell the thread to stop blocking and exit after it's processed the remainder of messages.
                    Stop(JoinMethod.DontJoin, false);
                }

                if (msg is EventId)
                {
                    CurrentEventType = (msg as EventId).EventType;
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
            catch (AuthorizationException )
            {
                //_log.Error("Could not log in to server. Reason: " + e.Message);
                throw;
            }
        }


        private void InitRuntime()
        {
            //_log.Info("Connecting to live stream....");
            try
            {
                IAuthKey authKeyService = Login(_username, _password);

                IKeyFrame keyFrameService = new KeyFrame();

                _handler = new MessageDispatcherImpl(this);

                MemoryStream memStream = new MemoryStream(MEMSTREAM_SIZE);

                _runtime = new Runtime.Runtime(memStream, authKeyService, keyFrameService, _handler);

                //  Create network component that drives the Runtime with data.
                CreateDriver(keyFrameService, memStream);
            }
            catch (AuthorizationException)
            {
                DoDispatchMessage(new F1.Messages.Control.AuthorizationProblem());
                Stop(true);
            }
        }


        private void CreateDriver(IKeyFrame keyFrameService, MemoryStream memStream)
        {
            try
            {
#if WINDOWS_PHONE
                _connection = new Wp7ConnectionDriver(_runtime, memStream);
#else
                _connection = new AsyncConnectionDriver(_runtime, memStream);
#endif
            }
            catch (ConnectionException)
            {
                _connection = new KeyFrameDriver(keyFrameService, _runtime);
            }
        }
        #endregion
    }
}

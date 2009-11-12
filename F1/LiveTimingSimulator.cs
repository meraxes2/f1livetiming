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
    /// <summary>
    /// <para>This concrete ILiveTimingApp type should be used to simulate a live timing application
    /// when no F1 event is live.</para>
    /// <para>There are three ways to construct this simulator:
    /// <list type="number">
    ///     <item>To perform a full simulation, live streaming and keyframe data.</item>
    ///     <item>To perform a full simulation + authentication. As above but authenticates with the live server</item>
    ///     <item>To perform a fallback simulation, key frames only.</item>
    /// </list>
    /// </para>
    /// <example>
    /// For an example see <see cref="ILiveTimingApp"/>.
    /// </example>
    /// </summary>
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


        /// <summary>
        /// Create the live timing simulator, and behave as if it can not connect to the streaming
        /// server. This means it will deliver keyframe data in bursts. You must provide an auth file
        /// for this.
        /// </summary>
        /// <param name="keyFramePath">Path which contains the files matching keyframe pattern: keyframe_00nnn.bin</param>
        /// <param name="authKeyFile">A valid authkey file containing event id and decryption key pairs. See <see cref="F1.Simulator.AuthorizationKey"/> for more information regarding this file type.</param>
        /// <param name="createThread">After construction a child thread will be created, and Run will execute.</param>
        public LiveTimingSimulator(string keyFramePath, string authKeyFile, bool createThread)
            : base(false)
        {
            BuildSimulator(keyFramePath, null, null, null, authKeyFile, createThread);
        }

        
        /// <summary>
        /// Create the live timing simulator with a genuine authentication server. This is useful if
        /// you have some archived data, but do not have the decrytpion key for it.
        /// </summary>
        /// <param name="keyFramePath">Path which contains the files matching keyframe pattern: keyframe_00nnn.bin</param>
        /// <param name="liveDataFile">Optional filepath of a live feed capture of data.</param>
        /// <param name="username">Genuine formula1.com username for receiving authentication.</param>
        /// <param name="password">Genuine formula1.com password for receiving authentication.</param>
        /// <param name="createThread">After construction a child thread will be created, and Run will execute.</param>
        public LiveTimingSimulator(string keyFramePath, string liveDataFile, string username, string password, bool createThread )
            : base(false)
        {
            BuildSimulator(keyFramePath, liveDataFile, username, password, null, createThread);
        }


        /// <summary>
        /// Create the live timing simulator with an auth key file. Use this if you have already retrieved
        /// the authentication key and have written it to a file.
        /// </summary>
        /// <param name="keyFramePath">Path which contains the files matching keyframe pattern: keyframe_00nnn.bin</param>
        /// <param name="liveDataFile">Optional filepath of a live feed capture of data.</param>
        /// <param name="authKeyFile">A valid authkey file containing event id and decryption key pairs. See <see cref="F1.Simulator.AuthorizationKey"/> for more information regarding this file type.</param>
        /// <param name="createThread">After construction a child thread will be created, and Run will execute.</param>
        public LiveTimingSimulator( string keyFramePath, string liveDataFile, string authKeyFile, bool createThread )
            : base(false)
        {
            BuildSimulator(keyFramePath, liveDataFile, null, null, authKeyFile, createThread);
        }

        #region ILiveTimingApp Members

        /// <summary>
        /// See <see cref="ILiveTimingApp.Run"/>
        /// </summary>
        public new void Run()
        {
            lock(_onceOnlyLock)
            {
                base.Run();
            }
        }

        /// <summary>
        /// See <see cref="ILiveTimingApp.Stop"/>
        /// </summary>
        public void Stop( bool discard )
        {
            Stop( JoinMethod.DontJoin, discard );
        }


        public F1.Enums.EventType CurrentEventType { get; private set; }

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

                if (msg is EventId)
                {
                    CurrentEventType = (msg as EventId).EventType;
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

        private void BuildSimulator(string keyFramePath, string liveDataFile, string username, string password, string authKeyFile, bool createThread)
        {
            _log.Info("Building live timing simulator...");

            IKeyFrame kf = new Simulator.KeyFrame(keyFramePath);
            
            IAuthKey ak;

            if( string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) )
            {
                ak = new AuthorizationKey(authKeyFile);
            }
            else
            {
                //  We can still use this in the sim, albeit a bit cheeky :)
                ak = new Protocol.AuthorizationKey(username, password);
            }

            _handler = new MessageDispatcherImpl(this);

            MemoryStream memStream = new MemoryStream(MEMSTREAM_SIZE);

            _runtime = new Runtime.Runtime(memStream, ak, kf, _handler);

            //  Create network component that drives the Runtime with data.
            CreateDriver(liveDataFile, kf, memStream);

            if (createThread)
            {
                _log.Info("Creating child thread to Run simulator");
                Start();
            }
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

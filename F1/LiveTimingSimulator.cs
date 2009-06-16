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

using F1.Runtime;
using Common.Utils.Threading;

namespace F1
{
    class LiveTimingSimulator : SimpleThreadBase, ILiveTimingApp
    {
        public event LiveTimingMessageHandlerDelegate SystemMessageHandler;
        public event LiveTimingMessageHandlerDelegate CarMessageHandler;


        private readonly IKeyFrame _kfProvider;
        private readonly IAuthKey _authProvider;

        
        public LiveTimingSimulator( string keyFramePath, string liveDataFile, string username, string password )
            : base(false)
        {

        }

        #region ILiveTimingApp Members

        public override void Run()
        {
            while (base.IsRunning)
            {

            }
        }

        public void Stop( bool discard )
        {
            Stop(JoinMethod.Join);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Stop(false);
        }

        #endregion
    }
}

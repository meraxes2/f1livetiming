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
using System.Threading;
using Common.Utils.Threading;
using F1.Runtime;
using System.Net;

namespace F1.Network
{
    /// <summary>
    /// Every 15 seconds this will ask for the next key frame when it becomes available.
    /// </summary>
    class KeyFrameDriver : SimpleThreadBase, IDriver, IDisposable
    {
        private const int SLEEP_TIME = 15000;
        
        private readonly IKeyFrame _provider;
        private readonly Runtime.Runtime _runtime;
        private int _currentFrameNumber;
        private readonly object _frameNumberLock = new object();


        public KeyFrameDriver(IKeyFrame provider, Runtime.Runtime runtime)
            : base(false)
        {
            _runtime = runtime;
            _runtime.Driver = this;
            _provider = provider;

            Start();
        }


        public override void Run()
        {
            //  First run
            _runtime.HandleReadAdHoc(_provider.GetKeyFrame());

            int lastTick = Environment.TickCount;

            while (IsRunning)
            {
                if( Environment.TickCount - lastTick >= SLEEP_TIME)
                {
                    try
                    {
                        _runtime.HandleReadAdHoc(_provider.GetKeyFrame(CurrentFrame + 1));
                    }
                    catch (WebException)
                    {
                    }

                    lastTick = Environment.TickCount;
                }

                Thread.Sleep(1000);//15secs
            }
        }


        private int CurrentFrame { get { lock(_frameNumberLock) return _currentFrameNumber; } }

        #region IDriver Members

        public void SetRefresh(int refreshRate)
        {
        }

        public void Terminate()
        {
            if(IsRunning)
            {
                // Don't join because Terminate could actually be on 
                // our call stack through the Runtime.
                Stop(JoinMethod.DontJoin);
            }
        }

        public void UpdateCurrentKeyFrame(int currentFrame)
        {
            lock(_frameNumberLock)
            {
                _currentFrameNumber = currentFrame;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //  Stop and join (only joins if the thread is still running).
            Stop(JoinMethod.Join);
        }

        #endregion
    }
}

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
#if WINRT
using System.Threading.Tasks;
#else
using System.Threading;
#endif

namespace Common.Utils.Threading
{
    /// <summary>
    /// This simple base class is used to give your class simple threading behaviour.
    /// </summary>
    public abstract class SimpleThreadBase
    {
        /// <summary>
        /// This type is defined because Stop(bool join) is too generic and could be
        /// needed else.
        /// </summary>
        public enum JoinMethod
        {
            DontJoin = 1,
            Join = 0
        } ;

        private readonly object _lock = new object();
        private bool _running = true;
#if WINRT
        private Task _task;
#else
        private Thread _thread;
#endif

        protected SimpleThreadBase(bool start)
        {
#if WINRT
            if(start)
            {                
                _task = Task.Run((System.Action)Run);
            }
#else
            _thread = new Thread(Run);
            if (start)
            {
                Start();
            }
#endif
        }


        protected bool IsRunning
        {
            get
            {
                lock (_lock)
                {
                    return _running;
                }
            }
        }


        /// <summary>
        /// You must implement this and call IsRunning to determine
        /// the life cycle.
        /// </summary>
        public abstract void Run();


        protected void Start()
        {
#if WINRT
            if (_task == null)
            {
                _task = Task.Run((System.Action)Run);
            }
#else
            _thread.Start();
#endif
        }


        public void Stop(JoinMethod join)
        {
            bool doJoin = false;
            lock (_lock)
            {
                if (_running)
                {
                    _running = false;
#if WINRT
                    if (_task != null)
#elif COMPACT
                    if( _thread != null )
#else
                    if( _thread != null && _thread.IsAlive )
#endif
                    {
                        doJoin = true;
                    }
                }
            }

#if WINRT
            if (doJoin && join == JoinMethod.Join)
            {
                _task.Wait();
                _task = null;
            }
#else
            if (doJoin && join == JoinMethod.Join && _thread != null )
            {
                _thread.Join();
                _thread = null;
            }
#endif
        }
    }
}

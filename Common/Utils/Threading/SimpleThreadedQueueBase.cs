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
#if WINRT
using System.Threading.Tasks;
#else
using System.Threading;
#endif
using Common.Patterns.Command;

namespace Common.Utils.Threading
{
    public abstract class SimpleThreadedQueueBase
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

        public bool IsAlive
        {
            get 
            {
#if WINRT
                return _task != null && _task.Status == TaskStatus.Running;
#else
                return _thread != null && _thread.IsAlive;
#endif
            }
        }


        #region Internal Data
        private readonly CommandQueue _cmdQueue = new CommandQueue();
        private bool _running = true;
#if WINRT
        private Task _task;
#else
        private Thread _thread;
#endif
        #endregion

      
        protected SimpleThreadedQueueBase(bool startThread)
        {
            if (startThread)
            {
                Start();
            }
        }

        /// <summary>
        /// Overridable method that executes in the thread, just before it blocks to
        /// process the command queue.
        /// </summary>
        protected virtual void PreRun()
        {

        }


        /// <summary>
        /// Overridable method that executes in the thread, just before it exits.
        /// </summary>
        protected virtual  void PostRun()
        {
            
        }


        /// <summary>
        /// If the thread wasn't started upon construction the implementing class
        /// must call this to begin the thread.
        /// </summary>
        protected void Start()
        {
#if WINRT
            if (_task == null)
            {
                _task = Task.Run((System.Action)Run);
            }
#else
            _thread = new Thread(Run);
            _thread.Start();
#endif
        }


        /// <summary>
        /// Reference to the internal command queue for adding new commands to for
        /// be processed by this thread.
        /// </summary>
        protected CommandQueue CmdQueue { get { return _cmdQueue;  } }


        /// <summary>
        /// This method must be used to tell the thread to exit.
        /// </summary>
        /// <param name="join">Specify Join to make the Stop block until the thread has exited.</param>
        /// <param name="discardMessages">true - Tells the thread to exit without processing the remainder of the command queue</param>
        public void Stop(JoinMethod join, bool discardMessages)
        {
            if (discardMessages)
            {
                _cmdQueue.PushUrgent(CommandFactory.MakeCommand(() => _running = false));
            }
            else
            {
                _cmdQueue.Push(CommandFactory.MakeCommand(() => _running = false));
            }

#if WINRT
            if (join == JoinMethod.Join && _task != null)
            {
                _task.Wait();
                _task = null;
            }
#else
    #if COMPACT
            if (join == JoinMethod.Join && _thread != null )
    #else
            if (join == JoinMethod.Join && _thread != null && _thread.IsAlive )
    #endif
            {
                _thread.Join();
                _thread = null;
            }
#endif
        }


        #region Internals
        protected void Run()
        {
            PreRun();

            try
            {
                while (_running)
                {
                    ICommand cmd = CmdQueue.Pop();
                    if(cmd != null)
                    {
                        using(cmd)
                        {
                            cmd.Execute();
                        }
                    }
                }
            }
            finally
            {
                PostRun();
            }
        }
        #endregion
    }
}

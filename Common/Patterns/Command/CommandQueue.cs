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
using System.Collections.Generic;


namespace Common.Patterns.Command
{
    public class CommandQueue
    {
        private readonly LinkedList<ICommand> _lowPriorityQueue;
        private readonly LinkedList<ICommand> _highPriorityQueue;
        private readonly object _thisLock;

        public CommandQueue()
        {
            _thisLock = new object();
            _lowPriorityQueue = new LinkedList<ICommand>();
            _highPriorityQueue = new LinkedList<ICommand>();
        }


        public void Push(ICommand cmd)
        {
            lock (_thisLock)
            {
                DoPushExtra();
                if (_lowPriorityQueue.Count == 0 && _highPriorityQueue.Count == 0)
                {
                    Monitor.PulseAll(_thisLock);
                }
                _lowPriorityQueue.AddLast(cmd);
            }
        }


        public void PushUrgent(ICommand cmd)
        {
            lock (_thisLock)
            {
                DoPushExtra();
                if (AreEmptyUnsafe)
                {
                    Monitor.PulseAll(_thisLock);
                }
                _highPriorityQueue.AddLast(cmd);
            }
        }


        public bool HasCommand
        {
            get
            {
                lock (_thisLock)
                {
                    return (!AreEmptyUnsafe);
                }
            }
        }


        public ICommand Pop(TimeSpan time)
        {
            ICommand ret = null;

            lock (_thisLock)
            {
                if (AreEmptyUnsafe)
                {
                    if (!Monitor.Wait(_thisLock, time))
                    {
                        DoPopExtra();
                        return null;
                    }
                }

                if (_highPriorityQueue.Count > 0)
                {
                    ret = _highPriorityQueue.First.Value;
                    _highPriorityQueue.RemoveFirst();
                }
                else if (_lowPriorityQueue.Count > 0)
                {
                    ret = _lowPriorityQueue.First.Value;
                    _lowPriorityQueue.RemoveFirst();
                }

                DoPopExtra();
            }

            return ret;
        }


        protected virtual void DoPushExtra() { }
        protected virtual void DoPopExtra() { }


        private bool AreEmptyUnsafe
        {
            get
            {
                return (_lowPriorityQueue.Count == 0 && _highPriorityQueue.Count == 0);
            }
        }
    }
}

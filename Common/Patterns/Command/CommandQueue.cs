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
        private bool _quit = false;

        public CommandQueue()
        {
            _thisLock = new object();
            _lowPriorityQueue = new LinkedList<ICommand>();
            _highPriorityQueue = new LinkedList<ICommand>();
        }

        public void Quit()
        {
            lock (_thisLock)
            {
                _quit = true;
                Monitor.PulseAll(_thisLock);
            }
        }

        public bool Push(ICommand cmd)
        {
            lock (_thisLock)
            {
                if (_quit) return false;

                _lowPriorityQueue.AddLast(cmd);

                Monitor.PulseAll(_thisLock);
            }

            return true;
        }


        public bool PushUrgent(ICommand cmd)
        {
            lock (_thisLock)
            {
                if (_quit) return false;

                _highPriorityQueue.AddLast(cmd);

                Monitor.PulseAll(_thisLock);
            }

            return true;
        }

        public ICommand Pop(bool waitForCommand = true)
        {
            ICommand ret = null;

            lock (_thisLock)
            {
                while (!_quit && _highPriorityQueue.Count == 0 && _lowPriorityQueue.Count == 0)
                {
                    if (!waitForCommand)
                    {
                        break;
                    }

                    Monitor.Wait(_thisLock);
                }

                if (_highPriorityQueue.Count > 0)
                {
                    ret = _highPriorityQueue.First.Value;
                    _highPriorityQueue.RemoveFirst();
                    Monitor.PulseAll(_thisLock);
                }
                else if (_lowPriorityQueue.Count > 0)
                {
                    ret = _lowPriorityQueue.First.Value;
                    _lowPriorityQueue.RemoveFirst();
                    Monitor.PulseAll(_thisLock);
                }
            }

            return ret;
        }
    }
}

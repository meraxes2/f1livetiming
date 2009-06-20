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
using F1.Messages;

namespace F1
{
    public delegate void LiveTimingMessageHandlerDelegate(IMessage msg);

    public interface ILiveTimingApp : IDisposable
    {
        event LiveTimingMessageHandlerDelegate SystemMessageHandler;
        event LiveTimingMessageHandlerDelegate CarMessageHandler;

        /// <summary>
        /// Body method to be called by parent process.
        /// </summary>
        void Run();

        /// <summary>
        /// Instruct Run method to exit
        /// </summary>
        void Stop( bool discard );
    }
}

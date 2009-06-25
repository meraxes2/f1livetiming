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

namespace F1.Runtime
{
    /// <summary>
    /// <para>
    /// Implement this interface when driving the Runtime if you want to
    /// receive callbacks from the Runtime. Tell the Runtime during
    /// construction of your implementation, see <see cref="Runtime.Driver"/>
    /// </para>
    /// <para>
    /// Your implementation should probably provide its own thread to run this
    /// method as the <see cref="ILiveTimingApp.Run"/> method will have blocked.
    /// However you could potentially use an event driven model for this to hang
    /// off of an application loop as processing time is quite low latency.
    /// </para>
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// The runtime will tell us how often we need to be polling for
        /// new data to the live timing server.
        /// </summary>
        /// <param name="refreshRate">The interval in seconds</param>
        void SetRefresh(int refreshRate);

        /// <summary>
        /// Runtime has decoded a message which indicates the end of the
        /// stream, so we should exit.
        /// </summary>
        void Terminate();
        
        /// <summary>
        /// Updates the driver with the latest known keyframe number.
        /// </summary>
        /// <param name="currentFrame"></param>
        void UpdateCurrentKeyFrame(int currentFrame);
    }
}

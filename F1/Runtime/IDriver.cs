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
    /// Implement this interface when driving the Runtime if you want
    /// receive callbacks from the Runtime.
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// The runtime will tell us how often we need to be polling for
        /// new data to the live timing server.
        /// </summary>
        /// <param name="refreshRate">The inteval in seconds</param>
        void SetRefresh(int refreshRate);

        /// <summary>
        /// Runtime has decoded a message which indicates the end of the
        /// stream, so we should exit.
        /// </summary>
        void Terminate();
        
        /// <summary>
        /// Updates the driver with the latest known frame number.
        /// </summary>
        /// <param name="currentFrame"></param>
        void UpdateCurrentFrame(int currentFrame);
    }
}

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

using F1.Messages.System;

namespace F1.Runtime
{
    /// <summary>
    /// Authorization key services implement this.
    /// </summary>
    public interface IAuthKey
    {
        /// <summary>
        /// Based on the specified session, ask for a key used to decrypt the
        /// stream.
        /// </summary>
        /// <param name="session">The session indicator, see <see cref="EventId.SessionId"/></param>
        /// <returns>The decryption key or 0.</returns>
        uint GetKey(string session);
    }
}

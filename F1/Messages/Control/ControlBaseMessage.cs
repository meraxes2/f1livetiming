/*
 *  f1livetiming - Part of the Live Timing Library for .NET
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
using F1.Enums;

namespace F1.Messages.Control
{
    public class ControlBaseMessage : IControlMessage
    {
        public ControlType ControlType { get; protected set;}

        public Data.Packets.Packet CreatePacketType(Data.Packets.Header header, global::System.IO.Stream rawStream, global::System.IO.Stream decryptStream)
        {
            throw new NotImplementedException();
        }

        public void Deserialise(Data.Packets.Header header, Data.Packets.Packet payload)
        {
            throw new NotImplementedException();
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.ControlType; }
        }
    }
}

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

using System.IO;
using F1.Data.Packets;
using F1.Enums;

namespace F1.Messages.System
{
    /// <summary>
    /// Time elapsed of the current event.
    /// </summary>
    public class TimeStamp : IMessage
    {
        /// <summary>
        /// The time in seconds of the amount of time elapsed in this event.
        /// </summary>
        public int Time { get; private set; }

        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new SimplePacket(2, decryptStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            SimplePacket sp = (SimplePacket) payload;

            Time = sp.Data[1] << 8 & 0xff00 | (sp.Data[0] & 0xff | header.Datum << 16 & 0xff0000);
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.Timestamp; }
        }

        #endregion

        public override string ToString()
        {
            return "SystemMessage: Timestamp - Time: " + Time;
        }
    }
}
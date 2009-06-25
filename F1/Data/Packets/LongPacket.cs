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

namespace F1.Data.Packets
{
    /// <summary>
    /// This defines the 'long packet' types, where we can receive up to 128
    /// bytes of data after our header which represents the message. The
    /// <see cref="Header.Datum"/> property contains this value.
    /// </summary>
    public class LongPacket : Packet
    {
        public LongPacket(Header header, Stream input)
            : base(input)
        {
            int longPacketSize = header.Datum;

            if (longPacketSize > 0 && longPacketSize < 128)
            {
                AcquirePayload(longPacketSize);
            }
        }

        /// <summary>
        /// The data received in this Packet, up to 128 bytes. See <see cref="Packet.IsComplete"/>.
        /// </summary>
        public byte[] Data { get { return Payload; } }
    }
}
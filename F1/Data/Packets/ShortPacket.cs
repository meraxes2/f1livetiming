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
    /// A Packet type which has two data portions. One is decoded
    /// from the Datum member of the Header and the other is downloaded.
    /// This means we have a maximum data size of 2^4 bytes. The short
    /// datum can contain a value between 0 and 2^3.
    /// </summary>
    public class ShortPacket : Packet
    {
        private const int NO_DATA = 0x0f;

        public ShortPacket(Header header, Stream input)
            : base(input)
        {
            /*  6 5 4 3 2 1 0
             * [ | | | | | | ]
             *  +-len-+ +-d-+
             */

            int size = (header.Datum >> 3);

            if (size != NO_DATA && size > 0)
            {
                //  Instruct the Packet to begin the data read
                AcquirePayload(size);
            }

            ShortDatum = (header.Datum & 0x07);
        }

        /// <summary>
        /// Long datum
        /// </summary>
        public byte[] Data { get { return Payload; } }

        /// <summary>
        /// Short datum.
        /// </summary>
        public int ShortDatum { get; private set; }
    }
}
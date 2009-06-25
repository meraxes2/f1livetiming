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
using System.IO;
using F1.Enums;

namespace F1.Data.Packets
{
    /// <summary>
    /// <para>Defines the basic header information type for every message received
    /// in the streams. These two bytes will always preceed and sometimes be
    /// a complete message.</para>
    /// <para>
    /// Messages have the following format:
    /// <c>
    /// _byte2           _byte1
    /// [0|1|2|3|4|5|6|7][0|1|2|3|4|5|6|7]
    ///  +-data------+ +-type-+ +-carid-+
    /// </c></para>
    /// </summary>
    public class Header : Packet
    {
        private const ushort SYSTEM_MESSAGE_CARID = 0;


        /// <summary>
        /// Construct from a stream to read 2 bytes. See <see cref="Packet.IsComplete"/> for
        /// true before interrogating the properties of this Packet.
        /// </summary>
        public Header( Stream input )
            : base(input)
        {
            AcquirePayload(2);
        }


        /// <summary>
        /// This is a car message if this value is not 0
        /// </summary>
        public int CarId
        {
            get
            {
                return (Payload[0] & 0x1F);
            }
        }

        
        /// <summary>
        /// Returns the bits an integer form used to represent the message or car type.
        /// </summary>
        public int RawType
        {
            get
            {
                return ((Payload[0] >> 5) | ((Payload[1] & 0x01) << 3));
            }
        }


        /// <summary>
        /// Returns the datum portion of the header which has a number of meanings depending
        /// on the packet type.
        /// </summary>
        public int Datum
        {
            get
            {
                return (Payload[1] >> 1);
            }
        }


        /// <summary>
        /// Utility property to detect for System messages
        /// </summary>
        public bool IsSystemMessage
        {
            get
            {
                return SYSTEM_MESSAGE_CARID == CarId;
            }
        }

        /// <summary>
        /// Opposite of IsSystemMessage.
        /// </summary>
        public bool IsCarMessage
        {
            get
            {
                return !IsSystemMessage;
            }
        }


        /// <summary>
        /// Helper to convert the RawType into a SystemPacketType enumeration.
        /// </summary>
        public SystemPacketType SystemType
        {
            get
            {
                if(IsSystemMessage)
                {
                    return (SystemPacketType) RawType;
                }

                throw new InvalidCastException("Not a System Packet Type.");
            }
        }


        /// <summary>
        /// Helper to convert the RawType into a CarType enumeraiton.
        /// </summary>
        public CarType CarType
        {
            get
            {
                if(IsCarMessage)
                {
                    return (CarType) RawType;
                }

                throw new InvalidCastException("Not a Car Packet Type.");
            }
        }
    }
}
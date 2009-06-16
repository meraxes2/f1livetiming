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
    public class Header : Packet
    {
        private const ushort SYSTEM_MESSAGE_CARID = 0;

        /*
        _b2              _b1
        [ | | | | | | | ][ | | | | | | | ]
         +-data------+ +-type-+ +-carid-+
        */

        public Header( Stream input )
            : base(input)
        {
            AcquirePayload(2);
        }


        public int CarId
        {
            get
            {
                return (Payload[0] & 0x1F);
            }
        }

        
        public int RawType
        {
            get
            {
                return ((Payload[0] >> 5) | ((Payload[1] & 0x01) << 3));
            }
        }


        public int Datum
        {
            get
            {
                return (Payload[1] >> 1);
            }
        }


        public bool IsSystemMessage
        {
            get
            {
                return SYSTEM_MESSAGE_CARID == CarId;
            }
        }


        public bool IsCarMessage
        {
            get
            {
                return !IsSystemMessage;
            }
        }


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
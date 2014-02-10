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
using Common.Utils.Strings;
using F1.Data.Packets;
using F1.Enums;
using F1.Exceptions;
using System.Globalization;

namespace F1.Messages.System
{
    /// <summary>
    /// The current status (represented by a Colour) of this event.
    /// </summary>
    public class TrackStatus : IMessage
    {
        /// <summary>
        /// The current status
        /// </summary>
        public enum Colour
        {
            /// <summary>
            /// All good.
            /// </summary>
            Green = 0,
            
            /// <summary>
            /// Yellow flag.
            /// </summary>
            Yellow = 1,

            /// <summary>
            /// Red flagged.
            /// </summary>
            Red = 2
        } ;

        /// <summary>
        /// The current status of this event.
        /// </summary>
        public Colour Status { get; private set; }

        /// <summary>
        /// Associated message with the current status of this event. Usefulness = unknown.
        /// </summary>
        public string Message { get; private set; }

        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new ShortPacket(header, decryptStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            ShortPacket sp = (ShortPacket)payload;

            if (sp.ShortDatum == 1)
            {
                int status = int.Parse(StringUtils.ASCIIBytesToString(sp.Data), CultureInfo.InvariantCulture);
                switch (status)
                {
                    case 1:
                        Status = Colour.Green;
                        break;
                    case 2:
                        Status = Colour.Yellow;
                        break;
                    case 3:
                        Status = Colour.Yellow;
                        Message = "SCS";
                        break;
                    case 4:
                        Status = Colour.Yellow;
                        Message = "SCD";
                        break;
                    case 5:
                        Status = Colour.Red;
                        break;
                    default:
                        throw new DeserialiseException("Unknown track status, status: " + status);
                }
            }
            else
            {
                throw new DeserialiseException("Unknown track status, short: " + sp.ShortDatum + ", data: " + HexString.BytesToHex(sp.Data));
            }
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.TrackStatus; }
        }

        #endregion

        public override string ToString()
        {
            return "SystemMessage: TrackStatus - Status: " + Status + ", Message: " + Message;
        }
    }
}
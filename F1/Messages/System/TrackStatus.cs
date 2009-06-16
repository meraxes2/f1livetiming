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
using System.Text;
using F1.Data.Packets;
using F1.Enums;
using F1.Exceptions;

namespace F1.Messages.System
{
    public class TrackStatus : IMessage
    {
        public enum Colour
        {
            Green = 0,
            Yellow = 1,
            Red = 2
        } ;

        public Colour Status { get; private set; }
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
                int status = int.Parse(Encoding.ASCII.GetString(sp.Data));
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
                throw new DeserialiseException("Unknown track status, short: " + sp.ShortDatum);
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
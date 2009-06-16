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
using F1.Enums;
using F1.Data.Packets;

namespace F1.Messages.System
{
    public class EventId : IMessage
    {
        public string SessionId { get; private set; }

        public EventType EventType { get; private set; }
        
        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new ShortPacket(header, rawStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            ShortPacket sp = (ShortPacket)payload;

            SessionId = Encoding.ASCII.GetString(sp.Data, 1, sp.Data.Length - 1);
            EventType = (EventType)sp.ShortDatum;
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.EventId; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("SystemMessage: EventId - EventType: {0}, SessionId: {1}", EventType, SessionId);
        }
    }
}
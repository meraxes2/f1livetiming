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
using F1.Enums;
using F1.Data.Packets;

namespace F1.Messages.System
{
    /// <summary>
    /// Generally received at the beginning of a session to indicate it's type (Race, Qualifying,
    /// Practice or None). As well as a unique identifier for this race. You will notice that this
    /// SessionId is used by authentication (see <see cref="F1.Simulator.AuthorizationKey"/>) to
    /// request a relevant decryption key. For none events, encryption is switched off, and this
    /// string therefore contains another undefined value.
    /// </summary>
    public class EventId : IMessage
    {
        /// <summary>
        /// 4 digit session Id or undefined value for none events.
        /// </summary>
        public string SessionId { get; private set; }

        /// <summary>
        /// The type of event (Race, Qualy, Practice or None). See <see cref="EventType"/>.
        /// </summary>
        public EventType EventType { get; private set; }
        
        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new ShortPacket(header, rawStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            ShortPacket sp = (ShortPacket)payload;

            SessionId = StringUtils.ASCIIBytesToString(sp.Data, 1, sp.Data.Length - 1);
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
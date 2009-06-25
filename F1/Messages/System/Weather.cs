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
using Common.Utils.Strings;
using F1.Enums;

namespace F1.Messages.System
{
    /// <summary>
    /// Weather data, not yet completed, do not use.
    /// </summary>
    public class Weather : IMessage
    {
        private byte[] _unkData;
        private int _unkShortDatum;

        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new ShortPacket(header, decryptStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            ShortPacket sp = (ShortPacket)payload;

            _unkData = sp.Data;
            _unkShortDatum = sp.ShortDatum;
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.Weather; }
        }

        #endregion

        public override string ToString()
        {
            return "SystemMessage: Weather - Unknown short: " + _unkShortDatum + ", Unknown data: " + HexString.BytesToHex(_unkData);
        }
    }
}
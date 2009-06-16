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
using System.Diagnostics;
using F1.Enums;
using F1.Data.Packets;

namespace F1.Messages.System
{
    public class KeyFrame : IMessage
    {
        public int FrameNumber { get; private set; }

        private int _unkShortDatum;
    
        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new ShortPacket(header, rawStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            ShortPacket sp = (ShortPacket)payload;

            Debug.Assert(sp.Data.Length == 2);

            FrameNumber = (sp.Data[1] << 8 & 0xff00) | (sp.Data[0] & 0xff);

            _unkShortDatum = sp.ShortDatum;
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.KeyFrame; }
        }

        #endregion

        public override string ToString()
        {
            return "SystemMessage: KeyFrame - FrameNumber: " + FrameNumber + ", Unknown Short: " + _unkShortDatum;
        }
    }
}
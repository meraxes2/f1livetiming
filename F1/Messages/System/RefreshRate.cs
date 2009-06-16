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
using F1.Enums;

namespace F1.Messages.System
{
    /// <summary>
    /// This is communicated from the server to instruct us how often to poll for data. The
    /// default is 1 second, and this will typically reflect that with a value of 1. However
    /// occasionally we get 60, presumably to reduce the traffic to the server in quiet
    /// times. Also 0 indicates the end of the stream and we can disconnect.
    /// </summary>
    public class RefreshRate : IMessage
    {
        public int Rate { get; private set; }

        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return null;
        }

        public void Deserialise(Header header, Packet payload)
        {
            Rate = header.Datum;
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.RefreshRate; }
        }

        #endregion


        public override string ToString()
        {
            return "SystemMessage: RefreshRate - RefreshRate: " + Rate + "secs";
        }
    }
}
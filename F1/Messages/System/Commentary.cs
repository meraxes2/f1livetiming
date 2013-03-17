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
using System;

namespace F1.Messages.System
{
    /// <summary>
    /// Free text message delivered from the server to provide commentary to the event.
    /// </summary>
    public class Commentary : IMessage
    {
        /// <summary>
        /// The next line of commentary. Words and sentances are often split between
        /// commentary which make their usage limited unless you just print the values.
        /// </summary>
        public string Message { get; private set; }
        public bool EndMessage { get; private set; }

        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new LongPacket(header, decryptStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            LongPacket sp = (LongPacket)payload;

            if( sp.Data[0] >= 32 )
            {
                // old style commentary message (i.e. no magic 2 bytes)
                Encoding oldEncoding = Encoding.GetEncoding("ISO-8859-1");
                Message = oldEncoding.GetString(sp.Data, 0, sp.Data.Length);
            }
            else if( (sp.Data[1] & 0x02) == 0x02 )
            {
                // Unicode string
                Message = Encoding.Unicode.GetString(sp.Data, 2, sp.Data.Length - 2);
            }
            else
            {
                // Not unicode, but new UTF8 style of message.
                Message = Encoding.UTF8.GetString(sp.Data, 2, sp.Data.Length - 2);
            }

            EndMessage = ((int)sp.Data[1] > 0);
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.Commentary; }
        }

        #endregion

        public override string ToString()
        {
            return String.Format("SystemMessage: Commentary - EndMessage: {0}, Message: {1}", EndMessage, Message);
        }
    }
}
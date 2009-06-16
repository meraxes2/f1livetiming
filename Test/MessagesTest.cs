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
using F1.Data;
using F1.Data.Packets;
using F1.Enums;
using F1.Messages;
using F1.Messages.System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Properties;
using System;

namespace Test
{
    /// <summary>
    /// Summary description for MessagesTest
    /// </summary>
    [TestClass]
    public class MessagesTest : UnitTestBase
    {
        private Stream Input { get; set; }

        public override void OnSetup()
        {
            Input = new MemoryStream(Resources.bahrainholding);
        }

        public override void OnTeardown()
        {
            Input.Close();
            Input.Dispose();
            Input = null;
        }

        [TestMethod]
        public void TestAllMessages()
        {
            while (Input.Position < (Input.Length - 1))
            {
                Header h = new Header(Input);
                switch (h.SystemType)
                {
                    case SystemPacketType.EventId:
                        TestEventId(h);
                        break;
                    case SystemPacketType.Copyright:
                        TestCopyright(h);
                        break;
                    case SystemPacketType.Notice:
                        TestNotice(h);
                        break;
                    case SystemPacketType.Unknown1:
                        TestUnknown1(h);
                        break;
                    case SystemPacketType.RefreshRate:
                        TestUnknown2(h);
                        break;
                    case SystemPacketType.KeyFrame:
                        TestKeyFrame(h);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        #region Event Specific Tests
        private void TestEventId( Header h )
        {
            EventId msg = new EventId();
            msg.Deserialise(h, msg.CreatePacketType(h, Input, Input));

            Assert.AreEqual("_09041905", msg.SessionId);
            Assert.AreEqual(EventType.NoEvent, msg.EventType);
        }

        private void TestCopyright(Header h)
        {
            Copyright msg = new Copyright();
            msg.Deserialise(h, msg.CreatePacketType(h, Input, Input));

            Assert.AreEqual("Copyright 2009 Formula One Administration Ltd.", msg.Message);
        }


        private void TestNotice(Header h)
        {
            Notice msg = new Notice();
            msg.Deserialise(h, msg.CreatePacketType(h, Input, Input));

            Assert.AreEqual("First Timed Session for 2009 FORMULA 1 GULF AIR BAHRAIN GRAND PRIX in Sakhir will be Friday Practice 1", msg.Message);
        }

        private void TestUnknown1(Header h)
        {
            Unknown1 msg = new Unknown1();

            msg.Deserialise(h, msg.CreatePacketType(h, Input, Input));
        }

        private void TestUnknown2(Header h)
        {
            RefreshRate msg = new RefreshRate();

            msg.Deserialise(h, msg.CreatePacketType(h, Input, Input));
        }

        private void TestKeyFrame(Header h)
        {
            KeyFrame msg = new KeyFrame();

            msg.Deserialise(h, msg.CreatePacketType(h, Input, Input));

            Assert.AreEqual(2, msg.FrameNumber);

        }
        #endregion

        [TestMethod]
        public void TestMessageConsumption()
        {
            PacketReader r = new PacketReader(Input, Input);
            while(true)
            {
                if (!r.ReadNext())
                {
                    break;
                }
            }

            Type[] expectedMessages = new[] { typeof(EventId), typeof(Copyright), typeof(Notice), typeof(RefreshRate), typeof(Unknown1), typeof(Unknown1), typeof(KeyFrame) };

            IMessage[] extractedMessages = r.MessageQueue.ToArray();

            for (int i = 0; i < expectedMessages.Length; ++i)
            {
                Assert.AreEqual(expectedMessages[i], extractedMessages[i].GetType());
            }
        }
    }
}

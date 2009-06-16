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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Properties;
using System;

namespace Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PacketTests : UnitTestBase
    {
        private DelayedMemoryStream Input { get; set; }

        class DelayedMemoryStream : MemoryStream
        {
            public bool Delay { private get; set;  }

            public DelayedMemoryStream(byte[] buffer)
                : base(buffer)
            {
                
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if(Delay)
                {
                    return base.Read(buffer, offset, count/2 + count%2);
                }
                
                return base.Read(buffer, offset, count);
            }
        }

        class PacketTest : Packet
        {
            public PacketTest(Stream input)
                : base(input)
            {
                AcquirePayload(10);
            }

            public byte[] Data { get { return Payload; } }
        }

        
        public override void OnSetup()
        {
            Input = new DelayedMemoryStream(Resources.bahrainholding);
        }

        public override void OnTeardown()
        {
            Input.Close();
            Input.Dispose();
            Input = null;
        }


        [TestMethod]
        public void TestPacketPartReading()
        {
            Input.Delay = true;

            PacketTest p = new PacketTest(Input);

            Assert.IsFalse(p.IsComplete);

            Input.Delay = false;

            p.ContinueDataRead();

            Assert.IsTrue(p.IsComplete);

            byte[] testData = new byte[] { 0x20, 0xA0, 0x02, 0x5f, 0x30, 0x39, 0x30, 0x34, 0x31, 0x39 };

            Assert.AreEqual(10, p.Data.Length);

            for (int i = 0; i < 10; ++i)
            {
                Assert.AreEqual(testData[i], p.Data[i]);
            }
        }

        [TestMethod]
        public void TestHeaderRead()
        {
            Header h = new Header(Input);

            // Header = 0xA0      0x20
            //        = 1010 0000 0010 0000

            Assert.IsTrue(h.IsComplete);
            Assert.IsTrue(h.IsSystemMessage);
            Assert.AreEqual(0, h.CarId);
            Assert.AreEqual(1, h.RawType);
            Assert.AreEqual(SystemPacketType.EventId, h.SystemType);
        }


        [TestMethod]
        public void TestShortPacket()
        {
            Header h = new Header(Input);

            ShortPacket sp = new ShortPacket(h, Input);

            Assert.IsTrue(sp.IsComplete);
            Assert.AreEqual(0, sp.ShortDatum);

            byte[] testData = new byte[] { 0x02, 0x5f, 0x30, 0x39, 0x30, 0x34, 0x31, 0x39, 0x30, 0x35 };

            Assert.AreEqual(10, sp.Data.Length);

            for( int i = 0; i < 10; ++i )
            {
                Assert.AreEqual(testData[i], sp.Data[i]);
            }
        }


        [TestMethod]
        public void TestLongPacket()
        {
            Input.Seek(12, SeekOrigin.Begin);

            Header h = new Header(Input);

            LongPacket lp = new LongPacket(h, Input);

            Assert.IsTrue(lp.IsComplete);
            Assert.AreEqual(46, lp.Data.Length);

            byte[] testData = new byte[] {0x43, 0x6f, 0x70, 0x79, 0x72, 0x69, 0x67, 0x68, 0x74, 0x20};

            for (int i = 0; i < 10; ++i)
            {
                Assert.AreEqual(testData[i], lp.Data[i]);
            }
        }


        [TestMethod]
        public void TestPacketConsumption()
        {
            // This just tests the packet reader's ability to
            // munch through all the packets in the stream, and
            // gracefully reach the end without exception.
            // Therefore it will return true, and we will keep
            // calling into it, until it returns false. At
            // which point it's assumed we've reached the end
            // of the stream. If not something went wrong.

            PacketReader r = new PacketReader(Input, Input);
            while(Input.Position < (Input.Length - 1))
            {
                if (!r.ReadNext())
                {
                    // Have we reached the end, if not
                    // this is an error.
                    Assert.IsTrue(Input.Position == (Input.Length - 1));
                }
            }
        }
    }
}

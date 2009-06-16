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

using F1.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using F1.Data;
using F1.Data.Encryption;

namespace Test
{
    [TestClass]
    public class KeyFrameTest : UnitTestBase
    {
        private static IKeyFrame ProKeyFrame
        {
            get
            {
                return new F1.Protocol.KeyFrame();
            }
        }


        private static IKeyFrame SimKeyFrame
        {
            get
            {
                return new F1.Simulator.KeyFrame(@"..\..\..\Data\Barcelona\Race");
            }
        }


        [TestMethod]
        public void TestGetKeyFrame()
        {
            Stream keyFrameData = ProKeyFrame.GetKeyFrame();

            Assert.AreNotEqual(null, keyFrameData);
            Assert.AreNotEqual(0, keyFrameData.Length);

            //TestMessageConsumption(keyFrameData);
        }


        [TestMethod]
        public void TestSimKeyFrame()
        {
            Stream keyFrameData = SimKeyFrame.GetKeyFrame();

            Assert.AreNotEqual(null, keyFrameData);
            Assert.AreNotEqual(0, keyFrameData.Length);

            TestMessageConsumption(keyFrameData);
        }


        [TestMethod]
        public void TestSimKeyFrames()
        {
            Stream keyFrameData = SimKeyFrame.GetKeyFrame(100);

            Assert.AreNotEqual(null, keyFrameData);
            Assert.AreNotEqual(0, keyFrameData.Length);

            TestMessageConsumption(keyFrameData);
        }

        
        private static void TestMessageConsumption(Stream input)
        {
            Stream decryptedInput = new DecryptStreamDecorator(input, new DataDecryptor { Key = 0xf8ffd6db });

            PacketReader r = new PacketReader(input, decryptedInput);
            while (true)
            {
                if (!r.ReadNext())
                {
                    break;
                }
            }

            Assert.AreEqual(input.Length, input.Position);
        }
    }
}

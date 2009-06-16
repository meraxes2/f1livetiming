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
using F1.Data.Encryption;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Properties;

namespace Test
{
    /// <summary>
    /// Summary description for TestMultipleScenarios
    /// </summary>
    [TestClass]
    public class TestMultipleScenarios : UnitTestBase
    {
        private Stream Input { get; set; }
        private Stream DecryptedInput { get; set; }
        private DataDecryptor Decryptor { get; set; }

        public override void OnSetup()
        {
            Decryptor = new DataDecryptor();
        }


        public override void OnTeardown()
        {
            Input.Close();
            Input.Dispose();
            Input = null;
        }


        [TestMethod]
        public void TestBahrainHolding()
        {
            Input = new MemoryStream(Resources.bahrainholding);
            DecryptedInput = Input;

            TestMessageConsumption();
        }


        [TestMethod]
        public void TestBahrain2ndPractice()
        {
            Decryptor.Key = 0xa2a5205c;

            Input = new MemoryStream(Resources.bahrainq2);
            DecryptedInput = new DecryptStreamDecorator(Input, Decryptor);

            TestMessageConsumption();
        }


        [TestMethod]
        public void TestBarcelonaTest2()
        {
            Decryptor.Key = 0xb9ca62be;

            Input = new MemoryStream(Resources.barcelonatest);
            DecryptedInput = new DecryptStreamDecorator(Input, Decryptor);

            TestMessageConsumption();
        }


        [TestMethod]
        public void TestBarcelonaRace()
        {
            Decryptor.Key = 0xf8ffd6db;

            Input = new MemoryStream(Resources.barcelonarace);
            DecryptedInput = new DecryptStreamDecorator(Input, Decryptor);

            TestMessageConsumption();
        }


        [TestMethod]
        public void TestMonacoQ3()
        {
            Decryptor.Key = 0xdc8af5ee;

            Input = new MemoryStream(Resources.monacoq3);
            DecryptedInput = new DecryptStreamDecorator(Input, Decryptor);

            TestMessageConsumption();
        }
        

        public void TestMessageConsumption()
        {
            PacketReader r = new PacketReader(Input, DecryptedInput);
            while (true)
            {
                if (!r.ReadNext())
                {
                    break;
                }
            }

            // This means we have sucesfully navigated our way to the end and understood
            // all the packets along the way ... yipee!
            Assert.AreEqual( Input.Length, Input.Position );
        }
    }
}

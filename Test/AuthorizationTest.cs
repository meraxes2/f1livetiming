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

using F1.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class AuthorizationTest : UnitTestBase
    {
        [TestMethod]
        public void TestGetAuthKey()
        {
            const string username = "Enter your username here";
            const string password = "Enter your password here";

            AuthorizationKey authKey = new AuthorizationKey(username, password);

            {
                const string session = "6630"; // 0xF3C3476F

                uint key = authKey.GetKey(session);

                Assert.AreNotEqual(AuthorizationKey.INVALID_KEY, key);
                Assert.AreEqual((uint)0xF3C3476F, key);
            }

            {
                const string session = "6629"; // 0xE5BCF17C

                uint key = authKey.GetKey(session);

                Assert.AreNotEqual(AuthorizationKey.INVALID_KEY, key);
                Assert.AreEqual((uint)0xE5BCF17C, key);
            }

            {
                const string session = "6628"; // 0xcf0a3056

                uint key = authKey.GetKey(session);

                Assert.AreNotEqual(AuthorizationKey.INVALID_KEY, key);
                Assert.AreEqual((uint)0xcf0a3056, key);
            }

            {
                const string session = "6632"; // 0xdc8af5ee

                uint key = authKey.GetKey(session);

                Assert.AreNotEqual(AuthorizationKey.INVALID_KEY, key);
                Assert.AreEqual((uint)0xdc8af5ee, key);
            }

            {
                const string session = "_040109"; // 0xcf0a3056

                uint key = authKey.GetKey(session);

                Assert.AreNotEqual(AuthorizationKey.INVALID_KEY, key);
                Assert.AreEqual((uint)0, key);
            }
        }
    }
}

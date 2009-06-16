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

using System;
using System.IO;
using F1.Messages;
using F1.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class RuntimeTest : UnitTestBase
    {
        private const string LIVEDUMP     = @"..\..\..\Data\Barcelona\Race.dat";
        private const string KEYFRAMEPATH = @"..\..\..\Data\Barcelona\Race";
        private const uint DECRYPT_KEY    = 0xf8ffd6db;


        public override void OnSetup()
        {
            // disable logging for this test because of the volume of data.
            //LogConfiguration.Instance.Disable();
        }


        public override void OnTeardown()
        {
            // re-enable logging.
            //LogConfiguration.Instance.Enable();
        }

        
        [TestMethod]
        public void TestRuntime()
        {
            Stream liveStream = File.OpenRead(LIVEDUMP);
            IAuthKey authKey = new F1.Simulator.AuthorizationKey(DECRYPT_KEY);
            IKeyFrame keyFrame = new F1.Simulator.KeyFrame(KEYFRAMEPATH);

            Runtime r = new Runtime(liveStream, authKey, keyFrame, new NullMessageDispatcher())
                            {
                                Driver = new NullDriver()
                            };

            while(r.HandleRead())
            {
                
            }
        }


        private class NullDriver : IDriver
        {
            #region Implementation of IDriver

            public void SetRefresh(int refreshRate)
            {
            }

            public void Terminate()
            {
            }

            public void UpdateCurrentFrame(int k)
            {
            }

            #endregion
        }


        private class NullMessageDispatcher : IMessageDispatch
        {
            public void Dispatch(IMessage msg)
            {
            }
        }
    }
}

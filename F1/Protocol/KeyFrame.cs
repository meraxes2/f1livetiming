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
using System.Net;
using F1.Exceptions;
using F1.Runtime;
using Common;

namespace F1.Protocol
{
    /// <summary>
    /// Provides keyframes downloaded from a webserver that is present during a live session.
    /// </summary>
    public class KeyFrame : IKeyFrame
    {
        private const string NUMBERED_URL = "http://live-timing.formula1.com/keyframe_{0}.bin";
        private const string LATEST_URL = "http://live-timing.formula1.com/keyframe.bin";
        private const int READ_CHUNK_SIZE = 1024;
        private const int ESTIMATED_SIZE = 4096;

        public Stream GetKeyFrame()
        {
            return GetKeyFrame(LATEST_URL);
        }

        public Stream GetKeyFrame(int frameNumber)
        {
            return GetKeyFrame(string.Format(NUMBERED_URL, frameNumber.ToString("D5")));
        }


        private static Stream GetKeyFrame(string url)
        {
            HttpWebAdaptor req = new HttpWebAdaptor(WebRequest.Create(url) as HttpWebRequest);

            if(null == req)
            {
                throw new KeyFrameException("Could not create request for url: " + url, null);
            }

            //if (null != req.Proxy)
            //{
            //    req.Proxy.Credentials = CredentialCache.DefaultCredentials;
            //}

            HttpWebResponse resp1 = req.GetResponse() as HttpWebResponse;

            if( null == resp1 )
            {
                throw new KeyFrameException("Error retrieving KeyFrame", null);
            }

            MemoryStream ms = new MemoryStream(ESTIMATED_SIZE);
            using(Stream s = resp1.GetResponseStream())
            {
                int read;
                byte[] next = new byte[READ_CHUNK_SIZE];
                do
                {
                    read = s.Read(next, 0, READ_CHUNK_SIZE);
                    if( read > 0 )
                    {
                        ms.Write(next, 0, read);
                    }
                } while (read > 0);
            }

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}

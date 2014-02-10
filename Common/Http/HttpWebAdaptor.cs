/*
 *  f1livetiming - Part of the Live Timing Library for .NET
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
using System.Net;
using System.IO;
using System.Threading;

namespace Common
{
    public class HttpWebAdaptor
    {
        private HttpWebRequest _request;
        private AutoResetEvent _requestDone = new AutoResetEvent(false);

        public HttpWebAdaptor(HttpWebRequest request)
        {
            _request = request;
        }

        public HttpWebRequest Request
        {
            get
            {
                return _request;
            }
        }

        public Stream GetRequestStream()
        {
            try
            {
                IAsyncResult asyncRes = _request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), null);
                _requestDone.WaitOne();

                return _request.EndGetRequestStream(asyncRes);
            }
            catch (System.Exception)
            {
            }

            return null;

        }

        private void GetRequestStreamCallback(IAsyncResult asyncRes)
        {
            _requestDone.Set();
        }

        public WebResponse GetResponse()
        {
            IAsyncResult asyncRes;

            try
            {
                asyncRes = _request.BeginGetResponse(new AsyncCallback(GetResponseCallback), null);
                _requestDone.WaitOne();

                return _request.EndGetResponse(asyncRes);
            }
            catch (System.Exception)
            {
            }

            return null;
        }

        private void GetResponseCallback(IAsyncResult asyncRes)
        {
            _requestDone.Set();
        }
    }
}

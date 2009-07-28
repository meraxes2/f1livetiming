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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Globalization;
using Common.Utils.Strings;
using F1.Runtime;
using F1.Exceptions;

namespace F1.Protocol
{
    /// <summary>
    /// Connects to the formula1.com secure servers to authenticate the username and password.
    /// </summary>
    public class AuthorizationKey : IAuthKey
    {
        public const uint INVALID_KEY = 0xFFFFFFFF;

        private readonly string _loginCookie;

        private readonly Dictionary<string, uint> _cachedKeys = new Dictionary<string, uint>();

        public AuthorizationKey(string user, string pass)
        {
            try
            {
                _loginCookie = Login(user, pass);
            }
            catch(WebException e)
            {
                throw new AuthorizationException(e.Message, e);
            }
        }


        public uint GetKey(string session)
        {
            if(_cachedKeys.ContainsKey(session))
            {
                return _cachedKeys[session];
            }

            uint newKey = TryGetKey(_loginCookie, session);

            if (INVALID_KEY != newKey)
            {
                _cachedKeys[session] = newKey;
            }

            return newKey;
        }


        #region Static Helpers
        private static string Login(string user, string pass)
        {
            string ret = TryGetCookie(user, pass, "live-timing.formula1.com");

            if( String.IsNullOrEmpty(ret) )
            {
                ret = TryGetCookie(user, pass, "secure.formula1.com");
            }

            return ret;
        }


        private static string TryGetCookie(string user, string pass, string host)
        {
            string body = string.Format("email={0}&password={1}", user, pass);
            byte[] bodyData = StringUtils.StringToASCIIBytes(body);

            HttpWebRequest req = WebRequest.Create("http://" + host + "/reg/login.asp") as HttpWebRequest;

            if (null != req.Proxy)
            {
                req.Proxy.Credentials = CredentialCache.DefaultCredentials;
            }

            req.AllowAutoRedirect = false;
            req.Method = "Post";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bodyData.Length;

            using (Stream reqBody = req.GetRequestStream())
            {
                reqBody.Write(bodyData, 0, bodyData.Length);
                reqBody.Close();
            }

            HttpWebResponse resp1 = req.GetResponse() as HttpWebResponse;

            string cookie = resp1.Headers["Set-Cookie"];

            if( string.IsNullOrEmpty(cookie))
            {
                throw new AuthorizationException("Incorrect login credentials", null);
            }

            return ParseCookie(cookie);
        }


        private static uint TryGetKey( string cookie, string sessionName )
        {
            string url = String.Format("http://secure.formula1.com/reg/getkey/{0}.asp?auth={1}", sessionName, cookie);
            
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;

            if (null != req.Proxy)
            {
                req.Proxy.Credentials = CredentialCache.DefaultCredentials;
            }

            HttpWebResponse resp1 = req.GetResponse() as HttpWebResponse;

            using(TextReader re = new StreamReader(resp1.GetResponseStream()))
            {
                return ParseKey(re.ReadLine());
            }
        }


        private static string ParseCookie(string header)
        {
            //  This is very risky parsing logic, but it will certainly
            // let us know when we've gone wrong.
            return header.Split(';')[0].Split('=')[1];
        }


        private static uint ParseKey(string key)
        {
            return uint.Parse(key, NumberStyles.HexNumber);
        }
        #endregion
    }
}

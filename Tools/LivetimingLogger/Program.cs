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
using System.Configuration;
using System.IO;
using F1;
using F1.Configuration;
using F1.Messages;
using F1.Exceptions;
using log4net;

namespace LivetimingLogger
{
    /// <summary>
    /// This is a very simple console application to demonstrate creation of
    /// the live timing api, how to use it, and how to receive data messages.
    /// 
    /// Tip: If you look at the ToString() method of every message you'll see
    /// what data is useful and probably what it means.
    /// </summary>
    class Program
    {
        private readonly LiveTiming  _lt;
        private static readonly ILog Log = LogManager.GetLogger("Program");

        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure(); // use app.config
            try
            {
                Program p = new Program();
                p.Run();
            }
            catch (AuthorizationException)
            {
                Log.Error("Failed to authenticate credentials. Exiting.");
            }
            catch (ConnectionException)
            {
                Log.Error("Failed to connect to live timing server. Exiting.");
            }
            catch(ConfigurationErrorsException e)
            {
               Log.Error("There was an reading the config file: " + e.Message);
            }
        }

        public Program()
        {
            Console.CancelKeyPress += ConsoleCancelKeyPress;

            string username = string.Empty, password = string.Empty;

            AuthSection auth = ConfigurationManager.GetSection("authSection") as AuthSection;

            if( auth == null || String.IsNullOrEmpty(auth.UserName) || String.IsNullOrEmpty(auth.Password) )
            {
                const string AUTHPATH = "..\\..\\..\\..\\auth.config";

                // Try the user auth.config file in the sandbox root (this tool is obviously incorrect if not running near the source code)
                if( File.Exists(AUTHPATH) )
                {
                    AuthData d = AuthData.Load(AUTHPATH);
                    if( d != null )
                    {
                        username = d.Username;
                        password = d.Password;
                    }
                }
                else
                {
                    throw new AuthorizationException("Invalid authSection in configuration file", null);    
                }
            }
            else
            {
                username = auth.UserName;
                password = auth.Password;
            }

            _lt = new LiveTiming(username, password, false);

            _lt.CarMessageHandler += MessageHandler;
            _lt.SystemMessageHandler += MessageHandler;
        }


        private static void MessageHandler(IMessage msg)
        {
            //  Log incoming messages
            Log.Info(msg.ToString());
        }

        
        private void Run()
        {
            try
            {
                // block on the main thread until CTRL+C
                _lt.Run();
            }
            finally
            {
                // When the thread stops, we clean up.
                _lt.Dispose();
            }
        }


        private void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Log.Info("Caught ctrl+c, exiting application.");
            _lt.Stop(false);
        }
    }
}

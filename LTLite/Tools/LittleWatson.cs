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

using Microsoft.Phone.Tasks;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace LTLite.Tools
{
    public class LittleWatson
    {
        static string _fileName = "crash.log";
        static string _email = "";
        static string _appName = "";

        public static void Init(string fileName, string email, string appName)
        {
            _fileName = fileName;
            _email = email;
            _appName = appName;
        }

        public static void LogException(Exception ex, string version)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if(store.FileExists(_fileName))
                    {
                        SafeDeleteFile(store, _fileName);
                    }

                    using (TextWriter output = new StreamWriter(store.CreateFile(_fileName)))
                    {
                        output.WriteLine("Version: " + version);
                        output.WriteLine(ex.Message);
                        output.WriteLine(ex.StackTrace);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void CheckForPreviousException()
        {
            try
            {
                string contents = null;

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(_fileName))
                    {
                        using (TextReader reader = new StreamReader(store.OpenFile(_fileName, FileMode.Open, FileAccess.Read, FileShare.None)))
                        {
                            contents = reader.ReadToEnd();
                        }

                        SafeDeleteFile(store, _fileName);
                    }
                }

                if (contents != null)
                {
                    MessageBoxResult result = MessageBox.Show("I'm terribly sorry for the last crash. Would you like to send an email to report it?", 
                                                                "Problem Report", MessageBoxButton.OKCancel);
                    if (MessageBoxResult.OK == result)
                    {
                        EmailComposeTask email = new EmailComposeTask();
                        email.To = _email;
                        email.Subject = _appName + " crash report";
                        email.Body = contents;
                        email.Show();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private static void SafeDeleteFile(IsolatedStorageFile store, string fileName)
        {
            try
            {
                store.DeleteFile(_fileName);
            }
            catch (Exception)
            {
            }
        }
    }
}

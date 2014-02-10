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
using System.IO.IsolatedStorage;
using WP7TilesCommon.Data;
using System.Xml.Serialization;

namespace WP7TilesCommon.Tools
{
    public static class DataSerializer
    {        
        public static bool Serialize(RacingEvent data)
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream("RacingEvent.xml", System.IO.FileMode.Create, store))
                    {
                        XmlSerializer serializer = new XmlSerializer(data.GetType());
                        serializer.Serialize(fs, data);
                        System.Diagnostics.Debug.WriteLine("Iso RacingEvent Serialize complete");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Iso RacingEvent Serialize Exception: " + e.Message);
                return false;
            }

            return true;
        }

        public static bool Deserialize(out RacingEvent data)
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.FileExists("RacingEvent.xml"))
                    {
                        data = new RacingEvent();
                        return false;
                    }

                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream("RacingEvent.xml", System.IO.FileMode.Open, store))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(RacingEvent));
                        data = (RacingEvent)serializer.Deserialize(fs);
                        System.Diagnostics.Debug.WriteLine("Iso RacingEvent Deserialize complete");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Iso RacingEvent Deserialize Exception: " + e.Message);
                data = new RacingEvent();
                return false;
            }

            return true;
        }

        public static void ClearCache()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists("RacingEvent.xml"))
                    {
                        store.DeleteFile("RacingEvent.xml");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ClearCache Exception: " + e.Message);
            }
        }       
    }
}

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

namespace LTStoreApp.Converters
{
    public static class DriverNameConverter
    {
        public static string Shorten(string name)
        {
            string shortName = "";
            string[] parts = name.Split(' ');

            if (parts.Length >= 4)
            {
                //G. VAN DER GARDE -> VDG
                for (int i = 1; i < 4; ++i)
                {
                    if (parts[i].Length > 0)
                    {
                        shortName += parts[i].Substring(0, 1);
                    }
                }
            }
            else if (parts.Length == 3)
            {
                //P. DI RESTA -> DIR
                if (parts[1].Length > 1)
                {
                    shortName = parts[1].Substring(0, 2);
                    if (parts[2].Length > 0)
                    {
                        shortName += parts[2].Substring(0, 1);
                    }
                }
                else if (parts[1].Length == 1)
                {
                    shortName = parts[1];
                    if (parts[2].Length > 1)
                    {
                        shortName += parts[2].Substring(0, 2);
                    }
                }
            }
            else if (parts.Length == 2)
            {
                //F. ALONSO -> ALO
                if (parts[1].Length >= 3)
                {
                    shortName = parts[1].Substring(0, 3);
                }
                else
                {
                    shortName = parts[1];
                }
            }
            else if (parts.Length == 1)
            {
                if (parts[0].Length > 3)
                {
                    shortName = parts[0].Substring(0, 3);
                }
                else
                {
                    return parts[0];
                }
            }

            return shortName;
        }

        //public static void Test()
        //{
        //    string res = DriverNameConverter.Shorten("");
        //    res = DriverNameConverter.Shorten("M"); //M
        //    res = DriverNameConverter.Shorten("MO"); //MO
        //    res = DriverNameConverter.Shorten("MOMBASA"); //MOM
        //    res = DriverNameConverter.Shorten("Q. MA"); //MA
        //    res = DriverNameConverter.Shorten("F. ALONSO"); //ALO
        //    res = DriverNameConverter.Shorten("P. K RUSTER"); //KRU
        //    res = DriverNameConverter.Shorten("P. DI RESTA"); //DIR
        //    res = DriverNameConverter.Shorten("P. KORBA RUSTER"); //KOR
        //    res = DriverNameConverter.Shorten("PANTERA KORBA RUSTER"); //KOR
        //    res = DriverNameConverter.Shorten("G. VAN DER GARDE"); //VDG
        //    res = DriverNameConverter.Shorten("G. VAN DER VART ASD QWE"); //VDV
        //}
    }
}

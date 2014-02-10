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

using System.Collections.Generic;
using System.Text;

namespace Common.Utils.Strings
{
    public class StringUtils
    {
        public static string CommaSeperateArray<T>(IEnumerable<T> input)
        {
            bool first = true;
            StringBuilder builder = new StringBuilder();
            foreach (T b in input)
            {
                if (first)
                {
                    first = false;
                    builder.Append(b.ToString());
                }
                else
                {
                    builder.Append("," + b);
                }
            }
            return builder.ToString();
        }


        public static string ASCIIBytesToString( byte[] data )
        {
            return ASCIIBytesToString(data, 0, data.Length);
        }


        public static string ASCIIBytesToString(byte[] data, int offset, int length)
        {
#if SILVERLIGHT
            return Encoding.UTF8.GetString(data, offset, length);
#else
            return Encoding.ASCII.GetString(data, offset, length);
#endif
        }


        public static byte[] StringToASCIIBytes(string str)
        {
#if SILVERLIGHT
            return Encoding.UTF8.GetBytes(str);
#else
            return Encoding.ASCII.GetBytes(str);
#endif
        }
    }
}

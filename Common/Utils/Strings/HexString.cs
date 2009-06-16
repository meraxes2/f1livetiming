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

using System.Text;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Common.Utils.Strings
{
    public class HexString
    {
        public static string BytesToHex(byte[] input)
        {
            if( null == input )
            {
                return "Null";
            }

            bool first = true;
            StringBuilder builder = new StringBuilder();
            foreach (byte b in input)
            {
                if (first)
                {
                    first = false;
                    builder.Append(b.ToString("x2"));
                }
                else
                {
                    builder.Append(" " + b.ToString("x2"));
                }
            }
            return builder.ToString();
        }


        public static string IntTypeToHex<T>(IEnumerable<T> input) where T : struct, IFormattable
        {
            string fmt = "X" + 2 * Marshal.SizeOf(input.GetEnumerator().Current);

            bool first = true;
            StringBuilder builder = new StringBuilder();
            foreach (T b in input)
            {
                if (first)
                {
                    first = false;
                    builder.Append(b.ToString(fmt, NumberFormatInfo.CurrentInfo));
                }
                else
                {
                    builder.Append(" " + b.ToString(fmt, NumberFormatInfo.CurrentInfo));
                }
            }
            return builder.ToString();
        }


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
    }
}

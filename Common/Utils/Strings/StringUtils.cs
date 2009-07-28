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

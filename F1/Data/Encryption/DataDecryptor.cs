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

namespace F1.Data.Encryption
{
    public class DataDecryptor
    {
        private const uint SEED_INIT = 0x55555555;
        private uint _salt;

        public uint Key { get; set; }

        public DataDecryptor()
        {
            Reset();
        }

        
        public void Reset()
        {
            _salt = SEED_INIT;
        }


        public void DecryptData(byte[] buffer, int offset, int count)
        {
            if (0 == Key)
            {
                return;
            }

            for (int index = offset; index < (count + offset); ++index)
            {
                _salt = ((_salt >> 1) ^ (0x01 == (_salt & 0x01) ? Key : 0));

                buffer[index] ^= (byte)(_salt & 0xff);
            }
        }
    }
}

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

using Microsoft.Phone.Info;

namespace WP7TilesCommon.Tools
{
    public static class DeviceType
    {
        private static readonly long _deviceMem = DeviceStatus.DeviceTotalMemory;
        private static readonly long _wp8LowMemLimit = 512 * 1024 * 1024;
        private static readonly long _wp7LowMemLimit = 256 * 1024 * 1024; 

        public static bool IsWP7LowMemDevice
        {
            get { return _deviceMem <= _wp7LowMemLimit; }
        }

        public static bool IsWP8LowMemDevice
        {
            get { return !IsWP7LowMemDevice && _deviceMem <= _wp8LowMemLimit; }
        }
    }
}

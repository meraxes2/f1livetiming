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

namespace F1.Messages.System
{
    public class WeatherSessionClock : WeatherMessage
    {
        public string TimeStr
        {
            get
            {
                return BaseData;
            }
        }

        public TimeSpan Time { get; private set; }

        protected override void OnDeserialiseComplete()
        {
            double time = DeserialiseTime();

            if (Double.IsNaN(time))
            {
                Time = TimeSpan.Zero;
            }
            else
            {
                Time = TimeSpan.FromSeconds(time);
            }
        }

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherSessionClock - TimeStr: {0}.", TimeStr);
        }
    }
}

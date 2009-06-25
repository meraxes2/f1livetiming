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

namespace F1.Messages.Car
{
    /// <summary>
    /// Latest time for the last complete lap.
    /// </summary>
    public class CarLapTime : CarBaseMessage
    {
        /// <summary>
        /// Latest time for the last complete lap.
        /// </summary>
        public double LapTime { get; private set; }

        /// <summary>
        /// Defines how to interpret the LapTime value, see <see cref="CarBaseMessage.TimeType"/>
        /// </summary>
        public TimeType LapTimeType { get; private set; }

        public override string ToString()
        {
            return "CarBaseMessage: CarLapTime - CarId: " + CarId + ", Colour: " + Colour + ", LapTime: " + PrintTimeValue(LapTime, LapTimeType);
        }

        protected override void OnDeserialiseComplete()
        {
            TimeType type;
            LapTime = DeserialiseLapTimeInfo(out type);
            LapTimeType = type;
        }
    }
}

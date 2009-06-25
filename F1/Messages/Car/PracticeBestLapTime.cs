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
    /// The best time set during a practice lap.
    /// </summary>
    public class PracticeBestLapTime : CarBaseMessage
    {
        /// <summary>
        /// Time in seconds of the best lap.
        /// </summary>
        public double BestLap { get; private set; }

        /// <summary>
        /// Defines the behaviour of BestLap, see <see cref="CarBaseMessage.TimeType"/>
        /// </summary>
        public TimeType BestLapType { get; private set; }

        public override string ToString()
        {
            return "CarBaseMessage: PracticeBestLap - CarId: " + CarId + ", Colour: " + Colour + ", BestLap: " + PrintTimeValue(BestLap, BestLapType);
        }

        protected override void OnDeserialiseComplete()
        {
            TimeType type;
            BestLap = DeserialiseLapTimeInfo(out type);
            BestLapType = type;
        }
    }
}

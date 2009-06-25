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
    /// One which lap the pit will/has occur/occured for the pit number, of which there can be only 3.
    /// </summary>
    public abstract class CarPitLap : CarBaseMessage
    {
        public string PitLap { get; private set; }
        public int PitLapNumber { get; private set; }

        protected CarPitLap(int pitLapNumber)
        {
            PitLapNumber = pitLapNumber;
        }

        public override string ToString()
        {
            return "CarBaseMessage: CarPitLap - CarId: " + CarId + ", Colour: " + Colour + ", Number: " + PitLapNumber + ", PitLap: " + PitLap;
        }

        protected override void OnDeserialiseComplete()
        {
            PitLap = BaseData;
        }
    }


    /// <summary>
    /// The lap on which the first pit will/has happen.
    /// </summary>
    public class CarPitLap1 : CarPitLap
    {
        public CarPitLap1()
            : base(1)
        {
        }
    }

    /// <summary>
    /// The lap on which the second pit will/has happen.
    /// </summary>
    public class CarPitLap2 : CarPitLap
    {
        public CarPitLap2()
            : base(2)
        {
        }
    }

    /// <summary>
    /// The lap on which the third pit will/has happen.
    /// </summary>
    public class CarPitLap3 : CarPitLap
    {
        public CarPitLap3()
            : base(3)
        {
        }
    }
}

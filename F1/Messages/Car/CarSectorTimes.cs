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
    /// Defines the time for a given sector during a race or qualifying.
    /// </summary>
    public abstract class CarSectorTime : CarBaseMessage
    {
        /// <summary>
        /// The sector for which this time was set.
        /// </summary>
        public int Sector { get; private set; }

        /// <summary>
        /// The sector time in seconds, or Nan.
        /// </summary>
        public double SectorTime { get; private set; }

        /// <summary>
        /// Defines the behaviour of SectorTime, see <see cref="CarBaseMessage.TimeType"/>
        /// </summary>
        public TimeType SectorTimeType { get; private set; }


        protected CarSectorTime(int sector)
        {
            Sector = sector;
        }

        public override string ToString()
        {
            return "CarBaseMessage: CarSectorTime - CarId: " + CarId + ", Colour: " + Colour + ", Sector: " + Sector + ", SectorTime: " + PrintTimeValue(SectorTime, SectorTimeType);
        }

        protected override void OnDeserialiseComplete()
        {
            TimeType type;
            SectorTime = DeserialiseLapTimeInfo(out type);
            SectorTimeType = type;
        }
    }


    /// <summary>
    /// Time for sector 1, see <see cref="CarSectorTime"/>
    /// </summary>
    public class CarSectorTime1 : CarSectorTime
    {
        public CarSectorTime1()
            : base(1)
        {
        }
    }


    /// <summary>
    /// Time for sector 2,  see <see cref="CarSectorTime"/>
    /// </summary>
    public class CarSectorTime2 : CarSectorTime
    {
        public CarSectorTime2()
            : base(2)
        {
        }
    }

    /// <summary>
    /// Time for sector 3,  see <see cref="CarSectorTime"/>
    /// </summary>
    public class CarSectorTime3 : CarSectorTime
    {
        public CarSectorTime3()
            : base(3)
        {
        }
    }
}

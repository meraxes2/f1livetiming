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
    /// The time set during a qualifying period. There are 3 qualifying sessions, 
    /// and this defines the time for each.
    /// </summary>
    public abstract class QualifyPeriodTime : CarBaseMessage
    {
        /// <summary>
        /// The qualifying session.
        /// </summary>
        public int Period { get; private set; }

        /// <summary>
        /// The time for this session.
        /// </summary>
        public double PeriodTime { get; private set; }

        /// <summary>
        /// Defines the behaviour of PeriodTime, see <see cref="CarBaseMessage.TimeType"/>
        /// </summary>
        public TimeType PeriodTimeType { get; private set; }


        protected QualifyPeriodTime(int period)
        {
            Period = period;
        }

        public override string ToString()
        {
            return "CarBaseMessage: QualifyPeriodTime - CarId: " + CarId + ", Colour: " + Colour + ", Period: " + Period + ", PeriodTime: " + PrintTimeValue(PeriodTime, PeriodTimeType);
        }

        protected override void OnDeserialiseComplete()
        {
            TimeType type;
            PeriodTime = DeserialiseLapTimeInfo(out type);
            PeriodTimeType = type;
        }
    }


    public class QualifyPeriodTime1 : CarSectorTime
    {
        public QualifyPeriodTime1()
            : base(1)
        {
        }
    }


    public class QualifyPeriodTime2 : CarSectorTime
    {
        public QualifyPeriodTime2()
            : base(2)
        {
        }
    }


    public class QualifyPeriodTime3 : CarSectorTime
    {
        public QualifyPeriodTime3()
            : base(3)
        {
        }
    }
}

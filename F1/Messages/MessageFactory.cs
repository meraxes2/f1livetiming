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

using System;
using F1.Enums;
using F1.Messages.Car;
using F1.Messages.System;

namespace F1.Messages
{
    public class MessageFactory
    {
        #region Lookup Tables for Car Messages
        private static readonly Type[] PracticeLookup =
            {
                null,
                typeof (CarPosition),
                typeof (CarNumber),
                typeof (CarDriver),
                typeof (PracticeBestLapTime),
                typeof (CarGap),
                typeof (CarSectorTime1),
                typeof (CarSectorTime2),
                typeof (CarSectorTime3),
                typeof (CarLapCount),
                typeof (PracticeUnknown)
            };


        private static readonly Type[] RaceLookup =
            {
                null,
                typeof (CarPosition),
                typeof (CarNumber),
                typeof (CarDriver),
                typeof (CarGap),
                typeof (CarInterval),
                typeof (CarLapTime),
                typeof (CarSectorTime1),
                typeof (CarPitLap1),
                typeof (CarSectorTime2),
                typeof (CarPitLap2),
                typeof (CarSectorTime3),
                typeof (CarPitLap3),
                typeof (CarPitCount)
            };


        private static readonly Type[] QualifyLookup =
            {
                null,
                typeof (CarPosition),
                typeof (CarNumber),
                typeof (CarDriver),
                typeof (QualifyPeriodTime1),
                typeof (QualifyPeriodTime2),
                typeof (QualifyPeriodTime3),
                typeof (CarSectorTime1),
                typeof (CarSectorTime2),
                typeof (CarSectorTime3),
                typeof (CarLapCount)
            };
        #endregion


        public static IMessage CreateMessage(SystemPacketType type, int datum)
        {
            switch (type)
            {
                case SystemPacketType.EventId:
                    return new EventId();
                case SystemPacketType.KeyFrame:
                    return new KeyFrame();
                case SystemPacketType.Unknown1:
                    return new Unknown1();
                case SystemPacketType.Commentary:
                    return new Commentary();
                case SystemPacketType.RefreshRate:
                    return new RefreshRate();
                case SystemPacketType.Notice:
                    return new Notice();
                case SystemPacketType.Timestamp:
                    return new TimeStamp();
                case SystemPacketType.Weather:
                    //return new Weather();   
                    return CreateMessage((WeatherType)(datum & 0x07));
                case SystemPacketType.Speed:
                    return new Speed();
                case SystemPacketType.TrackStatus:
                    return new TrackStatus();
                case SystemPacketType.Copyright:
                    return new Copyright();
                default:
                    return null;                    
            }
        }


        public static IMessage CreateMessage(CarType carType, EventType eventType)
        {
            switch (carType)
            {
                case CarType.History:
                    return new CarPositionHistory();
                case CarType.Position:
                    return new CarPositionUpdate();
                default:
                    switch(eventType)
                    {
                        case EventType.Race:
                            return CreateFromType(RaceLookup[(int) carType]);
                        case EventType.Practice:
                            return CreateFromType(PracticeLookup[(int) carType]);
                        case EventType.Qualifying:
                            return CreateFromType(QualifyLookup[(int)carType]);
                        default:
                            break;
                    }
                    break;
            }

            return null;
        }


        private static IMessage CreateFromType(Type msgType)
        {
            if (null != msgType)
            {
                return (IMessage) Activator.CreateInstance(msgType);
            }

            return null;
        }

        private static IMessage CreateMessage(WeatherType weatherType)
        {
            switch (weatherType)
            {
                case WeatherType.SessionClock:
                    return new WeatherSessionClock();

                case WeatherType.TrackTemperature:
                    return new WeatherTrackTemperature();

                case WeatherType.AirTemperature:
                    return new WeatherAirTemperature();

                case WeatherType.WetTrack:
                    return new WeatherWetTrack();

                case WeatherType.WindSpeed:
                    return new WeatherWindSpeed();

                case WeatherType.Humidity:
                    return new WeatherHumidity();

                case WeatherType.Pressure:
                    return new WeatherPressure();

                case WeatherType.WindDirection:
                    return new WeatherWindDirection();

                default:
                    return new Weather();
            }
        }
    }
}

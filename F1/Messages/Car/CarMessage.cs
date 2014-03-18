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
using System.IO;
using Common.Utils.Strings;
using F1.Data.Packets;
using F1.Enums;
using System.Globalization;


namespace F1.Messages.Car
{
    /// <summary>
    /// Defines common behaviour, and base type, to all ICarMessage implementations. It
    /// contains the methods common to all including CarId and Colour. see <see cref="ICarMessage"/>.
    /// </summary>
    public class CarBaseMessage : ICarMessage
    {
        /// <summary>
        /// Specifies how to interpret the accompanying data.
        /// </summary>
        public enum TimeType
        {
            /// <summary>
            /// Means that the message contained no valid data, hence the value will not contain
            /// a valid value.
            /// </summary>
            NoData,
            /// <summary>
            /// The value is a time quantity in seconds and fractions.
            /// </summary>
            Time,
            /// <summary>
            /// The car is a lap behind.
            /// </summary>
            Lapped,
            /// <summary>
            /// The car has stopped. 
            /// </summary>
            Stop,
            /// <summary>
            /// The car is in the pit.
            /// </summary>
            InPit,
            /// <summary>
            /// The car hass exited the pit.
            /// </summary>
            Out,
            /// <summary>
            /// The car chalks up another DNF.
            /// </summary>
            Retired,
            /// <summary>
            /// The number of laps completed by the front runner. Or the number
            /// of lapse the driver is behind. This is dependant on the message type.
            /// </summary>
            NLaps,
            /// <summary>
            /// The value is a text with no time.
            /// </summary>
            TextTime
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        protected string BaseData { get; private set; }

        /// <summary>
        /// Internal use.
        /// </summary>
        protected int BaseShort { get; private set; }


        #region ICarMessage Members

        /// <summary>
        /// See <see cref="ICarMessage.CarId"/>
        /// </summary>
        public int CarId { get; private set; }

        /// <summary>
        /// See <see cref="ICarMessage.CarType"/>
        /// </summary>
        public CarType CarType { get; private set; }

        /// <summary>
        /// See <see cref="ICarMessage.Colour"/>
        /// </summary>
        public CarColours Colour
        {
            get
            {
                if ((int)CarType <= 13) return (CarColours)BaseShort;
                return CarColours.Unknown;
            }
        }
        #endregion


        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new ShortPacket(header, decryptStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            ShortPacket p = (ShortPacket) payload;

            CarId = header.CarId;
            CarType = header.CarType;
            BaseShort = p.ShortDatum;
            BaseData = (p.Data == null) ? String.Empty : StringUtils.ASCIIBytesToString(p.Data);

            OnDeserialiseComplete();
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.CarType; }
        }

        #endregion


        public override string ToString()
        {
            return "CarBaseMessage: Unknown - CarId: " + CarId + ", CarType: " + CarType + ", Colour: " + Colour + ", Unknown data: " + BaseData;
        }


        protected virtual void OnDeserialiseComplete()
        {
        }


        #region Helpers and Decoders

        protected int DeserialiseInteger()
        {
            if( String.IsNullOrEmpty(BaseData))
            {
                return -1;
            }

            return int.Parse(BaseData, CultureInfo.InvariantCulture);
        }


        protected double DeserialiseLapTimeInfo(out TimeType type)
        {
            if (!String.IsNullOrEmpty(BaseData))
            {
                switch (BaseData)
                {
                    case "LAP":
                        type = TimeType.Lapped;
                        break;
                    case "STOP":
                        type = TimeType.Stop;
                        break;
                    case "IN PIT":
                        type = TimeType.InPit;
                        break;
                    case "OUT":
                        type = TimeType.Out;
                        break;
                    case "RETIRED":
                        type = TimeType.Retired;
                        break;
                    case "●":
                        type = TimeType.TextTime;
                        break;
                    default:
                        if (BaseData[BaseData.Length - 1] == 'L' && BaseData[BaseData.Length - 2] <= '9')
                        {
                            type = TimeType.NLaps;
                            return int.Parse(BaseData.Substring(0, BaseData.Length - 1), CultureInfo.InvariantCulture);
                        }

                        type = TimeType.Time;
                        return DeserialiseLapTime();
                }
            }
            else
            {
                type = TimeType.NoData;    
            }

            return double.NaN;
        }


        protected double DeserialiseLapTime()
        {
            if (String.IsNullOrEmpty(BaseData))
            {
                return double.NaN;
            }

            string[] datums = BaseData.Split(':');

            double timeSecs = 0.0;

            if( datums.Length >= 3 )
            {
                // hours
                timeSecs += double.Parse(datums[datums.Length - 3], CultureInfo.InvariantCulture) * 3600.0;
            }
            if( datums.Length >= 2 )
            {
                // mins
                timeSecs += double.Parse(datums[datums.Length-2], CultureInfo.InvariantCulture)*60;
            }
            if( datums.Length >= 1 )
            {
                // seconds and fraction of a second
                timeSecs += double.Parse(datums[datums.Length-1], CultureInfo.InvariantCulture);
            }

            return timeSecs;
        }


        protected string PrintTimeValue( double v, TimeType t )
        {
            return (t == TimeType.Time ? v.ToString() : (t == TimeType.NLaps) ? v + " Laps" : t.ToString());
        }

        #endregion
    }
}

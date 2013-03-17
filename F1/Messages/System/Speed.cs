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

using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Utils.Strings;
using F1.Data.Packets;
using F1.Enums;
using System.Globalization;

namespace F1.Messages.System
{
    /// <summary>
    /// This type as it serves two purposes. The first is to provide a list 
    /// of fastest 6 drivers and speeds in given sectors. The second is to 
    /// provide some other 'fastest' information for a Race.
    /// These values are as displayed on the second page on the java applet. 
    /// </summary>
    public class Speed : IMessage
    {
        /// <summary>
        /// Describes the behaviour of this message
        /// </summary>
        public enum ColumnType
        {
            /// <summary>
            /// Pairs of fastest drivers and their speeds in sector 1
            /// </summary>
            FastestSector1 = 0,

            /// <summary>
            /// Pairs of fastest drivers and their speeds in sector 2
            /// </summary>
            FastestSector2 = 1,

            /// <summary>
            /// Pairs of fastest drivers and their speeds in sector 3
            /// </summary>
            FastestSector3 = 2,

            /// <summary>
            /// Pairs of fastest drivers and their speeds in 'trap'
            /// </summary>
            FastestInTrap = 3,

            /// <summary>
            /// Use MetaData field to retrieve the number of the fastest driver.
            /// </summary>
            FastestDriverNumber = 4,

            /// <summary>
            /// Use MetaData field to retrieve the name of the fastest driver.
            /// </summary>
            FastestDriverName = 5,

            /// <summary>
            /// Use MetaData field to retrieve the lap time of the fastest driver.
            /// </summary>
            FastestLapTime = 6,

            /// <summary>
            /// Use MetaData field to retrieve the lap number on which the fastest lap was set.
            /// </summary>
            FastestLapNumber = 7
        }


        /// <summary>
        /// Describes a pairing of a driver and his speed (presumably in kph).
        /// </summary>
        public class SpeedPair
        {
            public string Driver;
            public double Speed;

            public override string ToString()
            {
                return Driver + ": " + Speed + "kph";
            }
        }

        /// <summary>
        /// Defines what is meant by the FastestSectors and MetaData properties.
        /// </summary>
        public ColumnType Column { get; private set; }

        /// <summary>
        /// A pair of values of drivers and their speed values.
        /// </summary>
        public SpeedPair[] FastestSectors { get; private set; }

        /// <summary>
        /// Other data associated with this message, see <see cref="Column"/> property.
        /// </summary>
        public string MetaData { get; private set; }

        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new LongPacket(header, decryptStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            LongPacket lp = (LongPacket)payload;

            int col = lp.Data[0] - 1;

            Column = (ColumnType)(col);

            string decoded = StringUtils.ASCIIBytesToString(lp.Data, 1, lp.Data.Length - 1);

            if( col >= 0 && col <= 3 && lp.Data.Length > 0 )
            {
                string[] pairs = decoded.Split('\r');

                List<SpeedPair> fsSectorBuilder = new List<SpeedPair>();

                for( int i=0; i < pairs.Length / 2; ++i )
                {
                    if (!string.IsNullOrEmpty(pairs[i * 2]) && !string.IsNullOrEmpty(pairs[(i * 2) + 1]))
                    {
                        fsSectorBuilder.Add(new SpeedPair
                                                {
                                                    Driver = pairs[i*2],
                                                    Speed = double.Parse(pairs[(i*2) + 1], CultureInfo.InvariantCulture)
                                                });
                    }
                    else
                    {
                        break;
                    }
                }

                FastestSectors = fsSectorBuilder.ToArray();
            }
            else if( decoded.Length > 0 )
            {
                MetaData = decoded;
            }
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.Speed; }
        }

        #endregion

        public override string ToString()
        {
            if((int)Column >= 0 && (int)Column <= 3)
            {
                StringBuilder str = new StringBuilder();
                foreach(SpeedPair pair in FastestSectors)
                {
                    str.Append(pair + ", ");
                }

                return "SystemMessage: Speed - Column: " + Column + ", Driver and Speeds: " + str;
            }

            return "SystemMessage: Speed - Column: " + Column + ", MetaData: " + MetaData;
        }
    }
}
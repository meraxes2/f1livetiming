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
using F1.Data.Packets;
using F1.Enums;

namespace F1.Messages.Car
{
    /// <summary>
    /// This is an array of position changes for this car, which can be
    /// used to plot the history for the car on a graph.
    /// </summary>
    public class CarPositionHistory : ICarMessage
    {
        /// <summary>
        /// These are all the positions of the car, where index=0 is the oldest. Note as
        /// with CarPositionUpdate, there may be a 0 value. Which is the equivelant of a
        /// 'no' value.
        /// </summary>
        public int[] PositionHistory { get; private set; }

        #region ICarMessage Members
        public int CarId { get; private set; }
        public CarType CarType { get { return CarType.History; } }
        public CarColours Colour { get { return CarColours.Unknown; } }
        #endregion

        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new LongPacket(header, decryptStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            LongPacket p = (LongPacket)payload;

            CarId = header.CarId;

            PositionHistory = new int[ p.Data.Length ];

            p.Data.CopyTo(PositionHistory, 0);
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.CarType; }
        }

        #endregion

        public override string ToString()
        {
            return "CarMessage: CarPositionHistory - CarId: " + CarId + ", Car Position History: " + Common.Utils.Strings.HexString.CommaSeperateArray(PositionHistory);
        }
    }
}
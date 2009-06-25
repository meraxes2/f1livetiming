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

using System.IO;
using F1.Exceptions;

namespace F1.Data.Packets
{
    /// <summary>
    /// This primitive type is a helper for tracking and receiving complete
    /// 'packets' of data. It must be implemented. This packet must do
    /// a ContinueDataRead() until it IsComplete.
    /// </summary>
    public abstract class Packet
    {
        private int _dataReadSoFar;
        private readonly Stream _input;

        protected Packet(Stream input)
        {
            _input = input;
        }

        /// <summary>
        /// Called from a constructor to begin the download process.
        /// </summary>
        /// <param name="dataSize">The amount of data required for this Packet.</param>
        protected void AcquirePayload(int dataSize)
        {
            try
            {
                // It's got some data, so let's retrieve it
                Payload = new byte[dataSize];

                _dataReadSoFar = _input.Read(Payload, 0, dataSize);
            }
            catch(PacketGarbageException e)
            {
                _dataReadSoFar = e.AmountRead;
                IsGarbage = true;
            }
        }


        /// <summary>
        /// If after calling AcquirePayload this Packet is not complete this
        /// method should be called when there is more data available in an
        /// attempt to try and retrieve a completed packet.
        /// </summary>
        public void ContinueDataRead()
        {
            try
            {
                if (_dataReadSoFar < Payload.Length)
                {
                    _dataReadSoFar += _input.Read(Payload, _dataReadSoFar, Payload.Length - _dataReadSoFar);
                }
            }
            catch(PacketGarbageException e)
            {
                _dataReadSoFar += e.AmountRead;
                IsGarbage = true;
            }
        }
        
      
        /// <summary>
        /// The implementing class can use this to retrieve the data received.
        /// </summary>
        protected byte[] Payload { get; private set; }


        /// <summary>
        /// Indicates that we have received all of the requested bytes.
        /// </summary>
        public bool IsComplete { get { return null == Payload || _dataReadSoFar == Payload.Length;  } }


        /// <summary>
        /// Indicates that the data we have received is garbage.
        /// </summary>
        public bool IsGarbage { get; private set; }
    }
}
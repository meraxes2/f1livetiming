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

namespace F1.Exceptions
{
    /// <summary>
    /// A type not seen before has been described in a packet header and therefore
    /// the live timing does not know how to continue with the stream. This is game
    /// over I'm afraid.
    /// </summary>
    class UnknownSystemTypeException : Exception
    {
        public UnknownSystemTypeException(int rawType, int carId, int datum)
            : base("Unrecognised message type: " + rawType + " carId: " + carId + " datum: " + datum)
        {
            
        }
    }
}

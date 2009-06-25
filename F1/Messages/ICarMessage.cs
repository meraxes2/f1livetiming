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

using F1.Enums;

namespace F1.Messages
{
    /// <summary>
    /// Todo - document each of the colour meaning.
    /// </summary>
    public enum CarColours
    {
        Unknown = -1,
        Black = 0,
        White = 1,
        Red = 2,
        Green = 3,
        Magenta = 4,
        Cyan = 5,
        Yellow = 6,
        Grey = 7
    }

    /// <summary>
    /// This is a specialisation of the IMessage type to further define
    /// car specific parameters.
    /// </summary>
    public interface ICarMessage : IMessage
    {
        /// <summary>
        /// The unique identifier of a car as references by this livetiming library.
        /// </summary>
        int CarId { get; }

        /// <summary>
        /// Specifies the type of car message.
        /// </summary>
        CarType CarType { get; }

        /// <summary>
        /// Different colours mean different things, if you have a look at the genuine
        /// applet you'll see what.
        /// </summary>
        CarColours Colour { get; }
    }
}

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
    /// Not sure what this means. I think it's likely to indicate some state of
    /// the car. For example the car has gone back into the pit.
    /// </summary>
    public class PracticeUnknown : CarBaseMessage
    {
        public override string ToString()
        {
            return "CarBaseMessage: PracticeUnknown - CarId: " + CarId + ", Colour: " + Colour + ", Unknown: " + BaseData;
        }
    }
}

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

namespace F1.Enums
{
    public enum SystemPacketType
    {
        CarType = 0,
        EventId = 1,
        KeyFrame = 2,
        Unknown1 = 3,
        Commentary = 4,
        RefreshRate = 5,
        Notice = 6,
        Timestamp = 7,
        Weather = 9,
        Speed = 10,
        TrackStatus = 11,
        Copyright = 12,
        RaceLapCount = 20,
        EndOfSession = 21
    }
}
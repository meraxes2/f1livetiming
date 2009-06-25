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
    /// <summary>
    /// Car specific message types
    /// </summary>
    public enum CarType
    {
        Position = 0,
        History = 15
    }


    /// <summary>
    /// Car message types during a race.
    /// </summary>
    public enum RaceCarType
    {
        RacePosition = 1,
        RaceNumber = 2,
        RaceDriver = 3,
        RaceGap = 4,
        RaceInterval = 5,
        RaceLapTime = 6,
        RaceSector_1 = 7,
        RacePitLap_1 = 8,
        RaceSector_2 = 9,
        RacePitLap_2 = 10,
        RaceSector_3 = 11,
        RacePitLap_3 = 12,
        RacePitCount = 13
    }


    /// <summary>
    /// Car message types during a practice session.
    /// </summary>
    public enum PracticeCarType
    {
        PracticePosition = 1,
        PracticeNumber = 2,
        PracticeDriver = 3,
        PracticeBest = 4,
        PracticeGap = 5,
        PracticeSector1 = 6,
        PracticeSector2 = 7,
        PracticeSector3 = 8,
        PracticeLaps = 9,
        PracticeUnknown = 10
    }


    /// <summary>
    /// Car message type during a qualifying session.
    /// </summary>
    public enum QualifyCarType
    {
        QualifyingPosition = 1,
        QualifyingNumber = 2,
        QualifyingDriver = 3,
        QualifyingPeriod_1 = 4,
        QualifyingPeriod_2 = 5,
        QualifyingPeriod_3 = 6,
        QualifyingSector_1 = 7,
        QualifyingSector_2 = 8,
        QualifyingSector_3 = 9,
        QualifyingLaps = 10,
    }
}
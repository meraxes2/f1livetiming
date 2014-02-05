/*
 *  f1livetiming - Part of the Live Timing Library for .NET
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
using WP7TilesCommon.Data;

namespace WP7TilesCommon.Tools
{
    public static class DataChecker
    {
        public static bool IsUpToDate(RacingEvent data)
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now - data.DownloadTimestamp;

            data.UpdateRemainingTime();

            if (span.TotalMinutes > 5) //prevent updating too often
            {
                if (!data.IsSessionStartTimeValid && data.RemainingTime == 0)
                {
                    return false;
                }

                if (data.IsSessionCompleted)
                {
                    return false;
                }     
            }

            return true;
        }
    }
}

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

using Microsoft.Phone.Shell;
using System;
using WP7TilesCommon.Data;

namespace WP7TilesCommon.Tools
{
    static class RacingEventDataConverter
    {
        public static StandardTileData Convert(RacingEvent data)
        {
            return new StandardTileData() { BackTitle = GetTitle(data), BackContent = GetContent(data) };
        }

        private static string GetContent(RacingEvent data)
        {
            string content = "";
            if (data.Country != "")
            {
                content += data.Country + "\n";
            }

            if (data.Circuit != "")
            {
                content += data.Circuit + "\n";
            }

            if (data.Session != "")
            {
                content += data.Session;
            }

            return content;
        }

        private static string GetTitle(RacingEvent data)
        {
            string title = "";

            if (data.IsSessionLive)
            {
                title = "Session live";
            }
            else if (data.IsSessionCompleted)
            {
                title = "Session end";
            }
            else if (DeviceType.IsWP7LowMemDevice && data.IsSessionStartTimeValid)
            {
                //date with time
                title = data.SessionStartTime.ToString("g");
            }
            else
            {
                TimeSpan left = new TimeSpan(0, 0, (int)data.RemainingTime);
                if (left.Days > 0)
                {
                    title = left.Days.ToString();
                    title += left.Days == 1 ? " Day" : " Days";
                }
                else
                {
                    if (data.IsSessionStartTimeValid)
                    {
                        //hour and minutes
                        title = data.SessionStartTime.ToShortTimeString();

                        //day of week
                        title += " " + data.SessionStartTime.ToString("dddd");
                    }
                    else
                    {
                        //do it standard way
                        if (left.Hours > 0)
                        {
                            title = left.Hours.ToString();
                            title += left.Hours == 1 ? " Hour" : " Hours";
                        }
                        else if (left.Minutes > 0)
                        {
                            title = left.Minutes.ToString();
                            title += left.Minutes == 1 ? " Minute" : " Minutes";
                        }
                        else if (left.Seconds > 0)
                        {
                            title = left.Seconds.ToString();
                            title += left.Seconds == 1 ? " Second" : " Seconds";
                        }
                        else
                        {
                            title = "No data";
                        }
                    }
                }
            }

            return title;
        }
    }
}

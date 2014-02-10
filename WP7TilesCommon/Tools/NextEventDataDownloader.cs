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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using WP7TilesCommon.Data;
using WP7TilesCommon.Enums;

namespace WP7TilesCommon.Tools
{
    public delegate void OnEventDataDownloadCompletedEventHandler(RacingEvent data, CompletionStatus status);

    public class NextEventDataDownloader
    {
        public event OnEventDataDownloadCompletedEventHandler OnDownloadCompleted;

        public void DownloadEventDataAsync()
        {
            WebClient web = new WebClient();
            web.OpenReadCompleted += web_OpenReadCompleted;
            try
            {
                web.OpenReadAsync(new Uri("http://mobile.formula1.com"));
            }
            catch (Exception)
            {
                OnDownloadCompleted.Invoke(new RacingEvent(), CompletionStatus.Failed);
            }
        }

        void web_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                string page = "";

                using (StreamReader sr = new StreamReader(e.Result))
                {
                    page = sr.ReadToEnd();
                }

                e.Result.Close();

                RacingEvent f1Event = new RacingEvent();
                //<td class="circuit">Shanghai - 12, 13, 14 Apr </td>
                //<td class="circuit">Las marina burn - 12, 13, 14 Apr </td>
                Match m = Regex.Match(page, "<td class=\"circuit\">(.*?)-.*?</td>", RegexOptions.Singleline);
                if (m.Groups.Count > 1)
                {
                    f1Event.Circuit = m.Groups[1].Value;
                    f1Event.Circuit = f1Event.Circuit.Trim();
                }

                //Search for country
                //<A class="btn" onclick="return false" href="http://mobile.formula1.com/races/895">CHINA</A>
                //<a class="btn" onclick="return false" href="/races/895">CHINA</a>
                m = Regex.Match(page, "<a.+?href=\".*?/races/\\d+\">(.*?)</a>", RegexOptions.Singleline);
                if (m.Groups.Count > 1)
                {
                    f1Event.Country = m.Groups[1].Value;
                    f1Event.Country = f1Event.Country.Trim();
                    f1Event.Country = f1Event.Country.ToLowerInvariant();
                    if (f1Event.Country.Length > 0)
                    {
                        //capitalize
                        var words = f1Event.Country.Split(' ');
                        f1Event.Country = "";
                        foreach (string word in words)
                        {
                            if (word.Length > 0)
                            {
                                string s = word.Substring(0, 1);
                                s = s.ToUpperInvariant();
                                f1Event.Country += " " + s + word.Substring(1);
                            }
                        }
                    }

                    f1Event.Country = f1Event.Country.Trim();
                }

                m = Regex.Match(page, "<td class=\"event\">(.*?)</td>", RegexOptions.Singleline);
                if (m.Groups.Count > 1)
                {
                    f1Event.Session = m.Groups[1].Value;
                    f1Event.Session = f1Event.Session.Trim();
                }

                if (string.IsNullOrEmpty(f1Event.Session))
                {
                    //still not found so search for 
                    //<div class="mainpad">Qualifying 1<br/>
                    m = Regex.Match(page, "<div class=\"mainpad\">(\\w+\\s*\\d*)<br/>", RegexOptions.Singleline);
                    if (m.Groups.Count > 1)
                    {
                        f1Event.Session = m.Groups[1].Value;
                        f1Event.Session = f1Event.Session.Trim();
                    }
                }

                if (string.IsNullOrEmpty(f1Event.Session))
                {
                    //no event block so its probably race time, search for race
                    if (page.IndexOf("Race Result") != -1 || page.IndexOf("Race Winner") != -1)
                    {
                        f1Event.Session = "Race";
                    }
                }

                //first try to find exact event time
                m = Regex.Match(page, "var t_unixtime = (\\d+)", RegexOptions.Singleline);
                if (m.Groups.Count > 1)
                {
                    ulong unixTime;
                    if (ulong.TryParse(m.Groups[1].Value, out unixTime))
                    {
                        DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
                        f1Event.SessionStartTime = time.ToLocalTime();
                        f1Event.UpdateRemainingTime();
                    }
                }
                else
                {
                    //next find remaining seconds
                    m = Regex.Match(page, "var t_time = (\\d+);", RegexOptions.Singleline);
                    if (m.Groups.Count > 1)
                    {
                        ulong seconds = 0;
                        if (ulong.TryParse(m.Groups[1].Value, out seconds))
                        {
                            f1Event.RemainingTime = seconds;
                        }
                    }
                }

                if (page.IndexOf("SESSION NOW ON") != -1)
                {
                    f1Event.IsSessionLive = true;
                }

                f1Event.DownloadTimestamp = DateTime.Now;

                OnDownloadCompleted.Invoke(f1Event, CompletionStatus.Completed);
            }
            catch (Exception)
            {
                OnDownloadCompleted.Invoke(new RacingEvent(), CompletionStatus.Failed);
            }
        }
    }
}

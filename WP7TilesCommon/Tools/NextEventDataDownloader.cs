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
using System.Collections.Generic;
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
                //web.OpenReadAsync(new Uri("http://mobile.formula1.com"));
                web.OpenReadAsync(new Uri("https://www.f1calendar.com/"));
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
                f1Event.Circuit = "";
                f1Event.Country = "unknown";

                List<SessionInfo> sessions = new List<SessionInfo>();

                Match m = Regex.Match(page, "<tbody .*? class=\".*?next-event.*?\">(.*?)</tbody>", RegexOptions.Singleline);
                if (m.Groups.Count > 1)
                {
                    string str;
                    Match ms = Regex.Match(m.Groups[1].Value, "<tr class=\"first-practice.*?>(.*?)</tr>", RegexOptions.Singleline);
                    if (ms.Groups.Count > 1)
                    {
                        SessionInfo practice1 = new SessionInfo();
                        practice1.Type = "Practice 1";

                        str = ms.Groups[1].Value;
                        var date = Regex.Match(str, "<td class=\"date-column\">(.*?)</td>", RegexOptions.Singleline);
                        if (date.Groups.Count > 1)
                        {
                            var time = Regex.Match(date.Groups[1].Value, "<abbr .*? title=\"(.*?)\".*?</abbr>", RegexOptions.Singleline);
                            if (time.Groups.Count > 1)
                            {
                                practice1.Start = DateTime.Parse(time.Groups[1].Value);
                                practice1.Time = SessionTimeSpan.Practice1;
                                sessions.Add(practice1);
                            }
                        }
                    }

                    ms = Regex.Match(m.Groups[1].Value, "<tr class=\"second-practice.*?>(.*?)</tr>", RegexOptions.Singleline);
                    if (ms.Groups.Count > 1)
                    {
                        SessionInfo practice2 = new SessionInfo();
                        practice2.Type = "Practice 2";

                        str = ms.Groups[1].Value;
                        var date = Regex.Match(str, "<td class=\"date-column\">(.*?)</td>", RegexOptions.Singleline);
                        if (date.Groups.Count > 1)
                        {
                            var time = Regex.Match(date.Groups[1].Value, "<abbr .*? title=\"(.*?)\".*?</abbr>", RegexOptions.Singleline);
                            if (time.Groups.Count > 1)
                            {
                                practice2.Start = DateTime.Parse(time.Groups[1].Value);
                                practice2.Time = SessionTimeSpan.Practice2;
                                sessions.Add(practice2);
                            }
                        }

                    }

                    ms = Regex.Match(m.Groups[1].Value, "<tr class=\"third-practice.*?>(.*?)</tr>", RegexOptions.Singleline);
                    if (ms.Groups.Count > 1)
                    {
                        SessionInfo practice3 = new SessionInfo();
                        practice3.Type = "Practice 3";

                        str = ms.Groups[1].Value;
                        var date = Regex.Match(str, "<td class=\"date-column\">(.*?)</td>", RegexOptions.Singleline);
                        if (date.Groups.Count > 1)
                        {
                            var time = Regex.Match(date.Groups[1].Value, "<abbr .*? title=\"(.*?)\".*?</abbr>", RegexOptions.Singleline);
                            if (time.Groups.Count > 1)
                            {
                                practice3.Start = DateTime.Parse(time.Groups[1].Value);
                                practice3.Time = SessionTimeSpan.Practice3;
                                sessions.Add(practice3);
                            }
                        }

                    }

                    ms = Regex.Match(m.Groups[1].Value, "<tr class=\"qualifying.*?>(.*?)</tr>", RegexOptions.Singleline);
                    if (ms.Groups.Count > 1)
                    {
                        SessionInfo quali = new SessionInfo();
                        quali.Type = "Qualifying";

                        str = ms.Groups[1].Value;
                        var date = Regex.Match(str, "<td class=\"date-column\">(.*?)</td>", RegexOptions.Singleline);
                        if (date.Groups.Count > 1)
                        {
                            var time = Regex.Match(date.Groups[1].Value, "<abbr .*? title=\"(.*?)\".*?</abbr>", RegexOptions.Singleline);
                            if (time.Groups.Count > 1)
                            {
                                quali.Start = DateTime.Parse(time.Groups[1].Value);
                                quali.Time = SessionTimeSpan.Qualifying;
                                sessions.Add(quali);
                            }
                        }

                    }

                    ms = Regex.Match(m.Groups[1].Value, "<tr class=\"race.*?>(.*?)</tr>", RegexOptions.Singleline);
                    if (ms.Groups.Count > 1)
                    {
                        SessionInfo race = new SessionInfo();
                        race.Type = "Race";

                        str = ms.Groups[1].Value;
                        var date = Regex.Match(str, "<td class=\"date-column\">(.*?)</td>", RegexOptions.Singleline);
                        if (date.Groups.Count > 1)
                        {
                            var time = Regex.Match(date.Groups[1].Value, "<abbr.*?title=\"(.*?)\".*?</abbr>", RegexOptions.Singleline);
                            if (time.Groups.Count > 1)
                            {
                                race.Start = DateTime.Parse(time.Groups[1].Value);
                                race.Time = SessionTimeSpan.Race;
                                sessions.Add(race);
                            }
                        }
                    }

                    ms = Regex.Match(m.Groups[1].Value, "<span class=\"location\">(.*?)</span>", RegexOptions.Singleline);
                    if (ms.Groups.Count > 1)
                    {
                        f1Event.Country = ms.Groups[1].Value.Trim();
                    }
                }

                SessionInfo selectedSession = null;

                var now = DateTime.Now;
                foreach (var session in sessions)
                {
                    selectedSession = session;
                    if (now < session.Start)
                    {
                        break;
                    }
                    else if (now <= session.Start + session.Time)
                    {
                        break;
                    }
                }

                if (selectedSession != null)
                {
                    f1Event.Session = selectedSession.Type;
                    f1Event.SessionStartTime = selectedSession.Start;
                    f1Event.UpdateRemainingTime();
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

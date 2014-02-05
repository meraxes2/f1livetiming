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
using WP7TilesCommon.Tools;

namespace WP7TilesCommon.Data
{
    public class RacingEvent
    {
        string _circuit = "";
        string _session = "";
        string _country = "";
        ulong _seconds = 0;
        DateTime _sessionStartTime = DateTime.MinValue;
        bool _sessionLive = false;
        DateTime _downloadTimestamp = DateTime.MinValue;

        public RacingEvent DeepCopy()
        {
            RacingEvent re = new RacingEvent();
            re._circuit = _circuit;
            re._session = _session;
            re._country = _country;
            re._seconds = _seconds;
            re._sessionStartTime = _sessionStartTime;
            re._sessionLive = _sessionLive;
            re._downloadTimestamp = _downloadTimestamp;

            return re;
        }

        public string Circuit
        {
            get { return _circuit; }
            set { _circuit = value; }
        }

        public string Session
        {
            get { return _session; }
            set { _session = value; }
        }

        public ulong RemainingTime
        {
            get { return _seconds; }
            set { _seconds = value; }
        }

        public DateTime SessionStartTime
        {
            get { return _sessionStartTime; }
            set { _sessionStartTime = value; }
        }

        public bool IsSessionLive
        {
            get { return _sessionLive; }
            set { _sessionLive = value; }
        }

        public bool IsSessionCompleted
        {
            get { return !IsSessionLive && _seconds == 0; }
        }

        public bool IsSessionStartTimeValid
        {
            get { return _sessionStartTime != DateTime.MinValue; }
        }

        public DateTime DownloadTimestamp
        {
            get { return _downloadTimestamp; }
            set { _downloadTimestamp = value; }
        }

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public void UpdateRemainingTime()
        {
            if (IsSessionStartTimeValid)
            {
                DateTime now = DateTime.Now;
                if (SessionStartTime > now)
                {
                    RemainingTime = (ulong)(SessionStartTime - now).TotalSeconds;
                }
                else
                {
                    RemainingTime = 0;
                }

                if (now < SessionStartTime)
                {
                    IsSessionLive = false;
                }
                else
                {
                    switch (Session)
                    {
                        case "Practice 1":
                            IsSessionLive = now < (SessionStartTime + SessionTimeSpan.Practice1);
                            break;

                        case "Practice 2":
                            IsSessionLive = now < (SessionStartTime + SessionTimeSpan.Practice2);
                            break;

                        case "Practice 3":
                            IsSessionLive = now < (SessionStartTime + SessionTimeSpan.Practice3);
                            break;

                        case "Qualifying":
                            IsSessionLive = now < (SessionStartTime + SessionTimeSpan.Qualifying);
                            break;

                        case "Race":
                        default:
                            IsSessionLive = now < (SessionStartTime + SessionTimeSpan.Race);
                            break;
                    }
                }
            }
        }
    }
}

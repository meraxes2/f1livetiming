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
using System.ComponentModel;
using System.Collections.ObjectModel;
using F1.Enums;
using System.Globalization;
using System.Collections.Generic;
using WP7TilesCommon.Tools;

namespace LTLite.ViewModel
{
    public class CarsViewModel : INotifyPropertyChanged
    {
        public CarsViewModel()
        {
            Cars = new ObservableCollection<CarViewModel>();
            Comments = new ObservableCollection<CommentViewModel>();
            SpeedSector1 = new ObservableCollection<SpeedViewModel>();
            SpeedSector2 = new ObservableCollection<SpeedViewModel>();
            SpeedSector3 = new ObservableCollection<SpeedViewModel>();
            SpeedTrap = new ObservableCollection<SpeedViewModel>();
            DriversNames = new Dictionary<string, string>();

            Comments.CollectionChanged += (s, e) => { IsCommentsEmpty = ((ObservableCollection<CommentViewModel>)s).Count == 0; };

            SpeedSector1.CollectionChanged += (s, e) => { IsSpeedSector1Empty = ((ObservableCollection<SpeedViewModel>)s).Count == 0; };
            SpeedSector2.CollectionChanged += (s, e) => { IsSpeedSector2Empty = ((ObservableCollection<SpeedViewModel>)s).Count == 0; };
            SpeedSector3.CollectionChanged += (s, e) => { IsSpeedSector3Empty = ((ObservableCollection<SpeedViewModel>)s).Count == 0; };
            SpeedTrap.CollectionChanged += (s, e) => { IsSpeedTrapEmpty = ((ObservableCollection<SpeedViewModel>)s).Count == 0; };
        }

        public ObservableCollection<CarViewModel> Cars { get; set; }
        public ObservableCollection<CommentViewModel> Comments { get; set; }

        #region Comments
        string _lastComment = "";
        public string LastComment 
        {
            get{return _lastComment;}
            set
            {
                if (value != _lastComment)
                {
                    _lastComment = value;
                    NotifyPropertyChanged("LastComment");
                }
            } 
        }

        bool _isCommentsEmpty = true;
        public bool IsCommentsEmpty 
        { 
            get { return _isCommentsEmpty; }
            set
            {
                if (value != _isCommentsEmpty)
                {
                    _isCommentsEmpty = value;
                    NotifyPropertyChanged("IsCommentsEmpty");
                }
            }
        }
        #endregion

        #region Speed
        public ObservableCollection<SpeedViewModel> SpeedSector1 { get; set; }
        public ObservableCollection<SpeedViewModel> SpeedSector2 { get; set; }
        public ObservableCollection<SpeedViewModel> SpeedSector3 { get; set; }
        public ObservableCollection<SpeedViewModel> SpeedTrap { get; set; }

        bool _isSpeedSector1Empty = true;
        public bool IsSpeedSector1Empty
        {
            get { return _isSpeedSector1Empty; }
            set
            {
                if (value != _isSpeedSector1Empty)
                {
                    _isSpeedSector1Empty = value;
                    NotifyPropertyChanged("IsSpeedSector1Empty");
                }
            }
        }

        bool _isSpeedSector2Empty = true;
        public bool IsSpeedSector2Empty
        {
            get { return _isSpeedSector2Empty; }
            set
            {
                if (value != _isSpeedSector2Empty)
                {
                    _isSpeedSector2Empty = value;
                    NotifyPropertyChanged("IsSpeedSector2Empty");
                }
            }
        }

        bool _isSpeedSector3Empty = true;
        public bool IsSpeedSector3Empty
        {
            get { return _isSpeedSector3Empty; }
            set
            {
                if (value != _isSpeedSector3Empty)
                {
                    _isSpeedSector3Empty = value;
                    NotifyPropertyChanged("IsSpeedSector3Empty");
                }
            }
        }

        bool _isSpeedTrapEmpty = true;
        public bool IsSpeedTrapEmpty
        {
            get { return _isSpeedTrapEmpty; }
            set
            {
                if (value != _isSpeedTrapEmpty)
                {
                    _isSpeedTrapEmpty = value;
                    NotifyPropertyChanged("IsSpeedTrapEmpty");
                }
            }
        }
        #endregion

        public Dictionary<string, string> DriversNames { get; set; }

        public void Clear()
        {
            Cars.Clear();
            Comments.Clear();
            SpeedSector1.Clear();
            SpeedSector2.Clear();
            SpeedSector3.Clear();
            SpeedTrap.Clear();
            DriversNames.Clear(); 
        }

        #region Copyright
        string _copyright = "";
        public string Copyright
        {
            get
            {
                return _copyright;
            }
            set
            {
                if (value != _copyright)
                {
                    _copyright = value;
                    NotifyPropertyChanged("Copyright");
                }
            }
        }
        #endregion

        #region Event data
        EventType _eventType = EventType.NoEvent;
        public EventType EventType
        {
            get
            {
                return _eventType;
            }
            set
            {
                if (value != _eventType)
                {
                    _eventType = value;
                    NotifyPropertyChanged("EventType");
                }
            }
        }

        int _eventTimeElapsed = 0;
        public int EventTimeElapsed
        {
            get
            {
                return _eventTimeElapsed;
            }
            set
            {
                if (value != _eventTimeElapsed)
                {
                    _eventTimeElapsed = value;
                    NotifyPropertyChanged("EventTimeElapsed");
                    NotifyPropertyChanged("EventTimeElapsedStr");
                }
            }
        }

        public string EventTimeElapsedStr
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(_eventTimeElapsed);
                if (_eventTimeElapsed < 60)
                {
                    return time.ToString(@"ss", CultureInfo.InvariantCulture);
                }
                else if (_eventTimeElapsed < 3600)
                {
                    return time.ToString(@"mm\:ss", CultureInfo.InvariantCulture);
                }
                else
                {
                    return time.ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture);
                }
            }
        }
        
        DateTime _startTime = DateTime.MinValue; 
        TimeSpan _lastCorrectRemainingTime = TimeSpan.Zero;
        public TimeSpan LastCorrectRemainingTime
        {
            get
            {
                return _lastCorrectRemainingTime;
            }

            set
            {
                if (value != _lastCorrectRemainingTime)
                {
                    _lastCorrectRemainingTime = value;
                    _startTime = DateTime.Now;
                    EventRemainingTime = _lastCorrectRemainingTime;
                }
            }
        }

        bool _allowTimeEstimation = false;
        public bool AllowTimeEstimation
        {
            get { return _allowTimeEstimation; }
            set { _allowTimeEstimation = value; }
        }

        TimeSpan _remainingTimeEstimated = TimeSpan.Zero;

        public TimeSpan EventRemainingTime
        {
            get
            {
                return _remainingTimeEstimated;
            }

            set
            {
                if (value != _remainingTimeEstimated)
                {
                    _remainingTimeEstimated = value;
                    NotifyPropertyChanged("EventRemainingTime");
                    NotifyPropertyChanged("EventRemainingTimeStr");
                    NotifyPropertyChanged("RemainingTimeHours");
                    NotifyPropertyChanged("IsRemainingTimeHEnabled");
                    NotifyPropertyChanged("RemainingTimeMinutes");
                    NotifyPropertyChanged("IsRemainingTimeMEnabled");
                    NotifyPropertyChanged("RemainingTimeSeconds");
                    NotifyPropertyChanged("IsRemainingTimeSEnabled");
                }
            }
        }

        public string EventRemainingTimeStr
        {
            get
            {
                TimeSpan time = EventRemainingTime;
                if (time.Hours == 0)
                {
                    return time.ToString(@"mm\:ss", CultureInfo.InvariantCulture);
                }
                else
                {
                    return time.ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture);
                }
            }
        }

        public int RemainingTimeHours
        {
            get 
            {
                return EventRemainingTime.Hours;
            }
        }

        public bool IsRemainingTimeHEnabled
        {
            get
            {
                return EventRemainingTime.TotalHours >= 1;
            }
        }

        public int RemainingTimeMinutes
        {
            get
            {
                return EventRemainingTime.Minutes;
            }
        }

        public bool IsRemainingTimeMEnabled
        {
            get
            {
                return EventRemainingTime.TotalMinutes >= 1;
            }
        }

        public int RemainingTimeSeconds
        {
            get
            {
                return EventRemainingTime.Seconds;
            }
        }

        public bool IsRemainingTimeSEnabled
        {
            get
            {
                return EventRemainingTime.TotalSeconds >= 1;
            }
        }

        public void UpdateEventRemainingTime()
        {
            if (_allowTimeEstimation)
            {
                if (_startTime != DateTime.MinValue && _lastCorrectRemainingTime != TimeSpan.Zero)
                {
                    TimeSpan timeElapsedFromLastUpdate = DateTime.Now - _startTime;
                    if (timeElapsedFromLastUpdate <= _lastCorrectRemainingTime)
                    {
                        EventRemainingTime = _lastCorrectRemainingTime - timeElapsedFromLastUpdate;
                    }
                    else
                    {
                        EventRemainingTime = TimeSpan.Zero;
                    }
                }
            }
        }
        #endregion

        #region Track status
        F1.Messages.System.TrackStatus.Colour _trackStatus = F1.Messages.System.TrackStatus.Colour.Red;
        public string TrackStatus
        {
            get
            {
                switch (_trackStatus)
                {
                    case F1.Messages.System.TrackStatus.Colour.Green:
                        return "\u26ab"; //green circle

                    case F1.Messages.System.TrackStatus.Colour.Yellow:
                        return "\u25b2"; //yellow triangle

                    default:
                    case F1.Messages.System.TrackStatus.Colour.Red:
                        return "\u25a0"; //red square
                }
            }
        }

        public F1.Messages.System.TrackStatus.Colour TrackStatusColor
        {
            get
            {
                return _trackStatus;
            }
            set
            {
                if (value != _trackStatus)
                {
                    _trackStatus = value;
                    NotifyPropertyChanged("TrackStatusColor");
                    NotifyPropertyChanged("TrackStatus");
                }
            }
        }
        #endregion

        #region Progress
        string _trackStatusMessage = "";
        public string TrackStatusMessage
        {
            get
            {
                return _trackStatusMessage;
            }
            set
            {
                if (value != _trackStatusMessage)
                {
                    _trackStatusMessage = value;
                    NotifyPropertyChanged("TrackStatusMessage");
                }
            }
        }

        bool _isIndeterminate = false;
        public bool ProgressIsIndeterminate
        {
            get
            {
                return _isIndeterminate;
            }
            set
            {
                if (value != _isIndeterminate)
                {
                    _isIndeterminate = value;
                    NotifyPropertyChanged("ProgressIsIndeterminate");
                }
            }
        }

        bool _isVisible = false;
        public bool ProgressIsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    NotifyPropertyChanged("ProgressIsVisible");
                }
            }
        }

        string _text = "";
        public string ProgressText
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    NotifyPropertyChanged("ProgressText");
                }
            }
        }
        #endregion

        #region Weather
        int _trackTemperature = 0;
        public int WeatherTrackTemperature
        {
            get
            {
                return _trackTemperature;
            }
            set
            {
                if (value != _trackTemperature)
                {
                    _trackTemperature = value;
                    NotifyPropertyChanged("WeatherTrackTemperature");
                    NotifyPropertyChanged("WeatherTrackTemperatureStr");
                }
            }
        }
        public string WeatherTrackTemperatureStr
        {
            get
            {
                return _trackTemperature.ToString() + "\u00B0" + "C";
            }
        }

        int _airTemperature = 0;
        public int WeatherAirTemperature
        {
            get
            {
                return _airTemperature;
            }
            set
            {
                if (value != _airTemperature)
                {
                    _airTemperature = value;
                    NotifyPropertyChanged("WeatherAirTemperature");
                    NotifyPropertyChanged("WeatherAirTemperatureStr");
                }
            }
        }
        public string WeatherAirTemperatureStr
        {
            get
            {
                return _airTemperature.ToString() + "\u00B0" + "C";
            }
        }

        bool _wet = false;
        public bool WeatherIsWet
        {
            get
            {
                return _wet;
            }
            set
            {
                if (value != _wet)
                {
                    _wet = value;
                    NotifyPropertyChanged("WeatherIsWet");
                    NotifyPropertyChanged("WeatherIsWetStr");
                }
            }
        }
        public string WeatherIsWetStr
        {
            get
            {
                return _wet ? "Yes" : "No";
            }
        }

        double _windSpeed = 0.0;
        public double WeatherWindSpeed
        {
            get
            {
                return _windSpeed;
            }
            set
            {
                if (value != _windSpeed)
                {
                    _windSpeed = value;
                    NotifyPropertyChanged("WeatherWindSpeed");
                    NotifyPropertyChanged("WeatherWindSpeedStr");
                }
            }
        }
        public string WeatherWindSpeedStr
        {
            get
            {
                return _windSpeed.ToString("F1", CultureInfo.InvariantCulture) + " mps";
            }
        }

        int _windDirection = 0;
        public int WeatherWindDirection
        {
            get
            {
                return _windDirection;
            }
            set
            {
                if (value != _windDirection)
                {
                    _windDirection = value;
                    NotifyPropertyChanged("WeatherWindDirection");
                }
            }
        }

        int _humidity = 0;
        public int WeatherHumidity
        {
            get
            {
                return _humidity;
            }
            set
            {
                if (value != _humidity)
                {
                    _humidity = value;
                    NotifyPropertyChanged("WeatherHumidity");
                    NotifyPropertyChanged("WeatherHumidityStr");
                }
            }
        }
        public string WeatherHumidityStr
        {
            get
            {
                return _humidity.ToString() + "%";
            }
        }

        double _pressure = 0.0;
        public double WeatherPressure
        {
            get
            {
                return _pressure;
            }
            set
            {
                if (value != _pressure)
                {
                    _pressure = value;
                    NotifyPropertyChanged("WeatherPressure");
                    NotifyPropertyChanged("WeatherPressureStr");
                }
            }
        }
        public string WeatherPressureStr
        {
            get
            {
                return _pressure.ToString("F1", CultureInfo.InvariantCulture) + " mBar";
            }
        }
        #endregion

        #region Upcomming event data
        DateTime _eventTimestamp = DateTime.MinValue;
        public DateTime EventTimeStamp { set { _eventTimestamp = value; } }
        DateTime _eventStartTime = DateTime.MinValue;
        public DateTime EventStartTime 
        {
            get
            {
                return _eventStartTime;
            }
            set 
            {
                if (value != _eventStartTime)
                {
                    _eventStartTime = value;
                    NotifyPropertyChanged("EventStartTime");
                    NotifyPropertyChanged("EventStartTimeStr");
                    NotifyPropertyChanged("IsEventStartTimeValid");
                }
            } 
        }

        public string EventStartTimeStr
        {
            get { return _eventStartTime.ToLongDateString() + " " + _eventStartTime.ToShortTimeString(); }
        }

        public bool IsEventStartTimeValid
        {
            get { return _eventStartTime != DateTime.MinValue; }
        }

        TimeSpan _eventStartRemainingTime = TimeSpan.FromSeconds(0);
        public TimeSpan EventStartRemainingTime { set { _eventStartRemainingTime = value; } }
        bool _sessionLive = false;
        TimeSpan _counterRemainingTime = TimeSpan.FromSeconds(0);

        string _circuit = "unknown circuit";
        public string Circuit
        {
            get
            {
                return _circuit;
            }
            set
            {
                if (value != _circuit)
                {
                    _circuit = value;
                    NotifyPropertyChanged("Circuit");
                }
            }
        }

        string _session = "unknown session";
        public string Session
        {
            get
            {
                return _session;
            }
            set
            {
                if (value != _session)
                {
                    _session = value;
                    NotifyPropertyChanged("Session");
                }
            }
        }

        int _counterDays = 0;
        public int CounterDays
        {
            get
            {
                return _counterDays;
            }
            set
            {
                if (value != _counterDays)
                {
                    _counterDays = value;
                    NotifyPropertyChanged("CounterDays");
                    NotifyPropertyChanged("CounterDaysTxt");
                    NotifyPropertyChanged("IsCounterDaysEnabled");
                }
            }
        }
        public bool IsCounterDaysEnabled
        {
            get
            {
                return _counterRemainingTime.TotalDays >= 1;
            }
        }

        int _counterHours = 0;
        public int CounterHours
        {
            get
            {
                return _counterHours;
            }
            set
            {
                if (value != _counterHours)
                {
                    _counterHours = value;
                    NotifyPropertyChanged("CounterHours");
                    NotifyPropertyChanged("IsCounterHoursEnabled");
                }
            }
        }
        public bool IsCounterHoursEnabled
        {
            get
            {
                return _counterRemainingTime.TotalHours >= 1;
            }
        }

        int _counterMinutes = 0;
        public int CounterMinutes
        {
            get
            {
                return _counterMinutes;
            }
            set
            {
                if (value != _counterMinutes)
                {
                    _counterMinutes = value;
                    NotifyPropertyChanged("CounterMinutes");
                    NotifyPropertyChanged("IsCounterMinutesEnabled");
                }
            }
        }
        public bool IsCounterMinutesEnabled
        {
            get
            {
                return _counterRemainingTime.TotalMinutes >= 1;
            }
        }

        int _counterSeconds = 0;
        public int CounterSeconds
        {
            get
            {
                return _counterSeconds;
            }
            set
            {
                if (value != _counterSeconds)
                {
                    _counterSeconds = value;
                    NotifyPropertyChanged("CounterSeconds");
                }
            }
        }

        public string CounterDaysTxt
        {
            get { return _counterDays == 1 ? "DAY" : "DAYS"; }
        }

        public bool IsSessionLive
        {
            get { return _sessionLive; }
            set 
            {
                if (_sessionLive != value)
                {
                    _sessionLive = value;
                    NotifyPropertyChanged("IsSessionLive");
                    NotifyPropertyChanged("IsSessionCompleted");
                    NotifyPropertyChanged("IsCounterEnabled");
                }
            }
        }
        public bool IsSessionCompleted
        {
            get { return !_sessionLive && _counterRemainingTime.TotalSeconds == 0; }
        }
        public bool IsCounterEnabled
        {
            get { return !IsSessionCompleted && !IsSessionLive; }
        }

        public void UpdateEventStartRemainingTime()
        {
            DateTime now = DateTime.Now;
            if (_eventStartTime != DateTime.MinValue)
            {
                if (_eventStartTime > now)
                {
                    _counterRemainingTime = _eventStartTime - now;
                }
                else
                {
                    _counterRemainingTime = TimeSpan.FromSeconds(0);
                    switch (Session)
                    {
                        case "Practice 1":
                            IsSessionLive = now < (_eventStartTime + SessionTimeSpan.Practice1);
                            break;

                        case "Practice 2":
                            IsSessionLive = now < (_eventStartTime + SessionTimeSpan.Practice2);
                            break;

                        case "Practice 3":
                            IsSessionLive = now < (_eventStartTime + SessionTimeSpan.Practice3);
                            break;

                        case "Qualifying":
                            IsSessionLive = now < (_eventStartTime + SessionTimeSpan.Qualifying);
                            break;

                        case "Race":
                        default:
                            IsSessionLive = now < (_eventStartTime + SessionTimeSpan.Race);
                            break;
                    }
                }
            }
            else
            {
                if (_eventTimestamp > now) _eventTimestamp = now;

                TimeSpan elapsed = now - _eventTimestamp;
                if (_eventStartRemainingTime > elapsed)
                {
                    _counterRemainingTime = _eventStartRemainingTime - elapsed;
                }
                else
                {
                    _counterRemainingTime = TimeSpan.FromSeconds(0);
                }
            }

            CounterDays = _counterRemainingTime.Days;
            CounterHours = _counterRemainingTime.Hours;
            CounterMinutes = _counterRemainingTime.Minutes;
            CounterSeconds = _counterRemainingTime.Seconds;

            NotifyPropertyChanged("IsSessionLive");
            NotifyPropertyChanged("IsSessionCompleted");
            NotifyPropertyChanged("IsCounterEnabled");

            NotifyPropertyChanged("IsCounterDaysEnabled");
            NotifyPropertyChanged("IsCounterHoursEnabled");
            NotifyPropertyChanged("IsCounterMinutesEnabled");
        }
        #endregion

        #region Fastest Lap
        string _flDriverName = "";
        public string FastestDriverName
        {
            get
            {
                return _flDriverName;
            }
            set
            {
                if (value != _flDriverName)
                {
                    _flDriverName = value;
                    NotifyPropertyChanged("FastestDriverName");
                }
            }
        }

        string _flDriverNumber = "";
        public string FastestDriverNumber
        {
            get
            {
                return _flDriverNumber;
            }
            set
            {
                if (value != _flDriverNumber)
                {
                    _flDriverNumber = value;
                    NotifyPropertyChanged("FastestDriverNumber");
                }
            }
        }

        string _flLapTime = "";
        public string FastestLapTime
        {
            get
            {
                return _flLapTime;
            }
            set
            {
                if (value != _flLapTime)
                {
                    _flLapTime = value;
                    NotifyPropertyChanged("FastestLapTime");
                }
            }
        }

        string _flLapNumber = "";
        public string FastestLapNumber
        {
            get
            {
                return _flLapNumber;
            }
            set
            {
                if (value != _flLapNumber)
                {
                    _flLapNumber = value;
                    NotifyPropertyChanged("FastestLapNumber");
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

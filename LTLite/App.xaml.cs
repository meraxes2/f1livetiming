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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using F1;
using LTLite.ViewModel;
using System.IO.IsolatedStorage;
using LTLite.Tools;

namespace LTLite
{
    public delegate void OnSettingsPageRequestEventHandler();

    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public string UserName { get; set; }
        public string UserPasswd { get; set; }

        public string ApplicationVersion { get; private set; }

        private Timer _timer = null;
        private Timer _counterTimer = null;

        public bool IsRunUnderLockScreenEnabled
        {
            get
            {
                return PhoneApplicationService.Current.ApplicationIdleDetectionMode == IdleDetectionMode.Disabled;
            }
        }

        public bool IsPreventLockScreenEnabled
        {
            get
            {
                return PhoneApplicationService.Current.UserIdleDetectionMode == IdleDetectionMode.Disabled;
            }
        }

        public bool IsLiveTimingRunning
        {
            get
            {
                return _lt != null;
            }
        }

        private static bool _phoneLocked = false;
        public static bool IsPhoneLocked
        {
            get
            {
                return _phoneLocked;
            }
        }

        ILiveTimingApp _lt = null;
        private List<CarViewModel> _invisibleCars = new List<CarViewModel>();

        static CarsViewModel _carsViewModel = null;
        public static CarsViewModel MainViewModel
        {
            get
            {
                if (_carsViewModel == null)
                {
                    _carsViewModel = new CarsViewModel();
                }
                return _carsViewModel;
            }
        }

        public static bool IsAppJustLaunched { get; set; }
        public static bool IsAppJustResumedFromTombStone { get; set; }
        public static bool NeedSettingsPage { get; set; }
        public static event OnSettingsPageRequestEventHandler OnSettingsPageRequest;

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            string mail = "andrej";
            mail += "360";
            mail += "@";
            mail += "gmail";
            mail += ".com";
            LittleWatson.Init("crash.log", mail, "LTLite");

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            //Application.Current.Host.Settings.EnableFrameRateCounter = true;
            //Application.Current.Host.Settings.EnableRedrawRegions = true;
            //Application.Current.Host.Settings.EnableCacheVisualization = true;

            ApplicationVersion = GetVersionNumber();

            IsAppJustLaunched = false;
            IsAppJustResumedFromTombStone = false;
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            IsAppJustLaunched = true;
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (e.IsApplicationInstancePreserved)
            {
                //we have everything in memory so just reconnect to server
                StartClient(false);
            }
            else
            {
                IsAppJustResumedFromTombStone = true;
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            IsAppJustLaunched = false;
            IsAppJustResumedFromTombStone = false;
            StopClient();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            IsAppJustLaunched = false;
            IsAppJustResumedFromTombStone = false;
            StopClient();
        }

        public void InitializeAfterFrameComplete()
        {
            SetIdleDetectionSettings();
            SubscribeToLockScreenEvents();
            StartClient();
            StartCounterTimer();
        }

        private void SubscribeToLockScreenEvents()
        {
            if (RootFrame != null)
            {
                RootFrame.Obscured += RootFrame_Obscured;
                RootFrame.Unobscured += RootFrame_Unobscured;
            }
        }

        void RootFrame_Unobscured(object sender, EventArgs e)
        {
            if (_phoneLocked)
            {
                _phoneLocked = false;
                if (!IsRunUnderLockScreenEnabled)
                {
                    StartClient(false);
                }

                StartCounterTimer();
            }
        }

        void RootFrame_Obscured(object sender, ObscuredEventArgs e)
        {
            if (e.IsLocked)
            {
                _phoneLocked = e.IsLocked;
                if (!IsRunUnderLockScreenEnabled)
                {
                    StopClient();
                }

                StopCounterTimer();
            }
        }

        private void StopClient()
        {
            if (_lt != null)
            {
                _lt.Stop(true);
                _lt.Dispose();
                _lt = null;
            }
        }

        private void StartClient(bool showSettingsIfNeeded = true)
        {
            if (_lt != null) return; //prevent from starting twice

            MainViewModel.Clear();

            string user = "", passwd = "";

            if (IsolatedStorageSettings.ApplicationSettings.Contains("username"))
            {
                user = (string)IsolatedStorageSettings.ApplicationSettings["username"];
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("password"))
            {
                passwd = (string)IsolatedStorageSettings.ApplicationSettings["password"];
            }

            if (user != "" && passwd != "")
            {
                MainViewModel.ProgressIsIndeterminate = true;
                MainViewModel.ProgressText = "Connecting";
                MainViewModel.ProgressIsVisible = true;

                _lt = new LiveTiming(user, passwd, false);
                _lt.SystemMessageHandler += new LiveTimingMessageHandlerDelegate(_lt_SystemMessageHandler);
                _lt.CarMessageHandler += new LiveTimingMessageHandlerDelegate(_lt_CarMessageHandler);
                _lt.ControlMessageHandler += _lt_ControlMessageHandler;
                _lt.StartThread();
            }
            else
            {
                //navigate to settings page cos there is no user and/or password set
                if (OnSettingsPageRequest != null)
                {
                    OnSettingsPageRequest.Invoke();
                }
                else
                {
                    NeedSettingsPage = true;
                }
            }
        }

        void _lt_ControlMessageHandler(F1.Messages.IMessage msg)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (MainViewModel.ProgressText.Contains("Connecting"))
                {
                    MainViewModel.ProgressIsVisible = false;
                }

                if (msg is F1.Messages.Control.AuthorizationProblem)
                {
                    MessageBox.Show("Please check credentials or try again later.", "Connection problem", MessageBoxButton.OK);
                }
            });
        }

        public void Reconnect()
        {
            StopClient();
            StartClient();
        }

        public void TryConnect()
        {
            if (_lt == null || (_lt != null && !_lt.IsAlive))
            {
                StopClient();
                StartClient(false);
            }
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            LittleWatson.LogException(e.Exception, ApplicationVersion);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            LittleWatson.LogException(e.ExceptionObject, ApplicationVersion);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            MessageBox.Show("I'm terribly sorry that it happened to you." +
                                " I'll be very grateful if you can send me the crash report next time you run the app.",
                                "Application crashed :(", MessageBoxButton.OK);
        }

        void _lt_CarMessageHandler(F1.Messages.IMessage msg)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (MainViewModel.ProgressText.Contains("Connecting"))
                {
                    MainViewModel.ProgressIsVisible = false;
                }

                F1.Messages.ICarMessage carMsg = (F1.Messages.ICarMessage)msg;
                ViewModel.CarViewModel carViewModel = null;

                int oldPosition = 0;

                try
                {
                    carViewModel = MainViewModel.Cars.First(car => car.CarId == carMsg.CarId);
                    oldPosition = carViewModel.TablePosition;
                }
                catch (InvalidOperationException)
                {
                    //check for it in our invisible car list
                    try
                    {
                        carViewModel = _invisibleCars.First(car => car.CarId == carMsg.CarId);
                        oldPosition = carViewModel.TablePosition;
                    }
                    catch (InvalidOperationException)
                    {
                        //not there so create new
                        carViewModel = new ViewModel.CarViewModel();
                        carViewModel.CarId = carMsg.CarId;
                        carViewModel.Position = 0;
                        carViewModel.TablePosition = 0;
                        _invisibleCars.Add(carViewModel);
                    }
                }

                UpdateCar(carViewModel, carMsg);
                UpdatePositionInList(oldPosition, carViewModel);
            });
        }

        void UpdatePositionInList(int oldPosition, ViewModel.CarViewModel carViewModel)
        {
            if (oldPosition == carViewModel.TablePosition)
            {
                //no change
                return;
            }

            if (oldPosition == 0 && carViewModel.TablePosition > 0)
            {
                int insertPosition = carViewModel.TablePosition - 1;
                //first check if we have other car at that position
                HideAtPosition(insertPosition);

                //now we can insert it at correct place
                InsertAtPosition(insertPosition, carViewModel);

                //remove previous element if any
                _invisibleCars.Remove(carViewModel);
            }
            else if (oldPosition > 0 && carViewModel.TablePosition == 0)
            {
                int pos = oldPosition - 1;

                //move to hidden list
                HideAtPosition(pos);

                //insert blank car at old place
                BlankAtPosition(pos);
            }
            else if (oldPosition > 0 && carViewModel.TablePosition > 0 && oldPosition != carViewModel.TablePosition)
            {
                //change places
                int newPos = carViewModel.TablePosition - 1;
                int oldPos = oldPosition - 1;

                //hide car at new pos
                HideAtPosition(newPos);

                //blank old position
                BlankAtPosition(oldPos);

                //now move to new position
                InsertAtPosition(newPos, carViewModel);
            }
        }

        void InsertAtPosition(int pos, CarViewModel car)
        {
            //now we can insert it at correct place

            //check if we have correct number of slots before inserting
            if (pos >= MainViewModel.Cars.Count)
            {
                for (int items = pos + 1 - MainViewModel.Cars.Count; items > 0; --items)
                {
                    //blank car
                    MainViewModel.Cars.Add(new CarViewModel()
                    {
                        CarId = -1,
                        TablePosition = pos + 1
                    });
                }
            }

            MainViewModel.Cars[pos] = car;
        }

        void BlankAtPosition(int pos)
        {
            try
            {
                //insert blank car at place
                MainViewModel.Cars[pos] = new CarViewModel()
                {
                    CarId = -1,
                    TablePosition = pos + 1
                };
            }
            catch (ArgumentOutOfRangeException)
            {
                //nothing at this position
            }
        }

        void HideAtPosition(int pos)
        {
            try
            {
                CarViewModel car = MainViewModel.Cars.ElementAt(pos);
                if (car.CarId != -1) //if it's -1 then it's just blank car
                {
                    //yes we have so now move it to invisible list
                    car.TablePosition = 0;
                    _invisibleCars.Add(car);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                //no car on this position
            }
        }

        void _lt_SystemMessageHandler(F1.Messages.IMessage msg)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (MainViewModel.ProgressText.Contains("Connecting"))
                {
                    MainViewModel.ProgressIsVisible = false;
                }

                if (msg is F1.Messages.System.Copyright)
                {
                    F1.Messages.System.Copyright system = (F1.Messages.System.Copyright)msg;
                    MainViewModel.Copyright = system.Message;
                }

                if (msg is F1.Messages.System.EventId)
                {
                    F1.Messages.System.EventId eventId = (F1.Messages.System.EventId)msg;
                    MainViewModel.EventType = eventId.EventType;
                }

                if (msg is F1.Messages.System.TrackStatus)
                {
                    F1.Messages.System.TrackStatus trackStatus = (F1.Messages.System.TrackStatus)msg;
                    MainViewModel.TrackStatusColor = trackStatus.Status;
                    MainViewModel.TrackStatusMessage = trackStatus.Message;
                }

                if (msg is F1.Messages.System.Commentary)
                {
                    F1.Messages.System.Commentary commentary = (F1.Messages.System.Commentary)msg;

                    CommentViewModel commentViewModel = new CommentViewModel();
                    commentViewModel.Comment = commentary.Message;
                    MainViewModel.Comments.Add(commentViewModel);
                    MainViewModel.LastComment = commentary.Message;
                }

                if (msg is F1.Messages.System.TimeStamp)
                {
                    F1.Messages.System.TimeStamp time = (F1.Messages.System.TimeStamp)msg;
                    MainViewModel.EventTimeElapsed = time.Time;
                }

                if (msg is F1.Messages.System.WeatherSessionClock)
                {
                    F1.Messages.System.WeatherSessionClock weather = (F1.Messages.System.WeatherSessionClock)msg;

                    if (weather.TimeStr == "")
                    {
                        MainViewModel.AllowTimeEstimation = true;

                        //start the timer
                        if (_timer == null)
                        {
                            _timer = new Timer(SessionTimerCallback, null, 0, 1000);
                        }
                    }
                    else
                    {
                        //stop the timer
                        if (_timer != null)
                        {
                            _timer.Dispose();
                            _timer = null;
                        }

                        //set correct remaining time
                        MainViewModel.AllowTimeEstimation = false;
                        MainViewModel.LastCorrectRemainingTime = weather.Time;
                    }
                }

                if (msg is F1.Messages.System.EndOfSession)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    MainViewModel.AllowTimeEstimation = false;
                    MainViewModel.LastCorrectRemainingTime = TimeSpan.Zero;
                }

                if (msg is F1.Messages.System.WeatherAirTemperature)
                {
                    F1.Messages.System.WeatherAirTemperature weather = (F1.Messages.System.WeatherAirTemperature)msg;
                    MainViewModel.WeatherAirTemperature = weather.Temperature;
                }

                if (msg is F1.Messages.System.WeatherHumidity)
                {
                    F1.Messages.System.WeatherHumidity weather = (F1.Messages.System.WeatherHumidity)msg;
                    MainViewModel.WeatherHumidity = weather.Humidity;
                }

                if (msg is F1.Messages.System.WeatherPressure)
                {
                    F1.Messages.System.WeatherPressure weather = (F1.Messages.System.WeatherPressure)msg;
                    MainViewModel.WeatherPressure = weather.Pressure;
                }

                if (msg is F1.Messages.System.WeatherTrackTemperature)
                {
                    F1.Messages.System.WeatherTrackTemperature weather = (F1.Messages.System.WeatherTrackTemperature)msg;
                    MainViewModel.WeatherTrackTemperature = weather.Temperature;
                }

                if (msg is F1.Messages.System.WeatherWetTrack)
                {
                    F1.Messages.System.WeatherWetTrack weather = (F1.Messages.System.WeatherWetTrack)msg;
                    MainViewModel.WeatherIsWet = weather.IsWet;
                }

                if (msg is F1.Messages.System.WeatherWindDirection)
                {
                    F1.Messages.System.WeatherWindDirection weather = (F1.Messages.System.WeatherWindDirection)msg;
                    MainViewModel.WeatherWindDirection = weather.WindDirection;
                }

                if (msg is F1.Messages.System.WeatherWindSpeed)
                {
                    F1.Messages.System.WeatherWindSpeed weather = (F1.Messages.System.WeatherWindSpeed)msg;
                    MainViewModel.WeatherWindSpeed = weather.Speed;
                }

                if (msg is F1.Messages.System.Speed)
                {
                    F1.Messages.System.Speed speed = (F1.Messages.System.Speed)msg;

                    switch (speed.Column)
                    {
                        case F1.Messages.System.Speed.ColumnType.FastestSector1:
                            {
                                MainViewModel.SpeedSector1.Clear();

                                foreach (var pair in speed.FastestSectors)
                                {
                                    if (MainViewModel.DriversNames.ContainsKey(pair.Driver))
                                    {
                                        pair.Driver = MainViewModel.DriversNames[pair.Driver];
                                    }

                                    MainViewModel.SpeedSector1.Add(new SpeedViewModel() { DriverName = pair.Driver, Speed = pair.Speed });
                                }
                                break;
                            }

                        case F1.Messages.System.Speed.ColumnType.FastestSector2:
                            {
                                MainViewModel.SpeedSector2.Clear();

                                foreach (var pair in speed.FastestSectors)
                                {
                                    if (MainViewModel.DriversNames.ContainsKey(pair.Driver))
                                    {
                                        pair.Driver = MainViewModel.DriversNames[pair.Driver];
                                    }

                                    MainViewModel.SpeedSector2.Add(new SpeedViewModel() { DriverName = pair.Driver, Speed = pair.Speed });
                                }
                                break;
                            }

                        case F1.Messages.System.Speed.ColumnType.FastestSector3:
                            {
                                MainViewModel.SpeedSector3.Clear();

                                foreach (var pair in speed.FastestSectors)
                                {
                                    if (MainViewModel.DriversNames.ContainsKey(pair.Driver))
                                    {
                                        pair.Driver = MainViewModel.DriversNames[pair.Driver];
                                    }

                                    MainViewModel.SpeedSector3.Add(new SpeedViewModel() { DriverName = pair.Driver, Speed = pair.Speed });
                                }
                                break;
                            }

                        case F1.Messages.System.Speed.ColumnType.FastestInTrap:
                            {
                                MainViewModel.SpeedTrap.Clear();

                                foreach (var pair in speed.FastestSectors)
                                {
                                    if (MainViewModel.DriversNames.ContainsKey(pair.Driver))
                                    {
                                        pair.Driver = MainViewModel.DriversNames[pair.Driver];
                                    }

                                    MainViewModel.SpeedTrap.Add(new SpeedViewModel() { DriverName = pair.Driver, Speed = pair.Speed });
                                }
                                break;
                            }

                        case F1.Messages.System.Speed.ColumnType.FastestDriverName:
                            MainViewModel.FastestDriverName = speed.MetaData;
                            break;

                        case F1.Messages.System.Speed.ColumnType.FastestDriverNumber:
                            MainViewModel.FastestDriverNumber = speed.MetaData;
                            break;

                        case F1.Messages.System.Speed.ColumnType.FastestLapNumber:
                            MainViewModel.FastestLapNumber = speed.MetaData;
                            break;

                        case F1.Messages.System.Speed.ColumnType.FastestLapTime:
                            MainViewModel.FastestLapTime = speed.MetaData;
                            break;
                    }
                }
            });
        }

        private void SessionTimerCallback(object state)
        {
            if (IsLiveTimingRunning)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MainViewModel.UpdateEventRemainingTime();
                    }
                );
            }
        }

        private void UpdateCar(ViewModel.CarViewModel car, F1.Messages.ICarMessage msg)
        {
            if (msg is F1.Messages.Car.CarDriver)
            {
                F1.Messages.Car.CarDriver carDriver = (F1.Messages.Car.CarDriver)msg;
                if (carDriver.Name != "")
                {
                    car.DriverName = carDriver.Name;

                    try
                    {
                        MainViewModel.DriversNames.Add(Converters.DriverNameConverter.Shorten(carDriver.Name), carDriver.Name);
                    }
                    catch (ArgumentException)
                    {
                        //such key already exists
                    }
                }
                car.DriverNameColor = GetColor(msg.Colour);
            }

            if (msg is F1.Messages.Car.CarInterval)
            {
                F1.Messages.Car.CarInterval carInt = (F1.Messages.Car.CarInterval)msg;
                car.SetInterval(carInt.Interval, carInt.IntervalType, GetColor(msg.Colour));
            }

            if (msg is F1.Messages.Car.CarLapTime)
            {
                F1.Messages.Car.CarLapTime carLapTime = (F1.Messages.Car.CarLapTime)msg;
                car.SetLapTime(carLapTime.LapTime, carLapTime.LapTimeType, GetColor(msg.Colour));
            }

            if (msg is F1.Messages.Car.CarLapCount)
            {
                F1.Messages.Car.CarLapCount lapCount = (F1.Messages.Car.CarLapCount)msg;
                car.LapCountSet = lapCount.LapCount;
                car.LapCountColor = GetColor(msg.Colour);
            }

            if (msg is F1.Messages.Car.CarPitCount)
            {
                F1.Messages.Car.CarPitCount pitCount = (F1.Messages.Car.CarPitCount)msg;
                car.PitCountSet = pitCount.Count;
                car.PitCountColor = GetColor(msg.Colour);
            }

            if (msg is F1.Messages.Car.CarSectorTime1)
            {
                F1.Messages.Car.CarSectorTime1 sect1 = (F1.Messages.Car.CarSectorTime1)msg;
                car.SetSector1Time(sect1.SectorTime, sect1.SectorTimeType, GetColor(msg.Colour));
            }

            if (msg is F1.Messages.Car.CarSectorTime2)
            {
                F1.Messages.Car.CarSectorTime2 sect2 = (F1.Messages.Car.CarSectorTime2)msg;
                car.SetSector2Time(sect2.SectorTime, sect2.SectorTimeType, GetColor(msg.Colour));
            }

            if (msg is F1.Messages.Car.CarSectorTime3)
            {
                F1.Messages.Car.CarSectorTime3 sect3 = (F1.Messages.Car.CarSectorTime3)msg;
                car.SetSector3Time(sect3.SectorTime, sect3.SectorTimeType, GetColor(msg.Colour));
            }

            if (msg is F1.Messages.Car.CarNumber)
            {
                F1.Messages.Car.CarNumber carNum = (F1.Messages.Car.CarNumber)msg;
                car.CarNumberSet = carNum.Number;
                car.CarNumberColor = GetColor(msg.Colour);
            }

            if (msg is F1.Messages.Car.PracticeBestLapTime)
            {
                F1.Messages.Car.PracticeBestLapTime lapTime = (F1.Messages.Car.PracticeBestLapTime)msg;
                car.SetBestPracticeLapTime(lapTime.BestLap, lapTime.BestLapType, GetColor(msg.Colour));
                car.BestPracticeLapTimeColor = GetColor(lapTime.Colour);
            }

            if (msg is F1.Messages.Car.QualifyPeriodTime1)
            {
                F1.Messages.Car.QualifyPeriodTime1 q1Time = (F1.Messages.Car.QualifyPeriodTime1)msg;
                car.SetQ1Time(q1Time.SectorTime, q1Time.SectorTimeType, GetColor(msg.Colour));
            }

            if (msg is F1.Messages.Car.QualifyPeriodTime2)
            {
                F1.Messages.Car.QualifyPeriodTime2 q2Time = (F1.Messages.Car.QualifyPeriodTime2)msg;
                car.SetQ2Time(q2Time.SectorTime, q2Time.SectorTimeType, GetColor(msg.Colour));
            }

            if (msg is F1.Messages.Car.QualifyPeriodTime3)
            {
                F1.Messages.Car.QualifyPeriodTime3 q3Time = (F1.Messages.Car.QualifyPeriodTime3)msg;
                car.SetQ3Time(q3Time.SectorTime, q3Time.SectorTimeType, GetColor(msg.Colour));
            }


            if (msg is F1.Messages.Car.CarPosition)
            {
                F1.Messages.Car.CarPosition carPos = (F1.Messages.Car.CarPosition)msg;

                car.Position = carPos.Position;

                if (msg.Colour != F1.Messages.CarColours.Unknown)
                {
                    car.PositionColor = GetColor(msg.Colour);
                }
            }

            if (msg is F1.Messages.Car.CarPositionUpdate)
            {
                F1.Messages.Car.CarPositionUpdate carPos = (F1.Messages.Car.CarPositionUpdate)msg;

                if (carPos.Position >= 0)
                {
                    //car.Position = carPos.Position;
                    car.TablePosition = carPos.Position;
                }

                if (msg.Colour != F1.Messages.CarColours.Unknown)
                {
                    car.PositionColor = GetColor(msg.Colour);
                }
            }

            if (msg is F1.Messages.Car.CarGap)
            {
                F1.Messages.Car.CarGap carGap = (F1.Messages.Car.CarGap)msg;
                car.SetGap(carGap.Gap, carGap.GapType);
                car.GapColor = GetColor(msg.Colour);
            }
        }

        private Color GetColor(F1.Messages.CarColours color)
        {
            switch (color)
            {
                case F1.Messages.CarColours.Black:
                    return Colors.Black;

                case F1.Messages.CarColours.Cyan:
                    return Color.FromArgb(255, 0, 255, 255);

                case F1.Messages.CarColours.Green:
                    return Color.FromArgb(255, 0, 255, 0);

                case F1.Messages.CarColours.Grey:
                    return Colors.Gray;

                case F1.Messages.CarColours.Magenta:
                    return Color.FromArgb(255, 255, 0, 255);

                case F1.Messages.CarColours.Red:
                    return Colors.Red;

                case F1.Messages.CarColours.White:
                    return Color.FromArgb(255, 255, 255, 255);

                case F1.Messages.CarColours.Yellow:
                    return Color.FromArgb(255, 255, 255, 0);

                default:
                    return Colors.Orange;
            }
        }

        private void SetIdleDetectionSettings()
        {
            bool preventLock = false, runUnderLock = false;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("preventLock"))
            {
                preventLock = (bool)IsolatedStorageSettings.ApplicationSettings["preventLock"];
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("runUnderLock"))
            {
                runUnderLock = (bool)IsolatedStorageSettings.ApplicationSettings["runUnderLock"];
            }

            PhoneApplicationService.Current.UserIdleDetectionMode = preventLock ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled;

            try
            {
                PhoneApplicationService.Current.ApplicationIdleDetectionMode = runUnderLock ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled;
            }
            catch (InvalidOperationException)
            {
                //this exception is expected here, will try enable it next time
                //MessageBox.Show("You must restart the application for the settings to take effect");
            }
        }

        private static string GetVersionNumber()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var parts = asm.FullName.Split(',');
            if (parts.Length >= 2)
            {
                var parts2 = parts[1].Split('=');
                if (parts2.Length >= 2)
                {
                    return parts2[1];
                }
            }

            return "";
        }

        private void StartCounterTimer()
        {
            if (_counterTimer == null)
            {
                _counterTimer = new Timer(CounterTimerCallback, null, 0, 1000);
            }
        }

        private void StopCounterTimer()
        {
            if (_counterTimer != null)
            {
                _counterTimer.Dispose();
                _counterTimer = null;
            }
        }

        private void CounterTimerCallback(object state)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MainViewModel.UpdateEventStartRemainingTime();
            });
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame() { Background = new SolidColorBrush(Colors.Transparent) };
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
            InitializeAfterFrameComplete();
        }

        #endregion
    }
}
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

using F1;
using LTStoreApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace LTStoreApp
{
    public delegate void OnSettingsPageRequestEventHandler();
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        static MainViewModel _viewModel = null;
        public static MainViewModel MainViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new MainViewModel();
                }
                return _viewModel;
            }
        }

        ILiveTimingApp _lt = null;
        private List<CarViewModel> _invisibleCars = new List<CarViewModel>();

        private Timer _timer = null;
        //private Timer _counterTimer = null;

        public bool IsLiveTimingRunning
        {
            get
            {
                return _lt != null && _lt.IsAlive;
            }
        }   

        public CoreDispatcher Dispatcher { get; set; }
        public static bool NeedSettingsPage { get; set; }
        public static event OnSettingsPageRequestEventHandler OnSettingsPageRequest;

        public bool IsPreventDeviceFromLockScreenEnabled { get; set; }
        bool _appDisplayOn = false;
        DisplayRequest _appDisplayRequest = new DisplayRequest();
        private void RequestDisplay()
        {
            if(!_appDisplayOn)
            {
                try
                {
                    _appDisplayRequest.RequestActive();
                    _appDisplayOn = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        public void ReleaseDisplay()
        {
            if (_appDisplayOn)
            {
                try
                {
                    _appDisplayRequest.RequestRelease();
                    _appDisplayOn = false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;

            Dispatcher = null;
            NeedSettingsPage = false;
            IsPreventDeviceFromLockScreenEnabled = false;

            ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Containers.ContainsKey("Settings"))
            {                
                if (settings.Containers["Settings"].Values.ContainsKey("preventLockScreen"))
                {
                    IsPreventDeviceFromLockScreenEnabled = (bool)settings.Containers["Settings"].Values["preventLockScreen"];
                }
            }
        }

        void App_Resuming(object sender, object e)
        {
            StartClient();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif            

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        public void StartClient()
        {
            if (_lt != null) return;

            MainViewModel.Clear();

            string user = "";
            string password = "";

            ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Containers.ContainsKey("Settings"))
            {
                if (settings.Containers["Settings"].Values.ContainsKey("user"))
                {
                    user = settings.Containers["Settings"].Values["user"].ToString();
                }

                if (settings.Containers["Settings"].Values.ContainsKey("password"))
                {
                    password = settings.Containers["Settings"].Values["password"].ToString();
                }
            }

            if (user != "" && password != "")
            {
                MainViewModel.ProgressIsIndeterminate = true;
                MainViewModel.ProgressIsVisible = true;

                _lt = new LiveTiming(user, password, false);
                _lt.CarMessageHandler += _lt_CarMessageHandler;
                _lt.ControlMessageHandler += _lt_ControlMessageHandler;
                _lt.SystemMessageHandler += _lt_SystemMessageHandler;
                _lt.StartThread();
            }
            else
            {
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
   
        public void StopClient()
        {
            if (_lt != null)
            {
                _lt.Stop(true);
                _lt.Dispose();
                _lt = null;
            }
        }

        void _lt_ControlMessageHandler(F1.Messages.IMessage msg)
        {
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                MainViewModel.ProgressIsVisible = false;

                if(msg.Type == F1.Enums.SystemPacketType.ControlType)
                {
                    MessageDialog dlg = new MessageDialog("Please check credentials or try again later.");
                    await dlg.ShowAsync();
                }
            }); 
        }

        void _lt_CarMessageHandler(F1.Messages.IMessage msg)
        {
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MainViewModel.ProgressIsVisible = false;

                    F1.Messages.ICarMessage carMsg = (F1.Messages.ICarMessage)msg;
                    ViewModels.CarViewModel carViewModel = null;

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
                            carViewModel = new ViewModels.CarViewModel();
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

        #region Position Updates
        void UpdatePositionInList(int oldPosition, ViewModels.CarViewModel carViewModel)
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
        #endregion

        private void UpdateCar(ViewModels.CarViewModel car, F1.Messages.ICarMessage msg)
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

        void _lt_SystemMessageHandler(F1.Messages.IMessage msg)
        {            
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainViewModel.ProgressIsVisible = false;

                if(IsPreventDeviceFromLockScreenEnabled)
                {
                    RequestDisplay();
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

                    ReleaseDisplay();
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
                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MainViewModel.UpdateEventRemainingTime();
                });
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            StopClient();

            deferral.Complete();
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += (s, e) =>
                {
                    SettingsCommand command = new SettingsCommand("settings", "Settings", (handler) =>
                    {
                        MainSettingsFlyout sf = new MainSettingsFlyout();
                        sf.Show();
                    });

                    e.Request.ApplicationCommands.Add(command);
                };

            base.OnWindowCreated(args);
        }
    }
}

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
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using LTLite.Tools;
using Microsoft.Phone.Shell;
using WP7TilesCommon;
using WP7TilesCommon.Data;
using WP7TilesCommon.Tools;

namespace LTLite
{
    public partial class MainPage : PhoneApplicationPage
    {
       public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = App.MainViewModel;

            App.OnSettingsPageRequest += App_OnSettingsPageRequest;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            App.OnSettingsPageRequest -= App_OnSettingsPageRequest;
        }

        void App_OnSettingsPageRequest()
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ApplicationBarMenuItem_Click_Settings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ApplicationBarIconButton_Click_Start(object sender, EventArgs e)
        {
            switch (App.MainViewModel.EventType)
            {
                case F1.Enums.EventType.Race:
                    NavigationService.Navigate(new Uri("/RacePage.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case F1.Enums.EventType.Practice:
                    NavigationService.Navigate(new Uri("/PracticePage.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case F1.Enums.EventType.Qualifying:
                    NavigationService.Navigate(new Uri("/QualifyingPage.xaml", UriKind.RelativeOrAbsolute));
                    break;

                default:
                    MessageBox.Show("No active session.", "Info", MessageBoxButton.OK);
                    break;
            }
        }

        private void ApplicationBarIconButton_Click_Reset(object sender, EventArgs e)
        {
            (App.Current as LTLite.App).Reconnect();
        }

        private void ApplicationBarMenuItem_Click_About(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (App.IsAppJustLaunched || App.IsAppJustResumedFromTombStone)
            {
                if (App.IsAppJustLaunched) App.IsAppJustLaunched = false;
                if (App.IsAppJustResumedFromTombStone) App.IsAppJustResumedFromTombStone = false;

                BackgroundTileUpdater.UpdateTiles(new OnTileUpdateCompletedEventHandler(OnTileUpdateComplete));
                BackgroundAgentRescheduler.RescheduleAgent();
                LittleWatson.CheckForPreviousException();

                if (App.NeedSettingsPage)
                {
                    App.NeedSettingsPage = false;
                    NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void OnTileUpdateComplete()
        {
            RacingEvent data;
            if (DataSerializer.Deserialize(out data))
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        data.UpdateRemainingTime();

                        App.MainViewModel.Circuit = data.Country + " " + data.Circuit;
                        App.MainViewModel.Session = data.Session;
                        App.MainViewModel.EventTimeStamp = data.DownloadTimestamp;
                        App.MainViewModel.EventStartTime = data.SessionStartTime;
                        App.MainViewModel.IsSessionLive = data.IsSessionLive;

                        TimeSpan left = TimeSpan.FromSeconds(data.RemainingTime);
                        App.MainViewModel.EventStartRemainingTime = left;
                        App.MainViewModel.CounterDays = left.Days;
                        App.MainViewModel.CounterHours = left.Hours;
                        App.MainViewModel.CounterMinutes = left.Minutes;
                        App.MainViewModel.CounterSeconds = left.Seconds;
                    });
            }
        }

        private void ApplicationBarIconButton_Click_ScrollDown(object sender, EventArgs e)
        {
            MainListBox.ScrollToBottom();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Pivot)sender).SelectedIndex == 1)
            {
                ApplicationBar = ((ApplicationBar)Resources["AppBarComments"]);
            }
            else
            {
                ApplicationBar = ((ApplicationBar)Resources["AppBar"]);
            }
        }
    }
}
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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using WP7TilesCommon.Tools;

namespace LTLite
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            bool incomplete = false;

            try
            {
                incomplete = State.ContainsKey("incomplete");
                if (incomplete)
                {
                    //restore incomplete data
                    user.Text = (string)State["username"];
                    preventLock.IsChecked = (bool)State["preventLock"];
                    runUnderLock.IsChecked = (bool)State["runUnderLock"];
                }
            }
            catch (InvalidOperationException)
            {
                //this is strange but sometimes it throws:
                //"You can only use State between OnNavigatedTo and OnNavigatedFrom"
            }

            if (!incomplete)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("username"))
                {
                    user.Text = (string)IsolatedStorageSettings.ApplicationSettings["username"];
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains("password"))
                {
                    password.Password = (string)IsolatedStorageSettings.ApplicationSettings["password"];
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains("preventLock"))
                {
                    preventLock.IsChecked = (bool)IsolatedStorageSettings.ApplicationSettings["preventLock"];
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains("runUnderLock"))
                {
                    runUnderLock.IsChecked = (bool)IsolatedStorageSettings.ApplicationSettings["runUnderLock"];
                }
            }
        }

        private void ApplicationBarIconButton_Click_Ok(object sender, EventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["username"] = user.Text;
            IsolatedStorageSettings.ApplicationSettings["password"] = password.Password;

            IsolatedStorageSettings.ApplicationSettings["preventLock"] = preventLock.IsChecked;
            IsolatedStorageSettings.ApplicationSettings["runUnderLock"] = runUnderLock.IsChecked;

            IsolatedStorageSettings.ApplicationSettings.Save();

            PhoneApplicationService.Current.UserIdleDetectionMode = (bool)preventLock.IsChecked ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled;

            try
            {
                PhoneApplicationService.Current.ApplicationIdleDetectionMode = (bool)runUnderLock.IsChecked ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("You must restart the application to turn off running under lock screen setting.");
            }

            if (user.Text != "" && password.Password != "")
            {
                (App.Current as LTLite.App).TryConnect();
            }

            NavigationService.GoBack();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode != NavigationMode.Back && e.NavigationMode != NavigationMode.Forward)
            {
                try
                {
                    State["incomplete"] = true;
                    State["username"] = user.Text;
                    State["preventLock"] = preventLock.IsChecked;
                    State["runUnderLock"] = runUnderLock.IsChecked;
                }
                catch (InvalidOperationException)
                {
                    //this is strange but sometimes it throws:
                    //"You can only use State between OnNavigatedTo and OnNavigatedFrom"
                }
            }
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask ie = new WebBrowserTask();
            ie.Uri = new Uri(@"http://www.formula1.com/reg/registration");
            ie.Show();
        }

        private void ApplicationBarMenuItem_Click_ClearCache(object sender, EventArgs e)
        {
            DataSerializer.ClearCache();
            MessageBox.Show("Cache cleared", "Info", MessageBoxButton.OK);
        }
    }
}
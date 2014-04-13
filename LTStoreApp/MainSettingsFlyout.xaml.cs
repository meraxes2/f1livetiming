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
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace LTStoreApp
{
    public sealed partial class MainSettingsFlyout : SettingsFlyout
    {
        string _oldUsername = "";
        string _oldPassword = "";

        public MainSettingsFlyout()
        {
            this.InitializeComponent();
            Loaded += MainSettingsFlyout_Loaded;
            Unloaded += MainSettingsFlyout_Unloaded;
        }

        void MainSettingsFlyout_Unloaded(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!settings.Containers.ContainsKey("Settings"))
            {
                settings.CreateContainer("Settings", ApplicationDataCreateDisposition.Always);
            }

            if (settings.Containers["Settings"].Values.ContainsKey("user"))
            {
                settings.Containers["Settings"].Values["user"] = user.Text;
            }
            else
            {
                settings.Containers["Settings"].Values.Add("user", user.Text);
            }

            if (settings.Containers["Settings"].Values.ContainsKey("password"))
            {
                settings.Containers["Settings"].Values["password"] = password.Password;
            }
            else
            {
                settings.Containers["Settings"].Values.Add("password", password.Password);
            }

            if (settings.Containers["Settings"].Values.ContainsKey("preventLockScreen"))
            {
                settings.Containers["Settings"].Values["preventLockScreen"] = preventFromLockScreen.IsOn;
            }
            else
            {
                settings.Containers["Settings"].Values.Add("preventLockScreen", preventFromLockScreen.IsOn);
            }

            if (settings.Containers["Settings"].Values.ContainsKey("estimateData"))
            {
                settings.Containers["Settings"].Values["estimateData"] = estimateData.IsOn;
            }
            else
            {
                settings.Containers["Settings"].Values.Add("estimateData", estimateData.IsOn);
            }

            App.MainViewModel.IsDataEstimationEnabled = estimateData.IsOn;

            var app = App.Current as App;
            app.IsPreventDeviceFromLockScreenEnabled = preventFromLockScreen.IsOn;
            if(!app.IsPreventDeviceFromLockScreenEnabled)
            {
                app.ReleaseDisplay();
            }

            if (!app.IsLiveTimingRunning && user.Text != "" && password.Password != ""
                && (_oldUsername != user.Text || _oldPassword != password.Password))
            {
                app.StopClient();
                app.StartClient();
            }
        }

        void MainSettingsFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Containers.ContainsKey("Settings"))
            {
                if (settings.Containers["Settings"].Values.ContainsKey("user"))
                {
                    user.Text = _oldUsername = settings.Containers["Settings"].Values["user"].ToString();
                }

                if (settings.Containers["Settings"].Values.ContainsKey("password"))
                {
                    password.Password = _oldPassword = settings.Containers["Settings"].Values["password"].ToString();
                }

                if (settings.Containers["Settings"].Values.ContainsKey("preventLockScreen"))
                {
                    preventFromLockScreen.IsOn = (bool)settings.Containers["Settings"].Values["preventLockScreen"];
                }

                if (settings.Containers["Settings"].Values.ContainsKey("estimateData"))
                {
                    estimateData.IsOn = (bool)settings.Containers["Settings"].Values["estimateData"];
                }
            }
        }
    }
}

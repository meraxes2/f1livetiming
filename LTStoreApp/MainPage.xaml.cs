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

using LTStoreApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LTStoreApp.Tools;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LTStoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public bool LockToBottom { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            LockToBottom = true;
            DataContext = App.MainViewModel;

            SizeChanged += MainPage_SizeChanged;
        }       

        void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1000 && e.NewSize.Height < 1000)
            {
                VisualStateManager.GoToState(this, "NarrowView", true);
            }
            else if(e.NewSize.Width < e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "PortraitView", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLandscapeView", true);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App app = App.Current as App;
            app.Dispatcher = Dispatcher;
            app.StartClient();            
        }

        private void MenuFlyoutItem_Comments_Click(object sender, RoutedEventArgs e)
        {
            App.MainViewModel.TableType = Enums.TableType.CommentsTable;
        }

        private void MenuFlyoutItem_LiveTiming_Click(object sender, RoutedEventArgs e)
        {
            App.MainViewModel.TableType = Enums.TableType.LiveTimingTable;
        }        

        private void AppBarButton_ReconnectClick(object sender, RoutedEventArgs e)
        {
            App app = App.Current as App;
            app.StopClient();
            app.StartClient();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.OnSettingsPageRequest += App_OnSettingsPageRequest;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            App.OnSettingsPageRequest -= App_OnSettingsPageRequest;
        }

        void App_OnSettingsPageRequest()
        {
            MainSettingsFlyout sf = new MainSettingsFlyout();
            sf.Show();
        }
    }
}

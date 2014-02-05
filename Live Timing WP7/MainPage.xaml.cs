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

using System.Windows;
using Microsoft.Phone.Controls;
using F1;

namespace Live_Timing_WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        ILiveTimingApp _lt = null;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void connectButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (_lt != null)
            {
                _lt.Stop(true);
                _lt.Dispose();
                _lt = null;

                listBox1.Items.Clear();
            }

            _lt = new LiveTiming(user.Text, password.Password, false);
            _lt.CarMessageHandler += _lt_CarMessageHandler;
            _lt.SystemMessageHandler += _lt_SystemMessageHandler;
            _lt.ControlMessageHandler += _lt_ControlMessageHandler;
            _lt.StartThread();
        }

        void _lt_ControlMessageHandler(F1.Messages.IMessage msg)
        {
            
        }

        void _lt_SystemMessageHandler(F1.Messages.IMessage msg)
        {
            Dispatcher.BeginInvoke(() =>
            {
                listBox1.Items.Add(msg.ToString());
            });
        }

        void _lt_CarMessageHandler(F1.Messages.IMessage msg)
        {
            Dispatcher.BeginInvoke(() =>
            {
                listBox1.Items.Add(msg.ToString());
            });
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_lt != null)
            {
                _lt.Stop(true);
                _lt.Dispose();
                _lt = null;
            }

            base.OnNavigatedFrom(e);
        }
    }
}
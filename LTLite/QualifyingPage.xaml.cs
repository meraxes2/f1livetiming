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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace LTLite
{
    public partial class QualifyingPage : PhoneApplicationPage
    {
        public QualifyingPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = App.MainViewModel;
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.PortraitUp ||
                e.Orientation == PageOrientation.PortraitDown ||
                e.Orientation == PageOrientation.Portrait)
            {
                LayoutLandscape.Visibility = Visibility.Collapsed;
                LayoutPortrait.Visibility = Visibility.Visible;
            }
            else
            {
                LayoutPortrait.Visibility = Visibility.Collapsed;
                LayoutLandscape.Visibility = Visibility.Visible;
            }

            base.OnOrientationChanged(e);
        }
    }
}
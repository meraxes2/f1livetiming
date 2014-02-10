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

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;

namespace LTLite
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            emailTextBlock.Text = "andrej";
            emailTextBlock.Text += "360";
            emailTextBlock.Text += "@";
            emailTextBlock.Text += "gmail";
            emailTextBlock.Text += ".com";
        }

        private void TextBlock_Tap_Email(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = "andrej";
            email.To += "360";
            email.To += "@";
            email.To += "gmail";
            email.To += ".com";
            email.Subject = "LTLite feedback";
            email.Show();
        }

        private void TextBlock_Tap_Toolkit(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask ie = new WebBrowserTask();
            ie.Uri = new Uri(@"http://phone.codeplex.com");
            ie.Show();
        }

        private void TextBlock_Tap_F1livetiming(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask ie = new WebBrowserTask();
            ie.Uri = new Uri(@"http://livetiming.turnitin.co.uk/");
            ie.Show();
        }

        private void TextBlock_Tap_apache(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask ie = new WebBrowserTask();
            ie.Uri = new Uri(@"http://www.apache.org/licenses/LICENSE-2.0");
            ie.Show();
        }
    }
}
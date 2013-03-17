using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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
            _lt.StartThread();
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
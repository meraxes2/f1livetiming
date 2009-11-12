using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Live_Timing_Viewer
{
	public partial class Launcher : Window
	{
		public Launcher()
		{
			InitializeComponent();
		}


	    private void CommandBinding_OnClose(object sender, ExecutedRoutedEventArgs e)
	    {
	        Close();
	    }


	    private void OnLaunch_Live(object sender, RoutedEventArgs e)
	    {
            _launchLiveButton.IsEnabled = false;

            if (String.IsNullOrEmpty(_userNameEdit.Text) ||
                String.IsNullOrEmpty(_passwordEdit.Text))
            {
                MessageBox.Show("Please enter a valid username and password.",
                                "Invalid username and/or password",
                                MessageBoxButton.OK,
                                MessageBoxImage.Hand);

            }
            else
            {
                App theApp = (App)App.Current;

                if (theApp.BeginLiveTiming(_userNameEdit.Text, _passwordEdit.Text))
                {
                    // go next
                }
                else
                {
                    _connectionErrorMessage.Visibility = Visibility.Visible;
                }
            }

            _launchLiveButton.IsEnabled = true;
	    }


	    private void OnLaunch_Simulator(object sender, RoutedEventArgs e)
	    {
            if (String.IsNullOrEmpty(_filePathDisplay.Text) ||
                !File.Exists(_filePathDisplay.Text))
            {
                MessageBox.Show("Please enter a simulator file.",
                                "No or incorrect file specified",
                                MessageBoxButton.OK,
                                MessageBoxImage.Hand);

                return;
            }
	    }


	    private void OnBrowse_Simulator(object sender, RoutedEventArgs e)
	    {
	        
	    }
	}
}
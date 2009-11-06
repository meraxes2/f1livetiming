using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AppCore;

namespace Live_Timing_Viewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

		    LiveTimingDataProvider prov =
		        (LiveTimingDataProvider) ((ObjectDataProvider) timingGrid.Resources["liveTimingData"]).ObjectInstance;
		}
	}
}
using System.Windows.Controls;
using AppCore;

namespace Live_Timing_Viewer
{
	/// <summary>
	/// Interaction logic for TimingGrid.xaml
	/// </summary>
	public partial class TimingGrid : UserControl
	{
		public TimingGrid()
		{
            InitializeComponent();

            LiveTimingDataProvider provider = FindResource("liveTimingData") as LiveTimingDataProvider;

            App theApp = App.Current as App;

            provider.GoLive(theApp.LiveTiming);
        }
	}
}
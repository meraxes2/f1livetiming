using System.Windows;
using F1;

namespace Live_Timing_Viewer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        public ILiveTimingApp LiveTiming { get; private set; }

        public bool BeginLiveTiming(string username, string password)
        {
            bool ret = true;

            try
            {
                LiveTiming = new F1.LiveTiming(username, password, true);
            }
            catch (F1.Exceptions.AuthorizationException)
            {
                ret = false;
            }
            catch (F1.Exceptions.ConnectionException)
            {
                ret = false;
            }

            return ret;
        }


        public void Terminate()
        {
            LiveTiming.Dispose();
        }
    }
}
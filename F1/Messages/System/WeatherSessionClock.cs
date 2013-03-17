using System;
using System.Text;

namespace F1.Messages.System
{
    public class WeatherSessionClock : WeatherMessage
    {
        public string TimeStr
        {
            get
            {
                return BaseData;
            }
        }

        public TimeSpan Time { get; private set; }

        protected override void OnDeserialiseComplete()
        {
            double time = DeserialiseTime();

            if (Double.IsNaN(time))
            {
                Time = TimeSpan.Zero;
            }
            else
            {
                Time = TimeSpan.FromSeconds(time);
            }
        }

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherSessionClock - TimeStr: {0}.", TimeStr);
        }
    }
}

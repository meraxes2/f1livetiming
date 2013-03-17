using System;
using System.Text;

namespace F1.Messages.System
{
    public class WeatherTrackTemperature: WeatherMessage
    {
        public int Temperature { get; private set; }

        protected override void OnDeserialiseComplete()
        {
            Temperature = base.DeserialiseInteger();
        }

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherTrackTemperature - Temperature: {0}.", Temperature);
        }
    }
}

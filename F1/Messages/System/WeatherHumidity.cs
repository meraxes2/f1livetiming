using System;
using System.Text;

namespace F1.Messages.System
{
    public class WeatherHumidity : WeatherMessage
    {
        public int Humidity { get; private set; }

        protected override void OnDeserialiseComplete()
        {
            Humidity = base.DeserialiseInteger();
        }

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherHumidity - Humidity: {0}.", Humidity);
        }
    }
}

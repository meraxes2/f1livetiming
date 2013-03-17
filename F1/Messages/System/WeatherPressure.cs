using System;
using System.Text;

namespace F1.Messages.System
{
    public class WeatherPressure : WeatherMessage
    {
        public double Pressure { get; private set; }

        protected override void OnDeserialiseComplete()
        {
            Pressure = base.DeserialiseDouble();
        }

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherPressure - Pressure: {0}.", Pressure);
        }
    }
}

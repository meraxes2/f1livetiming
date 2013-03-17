using System;
using System.Text;

namespace F1.Messages.System
{
    public class WeatherWindSpeed : WeatherMessage
    {
        public double Speed { get; private set; }

        protected override void OnDeserialiseComplete()
        {
            Speed = base.DeserialiseDouble();
        }

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherWindSpeed - Speed: {0}.", Speed);
        }
    }
}

using System;
using System.Text;

namespace F1.Messages.System
{
    public class WeatherWindDirection : WeatherMessage
    {
        public int WindDirection { get; private set; }

        protected override void OnDeserialiseComplete()
        {
            //data is integer - value seems to be between 0 - 359 (degrees?)
            WindDirection = base.DeserialiseInteger();
        }

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherWindDirection - WindDirection: {0}.", WindDirection);
        }
    }
}

using System;
using System.Text;

namespace F1.Messages.System
{
    public class WeatherWetTrack : WeatherMessage
    {
        public Boolean IsWet { get; private set; }

        protected override void OnDeserialiseComplete()
        {
            IsWet = base.DeserialiseInteger() > 0 ? true : false;
        }

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherWetTrack - IsWet: {0}.", IsWet);
        }
    }
}

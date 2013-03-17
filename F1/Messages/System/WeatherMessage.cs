using System;
using System.Text;
using F1.Data.Packets;
using F1.Enums;
using System.IO;
using Common.Utils.Strings;
using System.Globalization;

namespace F1.Messages.System
{
    public class WeatherMessage : IMessage
    {
        protected WeatherType DetailedType { get; private set; }
        protected string BaseData { get; private set; }

        #region IMessage Members

        public Packet CreatePacketType(Header header, Stream rawStream, Stream decryptStream)
        {
            return new ShortPacket(header, decryptStream);
        }

        public void Deserialise(Header header, Packet payload)
        {
            ShortPacket sp = (ShortPacket)payload;

            DetailedType = (WeatherType)sp.ShortDatum;
            BaseData = (sp.Data == null) ? String.Empty : StringUtils.ASCIIBytesToString(sp.Data);

            OnDeserialiseComplete();
        }

        public SystemPacketType Type
        {
            get { return SystemPacketType.Weather; }
        }

        #endregion

        protected virtual void OnDeserialiseComplete()
        {
        }

        #region Helpers and Decoders

        protected int DeserialiseInteger()
        {
            if (String.IsNullOrEmpty(BaseData))
            {
                return -1;
            }

            return int.Parse(BaseData, CultureInfo.InvariantCulture);
        }

        protected double DeserialiseTime()
        {
            if (String.IsNullOrEmpty(BaseData))
            {
                return double.NaN;
            }

            string[] datums = BaseData.Split(':');

            double timeSecs = 0.0;

            if (datums.Length >= 3)
            {
                // hours
                timeSecs += double.Parse(datums[datums.Length - 3], CultureInfo.InvariantCulture) * 3600.0;
            }
            if (datums.Length >= 2)
            {
                // mins
                timeSecs += double.Parse(datums[datums.Length - 2], CultureInfo.InvariantCulture) * 60;
            }
            if (datums.Length >= 1)
            {
                // seconds and fraction of a second
                timeSecs += double.Parse(datums[datums.Length - 1], CultureInfo.InvariantCulture);
            }

            return timeSecs;
        }

        protected double DeserialiseDouble()
        {
            if (String.IsNullOrEmpty(BaseData))
            {
                return double.NaN;
            }

            return double.Parse(BaseData, CultureInfo.InvariantCulture);
        }

        #endregion

        public override string ToString()
        {
            return String.Format("SystemMessage: WeatherMessage: Unknown -  BaseData: {0}.", BaseData);
        }
    }
}

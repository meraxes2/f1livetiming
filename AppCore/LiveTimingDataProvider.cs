using System;
using System.Data;
using AppCore.Schemas;

namespace AppCore
{
    public class LiveTimingDataProvider
    {
        private readonly MainStateDS _data;

        public LiveTimingDataProvider()
        {
            _data = new MainStateDS();

            for (int i = 0; i < 20; i++)
            {
                _data.Main.AddMainRow(i, i, "Driver " + (i + 1), Math.Round(Math.Sin(i), 2), i * 0.5, 2.33 + Math.Round(Math.Cos(i),2 ), 22 + Math.Round(Math.Sin(i),2),
                                   23 + Math.Round(Math.Cos(i),2), 24 + Math.Round(Math.Sin(i),2), 1);
            }

            _data.AcceptChanges();
        }


        public DataView DefaultView
        {
            get
            {
                return _data.Main.DefaultView;
            }
        }
    }
}

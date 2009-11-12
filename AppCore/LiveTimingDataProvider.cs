using System;
using System.Data;
using AppCore.Schemas;
using F1;
using F1.Messages;
using F1.Messages.Car;
using System.Linq;

namespace AppCore
{
    public class LiveTimingDataProvider
    {
        private readonly MainStateDS _data;
        private ILiveTimingApp _app;

        public LiveTimingDataProvider()
        {
            _data = new MainStateDS();

            // Build display data for designer
            for (int i = 0; i < 20; i++)
            {
                _data.Main.AddMainRow(i+1, i, "Driver " + (i + 1), Math.Round(Math.Sin(i), 2), i * 0.5, 2.33 + Math.Round(Math.Cos(i),2 ), 22 + Math.Round(Math.Sin(i),2),
                                   23 + Math.Round(Math.Cos(i),2), 24 + Math.Round(Math.Sin(i),2), 1);
            }

            _data.AcceptChanges();
        }


        public void GoLive( ILiveTimingApp app )
        {
            app.CarMessageHandler += this.CarMessageHandler;

            _data.Main.DefaultView.Sort = "Position";   

            _app = app;
        }


        public DataView DefaultView
        {
            get
            {
                return _data.Main.DefaultView;
            }
        }


        private F1.Enums.EventType CurrentEventType
        {
            get
            {
                return _app.CurrentEventType;
            }
        }


        private void CarMessageHandler(F1.Messages.IMessage msg)
        {
            CarBaseMessage carMsg = (CarBaseMessage)msg;

            MainStateDS.MainRow row = GetCarRow(carMsg.CarId);

            switch (CurrentEventType)
            {
                case F1.Enums.EventType.NoEvent:
                    break;
                case F1.Enums.EventType.Practice:
                    DispatchPracticeHAndler(carMsg, row);
                    break;
                case F1.Enums.EventType.Qualifying:
                    DispatchQualyHAndler(carMsg, row);
                    break;
                case F1.Enums.EventType.Race:
                    DispatchRaceHAndler(carMsg, row);
                    break;
            }
        }


        private void DispatchRaceHAndler(CarBaseMessage msg, MainStateDS.MainRow row)
        {
            switch ((F1.Enums.RaceCarType)msg.CarType)
            {
                case F1.Enums.RaceCarType.RaceDriver:
                    row.Driver = (msg as CarDriver).Name;
                    break;
                case F1.Enums.RaceCarType.RaceGap:
                    row.Gap = (msg as CarGap).Gap;
                    break;
                case F1.Enums.RaceCarType.RaceInterval:
                    row.Interval = (msg as CarInterval).Interval;
                    break;
                case F1.Enums.RaceCarType.RaceLapTime:
                    row.LapTime = (msg as CarLapTime).LapTime;
                    break;
                case F1.Enums.RaceCarType.RacePitCount:
                    row.Pits = (msg as CarPitCount).Count;
                    break;
                case F1.Enums.RaceCarType.RacePosition:
                    row.Position = (msg as CarPosition).Position;
                    break;
                case F1.Enums.RaceCarType.RaceSector_1:
                    row.Sector_1 = (msg as CarSectorTime).SectorTime;
                    break;
                case F1.Enums.RaceCarType.RaceSector_2:
                    row.Sector_2 = (msg as CarSectorTime).SectorTime;
                    break;
                case F1.Enums.RaceCarType.RaceSector_3:
                    row.Sector_3 = (msg as CarSectorTime).SectorTime;
                    break;
                case F1.Enums.RaceCarType.RacePitLap_1:
                case F1.Enums.RaceCarType.RacePitLap_2:
                case F1.Enums.RaceCarType.RacePitLap_3:
                case F1.Enums.RaceCarType.RaceNumber:
                    break;
            }
        }


        private void DispatchQualyHAndler(CarBaseMessage msg, MainStateDS.MainRow row)
        {
            switch ((F1.Enums.QualifyCarType)msg.CarType)
            {
                default:
                    break;
            }
        }


        private void DispatchPracticeHAndler(CarBaseMessage msg, MainStateDS.MainRow row)
        {
            switch ((F1.Enums.PracticeCarType)msg.CarType)
            {
                default:
                    break;
            }
        }


        private MainStateDS.MainRow GetCarRow(int carId)
        {
            return _data.Main.FindByCarID(carId);
        }
    }
}

/*
 *  f1livetiming - Part of the Live Timing Library for .NET
 *  
 *      http://livetiming.turnitin.co.uk/
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  you may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at 
 *  
 *      http://www.apache.org/licenses/LICENSE-2.0 
 *  
 *  Unless required by applicable law or agreed to in writing, software 
 *  distributed under the License is distributed on an "AS IS" BASIS, 
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 *  See the License for the specific language governing permissions and 
 *  limitations under the License. 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1.Messages.Car;
using Windows.UI;
using System.Globalization;

namespace LTStoreApp.ViewModels
{
    public class CarViewModel : INotifyPropertyChanged, IEquatable<CarViewModel>
    {
        public CarViewModel()
        {
            IsDataEstimationEnabled = false;
        }

        #region Car id
        int _carId;
        public int CarId
        {
            get
            {
                return _carId;
            }
            set
            {
                if (value != _carId)
                {
                    _carId = value;
                    NotifyPropertyChanged("CarId");
                }
            }
        } 
        #endregion

        #region Driver name
        string _driverName = "";
        public string DriverName
        {
            get
            {            
                return _driverName;
            }
            set
            {
                if (value != _driverName)
                {
                    _driverName = value;
                    NotifyPropertyChanged("DriverName");
                }
            }
        }

        Color _driverNameColor = Colors.Orange;
        public Color DriverNameColor
        {
            get
            {
                return _driverNameColor;
            }
            set
            {
                if (value != _driverNameColor)
                {
                    _driverNameColor = value;
                    NotifyPropertyChanged("DriverNameColor");
                }
            }
        }
        #endregion

        #region Gap
        double _gap = 0.0;
        CarBaseMessage.TimeType _gapType = CarBaseMessage.TimeType.NoData;

        public void SetGap(double gap, CarBaseMessage.TimeType type)
        {
            _gap = gap;
            _gapType = type;
            NotifyPropertyChanged("Gap");
        }

        public string Gap
        {
            get
            {
                switch (_gapType)
                {
                    case CarBaseMessage.TimeType.InPit:
                        return "PIT";

                    case CarBaseMessage.TimeType.Lapped:
                        return "LAP";

                    case CarBaseMessage.TimeType.NLaps:
                        return _gap.ToString("##L", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Out:
                        return "OUT";

                    case CarBaseMessage.TimeType.Retired:
                        return "DNF";

                    case CarBaseMessage.TimeType.Stop:
                        return "STOP";

                    case CarBaseMessage.TimeType.Time:
                        return _gap.ToString("#0.0", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.NoData:
                    default:
                        return "";
                }
            }
        }

        Color _gapColor = Colors.Orange;
        public Color GapColor
        {
            get
            {
                return _gapColor;
            }
            set
            {
                if (value != _gapColor)
                {
                    _gapColor = value;
                    NotifyPropertyChanged("GapColor");
                }
            }
        }

        public double GapRaw
        {
            get { return _gap; }
        }

        public CarBaseMessage.TimeType GapType
        {
            get
            {
                return _gapType;
            }
        }
        #endregion

        #region Interval
        double _interval;
        CarBaseMessage.TimeType _intervalType;

        public void SetInterval(double interval, CarBaseMessage.TimeType type, Color color)
        {
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_intervalColor == color)
                {
                    _intervalType = type;
                    NotifyPropertyChanged("Interval");
                }
            }
            else
            {
                _interval = interval;
                _intervalType = type;
                NotifyPropertyChanged("Interval");
            }

            IntervalColor = color;
        }

        public string Interval
        {
            get
            {
                switch (_intervalType)
                {
                    case CarBaseMessage.TimeType.Time:
                        if (_position == 1)
                        {
                            //strange behavor
                            return _interval.ToString("##", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            return _interval.ToString("#0.0", CultureInfo.InvariantCulture);
                        }

                    case CarBaseMessage.TimeType.Lapped:
                        return _interval.ToString("##", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.NLaps:
                        return _interval.ToString("##L", CultureInfo.InvariantCulture);

                    default:
                        return "";
                }
            }
            private set
            {

            }
        }

        Color _intervalColor = Colors.Orange;
        public Color IntervalColor
        {
            get
            {
                return _intervalColor;
            }
            set
            {
                if (value != _intervalColor)
                {
                    _intervalColor = value;
                    NotifyPropertyChanged("IntervalColor");
                }
            }
        }

        public double IntervalRaw
        {
            get { return _interval; }
        }

        public CarBaseMessage.TimeType IntervalType
        {
            get
            {
                return _intervalType;
            }
        }

        #endregion

        #region Lap count
        int _lapCount = -1;
        public string LapCount
        {
            get
            {
                if (_lapCount != -1)
                {
                    return _lapCount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    return "";
                }
            }
        }

        public int LapCountSet
        {
            set
            {
                if (value != _lapCount)
                {
                    _lapCount = value;
                    NotifyPropertyChanged("LapCount");
                }
            }
        }

        Color _lapCountColor = Colors.Orange;
        public Color LapCountColor
        {
            get
            {
                return _lapCountColor;
            }
            set
            {
                if (value != _lapCountColor)
                {
                    _lapCountColor = value;
                    NotifyPropertyChanged("LapCountColor");
                }
            }
        }
        #endregion

        #region Lap time
        double _lapTime;
        CarBaseMessage.TimeType _lapTimeType;
        public void SetLapTime(double lapTime, CarBaseMessage.TimeType type, Color color)
        {            
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_lapTimeColor == color)
                {
                    _lapTimeType = type;
                    NotifyPropertyChanged("LapTime");
                }
            }
            else
            {
                _lapTime = lapTime;
                _lapTimeType = type;
                NotifyPropertyChanged("LapTime");
            }

            LapTimeColor = color;
        }

        public string LapTime
        {
            get
            {
                switch (_lapTimeType)
                {
                    case CarBaseMessage.TimeType.Time:
                        return TimeSpan.FromSeconds(_lapTime).ToString(@"m\:ss\.fff", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Out:
                        return "OUT";

                    case CarBaseMessage.TimeType.InPit:
                        return "IN PIT";

                    case CarBaseMessage.TimeType.Retired:
                        return "RETIRED";

                    default:
                        return "";
                }
            }

            private set
            {

            }
        }

        public CarBaseMessage.TimeType LapTimeType
        {
            get
            {
                return _lapTimeType;
            }
        }

        Color _lapTimeColor = Colors.Orange;
        public Color LapTimeColor
        {
            get
            {
                return _lapTimeColor;
            }
            set
            {
                if (value != _lapTimeColor)
                {
                    _lapTimeColor = value;
                    NotifyPropertyChanged("LapTimeColor");
                }
            }
        }
        #endregion

        #region Best practice lap time
        double _bestPracticeLapTime;
        CarBaseMessage.TimeType _bestPracticeLapTimeType;
        public void SetBestPracticeLapTime(double lapTime, CarBaseMessage.TimeType type, Color color)
        {
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_bestPracticeLapTimeColor == color)
                {
                    _bestPracticeLapTimeType = type;
                    NotifyPropertyChanged("BestPracticeLapTime");
                }
            }
            else
            {
                _bestPracticeLapTime = lapTime;
                _bestPracticeLapTimeType = type;
                NotifyPropertyChanged("BestPracticeLapTime");
            }

            BestPracticeLapTimeColor = color;
        }

        public string BestPracticeLapTime
        {
            get
            {
                switch (_bestPracticeLapTimeType)
                {
                    case CarBaseMessage.TimeType.Time:
                        return TimeSpan.FromSeconds(_bestPracticeLapTime).ToString(@"m\:ss\.fff", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Out:
                        return "OUT";

                    case CarBaseMessage.TimeType.InPit:
                        return "IN PIT";

                    case CarBaseMessage.TimeType.Retired:
                        return "RETIRED";

                    default:
                        return "";
                }
            }

            private set
            {

            }
        }

        Color _bestPracticeLapTimeColor = Colors.Orange;
        public Color BestPracticeLapTimeColor
        {
            get
            {
                return _bestPracticeLapTimeColor;
            }
            set
            {
                if (value != _bestPracticeLapTimeColor)
                {
                    _bestPracticeLapTimeColor = value;
                    NotifyPropertyChanged("BestPracticeLapTimeColor");
                }
            }
        }
        #endregion

        #region Car number
        int _carNumber = -1;
        public string CarNumber
        {
            get
            {
                if (_carNumber != -1)
                {
                    return _carNumber.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    return "";
                }
            }
        }

        public int CarNumberSet
        {
            set
            {
                if (value != _carNumber && value != -1)
                {
                    _carNumber = value;
                    NotifyPropertyChanged("CarNumber");
                }
            }
        }

        Color _carNumberColor = Colors.Orange;
        public Color CarNumberColor
        {
            get
            {
                return _carNumberColor;
            }
            set
            {
                if (value != _carNumberColor)
                {
                    _carNumberColor = value;
                    NotifyPropertyChanged("CarNumberColor");
                }
            }
        }
        #endregion

        #region Pit count
        int _pitCount = -1;
        public string PitCount
        {
            get
            {
                if (_pitCount != -1)
                {
                    return _pitCount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    return "";
                }
            }
        }

        public int PitCountSet
        {
            set
            {
                if (value != _pitCount)
                {
                    _pitCount = value;
                    NotifyPropertyChanged("PitCount");
                }
            }
        }

        Color _pitCountColor = Colors.Orange;
        public Color PitCountColor
        {
            get
            {
                return _pitCountColor;
            }
            set
            {
                if (value != _pitCountColor)
                {
                    _pitCountColor = value;
                    NotifyPropertyChanged("PitCountColor");
                }
            }
        } 
        #endregion

        #region Position
        int _position = -1;
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value != _position)
                {
                    _position = value;
                    NotifyPropertyChanged("Position");
                }
            }
        }

        int _tablePosition = -1;
        public int TablePosition
        {
            get
            {
                return _tablePosition;
            }
            set
            {
                if (value != _tablePosition)
                {
                    _tablePosition = value;
                    NotifyPropertyChanged("TablePosition");
                }
            }
        }

        Color _positionColor = Colors.Orange;
        public Color PositionColor
        {
            get
            {
                return _positionColor;
            }
            set
            {
                if (value != _positionColor)
                {
                    _positionColor = value;
                    NotifyPropertyChanged("PositionColor");
                }
            }
        } 
        #endregion

        #region Sector 1 time
        DateTime _sector1Start = DateTime.MinValue;
        DateTime _sector1End = DateTime.MinValue;
        double _sector1Time;
        CarBaseMessage.TimeType _sector1Type;
        public void SetSector1Time(double sector1Time, CarBaseMessage.TimeType type, Color color)
        {            
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_sector1TimeColor == color)
                {
                    _sector1Type = type;
                    NotifyPropertyChanged("Sector1Time");
                }
            }
            else
            {
                if (_carNumberColor == Colors.Red)
                {
                    _sector1Start = _sector2Start = _sector3Start = DateTime.MinValue;
                }

                _sector2Start = _sector1End = DateTime.Now;
                _sector1Time = sector1Time;
                _sector1Type = type;
                NotifyPropertyChanged("Sector1Time");
            }

            Sector1TimeColor = color;
        }

        public string Sector1Time
        {
            get
            {
                switch (_sector1Type)
                {
                    case CarBaseMessage.TimeType.Time:
                        return _sector1Time.ToString(@"##.0", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Stop:
                        return "STOP";

                    case CarBaseMessage.TimeType.Retired:
                        return "DNF";

                    case CarBaseMessage.TimeType.TextTime:
                        if (IsDataEstimationEnabled && _sector1Start != DateTime.MinValue && _carNumberColor != Colors.Red
                            && _sector1End > _sector1Start && (_sector1End - _sector1Start).TotalSeconds > 10.0
                            && (_sector1End - _sector1Start).TotalSeconds < 90.0)
                        {
                            return (_sector1End - _sector1Start).TotalSeconds.ToString(@"##.0", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            return "●";
                        }                        

                    default:
                        return "";
                }
            }

            private set
            {

            }
        }

        Color _sector1TimeColor = Colors.Orange;
        public Color Sector1TimeColor
        {
            get
            {
                return _sector1TimeColor;
            }
            set
            {
                if (value != _sector1TimeColor)
                {
                    _sector1TimeColor = value;
                    NotifyPropertyChanged("Sector1TimeColor");
                }
            }
        } 
        #endregion

        #region Sector 2 time
        DateTime _sector2Start = DateTime.MinValue;
        DateTime _sector2End = DateTime.MinValue;
        double _sector2Time;
        CarBaseMessage.TimeType _sector2Type;
        public void SetSector2Time(double sector2Time, CarBaseMessage.TimeType type, Color color)
        {
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_sector2TimeColor == color)
                {
                    _sector2Type = type;
                    NotifyPropertyChanged("Sector2Time");
                }
            }
            else
            {
                if (_carNumberColor == Colors.Red)
                {
                    _sector1Start = _sector2Start = _sector3Start = DateTime.MinValue;
                }

                _sector3Start = _sector2End = DateTime.Now;
                _sector2Time = sector2Time;
                _sector2Type = type;
                NotifyPropertyChanged("Sector2Time");
            }

            Sector2TimeColor = color;
        }

        public string Sector2Time
        {
            get
            {
                switch (_sector2Type)
                {
                    case CarBaseMessage.TimeType.Time:
                        return _sector2Time.ToString(@"##.0", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Stop:
                        return "STOP";

                    case CarBaseMessage.TimeType.Retired:
                        return "DNF";

                    case CarBaseMessage.TimeType.TextTime:
                        if (IsDataEstimationEnabled && _sector2Start != DateTime.MinValue && _carNumberColor != Colors.Red
                            && _sector2End > _sector2Start && (_sector2End - _sector2Start).TotalSeconds > 10.0
                            && (_sector2End - _sector2Start).TotalSeconds < 90.0)
                        {
                            return (_sector2End - _sector2Start).TotalSeconds.ToString(@"##.0", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            return "●";
                        }      

                    default:
                        return "";
                }
            }

            private set
            {

            }
        }

        Color _sector2TimeColor = Colors.Orange;
        public Color Sector2TimeColor
        {
            get
            {
                return _sector2TimeColor;
            }
            set
            {
                if (value != _sector2TimeColor)
                {
                    _sector2TimeColor = value;
                    NotifyPropertyChanged("Sector2TimeColor");
                }
            }
        } 
        #endregion

        #region Sector 3 time
        DateTime _sector3Start = DateTime.MinValue;
        DateTime _sector3End = DateTime.MinValue;
        double _sector3Time;
        CarBaseMessage.TimeType _sector3Type;
        public void SetSector3Time(double sector3Time, CarBaseMessage.TimeType type, Color color)
        {
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_sector3TimeColor == color)
                {
                    _sector3Type = type;
                    NotifyPropertyChanged("Sector3Time");
                }
            }
            else
            {
                if (_carNumberColor == Colors.Red)
                {
                    _sector1Start = _sector2Start = _sector3Start = DateTime.MinValue;
                }

                _sector1Start = _sector3End = DateTime.Now;
                _sector3Time = sector3Time;
                _sector3Type = type;
                NotifyPropertyChanged("Sector3Time");
            }

            Sector3TimeColor = color;
        }

        public string Sector3Time
        {
            get
            {
                switch (_sector3Type)
                {
                    case CarBaseMessage.TimeType.Time:
                        return _sector3Time.ToString(@"##.0", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Stop:
                        return "STOP";

                    case CarBaseMessage.TimeType.Retired:
                        return "DNF";

                    case CarBaseMessage.TimeType.TextTime:
                        if (IsDataEstimationEnabled && _sector3Start != DateTime.MinValue && _carNumberColor != Colors.Red
                            && _sector3End > _sector3Start && (_sector3End - _sector3Start).TotalSeconds > 10.0
                            && (_sector3End - _sector3Start).TotalSeconds < 90.0)
                        {
                            return (_sector3End - _sector3Start).TotalSeconds.ToString(@"##.0", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            return "●";
                        }      

                    default:
                        return "";
                }
            }

            private set
            {

            }
        }

        Color _sector3TimeColor = Colors.Orange;
        public Color Sector3TimeColor
        {
            get
            {
                return _sector3TimeColor;
            }
            set
            {
                if (value != _sector3TimeColor)
                {
                    _sector3TimeColor = value;
                    NotifyPropertyChanged("Sector3TimeColor");
                }
            }
        } 
        #endregion

        #region Q1 time
        double _q1Time;
        CarBaseMessage.TimeType _q1Type;

        public void SetQ1Time(double q1Time, CarBaseMessage.TimeType type, Color color)
        {
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_q1TimeColor == color)
                {
                    _q1Type = type;
                    NotifyPropertyChanged("Q1Time");
                }
            }
            else
            {
                _q1Time = q1Time;
                _q1Type = type;
                NotifyPropertyChanged("Q1Time");
            }

            Q1TimeColor = color;
        }

        public string Q1Time
        {
            get
            {
                switch (_q1Type)
                {
                    case CarBaseMessage.TimeType.Time:
                        return TimeSpan.FromSeconds(_q1Time).ToString(@"m\:ss\.fff", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Stop:
                        return "STOP";

                    case CarBaseMessage.TimeType.Retired:
                        return "DNF";

                    default:
                        return "";
                }
            }

            private set
            {

            }
        }

        Color _q1TimeColor = Colors.Orange;
        public Color Q1TimeColor
        {
            get
            {
                return _q1TimeColor;
            }
            set
            {
                if (value != _q1TimeColor)
                {
                    _q1TimeColor = value;
                    NotifyPropertyChanged("Q1TimeColor");
                }
            }
        } 
        #endregion

        #region Q2 time
        double _q2Time;
        CarBaseMessage.TimeType _q2Type;

        public void SetQ2Time(double q2Time, CarBaseMessage.TimeType type, Color color)
        {
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_q2TimeColor == color)
                {
                    _q2Type = type;
                    NotifyPropertyChanged("Q2Time");
                }
            }
            else
            {
                _q2Time = q2Time;
                _q2Type = type;
                NotifyPropertyChanged("Q2Time");
            }

            Q2TimeColor = color;
        }

        public string Q2Time
        {
            get
            {
                switch (_q2Type)
                {
                    case CarBaseMessage.TimeType.Time:
                        return TimeSpan.FromSeconds(_q2Time).ToString(@"m\:ss\.fff", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Stop:
                        return "STOP";

                    case CarBaseMessage.TimeType.Retired:
                        return "DNF";

                    default:
                        return "";
                }
            }

            private set
            {

            }
        }

        Color _q2TimeColor = Colors.Orange;
        public Color Q2TimeColor
        {
            get
            {
                return _q2TimeColor;
            }
            set
            {
                if (value != _q2TimeColor)
                {
                    _q2TimeColor = value;
                    NotifyPropertyChanged("Q2TimeColor");
                }
            }
        } 
        #endregion

        #region Q3 time
        double _q3Time;
        CarBaseMessage.TimeType _q3Type;

        public void SetQ3Time(double q3Time, CarBaseMessage.TimeType type, Color color)
        {
            if (type == CarBaseMessage.TimeType.NoData)
            {
                //when color is the same NoData is used to clear time value 
                //else it has no meaning - only color is changed 
                if (_q3TimeColor == color)
                {
                    _q3Type = type;
                    NotifyPropertyChanged("Q3Time");
                }
            }
            else
            {
                _q3Time = q3Time;
                _q3Type = type;
                NotifyPropertyChanged("Q3Time");
            }

            Q3TimeColor = color;
        }

        public string Q3Time
        {
            get
            {
                switch (_q3Type)
                {
                    case CarBaseMessage.TimeType.Time:
                        return TimeSpan.FromSeconds(_q3Time).ToString(@"m\:ss\.fff", CultureInfo.InvariantCulture);

                    case CarBaseMessage.TimeType.Stop:
                        return "STOP";

                    case CarBaseMessage.TimeType.Retired:
                        return "DNF";

                    default:
                        return "";
                }
            }

            private set
            {

            }
        }

        Color _q3TimeColor = Colors.Orange;
        public Color Q3TimeColor
        {
            get
            {
                return _q3TimeColor;
            }
            set
            {
                if (value != _q3TimeColor)
                {
                    _q3TimeColor = value;
                    NotifyPropertyChanged("Q3TimeColor");
                }
            }
        } 
        #endregion

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public bool Equals(CarViewModel other)
        {
            return _carId == other._carId;
        }

        public bool IsDataEstimationEnabled { get; set; }
    }
}

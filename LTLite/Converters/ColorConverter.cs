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
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using F1.Messages.System;

namespace LTLite.Converters
{
    //[ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                return new SolidColorBrush((Color)value);
            }
            else if (value is TrackStatus.Colour)
            {
                switch ((TrackStatus.Colour)value)
                {
                    case TrackStatus.Colour.Green:
                        return new SolidColorBrush(Colors.Green);

                    case TrackStatus.Colour.Yellow:
                        return new SolidColorBrush(Colors.Yellow);

                    case TrackStatus.Colour.Red:
                    default:
                        return new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.Yellow);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

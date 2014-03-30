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
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace LTStoreApp.Converters
{
    public class OddEvenColorConverter : IValueConverter
    {
        SolidColorBrush _even = new SolidColorBrush(Colors.Transparent);
        SolidColorBrush _odd = new SolidColorBrush(Color.FromArgb(255, 25, 25, 25));

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int)
            {
                return ((int)value) % 2 == 0 ? _even : _odd;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

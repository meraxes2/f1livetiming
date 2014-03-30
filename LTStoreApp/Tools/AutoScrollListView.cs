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

using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace LTStoreApp.Tools
{
    public class AutoScrollListView: ListView
    {
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);

            var scrollbars = this.GetDescendantsOfType<ScrollBar>().ToList();
            var verticalBar = scrollbars.FirstOrDefault(x => x.Orientation == Orientation.Vertical);
            if (verticalBar != null)
            {
                if (verticalBar.Value >= verticalBar.Maximum)
                {
                    int selectedIndex = Items.Count - 1;
                    if (selectedIndex >= 0)
                    {
                        SelectedIndex = selectedIndex;
                        UpdateLayout();

                        ScrollIntoView(SelectedItem);
                    }
                }
            }                 
        }
    }
}

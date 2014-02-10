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

using System.Windows;
using Microsoft.Phone.Scheduler;
using WP7TilesCommon;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;

namespace WP7BackgroundAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
#if DEBUG
            ShellToast toast = new ShellToast()
            {
                Title = "Start " + (DeviceStatus.ApplicationCurrentMemoryUsage / 1024).ToString() + " KB",
                Content = (DeviceStatus.ApplicationMemoryUsageLimit / 1024).ToString() + " KB"
            };
            toast.Show();
#endif

            TileUpdater updater = new TileUpdater();
            updater.OnUpdateCompleted += updater_OnUpdateCompleted;
            updater.UpdateAsync();
        }

        void updater_OnUpdateCompleted()
        {
#if DEBUG
            ShellToast toast = new ShellToast()
            {
                Title = "Done " + (DeviceStatus.ApplicationCurrentMemoryUsage / 1024).ToString() + " KB",
                Content = (DeviceStatus.ApplicationMemoryUsageLimit / 1024).ToString() + " KB"
            };
            toast.Show();
#endif
            NotifyComplete();
        }
    }
}
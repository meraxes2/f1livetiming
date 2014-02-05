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

using Microsoft.Phone.Scheduler;
using System;
using System.Threading;
using System.Windows;

namespace LTLite.Tools
{
    public static class BackgroundAgentRescheduler
    {
        static string TASK_NAME = "LTLiteLiveTileUpdateTask";

        public static void RescheduleAgent()
        {
            ThreadPool.QueueUserWorkItem(DoReschedule);
        }

        private static void DoReschedule(object state)
        {
            PeriodicTask task = ScheduledActionService.Find(TASK_NAME) as PeriodicTask;

            if (task != null)
            {
#if DEBUG
                if (task.LastExitReason != AgentExitReason.Completed)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("Last agent execution status:\n" + task.LastExitReason.ToString(), "Background message", MessageBoxButton.OK);
                    });
                }
#endif

                try
                {
                    ScheduledActionService.Remove(TASK_NAME);
                }
                catch (Exception)
                {
                    //Skip exception here
                }
            }

            try
            {
                task = new PeriodicTask(TASK_NAME);
                task.Description = "Updating live tiles";

                ScheduledActionService.Add(task);

#if DEBUG
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException er)
            {
                if (er.Message.Contains("BNS Error: The action is disabled"))
                {
                    string msg = "Background task has been disabled from system settings.";
                    msg += " Please enable it if you want the app to update live tiles in the background.";

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(msg, "Warning", MessageBoxButton.OK);
                    });
                }
            }
        }
    }
}

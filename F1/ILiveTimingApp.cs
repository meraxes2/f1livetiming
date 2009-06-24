/*
 *  f1livetiming - Part of the Live Timing Library for .NET
 *  Copyright (C) 2009 Liam Lowey
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
using F1.Messages;

namespace F1
{
    /// <summary>
    /// Define a function pattern for receiving live timing messages. <see cref="IMessage"/>
    /// </summary>
    /// 
    /// <example>
    /// Following is an example of how to use the messages you're interested in.
    /// <code>
    /// //TODO
    /// </code> 
    /// </example>
    /// 
    /// <param name="msg">Will contain the base message type</param>
    public delegate void LiveTimingMessageHandlerDelegate(IMessage msg);

    /// <summary>
    /// This interface represents the Live Timing application functionality and 
    /// is implemented in the library by two concrete types, one for connecting
    /// to the real live timing servers, and the other to simulate a live timing
    /// server.
    /// 
    /// In both cases the usage of this object will be the same.
    /// 
    /// <example>
    /// The following a simple example demonstrating being called from a console
    /// application where the main thread is used to drive the <c>Run()</c>
    /// method.
    /// 
    /// <code>
    /// class Program
    /// {
    /// private readonly ILiveTimingApp _lt;
    /// private static readonly ILog Log = LogManager.GetLogger("Program");
    ///
    /// static void Main()
    /// {
    ///     log4net.Config.XmlConfigurator.Configure(); // use app.config
    ///     try
    ///     {
    ///         Program p = new Program();
    ///         p.Run();
    ///     }
    ///     catch (AuthorizationException)
    ///     {
    ///         Log.Error("Failed to authenticate credentials. Exiting.");
    ///     }
    ///     catch (ConnectionException)
    ///     {
    ///         Log.Error("Failed to connect to live timing server. Exiting.");
    ///     }
    /// }
    /// 
    /// public Program()
    /// {
    ///     Console.CancelKeyPress += ConsoleCancelKeyPress;
    ///     
    ///     // We say false to starting a thread because we will call the Run method
    ///     // for this. If however we could not have a blocking call, specifying true
    ///     // here would create a new thread to run the live timing component on.
    ///     _lt = new LiveTiming("your@username.com", "Your password", false);
    ///     _lt.CarMessageHandler += MessageHandler;
    ///     _lt.SystemMessageHandler += MessageHandler;
    /// }
    /// 
    /// private static void MessageHandler(IMessage msg)
    /// {
    ///     //  Log incoming messages
    ///     Log.Info(msg.ToString());
    /// }
    /// 
    /// private void Run()
    /// {
    ///     try {
    ///         // block on the main thread until CTRL+C or EOS message.
    ///         _lt.Run();
    ///     }
    ///     finally {
    ///         // When the thread stops, we clean up.
    ///         _lt.Dispose();
    ///     }
    /// }
    /// 
    /// private void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    /// {
    ///     Log.Info("Caught ctrl+c, exiting application.");
    ///     _lt.Stop(false);
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public interface ILiveTimingApp : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        event LiveTimingMessageHandlerDelegate SystemMessageHandler;
        event LiveTimingMessageHandlerDelegate CarMessageHandler;

        /// <summary>
        /// Body method to be called by parent process.
        /// </summary>
        void Run();

        /// <summary>
        /// Instruct Run method to exit
        /// </summary>
        void Stop( bool discard );
    }
}

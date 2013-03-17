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
using F1.Messages.System;

namespace F1
{
    /// <summary>
    /// Define a function pattern for receiving live timing messages. <see cref="IMessage"/>
    /// </summary>
    /// <param name="msg">Will contain a message implementation which can be queried by reflection or
    /// by inspecting the <see cref="IMessage.Type"/> property</param>
    public delegate void LiveTimingMessageHandlerDelegate(IMessage msg);

    /// <summary>
    /// <para>This interface represents the Live Timing application functionality and 
    /// is implemented in the library by two concrete types, one for connecting
    /// to the real live timing servers, and the other to simulate a live timing
    /// server.</para>
    /// 
    /// <h1>Usage</h1>
    /// <para>In both cases the usage of this object will be similar.</para>
    /// 
    /// <h2>Console Application</h2>
    /// <para>For a Console application it is quite likely that you will use the <see cref="Run"/> method
    /// as your application's runtime loop. Hence you should construct your concreate type
    /// without a new thread and then call <see cref="Run"/> at the point your application should block.
    /// A suitable keypress handler, for example CTRL+C, will allow you to interact with your
    /// application. See the example below.</para>
    /// 
    /// <h2>WinForms Application</h2>
    /// <para>When creating a forms based application it may not be suitable to block any calling thread
    /// with a call to <see cref="Run"/> as this will prevent any interaction with the GUI. The correct
    /// behaviour then is to indicate that a thread should be created on which to execute Run. Be aware 
    /// therefore that any event handlers will be called by this new thread, and not by applications main
    /// thread, hence you should take caution to protect your data from thread conditions. </para>
    /// 
    /// <h2>Notice</h2>
    /// <para>This library uses the log4net library of components and it is therefore the application's
    /// responsibility to ensure these are correct initialised. See http://logging.apache.org/log4net/</para>
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
        /// Handle this event to receive System IMessage types. <see cref="LiveTimingMessageHandlerDelegate"/>
        /// </summary>
        event LiveTimingMessageHandlerDelegate SystemMessageHandler;

        /// <summary>
        /// Handle this event to receive Car IMessage types. <see cref="LiveTimingMessageHandlerDelegate"/>
        /// </summary>
        event LiveTimingMessageHandlerDelegate CarMessageHandler;

        /// <summary>
        /// This function implements the 'Runtime' of this library, and hence the calling
        /// thread is responsible for invoking the associated message handlers. Calling this method
        /// will block until one of either <see cref="Stop"/> is called or an End of session
        /// message is generated <see cref="EndOfSession"/>.
        /// </summary>
        void Run();

        /// <summary>
        /// For use by the owning application to 'Stop' the livetiming application. This will
        /// result in the <see cref="Run"/> method exiting, and hence unblocking the calling
        /// thread.
        /// </summary>
        /// <param name="discard">true will cause the thread to exit abondoning any processing and messages that may
        /// remain. Use false to complete the current task before exiting.</param>
        void Stop( bool discard );

        void StartThread();


        /// <summary>
        /// The current event type once this race/qualy/free practice has started
        /// </summary>B
        F1.Enums.EventType CurrentEventType { get; }
    }
}

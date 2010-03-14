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

using System.IO;
using System.Collections.Generic;
using Common.Patterns.State;
using F1.Data.PacketReaderImpl;
using F1.Data.Packets;
using F1.Enums;
using F1.Exceptions;
using F1.Messages;
using F1.Messages.System;
using log4net;
using System;

namespace F1.Data
{
    namespace PacketReaderImpl
    {
        public class DataContext : IContext
        {
            public Stream Input { get; set; }
            public Stream DecryptInput { get; set; }
            public Header Header { get; set; }
            public Packet Packet { get; set; }
            public IMessage CurrentMessage { get; set; }
            public Queue<IMessage> MessageQueue { get; set; }
            public EventType EventType { get; set; }
            public ILog Logger { get; set; }

            public DataContext()
            {
                EventType = EventType.NoEvent;
                MessageQueue = new Queue<IMessage>();
                Logger = LogManager.GetLogger("PacketReader");
            }

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion
        }

        /// <summary>
        /// Define the contract for this state machine.
        /// methods.
        /// </summary>
        public interface IPacketReaderContract
        {
            /// <summary>
            /// The states must implement this and process the next
            /// chunk of available data in it. It's ok to change state
            /// but if you require further processing, return true.
            /// </summary>
            /// <returns>true indicates we may not have completed the stream, and
            /// hence we should want to continue processing.</returns>
            bool ReadNext();
        }


        /// <summary>
        /// The first state for initialising the packet reader.
        /// </summary>
        internal class PacketReaderInit : State<DataContext>, IPacketReaderContract
        {
            public override void Entry()
            {
                StateMachine.ChangeTo<NewMessageStartState>();
            }

            public bool ReadNext()
            {
                // this should never be called
                return false;
            }
        }


        /// <summary>
        /// Start the process of reading the next message from
        /// the stream.
        /// </summary>
        internal class NewMessageStartState : State<DataContext>, IPacketReaderContract
        {
            public bool ReadNext()
            {
                Context.Header = new Header(Context.Input);

                if (Context.Header.IsComplete)
                {
                    // We've read the header completely, so now we
                    // move on to understanding it.

                    StateMachine.ChangeTo<ProcessHeaderState>();

                    return true;
                }
                
                // We haven't read some or all of the header yet
                // so exit and wait for more data.

                StateMachine.ChangeTo<HeaderContinuationState>();

                return false;
            }
        }


        /// <summary>
        /// Read the remainder of the incomplete header message.
        /// </summary>
        internal class HeaderContinuationState : State<DataContext>, IPacketReaderContract
        {
            public bool ReadNext()
            {
                Context.Header.ContinueDataRead();

                if (Context.Header.IsComplete)
                {
                    // We've read all of the header now we need to understand it

                    StateMachine.ChangeTo<ProcessHeaderState>();

                    return true;
                }

                // This probably shouldn't happen given that the header is only two bytes
                // and we should have read 1 of them. Generally with sockets if you read
                // 0 bytes, the connection has been closed. Ignore for now, but may
                // need a Stream closed state.
               
                return false;
            }
        }


        /// <summary>
        /// Process the completed header data, and start getting the data
        /// appropriate for the given message.
        /// </summary>
        internal class ProcessHeaderState : State<DataContext>, IPacketReaderContract
        {
            public override void Entry()
            {
                // It's not our business to choose the message, so rely
                // on the factory object.
                if (Context.Header.IsSystemMessage)
                {
                    Context.CurrentMessage = MessageFactory.CreateMessage(Context.Header.SystemType);
                }
                else
                {
                    Context.CurrentMessage = MessageFactory.CreateMessage(Context.Header.CarType, Context.EventType);
                }


                if( null == Context.CurrentMessage )
                {
                    Context.Logger.ErrorFormat("Unknown message type={0}, carId={1}, datum=0x{2:x}.", Context.Header.RawType, Context.Header.CarId, Context.Header.Datum);

                    // Couldn't resolve the message, which means we don't know how
                    // to continue with this stream. "Computer says no".

                    StateMachine.ChangeTo<ErrorState>();
                    throw new UnknownSystemTypeException(Context.Header.RawType, Context.Header.CarId, Context.Header.Datum);        
                }

                Context.Packet = Context.CurrentMessage.CreatePacketType(Context.Header, Context.Input, Context.DecryptInput);

                if (null == Context.Packet || Context.Packet.IsComplete)
                {
                    // The packet is completed, so Complete the processing of this message by
                    // changing to the next state now.
                    StateMachine.ChangeTo<ProcessMessageState>();
                }
                else
                {
                    // Otherwise we need to wait for the rest of the data for the
                    // message so put it into the read continuation state.
                    StateMachine.ChangeTo<PacketContinuationState>();
                }
            }


            public bool ReadNext()
            {
                throw new NotImplementedException("This method can never be called.");
            }
        }


        /// <summary>
        /// Continue reading the data for this packet
        /// </summary>
        internal class PacketContinuationState : State<DataContext>, IPacketReaderContract
        {
            public override void Entry()
            {
                if (Context.CurrentMessage == null)
                {
                    throw new NullReferenceException();
                }
            }

            public bool ReadNext()
            {
                Context.Packet.ContinueDataRead();

                if (Context.Packet.IsComplete)
                {
                    // We managed to get the rest of the message data
                    // so complete processing of the message.

                    StateMachine.ChangeTo<ProcessMessageState>();
                    return true;
                }

                // still need to retrieve more data.
                // Might be worth checking if we've retrieved any data, and
                // hence the stream may have been closed.

                return false;
            }
        }


        /// <summary>
        /// Complete the extraction of data for the message and queue the
        /// completed message.
        /// </summary>
        internal class ProcessMessageState : State<DataContext>, IPacketReaderContract
        {
            public override void Entry()
            {
                if (Context.Packet == null || !Context.Packet.IsGarbage)
                {
                    Context.CurrentMessage.Deserialise(Context.Header, Context.Packet);

                    Context.MessageQueue.Enqueue(Context.CurrentMessage);

                    if (Context.Header.IsSystemMessage && Context.Header.SystemType == SystemPacketType.EventId)
                    {
                        Context.EventType = ((EventId) Context.CurrentMessage).EventType;
                    }

                    if (Context.Logger.IsDebugEnabled)
                    {
                        Context.Logger.Debug(Context.CurrentMessage.ToString());
                    }
                }
                else
                {
                    Context.Logger.Debug("Writing off garbage packet.");
                }

                Context.CurrentMessage = null;
                
                // Start again:

                StateMachine.ChangeTo<NewMessageStartState>();
            }


            public bool ReadNext()
            {
                throw new NotImplementedException("This method can not be called.");
            }
        }


        /// <summary>
        /// We've reached an unrecoverable state.
        /// </summary>
        internal class ErrorState : State<DataContext>, IPacketReaderContract
        {
            public bool ReadNext()
            {
                throw new ErrorStateException();
            }
        }
    }

    /// <summary>
    /// Packet Reader is used to process an incoming stream of data
    /// and retrieve the appropriate messages from it.
    /// </summary>
    /// <remarks>
    /// This implements the Finite State Machine pattern to track
    /// progressing states through the stream of bytes. This is
    /// important because at any one time we may have a break in
    /// data from which we will need to continue another iteration
    /// later.
    /// </remarks>
    public class PacketReader : StateMachine<DataContext, IPacketReaderContract>
    {
        /// <summary>
        /// Initialise the state machine with the raw input data
        /// as a stream.
        /// </summary>
        public PacketReader(Stream input, Stream decryptorStream)
        {
            Context.Input = input;
            Context.DecryptInput = decryptorStream;

            InitialState<PacketReaderInit>();
        }

        
        /// <summary>
        /// Call this repeatedly every time data is available. This responsibility
        /// is up to the caller. This method will return false when it has
        /// exhausted it's data source. Otherwise the caller should call the
        /// method again.
        /// </summary>
        public bool ReadNext()
        {
            return TypedState.ReadNext();
        }


        /// <summary>
        /// Call this method in between ReadNext calls to determine the current
        /// state of the EventType variable. It can be used to change the current
        /// type, but remember this is not thread safe so do not call while
        /// in ReadNext.
        /// </summary>
        public EventType CurrentEventType
        {
            get
            {
                return Context.EventType;
            }
            set
            {
                Context.EventType = value;
            }
        }


        /// <summary>
        /// This queue can be inspected inbetween each call to ReadNext, any new
        /// messages can be pop'd. Do not change the state (pop) of this queue while in
        /// ReadNext as this is not thread safe.
        /// </summary>
        public Queue<IMessage> MessageQueue { get { return Context.MessageQueue; } }
    }
}

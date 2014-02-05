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
using F1.Data;
using F1.Data.Encryption;
using F1.Enums;
using F1.Messages;
using F1.Messages.Car;
using F1.Messages.System;
//using log4net;

namespace F1.Runtime
{
    /// <summary>
    /// Defines the business logic of livetiming. This is designed to respond to certain conditions
    /// and events to ensure a smooth pipeline. It will deal with requests for Keyframes, Authentication
    /// and Session tracking.
    /// </summary>
    public class Runtime
    {
        #region State variables
        private readonly IAuthKey _authKeyService;
        private readonly IKeyFrame _keyFrameService;
        private readonly DataDecryptor _decryptor;
        private readonly IMessageDispatch _messageDispatch;
        private readonly PacketReader _packetReader;
        private bool _keyFrameReceived;
        private int _currentLap;
        #endregion

        //private readonly ILog _logger = LogManager.GetLogger("Runtime");


        /// <summary>
        /// Create the Runtime object to combine all the various components of the livetiming.
        /// </summary>
        /// <param name="liveStream">A stream used to represent the incoming data of the live stream.</param>
        /// <param name="authKeyService">Provider for authorization key requests.</param>
        /// <param name="keyFrameService">Provider for keyframes.</param>
        /// <param name="messageDispatch">A receiver for messages.</param>
        public Runtime(Stream liveStream, IAuthKey authKeyService, IKeyFrame keyFrameService, IMessageDispatch messageDispatch)
        {
            _decryptor = new DataDecryptor();
            _packetReader = new PacketReader(liveStream, new DecryptStreamDecorator(liveStream, _decryptor));
            _messageDispatch = messageDispatch;
            _authKeyService = authKeyService;
            _keyFrameService = keyFrameService;
        }


        /// <summary>
        /// IDriver implementations can call this from their constructor to 'take control' of this runtime.
        /// I.e. become the driving force to processing messages for the runtime. Only one IDriver can exist.
        /// </summary>
        public IDriver Driver { get; set; }


        /// <summary>
        /// Call this from your IDriver to process more data available on the liveStream.
        /// </summary>
        /// <returns>true indicates there may be more data on the stream so call it again. false
        /// means that it has exhausted its current supply and your IDriver should go about sourcing
        /// more.</returns>
        public bool HandleRead()
        {
            bool read = _packetReader.ReadNext();

            while( _packetReader.MessageQueue.Count > 0 )
            {
                DispatchMessage(_packetReader.MessageQueue.Dequeue(), true);
            }

            return read;
        }


        /// <summary>
        /// Call this from your IDriver if you have an entire frames worth of data to process. This useful only
        /// when processing keyframe data as it disregards any stateful information associated with the live stream.
        /// </summary>
        /// <param name="keyFrameData">The stream must contain complete data for a keyframe.</param>
        public void HandleReadAdHoc(Stream keyFrameData)
        {
            PacketReader keyFrameReader = new PacketReader(keyFrameData, new DecryptStreamDecorator(keyFrameData, _decryptor));

            _decryptor.Reset();

            bool keepReading;
            do
            {
                keepReading = keyFrameReader.ReadNext();
                while (keyFrameReader.MessageQueue.Count > 0)
                {
                    DispatchMessage(keyFrameReader.MessageQueue.Dequeue(), false);
                }
            } while (keepReading);

            //  The live stream may not yet know its own event type because it probably
            // hasn't received the EventId message. So we copy the value across from the
            // keyframe. This is necessary to know how to deserialise car packets.
            UpdateEventType(keyFrameReader.CurrentEventType);
        }


        #region Internal Message Dispatching
        Commentary _incompleteComentaryMsg = null;

        private void DispatchMessage(IMessage msg, bool dispatchKeyFrame)
        {
            bool dispatch = true;

            switch(msg.Type)
            {
                case SystemPacketType.EventId:
                    InitNewEvent(msg as EventId);
                    break;
                case SystemPacketType.KeyFrame:
                    dispatch = false;
                    HandleKeyFrame(msg as KeyFrame, dispatchKeyFrame);
                    break;
                case SystemPacketType.RefreshRate:
                    dispatch = false;
                    SetRefreshRate(msg as RefreshRate);
                    break;
                case SystemPacketType.CarType:
                    if( msg is CarInterval  )
                    {
                        UpdateLapCount(msg as CarInterval);
                    }
                    break;
                case SystemPacketType.Commentary:
                    {
                        Commentary comentary = msg as Commentary;

                        if (_incompleteComentaryMsg != null)
                        {
                            _incompleteComentaryMsg.Message += comentary.Message;
                            _incompleteComentaryMsg.EndMessage = comentary.EndMessage;
                        }
                        else
                        {
                            _incompleteComentaryMsg = comentary;
                        }

                        if (_incompleteComentaryMsg.EndMessage)
                        {
                            //message is completed so dispatch it
                            msg = _incompleteComentaryMsg;
                            _incompleteComentaryMsg = null;
                        }
                        else
                        {
                            //we need to wait for the rest of the message before we can dispatch
                            dispatch = false;
                        }
                    }
                    break;
                default:
                    break;
            }


            if( dispatch )
            {
                _messageDispatch.Dispatch(msg);
            }
        }


        private void InitNewEvent(EventId e)
        {
            //_logger.Info("Retrieving decryption key for session: " + e.SessionId);

            _decryptor.Key = _authKeyService.GetKey(e.SessionId);

            //_logger.Info("Decryption key retrieved: 0x" + _decryptor.Key.ToString("X"));

            //  It's a new event, so get the new KeyFrame for the event.
            _keyFrameReceived = false;
        }


        private void HandleKeyFrame(KeyFrame msg, bool dispatchKeyFrame)
        {
            if (dispatchKeyFrame && !_keyFrameReceived)
            {
                //  We're not already processing a key frame, so dispatch
                // a call to process the data for this keyframe.
                RetrieveAndProcessKeyFrame(msg);

                // Only need the first keyframe, then rely on live data.
                _keyFrameReceived = true;
            }

            Driver.UpdateCurrentKeyFrame(msg.FrameNumber);

            _decryptor.Reset();
        }


        private void RetrieveAndProcessKeyFrame(KeyFrame k)
        {
            //_logger.Info("Retrieving keyframe: " + k.FrameNumber.ToString("d5"));

            Stream keyFrameData = _keyFrameService.GetKeyFrame(k.FrameNumber);

            //_logger.DebugFormat("Retrieved keyframe, {0} bytes.", keyFrameData.Length);

            HandleReadAdHoc(keyFrameData);
        }


        private void UpdateEventType( EventType newEventType )
        {
            if( _packetReader.CurrentEventType != newEventType )
            {
                //_logger.InfoFormat("Applying EventType: {0}, to Live Stream.", newEventType.ToString());
                _packetReader.CurrentEventType = newEventType;
            }
        }


        private void UpdateLapCount( CarInterval msg )
        {
            if (CarBaseMessage.TimeType.NLaps == msg.IntervalType && msg.Interval > _currentLap )
            {
                _currentLap = (int) msg.Interval;

                IMessage newMsg = new RaceLapNumber(_currentLap);

                //if( _logger.IsDebugEnabled )
                //    _logger.Debug(newMsg.ToString());

                // We use the front runner's lap count to artificially generate
                // a lap count for general display.
                DispatchMessage( newMsg, true );
            }
        }


        private void SetRefreshRate( RefreshRate msg )
        {
            if (Driver != null)
            {
                if (msg.Rate > 0)
                {
                   // _logger.InfoFormat("Changing polling frequence to every {0} seconds", msg.Rate);
                    Driver.SetRefresh(msg.Rate);
                }
                else if (msg.Rate == 0)
                {
                    //_logger.Info("Received end of stream message, exiting.");

                    //  Terminate the socket driver so it stops trying.
                    Driver.Terminate();

                    //  Tell anyone listening that's the end of the session.
                    DispatchMessage( new EndOfSession(), true );
                }
            }
        }

        #endregion
    }
}

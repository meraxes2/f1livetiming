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
using System.IO;
using System.Threading;
using Common.Utils.Threading;
using F1.Runtime;
#if WINRT
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#endif
//using log4net;

namespace F1.Simulator
{
    /// <summary>
    /// A driver implementation used to simulator the live stream from a file capture.
    /// </summary>
    class FileCapDriver : SimpleThreadBase, IDriver, IDisposable
    {
        private const int CHUNK_SIZE = 128; // completely arbitary
        private const int INTERVAL = 1500; // also arbitary
        
        private readonly MemoryStream _memStream;
#if WINRT        
        private readonly string _path;
#else
        private readonly FileStream _fileStream;
#endif
        private readonly Runtime.Runtime _runtime;

        //private readonly ILog _log = LogManager.GetLogger("FileCapDriver");

        public FileCapDriver(string capFile, MemoryStream memStream, Runtime.Runtime runtime)
            : base(false)
        {
            _memStream = memStream;
#if WINRT
            _path = capFile;
#else
            _fileStream = File.OpenRead(capFile);
#endif
            _runtime = runtime;
            _runtime.Driver = this;

            Start();
        }

        #region Driver Impl
        public override void Run()
        {
#if WINRT
            StorageFile file = Package.Current.InstalledLocation.GetFileAsync(_path).AsTask().Result;
            using(var raStream = file.OpenReadAsync().AsTask().Result)
            {
                byte[] blob = new byte[CHUNK_SIZE];

                while (IsRunning && raStream.CanRead)
                {
                    DataReader reader = new DataReader(raStream.GetInputStreamAt(0));
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    try
                    {
                        reader.LoadAsync((uint)blob.Length).AsTask().Wait();
                        int bytesRead = 0;
                        while (reader.UnconsumedBufferLength > 0)
                        {
                            blob[bytesRead++] = reader.ReadByte();
                        }

                        if (bytesRead > 0)
                        {
                            HandleData(blob, bytesRead);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        reader.DetachStream();
                        reader.Dispose();
                    }

                    if (raStream.Position >= raStream.Size)
                    {
                        Terminate();
                        break;
                    }

                    Task.Delay(INTERVAL).Wait();
                }
            }  
#else
            byte [] blob = new byte[CHUNK_SIZE];

            while(IsRunning && _fileStream.CanRead)
            {
                int read = _fileStream.Read(blob, 0, CHUNK_SIZE);

                if( read > 0 )
                {
                    HandleData(blob, read); 
                }

                if(_fileStream.CanSeek && _fileStream.Position >= _fileStream.Length)
                {
                    Terminate();
                    break;
                }

                Thread.Sleep(INTERVAL);
            }
#endif
        }

        private void HandleData(byte[] blob, int dataLength)
        {
            long oldPosition = _memStream.Position;

            if (oldPosition == _memStream.Length)
            {
                //  We're at the edge of the stream, so to prevent a backlog
                // of data building up, start overwriting from the beginning.
                _memStream.Seek(0, SeekOrigin.Begin);
                _memStream.SetLength(0);
                oldPosition = 0;
            }
            else
            {
                //  There is some data still left to be read, so to avoid overwriting
                // it we must append this block to the end of the stream.
                _memStream.Seek(0, SeekOrigin.End);
            }


            //  Push new data onto the end of our stream.
            _memStream.Write(blob, 0, dataLength);


            //  Restore the read head position after the write so that the reads
            // start from the correct position.
            _memStream.Seek(oldPosition, SeekOrigin.Begin);


            //  Process new data
            try
            {
                while (_runtime.HandleRead())
                {
                }
            }
            catch (Exception)
            {
                //_log.Warn(e.Message);

                // Error while processing the data, so we abort and discard the remainder
                // of read/write requests pending.
                Stop(JoinMethod.DontJoin); // Don't join this thread to this thread.
            }
        }
        #endregion

        #region IDriver members
        public void SetRefresh(int refreshRate)
        {
            
        }

        public void Terminate()
        {
            Stop(JoinMethod.DontJoin);
        }

        public void UpdateCurrentKeyFrame(int currentFrame)
        {

        }
        #endregion

        #region IDispose members
        public void Dispose()
        {
            Stop(JoinMethod.Join);
        }
        #endregion
    }
}

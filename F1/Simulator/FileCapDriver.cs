using System;
using System.IO;
using System.Threading;
using Common.Utils.Threading;
using F1.Runtime;
using log4net;

namespace F1.Simulator
{
    class FileCapDriver : SimpleThreadBase, IDriver, IDisposable
    {
        private const int CHUNK_SIZE = 128; // completely arbitary
        private const int INTERVAL = 1500; // also arbitary
        
        private readonly MemoryStream _memStream;
        private readonly FileStream _fileStream;
        private readonly Runtime.Runtime _runtime;

        private readonly ILog _log = LogManager.GetLogger("FileCapDriver");

        public FileCapDriver(string capFile, MemoryStream memStream, Runtime.Runtime runtime)
            : base(false)
        {
            _memStream = memStream;
            _fileStream = File.OpenRead(capFile);
            _runtime = runtime;
            _runtime.Driver = this;

            Start();
        }

        #region Driver Impl
        public override void Run()
        {
            byte [] blob = new byte[CHUNK_SIZE];

            while(IsRunning && _fileStream.CanRead)
            {
                int read = _fileStream.Read(blob, 0, CHUNK_SIZE);

                if( read > 0 )
                {
                    HandleData(blob, read); 
                }

                Thread.Sleep(INTERVAL);
            }
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
            catch (Exception e)
            {
                _log.Warn(e.Message);

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

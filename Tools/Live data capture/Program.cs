using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Live_data_capture
{
    class Program : IDisposable
    {
        #region Configuration Constants
        const string HOST = "live-timing.formula1.com";
        const int PORT = 4321;
        const int BLOB_SIZE = 2048;
        const int SEND_INTERVAL = 1000;
        #endregion

        private FileStream _output;
        private Socket _incoming;
        private bool _running = false;
        private object _runningLock = new object();
        private byte[] _blob = new byte[BLOB_SIZE];

        public Program(string outputfilename)
        {
            try
            {
                _output = File.Open(outputfilename, FileMode.Create);

                System.Console.WriteLine("Capturing output to: " + _output.Name);
            }
            catch (IOException e)
            {
                Abort(e);
            }

            try
            {
                IPHostEntry e = Dns.GetHostEntry(HOST);

                _incoming = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                System.Console.WriteLine("Resolved {0} to {1}.", HOST, e.AddressList[0].ToString());

                _incoming.Connect(e.AddressList, PORT);

                _incoming.ReceiveTimeout = 10000; // 10 seconds because we may not always be receiving data

                _incoming.BeginReceive(_blob, 0, BLOB_SIZE, SocketFlags.None, new AsyncCallback(OnReceiveData), null);
            }
            catch (SocketException e)
            {
                Abort(e);
            }
        }


        public void Run()
        {
            _running = true;

            System.Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            byte [] reqPacket = { 0x10 };

            do
            {
                System.Threading.Thread.Sleep(SEND_INTERVAL);

                _incoming.Send(reqPacket);

                System.Console.WriteLine("Ping");
            }
            while (IsRunning);
        }


        protected void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            System.Console.WriteLine("User exiting...");
            Stop();
        }


        protected void OnReceiveData(IAsyncResult res)
        {
            try
            {
                int data = _incoming.EndReceive(res);
                if (data > 0)
                {
                    System.Console.WriteLine("Recieved {0} bytes.", data);
                    _output.Write(_blob, 0, data);
                    _output.Flush();
                }
                _incoming.BeginReceive(_blob, 0, BLOB_SIZE, SocketFlags.None, new AsyncCallback(OnReceiveData), null);
            }
            catch (Exception e)
            {
                Abort(e);
            }
        }


        protected void Abort(Exception e)
        {
            System.Console.WriteLine(e);
            Stop();
        }


        protected void Stop()
        {
            lock (_runningLock)
            {
                _running = false;
            }
        }


        protected bool IsRunning
        {
            get
            {
                lock (_runningLock)
                {
                    return _running;
                }
            }
        }


        #region IDisposable Members
        public void Dispose()
        {
            if (null != _incoming)
            {
                _incoming.Disconnect(false);
                _incoming.Close();
                _incoming = null;
            }

            if (null != _output)
            {
                _output.Flush();
                _output.Close();
                _output = null;
            }
        }
        #endregion

        #region Main
        static void Main(string[] args)
        {
            using (Program p = new Program(args[0]))
            {
                p.Run();
            }
        }
        #endregion
    }
}

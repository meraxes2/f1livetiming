using System.IO;
using System;
using Common.Utils.Strings;
using F1.Data;
using F1.Data.Encryption;
using F1.Data.Packets;
using F1.Enums;
using F1.Messages;

namespace KeyCracker
{
    class Program
    {
        private const string MESSAGE = "Please Wait ...";
        private const uint SALT = 0x55555555;

        private Stream _infile;
        private string _rawMessage;
        private EventType _evtType = EventType.Race;

        public Program(string file, string rawMessage)
        {
            _infile = File.OpenRead(file);
            _rawMessage = rawMessage;
        }


        void Run()
        {
            IMessage msg;
            Header h;
            while (true)
            {
                h = new Header(_infile);

                msg = null;
                if (h.IsSystemMessage)
                {
                    msg = MessageFactory.CreateMessage(h.SystemType, h.Datum);
                    UpdateEventType(msg);
                }
                else
                {
                    msg = MessageFactory.CreateMessage(h.CarType, _evtType);
                }

                try
                {
                    Packet p = msg.CreatePacketType(h, _infile, null);

                    msg.Deserialise(h, p);

                    //Console.WriteLine(msg);
                }
                catch (NullReferenceException)
                {
                    //Console.WriteLine("*" + msg);
                    break;
                }
            } 


            if( msg is F1.Messages.System.Notice )
            {
                LongPacket packet = new LongPacket(h, _infile);

                Test((byte[])packet.Data.Clone(), StringUtils.StringToASCIIBytes(_rawMessage));
                Crack(packet.Data, StringUtils.StringToASCIIBytes(_rawMessage));
            }
            else
            {
                Console.WriteLine("Notice is not the first encrypted packet.");
            }
        }

        //0xf8ffd6db
        //f8 ff d6 db

        void Test( byte [] encryptedData, byte [] unencryptedData )
        {
            DataDecryptor d = new DataDecryptor {Key = 0xf8ffd6db};
            d.Reset();
            
            d.DecryptData(encryptedData,0,encryptedData.Length);
        }

        void Crack( byte[] encryptedData, byte [] unencryptedData )
        {
            byte[] temp = new byte[encryptedData.Length];

            for(int i=0; i < temp.Length; ++i)
            {
                temp[i] = (byte)(encryptedData[i] ^ unencryptedData[i]);
            }

            Console.WriteLine("B> " + HexString.BytesToHex(unencryptedData));
            Console.WriteLine("*> " + HexString.BytesToHex(encryptedData));
            Console.WriteLine("^> " + HexString.BytesToHex(temp));

            uint shiftedSalt = (SALT >> 1);

            byte first1OfKey = (byte)(temp[0] ^ (byte)(shiftedSalt & 0xFF));

            Console.WriteLine("First Byte of Key = " + first1OfKey.ToString("x2"));
        }


        void UpdateEventType(IMessage msg)
        {
            if (msg is F1.Messages.System.EventId)
            {
                F1.Messages.System.EventId emsg = (F1.Messages.System.EventId)msg;

                _evtType = emsg.EventType;
            }
        }


        static void Main(string[] args)
        {
            if (0 == args.Length)
            {
                Console.Error.WriteLine("You must provide the file path");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.Error.WriteLine("File {0} doesn't exist.", args[0]);
                return;
            }

            string rawMessage = MESSAGE;

            if( args.Length > 1 )
            {
                rawMessage = args[1];
            }


            Program p = new Program(args[0], rawMessage);
            p.Run();
        }
    }
}

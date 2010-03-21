using System.IO;
using System;
using F1.Data;
using F1.Data.Packets;
using F1.Messages;

namespace KeyCracker
{
    class Program
    {
        Stream _infile;
        F1.Enums.EventType _evtType = F1.Enums.EventType.Race;


        public Program(string file)
        {
            _infile = File.OpenRead(file);           
        }


        void Run()
        {            
            while (true)
            {
                Header h = new Header(_infile);

                IMessage msg;
                if (h.IsCarMessage)
                {
                    msg = MessageFactory.CreateMessage(h.SystemType);
                    UpdateEventType(msg);
                }
                else
                {
                    msg = MessageFactory.CreateMessage(h.CarType, _evtType);
                }

                Console.WriteLine(msg);

                try
                {
                    Packet p = msg.CreatePacketType(h, _infile, null);
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("Found first encrypted msg.");
                    break;
                }
            } 
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
            }

            if (!File.Exists(args[0]))
            {
                Console.Error.WriteLine("File {0} doesn't exist.", args[0]);
            }

            Program p = new Program(args[0]);
            p.Run();
        }
    }
}

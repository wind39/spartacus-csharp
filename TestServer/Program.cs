using System;
using Spartacus;

namespace TestServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Spartacus.Net.Server v_server;
            Spartacus.Net.Packet v_packet;

            v_server = new Spartacus.Net.Server(args[0], 3900);
            v_server.Accept();

            v_packet = v_server.Recv();
            switch (v_packet.v_type)
            {
                case Spartacus.Net.PacketType.QUERY:
                    System.Console.WriteLine("Recebido: QUERY");
                    break;
                case Spartacus.Net.PacketType.DATA:
                    System.Console.WriteLine("Recebido: DATA");
                    break;
                case Spartacus.Net.PacketType.ACK:
                    System.Console.WriteLine("Recebido: ACK");
                    break;
                case Spartacus.Net.PacketType.NACK:
                    System.Console.WriteLine("Recebido: NACK");
                    break;
                case Spartacus.Net.PacketType.WARNING:
                    System.Console.WriteLine("Recebido: WARNING");
                    break;
                case Spartacus.Net.PacketType.ERROR:
                    System.Console.WriteLine("Recebido: ERROR");
                    break;
            }
            System.Console.WriteLine("Recebido: {0}", v_packet.GetString());

            v_packet = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK);
            v_server.Send(v_packet);

            v_server.Stop();
            v_server.StopServer();
        }
    }
}

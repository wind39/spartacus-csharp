using System;
using Spartacus;

namespace TestClient
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Spartacus.Net.Client v_client;
            Spartacus.Net.Packet v_packet;

            v_client = new Spartacus.Net.Client(args[0], 3900, args[1], 3901);
            v_client.Connect();

            v_packet = new Spartacus.Net.Packet(Spartacus.Net.PacketType.QUERY, 0, 1, "select * from tabelas");
            v_client.Send(v_packet);

            v_packet = v_client.Recv();
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

            v_client.Stop();
        }
    }
}

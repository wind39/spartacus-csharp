using System;

namespace SpartacusDataServer
{
    class MainClass
    {
        // args[0] = ip
        // args[1] = porta
        // args[2] = base
        // args[3] = usuario
        // args[4] = senha
        public static void Main(string[] args)
        {
            Spartacus.Database.Generic v_database;
            Spartacus.Net.Server v_server;
            Spartacus.Net.Packet v_packet, v_packetack, v_packetnack, v_packeterror;
            System.Data.DataTable v_table;

            if (args.Length != 5)
            {
                System.Console.WriteLine("Uso: ./server <ip> <porta> <base> <usuario> <senha>");
                System.Environment.Exit(0);
            }

            // criando pacotes ACK e NACK
            v_packetack = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK);
            v_packetnack = new Spartacus.Net.Packet(Spartacus.Net.PacketType.NACK);

            try
            {
                // iniciando servidor
                v_server = new Spartacus.Net.Server(args[0], System.Int32.Parse(args[1]));
                System.Console.WriteLine("Servidor iniciado no endereço {0}, ouvindo na porta {1}.", args[0], args[1]);

                try
                {
                    // conectando-se ao banco de dados
                    v_database = new Spartacus.Database.Odbc(args[2], args[3], args[4]);
                    System.Console.WriteLine("Conectado ao banco de dados {0}/{1}@{2}.", args[3], args[4], args[2]);

                    System.Console.Write("Aguardando conexões de clientes... ");
                    v_server.Accept();
                    System.Console.WriteLine("Cliente conectado.");

                    while (true)
                    {
                        System.Console.Write("Aguardando comandos... ");

                        v_packet = v_server.Recv();

                        switch (v_packet.v_type)
                        {
                            case Spartacus.Net.PacketType.QUERY:
                                System.Console.WriteLine("Recebido: QUERY.");
                                System.Console.Write("Enviando ACK... ");
                                v_server.Send(v_packetack);
                                System.Console.WriteLine("Enviei ACK.");

                                try
                                {
                                    System.Console.WriteLine("Executando consulta... ");
                                    System.Console.WriteLine(v_packet.GetString());
                                    v_table = v_database.Query(v_packet.GetString(), "RESULTS");
                                    System.Console.WriteLine("Consulta executada.");

                                    System.Console.Write("Enviando dados... ");
                                    v_server.SendDataTable(v_table);
                                    System.Console.WriteLine("Dados enviados.");
                                }
                                catch (Spartacus.Database.Exception exc_database)
                                {
                                    System.Console.WriteLine("ERRO! " + exc_database.v_message);
                                    v_packeterror = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ERROR, 0, 1, exc_database.v_message);
                                    v_server.Send(v_packeterror);
                                }
                                catch (System.Exception exc)
                                {
                                    System.Console.WriteLine("ERRO! " + exc.Message);
                                    v_packeterror = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ERROR, 0, 1, exc.Message);
                                    v_server.Send(v_packeterror);
                                }

                                break;
                            case Spartacus.Net.PacketType.DATA:
                                System.Console.WriteLine("Recebido: DATA. Não sei como tratar!");
                                System.Console.Write("Enviando NACK... ");
                                v_server.Send(v_packetnack);
                                System.Console.WriteLine("Enviei NACK.");
                                break;
                            case Spartacus.Net.PacketType.ACK:
                                System.Console.WriteLine("Recebido: ACK. Não sei como tratar!");
                                System.Console.Write("Enviando NACK... ");
                                v_server.Send(v_packetnack);
                                System.Console.WriteLine("Enviei NACK.");
                                break;
                            case Spartacus.Net.PacketType.NACK:
                                System.Console.WriteLine("Recebido: NACK. Não sei como tratar!");
                                System.Console.Write("Enviando NACK... ");
                                v_server.Send(v_packetnack);
                                System.Console.WriteLine("Enviei NACK.");
                                break;
                            case Spartacus.Net.PacketType.WARNING:
                                System.Console.WriteLine("Recebido: WARNING. Não sei como tratar!");
                                System.Console.Write("Enviando NACK... ");
                                v_server.Send(v_packetnack);
                                System.Console.WriteLine("Enviei NACK.");
                                break;
                            case Spartacus.Net.PacketType.ERROR:
                                System.Console.WriteLine("Recebido: ERROR. Não sei como tratar!");
                                System.Console.Write("Enviando NACK... ");
                                v_server.Send(v_packetnack);
                                System.Console.WriteLine("Enviei NACK.");
                                break;
                            default:
                                System.Console.WriteLine("Recebido: Desconhecido. Não sei como tratar!");
                                System.Console.Write("Enviando NACK... ");
                                v_server.Send(v_packetnack);
                                System.Console.WriteLine("Enviei NACK.");
                                break;
                        }
                    }
                }
                catch (Spartacus.Database.Exception exc_database)
                {
                    System.Console.WriteLine("Problema ao se conectar com o banco de dados. {0}", exc_database.v_message);
                    System.Environment.Exit(0);
                }
                catch (System.Exception exc)
                {
                    System.Console.WriteLine("Problema ao se conectar com o banco de dados. {0}", exc.Message);
                    System.Environment.Exit(0);
                }
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                System.Console.WriteLine("Problema ao iniciar o servidor. {0}", exc_net.v_message);
                System.Environment.Exit(0);
            }
            catch (System.Exception exc)
            {
                System.Console.WriteLine("Problema ao iniciar o servidor. {0}", exc.Message);
                System.Environment.Exit(0);
            }
        }
    }
}

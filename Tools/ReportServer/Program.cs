/*
The MIT License (MIT)

Copyright (c) 2014,2015 William Ivanski

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace Spartacus.Tools.ReportServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            (new MainClass()).Initialize(args);
        }

        private Spartacus.Net.Server v_server;
        private System.Collections.ArrayList v_percents;
        private System.Collections.ArrayList v_messages;
        private System.Collections.ArrayList v_errors;

        public void Initialize(string[] p_args)
        {
            this.v_percents = new System.Collections.ArrayList();
            this.v_messages = new System.Collections.ArrayList();
            this.v_errors = new System.Collections.ArrayList();

            if (p_args.Length == 2)
                this.v_server = new Spartacus.Net.Server(p_args[0], int.Parse(p_args[1]));
            else
                this.v_server = new Spartacus.Net.Server(
                    System.Configuration.ConfigurationManager.AppSettings["ip"].ToString(),
                    int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"].ToString())
                );

            this.v_server.v_connect.ConnectEvent += new Spartacus.Net.ConnectEventClass.ConnectEventHandler(this.OnConnect);
            this.v_server.v_disconnect.DisconnectEvent += new Spartacus.Net.DisconnectEventClass.DisconnectEventHandler(this.OnDisconnect);
            this.v_server.v_available.AvailableEvent += new Spartacus.Net.AvailableEventClass.AvailableEventHandler(this.OnAvailable);

            this.v_server.Accept();

            Console.WriteLine("Servidor escutando em {0}:{1}", this.v_server.v_ip, this.v_server.v_port);

            while (true)
            {
            }
        }

        private void OnConnect(Spartacus.Net.ConnectEventClass obj, Spartacus.Net.ConnectEventArgs e)
        {
            Console.WriteLine(
                "[{0}:{1}] [{2}]: Cliente {3}:{4} conectou-se",
                e.v_serverip,
                e.v_serverport,
                System.DateTime.Now.ToString(),
                e.v_clientip,
                e.v_clientport
            );

            this.v_percents.Add((double)0);
            this.v_messages.Add("Aguardando início");
            this.v_errors.Add(false);
        }

        private void OnDisconnect(Spartacus.Net.DisconnectEventClass obj, Spartacus.Net.DisconnectEventArgs e)
        {
            Console.WriteLine(
                "[{0}:{1}] [{2}]: Cliente {3}:{4} desconectou-se",
                e.v_serverip,
                e.v_serverport,
                System.DateTime.Now.ToString(),
                e.v_clientip,
                e.v_clientport
            );

            //this.v_percents.RemoveAt(e.v_index);
            //this.v_messages.RemoveAt(e.v_index);
            //this.v_errors.RemoveAt(e.v_index);
        }

        private void OnAvailable(Spartacus.Net.AvailableEventClass obj, Spartacus.Net.AvailableEventArgs e)
        {
            string v_received_data;

            v_received_data = this.v_server.RecvString(e.v_index);
            Console.WriteLine(
                "[{0}:{1}] [{2}]: Mensagem recebida do cliente {3}:{4} = {5}",
                e.v_serverip,
                e.v_serverport,
                System.DateTime.Now.ToString(),
                e.v_clientip,
                e.v_clientport,
                v_received_data
            );

            switch(v_received_data.Substring(0, 1))
            {
                case "R":
                    this.HandleReport(e.v_index, v_received_data);
                    break;
                case "P":
                    this.v_server.SendString(((double)this.v_percents[e.v_index]).ToString() + ";" + (string)this.v_messages[e.v_index] + ";" + (bool)this.v_errors[e.v_index]);
                    break;
                default:
                    this.v_server.Send(e.v_index, new Spartacus.Net.Packet(Spartacus.Net.PacketType.NACK));
                    break;
            }
        }

        private void HandleReport(int p_index, string p_received_data)
        {
            Spartacus.Reporting.Report v_report;
            string[] v_options;
            string[] v_parameters;
            string[] v_param;

            v_options = p_received_data.Split(';');
            if (v_options.Length != 5)
            {
                this.v_server.Send(p_index, new Spartacus.Net.Packet(Spartacus.Net.PacketType.NACK));
                return;
            }

            try
            {
                v_report = new Spartacus.Reporting.Report(int.Parse(v_options[1]), v_options[2]);

                v_parameters = v_options[4].Split(',');
                if (v_parameters.Length != v_report.v_cmd.v_parameters.Count)
                {
                    this.v_server.Send(p_index, new Spartacus.Net.Packet(Spartacus.Net.PacketType.NACK));
                    return;
                }

                for (int k = 0; k < v_parameters.Length; k++)
                {
                    v_param = v_parameters[k].Split('=');
                    if (v_param.Length != 2)
                    {
                        this.v_server.Send(p_index, new Spartacus.Net.Packet(Spartacus.Net.PacketType.NACK));
                        return;
                    }

                    v_report.v_cmd.SetValue(v_param[0], v_param[1]);
                }

                this.v_server.Send(p_index, new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK));

                v_report.v_progress.ProgressEventArgs.v_index = p_index;
                v_report.v_progress.ProgressEvent += new Spartacus.Utils.ProgressEventClass.ProgressEventHandler(this.OnReportProgress);
                v_report.Execute();

                if (v_report.v_table.Rows.Count == 0)
                {
                    this.v_percents[p_index] = 100.0;
                    this.v_messages[p_index] = "Relatorio nao sera salvo em PDF porque nao possui dados.";
                }
                else
                {
                    v_report.Save(v_options[3]);

                    this.v_percents[p_index] = 100.0;
                    this.v_messages[p_index] = "Pronto!";
                }
            }
            catch (Spartacus.Reporting.Exception exc)
            {
                this.v_messages[p_index] = exc.v_message;
                this.v_errors[p_index] = true;
            }
            catch (Spartacus.Database.Exception exc)
            {
                this.v_messages[p_index] = exc.v_message;
                this.v_errors[p_index] = true;
            }
            catch (Spartacus.Net.Exception exc)
            {
                this.v_messages[p_index] = exc.v_message;
                this.v_errors[p_index] = true;
            }
            catch (System.Exception exc)
            {
                this.v_messages[p_index] = exc.Message;
                this.v_errors[p_index] = true;
            }
        }

        private void OnReportProgress(Spartacus.Utils.ProgressEventClass sender, Spartacus.Utils.ProgressEventArgs e)
        {
            this.v_percents[e.v_index] = e.v_percentage;
            this.v_messages[e.v_index] = e.v_message;
        }
    }
}

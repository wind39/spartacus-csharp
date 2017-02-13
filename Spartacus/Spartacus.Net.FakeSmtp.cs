/*
The MIT License (MIT)

Copyright (c) 2014-2016 William Ivanski

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

namespace Spartacus.Net
{
	public class FakeSmtp : Spartacus.Net.Server
	{
        public Spartacus.Net.MailClient v_mailclient;

        public Spartacus.Database.Generic v_database;

        public string v_select;

        public Spartacus.Database.Command v_insert;

        public string v_realhost;

        public int v_realport;

        public System.Net.NetworkCredential v_credential;

        public bool v_log;

        public bool v_logeml;

        public bool v_redirect;

        public bool v_whitelist;


		public FakeSmtp(
            string p_ip,
            int p_port
        ) : base (p_ip, p_port)
		{
			this.v_connect.ConnectEvent += new Spartacus.Net.ConnectEventClass.ConnectEventHandler(OnConnect);

            this.v_mailclient = new Spartacus.Net.MailClient();
            this.v_database = null;
            this.v_select = null;
            this.v_insert = null;
            this.v_credential = null;

            this.v_realhost = null;
            this.v_realport = -1;
            this.v_log = true;
            this.v_logeml = false;
            this.v_redirect = false;
            this.v_whitelist = false;

			Console.WriteLine("Spartacus FakeSMTP {0}:{1}", this.v_ip, this.v_port);
			Console.WriteLine();
		}

        public FakeSmtp(
            string p_ip,
            int p_port,
            string p_realhost,
            int p_realport,
            bool p_log,
            bool p_logeml,
            bool p_redirect
        ) : base (p_ip, p_port)
        {
            this.v_connect.ConnectEvent += new Spartacus.Net.ConnectEventClass.ConnectEventHandler(OnConnect);

            this.v_mailclient = new Spartacus.Net.MailClient();
            this.v_database = null;
            this.v_select = null;
            this.v_insert = null;
            this.v_credential = null;

            this.v_realhost = p_realhost;
            this.v_realport = p_realport;
            this.v_log = p_log;
            this.v_logeml = p_logeml;
            this.v_redirect = p_redirect;
            this.v_whitelist = false;

            if (this.v_log)
            {
                Console.WriteLine("Spartacus FakeSMTP {0}:{1}", this.v_ip, this.v_port);
                Console.WriteLine();
            }
        }

        public FakeSmtp(
            string p_ip,
            int p_port,
            string p_realhost,
            int p_realport,
            bool p_log,
            bool p_logeml,
            bool p_redirect,
            bool p_whitelist,
            Spartacus.Database.Generic p_database,
            string p_select,
            Spartacus.Database.Command p_insert
        ) : base (p_ip, p_port)
        {
            this.v_connect.ConnectEvent += new Spartacus.Net.ConnectEventClass.ConnectEventHandler(OnConnect);

            this.v_mailclient = new Spartacus.Net.MailClient();
            this.v_database = p_database;
            this.v_select = p_select;
            this.v_insert = p_insert;
            this.v_credential = null;

            this.v_realhost = p_realhost;
            this.v_realport = p_realport;
            this.v_log = p_log;
            this.v_logeml = p_logeml;
            this.v_redirect = p_redirect;
            this.v_whitelist = p_whitelist;

            if (this.v_log)
            {
                Console.WriteLine("Spartacus FakeSMTP {0}:{1}", this.v_ip, this.v_port);
                Console.WriteLine();
            }
        }

        public FakeSmtp(
            string p_ip,
            int p_port,
            string p_realhost,
            int p_realport,
            System.Net.NetworkCredential p_credential,
            bool p_log,
            bool p_logeml,
            bool p_redirect,
            bool p_whitelist,
            Spartacus.Database.Generic p_database,
            string p_select,
            Spartacus.Database.Command p_insert
        ) : base (p_ip, p_port)
        {
            this.v_connect.ConnectEvent += new Spartacus.Net.ConnectEventClass.ConnectEventHandler(OnConnect);

            this.v_mailclient = new Spartacus.Net.MailClient();
            this.v_database = p_database;
            this.v_select = p_select;
            this.v_insert = p_insert;
            this.v_credential = p_credential;

            this.v_realhost = p_realhost;
            this.v_realport = p_realport;
            this.v_log = p_log;
            this.v_logeml = p_logeml;
            this.v_redirect = p_redirect;
            this.v_whitelist = p_whitelist;

            if (this.v_log)
            {
                Console.WriteLine("Spartacus FakeSMTP {0}:{1}", this.v_ip, this.v_port);
                Console.WriteLine();
            }
        }

		private void OnConnect(Spartacus.Net.ConnectEventClass obj, Spartacus.Net.ConnectEventArgs e)
		{
            System.IO.StreamReader v_reader;
            System.IO.StreamWriter v_writer;
            System.Text.StringBuilder v_data;
            Spartacus.Utils.Cryptor v_cryptor;
            System.Net.Mail.MailMessage v_message;
            string v_line = null;
            System.Data.DataTable v_table;
            bool v_achou;
            int k, v_inserted;

            if (this.v_log)
                Console.WriteLine("Cliente {0} - {1}:{2} conectado", e.v_index, e.v_clientip, e.v_clientport);

            v_reader = new System.IO.StreamReader(this.v_streams[e.v_index]);
            v_writer = new System.IO.StreamWriter(this.v_streams[e.v_index]);
            v_writer.NewLine = "\r\n";
            v_writer.AutoFlush = true;

            v_cryptor = new Spartacus.Utils.Cryptor(null);

            v_message = new System.Net.Mail.MailMessage();
            v_message.BodyEncoding = System.Text.Encoding.UTF8;
            v_message.IsBodyHtml = false;

            try
            {
                v_writer.WriteLine("220 Spartacus FakeSMTP Server");

                while (v_reader != null)
                {
                    v_line = v_reader.ReadLine();
					if (this.v_log)
						Console.WriteLine("Client: {0} - Line: [{1}]", e.v_index, v_line);

					if (!string.IsNullOrWhiteSpace(v_line))
                    {
                        if (v_line.StartsWith("HELO") || v_line.StartsWith("EHLO"))
                        {
                            if (this.v_credential == null)
                            {
                                this.v_credential = new System.Net.NetworkCredential("", "");

                                v_writer.WriteLine("250 AUTH LOGIN");
                                v_line = v_reader.ReadLine();

                                v_writer.WriteLine("334 VXNlcm5hbWU6");
                                this.v_credential.UserName = v_cryptor.Base64Decode(v_reader.ReadLine());

                                v_writer.WriteLine("334 UGFzc3dvcmQ6");
                                this.v_credential.Password = v_cryptor.Base64Decode(v_reader.ReadLine());

                                v_writer.WriteLine("235 OK");
                            }
                            else
                                v_writer.WriteLine("250 OK");
                        }
                        else if (v_line.StartsWith("MAIL FROM:"))
                        {
                            v_message.From = new System.Net.Mail.MailAddress(v_line.Substring("MAIL FROM:".Length+1, v_line.Length-("MAIL FROM:".Length+2)).Replace("<", "").Replace(">", ""));
                            v_writer.WriteLine("250 OK");
                        }
                        else if (v_line.StartsWith("RCPT TO:"))
                        {
                            v_message.To.Add(new System.Net.Mail.MailAddress(v_line.Substring("RCPT TO:".Length+1, v_line.Length-("RCPT TO:".Length+2)).Replace("<", "").Replace(">", "")));
                            v_writer.WriteLine("250 OK");
                        }
                        else if (v_line == "DATA")
                        {
                            v_writer.WriteLine("354 Start input, end data with <CRLF>.<CRLF>");

                            v_data = new System.Text.StringBuilder();

                            v_line = v_reader.ReadLine();

                            while (v_line != null && v_line != ".")
                            {
                                if (v_line.StartsWith("Subject: "))
                                    v_message.Subject = v_line.Substring("Subject: ".Length);

                                v_data.AppendLine(v_line);
                                v_line = v_reader.ReadLine();
                            }

                            v_message.Body = v_data.ToString();

                            // logando mensagem
                            if (this.v_log)
                            {
                                Console.WriteLine("===============================================================================");
								Console.WriteLine("{0}: Currently connected clients: {1}", System.DateTime.Now, this.v_numclients);
                                Console.WriteLine("All good, received ­email");
                                Console.WriteLine("-------------------------------------------------------------------------------");
                                Console.WriteLine("User: {0}", this.v_credential.UserName);
                                Console.WriteLine("Password: {0}", this.v_credential.Password);
                                Console.WriteLine("-------------------------------------------------------------------------------");
                                Console.WriteLine("From: " + v_message.From);
                                Console.Write("To: ");
                                foreach (System.Net.Mail.MailAddress m in v_message.To)
                                    Console.Write("{0} ", m.Address);
                                Console.WriteLine();
                                Console.WriteLine("Subject: " + v_message.Subject);
                                if (this.v_logeml)
                                {
                                    Console.WriteLine("-------------------------------------------------------------------------------");
                                    Console.WriteLine(v_message.Body);
									Console.WriteLine("-------------------------------------------------------------------------------");
                                }
								Console.WriteLine("===============================================================================");
                            }

                            // redirecionando mensagem
                            if (this.v_redirect)
                            {
                                if (this.v_log)
                                    Console.Write("Sending message to {0}:{1}... ", this.v_realhost, this.v_realport);

                                this.v_mailclient.Send(this.v_realhost, this.v_realport, v_credential, v_message.Body);

                                if (this.v_log)
                                    Console.WriteLine("OK");
                            }

                            // inserindo enderecos na whitelist
                            if (this.v_whitelist)
                            {
                                if (this.v_log)
                                    Console.Write("Updating whitelist... ");

                                v_inserted = 0;
                                this.v_database.Open();
                                v_table = this.v_database.Query(this.v_select, "WHITELIST");

                                k = 0;
                                v_achou = false;
                                while (k < v_table.Rows.Count && !v_achou)
                                {
                                    if (v_table.Rows[k][0].ToString() == v_message.From.Address)
                                        v_achou = true;
                                    else
                                        k++;
                                }
                                if (!v_achou)
                                {
                                    this.v_insert.SetValue(0, v_message.From.Address);
                                    this.v_database.Execute(this.v_insert.GetUpdatedText());
                                    v_inserted++;
                                }

                                foreach (System.Net.Mail.MailAddress m in v_message.To)
                                {
                                    if (m.Address != v_message.From.Address)
                                    {
                                        k = 0;
                                        v_achou = false;
                                        while (k < v_table.Rows.Count && !v_achou)
                                        {
                                            if (v_table.Rows[k][0].ToString() == m.Address)
                                                v_achou = true;
                                            else
                                                k++;
                                        }
                                        if (!v_achou)
                                        {
                                            this.v_insert.SetValue(0, m.Address);
                                            this.v_database.Execute(this.v_insert.GetUpdatedText());
                                            v_inserted++;
                                        }
                                    }
                                }

                                this.v_database.Close();
                                if (this.v_log)
                                    Console.WriteLine("OK, {0} addresses inserted on whitelist.", v_inserted);
                            }

                            if (this.v_log)
                                Console.WriteLine();

                            v_writer.WriteLine("250 OK");
                        }
                        else if (v_line == "QUIT")
                        {
                            v_writer.WriteLine("250 OK");
                            v_reader = null;
							if (this.v_log)
								Console.WriteLine();
                        }
                        else
                        {
                            if (this.v_log)
                                Console.WriteLine("UNHANDLED: {0}", v_line);
                            v_writer.WriteLine("250 OK");
                        }
                    }
                    else
                    {
                        if (this.v_log)
                            Console.WriteLine("EMPTY: {0}", v_line);
                        v_writer.WriteLine("250 OK");
                    }
                }
            }
            catch(System.Exception exc)
            {
				if (this.v_log)
					Console.WriteLine("ERROR: {0}", exc.Message);
            }
            finally
            {
                this.StopClient(e.v_index);
            }
		}
	}
}

using System;

namespace Spartacus.Net
{
    public class Endpoint
    {
        public string v_ip;

        public int v_port;

        public System.Net.Sockets.TcpClient v_socket;

        public System.Net.Sockets.NetworkStream v_stream;

        public readonly int v_buffersize;

        public byte[] v_recvbuffer;


        public Endpoint(string p_ip, int p_port)
        {
            this.v_ip = p_ip;
            this.v_port = p_port;

            this.v_socket = null;
            this.v_stream = null;

            this.v_buffersize = 1048576;

            this.v_recvbuffer = new byte[this.v_buffersize];
        }

        #region RECEIVE

        public Spartacus.Net.Packet Recv()
        {
            byte[] v_tmpbuffer;
            int v_numbytesrecv;
            string v_context;

            try
            {
                v_numbytesrecv = this.v_stream.Read(this.v_recvbuffer, 0, this.v_recvbuffer.Length);

                v_tmpbuffer = new byte[v_numbytesrecv];
                System.Array.Copy(this.v_recvbuffer, 0, v_tmpbuffer, 0, v_numbytesrecv);

                return new Packet(v_tmpbuffer);
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                throw exc_net;
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        public string RecvString()
        {
            string v_text;
            Spartacus.Net.Packet v_packetrecv, v_packetsend;
            int v_numpackets, v_sequence;
            string v_context;

            try
            {
                v_packetrecv = this.Recv();

                v_sequence = 0;
                v_numpackets = v_packetrecv.v_numpackets;

                // se a sequencia estah errada, entao precisa tratar um erro
                // if (v_packet.v_sequence != v_sequence)

                v_text = v_packetrecv.GetString();

                // enviando ack
                v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                this.Send(v_packetsend);

                v_sequence++;
                while (v_sequence < v_numpackets)
                {
                    // recebendo pacote
                    v_packetrecv = this.Recv();

                    // se a sequencia estah errada, entao precisa tratar um erro
                    // if (v_packet.v_sequence != v_sequence)

                    // se o numero de pacotes estah errado, entao precisa tratar um erro
                    // if (v_packet.v_numpackets != v_numpackets)

                    // acumulando conteudo dos dados do pacote na string
                    v_text += v_packetrecv.GetString();

                    // enviando ack
                    v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                    this.Send(v_packetsend);

                    v_sequence++;
                }

                return v_text;
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                throw exc_net;
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        /*public System.Data.DataTable RecvDataTable()
        {
            System.Data.DataTable v_table;
            System.Data.DataRow v_row;
            string v_text;
            string[] v_lines;
            string[] v_line;
            int i, j;
            string[] v_sep;
            string v_context;

            v_sep = new string[1];
            v_sep [0] = "\r\n";

            try
            {
                v_text = this.RecvString();

                v_table = new System.Data.DataTable("RESULTS");

                v_lines = v_text.Split(v_sep, System.StringSplitOptions.None);

                v_line = v_lines[0].Split(';');

                foreach (string s in v_line)
                    v_table.Columns.Add(s);

                for (i = 1; i < v_lines.Length; i++)
                {
                    v_row = v_table.NewRow();
                    v_line = v_lines[i].Split(';');

                    if (v_line.Length != v_table.Columns.Count)
                    {
                        v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                        throw new Spartacus.Net.Exception(v_context, "Numero de colunas diferente na linha " + i.ToString() + ". " + v_table.Columns.Count.ToString() + " x " + v_line.Length.ToString() + ".");
                    }

                    for (j = 0; j < v_table.Columns.Count; j++)
                        v_row[j] = v_line[j];

                    v_table.Rows.Add(v_row);
                }

                return v_table;
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                throw exc_net;
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }*/

        public System.Data.DataTable RecvDataTable()
        {
            System.Data.DataTable v_table;
            System.Data.DataRow v_row;
            Spartacus.Net.Packet v_packetrecv, v_packetsend;
            int v_numpackets, v_sequence;
            string[] v_fields;
            int k;
            string v_context;

            try
            {
                v_packetrecv = this.Recv();

                v_sequence = 0;
                v_numpackets = v_packetrecv.v_numpackets;

                // se a sequencia estah errada, entao precisa tratar um erro
                // if (v_packet.v_sequence != v_sequence)

                // criando tabela
                v_table = new System.Data.DataTable();

                // adicionando colunas
                v_fields = v_packetrecv.GetString().Split(';');
                foreach(string v_column in v_fields)
                    v_table.Columns.Add(v_column);

                // enviando ack
                v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                this.Send(v_packetsend);

                v_sequence++;
                while (v_sequence < v_numpackets)
                {
                    // recebendo pacote
                    v_packetrecv = this.Recv();

                    // se a sequencia estah errada, entao precisa tratar um erro
                    // if (v_packet.v_sequence != v_sequence)

                    // se o numero de pacotes estah errado, entao precisa tratar um erro
                    // if (v_packet.v_numpackets != v_numpackets)

                    v_row = v_table.NewRow();
                    v_fields = v_packetrecv.GetString().Split(';');
                     
                    if (v_fields.Length != v_table.Columns.Count)
                    {
                        v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                        throw new Spartacus.Net.Exception(v_context, "Numero de colunas diferente na linha " + v_sequence.ToString() + ". " + v_table.Columns.Count.ToString() + " x " + v_fields.Length.ToString() + ".");
                    }

                    for (k = 0; k < v_table.Columns.Count; k++)
                        v_row[k] = v_fields[k];

                    v_table.Rows.Add(v_row);

                    // enviando ack
                    v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                    this.Send(v_packetsend);

                    v_sequence++;
                }

                return v_table;
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                throw exc_net;
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        #endregion

        #region SEND

        public void Send(Spartacus.Net.Packet p_packet)
        {
            string v_context;

            try
            {
                this.v_stream.Write(p_packet.v_buffer, 0, p_packet.v_buffer.Length);
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                throw exc_net;
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        public void SendString(string p_text)
        {
            Spartacus.Net.Packet v_packetsend, v_packetrecv;
            int v_chunksize;
            int v_numpackets, v_sequence;
            int k;
            string v_context;
            bool v_ack;

            try
            {
                // tamanho dos dados eh igual ao tamanho total do pacote menos o cabecalho (25)
                v_chunksize = this.v_buffersize - 25;

                // numero de pacotes
                v_numpackets = (int) System.Math.Ceiling((double) p_text.Length / (double) v_chunksize);

                v_sequence = 0;
                for (k = 0; k < p_text.Length; k += v_chunksize)
                {
                    // se eh o ultimo pacote, entao o tamanho dos dados serah menor
                    if ((k + v_chunksize) > p_text.Length)
                        v_chunksize = p_text.Length - k;

                    // montando pacote
                    v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.DATA, v_sequence, v_numpackets, p_text.Substring(k, v_chunksize));

                    // enquanto nao recebeu ACK, precisa reenviar ate receber
                    v_ack = false;
                    while (! v_ack)
                    {
                        // enviando pacote
                        this.Send(v_packetsend);

                        // recebendo ACK
                        v_packetrecv = this.Recv();
                        if (v_packetrecv.v_type == Spartacus.Net.PacketType.ACK && v_packetrecv.v_sequence == v_sequence)
                            v_ack = true;
                    }

                    v_sequence++;
                }
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                throw exc_net;
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        public void SendDataTable(System.Data.DataTable p_table)
        {
            Spartacus.Net.Packet v_packetsend, v_packetrecv;
            int v_sequence, v_numpackets;
            string v_text;
            int i, j;
            bool v_ack;
            string v_context;

            try
            {
                v_numpackets = p_table.Rows.Count + 1;

                // ENVIANDO NOMES DE COLUNAS

                v_sequence = 0;

                v_text = p_table.Columns[0].ColumnName;
                for (j = 1; j < p_table.Columns.Count; j++)
                    v_text += ";" + p_table.Columns[j].ColumnName;

                // montando pacote
                v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.DATA, v_sequence, v_numpackets, v_text);

                // enquanto nao recebeu ACK, precisa reenviar ate receber
                v_ack = false;
                while (! v_ack)
                {
                    // enviando pacote
                    this.Send(v_packetsend);

                    // recebendo ACK
                    v_packetrecv = this.Recv();
                    if (v_packetrecv.v_type == Spartacus.Net.PacketType.ACK && v_packetrecv.v_sequence == v_sequence)
                        v_ack = true;
                }

                v_sequence++;

                // ENVIANDO LINHAS

                for (i = 0; i < p_table.Rows.Count; i++)
                {
                    v_text = p_table.Rows[i][0].ToString();
                    for (j = 1; j < p_table.Columns.Count; j++)
                        v_text += ";" + p_table.Rows[i][j].ToString().Replace(";", ",");

                    // montando pacote
                    v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.DATA, v_sequence, v_numpackets, v_text);

                    // enquanto nao recebeu ACK, precisa reenviar ate receber
                    v_ack = false;
                    while (! v_ack)
                    {
                        // enviando pacote
                        this.Send(v_packetsend);

                        // recebendo ACK
                        v_packetrecv = this.Recv();
                        if (v_packetrecv.v_type == Spartacus.Net.PacketType.ACK && v_packetrecv.v_sequence == v_sequence)
                            v_ack = true;
                    }

                    v_sequence++;
                }
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                throw exc_net;
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        /*public void SendDataTable(System.Data.DataTable p_table)
        {
            string v_text;
            int i, j;
            string v_context;

            v_text = p_table.Columns[0].ColumnName;
            for (j = 1; j < p_table.Columns.Count; j++)
                v_text += ";" + p_table.Columns[j].ColumnName;
            v_text += "\r\n";

            for (i = 0; i < p_table.Rows.Count; i++)
            {
                v_text += p_table.Rows[i][0].ToString();
                for (j = 1; j < p_table.Columns.Count; j++)
                    v_text += ";" + p_table.Rows[i][j].ToString().Replace(";", ",").Replace("\r\n", " ");
                if (i < (p_table.Rows.Count-1))
                    v_text += "\r\n";
            }

            try
            {
                this.SendString(v_text);
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                throw exc_net;
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }*/

        #endregion

        public void Stop()
        {
            string v_context;

            try
            {
                this.v_stream.Close();
                this.v_socket.Close();
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }
    }
}

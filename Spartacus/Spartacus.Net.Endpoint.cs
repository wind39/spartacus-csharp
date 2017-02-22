/*
The MIT License (MIT)

Copyright (c) 2014-2017 William Ivanski

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
    /// <summary>
    /// Classe Endpoint.
    /// Representa um ponto de comunicação, que pode ser tanto um cliente ou um servidor.
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        /// IP do ponto de comunicação.
        /// </summary>
        public string v_ip;

        /// <summary>
        /// Porta do ponto de comunicação.
        /// </summary>
        public int v_port;

        /// <summary>
        /// Sockets usado para comunicação.
        /// </summary>
        public System.Collections.Generic.List<System.Net.Sockets.TcpClient> v_sockets;

        /// <summary>
        /// Streams usadas para comunicação.
        /// </summary>
        public System.Collections.Generic.List<System.Net.Sockets.NetworkStream> v_streams;

        /// <summary>
        /// Tamanho do buffer para envio e recebimento.
        /// O padrão é 1 MB.
        /// </summary>
        public readonly int v_buffersize;

        /// <summary>
        /// Buffer usado para envio e recebimento.
        /// </summary>
        public byte[] v_recvbuffer;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Net.Endpoint"/>.
        /// </summary>
        /// <param name="p_ip">IP do ponto de comunicação.</param>
        /// <param name="p_port">Porta do ponto de comunicação.</param>
        public Endpoint(string p_ip, int p_port)
        {
            this.v_ip = p_ip;
            this.v_port = p_port;

            this.v_sockets = new System.Collections.Generic.List<System.Net.Sockets.TcpClient>();
            this.v_streams = new System.Collections.Generic.List<System.Net.Sockets.NetworkStream>();

            this.v_buffersize = 1048576;

            this.v_recvbuffer = new byte[this.v_buffersize];
        }

        #region RECEIVE

        /// <summary>
        /// Recebe um pacote.
        /// </summary>
        /// <returns>Pacote.</returns>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir receber o pacote.</exception>
        public Spartacus.Net.Packet Recv(int p_endpoint)
        {
            byte[] v_tmpbuffer;
            int v_numbytesrecv;

            try
            {
                v_numbytesrecv = this.v_streams[p_endpoint].Read(this.v_recvbuffer, 0, this.v_recvbuffer.Length);

                v_tmpbuffer = new byte[v_numbytesrecv];
                System.Array.Copy(this.v_recvbuffer, 0, v_tmpbuffer, 0, v_numbytesrecv);

                return new Packet(v_tmpbuffer);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Recebe um pacote.
        /// </summary>
        /// <returns>Pacote.</returns>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir receber o pacote.</exception>
        public Spartacus.Net.Packet Recv()
        {
            try
            {
                return this.Recv(0);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Recebe uma string.
        /// Pode ser necessário vários pacotes para montar essa string.
        /// </summary>
        /// <returns>String.</returns>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir receber a string.</exception>
        public string RecvString(int p_endpoint)
        {
            string v_text;
            Spartacus.Net.Packet v_packetrecv, v_packetsend;
            int v_numpackets, v_sequence;

            try
            {
                v_packetrecv = this.Recv(p_endpoint);

                v_sequence = 0;
                v_numpackets = v_packetrecv.v_numpackets;

                // se a sequencia estah errada, entao precisa tratar um erro
                // if (v_packet.v_sequence != v_sequence)

                v_text = v_packetrecv.GetString();

                // enviando ack
                v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                this.Send(p_endpoint, v_packetsend);

                v_sequence++;
                while (v_sequence < v_numpackets)
                {
                    // recebendo pacote
                    v_packetrecv = this.Recv(p_endpoint);

                    // se a sequencia estah errada, entao precisa tratar um erro
                    // if (v_packet.v_sequence != v_sequence)

                    // se o numero de pacotes estah errado, entao precisa tratar um erro
                    // if (v_packet.v_numpackets != v_numpackets)

                    // acumulando conteudo dos dados do pacote na string
                    v_text += v_packetrecv.GetString();

                    // enviando ack
                    v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                    this.Send(p_endpoint, v_packetsend);

                    v_sequence++;
                }

                return v_text;
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Recebe uma string.
        /// Pode ser necessário vários pacotes para montar essa string.
        /// </summary>
        /// <returns>String.</returns>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir receber a string.</exception>
        public string RecvString()
        {
            try
            {
                return this.RecvString(0);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Recebe uma <see cref="System.Data.DataTable"/>.
        /// Recebe um pacote por linha da <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <returns><see cref="System.Data.DataTable"/>.</returns>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir receber a <see cref="System.Data.DataTable"/>.</exception>
        public System.Data.DataTable RecvDataTable(int p_endpoint)
        {
            System.Data.DataTable v_table;
            System.Data.DataRow v_row;
            Spartacus.Net.Packet v_packetrecv, v_packetsend;
            int v_numpackets, v_sequence;
            string[] v_fields;
            int k;

            try
            {
                v_packetrecv = this.Recv(p_endpoint);

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
                this.Send(p_endpoint, v_packetsend);

                v_sequence++;
                while (v_sequence < v_numpackets)
                {
                    // recebendo pacote
                    v_packetrecv = this.Recv(p_endpoint);

                    // se a sequencia estah errada, entao precisa tratar um erro
                    // if (v_packet.v_sequence != v_sequence)

                    // se o numero de pacotes estah errado, entao precisa tratar um erro
                    // if (v_packet.v_numpackets != v_numpackets)

                    v_row = v_table.NewRow();
                    v_fields = v_packetrecv.GetString().Split(';');
                     
                    if (v_fields.Length != v_table.Columns.Count)
                    {
                        throw new Spartacus.Net.Exception("Numero de colunas diferente na linha " + v_sequence.ToString() + ". " + v_table.Columns.Count.ToString() + " x " + v_fields.Length.ToString() + ".");
                    }

                    for (k = 0; k < v_table.Columns.Count; k++)
                        v_row[k] = v_fields[k];

                    v_table.Rows.Add(v_row);

                    // enviando ack
                    v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                    this.Send(p_endpoint, v_packetsend);

                    v_sequence++;
                }

                return v_table;
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Recebe uma <see cref="System.Data.DataTable"/>.
        /// Recebe um pacote por linha da <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <returns><see cref="System.Data.DataTable"/>.</returns>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir receber a <see cref="System.Data.DataTable"/>.</exception>
        public System.Data.DataTable RecvDataTable()
        {
            try
            {
                return this.RecvDataTable(0);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Recebe um arquivo.
        /// </summary>
        /// <returns>Nome do arquivo.</returns>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir receber o arquivo.</exception>
        public string RecvFile(int p_endpoint)
        {
            System.IO.FileStream v_file;
            System.IO.BinaryWriter v_writer;
            Spartacus.Net.Packet v_packetrecv, v_packetsend;
            int v_numpackets, v_sequence;

            try
            {
                // recebendo nome do arquivo
                v_packetrecv = this.Recv(p_endpoint);

                if (v_packetrecv.v_type != Spartacus.Net.PacketType.FILE)
                    return null;

                v_file = new System.IO.FileStream(v_packetrecv.GetString(), System.IO.FileMode.Create, System.IO.FileAccess.Write);

                v_writer = new System.IO.BinaryWriter(v_file);

                // enviando ack
                v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, "");
                this.Send(p_endpoint, v_packetsend);

                // recebendo primeiro pacote de dados do arquivo
                v_packetrecv = this.Recv(p_endpoint);

                v_sequence = 0;
                v_numpackets = v_packetrecv.v_numpackets;

                // se a sequencia estah errada, entao precisa tratar um erro
                // if (v_packet.v_sequence != v_sequence)

                v_writer.Write(v_packetrecv.v_data);

                // enviando ack
                v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                this.Send(p_endpoint, v_packetsend);

                v_sequence++;
                while (v_sequence < v_numpackets)
                {
                    // recebendo pacote
                    v_packetrecv = this.Recv(p_endpoint);

                    // se a sequencia estah errada, entao precisa tratar um erro
                    // if (v_packet.v_sequence != v_sequence)

                    // se o numero de pacotes estah errado, entao precisa tratar um erro
                    // if (v_packet.v_numpackets != v_numpackets)

                    // acumulando conteudo dos dados do pacote na string
                    v_writer.Write(v_packetrecv.v_data);

                    // enviando ack
                    v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.ACK, v_sequence, v_numpackets, "");
                    this.Send(p_endpoint, v_packetsend);

                    v_sequence++;
                }

                v_writer.Flush();
                v_writer.Close();

                return v_file.Name;
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Recebe um arquivo.
        /// </summary>
        /// <returns>Nome do arquivo.</returns>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir receber o arquivo.</exception>
        public string RecvFile()
        {
            try
            {
                return this.RecvFile(0);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region SEND

        /// <summary>
        /// Envia um pacote.
        /// </summary>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        /// <param name="p_packet">Pacote.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir enviar o pacote.</exception>
        public void Send(int p_endpoint, Spartacus.Net.Packet p_packet)
        {
            try
            {
                this.v_streams[p_endpoint].Write(p_packet.v_buffer, 0, p_packet.v_buffer.Length);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Envia um pacote.
        /// </summary>
        /// <param name="p_packet">Pacote.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir enviar o pacote.</exception>
        public void Send(Spartacus.Net.Packet p_packet)
        {
            try
            {
                this.Send(0, p_packet);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Envia uma string.
        /// Pode ser necessário quebrar essa string em vários pacotes.
        /// </summary>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        /// <param name="p_text">String.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir enviar a string.</exception>
        public void SendString(int p_endpoint, string p_text)
        {
            Spartacus.Net.Packet v_packetsend, v_packetrecv;
            int v_chunksize;
            int v_numpackets, v_sequence;
            int k;
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
                        this.Send(p_endpoint, v_packetsend);

                        // recebendo ACK
                        v_packetrecv = this.Recv(p_endpoint);
                        if (v_packetrecv.v_type == Spartacus.Net.PacketType.ACK && v_packetrecv.v_sequence == v_sequence)
                            v_ack = true;
                    }

                    v_sequence++;
                }
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Envia uma string.
        /// Pode ser necessário quebrar essa string em vários pacotes.
        /// </summary>
        /// <param name="p_text">String.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir enviar a string.</exception>
        public void SendString(string p_text)
        {
            try
            {
                this.SendString(0, p_text);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Envia uma <see cref="System.Data.DataTable"/>.
        /// Envia uma linha por pacote.
        /// </summary>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        /// <param name="p_table"><see cref="System.Data.DataTable"/>.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir enviar a <see cref="System.Data.DataTable"/>.</exception>
        public void SendDataTable(int p_endpoint, System.Data.DataTable p_table)
        {
            Spartacus.Net.Packet v_packetsend, v_packetrecv;
            int v_sequence, v_numpackets;
            string v_text;
            int i, j;
            bool v_ack;

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
                    this.Send(p_endpoint, v_packetsend);

                    // recebendo ACK
                    v_packetrecv = this.Recv(p_endpoint);
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
                        this.Send(p_endpoint, v_packetsend);

                        // recebendo ACK
                        v_packetrecv = this.Recv(p_endpoint);
                        if (v_packetrecv.v_type == Spartacus.Net.PacketType.ACK && v_packetrecv.v_sequence == v_sequence)
                            v_ack = true;
                    }

                    v_sequence++;
                }
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Envia uma <see cref="System.Data.DataTable"/>.
        /// Envia uma linha por pacote.
        /// </summary>
        /// <param name="p_table"><see cref="System.Data.DataTable"/>.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir enviar a <see cref="System.Data.DataTable"/>.</exception>
        public void SendDataTable(System.Data.DataTable p_table)
        {
            try
            {
                this.SendDataTable(0, p_table);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Envia um arquivo.
        /// </summary>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir enviar o arquivo.</exception>
        public void SendFile(int p_endpoint, string p_filename)
        {
            System.IO.FileStream v_file;
            System.IO.BinaryReader v_reader;
            Spartacus.Net.Packet v_packetsend, v_packetrecv;
            int v_chunksize;
            byte[] v_chunk;
            int v_numpackets, v_sequence;
            int k;
            bool v_ack;

            try
            {
                v_file = new System.IO.FileStream(p_filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                v_reader = new System.IO.BinaryReader(v_file);

                // tamanho dos dados eh igual ao tamanho total do pacote menos o cabecalho (25)
                v_chunksize = this.v_buffersize - 25;

                // numero de pacotes
                v_numpackets = (int) System.Math.Ceiling((double) v_file.Length / (double) v_chunksize);

                // montando pacote com nome do arquivo
                v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.FILE, p_filename);

                // enquanto nao recebeu ACK, precisa reenviar ate receber
                v_ack = false;
                while (! v_ack)
                {
                    // enviando pacote
                    this.Send(p_endpoint, v_packetsend);

                    // recebendo ACK
                    v_packetrecv = this.Recv(p_endpoint);
                    if (v_packetrecv.v_type == Spartacus.Net.PacketType.ACK)
                        v_ack = true;
                }

                v_sequence = 0;
                for (k = 0; k < v_file.Length; k += v_chunksize)
                {
                    // se eh o ultimo pacote, entao o tamanho dos dados serah menor
                    if ((k + v_chunksize) > v_file.Length)
                        v_chunksize = (int) v_file.Length - k;

                    // lendo dados do arquivo
                    v_chunk = v_reader.ReadBytes(v_chunksize);

                    // montando pacote
                    v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.DATA, v_sequence, v_numpackets, v_chunk);

                    // enquanto nao recebeu ACK, precisa reenviar ate receber
                    v_ack = false;
                    while (! v_ack)
                    {
                        // enviando pacote
                        this.Send(p_endpoint, v_packetsend);

                        // recebendo ACK
                        v_packetrecv = this.Recv(p_endpoint);
                        if (v_packetrecv.v_type == Spartacus.Net.PacketType.ACK && v_packetrecv.v_sequence == v_sequence)
                            v_ack = true;
                    }

                    v_sequence++;
                }

                v_reader.Close();
            }
            catch (System.IO.IOException e)
            {
                throw new Spartacus.Net.Exception(e);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Envia um arquivo.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer quando não conseguir enviar o arquivo.</exception>
        public void SendFile(string p_filename)
        {
            try
            {
                this.SendFile(0, p_filename);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }

        #endregion

        /// <summary>
        /// Fecha o canal de comunicação.
        /// </summary>
        /// <param name="p_endpoint">Ponto de comunicação.</param>
        public void Stop(int p_endpoint)
        {
            try
            {
                this.v_streams[p_endpoint].Close();
                this.v_sockets[p_endpoint].Close();
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Fecha o canal de comunicação.
        /// </summary>
        public void Stop()
        {
            try
            {
                this.Stop(0);
            }
            catch (Spartacus.Net.Exception e)
            {
                throw e;
            }
        }
    }
}

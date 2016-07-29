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
using System.Net;

namespace Spartacus.Net
{
    public enum ServerStatus
    {
        LISTENING,
        NOTLISTENING
    }

    public class ClientHandler
    {
        public string v_ip;
        public int v_port;
        public bool v_isconnected;

        public ClientHandler(string p_ip, int p_port)
        {
            this.v_ip = p_ip;
            this.v_port = p_port;
            this.v_isconnected = true;
        }
    }

    /// <summary>
    /// Classe Server.
    /// Herda da classe <see cref="Spartacus.Net.Endpoint"/>.
    /// </summary>
    public class Server : Spartacus.Net.Endpoint
    {
        /// <summary>
        /// Situação do servidor: escutando ou não escutando.
        /// </summary>
        public Spartacus.Net.ServerStatus v_status;

        /// <summary>
        /// Número de clientes conectados.
        /// </summary>
        public int v_numclients;

        /// <summary>
        /// Objeto que gerencia o evento de Conexão.
        /// </summary>
        public Spartacus.Net.ConnectEventClass v_connect;

        /// <summary>
        /// Objeto que gerencia o evento de Desconexão.
        /// </summary>
        public Spartacus.Net.DisconnectEventClass v_disconnect;

        /// <summary>
        /// Objeto que gerencia o evento de Conexão.
        /// </summary>
        public Spartacus.Net.AvailableEventClass v_available;

        /// <summary>
        /// Listener usado para recebe conexões de clientes.
        /// </summary>
        private System.Net.Sockets.TcpListener v_listener;

        /// <summary>
        /// Gerenciadores de conexão com clientes.
        /// </summary>
        private System.Collections.Generic.List<Spartacus.Net.ClientHandler> v_clienthandlers;

        /// <summary>
        /// Thread para escuta de novos clientes.
        /// </summary>
        private System.Threading.Thread v_threadaccept;

        /// <summary>
        /// Thread para verificação de clientes conectados.
        /// </summary>
        private System.Threading.Thread v_threadcheck;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Net.Server"/>.
        /// </summary>
        /// <param name="p_ip">IP do Servidor.</param>
        /// <param name="p_port">Porta do Servidor.</param>
        public Server(string p_ip, int p_port)
            : base(p_ip, p_port)
        {
            this.v_ip = p_ip;
            this.v_port = p_port;

            this.v_connect = new Spartacus.Net.ConnectEventClass();
            this.v_disconnect = new Spartacus.Net.DisconnectEventClass();
            this.v_available = new Spartacus.Net.AvailableEventClass();

            this.v_status = Spartacus.Net.ServerStatus.NOTLISTENING;

            this.v_listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse(p_ip), p_port);

            this.v_numclients = 0;
            this.v_clienthandlers = new System.Collections.Generic.List<ClientHandler>();

            this.v_threadaccept = new System.Threading.Thread(this.ThreadAccept);
            this.v_threadcheck = new System.Threading.Thread(this.ThreadCheck);
        }

        /// <summary>
        /// Escuta por conexões de clientes e as aceita.
        /// </summary>
        public void Accept()
        {
            this.v_listener.Start();

            this.v_status = Spartacus.Net.ServerStatus.LISTENING;

            this.v_threadaccept.Start();
            this.v_threadcheck.Start();
        }

        /// <summary>
        /// Thread para escuta de novos clientes.
        /// </summary>
        private void ThreadAccept()
        {
            while (this.v_status == Spartacus.Net.ServerStatus.LISTENING)
            {
                try
                {
                    this.v_sockets.Add(this.v_listener.AcceptTcpClient());
                    this.v_streams.Add(this.v_sockets[this.v_numclients].GetStream());

                    this.v_clienthandlers.Add(new Spartacus.Net.ClientHandler(
                        this.v_sockets[this.v_numclients].Client.RemoteEndPoint.ToString().Split(':')[0],
                        int.Parse(this.v_sockets[this.v_numclients].Client.RemoteEndPoint.ToString().Split(':')[1])
                    ));

                    this.v_connect.FireEvent(this.v_ip, this.v_port, this.v_clienthandlers[this.v_numclients].v_ip, this.v_clienthandlers[this.v_numclients].v_port);

                    this.v_numclients++;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Net.Exception(e);
                }
            }
        }

        /// <summary>
        /// Thread para verificação de clientes conectados.
        /// </summary>
        private void ThreadCheck()
        {
            bool v_socketconnected;

            while (this.v_status == Spartacus.Net.ServerStatus.LISTENING)
            {
                for (int i = 0; i < this.v_numclients; i++)
                {
                    v_socketconnected = !(this.v_sockets[i].Client.Poll(1000, System.Net.Sockets.SelectMode.SelectRead) && this.v_sockets[i].Client.Available == 0);

                    if (this.v_clienthandlers[i].v_isconnected && !v_socketconnected)
                    {
                        this.v_clienthandlers[i].v_isconnected = false;
                        this.v_disconnect.FireEvent(this.v_ip, this.v_port, this.v_clienthandlers[i].v_ip, this.v_clienthandlers[i].v_port);
                    }
                    else
                    {
                        if (this.v_clienthandlers[i].v_isconnected && v_socketconnected && this.v_sockets[i].Client.Available > 0)
                            this.v_available.FireEvent(this.v_ip, this.v_port, this.v_clienthandlers[i].v_ip, this.v_clienthandlers[i].v_port, i);
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Encerra o Servidor.
        /// </summary>
        public void StopServer()
        {
            try
            {
                this.v_status = Spartacus.Net.ServerStatus.NOTLISTENING;
                if (this.v_threadaccept.IsAlive)
                    this.v_threadaccept.Abort();
                this.v_listener.Stop();
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }
    }
}

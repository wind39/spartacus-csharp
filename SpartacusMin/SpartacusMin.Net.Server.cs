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

namespace SpartacusMin.Net
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
    /// Herda da classe <see cref="SpartacusMin.Net.Endpoint"/>.
    /// </summary>
    public class Server : SpartacusMin.Net.Endpoint
    {
        /// <summary>
        /// Situação do servidor: escutando ou não escutando.
        /// </summary>
        public SpartacusMin.Net.ServerStatus v_status;

        /// <summary>
        /// Número de clientes conectados.
        /// </summary>
        public int v_numclients;

		/// <summary>
		/// Gerenciadores de conexão com clientes.
		/// </summary>
		public System.Collections.Generic.List<SpartacusMin.Net.ClientHandler> v_clienthandlers;

        /// <summary>
        /// Objeto que gerencia o evento de Conexão.
        /// </summary>
        public SpartacusMin.Net.ConnectEventClass v_connect;

        /// <summary>
        /// Objeto que gerencia o evento de Desconexão.
        /// </summary>
        public SpartacusMin.Net.DisconnectEventClass v_disconnect;

        /// <summary>
        /// Objeto que gerencia o evento de Conexão.
        /// </summary>
        public SpartacusMin.Net.AvailableEventClass v_available;

        /// <summary>
		/// Listener usado para recebe conexões de clientes.
		/// </summary>
		private System.Net.Sockets.TcpListener v_listener;

        /// <summary>
        /// Thread para escuta de novos clientes.
        /// </summary>
        private System.Threading.Thread v_threadaccept;

        /// <summary>
        /// Thread para verificação de clientes conectados.
        /// </summary>
        private System.Threading.Thread v_threadcheck;

        /// <summary>
        /// Thread para limpeza de clientes que já foram desconectados.
        /// </summary>
        private System.Threading.Thread v_threadclean;

        /// <summary>
        /// Semáforo para controle de pool de clientes.
        /// </summary>
        private readonly object v_lock;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="SpartacusMin.Net.Server"/>.
        /// </summary>
        /// <param name="p_ip">IP do Servidor.</param>
        /// <param name="p_port">Porta do Servidor.</param>
        public Server(string p_ip, int p_port)
            : base(p_ip, p_port)
        {
            this.v_ip = p_ip;
            this.v_port = p_port;

            this.v_connect = new SpartacusMin.Net.ConnectEventClass();
            this.v_disconnect = new SpartacusMin.Net.DisconnectEventClass();
            this.v_available = new SpartacusMin.Net.AvailableEventClass();

            this.v_status = SpartacusMin.Net.ServerStatus.NOTLISTENING;

            this.v_listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse(p_ip), p_port);

            this.v_numclients = 0;
            this.v_clienthandlers = new System.Collections.Generic.List<ClientHandler>();

            this.v_threadaccept = new System.Threading.Thread(this.ThreadAccept);
            this.v_threadcheck = new System.Threading.Thread(this.ThreadCheck);
            this.v_threadclean = new System.Threading.Thread(this.ThreadClean);

            this.v_lock = new object();
        }

        /// <summary>
        /// Escuta por conexões de clientes e as aceita.
        /// </summary>
        public void Accept()
        {
            this.v_listener.Start();

            this.v_status = SpartacusMin.Net.ServerStatus.LISTENING;

            this.v_threadaccept.Start();
            this.v_threadcheck.Start();
            this.v_threadclean.Start();
        }

		/// <summary>
		/// Thread para escuta de novos clientes.
		/// </summary>
		private void ThreadAccept()
		{
			bool v_fire;

			while (this.v_status == SpartacusMin.Net.ServerStatus.LISTENING)
			{
				try
				{
					v_fire = false;
					lock(this.v_lock)
					{
						this.v_sockets.Add(this.v_listener.AcceptTcpClient());
						this.v_streams.Add(this.v_sockets[this.v_numclients].GetStream());

						this.v_clienthandlers.Add(new SpartacusMin.Net.ClientHandler(
							this.v_sockets[this.v_numclients].Client.RemoteEndPoint.ToString().Split(':')[0],
							int.Parse(this.v_sockets[this.v_numclients].Client.RemoteEndPoint.ToString().Split(':')[1])
						));

						this.v_numclients++;
						v_fire = true;
					}
					if (v_fire)
						this.v_connect.FireEvent(this.v_ip, this.v_port, this.v_clienthandlers[this.v_numclients-1].v_ip, this.v_clienthandlers[this.v_numclients-1].v_port, this.v_numclients-1);
				}
				catch (System.Exception e)
				{
					throw new SpartacusMin.Net.Exception(e);
				}
			}
		}

		/// <summary>
		/// Thread para verificação de clientes conectados.
		/// </summary>
		private void ThreadCheck()
		{
			bool v_socketconnected;

			while (this.v_status == SpartacusMin.Net.ServerStatus.LISTENING)
			{
				try
				{
					lock(this.v_lock)
					{
						for (int i = 0; i < this.v_numclients; i++)
						{
							if (this.v_clienthandlers[i].v_isconnected && this.v_sockets[i] != null && this.v_sockets[i].Client != null)
							{
								try
								{
									v_socketconnected = !(this.v_sockets[i].Client.Poll(1000, System.Net.Sockets.SelectMode.SelectRead) && this.v_sockets[i].Client.Available == 0);
								}
								catch
								{
									v_socketconnected = false;
								}

								if (!v_socketconnected)
								{
									this.v_clienthandlers[i].v_isconnected = false;
									this.v_disconnect.FireEvent(this.v_ip, this.v_port, this.v_clienthandlers[i].v_ip, this.v_clienthandlers[i].v_port, i);
								}
								else
								{
									if (v_socketconnected && this.v_sockets[i].Client.Available > 0)
										this.v_available.FireEvent(this.v_ip, this.v_port, this.v_clienthandlers[i].v_ip, this.v_clienthandlers[i].v_port, i);
								}
							}
						}
					}
				}
				catch (System.Exception e)
				{
					throw new SpartacusMin.Net.Exception(e);
				}

				System.Threading.Thread.Sleep(100);
			}
		}

		/// <summary>
		/// Thread para limpeza de clientes que já foram desconectados.
		/// </summary>
		private void ThreadClean()
		{
			bool v_achou;
			int k;

			while (this.v_status == SpartacusMin.Net.ServerStatus.LISTENING)
			{
				try
				{
					lock (this.v_lock)
					{
						v_achou = false;
						k = this.v_numclients - 1;
						while (k >= 0 && !v_achou)
						{
							if (!this.v_clienthandlers[k].v_isconnected)
							{
								this.v_sockets.RemoveAt(k);
								this.v_streams.RemoveAt(k);
								this.v_clienthandlers.RemoveAt(k);
								this.v_numclients--;
								k--;
							}
							else
								v_achou = true;
						}
					}
				}
				catch (System.Exception e)
				{
					throw new SpartacusMin.Net.Exception(e);
				}

				System.Threading.Thread.Sleep(10000);
			}
		}

		/// <summary>
		/// Encerra a conexão com um cliente.
		/// </summary>
		/// <param name="p_clientid">Código do cliente.</param>
		public void StopClient(int p_clientid)
		{
			lock (this.v_lock)
			{
				base.Stop(p_clientid);
				this.v_clienthandlers[p_clientid].v_isconnected = false;
			}
		}

        /// <summary>
        /// Encerra o Servidor.
        /// </summary>
        public void StopServer()
        {
            try
            {
                this.v_status = SpartacusMin.Net.ServerStatus.NOTLISTENING;
                if (this.v_threadaccept.IsAlive)
                    this.v_threadaccept.Abort();
                this.v_listener.Stop();
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Net.Exception(e);
            }
        }
    }
}

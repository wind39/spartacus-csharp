/*
The MIT License (MIT)

Copyright (c) 2014 William Ivanski

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
    /// <summary>
    /// Classe Server.
    /// Herda da classe <see cref="Spartacus.Net.Endpoint"/>.
    /// </summary>
    public class Server : Spartacus.Net.Endpoint
    {
        /// <summary>
        /// Listener usado para recebe conexões de clientes.
        /// </summary>
        private System.Net.Sockets.TcpListener v_listener;


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

            this.v_listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse(p_ip), p_port);
            this.v_listener.Start();
        }

        /// <summary>
        /// Escuta por conexões de clientes e as aceita.
        /// </summary>
        public void Accept()
        {
            try
            {
                this.v_socket = this.v_listener.AcceptTcpClient();
                this.v_stream = this.v_socket.GetStream();
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }

        /// <summary>
        /// Encerra o Servidor.
        /// </summary>
        public void StopServer()
        {
            try
            {
                this.v_listener.Stop();
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Net.Exception(e);
            }
        }
    }
}

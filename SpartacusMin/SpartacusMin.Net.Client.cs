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
    /// <summary>
    /// Classe Client.
    /// Herda da classe <see cref="SpartacusMin.Net.Endpoint"/>.
    /// </summary>
    public class Client : SpartacusMin.Net.Endpoint
    {
        /// <summary>
        /// IP do servidor.
        /// </summary>
        public string v_serverip;

        /// <summary>
        /// Porta do servidor.
        /// </summary>
        public int v_serverport;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="SpartacusMin.Net.Client"/>.
        /// </summary>
        /// <param name="p_serverip">IP do servidor.</param>
        /// <param name="p_serverport">Porta do servidor.</param>
        /// <param name="p_clientip">IP do cliente.</param>
        /// <param name="p_clientport">Porta do cliente.</param>
        public Client(string p_serverip, int p_serverport, string p_clientip, int p_clientport)
            : base(p_clientip, p_clientport)
        {
            System.Net.IPEndPoint v_endpoint;

            this.v_serverip = p_serverip;
            this.v_serverport = p_serverport;

            v_endpoint = new IPEndPoint(System.Net.IPAddress.Parse(p_clientip), p_clientport);

            this.v_sockets.Add(new System.Net.Sockets.TcpClient(v_endpoint));
        }

        /// <summary>
        /// Conecta-se com o servidor.
        /// </summary>
        /// <exception cref="SpartacusMin.Net.Exception">Exceção pode ocorrer se não conseguir se conectar com o servidor.</exception>
        public void Connect()
        {
            try
            {
                this.v_sockets[0].Connect(this.v_serverip, this.v_serverport);
                this.v_streams.Add(this.v_sockets[0].GetStream());
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Net.Exception(e);
            }
        }
    }
}

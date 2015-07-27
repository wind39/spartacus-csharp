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

namespace Spartacus.Net
{
    /// <summary>
    /// Classe ConnectEventArgs.
    /// Representa os argumentos do evento de Conexão.
    /// Herda da classe <see cref="System.EventArgs"/>.
    /// </summary>
    public class ConnectEventArgs : System.EventArgs
    {
        /// <summary>
        /// IP do Servidor.
        /// </summary>
        public string v_serverip;

        /// <summary>
        /// Porta do Servidor.
        /// </summary>
        public int v_serverport;

        /// <summary>
        /// IP do Cliente.
        /// </summary>
        public string v_clientip;

        /// <summary>
        /// Porta do Cliente.
        /// </summary>
        public int v_clientport;

        /// <summary>
        /// Índice do Cliente na lista de Clientes.
        /// </summary>
        public int v_index;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Net.ConnectEventArgs"/>.
        /// </summary>
        public ConnectEventArgs()
        {
            
        }
    }

    /// <summary>
    /// Classe ConnectEventClass.
    /// Representa um evento de Conexão.
    /// </summary>
    public class ConnectEventClass
    {
        /// <summary>
        /// Delegate para gerenciar o evento de Conexão.
        /// </summary>
        public delegate void ConnectEventHandler(Spartacus.Net.ConnectEventClass obj, Spartacus.Net.ConnectEventArgs e);

        /// <summary>
        /// Evento de Conexão propriamente dito.
        /// </summary>
        public event ConnectEventHandler ConnectEvent;

        /// <summary>
        /// Argumentos do evento de Conexão.
        /// </summary>
        public Spartacus.Net.ConnectEventArgs ConnectEventArgs = null;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Net.ConnectEventClass"/>.
        /// </summary>
        public ConnectEventClass()
        {
            this.ConnectEventArgs = new Spartacus.Net.ConnectEventArgs();
        }

        /// <summary>
        /// Dispara o evento de Conexão.
        /// </summary>
        /// <param name="p_serverip">IP do Servidor.</param>
        /// <param name="p_serverport">Porta do Servidor.</param>
        /// <param name="p_clientip">IP do Cliente.</param>
        /// <param name="p_clientport">Porta do Cliente.</param>
        public void FireEvent(string p_serverip, int p_serverport, string p_clientip, int p_clientport)
        {
            if (this.ConnectEvent != null)
            {
                this.ConnectEventArgs.v_serverip = p_serverip;
                this.ConnectEventArgs.v_serverport = p_serverport;
                this.ConnectEventArgs.v_clientip = p_clientip;
                this.ConnectEventArgs.v_clientport = p_clientport;

                this.ConnectEvent(this, this.ConnectEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Conexão.
        /// </summary>
        /// <param name="p_serverip">IP do Servidor.</param>
        /// <param name="p_serverport">Porta do Servidor.</param>
        /// <param name="p_clientip">IP do Cliente.</param>
        /// <param name="p_clientport">Porta do Cliente.</param>
        /// <param name="p_index">Índice do Cliente.</param>
        public void FireEvent(string p_serverip, int p_serverport, string p_clientip, int p_clientport, int p_index)
        {
            if (this.ConnectEvent != null)
            {
                this.ConnectEventArgs.v_serverip = p_serverip;
                this.ConnectEventArgs.v_serverport = p_serverport;
                this.ConnectEventArgs.v_clientip = p_clientip;
                this.ConnectEventArgs.v_clientport = p_clientport;
                this.ConnectEventArgs.v_index = p_index;

                this.ConnectEvent(this, this.ConnectEventArgs);
            }
        }
    }
}

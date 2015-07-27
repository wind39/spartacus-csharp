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
    /// Classe AvailableEventArgs.
    /// Representa os argumentos do evento de Dados Disponíveis.
    /// Herda da classe <see cref="System.EventArgs"/>.
    /// </summary>
    public class AvailableEventArgs : System.EventArgs
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
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Net.AvailableEventArgs"/>.
        /// </summary>
        public AvailableEventArgs()
        {

        }
    }

    /// <summary>
    /// Classe AvailableEventClass.
    /// Representa um evento de Dados Disponíveis.
    /// </summary>
    public class AvailableEventClass
    {
        /// <summary>
        /// Delegate para gerenciar o evento de Dados Disponíveis.
        /// </summary>
        public delegate void AvailableEventHandler(Spartacus.Net.AvailableEventClass obj, Spartacus.Net.AvailableEventArgs e);

        /// <summary>
        /// Evento de Dados Disponíveis propriamente dito.
        /// </summary>
        public event AvailableEventHandler AvailableEvent;

        /// <summary>
        /// Argumentos do evento de Dados Disponíveis.
        /// </summary>
        public Spartacus.Net.AvailableEventArgs AvailableEventArgs = null;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Net.AvailableEventClass"/>.
        /// </summary>
        public AvailableEventClass()
        {
            this.AvailableEventArgs = new Spartacus.Net.AvailableEventArgs();
        }

        /// <summary>
        /// Dispara o evento de Dados Disponíveis.
        /// </summary>
        /// <param name="p_serverip">IP do Servidor.</param>
        /// <param name="p_serverport">Porta do Servidor.</param>
        /// <param name="p_clientip">IP do Cliente.</param>
        /// <param name="p_clientport">Porta do Cliente.</param>
        public void FireEvent(string p_serverip, int p_serverport, string p_clientip, int p_clientport)
        {
            if (this.AvailableEvent != null)
            {
                this.AvailableEventArgs.v_serverip = p_serverip;
                this.AvailableEventArgs.v_serverport = p_serverport;
                this.AvailableEventArgs.v_clientip = p_clientip;
                this.AvailableEventArgs.v_clientport = p_clientport;

                this.AvailableEvent(this, this.AvailableEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Dados Disponíveis.
        /// </summary>
        /// <param name="p_serverip">IP do Servidor.</param>
        /// <param name="p_serverport">Porta do Servidor.</param>
        /// <param name="p_clientip">IP do Cliente.</param>
        /// <param name="p_clientport">Porta do Cliente.</param>
        /// <param name="p_index">Índice do Cliente.</param>
        public void FireEvent(string p_serverip, int p_serverport, string p_clientip, int p_clientport, int p_index)
        {
            if (this.AvailableEvent != null)
            {
                this.AvailableEventArgs.v_serverip = p_serverip;
                this.AvailableEventArgs.v_serverport = p_serverport;
                this.AvailableEventArgs.v_clientip = p_clientip;
                this.AvailableEventArgs.v_clientport = p_clientport;
                this.AvailableEventArgs.v_index = p_index;

                this.AvailableEvent(this, this.AvailableEventArgs);
            }
        }
    }
}

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

namespace Spartacus.Utils
{
    /// <summary>
    /// Classe ErrorEventArgs.
    /// Representa os argumentos do evento de Erro.
    /// Herda da classe <see cref="System.EventArgs"/>.
    /// </summary>
    public class ErrorEventArgs : System.EventArgs
    {
        /// <summary>
        /// Nome do processo.
        /// </summary>
        public string v_process;

        /// <summary>
        /// Nome do subprocesso, método ou rotina.
        /// </summary>
        public string v_subprocess;

        /// <summary>
        /// Mensagem atual do processo.
        /// </summary>
        public string v_message;

        /// <summary>
        /// Stacktrace atual do processo (se aplicável).
        /// </summary>
        public string v_stacktrace;

        /// <summary>
        /// Informa se o processo deve mostrar suas mensagens ao usuário ou não.
        /// </summary>
        public bool v_verbose;

        /// <summary>
        /// Índice da instância.
        /// </summary>
        public int v_index;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ErrorEventArgs"/>.
        /// </summary>
        public ErrorEventArgs()
        {
            this.v_verbose = false;
            this.v_index = 0;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ErrorEventArgs"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        public ErrorEventArgs(bool p_verbose)
        {
            this.v_verbose = p_verbose;
            this.v_index = 0;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ErrorEventArgs"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        /// <param name="p_index">Índice da instância.</param>
        public ErrorEventArgs(bool p_verbose, int p_index)
        {
            this.v_verbose = p_verbose;
            this.v_index = p_index;
        }
    }

    /// <summary>
    /// Classe ErrorEventClass.
    /// Representa um evento de Erro.
    /// </summary>
    public class ErrorEventClass
    {
        /// <summary>
        /// Delegate para gerenciar o evento de Erro.
        /// </summary>
        public delegate void ErrorEventHandler(Spartacus.Utils.ErrorEventClass obj, Spartacus.Utils.ErrorEventArgs e);

        /// <summary>
        /// Evento de Erro propriamente dito.
        /// </summary>
        public event ErrorEventHandler ErrorEvent;

        /// <summary>
        /// Argumentos do evento de Erro.
        /// </summary>
        public Spartacus.Utils.ErrorEventArgs ErrorEventArgs = null;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ErrorEventClass"/>.
        /// </summary>
        public ErrorEventClass()
        {
            this.ErrorEventArgs = new Spartacus.Utils.ErrorEventArgs();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ErrorEventClass"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        public ErrorEventClass(bool p_verbose)
        {
            this.ErrorEventArgs = new Spartacus.Utils.ErrorEventArgs(p_verbose);
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ErrorEventClass"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        /// <param name="p_index">Índice da instância.</param>
        public ErrorEventClass(bool p_verbose, int p_index)
        {
            this.ErrorEventArgs = new Spartacus.Utils.ErrorEventArgs(p_verbose, p_index);
        }

        /// <summary>
        /// Dispara o evento de Erro.
        /// </summary>
        /// <param name="p_message">Mensagem atual do processo.</param>
        public void FireEvent(string p_message)
        {
            if (this.ErrorEvent != null)
            {
                this.ErrorEventArgs.v_message = p_message;

                this.ErrorEvent(this, this.ErrorEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Erro.
        /// </summary>
        /// <param name="p_message">Mensagem atual do processo.</param>
        /// <param name="p_stacktrace">Stacktrace atual do processo.</param>
        public void FireEvent(string p_message, string p_stacktrace)
        {
            if (this.ErrorEvent != null)
            {
                this.ErrorEventArgs.v_message = p_message;
                this.ErrorEventArgs.v_stacktrace = p_stacktrace;

                this.ErrorEvent(this, this.ErrorEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Erro.
        /// </summary>
        /// <param name="p_process">Nome do processo.</param>
        /// <param name="p_subprocess">Nome do subprocesso, método ou rotina.</param>
        /// <param name="p_message">Mensagem atual do processo.</param>
        public void FireEvent(string p_process, string p_subprocess, string p_message)
        {
            if (this.ErrorEvent != null)
            {
                this.ErrorEventArgs.v_process = p_process;
                this.ErrorEventArgs.v_subprocess = p_subprocess;
                this.ErrorEventArgs.v_message = p_message;
                this.ErrorEventArgs.v_stacktrace = "";

                this.ErrorEvent(this, this.ErrorEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Erro.
        /// </summary>
        /// <param name="p_process">Nome do processo.</param>
        /// <param name="p_subprocess">Nome do subprocesso, método ou rotina.</param>
        /// <param name="p_message">Mensagem atual do processo.</param>
        /// <param name="p_stacktrace">Stacktrace atual do processo.</param>
        public void FireEvent(string p_process, string p_subprocess, string p_message, string p_stacktrace)
        {
            if (this.ErrorEvent != null)
            {
                this.ErrorEventArgs.v_process = p_process;
                this.ErrorEventArgs.v_subprocess = p_subprocess;
                this.ErrorEventArgs.v_message = p_message;
                this.ErrorEventArgs.v_stacktrace = p_stacktrace;

                this.ErrorEvent(this, this.ErrorEventArgs);
            }
        }
    }
}
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

namespace Spartacus.Utils
{
    /// <summary>
    /// Classe WarningEventArgs.
    /// Representa os argumentos do evento de Aviso.
    /// Herda da classe <see cref="System.EventArgs"/>.
    /// </summary>
    public class WarningEventArgs : System.EventArgs
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
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.WarningEventArgs"/>.
        /// </summary>
        public WarningEventArgs()
        {
            this.v_verbose = false;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.WarningEventArgs"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        public WarningEventArgs(bool p_verbose)
        {
            this.v_verbose = p_verbose;
        }
    }

    /// <summary>
    /// Classe WarningEventClass.
    /// Representa um evento de Aviso.
    /// </summary>
    public class WarningEventClass
    {
        /// <summary>
        /// Delegate para gerenciar o evento de Aviso.
        /// </summary>
        public delegate void WarningEventHandler(Spartacus.Utils.WarningEventClass obj, Spartacus.Utils.WarningEventArgs e);

        /// <summary>
        /// Evento de Aviso propriamente dito.
        /// </summary>
        public event WarningEventHandler WarningEvent;

        /// <summary>
        /// Argumentos do evento de Aviso.
        /// </summary>
        public Spartacus.Utils.WarningEventArgs WarningEventArgs = null;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.WarningEventClass"/>.
        /// </summary>
        public WarningEventClass()
        {
            this.WarningEventArgs = new Spartacus.Utils.WarningEventArgs();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.WarningEventClass"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        public WarningEventClass(bool p_verbose)
        {
            this.WarningEventArgs = new Spartacus.Utils.WarningEventArgs(p_verbose);
        }

        /// <summary>
        /// Dispara o evento de Aviso.
        /// </summary>
        /// <param name="p_process">Nome do processo.</param>
        /// <param name="p_subprocess">Nome do subprocesso, método ou rotina.</param>
        /// <param name="p_message">Mensagem atual do processo.</param>
        public void FireEvent(string p_process, string p_subprocess, string p_message)
        {
            if (this.WarningEvent != null)
            {
                this.WarningEventArgs.v_process = p_process;
                this.WarningEventArgs.v_subprocess = p_subprocess;
                this.WarningEventArgs.v_message = p_message;
                this.WarningEventArgs.v_stacktrace = "";

                this.WarningEvent(this, this.WarningEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Aviso.
        /// </summary>
        /// <param name="p_process">Nome do processo.</param>
        /// <param name="p_subprocess">Nome do subprocesso, método ou rotina.</param>
        /// <param name="p_message">Mensagem atual do processo.</param>
        /// <param name="p_stacktrace">Stacktrace atual do processo.</param>
        public void FireEvent(string p_process, string p_subprocess, string p_message, string p_stacktrace)
        {
            if (this.WarningEvent != null)
            {
                this.WarningEventArgs.v_process = p_process;
                this.WarningEventArgs.v_subprocess = p_subprocess;
                this.WarningEventArgs.v_message = p_message;
                this.WarningEventArgs.v_stacktrace = p_stacktrace;

                this.WarningEvent(this, this.WarningEventArgs);
            }
        }
    }
}

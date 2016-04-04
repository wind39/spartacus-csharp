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

namespace Spartacus.Utils
{
    /// <summary>
    /// Classe ProgressEventArgs.
    /// Representa os argumentos do evento de Progresso.
    /// Herda da classe <see cref="System.EventArgs"/>.
    /// </summary>
    public class ProgressEventArgs : System.EventArgs
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
        /// Percentual de execução do processo.
        /// </summary>
        public double v_percentage;

        /// <summary>
        /// Mensagem atual do processo.
        /// </summary>
        public string v_message;

        /// <summary>
        /// Contador de número de elementos processados até o momento.
        /// </summary>
        public uint v_counter;

        /// <summary>
        /// Número total de elementos a serem processados.
        /// </summary>
        public uint v_total;

        /// <summary>
        /// Informa se o processo deve mostrar suas mensagens ao usuário ou não.
        /// </summary>
        public bool v_verbose;

        /// <summary>
        /// Índice da instância.
        /// </summary>
        public int v_index;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ProgressEventArgs"/>.
        /// </summary>
        public ProgressEventArgs()
        {
            this.v_verbose = false;
            this.v_index = 0;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ProgressEventArgs"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        public ProgressEventArgs(bool p_verbose)
        {
            this.v_verbose = p_verbose;
            this.v_index = 0;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ProgressEventArgs"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        /// <param name="p_index">Índice da instância.</param>
        public ProgressEventArgs(bool p_verbose, int p_index)
        {
            this.v_verbose = p_verbose;
            this.v_index = p_index;
        }
    }

    /// <summary>
    /// Classe ProgressEventClass.
    /// Representa um evento de Progresso.
    /// </summary>
    public class ProgressEventClass
    {
        /// <summary>
        /// Delegate para gerenciar o evento de Progresso.
        /// </summary>
        public delegate void ProgressEventHandler(Spartacus.Utils.ProgressEventClass obj, Spartacus.Utils.ProgressEventArgs e);

        /// <summary>
        /// Evento de Progresso propriamente dito.
        /// </summary>
        public event ProgressEventHandler ProgressEvent;

        /// <summary>
        /// Argumentos do evento de Progresso.
        /// </summary>
        public Spartacus.Utils.ProgressEventArgs ProgressEventArgs = null;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ProgressEventClass"/>.
        /// </summary>
        public ProgressEventClass()
        {
            this.ProgressEventArgs = new Spartacus.Utils.ProgressEventArgs();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ProgressEventClass"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        public ProgressEventClass(bool p_verbose)
        {
            this.ProgressEventArgs = new Spartacus.Utils.ProgressEventArgs(p_verbose);
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.ProgressEventClass"/>.
        /// </summary>
        /// <param name="p_verbose">Se o processo deve mostrar suas mensagens ao usuário ou não.</param>
        /// <param name="p_index">Índice da instância.</param>
        public ProgressEventClass(bool p_verbose, int p_index)
        {
            this.ProgressEventArgs = new Spartacus.Utils.ProgressEventArgs(p_verbose, p_index);
        }

        /// <summary>
        /// Dispara o evento de Progresso.
        /// </summary>
        /// <param name="p_percentage">Percentual de execução do processo.</param>
        /// <param name="p_message">Mensagem atual do processo.</param>
        public void FireEvent(double p_percentage, string p_message)
        {
            if (this.ProgressEvent != null)
            {
                this.ProgressEventArgs.v_percentage = p_percentage;
                this.ProgressEventArgs.v_message = p_message;

                this.ProgressEvent(this, this.ProgressEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Progresso.
        /// </summary>
        /// <param name="p_process">Nome do processo.</param>
        /// <param name="p_subprocess">Nome do subprocesso, método ou rotina.</param>
        /// <param name="p_percentage">Percentual de execução do processo.</param>
        /// <param name="p_message">Mensagem atual do processo.</param>
        public void FireEvent(string p_process, string p_subprocess, double p_percentage, string p_message)
        {
            if (this.ProgressEvent != null)
            {
                this.ProgressEventArgs.v_process = p_process;
                this.ProgressEventArgs.v_subprocess = p_subprocess;
                this.ProgressEventArgs.v_percentage = p_percentage;
                this.ProgressEventArgs.v_message = p_message;

                this.ProgressEvent(this, this.ProgressEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Progresso.
        /// </summary>
        /// <param name="p_counter">Número de elementos processados até o momento.</param>
        public void FireEvent(uint p_counter)
        {
            if (this.ProgressEvent != null)
            {
                this.ProgressEventArgs.v_counter = p_counter;

                this.ProgressEvent(this, this.ProgressEventArgs);
            }
        }

        /// <summary>
        /// Dispara o evento de Progresso.
        /// </summary>
        /// <param name="p_counter">Número de elementos processados até o momento.</param>
        /// <param name="p_total">Número total de elementos a serem processados.</param>
        public void FireEvent(uint p_counter, uint p_total)
        {
            if (this.ProgressEvent != null)
            {
                this.ProgressEventArgs.v_counter = p_counter;
                this.ProgressEventArgs.v_total = p_counter;

                this.ProgressEvent(this, this.ProgressEventArgs);
            }
        }
    }
}

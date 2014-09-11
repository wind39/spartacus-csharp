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

                this.WarningEvent(this, this.WarningEventArgs);
            }
        }
    }
}

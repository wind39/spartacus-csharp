using System;

namespace Spartacus.Forms
{
    /// <summary>
    /// Classe Spartacus.Forms.Exception.
    /// Herda da classe <see cref="System.Exception"/>.
    /// </summary>
    public class Exception : System.Exception
    {
        /// <summary>
        /// Mensagem de Exceção.
        /// Formato:
        ///   {Classe de Exceção} at {Contexto}:
        ///   [{Data e Hora}] [{Mensagem}|<<NOMESSAGE>>]\n\n
        ///   [({Exceção Interna})|] [{Mensagem da Exceção Interna}|]
        /// </summary>
        public string v_message;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Exception"/>.
        /// </summary>
        /// <param name='p_context'>
        /// Contexto onde ocorreu a exceção (método caller -> nome completo do método calee).
        /// </param>
        public Exception(string p_context)
            :base()
        {
            this.v_message = "Spartacus.Forms.Exception at "
                + p_context + "\n["
                + System.DateTime.Now.ToString() + "] "
                + "NOMESSAGE";
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Exception"/>.
        /// </summary>
        /// <param name='p_context'>
        /// Contexto onde ocorreu a exceção.
        /// </param>
        /// <param name='p_inner'>
        /// Exceção interna, encapsulada por esta exceção.
        /// </param>
        public Exception(string p_context, System.Exception p_inner)
            : base(null, p_inner)
        {
            this.v_message = "Spartacus.Forms.Exception at "
                + p_context + "\n["
                + System.DateTime.Now.ToString() + "] "
                + "NOMESSAGE \n\n("
                + p_inner.GetType().Name + ") "
                + p_inner.Message;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Exception"/>.
        /// </summary>
        /// <param name='p_context'>
        /// Contexto onde ocorreu a exceção (método caller -> nome completo do método calee).
        /// </param>
        /// <param name='p_message'>
        /// Mensagem descritiva da exceção.
        /// </param>
        public Exception(string p_context, string p_message)
            : base(p_message)
        {
            this.v_message = "Spartacus.Forms.Exception at "
                + p_context + "\n["
                + System.DateTime.Now.ToString() + "] "
                + p_message;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Exception"/>.
        /// </summary>
        /// <param name='p_context'>
        /// Contexto onde ocorreu a exceção (método caller -> nome completo do método calee).
        /// </param>
        /// <param name='p_format'>
        /// Formato da mensagem descritiva da exceção.
        /// </param>
        /// <param name='p_args'>
        /// Argumentos da mensagem descritiva da exceção.
        /// </param>
        public Exception(string p_context, string p_format, params object[] p_args)
            : base(string.Format(p_format, p_args))
        {
            this.v_message = "Spartacus.Forms.Exception at "
                + p_context + "\n["
                + System.DateTime.Now.ToString() + "] "
                + string.Format(p_format, p_args);
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Exception"/>.
        /// </summary>
        /// <param name='p_context'>
        /// Contexto onde ocorreu a exceção (método caller -> nome completo do método calee).
        /// </param>
        /// <param name='p_message'>
        /// Mensagem descritiva da exceção.
        /// </param>
        /// <param name='p_inner'>
        /// Exceção interna, encapsulada por esta exceção.
        /// </param>
        public Exception(string p_context, string p_message, System.Exception p_inner)
            : base(p_message, p_inner)
        {
            this.v_message = "Spartacus.Forms.Exception at "
                + p_context + "\n["
                + System.DateTime.Now.ToString() + "] "
                + p_message + "\n\n("
                + p_inner.GetType().Name + ") "
                + p_inner.Message;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Exception"/>.
        /// </summary>
        /// <param name='p_context'>
        /// Contexto onde ocorreu a exceção (método caller -> nome completo do método calee).
        /// </param>
        /// <param name='p_format'>
        /// Formato da mensagem descritiva da exceção.
        /// </param>
        /// <param name='p_inner'>
        /// Exceção interna, encapsulada por esta exceção.
        /// </param>
        /// <param name='p_args'>
        /// Argumentos da mensagem descritiva da exceção.
        /// </param>
        public Exception(string p_context, string p_format, System.Exception p_inner, params object[] p_args)
            : base(string.Format(p_format, p_args), p_inner)
        {
            this.v_message = "Spartacus.Forms.Exception at "
                + p_context + "\n["
                + System.DateTime.Now.ToString() + "] "
                + string.Format(p_format, p_args) + "\n\n("
                + p_inner.GetType().Name + ") "
                + p_inner.Message;
        }
    }
}

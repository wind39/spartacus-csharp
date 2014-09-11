using System;

namespace Spartacus.Forms
{
    /// <summary>
    /// Tipo do Formulário.
    /// </summary>
    public enum FormType
    {
        PARENT,
        CHILD
    }

    /// <summary>
    /// Classe Form.
    /// Extende a classe <see cref="System.Windows.Forms.Form"/>, tratando eventos de OnVisibleChanged e OnFormClosing.
    /// </summary>
    public class Form : System.Windows.Forms.Form
    {
        /// <summary>
        /// Tipo do Formulário.
        /// </summary>
        public Spartacus.Forms.FormType v_type;

        /// <summary>
        /// Janela pai do Formulário atual.
        /// Tem valor null se o Formulário atual pertencer à Janela principal.
        /// </summary>
        public Spartacus.Forms.Window v_parent;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Form"/>.
        /// </summary>
        public Form()
            : base()
        {
            this.v_type = Spartacus.Forms.FormType.PARENT;
            this.v_parent = null;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Form"/>.
        /// </summary>
        /// <param name="p_parent">Janela pai.</param>
        public Form(Spartacus.Forms.Window p_parent)
            : base()
        {
            this.v_type = Spartacus.Forms.FormType.CHILD;
            this.v_parent = p_parent;
            this.VisibleChanged += new System.EventHandler(OnVisibleChanged);
        }

        /// <summary>
        /// Evento disparado ao mudar o atributo Visible.
        /// </summary>
        protected void OnVisibleChanged(object sender, System.EventArgs e)
        {
            if (this.v_type == Spartacus.Forms.FormType.CHILD &&
                this.v_parent != null)
            {
                if (this.Visible)
                    this.v_parent.Disable();
                else
                    this.v_parent.Enable();
            }
        }

        /// <summary>
        /// Evento disparado quando o usuário clicar no X de fechar a Janela.
        /// </summary>
        protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e)
        {
            if (e.CloseReason == System.Windows.Forms.CloseReason.WindowsShutDown ||
                e.CloseReason == System.Windows.Forms.CloseReason.ApplicationExitCall ||
                e.CloseReason == System.Windows.Forms.CloseReason.TaskManagerClosing ||
                this.v_type == Spartacus.Forms.FormType.PARENT)
            { 
                return; 
            }

            e.Cancel = true;

            this.Hide();
        }
    }
}


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
                this.v_parent.Refresh();

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


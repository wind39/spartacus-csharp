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
    /// Classe Progressbar.
    /// Representa um componente que mostra o progresso de execução de alguma tarefa.
    /// </summary>
    public class Progressbar : Spartacus.Forms.Container
    {
        /// <summary>
        /// Rótulo do Progressbar.
        /// </summary>
        public System.Windows.Forms.Label v_label;

        /// <summary>
        /// Controle nativo que representa o Progressbar.
        /// </summary>
        public System.Windows.Forms.ProgressBar v_progressbar;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Progressbar"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        public Progressbar(Spartacus.Forms.Container p_parent)
            : base(p_parent)
        {
            this.v_control = new System.Windows.Forms.Panel();

            this.v_isfrozen = false;

            this.SetWidth(p_parent.v_width);
            this.SetHeight(80);
            this.SetLocation(0, p_parent.v_offsety);

            this.v_label = new System.Windows.Forms.Label();
            this.v_label.Text = "";
            this.v_label.Location = new System.Drawing.Point(10, 10);
            this.v_label.AutoSize = true;
            this.v_label.Parent = this.v_control;

            this.v_progressbar = new System.Windows.Forms.ProgressBar();
            this.v_progressbar.Location = new System.Drawing.Point(5, 35);
            this.v_progressbar.Width = this.v_width - 10 - this.v_progressbar.Location.X;
            this.v_progressbar.Height = this.v_height - 35;
            this.v_progressbar.Parent = this.v_control;
            this.v_progressbar.Minimum = 0;
            this.v_progressbar.Maximum = 100;
            this.v_progressbar.Value = 0;
        }

        /// <summary>
        /// Redimensiona o Componente atual.
        /// Também reposiciona dentro do Container pai, se for necessário.
        /// </summary>
        /// <param name="p_newwidth">Nova largura.</param>
        /// <param name="p_newheight">Nova altura.</param>
        /// <param name="p_newposx">Nova posição X.</param>
        /// <param name="p_newposy">Nova posição Y.</param>
        public override void Resize(int p_newwidth, int p_newheight, int p_newposx, int p_newposy)
        {
            this.v_control.SuspendLayout();
            this.v_progressbar.SuspendLayout();

            this.SetWidth(p_newwidth);
            this.SetLocation(p_newposx, p_newposy);

            this.v_progressbar.Width = this.v_control.Width - 10 - this.v_progressbar.Location.X;

            this.v_progressbar.ResumeLayout();
            this.v_control.ResumeLayout();
            this.v_control.Refresh();
        }

        /// <summary>
        /// Habilita o Container atual.
        /// </summary>
        public override void Enable()
        {
            this.v_progressbar.Enabled = true;
        }

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public override void Disable()
        {
            this.v_progressbar.Enabled = false;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            this.v_label.Text = "";
            this.v_label.Refresh();
            this.v_progressbar.Value = 0;
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
        }

        /// <summary>
        /// Informa o texto a ser mostrado no Label.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Label.</param>
        public override void SetValue(string p_text)
        {
            this.v_label.Text = p_text;
            this.v_label.Refresh();
        }

        /// <summary>
        /// Informa o texto a ser mostrado no Label e valor a ser mostrado no Progressbar.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Label.</param>
        /// <param name="p_value">Valor a ser mostrado no Progressbar</param>
        public void SetValue(string p_text, int p_value)
        {
            this.v_label.Text = p_text;
            this.v_label.Refresh();
            this.v_progressbar.Value = p_value;
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Textbox.
        /// </summary>
        /// <returns>Texto ou valor atual do Textbox.</returns>
        public override string GetValue()
        {
            return this.v_label.Text;
        }
    }
}

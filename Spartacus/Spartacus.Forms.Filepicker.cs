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

namespace Spartacus.Forms
{
    /// <summary>
    /// Classe Filepicker.
    /// Representa um componente em que o usuário pode selecionar um arquivo.
    /// </summary>
    public class Filepicker : Spartacus.Forms.Container
    {
        /// <summary>
        /// Tipo do diálogo do Filepicker.
        /// </summary>
        public enum Type
        {
            OPEN,
            SAVE
        }

        /// <summary>
        /// Rótulo do Filepicker.
        /// </summary>
        public System.Windows.Forms.Label v_label;

        /// <summary>
        /// Tipo do diálogo do Filepicker.
        /// </summary>
        public Spartacus.Forms.Filepicker.Type v_type;

        /// <summary>
        /// Controle nativo que armazena o nome do arquivo.
        /// </summary>
        public System.Windows.Forms.TextBox v_textbox;

        /// <summary>
        /// Botão que mostra o diálogo do Filepicker.
        /// </summary>
        public System.Windows.Forms.Button v_button;

        /// <summary>
        /// Controle nativo que representa o diálogo do Filepicker.
        /// </summary>
        public System.Windows.Forms.FileDialog v_filedialog;

        /// <summary>
        /// Proporção entre o Label e o Textbox.
        /// </summary>
        public int v_proportion;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Filepicker"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        /// <param name="p_type">Tipo do diálogo do Filepicker.</param>
        /// <param name="p_label">Texto exibido no rótulo.</param>
        /// <param name="p_filter">Filtro de arquivos a serem exibidos no diálogo do Filepicker.</param>
        public Filepicker(Spartacus.Forms.Container p_parent, Spartacus.Forms.Filepicker.Type p_type, string p_label, string p_filter)
            : base(p_parent)
        {
            this.v_control = new System.Windows.Forms.Panel();

            this.SetWidth(p_parent.v_width);
            this.SetHeight(40);
            this.SetLocation(0, p_parent.v_offsety);

            this.v_label = new System.Windows.Forms.Label();
            this.v_label.Text = p_label;
            this.v_label.Location = new System.Drawing.Point(10, 10);
            this.v_label.AutoSize = true;
            this.v_label.Parent = this.v_control;

            this.v_button = new System.Windows.Forms.Button();
            this.v_button.Text = "...";
            this.v_button.Width = 30;
            this.v_button.Location = new System.Drawing.Point(this.v_width - 10 - this.v_button.Width, 5);
            this.v_button.Click += new System.EventHandler(this.OnClick);
            this.v_button.Parent = this.v_control;

            this.v_proportion = 40;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion / (double) 100)), 5);
            this.v_textbox.Width = this.v_width - 10 - this.v_button.Width - this.v_textbox.Location.X;
            this.v_textbox.Parent = this.v_control;

            this.v_type = p_type;
            if (this.v_type == Spartacus.Forms.Filepicker.Type.OPEN)
                this.v_filedialog = new System.Windows.Forms.OpenFileDialog();
            else
                this.v_filedialog = new System.Windows.Forms.SaveFileDialog();
            this.v_filedialog.Title = p_label;
            this.v_filedialog.Filter = p_filter;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Filepicker"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        /// <param name="p_type">Tipo do diálogo do Filepicker.</param>
        /// <param name="p_label">Texto exibido no rótulo.</param>
        /// <param name="p_filter">Filtro de arquivos a serem exibidos no diálogo do Filepicker.</param>
        /// <param name="p_proportion">Proporção entre o Label e o Textbox.</param>
        public Filepicker(Spartacus.Forms.Container p_parent, Spartacus.Forms.Filepicker.Type p_type, string p_label, string p_filter, int p_proportion)
            : base(p_parent)
        {
            this.v_control = new System.Windows.Forms.Panel();

            this.SetWidth(p_parent.v_width);
            this.SetHeight(40);
            this.SetLocation(0, p_parent.v_offsety);

            this.v_label = new System.Windows.Forms.Label();
            this.v_label.Text = p_label;
            this.v_label.Location = new System.Drawing.Point(10, 10);
            this.v_label.AutoSize = true;
            this.v_label.Parent = this.v_control;

            this.v_button = new System.Windows.Forms.Button();
            this.v_button.Text = "...";
            this.v_button.Width = 30;
            this.v_button.Location = new System.Drawing.Point(this.v_width - 10 - this.v_button.Width, 5);
            this.v_button.Click += new System.EventHandler(this.OnClick);
            this.v_button.Parent = this.v_control;

            this.v_proportion = p_proportion;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion / (double) 100)), 5);
            this.v_textbox.Width = this.v_width - 10 - this.v_button.Width - this.v_textbox.Location.X;
            this.v_textbox.Parent = this.v_control;

            this.v_type = p_type;
            if (this.v_type == Spartacus.Forms.Filepicker.Type.OPEN)
                this.v_filedialog = new System.Windows.Forms.OpenFileDialog();
            else
                this.v_filedialog = new System.Windows.Forms.SaveFileDialog();
            this.v_filedialog.Title = p_label;
            this.v_filedialog.Filter = p_filter;
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
            this.v_textbox.SuspendLayout();
            this.v_button.SuspendLayout();

            this.SetWidth(p_newwidth);
            this.SetLocation(p_newposx, p_newposy);

            this.v_textbox.Width = this.v_width - 10 - this.v_button.Width - this.v_textbox.Location.X;
            this.v_button.Location = new System.Drawing.Point(this.v_width - 10 - this.v_button.Width, 5);

            this.v_button.ResumeLayout();
            this.v_textbox.ResumeLayout();
            this.v_control.ResumeLayout();
            this.v_control.Refresh();
        }

        /// <summary>
        /// Habilita o Container atual.
        /// </summary>
        public override void Enable()
        {
            this.v_textbox.Enabled = true;
            this.v_button.Enabled = true;
        }

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public override void Disable()
        {
            this.v_textbox.Enabled = false;
            this.v_button.Enabled = false;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            this.v_textbox.Text = "";
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
        }

        /// <summary>
        /// Informa o texto ou valor a ser mostrado no Textbox.
        /// Usado para mostrar ao usuário um formulário já preenchido.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Textbox.</param>
        public override void SetValue(string p_text)
        {
            this.v_textbox.Text = p_text;
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Textbox.
        /// </summary>
        /// <returns>Texto ou valor atual do Textbox.</returns>
        public override string GetValue()
        {
            return this.v_textbox.Text;
        }

        /// <summary>
        /// Evento executado quando o botão de selecionar arquivo é clicado.
        /// </summary>
        private void OnClick(object sender, System.EventArgs e)
        {
            string v_currentdirectory;

            v_currentdirectory = System.Environment.CurrentDirectory;

            if (this.v_filedialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.v_textbox.Text = this.v_filedialog.FileName;
            }

            System.Environment.CurrentDirectory = v_currentdirectory;
        }
    }
}

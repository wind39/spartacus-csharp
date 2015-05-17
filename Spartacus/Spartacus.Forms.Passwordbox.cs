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
    /// Classe Passwordbox.
    /// Representa um componente em que o usuário pode digitar texto em uma única linha, porém o texto é omitido.
    /// </summary>
    public class Passwordbox : Spartacus.Forms.Container
    {
        /// <summary>
        /// Rótulo do Passwordbox.
        /// </summary>
        public System.Windows.Forms.Label v_label;

        /// <summary>
        /// Controle nativo que representa o Passwordbox.
        /// </summary>
        public System.Windows.Forms.TextBox v_textbox;

        /// <summary>
        /// Proporção entre o Label e o Textbox.
        /// </summary>
        public int v_proportion;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Passwordbox"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        /// <param name="p_label">Texto exibido no rótulo.</param>
        public Passwordbox(Spartacus.Forms.Container p_parent, string p_label)
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

            this.v_proportion = 40;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion / (double) 100)), 5);
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;
            this.v_textbox.UseSystemPasswordChar = true;
            this.v_textbox.Parent = this.v_control;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Passwordbox"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        /// <param name="p_label">Texto exibido no rótulo.</param>
        /// <param name="p_proportion">Proporção entre o Label e o Textbox.</param>
        public Passwordbox(Spartacus.Forms.Container p_parent, string p_label, int p_proportion)
            : base(p_parent)
        {
            this.v_control = new System.Windows.Forms.Panel();

            this.SetWidth(p_parent.v_width);
            this.SetHeight(40);
            this.SetLocation(0, p_parent.v_offsety);

            this.v_label = new System.Windows.Forms.Label();
            this.v_label.Text = p_label;
            this.v_label.Location = new System.Drawing.Point(10, 10);
            this.v_label.Parent = this.v_control;

            this.v_proportion = p_proportion;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion / (double) 100)), 5);
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;
            this.v_textbox.UseSystemPasswordChar = true;
            this.v_textbox.Parent = this.v_control;
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

            this.SetWidth(p_newwidth);
            this.SetLocation(p_newposx, p_newposy);

            this.v_textbox.Width = this.v_control.Width - 10 - this.v_textbox.Location.X;

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
        }

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public override void Disable()
        {
            this.v_textbox.Enabled = false;
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
        public void SetValue(string p_text)
        {
            this.v_textbox.Text = p_text;
        }

        /// <summary>
        /// Informa o texto ou valor a ser mostrado no Textbox.
        /// Usado para mostrar ao usuário um formulário já preenchido.
        /// O texto é passado criptografado, e é descriptografado para ser exibido ao usuário.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Textbox.</param>
        /// <param name="p_decryptpassword">Senha para descriptografar.</param>
        public void SetEncryptedValue(string p_text, string p_decryptpassword)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_decrypted;

            v_cryptor = new Spartacus.Net.Cryptor(p_decryptpassword);

            try
            {
                v_decrypted = v_cryptor.Decrypt(p_text);
            }
            catch (Spartacus.Net.Exception)
            {
                v_decrypted = p_text;
            }
            catch (System.Exception)
            {
                v_decrypted = p_text;
            }

            this.v_textbox.Text = v_decrypted;
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Textbox.
        /// </summary>
        /// <returns>Texto ou valor atual do Textbox.</returns>
        public string GetValue()
        {
            return this.v_textbox.Text;
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Textbox, porém criptografado.
        /// </summary>
        /// <returns>Texto ou valor atual do Textbox, porém criptografado.</returns>
        /// <param name="p_encryptpassword">Senha para criptografar.</param>
        public string GetEncryptedValue(string p_encryptpassword)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_encrypted;

            v_cryptor = new Spartacus.Net.Cryptor(p_encryptpassword);

            try
            {
                v_encrypted = v_cryptor.Encrypt(this.v_textbox.Text);
            }
            catch (Spartacus.Net.Exception)
            {
                v_encrypted = this.v_textbox.Text;
            }
            catch (System.Exception)
            {
                v_encrypted = this.v_textbox.Text;
            }

            return v_encrypted;
        }
    }
}
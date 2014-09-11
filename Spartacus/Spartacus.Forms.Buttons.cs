using System;

namespace Spartacus.Forms
{
    /// <summary>
    /// Classe Buttons.
    /// Herda da classe <see cref="Spartacus.Forms.Container"/> 
    /// Representa um componente com um ou mais botões.
    /// </summary>
    public class Buttons : Spartacus.Forms.Container
    {
        /// <summary>
        /// Lista de botões.
        /// </summary>
        public System.Collections.ArrayList v_list;

        /// <summary>
        /// Deslocamento horizontal.
        /// Usado para saber onde posicionar o próximo botão.
        /// </summary>
        public int v_offsetx;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Buttons"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        public Buttons(Spartacus.Forms.Container p_parent)
            : base(p_parent)
        {
            this.v_control = new System.Windows.Forms.Panel();

            this.SetWidth(p_parent.v_width);
            this.SetHeight(40);
            this.SetLocation(0, p_parent.v_offsety);

            this.v_list = new System.Collections.ArrayList();

            this.v_offsetx = this.v_width - 10;
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
            int v_dif;

            v_dif = p_newwidth - this.v_width;

            this.v_control.SuspendLayout();

            this.SetWidth(p_newwidth);
            this.SetLocation(p_newposx, p_newposy);

            foreach (System.Windows.Forms.Button v_button in this.v_list)
            {
                v_button.SuspendLayout();
                v_button.Location = new System.Drawing.Point(v_button.Location.X + v_dif, 10);
                v_button.ResumeLayout();
                v_button.Refresh();
            }

            this.v_control.ResumeLayout();
            this.v_control.Refresh();
        }

        /// <summary>
        /// Habilita o Container atual.
        /// </summary>
        public override void Enable()
        {
            foreach (System.Windows.Forms.Button v_button in this.v_list)
                v_button.Enabled = true;
        }

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public override void Disable()
        {
            foreach (System.Windows.Forms.Button v_button in this.v_list)
                v_button.Enabled = false;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
        }

        /// <summary>
        /// Adiciona um botão à lista de botões.
        /// </summary>
        /// <param name="p_text">Texto do botão.</param>
        /// <param name="p_delegate">Método que deve ser disparado quando o usuário clicar no botão.</param>
        public void AddButton(string p_text, System.EventHandler p_delegate)
        {
            System.Windows.Forms.Button v_button;

            v_button = new System.Windows.Forms.Button();
            v_button.Text = p_text;
            v_button.Width = 100;
            v_button.Location = new System.Drawing.Point(this.v_offsetx - v_button.Width, 10);
            v_button.Click += p_delegate;
            v_button.Parent = this.v_control;

            this.v_list.Add(v_button);

            this.v_offsetx = v_button.Location.X - 10;
        }
    }
}

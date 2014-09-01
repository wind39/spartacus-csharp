using System;

namespace Spartacus.Forms
{
    /// <summary>
    /// Classe Window.
    /// Representa uma Janela.
    /// </summary>
    public class Window : Spartacus.Forms.Container
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Window"/>.
        /// </summary>
        public Window(string p_text, int p_width, int p_height)
            :base()
        {
            this.v_control = new System.Windows.Forms.Form();
            this.v_control.Resize += new System.EventHandler(this.OnResize);

            this.v_control.Text = p_text;

            this.SetWidth(p_width);
            this.SetHeight(p_height);
        }

        /// <summary>
        /// Evento disparado ao ocorrer o evento de redimensionamento da Janela.
        /// </summary>
        public void OnResize(object sender, System.EventArgs e)
        {
            this.Resize(this.v_control.Width, this.v_control.Height, 0, 0);
        }

        /// <summary>
        /// Redimensiona a Janela atual.
        /// Também redimensiona todos os Containers filhos.
        /// </summary>
        /// <param name="p_newwidth">Nova largura.</param>
        /// <param name="p_newheight">Nova altura.</param>
        /// <param name="p_newposx">Nova posição X.</param>
        /// <param name="p_newposy">Nova posição Y.</param>
        public override void Resize(int p_newwidth, int p_newheight, int p_newposx, int p_newposy)
        {
            Spartacus.Forms.Container v_container;
            int k, posy;

            this.v_control.SuspendLayout();

            // redimensionando containers filhos
            posy = 0;
            for (k = 0; k < this.v_containers.Count; k++)
            {
                v_container = (Spartacus.Forms.Container)this.v_containers[k];

                v_container.Resize(
                    (int) ((double) p_newwidth * (double) v_container.v_width / (double) this.v_width),
                    (int) (((double) (p_newheight - this.v_frozenheight) * (double) v_container.v_height) / (double) (this.v_height - this.v_frozenheight)),
                    0,
                    posy
                );

                posy += v_container.v_height;
            }

            this.v_width = p_newwidth;
            this.v_height = p_newheight;

            this.v_control.ResumeLayout();
            this.v_control.Refresh();
        }
    }
}

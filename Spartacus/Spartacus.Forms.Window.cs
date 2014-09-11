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
        /// <param name="p_text">Título da Janela.</param>
        /// <param name="p_width">Largura da Janela.</param>
        /// <param name="p_height">Altura da Janela.</param>
        public Window(string p_text, int p_width, int p_height)
            :base()
        {
            this.v_control = new Spartacus.Forms.Form();
            ((Spartacus.Forms.Form) this.v_control).Resize += new System.EventHandler(this.OnResize);

            this.v_control.Text = p_text;

            this.SetWidth(p_width);
            this.SetHeight(p_height);
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Window"/>.
        /// </summary>
        /// <param name="p_text">Título da Janela.</param>
        /// <param name="p_width">Largura da Janela.</param>
        /// <param name="p_height">Altura da Janela.</param>
        /// <param name="p_parent">Janela pai.</param>
        public Window(string p_text, int p_width, int p_height, Spartacus.Forms.Window p_parent)
            :base()
        {
            this.v_control = new Spartacus.Forms.Form(p_parent);
            ((Spartacus.Forms.Form) this.v_control).Resize += new System.EventHandler(this.OnResize);

            this.v_control.Text = p_text;

            this.SetWidth(p_width);
            this.SetHeight(p_height);
        }

        /// <summary>
        /// Evento disparado ao ocorrer o evento de redimensionamento da Janela.
        /// </summary>
        public void OnResize(object sender, System.EventArgs e)
        {
            this.Resize(((Spartacus.Forms.Form) this.v_control).Width, ((Spartacus.Forms.Form) this.v_control).Height, 0, 0);
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

            ((Spartacus.Forms.Form) this.v_control).SuspendLayout();

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

            ((Spartacus.Forms.Form) this.v_control).ResumeLayout();
            ((Spartacus.Forms.Form) this.v_control).Refresh();
        }

        /// <summary>
        /// Habilita o Container atual.
        /// </summary>
        public override void Enable()
        {
            Spartacus.Forms.Container v_container;
            int k;

            for (k = 0; k < this.v_containers.Count; k++)
            {
                v_container = (Spartacus.Forms.Container)this.v_containers [k];
                v_container.Enable();
            }
        }

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public override void Disable()
        {
            Spartacus.Forms.Container v_container;
            int k;

            for (k = 0; k < this.v_containers.Count; k++)
            {
                v_container = (Spartacus.Forms.Container)this.v_containers [k];
                v_container.Disable();
            }
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            Spartacus.Forms.Container v_container;
            int k;

            for (k = 0; k < this.v_containers.Count; k++)
            {
                v_container = (Spartacus.Forms.Container)this.v_containers [k];
                v_container.Clear();
            }
        }

        /// <summary>
        /// Mostra a Janela atual.
        /// </summary>
        public void Show()
        {
            ((Spartacus.Forms.Form) this.v_control).Show();
        }

        /// <summary>
        /// Esconde a Janela atual.
        /// </summary>
        public void Hide()
        {
            ((Spartacus.Forms.Form) this.v_control).Hide();
        }

        /// <summary>
        /// Carrega a Janela atual para a memória.
        /// Deve ser chamada após a instanciação da Janela.
        /// Na prática, apenas mostra e em seguida esconde a Janela.
        /// </summary>
        public void Load()
        {
            ((Spartacus.Forms.Form) this.v_control).Show();
            ((Spartacus.Forms.Form) this.v_control).Hide();
        }
    }
}

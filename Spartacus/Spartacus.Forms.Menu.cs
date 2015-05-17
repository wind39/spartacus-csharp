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
    /// Classe Menu.
    /// Representa um componente Menu, que normalmente fica no topo de uma Janela.
    /// </summary>
    public class Menu : Spartacus.Forms.Container
    {
        /// <summary>
        /// Controle nativo MenuStrip.
        /// </summary>
        public System.Windows.Forms.MenuStrip v_menustrip;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Menu"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        public Menu(Spartacus.Forms.Container p_parent)
            :base(p_parent)
        {
            this.v_control = new System.Windows.Forms.Panel();

            this.SetWidth(p_parent.v_width);
            this.SetHeight(40);
            this.SetLocation(0, p_parent.v_offsety);

            v_menustrip = new System.Windows.Forms.MenuStrip();
            v_menustrip.Width = p_parent.v_width;
            v_menustrip.Parent = this.v_control;
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
            this.v_menustrip.SuspendLayout();

            this.SetWidth(p_newwidth);
            this.v_menustrip.Width = p_newwidth;

            this.v_menustrip.ResumeLayout();
            this.v_control.ResumeLayout();
            this.v_control.Refresh();
        }

        /// <summary>
        /// Habilita o Container atual.
        /// </summary>
        public override void Enable()
        {
            this.v_menustrip.Enabled = true;
        }

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public override void Disable()
        {
            this.v_menustrip.Enabled = false;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
        }

        /// <summary>
        /// Adiciona um grupo ao Menu.
        /// </summary>
        /// <returns>Retorna o grupo, para poder adicionar outros grupos ou itens a ele.</returns>
        /// <param name="p_text">Texto exibido no menu para representar o grupo.</param>
        public System.Windows.Forms.ToolStripMenuItem AddGroup(string p_text)
        {
            System.Windows.Forms.ToolStripMenuItem v_menugroup;

            v_menugroup = new System.Windows.Forms.ToolStripMenuItem();
            v_menugroup.Text = p_text;

            this.v_menustrip.Items.Add(v_menugroup);

            return v_menugroup;
        }

        /// <summary>
        /// Adiciona um grupo a um grupo existente.
        /// </summary>
        /// <returns>Retorna o grupo, para poder adicionar outros grupos ou itens a ele.</returns>
        /// <param name="p_menugroup">Grupo existente.</param>
        /// <param name="p_text">Texto exibido no menu para representar o grupo.</param>
        public System.Windows.Forms.ToolStripMenuItem AddGroup(System.Windows.Forms.ToolStripMenuItem p_menugroup, string p_text)
        {
            System.Windows.Forms.ToolStripMenuItem v_menugroup;

            v_menugroup = new System.Windows.Forms.ToolStripMenuItem();
            v_menugroup.Text = p_text;

            p_menugroup.DropDownItems.Add(v_menugroup);

            return v_menugroup;
        }

        /// <summary>
        /// Adiciona um item a um grupo do Menu.
        /// </summary>
        /// <param name="p_menugroup">Grupo existente.</param>
        /// <param name="p_text">Texto exibido no menu para representar o item.</param>
        /// <param name="p_delegate">Método executado ao clicar no item do Menu.</param>
        public void AddItem(System.Windows.Forms.ToolStripMenuItem p_menugroup, string p_text, System.EventHandler p_delegate)
        {
            System.Windows.Forms.ToolStripMenuItem v_menuitem;

            v_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            v_menuitem.Text = p_text;
            if (p_delegate != null)
                v_menuitem.Click += p_delegate;

            p_menugroup.DropDownItems.Add(v_menuitem);
        }
    }
}

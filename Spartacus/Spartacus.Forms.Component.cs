using System;

namespace Spartacus.Forms
{
    public abstract class Component
    {
        public Spartacus.Forms.Container v_parent;

        public System.Windows.Forms.Panel v_panel;

        public int v_width;
        public int v_height;

        public bool v_frozenheight;

        public int v_posx;
        public int v_posy;


        public Component(Spartacus.Forms.Container p_parent)
        {
            this.v_parent = p_parent;

            this.v_panel = new System.Windows.Forms.Panel();

            this.SetWidth(p_parent.v_width);
            this.SetHeight(40);

            this.v_frozenheight = true;

            this.SetLocation(0, p_parent.v_offsety);
        }

        public void SetWidth(int p_width)
        {
            this.v_width = p_width;
            this.v_panel.Width = p_width;
        }

        public void SetHeight(int p_height)
        {
            this.v_height = p_height;
            this.v_panel.Height = p_height;
        }

        public void SetLocation(int p_posx, int p_posy)
        {
            this.v_posx = p_posx;
            this.v_posy = p_posy;
            this.v_panel.Location = new System.Drawing.Point(p_posx, p_posy);
        }

        public abstract void Resize(int p_newwidth, int p_newheight, int p_newposx, int p_newposy);
    }
}

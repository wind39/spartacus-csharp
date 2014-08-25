using System;

namespace Spartacus.Forms
{
    public class Buttons : Spartacus.Forms.Component
    {
        public System.Collections.ArrayList v_list;

        public int v_offsetx;


        public Buttons(Spartacus.Forms.Container p_parent)
            : base(p_parent)
        {
            this.v_list = new System.Collections.ArrayList();

            this.v_offsetx = this.v_width - 15;

            this.v_panel.BackColor = System.Drawing.Color.Blue;
        }

        public override void SetTitle(string p_title)
        {
            this.v_title = p_title;
        }

        public override void Resize(int p_newwidth, int p_newheight, int p_newposx, int p_newposy)
        {
            int v_dif;

            v_dif = p_newwidth - this.v_width;

            this.v_panel.SuspendLayout();

            this.v_panel.Location = new System.Drawing.Point(p_newposx, p_newposy);

            this.v_width = p_newwidth;
            this.v_panel.Width = p_newwidth;

            foreach (System.Windows.Forms.Button v_button in this.v_list)
            {
                v_button.SuspendLayout();
                v_button.Location = new System.Drawing.Point(v_button.Location.X + v_dif, 10);
                v_button.ResumeLayout();
                v_button.Refresh();
            }

            this.v_panel.ResumeLayout();
            this.v_panel.Refresh();
        }

        public override void Populate(System.Data.DataTable p_table)
        {
        }

        public override void AddButton(string p_text, System.EventHandler p_delegate)
        {
            System.Windows.Forms.Button v_button;

            v_button = new System.Windows.Forms.Button();
            v_button.Text = p_text;
            v_button.Width = 100;
            v_button.Location = new System.Drawing.Point(this.v_offsetx - v_button.Width, 10);
            v_button.Click += p_delegate;
            v_button.Parent = this.v_panel;

            this.v_list.Add(v_button);
        }
    }
}


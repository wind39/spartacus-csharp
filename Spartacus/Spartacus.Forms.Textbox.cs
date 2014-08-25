using System;

namespace Spartacus.Forms
{
    public class Textbox : Spartacus.Forms.Component
    {
        public System.Windows.Forms.Label v_label;

        public System.Windows.Forms.TextBox v_textbox;

        public int v_proportion;


        public Textbox(Spartacus.Forms.Container p_parent)
            : base(p_parent)
        {
            if (this.v_showlabel)
            {
                this.v_label = new System.Windows.Forms.Label();
                this.v_label.Location = new System.Drawing.Point(10, 15);
                this.v_label.AutoSize = true;
                //this.v_label.Name = "label1";
                this.v_label.Parent = this.v_panel;
            }

            this.v_proportion = 50;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion / (double) 100)), 10);
            //this.v_textbox.Name = "";
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;
            this.v_textbox.Parent = this.v_panel;
        }

        public Textbox(Spartacus.Forms.Container p_parent, int p_proportion)
            : base(p_parent)
        {
            if (this.v_showlabel)
            {
                this.v_label = new System.Windows.Forms.Label();
                this.v_label.Location = new System.Drawing.Point(10, 15);
                //this.v_label.Name = "label1";
                this.v_label.Parent = this.v_panel;
            }

            this.v_proportion = p_proportion;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion / (double) 100)), 10);
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;
            //this.v_textbox.Name = "";
            this.v_textbox.Parent = this.v_panel;
        }

        public override void SetTitle(string p_title)
        {
            this.v_title = p_title;
            this.v_label.Text = p_title;
        }

        public override void Resize(int p_newwidth, int p_newheight, int p_newposx, int p_newposy)
        {
            this.v_panel.SuspendLayout();
            this.v_textbox.SuspendLayout();

            this.v_panel.Location = new System.Drawing.Point(p_newposx, p_newposy);

            this.v_width = p_newwidth;
            this.v_panel.Width = p_newwidth;

            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion / (double) 100)), 10);
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;

            this.v_textbox.ResumeLayout();
            this.v_panel.ResumeLayout();
            this.v_panel.Refresh();
        }

        public override void Populate(System.Data.DataTable p_table)
        {
        }

        public override void AddButton(string p_text, System.EventHandler p_delegate)
        {
        }
    }
}

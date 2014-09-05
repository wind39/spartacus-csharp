using System;

namespace Spartacus.Forms
{
    public class Textbox : Spartacus.Forms.Container
    {
        public System.Windows.Forms.Label v_label;

        public System.Windows.Forms.TextBox v_textbox;

        public int v_proportion;


        public Textbox(Spartacus.Forms.Container p_parent, string p_label)
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
            this.v_textbox.Parent = this.v_control;
        }

        public Textbox(Spartacus.Forms.Container p_parent, string p_label, int p_proportion)
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
            this.v_textbox.Parent = this.v_control;
        }

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

        public override void Enable()
        {
            this.v_textbox.Enabled = true;
        }

        public override void Disable()
        {
            this.v_textbox.Enabled = false;
        }

        public override void Clear()
        {
            this.v_textbox.Text = "";
        }

        public void SetValue(string p_text)
        {
            this.v_textbox.Text = p_text;
        }

        public string GetValue()
        {
            return this.v_textbox.Text;
        }
    }
}

using System;

namespace Spartacus.Forms
{
    public class Menu : Spartacus.Forms.Container
    {
        public System.Windows.Forms.MenuStrip v_menustrip;


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

        public override void Enable()
        {
            this.v_menustrip.Enabled = true;
        }

        public override void Disable()
        {
            this.v_menustrip.Enabled = false;
        }

        public override void Clear()
        {
        }

        public System.Windows.Forms.ToolStripMenuItem AddGroup(string p_text, System.EventHandler p_delegate)
        {
            System.Windows.Forms.ToolStripMenuItem v_menugroup;

            v_menugroup = new System.Windows.Forms.ToolStripMenuItem();
            v_menugroup.Text = p_text;
            v_menugroup.Click += p_delegate;

            this.v_menustrip.Items.Add(v_menugroup);

            return v_menugroup;
        }

        public System.Windows.Forms.ToolStripMenuItem AddGroup(System.Windows.Forms.ToolStripMenuItem p_menugroup, string p_text, System.EventHandler p_delegate)
        {
            System.Windows.Forms.ToolStripMenuItem v_menugroup;

            v_menugroup = new System.Windows.Forms.ToolStripMenuItem();
            v_menugroup.Text = p_text;
            v_menugroup.Click += p_delegate;

            p_menugroup.DropDownItems.Add(v_menugroup);

            return v_menugroup;
        }

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

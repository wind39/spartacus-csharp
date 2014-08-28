using System;

namespace Spartacus.Forms
{
    public class Lookup : Spartacus.Forms.Component
    {
        public System.Windows.Forms.Label v_label;

        public Spartacus.Forms.MultiColumnComboBox v_lookup;

        public System.Windows.Forms.TextBox v_textbox;

        public int v_proportion1;

        public int v_proportion2;

        //public bool v_frozenlocation;


        public Lookup(Spartacus.Forms.Container p_parent)
            : base(p_parent)
        {
            //this.v_frozenlocation = true;

            this.v_label = new System.Windows.Forms.Label();
            this.v_label.Location = new System.Drawing.Point(10, 10);
            this.v_label.AutoSize = true;
            this.v_label.Parent = this.v_panel;

            this.v_proportion1 = 40;
            this.v_proportion2 = 70;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Enabled = false;
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion2 / (double) 100)), 5);
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;
            this.v_textbox.Parent = this.v_panel;

            this.v_lookup = new Spartacus.Forms.MultiColumnComboBox();
            this.v_lookup.AutoComplete = false;
            this.v_lookup.AutoDropdown = true;
            this.v_lookup.BackColorEven = System.Drawing.Color.White;
            this.v_lookup.BackColorOdd = System.Drawing.Color.Silver;
            this.v_lookup.ColumnWidthDefault = 75;
            this.v_lookup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.v_lookup.FormattingEnabled = true;
            this.v_lookup.LinkedColumnIndex = 1;
            this.v_lookup.LinkedTextBox = this.v_textbox;
            this.v_lookup.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion1 / (double) 100)), 5);
            this.v_lookup.Width = this.v_textbox.Location.X - this.v_lookup.Location.X - 5;
            this.v_lookup.Parent = this.v_panel;
        }

        public Lookup(Spartacus.Forms.Container p_parent, int p_proportion1, int p_proportion2)
            : base(p_parent)
        {
            this.v_label = new System.Windows.Forms.Label();
            this.v_label.Location = new System.Drawing.Point(10, 10);
            this.v_label.AutoSize = true;
            this.v_label.Parent = this.v_panel;

            this.v_proportion1 = p_proportion1;
            this.v_proportion2 = p_proportion2;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion2 / (double) 100)), 5);
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;
            this.v_textbox.Parent = this.v_panel;

            this.v_lookup = new Spartacus.Forms.MultiColumnComboBox();
            this.v_lookup.AutoComplete = false;
            this.v_lookup.AutoDropdown = true;
            this.v_lookup.BackColorEven = System.Drawing.Color.White;
            this.v_lookup.BackColorOdd = System.Drawing.Color.Silver;
            this.v_lookup.ColumnWidthDefault = 75;
            this.v_lookup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.v_lookup.FormattingEnabled = true;
            this.v_lookup.LinkedColumnIndex = 1;
            this.v_lookup.LinkedTextBox = this.v_textbox;
            this.v_lookup.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion1 / (double) 100)), 5);
            this.v_lookup.Width = this.v_textbox.Location.X - this.v_lookup.Location.X - 5;
            this.v_lookup.Parent = this.v_panel;
        }

        public override void Resize(int p_newwidth, int p_newheight, int p_newposx, int p_newposy)
        {
            this.v_panel.SuspendLayout();
            this.v_textbox.SuspendLayout();
            this.v_lookup.SuspendLayout();

            this.v_panel.Location = new System.Drawing.Point(p_newposx, p_newposy);

            this.v_width = p_newwidth;
            this.v_panel.Width = p_newwidth;

            //if (! this.v_frozenlocation)
            //    this.v_lookup.Location = new System.Drawing.Point((int) (this.v_panel.Width * ((double) this.v_proportion1 / (double) 100)), 5);
            //this.v_textbox.Location = new System.Drawing.Point((int) (this.v_panel.Width * ((double) this.v_proportion2 / (double) 100)), 5);

            this.v_textbox.Width = this.v_panel.Width - 10 - this.v_textbox.Location.X;
            this.v_lookup.Width = this.v_textbox.Location.X - this.v_lookup.Location.X - 5;

            this.v_lookup.ResumeLayout();
            this.v_textbox.ResumeLayout();
            this.v_panel.ResumeLayout();
            this.v_panel.Refresh();
        }

        public void SetLabel(string p_title)
        {
            this.v_label.Text = p_title;
        }

        public void Populate(System.Data.DataTable p_table, string p_columnvalue, string p_columndisplay, string p_columnwidths)
        {
            string v_columnnames;
            int k;

            v_columnnames = p_table.Columns[0].ColumnName;
            for (k = 1; k < p_table.Columns.Count; k++)
                v_columnnames += ";" + p_table.Columns[k].ColumnName;

            this.v_lookup.ColumnNames = v_columnnames;
            this.v_lookup.ColumnWidths = p_columnwidths;

            this.v_lookup.DataSource = p_table;
            this.v_lookup.DisplayMember = p_columnvalue;
            this.v_lookup.ValueMember = p_columndisplay;
            this.v_lookup.SelectedIndex = -1;
            this.v_lookup.Text = "";
        }
    }
}

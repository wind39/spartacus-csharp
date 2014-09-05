using System;

namespace Spartacus.Forms
{
    public class Grid : Spartacus.Forms.Container
    {
        public System.Windows.Forms.DataGridView v_grid;


        public Grid(Spartacus.Forms.Container p_parent, int p_width, int p_height)
            : base(p_parent)
        {
            this.v_control = new System.Windows.Forms.Panel();

            this.v_isfrozen = false;

            this.v_width = p_width;
            this.v_control.Width = p_width - 5;

            this.SetHeight(p_height);
            this.SetLocation(0, p_parent.v_offsety);

            this.v_grid = new System.Windows.Forms.DataGridView();
            this.v_grid.RowHeadersVisible = false;
            this.v_grid.EnableHeadersVisualStyles = false;
            this.v_grid.ColumnHeadersHeight = 20;
            this.v_grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.v_grid.AutoGenerateColumns = true;
            this.v_grid.ReadOnly = true;
            this.v_grid.MultiSelect = false;
            this.v_grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.v_grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.v_grid.DoubleBuffered(true);
            this.v_grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.v_grid.AllowDrop = false;
            this.v_grid.AllowUserToAddRows = false;
            this.v_grid.AllowUserToDeleteRows = false;
            this.v_grid.AllowUserToOrderColumns = true;
            this.v_grid.AllowUserToResizeColumns = true;
            this.v_grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_grid.Parent = this.v_control;
        }

        public override void Resize(int p_newwidth, int p_newheight, int p_newposx, int p_newposy)
        {
            this.v_control.SuspendLayout();
            this.v_grid.SuspendLayout();

            this.v_width = p_newwidth;
            this.v_control.Width = p_newwidth - 5;

            this.SetHeight(p_newheight);
            this.SetLocation(p_newposx, p_newposy);

            this.v_grid.ResumeLayout();
            this.v_control.ResumeLayout();
            this.v_control.Refresh();
        }

        public override void Enable()
        {
            this.v_grid.Enabled = true;
        }

        public override void Disable()
        {
            this.v_grid.Enabled = false;
        }

        public override void Clear()
        {
        }

        public void Populate(System.Data.DataTable p_table)
        {
            this.v_grid.DataSource = p_table;
            this.v_grid.AutoResizeColumns(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells);

            if (p_table != null && ((System.Data.DataTable)this.v_grid.DataSource).Rows.Count > 0)
                this.v_grid.Rows[0].Selected = true;
        }

        public System.Data.DataRow CurrentRow()
        {
            if (this.v_grid.CurrentRow.Index >= 0 &&
                this.v_grid.CurrentRow.Index < this.v_grid.Rows.Count)
            {
                return ((System.Data.DataTable) this.v_grid.DataSource).Rows[this.v_grid.CurrentRow.Index];
            }
            else
            {
                return null;
            }
        }
    }

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this System.Windows.Forms.DataGridView p_grid, bool p_setting)
        {
            System.Type v_type;
            System.Reflection.PropertyInfo v_property;

            v_type = p_grid.GetType();
            v_property = v_type.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            v_property.SetValue(p_grid, p_setting, null);
        }
    }
}

/*
The MIT License (MIT)

Copyright (c) 2014 William Ivanski

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
    /// Classe Grid.
    /// Representa um componente Grid.
    /// Herda da classe <see cref="Spartacus.Forms.Container"/>. 
    /// </summary>
    public class Grid : Spartacus.Forms.Container
    {
        /// <summary>
        /// O controle do Grid propriamente dito.
        /// </summary>
        public System.Windows.Forms.DataGridView v_grid;

        /// <summary>
        /// Objeto de conexão com o banco de dados.
        /// </summary>
        public Spartacus.Database.Generic v_database;

        /// <summary>
        /// Consulta SQL para alimentar o Grid.
        /// </summary>
        public string v_sql;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Grid"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        /// <param name="p_width">Largura.</param>
        /// <param name="p_height">Altura.</param>
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

            this.v_database = null;
            this.v_sql = null;
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
            this.v_grid.SuspendLayout();

            this.v_width = p_newwidth;
            this.v_control.Width = p_newwidth - 5;

            this.SetHeight(p_newheight);
            this.SetLocation(p_newposx, p_newposy);

            this.v_grid.ResumeLayout();
            this.v_control.ResumeLayout();
            this.v_control.Refresh();
        }

        /// <summary>
        /// Habilita o Container atual.
        /// </summary>
        public override void Enable()
        {
            this.v_grid.Enabled = true;
        }

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public override void Disable()
        {
            this.v_grid.Enabled = false;
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
            this.Populate();
        }

        /// <summary>
        /// Popula o Grid atual com os dados obtidos a partir da execução da consulta SQL no banco de dados.
        /// </summary>
        /// <param name="p_database">Objeto de conexão com o banco de dados.</param>
        /// <param name="p_sql">Consulta SQL.</param>
        public void Populate(Spartacus.Database.Generic p_database, string p_sql)
        {
            this.v_database = p_database;
            this.v_sql = p_sql;

            this.v_grid.DataSource = this.v_database.Query(this.v_sql, "GRID");
            this.v_grid.AutoResizeColumns(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells);
        }

        /// <summary>
        /// Popula o Grid atual com os dados obtidos a partir da execução da consulta SQL no banco de dados.
        /// </summary>
        public void Populate()
        {
            if (this.v_database != null)
            {
                this.v_grid.DataSource = this.v_database.Query(this.v_sql, "GRID");
                this.v_grid.AutoResizeColumns(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        /// <summary>
        /// Retorna a linha selecionada.
        /// </summary>
        /// <returns>Linha selecionada.</returns>
        public System.Data.DataRow CurrentRow()
        {
            if (this.v_grid.Rows.Count > 0 &&
                this.v_grid.CurrentRow.Index >= 0 &&
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

    /// <summary>
    /// Classe ExtensionMethods.
    /// Usada somente para colocar o Grid em modo DoubleBuffered, que faz com que renderize o grid mais rápido em troca de mais memória.
    /// </summary>
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

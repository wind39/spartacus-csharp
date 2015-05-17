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
    /// Classe Lookup.
    /// Representa um componente Lookup.
    /// Herda da classe <see cref="Spartacus.Forms.Container"/>.
    /// </summary>
    public class Lookup : Spartacus.Forms.Container
    {
        /// <summary>
        /// Rótulo do Lookup.
        /// </summary>
        public System.Windows.Forms.Label v_label;

        /// <summary>
        /// Componente Lookup.
        /// </summary>
        public ThirdParty.MultiColumnComboBox v_lookup;

        /// <summary>
        /// Textbox não editável, atualizado pelo componente Lookup.
        /// </summary>
        public System.Windows.Forms.TextBox v_textbox;

        /// <summary>
        /// Proporção entre o Label e o componente Lookup.
        /// </summary>
        public int v_proportion1;

        /// <summary>
        /// Proporção entre o componente Lookup e o Textbox.
        /// </summary>
        public int v_proportion2;

        /// <summary>
        /// Objeto de conexão com o banco de dados.
        /// </summary>
        public Spartacus.Database.Generic v_database;

        /// <summary>
        /// Consulta SQL para alimentar o Lookup.
        /// </summary>
        public string v_sql;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Lookup"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        /// <param name="p_label">Rótulo do componente.</param>
        public Lookup(Spartacus.Forms.Container p_parent, string p_label)
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

            this.v_proportion1 = 40;
            this.v_proportion2 = 70;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Enabled = false;
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion2 / (double) 100)), 5);
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;
            this.v_textbox.Parent = this.v_control;

            this.v_lookup = new ThirdParty.MultiColumnComboBox();
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
            this.v_lookup.Parent = this.v_control;

            this.v_database = null;
            this.v_sql = null;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Lookup"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        /// <param name="p_label">Rótulo do componente.</param>
        /// <param name="p_proportion1">Proporção entre o Label e o componente Lookup.</param>
        /// <param name="p_proportion2">Proporção entre o componente Lookup e o Textbox.</param>
        public Lookup(Spartacus.Forms.Container p_parent, string p_label, int p_proportion1, int p_proportion2)
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

            this.v_proportion1 = p_proportion1;
            this.v_proportion2 = p_proportion2;

            this.v_textbox = new System.Windows.Forms.TextBox();
            this.v_textbox.Location = new System.Drawing.Point((int) (this.v_width * ((double) this.v_proportion2 / (double) 100)), 5);
            this.v_textbox.Width = this.v_width - 10 - this.v_textbox.Location.X;
            this.v_textbox.Parent = this.v_control;

            this.v_lookup = new ThirdParty.MultiColumnComboBox();
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
            this.v_lookup.Parent = this.v_control;
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
            this.v_textbox.SuspendLayout();
            this.v_lookup.SuspendLayout();

            this.SetWidth(p_newwidth);
            this.SetLocation(p_newposx, p_newposy);

            this.v_textbox.Width = this.v_control.Width - 10 - this.v_textbox.Location.X;
            this.v_lookup.Width = this.v_textbox.Location.X - this.v_lookup.Location.X - 5;

            this.v_lookup.ResumeLayout();
            this.v_textbox.ResumeLayout();
            this.v_control.ResumeLayout();
            this.v_control.Refresh();
        }

        /// <summary>
        /// Habilita o Container atual.
        /// </summary>
        public override void Enable()
        {
            this.v_lookup.Enabled = true;
        }

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public override void Disable()
        {
            this.v_lookup.Enabled = false;
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
        /// Informa o valor do componente Lookup.
        /// Usado para mostrar um formulário já preenchido ao usuário.
        /// </summary>
        /// <param name="p_text">Valor do componente (não do textbox).</param>
        public void SetValue(string p_text)
        {
            System.Data.DataRow v_row;
            bool v_achou;
            int k;

            k = 0;
            v_achou = false;
            while (k < ((System.Data.DataTable)this.v_lookup.DataSource).Rows.Count && ! v_achou)
            {
                v_row = ((System.Data.DataTable) this.v_lookup.DataSource).Rows[k];

                if (v_row [this.v_lookup.DisplayMember].ToString() == p_text)
                {
                    this.v_lookup.SelectedIndex = k;

                    v_achou = true;
                }
                else
                    k++;
            }
        }

        /// <summary>
        /// Retorna o valor atual do componente.
        /// </summary>
        /// <returns>Valor atual do componente.</returns>
        public string GetValue()
        {
            if (this.v_lookup.SelectedIndex >= 0 &&
                this.v_lookup.SelectedIndex < ((System.Data.DataTable)this.v_lookup.DataSource).Rows.Count)
            {
                return ((System.Data.DataTable)this.v_lookup.DataSource).Rows [this.v_lookup.SelectedIndex] [this.v_lookup.DisplayMember].ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Popula o componente Lookup com os dados obtidos a partir da execução da consulta SQL no banco de dados.
        /// </summary>
        /// <param name="p_database">Objeto de conexão com o banco de dados.</param>
        /// <param name="p_columnvalue">Coluna de valor.</param>
        /// <param name="p_columndisplay">Coluna para mostrar no Textbox.</param>
        /// <param name="p_columnwidths">Larguras das colunas mostradas no Lookup, separadas por ponto-e-vírgula.</param>
        public void Populate(Spartacus.Database.Generic p_database, string p_sql, string p_columnvalue, string p_columndisplay, string p_columnwidths)
        {
            System.Data.DataTable v_table;
            string v_columnnames;
            int k;

            this.v_database = p_database;
            this.v_sql = p_sql;

            v_table = this.v_database.Query(this.v_sql, "LOOKUP");

            v_columnnames = v_table.Columns[0].ColumnName;
            for (k = 1; k < v_table.Columns.Count; k++)
                v_columnnames += ";" + v_table.Columns[k].ColumnName;

            this.v_lookup.ColumnNames = v_columnnames;
            this.v_lookup.ColumnWidths = p_columnwidths;

            this.v_lookup.DataSource = v_table;
            this.v_lookup.DisplayMember = p_columnvalue;
            this.v_lookup.ValueMember = p_columndisplay;
            this.v_lookup.SelectedIndex = -1;
            this.v_lookup.Text = "";
        }

        /// <summary>
        /// Popula o componente Lookup com os dados obtidos a partir da execução da consulta SQL no banco de dados.
        /// </summary>
        public void Populate()
        {
            if (this.v_database != null)
                this.v_lookup.DataSource = this.v_database.Query(this.v_sql, "LOOKUP");
        }
    }
}

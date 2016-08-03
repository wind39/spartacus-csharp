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

namespace Spartacus.Web
{
    /// <summary>
    /// Classe Grid.
    /// Representa um componente Grid.
    /// Herda da classe <see cref="Spartacus.Web.Container"/>. 
    /// </summary>
    public class Grid : Spartacus.Web.Container
    {
        /// <summary>
        /// Table HTML representando o grid.
        /// </summary>
        public string v_gridhtml;

        /// <summary>
        /// Controle nativo que representa o valor selecionado.
        /// </summary>
        public System.Web.UI.HtmlControls.HtmlGenericControl v_selected;


        /// <summary>
        /// Objeto de conexão com o banco de dados.
        /// </summary>
        public Spartacus.Database.Generic v_database;

        /// <summary>
        /// Consulta SQL para alimentar o Grid.
        /// </summary>
        public string v_sql;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Grid"/>.
        /// </summary>
        /// <param name="p_id">Identificador do Container atual.</param>
        /// <param name="p_parent">Container pai.</param>
        public Grid(string p_id, Spartacus.Web.Container p_parent)
            : base(p_id, p_parent)
        {
            this.v_type = Spartacus.Web.ContainerType.GRID;

            this.v_selected = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
            this.v_selected.ID = p_id;
            this.v_selected.Attributes.Add("runat", "server");
            this.v_selected.Attributes.Add("type", "text");
            this.v_selected.Attributes.Add("value", "");
            this.v_selected.Style.Add("display", "none");

            this.v_database = null;
            this.v_sql = null;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            this.v_selected.Attributes["value"] = "";
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
            this.Populate();
        }

        /// <summary>
        /// Informa o texto ou valor a ser mostrado no Container.
        /// Usado para mostrar ao usuário um formulário já preenchido.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Container.</param>
        public override void SetValue(string p_text)
        {
            this.v_selected.Attributes["value"] = p_text;
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Container.
        /// </summary>
        /// <returns>Texto ou valor atual do Container.</returns>
        public override string GetValue()
        {
            return this.v_selected.Attributes["value"];
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

            this.v_gridhtml = this.v_database.QueryHtml(this.v_sql, "grid_" + this.v_id, "class='display compact'");
        }

        /// <summary>
        /// Popula o Grid atual com os dados de um DataTable.
        /// </summary>
        public void Populate(System.Data.DataTable p_table)
        {
            
        }

        /// <summary>
        /// Popula o Grid atual com os dados obtidos a partir da execução da consulta SQL no banco de dados.
        /// </summary>
        public void Populate()
        {
            if (this.v_database != null)
            {
                this.v_gridhtml = this.v_database.QueryHtml(this.v_sql, "grid_" + this.v_id, "class='display compact'");
            }
        }

        /// <summary>
        /// Renderiza o HTML do Container.
        /// </summary>
        public override string Render()
        {
            string v_html;
            System.Text.StringBuilder v_builder;
            System.Web.UI.HtmlTextWriter v_writer;

            v_builder = new System.Text.StringBuilder();
            v_writer = new System.Web.UI.HtmlTextWriter(new System.IO.StringWriter(v_builder));

            this.v_selected.RenderControl(v_writer);

            v_html = v_builder.ToString();
            v_html += this.v_gridhtml;

            return v_html;
        }
    }
}

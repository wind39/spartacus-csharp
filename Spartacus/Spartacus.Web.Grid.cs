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
        public string v_html;

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

            this.v_database = null;
            this.v_sql = null;
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
        /// Informa o texto ou valor a ser mostrado no Container.
        /// Usado para mostrar ao usuário um formulário já preenchido.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Container.</param>
        public override void SetValue(string p_text)
        {
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Container.
        /// </summary>
        /// <returns>Texto ou valor atual do Container.</returns>
        public override string GetValue()
        {
            return null;
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

            this.v_html = this.v_database.QueryHtml(this.v_sql, this.v_id, "class='display compact' cellspacing='0' width='100%'");
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
                this.v_html = this.v_database.QueryHtml(this.v_sql, this.v_id, "class='display' cellspacing='0' width='100%'");
            }
        }

        /// <summary>
        /// Renderiza o HTML do Container.
        /// </summary>
        public override string Render()
        {
            return this.v_html;
        }
    }
}

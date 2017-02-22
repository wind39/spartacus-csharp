/*
The MIT License (MIT)

Copyright (c) 2014-2017 William Ivanski

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

namespace Spartacus.Reporting
{
    /// <summary>
    /// Classe Group.
    /// Representa um agrupamento de dados do relatório.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Nível do grupo.
        /// </summary>
        public int v_level;

        /// <summary>
        /// Coluna associada ao grupo.
        /// </summary>
        public string v_column;

        /// <summary>
        /// Ordenação do grupo.
        /// </summary>
        public string v_sort;

        /// <summary>
        /// Indica se o cabeçalho do grupo deve ser mostrado ou não.
        /// </summary>
        public bool v_showheader;

        /// <summary>
        /// Indica se o rodapé do grupo deve ser mostrado ou não.
        /// </summary>
        public bool v_showfooter;

        /// <summary>
        /// Lista de campos do cabeçalho.
        /// </summary>
        public System.Collections.Generic.List<Spartacus.Reporting.Field> v_headerfields;

        /// <summary>
        /// Lista de campos do rodapé.
        /// </summary>
        public System.Collections.Generic.List<Spartacus.Reporting.Field> v_footerfields;

        /// <summary>
        /// Tabela com os dados do grupo.
        /// </summary>
        public System.Data.DataTable v_table;

        /// <summary>
        /// Número de linhas dentro do cabeçalho do grupo.
        /// </summary>
        public int v_numrowsheader;

        /// <summary>
        /// Número de linhas dentro do rodapé do grupo.
        /// </summary>
        public int v_numrowsfooter;

        /// <summary>
        /// Modelo de renderização do cabeçalho do grupo.
        /// </summary>
        public System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_headertemplate;

        /// <summary>
        /// Modelo de renderização do rodapé do grupo.
        /// </summary>
        public System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_footertemplate;

        /// <summary>
        /// Número de linhas da tabela de dados do grupo que já foram renderizadas.
        /// </summary>
        public int v_renderedrows;

        /// <summary>
        /// Indica se os títulos dos campos de cabeçalho do grupo devem ser mostrados no cabeçalho de dados ou não.
        /// </summary>
        public bool v_showheadertitles;

        /// <summary>
        /// Indica se os títulos dos campos de rodapé do grupo devem ser mostrados no cabeçalho de dados ou não.
        /// </summary>
        public bool v_showfootertitles;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Group"/>.
        /// </summary>
        public Group()
        {
            this.v_showheader = true;
            this.v_showfooter = true;

            this.v_headerfields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_footerfields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();

            this.v_renderedrows = 0;

            this.v_showheadertitles = false;
            this.v_showfootertitles = false;
        }

        /// <summary>
        /// Constrói dados do grupo.
        /// Percorre tabela de dados do relatório, filtrando e distinguindo os dados pela coluna do grupo.
        /// </summary>
        /// <param name="p_table">Tabela de dados do relatório.</param>
        /// <param name="p_parentgroupcolumn">Coluna do grupo pai.</param>
        public void Build(System.Data.DataTable p_table, string p_parentgroupcolumn)
        {
            System.Collections.Generic.List<string> v_allcolumns_temp;
            string[] v_allcolumns;
            int k;

            // alocando lista de colunas
            v_allcolumns_temp = new System.Collections.Generic.List<string>();

            // adicionando coluna do grupo pai
            if (p_parentgroupcolumn != null && p_parentgroupcolumn != "")
                v_allcolumns_temp.Add(p_parentgroupcolumn);

            // adicionando coluna do grupo
            v_allcolumns_temp.Add(this.v_column);

            // adicionando todas as colunas do cabeçalho do grupo
            for (k = 0; k < this.v_headerfields.Count; k++)
            {
                if (! v_allcolumns_temp.Contains(this.v_headerfields[k].v_column) && this.v_headerfields[k].v_column != "")
                    v_allcolumns_temp.Add(this.v_headerfields[k].v_column);
            }

            // adicionando todas as colunas do rodapé do grupo
            for (k = 0; k < this.v_footerfields.Count; k++)
            {
                if (! v_allcolumns_temp.Contains(this.v_footerfields[k].v_column) && this.v_footerfields[k].v_column != "")
                    v_allcolumns_temp.Add(this.v_footerfields[k].v_column);
            }

            // alocando vetor de string
            v_allcolumns = new string[v_allcolumns_temp.Count];

            // copiando nomes de colunas para o vetor de string
            for (k = 0; k < v_allcolumns_temp.Count; k++)
                v_allcolumns[k] = v_allcolumns_temp[k];

            // filtrando dados distintos pela lista de colunas, e armazenando em tabela
            if (p_parentgroupcolumn != null && p_parentgroupcolumn != "")
                p_table.DefaultView.Sort = p_parentgroupcolumn + ", " + this.v_column;
            else
                p_table.DefaultView.Sort = this.v_column;
            this.v_table = p_table.DefaultView.ToTable(true, v_allcolumns);
        }

        /// <summary>
        /// Constrói dados do grupo.
        /// Percorre tabela de dados do relatório, filtrando e distinguindo os dados pela coluna do grupo.
        /// Também calcula os valores de cada grupo, totalizando-os.
        /// </summary>
        /// <param name="p_table">Tabela de dados do relatório.</param>
        /// <param name="p_parentgroupcolumn">Coluna do grupo pai.</param>
        public void BuildCalculate(System.Data.DataTable p_table, string p_parentgroupcolumn)
        {
            System.Collections.Generic.List<string> v_allcolumns_temp;
            string[] v_allcolumns;
            int i, j, k;

            // PASSO 1: PEGANDO COLUNAS E DADOS DO GRUPO

            // alocando lista de colunas
            v_allcolumns_temp = new System.Collections.Generic.List<string>();

            // adicionando coluna do grupo pai
            if (p_parentgroupcolumn != null && p_parentgroupcolumn != "")
                v_allcolumns_temp.Add(p_parentgroupcolumn);

            // adicionando coluna do grupo
            v_allcolumns_temp.Add(this.v_column);

            // adicionando todas as colunas do cabeçalho do grupo (exceto as colunas de valor)
            for (k = 0; k < this.v_headerfields.Count; k++)
            {
                if (this.v_headerfields[k].v_column != "" &&
                    !this.v_headerfields[k].v_groupedvalue &&
                    !v_allcolumns_temp.Contains(this.v_headerfields[k].v_column))
                    v_allcolumns_temp.Add(this.v_headerfields[k].v_column);
            }

            // adicionando todas as colunas do rodapé do grupo (exceto as colunas de valor)
            for (k = 0; k < this.v_footerfields.Count; k++)
            {
                if (this.v_footerfields[k].v_column != "" &&
                    !this.v_footerfields[k].v_groupedvalue &&
                    !v_allcolumns_temp.Contains(this.v_footerfields[k].v_column))
                    v_allcolumns_temp.Add(this.v_footerfields[k].v_column);
            }

            // alocando vetor de string
            v_allcolumns = new string[v_allcolumns_temp.Count];

            // copiando nomes de colunas para o vetor de string
            for (k = 0; k < v_allcolumns_temp.Count; k++)
                v_allcolumns[k] = v_allcolumns_temp[k];

            // filtrando dados distintos pela lista de colunas, e armazenando em tabela
            if (p_parentgroupcolumn != null && p_parentgroupcolumn != "")
                p_table.DefaultView.Sort = p_parentgroupcolumn + ", " + this.v_column;
            else
                p_table.DefaultView.Sort = this.v_column;
            this.v_table = p_table.DefaultView.ToTable(true, v_allcolumns);

            // PASSO 2: PREENCHENDO VALORES

            // limpando array temporario
            v_allcolumns_temp.Clear();

            // adicionando todas as colunas do cabeçalho do grupo (somente as colunas de valor)
            for (k = 0; k < this.v_headerfields.Count; k++)
            {
                if (this.v_headerfields[k].v_column != "" &&
                    this.v_headerfields[k].v_groupedvalue &&
                    ! v_allcolumns_temp.Contains(this.v_headerfields[k].v_column))
                    v_allcolumns_temp.Add(this.v_headerfields[k].v_column);
            }

            // adicionando todas as colunas do rodapé do grupo (somente as colunas de valor)
            for (k = 0; k < this.v_footerfields.Count; k++)
            {
                if (this.v_footerfields[k].v_column != "" &&
                    this.v_footerfields[k].v_groupedvalue &&
                    ! v_allcolumns_temp.Contains(this.v_footerfields[k].v_column))
                    v_allcolumns_temp.Add(this.v_footerfields[k].v_column);
            }

            // criando colunas de valor na tabela do grupo
            for (k = 0; k < v_allcolumns_temp.Count; k++)
                this.v_table.Columns.Add(v_allcolumns_temp[k]);

            // preenchendo valores sumarizados
            for (i = 0; i < this.v_table.Rows.Count; i++)
            {
                for (j = 0; j < v_allcolumns_temp.Count; j++)
                    this.v_table.Rows[i][v_allcolumns_temp[j]] = p_table.Compute("Sum(" + v_allcolumns_temp[j] + ")", this.v_column + " = '" + this.v_table.Rows[i][this.v_column].ToString() + "'").ToString();
            }
        }
    }
}

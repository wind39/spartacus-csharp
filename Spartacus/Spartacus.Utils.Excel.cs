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
using OfficeOpenXml;

namespace Spartacus.Utils
{
    public class Excel
    {
        struct Dimension
        {
            public int v_posx;
            public int v_posy;
            public int v_totalcols;
            public int v_totalrows;
        }

        /// <summary>
        /// Conjunto de tabelas do arquivo Excel.
        /// </summary>
        public System.Data.DataSet v_set;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.Excel"/>.
        /// </summary>
        public Excel()
        {
            this.v_set = new System.Data.DataSet();
        }

        /// <summary>
        /// Limpa os dados de todas as tabelas e deleta todas as tabelas.
        /// </summary>
        public void Clear()
        {
            foreach (System.Data.DataTable v_table in this.v_set.Tables)
            {
                v_table.Clear();
                v_table.Columns.Clear();

                this.v_set.Tables.Remove(v_table);
            }

            this.v_set.Clear();
        }

        /// <summary>
        /// Importa todas as planilhas de um arquivo Excel para várias <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX a ser importado.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir ler o arquivo de origem, ou quando ocorrer qualquer problema na EPPlus.</exception>
        public void Import(string p_filename)
        {
            OfficeOpenXml.ExcelPackage v_package;
            System.IO.FileInfo v_fileinfo;

            v_fileinfo = new System.IO.FileInfo(p_filename);
            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Utils.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }

            System.Console.WriteLine("Vai começar a ler o arquivo {0}", p_filename);
            System.DateTime t = System.DateTime.Now;

            try
            {
                using (v_package = new OfficeOpenXml.ExcelPackage(v_fileinfo))
                {
                    // se o arquivo tem dados
                    if (v_package.Workbook != null && v_package.Workbook.Worksheets.Count > 0)
                    {
                        foreach (OfficeOpenXml.ExcelWorksheet v_worksheet in v_package.Workbook.Worksheets)
                        {
                            if (v_worksheet != null)
                                this.v_set.Tables.Add(this.WorksheetToDataTable(v_worksheet, t));
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Lê uma planilha do arquivo Excel e alimenta uma <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <returns>Tabela com os dados da planilha.</returns>
        /// <param name="p_worksheet">Planilha do arquivo Excel.</param>
        private System.Data.DataTable WorksheetToDataTable(OfficeOpenXml.ExcelWorksheet p_worksheet, System.DateTime t)
        {
            System.Data.DataTable v_table;
            System.Data.DataRow v_row;
            Spartacus.Utils.Excel.Dimension v_dimension;
            int i, j;

            v_table = new System.Data.DataTable(p_worksheet.Name);

            System.DateTime t0 = System.DateTime.Now;
            System.Console.WriteLine("Leu a planilha em {0} segundos", (t0-t).TotalSeconds);
            System.Console.WriteLine("Vai começar a pegar as dimensões da planilha {0}", p_worksheet.Name);

            // pegando limites dos dados
            v_dimension = this.GetWorksheetDimension(p_worksheet);

            System.DateTime t1 = System.DateTime.Now;
            System.Console.WriteLine("Pegou dimensões posx = {0}, posy = {1}, numcols = {2}, numrows = {3}", v_dimension.v_posx, v_dimension.v_posy, v_dimension.v_totalcols, v_dimension.v_totalrows);
            System.Console.WriteLine("Tempo decorrido: {0} segundos", (t1-t0).TotalSeconds);

            // lendo nomes de colunas
            for (j = v_dimension.v_posx; j <= v_dimension.v_totalcols; j++)
                v_table.Columns.Add(p_worksheet.Cells [v_dimension.v_posy, j].Value.ToString());

            // lendo dados
            for (i = v_dimension.v_posy+1; i <= v_dimension.v_totalrows; i++)
            {
                v_row = v_table.NewRow();

                for (j = v_dimension.v_posx; j <= v_dimension.v_totalcols; j++)
                {
                    if (p_worksheet.Cells [i, j].Value != null)
                        v_row [j - v_dimension.v_posx] = p_worksheet.Cells [i, j].Value.ToString();
                    else
                        v_row [j - v_dimension.v_posx] = "";
                }

                v_table.Rows.Add(v_row);
            }

            System.DateTime t2 = System.DateTime.Now;
            System.Console.WriteLine("Carregou todos os dados da planilha.");
            System.Console.WriteLine("Tempo decorrido: {0} segundos", (t2-t1).TotalSeconds);

            return v_table;
        }

        /// <summary>
        /// Verifica dimensões dos dados dentro da planilha.
        /// Considera que os dados iniciam na célula A1 da planilha.
        /// </summary>
        /// <returns>Dimensões dos dados.</returns>
        /// <param name="p_worksheet">Planilha do arquivo Excel.</param>
        private Spartacus.Utils.Excel.Dimension GetWorksheetDimension(OfficeOpenXml.ExcelWorksheet p_worksheet)
        {
            Spartacus.Utils.Excel.Dimension v_dimension;
            int i, j;

            v_dimension = new Spartacus.Utils.Excel.Dimension();

            v_dimension.v_posy = 1;
            v_dimension.v_posx = 1;
            v_dimension.v_totalcols = 0;
            v_dimension.v_totalrows = 0;

            for (i = v_dimension.v_posy; i <= 1024 * 1024; i++)
            {
                if (p_worksheet.Cells [i, 1].Value == null)
                    break;
                else
                    v_dimension.v_totalrows++;

                if (i == 1)
                {
                    for (j = v_dimension.v_posx; j <= 1024 * 1024; j++)
                    {
                        if (p_worksheet.Cells [1, j].Value == null)
                            break;
                        else
                            v_dimension.v_totalcols++;
                    }
                }
            }

            return v_dimension;
        }
    }
}

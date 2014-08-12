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
            string v_context;
            //int k;

            v_fileinfo = new System.IO.FileInfo(p_filename);
            if (! v_fileinfo.Exists)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Utils.Exception(v_context, string.Format("Arquivo {0} nao existe.", p_filename));
            }

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
                                this.v_set.Tables.Add(this.WorksheetToDataTable(v_worksheet));
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Utils.Exception(v_context, e);
            }
        }

        /// <summary>
        /// Lê uma planilha do arquivo Excel e alimenta uma <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <returns>Tabela com os dados da planilha.</returns>
        /// <param name="p_worksheet">Planilha do arquivo Excel.</param>
        private System.Data.DataTable WorksheetToDataTable(OfficeOpenXml.ExcelWorksheet p_worksheet)
        {
            System.Data.DataTable v_table;
            System.Data.DataRow v_row;
            Spartacus.Utils.Excel.Dimension v_dimension;
            int i, j;

            v_table = new System.Data.DataTable(p_worksheet.Name);

            // pegando limites dos dados
            v_dimension = this.GetWorksheetDimension(p_worksheet);

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

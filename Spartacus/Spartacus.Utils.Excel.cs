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
using System.Linq;
using OfficeOpenXml;

namespace Spartacus.Utils
{
    /// <summary>
    /// Classe Excel.
    /// Manipulação de arquivos CSV e XLSX.
    /// </summary>
    public class Excel
    {
        /// <summary>
        /// Estrutura usada para armazenar as informações da coluna.
        /// </summary>
        public struct Column
        {
            public int v_columnindex;
            public string v_columncontrol;
            public string v_columnname;

            public Column(int p_columnindex, string p_columncontrol, string p_columnname)
            {
                this.v_columnindex = p_columnindex;
                this.v_columncontrol = p_columncontrol;
                this.v_columnname = p_columnname;
            }
        }

        /// <summary>
        /// Estrutura usada para armazenar as informações da planilha.
        /// </summary>
        public class Sheet
        {
            /// <summary>
            /// Noma da planilha atual, utilizando SejExcel.
            /// </summary>
            public string v_name;

            /// <summary>
            /// Utilizado para criar planilhas Excel utilizando SejExcel.
            /// </summary>
            public System.Data.DataTable v_data;

            /// <summary>
            /// Linha atual da planilha atual, utilizando SejExcel.
            /// </summary>
            public int v_currentrow;

            /// <summary>
            /// Número de linhas fixas no template.
            /// </summary>
            public int v_fixedrows;

            /// <summary>
            /// Dicionário usado para fazer mapeamentos de colunas de uma DataTable para o modelo em XLSX.
            /// </summary>
            public System.Collections.Generic.Dictionary<int, string> v_mapping;

            /// <summary>
            /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.Excel.Sheet"/>.
            /// </summary>
            public Sheet()
            {
            }
        }

        /// <summary>
        /// Conjunto de tabelas do arquivo Excel.
        /// </summary>
        public System.Data.DataSet v_set;

        /// <summary>
        /// Lista de informações sobre planilhas usadas pela SejExcel para salvar arquivos XLSX.
        /// </summary>
        public System.Collections.ArrayList v_sheets;

        /// <summary>
        /// Objeto que gerencia eventos de progresso do processamento.
        /// </summary>
        public Spartacus.Utils.ProgressEventClass v_progress;

        /// <summary>
        /// Percentual global de progresso do processamento.
        /// </summary>
        private double v_perc;

        /// <summary>
        /// Incremento global de percentual do processamento.
        /// </summary>
        private double v_inc;

        /// <summary>
        /// Linha atual no processamento.
        /// </summary>
        private int v_currentrow;

        /// <summary>
        /// Número total de linhas do processamento.
        /// </summary>
        private int v_numtotalrows;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.Excel"/>.
        /// </summary>
        public Excel()
        {
            this.v_set = new System.Data.DataSet();
            this.v_sheets = new System.Collections.ArrayList();
            this.v_progress = new Spartacus.Utils.ProgressEventClass();
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
            }

            this.v_set.Tables.Clear();
            this.v_set.Clear();
        }

        /// <summary>
        /// Importa todas as planilhas de um arquivo Excel para várias <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX, CSV ou DBF a ser importado.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir ler o arquivo de origem, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Import(string p_filename)
        {
            System.IO.FileInfo v_fileinfo;
            Spartacus.Utils.File v_file;

            v_fileinfo = new System.IO.FileInfo(p_filename);

            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Utils.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }
            else
            {
                try
                {
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                    switch (v_file.v_extension.ToLower())
                    {
                        case "xlsx":
                            this.ImportXLSX(p_filename);
                            break;
                        case "csv":
                            this.ImportCSV(p_filename, ';', true, System.Text.Encoding.Default);
                            break;
                        case "dbf":
                            this.ImportDBF(p_filename);
                            break;
                        default:
                            throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                    }
                }
                catch (Spartacus.Utils.Exception e)
                {
                    throw new Spartacus.Utils.Exception("Erro ao converter para DataSet o arquivo {0}.", e, p_filename);
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Utils.Exception("Erro ao converter para DataSet o arquivo {0}.", e, p_filename);
                }
            }
        }

        /// <summary>
        /// Importa todas as planilhas de um arquivo Excel para várias <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo CSV a ser importado.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_header">Se deve considerar a primeira linha como cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação para leitura do arquivo CSV.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir ler o arquivo de origem, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Import(string p_filename, char p_separator, bool p_header, System.Text.Encoding p_encoding)
        {
            System.IO.FileInfo v_fileinfo;
            Spartacus.Utils.File v_file;

            v_fileinfo = new System.IO.FileInfo(p_filename);

            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Utils.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }
            else
            {
                try
                {
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                    switch (v_file.v_extension.ToLower())
                    {
                        case "xlsx":
                            this.ImportXLSX(p_filename);
                            break;
                        case "csv":
                            this.ImportCSV(p_filename, p_separator, p_header, p_encoding);
                            break;
                        case "dbf":
                            this.ImportDBF(p_filename);
                            break;
                        default:
                            throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                    }
                }
                catch (Spartacus.Utils.Exception e)
                {
                    throw new Spartacus.Utils.Exception("Erro ao converter para DataSet o arquivo {0}.", e, p_filename);
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Utils.Exception("Erro ao converter para DataSet o arquivo {0}.", e, p_filename);
                }
            }
        }

        /// <summary>
        /// Importa todas as planilhas de um arquivo Excel para várias <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo CSV a ser importado.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_delimitator">Delimitador de campos do arquivo CSV.</param>
        /// <param name="p_header">Se deve considerar a primeira linha como cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação para leitura do arquivo CSV.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir ler o arquivo de origem, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Import(string p_filename, char p_separator, char p_delimitator, bool p_header, System.Text.Encoding p_encoding)
        {
            System.IO.FileInfo v_fileinfo;
            Spartacus.Utils.File v_file;

            v_fileinfo = new System.IO.FileInfo(p_filename);

            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Utils.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }
            else
            {
                try
                {
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                    switch (v_file.v_extension.ToLower())
                    {
                        case "xlsx":
                            this.ImportXLSX(p_filename);
                            break;
                        case "csv":
                            this.ImportCSV(p_filename, p_separator, p_delimitator, p_header, p_encoding);
                            break;
                        case "dbf":
                            this.ImportDBF(p_filename);
                            break;
                        default:
                            throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                    }
                }
                catch (Spartacus.Utils.Exception e)
                {
                    throw new Spartacus.Utils.Exception("Erro ao converter para DataSet o arquivo {0}.", e, p_filename);
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Utils.Exception("Erro ao converter para DataSet o arquivo {0}.", e, p_filename);
                }
            }
        }

        /// <summary>
        /// Importa uma lista de arquivos Excel.
        /// A lista pode conter arquivos XLSX, CSV ou DBF, e pode ser misturado.
        /// </summary>
        /// <param name="p_filelist">Lista de nomes de arquivos.</param>
        public void Import(System.Collections.ArrayList p_filelist)
        {
            try
            {
                foreach (string v_filename in p_filelist)
                    this.Import(v_filename);
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Importa um arquivo CSV para um <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo CSV.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_header">Se deve considerar a primeira linha como cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação para leitura do arquivo CSV.</param>
        private void ImportCSV(string p_filename, char p_separator, bool p_header, System.Text.Encoding p_encoding)
        {
            Spartacus.Utils.File v_file;
            System.IO.StreamReader v_reader = null;
            System.Data.DataTable v_table;
            System.Data.DataRow v_row;
            string[] v_line;
            int i, j;

            try
            {
                v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);
                v_table = new System.Data.DataTable(v_file.v_name.Replace("." + v_file.v_extension, ""));

                v_reader = new System.IO.StreamReader(p_filename, p_encoding);

                i = 0;
                while (! v_reader.EndOfStream)
                {
                    v_line = v_reader.ReadLine().Split(p_separator);

                    if (i == 0)
                    {
                        if (p_header)
                        {
                            for (j = 0; j < v_line.Length; j++)
                                v_table.Columns.Add(v_line [j]);
                        }
                        else
                        {
                            for (j = 0; j < v_line.Length; j++)
                                v_table.Columns.Add("col" + j.ToString());

                            v_row = v_table.NewRow();
                            for (j = 0; j < v_table.Columns.Count; j++)
                                v_row[j] = v_line[j];
                            v_table.Rows.Add(v_row);
                        }
                    }
                    else
                    {
                        v_row = v_table.NewRow();
                        for (j = 0; j < System.Math.Min(v_table.Columns.Count, v_line.Length); j++)
                            v_row[j] = v_line[j];
                        v_table.Rows.Add(v_row);
                    }

                    i++;
                }

                this.v_set.Tables.Add(v_table);
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao carregar o arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao carregar o arquivo {0}.", e, p_filename);
            }
            finally
            {
                if (v_reader != null)
                {
                    v_reader.Close();
                    v_reader = null;
                }
            }
        }

        /// <summary>
        /// Importa um arquivo CSV para um <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo CSV.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_delimitator">Delimitador de campos do arquivo CSV.</param>
        /// <param name="p_header">Se deve considerar a primeira linha como cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação para leitura do arquivo CSV.</param>
        private void ImportCSV(string p_filename, char p_separator, char p_delimitator, bool p_header, System.Text.Encoding p_encoding)
        {
            Spartacus.Utils.File v_file;
            System.IO.StreamReader v_reader = null;
            System.Data.DataTable v_table;
            System.Data.DataRow v_row;
            string[] v_line;
            int i, j;

            try
            {
                v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);
                v_table = new System.Data.DataTable(v_file.v_name.Replace("." + v_file.v_extension, ""));

                v_reader = new System.IO.StreamReader(p_filename, p_encoding);

                i = 0;
                while (! v_reader.EndOfStream)
                {
                    v_line = v_reader.ReadLine().Split(p_separator);

                    if (i == 0)
                    {
                        if (p_header)
                        {
                            for (j = 0; j < v_line.Length; j++)
                                v_table.Columns.Add(v_line [j].Trim(p_delimitator));
                        }
                        else
                        {
                            for (j = 0; j < v_line.Length; j++)
                                v_table.Columns.Add("col" + j.ToString());

                            v_row = v_table.NewRow();
                            for (j = 0; j < v_table.Columns.Count; j++)
                                v_row[j] = v_line[j].Trim(p_delimitator);
                            v_table.Rows.Add(v_row);
                        }
                    }
                    else
                    {
                        v_row = v_table.NewRow();
                        for (j = 0; j < System.Math.Min(v_table.Columns.Count, v_line.Length); j++)
                            v_row[j] = v_line[j].Trim(p_delimitator);
                        v_table.Rows.Add(v_row);
                    }

                    i++;
                }

                this.v_set.Tables.Add(v_table);
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao carregar o arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao carregar o arquivo {0}.", e, p_filename);
            }
            finally
            {
                if (v_reader != null)
                {
                    v_reader.Close();
                    v_reader = null;
                }
            }
        }

        /// <summary>
        /// Importa um arquivo DBF para um <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo DBF.</param>
        private void ImportDBF(string p_filename)
        {
            Spartacus.Utils.File v_file;
            System.Data.DataTable v_table;
            System.Data.DataRow v_row;
            SocialExplorer.IO.FastDBF.DbfFile v_dbf;
            SocialExplorer.IO.FastDBF.DbfRecord v_record;

            try
            {
                v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);
                v_table = new System.Data.DataTable(v_file.v_name.Replace("." + v_file.v_extension, ""));

                v_dbf = new SocialExplorer.IO.FastDBF.DbfFile(System.Text.Encoding.UTF8);
                v_dbf.Open(p_filename, System.IO.FileMode.Open);

                for (int i = 0; i < v_dbf.Header.ColumnCount; i++)
                {
                    if (v_dbf.Header[i].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Binary &&
                        v_dbf.Header[i].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Memo)
                        v_table.Columns.Add(v_dbf.Header[i].Name, typeof(string));
                }

                v_record = new SocialExplorer.IO.FastDBF.DbfRecord(v_dbf.Header);
                while (v_dbf.ReadNext(v_record))
                {
                    if (! v_record.IsDeleted)
                    {
                        v_row = v_table.NewRow();
                        for (int i = 0; i < v_record.ColumnCount; i++)
                        {
                            if (v_dbf.Header[i].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Binary &&
                                v_dbf.Header[i].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Memo)
                            {
                                v_row[i] = v_record[i].Trim();
                            }
                        }
                        v_table.Rows.Add(v_row);
                    }
                }

                v_dbf.Close();

                this.v_set.Tables.Add(v_table);
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao carregar o arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao carregar o arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Importa todas as planilhas de um arquivo XLSX para várias <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX.</param>
        private void ImportXLSX(string p_filename)
        {
            Spartacus.ThirdParty.SejExcel.OoXml v_package = null;
            Spartacus.ThirdParty.SejExcel.gSheet v_sheet;

            try
            {
                v_package = new Spartacus.ThirdParty.SejExcel.OoXml(p_filename);

                if (v_package != null && v_package.sheets != null && v_package.sheets.Count > 0)
                {
                    foreach (string v_key in v_package.sheets.Keys)
                    {
                        v_sheet = v_package.sheets[v_key];
                        if (v_sheet != null)
                            this.v_set.Tables.Add(this.SheetToDataTable(v_package, v_sheet));
                    }
                }
                else
                    throw new Spartacus.Utils.Exception("Arquivo {0} nao pode ser aberto, ou nao contem planilhas com dados.", p_filename);
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao carregar o arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao carregar o arquivo {0}.", e, p_filename);
            }
            finally
            {
                if (v_package != null)
                {
                    v_package.Close();
                    v_package = null;
                }
            }
        }

        /// <summary>
        /// Converte uma planilha para uma <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <returns><see cref="System.Data.DataTable"/> com os dados contidos na planilha.</returns>
        /// <param name="p_package">Objeto de manipulação de arquivos Excel (XLSX).</param>
        /// <param name="p_sheet">Objeto de manipulação de planilhas Excel (XLSX).</param>
        /// <remarks>
        /// A primeira linha é considerada como nomes de colunas, e todas as células dessa linha devem estar preenchidas.
        /// Não pode haver linhas em branco acima, ou colunas em branco à esquerda dos dados a serem convertidos.
        /// </remarks>
        private System.Data.DataTable SheetToDataTable(Spartacus.ThirdParty.SejExcel.OoXml p_package, Spartacus.ThirdParty.SejExcel.gSheet p_sheet)
        {
            System.Data.DataTable v_table;
            System.Data.DataRow v_row = null;
            bool v_firstrow = true;
            bool v_datanode = false;
            bool v_istext = false;
            string v_cellcontent;
            double v_value;
            int v_col = -1;
            string v_columncontrol = "";
            System.Collections.ArrayList v_columnlist;

            v_table = new System.Data.DataTable(p_sheet.Name);
            v_columnlist = new System.Collections.ArrayList();

            try
            {
                using (System.Xml.XmlReader v_reader = System.Xml.XmlReader.Create(p_sheet.GetStream()))
                {
                    while (v_reader.Read())
                    {
                        switch (v_reader.NodeType)
                        {
                            case System.Xml.XmlNodeType.Element:
                                v_datanode = false;
                                switch (v_reader.Name)
                                {
                                    case "row":
                                        if (! v_firstrow)
                                        {
                                            v_row = v_table.NewRow();
                                            v_col = -1;
                                        }
                                        break;
                                    case "c":
                                        v_istext = false;
                                        while (v_reader.MoveToNextAttribute())
                                        {
                                            if (v_reader.Name == "t")
                                            {
                                                if (v_reader.Value == "s")
                                                    v_istext = true;
                                            }
                                            else
                                            {
                                                if (v_reader.Name == "r")
                                                {
                                                    if (v_reader.Value.Length > 1)
                                                    {
                                                        v_columncontrol = v_reader.Value;
                                                        v_col++;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case "v":
                                        v_datanode = true;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case System.Xml.XmlNodeType.EndElement:
                                v_datanode = false;
                                if (v_reader.Name == "row")
                                {
                                    if (v_firstrow)
                                        v_firstrow = false;
                                    else
                                        v_table.Rows.Add(v_row);
                                }
                                break;
                            case System.Xml.XmlNodeType.Text:
                                if (v_datanode)
                                {
                                    if (v_istext)
                                        v_cellcontent = p_package.words [System.Int32.Parse(v_reader.Value)];
                                    else
                                        v_cellcontent = v_reader.Value;
                                    if (v_firstrow)
                                        this.AddColumn(v_columnlist, v_table, v_col, v_columncontrol, v_cellcontent);
                                    else
                                    {
                                        if (double.TryParse(v_cellcontent, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out v_value))
                                            v_row [this.ColumnIndex(v_columnlist, v_columncontrol)] = System.Math.Round(v_value, 8).ToString();
                                        else
                                            v_row [this.ColumnIndex(v_columnlist, v_columncontrol)] = v_cellcontent;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao converter para DataTable a planilha {0}.", e, p_sheet.Name);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao converter para DataTable a planilha {0}.", e, p_sheet.Name);
            }

            return v_table;
        }

        /// <summary>
        /// Adiciona uma coluna à lista de colunas, e à <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <param name="p_columnlist">Lista de colunas.</param>
        /// <param name="p_table"><see cref="System.Data.DataTable"/>.</param>
        /// <param name="p_columnindex">Índice da coluna.</param>
        /// <param name="p_columncontrol">Controle da coluna, que é o endereço da célula no Excel.</param>
        /// <param name="p_columnname">Nome da coluna.</param>
        private void AddColumn(System.Collections.ArrayList p_columnlist, System.Data.DataTable p_table, int p_columnindex, string p_columncontrol, string p_columnname)
        {
            Spartacus.Utils.Excel.Column v_column;
            char[] v_columnarray;
            string v_columncontrol;
            int k;

            v_columnarray = p_columncontrol.ToCharArray();
            v_columncontrol = "";
            k = 0;
            while (k < v_columnarray.Length &&
                   v_columnarray[k] != '0' &&
                   v_columnarray[k] != '1' &&
                   v_columnarray[k] != '2' &&
                   v_columnarray[k] != '3' &&
                   v_columnarray[k] != '4' &&
                   v_columnarray[k] != '5' &&
                   v_columnarray[k] != '6' &&
                   v_columnarray[k] != '7' &&
                   v_columnarray[k] != '8' &&
                   v_columnarray[k] != '9')
            {
                v_columncontrol += v_columnarray [k];
                k++;
            }

            v_column = new Spartacus.Utils.Excel.Column(p_columnindex, v_columncontrol, p_columnname);
            p_columnlist.Add(v_column);

            p_table.Columns.Add(p_columnname);
        }

        /// <summary>
        /// Pega o índice da coluna baseado no seu controle.
        /// </summary>
        /// <returns>Índice da coluna.</returns>
        /// <param name="p_columnlist">Lista de colunas.</param>
        /// <param name="p_columncontrol">Controle da coluna, que é o endereço da célula no Excel..</param>
        private int ColumnIndex(System.Collections.ArrayList p_columnlist, string p_columncontrol)
        {
            char[] v_columnarray;
            string v_columncontrol;
            int k;

            v_columnarray = p_columncontrol.ToCharArray();
            v_columncontrol = "";
            k = 0;
            while (k < v_columnarray.Length &&
                   v_columnarray[k] != '0' &&
                   v_columnarray[k] != '1' &&
                   v_columnarray[k] != '2' &&
                   v_columnarray[k] != '3' &&
                   v_columnarray[k] != '4' &&
                   v_columnarray[k] != '5' &&
                   v_columnarray[k] != '6' &&
                   v_columnarray[k] != '7' &&
                   v_columnarray[k] != '8' &&
                   v_columnarray[k] != '9')
            {
                v_columncontrol += v_columnarray [k];
                k++;
            }

            k = 0;
            while (k < p_columnlist.Count)
            {
                if (((Spartacus.Utils.Excel.Column)p_columnlist [k]).v_columncontrol == v_columncontrol)
                    return ((Spartacus.Utils.Excel.Column)p_columnlist [k]).v_columnindex;
                else
                    k++;
            }

            throw new Spartacus.Utils.Exception("Controle de coluna {0} nao existe na lista de controles de colunas.", v_columncontrol);
        }

        /// <summary>
        /// Exporta todas as <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/> para um arquivo Excel.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser exportado.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir escrever no arquivo de destino, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Export(string p_filename)
        {
            Spartacus.Utils.File v_file;
            string v_markup;

            v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

            try
            {
                switch (v_file.v_extension.ToLower())
                {
                    case "xlsx":
                        v_markup = this.CreateTemplate(false, false);
                        this.ExportXLSX(p_filename, v_markup);
                        (new System.IO.FileInfo(v_markup)).Delete();
                        break;
                    case "csv":
                        this.ExportCSV(p_filename, ';', System.Text.Encoding.Default);
                        break;
                    default:
                        throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                }
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Exporta todas as <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/> para um arquivo Excel.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser exportado.</param>
        /// <param name="p_freezeheader">Se deve congelar ou não a primeira linha da planilha.</param>
        /// <param name="p_showfilter">Se deve mostrar ou não o filtro na primeira linha da planilha.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir escrever no arquivo de destino, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Export(string p_filename, bool p_freezeheader, bool p_showfilter)
        {
            Spartacus.Utils.File v_file;
            string v_markup;

            v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

            try
            {
                switch (v_file.v_extension.ToLower())
                {
                    case "xlsx":
                        v_markup = this.CreateTemplate(p_freezeheader, p_showfilter);
                        this.ExportXLSX(p_filename, v_markup);
                        (new System.IO.FileInfo(v_markup)).Delete();
                        break;
                    case "csv":
                        this.ExportCSV(p_filename, ';', System.Text.Encoding.Default);
                        break;
                    default:
                        throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                }
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Exporta todas as <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/> para um arquivo Excel.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser exportado.</param>
        /// <param name="p_templatename">Nome do arquivo XLSX a ser usado como template.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir escrever no arquivo de destino, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Export(string p_filename, string p_templatename)
        {
            Spartacus.Utils.File v_file;

            v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

            try
            {
                switch (v_file.v_extension.ToLower())
                {
                    case "xlsx":
                        this.ExportXLSX(p_filename, p_templatename);
                        break;
                    case "csv":
                        this.ExportCSV(p_filename, ';', System.Text.Encoding.Default);
                        break;
                    default:
                        throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                }
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Exporta todas as <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/> para um arquivo Excel.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser exportado.</param>
        /// <param name="p_templatename">Nome do arquivo XLSX a ser usado como template.</param>
        /// <param name="p_replacemarkup">Se deve substituir o markup do cabeçalho ou não.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir escrever no arquivo de destino, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Export(string p_filename, string p_templatename, bool p_replacemarkup)
        {
            Spartacus.Utils.File v_file;
            string v_markup;

            v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

            try
            {
                switch (v_file.v_extension.ToLower())
                {
                    case "xlsx":
                        if (p_replacemarkup)
                        {
                            v_markup = this.ReplaceMarkup(p_templatename);
                            this.ExportXLSX(p_filename, v_markup);
                            (new System.IO.FileInfo(v_markup)).Delete();
                        }
                        else
                            this.ExportXLSX(p_filename, p_templatename);
                        break;
                    case "csv":
                        this.ExportCSV(p_filename, ';', System.Text.Encoding.Default);
                        break;
                    default:
                        throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                }
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Exporta todas as <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/> para um arquivo Excel.
        /// O markup do cabeçalho sempre é substituído.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser exportado.</param>
        /// <param name="p_templatenames">Nome do arquivo XLSX a ser usado como template.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir escrever no arquivo de destino, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Export(string p_filename, System.Collections.ArrayList p_templatenames)
        {
            Spartacus.Utils.File v_file;
            string v_markup;

            v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

            try
            {
                switch (v_file.v_extension.ToLower())
                {
                    case "xlsx":
                        v_markup = this.ReplaceMarkup(p_templatenames);
                        this.ExportXLSX(p_filename, v_markup);
                        (new System.IO.FileInfo(v_markup)).Delete();
                        break;
                    case "csv":
                        this.ExportCSV(p_filename, ';', System.Text.Encoding.Default);
                        break;
                    default:
                        throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                }
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Exporta todas as <see cref="System.Data.DataTable"/> dentro de um <see cref="System.Data.DataSet"/> para um arquivo Excel.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser exportado.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_encoding">Codificação de escrita do arquivo CSV.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir escrever no arquivo de destino, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Export(string p_filename, char p_separator, System.Text.Encoding p_encoding)
        {
            Spartacus.Utils.File v_file;

            v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

            try
            {
                switch (v_file.v_extension.ToLower())
                {
                    case "xlsx":
                        this.ExportXLSX(p_filename, "template.xlsx");
                        break;
                    case "csv":
                        this.ExportCSV(p_filename, p_separator, p_encoding);
                        break;
                    default:
                        throw new Spartacus.Utils.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                }
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o DataSet no arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Exporta a primeira <see cref="System.Data.DataTable"/> de um <see cref="System.Data.DataSet"/> para um arquivo CSV.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo CSV.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_encoding">Codificação de escrita do arquivo CSV.</param>
        private void ExportCSV(string p_filename, char p_separator, System.Text.Encoding p_encoding)
        {
            System.IO.StreamWriter v_writer = null;
            string v_text;
            char v_notseparator;
            int i, k;

            if (p_separator == ',')
                v_notseparator = '.';
            else
                v_notseparator = ',';

            try
            {
                v_writer = new System.IO.StreamWriter(new System.IO.FileStream(p_filename, System.IO.FileMode.Create), p_encoding);

                this.v_progress.FireEvent("Spartacus.Utils.Excel", "ExportCSV", 0.0, "Salvando arquivo " + p_filename);

                v_text = this.v_set.Tables[0].Columns[0].ColumnName;
                for (i = 1; i < this.v_set.Tables[0].Columns.Count; i++)
                    v_text += p_separator + this.v_set.Tables[0].Columns[i].ColumnName;
                v_writer.WriteLine(v_text);

                this.v_inc = 100.0 / (double) this.v_set.Tables[0].Rows.Count;
                this.v_perc = 0.0;
                k = 0;
                foreach (System.Data.DataRow r in this.v_set.Tables[0].Rows)
                {
                    v_text = r[0].ToString();
                    for (i = 1; i < this.v_set.Tables[0].Columns.Count; i++)
                        v_text += p_separator + r[i].ToString().Replace(p_separator, v_notseparator);
                    v_writer.WriteLine(v_text);

                    this.v_perc += this.v_inc;
                    k++;
                    this.v_progress.FireEvent("Spartacus.Utils.Excel", "ExportCSV", this.v_perc, "Planilha " + this.v_set.Tables[0].TableName + ": linha " + k.ToString() + " de " + this.v_set.Tables[0].Rows.Count.ToString());
                }

                v_writer.Flush();

                this.v_progress.FireEvent("Spartacus.Utils.Excel", "ExportCSV", 100.0, "Arquivo " + p_filename + " salvo.");
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o arquivo {0}", e, p_filename);
            }
            finally
            {
                if (v_writer != null)
                {
                    v_writer.Close();
                    v_writer = null;
                }
            }
        }

        /// <summary>
        /// Exporta todas as <see cref="System.Data.DataTable"/> de um <see cref="System.Data.DataSet"/> para um arquivo XLSX.
        /// Utiliza como modelo um arquivo XLSX passado como parâmetro.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX a ser salvo.</param>
        /// <param name="p_templatename">Nome do arquivo XLSX a ser usado como template.</param>
        private void ExportXLSX(string p_filename, string p_templatename)
        {
            Spartacus.ThirdParty.SejExcel.OoXml v_package = null;
            Spartacus.ThirdParty.SejExcel.gSheet v_sheet;

            try
            {
                v_package = new Spartacus.ThirdParty.SejExcel.OoXml(p_templatename);

                if (v_package != null && v_package.sheets != null && v_package.sheets.Count > 0)
                {
                    this.v_progress.FireEvent("Spartacus.Utils.Excel", "ExportXLSX", 0.0, "Salvando arquivo " + p_filename);

                    this.v_numtotalrows = 0;
                    foreach (System.Data.DataTable v_table in this.v_set.Tables)
                        this.v_numtotalrows += v_table.Rows.Count;
                    this.v_inc = 100.0 / (double) this.v_numtotalrows;

                    this.v_perc = 0.0;
                    foreach (string v_key in v_package.sheets.Keys)
                    {
                        v_sheet = v_package.sheets[v_key];
                        if (v_sheet != null)
                            this.FillSheetWithDataTable(v_sheet, this.v_set.Tables[v_sheet.Name]);
                        else
                            throw new Spartacus.Utils.Exception("Arquivo {0} contem uma planilha invalida.", p_templatename);
                    }
                }
                else
                    throw new Spartacus.Utils.Exception("Arquivo {0} nao pode ser aberto, ou nao contem planilhas com dados.", p_templatename);

                v_package.Save(p_filename);

                this.v_progress.FireEvent("Spartacus.Utils.Excel", "ExportXLSX", 100.0, "Arquivo " + p_filename + " salvo.");
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o arquivo {0}", e, p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception("Erro ao salvar o arquivo {0}", e, p_filename);
            }
            finally
            {
                if (v_package != null)
                {
                    v_package.Close();
                    v_package = null;
                }
            }
        }

        /// <summary>
        /// Preenche a planilha do template com os dados de uma <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <param name="p_sheet">Planilha do template.</param>
        /// <param name="p_table">Dados a serem preenchidos.</param>
        private void FillSheetWithDataTable(Spartacus.ThirdParty.SejExcel.gSheet p_sheet, System.Data.DataTable p_table)
        {
            Spartacus.Utils.Excel.Sheet v_info;
            string[] v_cells;
            string v_value;
            int k;

            v_info = new Spartacus.Utils.Excel.Sheet();
            v_info.v_name = p_sheet.Name;
            v_info.v_data = p_table;
            v_info.v_fixedrows = 0;

            v_cells = p_sheet.Row(0);
            if (int.TryParse(v_cells[0], out k))
                v_cells = p_sheet.Row(k);
            else
            {
                k = 1;
                v_cells = p_sheet.Row(k);
            }

            v_info.v_mapping = new System.Collections.Generic.Dictionary<int, string>();
            if (v_cells != null)
            {
                for (int i = 0; i < v_cells.Length; i++)
                {
                    if (!string.IsNullOrEmpty(v_cells[i]) && v_cells[i].Length > 1)
                        v_value = v_cells[i].Substring(0, 1);
                    else
                        v_value = v_cells[i];
                    if (v_value == "*")
                        v_info.v_mapping[i] = v_cells[i].Replace("*", "");
                }
                if (v_info.v_mapping.Count > 0)
                    v_info.v_fixedrows = k;
            }

            v_info.v_currentrow = v_info.v_fixedrows + 1;

            this.v_sheets.Add(v_info);

            p_sheet.SetSource(OnDataRow, v_info.v_fixedrows);
        }

        /// <summary>
        /// Evento disparado quando uma nova linha é incluída.
        /// </summary>
        /// <param name="p_sheet">Planilha do template.</param>
        private void OnDataRow(Spartacus.ThirdParty.SejExcel.gSheet p_sheet)
        {
            Spartacus.Utils.Excel.Sheet v_info;
            System.Data.DataRow v_row;
            string[] v_tmp;
            int v_in_value;
            double v_re_value;
            string v_tail;

            v_info = (Spartacus.Utils.Excel.Sheet) this.v_sheets [p_sheet.Index-1];

            if ((v_info.v_currentrow - v_info.v_fixedrows - 1) < v_info.v_data.Rows.Count)
            {
                p_sheet.BeginRow(v_info.v_currentrow);
                v_row = v_info.v_data.Rows [v_info.v_currentrow - v_info.v_fixedrows - 1];
                foreach (System.Collections.Generic.KeyValuePair<int,string> v_pair in v_info.v_mapping)
                {
                    v_tmp = v_pair.Value.Split('_');
                    v_tail = v_pair.Value.Substring(3);

                    switch (v_tmp [0].ToLower())
                    {
                        case "in":
                            if (int.TryParse(v_row [v_tail].ToString(), out v_in_value))
                                p_sheet.WriteCell(v_pair.Key, v_in_value);
                            else
                                p_sheet.WriteCell(v_pair.Key, v_row [v_tail].ToString());
                            break;
                        case "re":
                            if (double.TryParse(v_row[v_tail].ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out v_re_value))
                                p_sheet.WriteCell(v_pair.Key, v_re_value);
                            else
                                p_sheet.WriteCell(v_pair.Key, v_row [v_tail].ToString());
                            break;
                        default:
                            p_sheet.WriteCell(v_pair.Key, v_row [v_tail].ToString());
                            break;
                    }
                }
                p_sheet.EndRow();

                ((Spartacus.Utils.Excel.Sheet) this.v_sheets [p_sheet.Index - 1]).v_currentrow++;

                this.v_perc += this.v_inc;
                this.v_currentrow++;
                this.v_progress.FireEvent("Spartacus.Utils.Excel", "ExportXLSX", this.v_perc, "Planilha " + ((Spartacus.Utils.Excel.Sheet) this.v_sheets [p_sheet.Index - 1]).v_name + ": linha " + this.v_currentrow.ToString() + " de " + this.v_numtotalrows.ToString());
            }
        }

        /// <summary>
        /// Substitui valores de células conforme configuração do cabeçalho, que deve estar na célula A1 e seguir formato específico.
        /// </summary>
        /// <returns>Nome do arquivo XLSX com cabeçalho aplicado.</returns>
        /// <param name="p_templatename">Nome do arquivo XLSX usado como template.</param>
        private string ReplaceMarkup(string p_templatename)
        {
            Spartacus.Net.Cryptor v_cryptor;
            System.IO.FileInfo v_src;
            System.IO.FileInfo v_dst;
            string v_dstname;
            System.Data.DataTable v_table;
            string v_imagefilename;
            string v_line;
            string[] v_options;
            int k;
            System.Drawing.Bitmap v_image;
            OfficeOpenXml.Drawing.ExcelPicture v_picture;
            int v_col, v_row;
            int v_offset;
            int v_datastart = 1;
            int v_height, v_width;

            v_cryptor = new Spartacus.Net.Cryptor("spartacus");

            v_src = new System.IO.FileInfo(p_templatename);

            using (OfficeOpenXml.ExcelPackage v_package = new OfficeOpenXml.ExcelPackage(v_src))
            {
                foreach (OfficeOpenXml.ExcelWorksheet v_worksheet in v_package.Workbook.Worksheets)
                {
                    v_table = this.v_set.Tables [v_worksheet.Name];

                    using (System.IO.StringReader v_reader = new System.IO.StringReader(v_worksheet.Cells ["A1"].Value.ToString()))
                    {
                        /* EXEMPLO DE CONFIGURACAO DE MARKUP:
                            TIPO|CAMPO|POSICAO|OPCIONAL
                            CA||A1:AD12|
                            ST|titulo|A6|
                            ST|filtro|A8|
                            ST|ano|E2:J2|
                            ST|empresa|E4:J4|
                            FO|U10/S10|V7|
                            TO|SUM(#)|M9|M12
                            TO|SUBTOTAL(9,#)|M10|M12
                            TO|SUMIF(#0,$W12=2,#1)|N10|O12;N12
                            CF|#DBE5F1|A11:AD11|
                            TA|A:AD|11|30
                            IM|imagem|0:0|80;240
                            TD|metodo,margem;qtdetotal,custototal,ajustetotal|Método,Margem,Qtde Total,Custo Total,Ajuste Total|F6
                            FC|$W2=2|#D3D3D3|A:AD
                        */

                        v_worksheet.Cells ["A1"].Value = "";

                        v_line = string.Empty;
                        k = 0;

                        do
                        {
                            v_line = v_reader.ReadLine();

                            if (v_line != null && k > 0)
                            {
                                v_options = v_line.Split('|');

                                switch (v_options[0])
                                {
                                    case "ST":
                                        v_worksheet.Cells [v_options[2]].Value = System.Net.WebUtility.HtmlDecode(v_table.Rows [0] [v_options[1]].ToString());
                                        break;
                                    case "FO":
                                        v_worksheet.Cells [v_options[2]].Formula = v_options[1];
                                        break;
                                    case "IM":
                                        try
                                        {
                                            v_imagefilename = v_cryptor.Decrypt(v_table.Rows [0] [v_options[1]].ToString());
                                        }
                                        catch (Spartacus.Net.Exception)
                                        {
                                            v_imagefilename = "";
                                        }
                                        if (v_imagefilename != "")
                                        {
                                            try
                                            {
                                                v_image = new System.Drawing.Bitmap(v_imagefilename);
                                                v_picture = null;
                                                if (v_image != null)
                                                {
                                                    v_picture = v_worksheet.Drawings.AddPicture(v_imagefilename, v_image);
                                                    v_picture.SetPosition(int.Parse(v_options[2].Split(':')[0]), int.Parse(v_options[2].Split(':')[1]));

                                                    if (v_options[3].Split(';').Length > 1)
                                                    {
                                                        v_height = int.Parse(v_options[3].Split(';')[0]);
                                                        v_width = v_height * v_image.Width / v_image.Height;
                                                        if (v_width > int.Parse(v_options[3].Split(';')[1]))
                                                        {
                                                            v_width = int.Parse(v_options[3].Split(';')[1]);
                                                            v_height = v_width * v_image.Height / v_image.Width;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        v_height = int.Parse(v_options[3].Split(';')[0]);
                                                        v_width = v_height * v_image.Width / v_image.Height;
                                                    }
                                                    v_picture.SetSize(v_width, v_height);
                                                }
                                            }
                                            catch (System.Exception)
                                            {
                                            }
                                        }
                                        break;
                                    case "TO":
                                        v_worksheet.Cells [v_options[2]].Value = "";
                                        if (v_options[3].Split(';').Length > 1)
                                        {
                                            k = 0;
                                            v_worksheet.Cells [v_options[2]].Formula = v_options[1];
                                            foreach (string v_dest in v_options[3].Split(';'))
                                            {
                                                v_row = v_worksheet.Cells[v_dest].Start.Row;
                                                v_col = v_worksheet.Cells[v_dest].Start.Column;
                                                if (v_options[1] != "")
                                                    v_worksheet.Cells [v_options[2]].Formula = v_worksheet.Cells [v_options[2]].Formula.Replace("#" + k.ToString(), v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address);
                                                else
                                                    v_worksheet.Cells [v_options[2]].Formula = "SUM(" + v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address + ")";
                                                k++;
                                            }
                                        }
                                        else
                                        {
                                            v_row = v_worksheet.Cells[v_options[3]].Start.Row;
                                            v_col = v_worksheet.Cells[v_options[3]].Start.Column;
                                            if (v_options[1] != "")
                                                v_worksheet.Cells [v_options[2]].Formula = v_options[1].Replace("#", v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address);
                                            else
                                                v_worksheet.Cells [v_options[2]].Formula = "SUM(" + v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address + ")";
                                        }
                                        break;
                                    case "TA":
                                        v_row = int.Parse(v_options[2]);
                                        v_worksheet.View.FreezePanes(v_row + 1, 1);
                                        v_worksheet.Tables.Add(v_worksheet.Cells[v_options[1].Split(':')[0] + v_options[2] + ":" + v_options[1].Split(':')[1] + (v_table.Rows.Count + v_row).ToString()], v_table.TableName);
                                        v_worksheet.Tables[0].TableStyle = OfficeOpenXml.Table.TableStyles.None;
                                        v_worksheet.Tables[0].ShowFilter = true;
                                        // passando informação para demais configurações
                                        v_datastart = int.Parse(v_options[2]);
                                        // passando informação para SejExcel
                                        v_worksheet.Cells["A1"].Value = v_options[2];
                                        break;
                                    case "TD":
                                        v_worksheet.Cells[v_options[3]].LoadFromDataTable(this.CreatePivotTable(v_table, v_options[1], v_options[2]), true, OfficeOpenXml.Table.TableStyles.Medium23);
                                        v_worksheet.Tables[v_table.TableName.Replace(' ', '_') + "_PIVOT"].ShowTotal = true;
                                        v_offset = v_options[1].Split(';')[0].Split(',').Length;
                                        for (int j = 0; j < v_options[1].Split(';')[1].Split(',').Length; j++)
                                            v_worksheet.Tables[v_table.TableName.Replace(' ', '_') + "_PIVOT"].Columns[j+v_offset].TotalsRowFunction = OfficeOpenXml.Table.RowFunctions.Sum;
                                        break;
                                    case "FC":
                                        var v_rule = v_worksheet.ConditionalFormatting.AddExpression(new OfficeOpenXml.ExcelAddress(v_options[3].Split(':')[0] + (v_datastart+1).ToString() + ":" + v_options[3].Split(':')[1] + (v_table.Rows.Count + v_datastart).ToString()));
                                        v_rule.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        v_rule.Style.Fill.BackgroundColor.Color = System.Drawing.ColorTranslator.FromHtml(v_options[2]);
                                        v_rule.Formula = v_options[1];
                                        break;
                                    default:
                                        break;
                                }
                            }

                            k++;
                        }
                        while (v_line != null);
                    }
                }

                v_dstname = v_cryptor.RandomString() + ".xlsx";
                v_dst = new System.IO.FileInfo(v_dstname);

                v_package.SaveAs(v_dst);
            }

            return v_dstname;
        }

        /// <summary>
        /// Substitui valores de células conforme configuração do cabeçalho, que deve estar na célula A1 e seguir formato específico.
        /// Cada planilha a princípio está em um arquivo diferente, e o arquivo resultante contém todas as planilhas de todos os arquivos.
        /// </summary>
        /// <returns>Nome do arquivo XLSX com cabeçalho aplicado em todas as planilhas.</returns>
        /// <param name="p_templatenames">Nome dos arquivo XLSX usados como templates.</param>
        private string ReplaceMarkup(System.Collections.ArrayList p_templatenames)
        {
            Spartacus.Net.Cryptor v_cryptor;
            System.IO.FileInfo v_src;
            System.IO.FileInfo v_dst;
            string v_dstname;
            System.Data.DataTable v_table;
            string v_imagefilename;
            string v_line;
            string[] v_options;
            int k;
            System.Drawing.Bitmap v_image;
            OfficeOpenXml.Drawing.ExcelPicture v_picture;
            int v_col, v_row;
            int v_offset;
            int v_datastart = 1;
            int v_width, v_height;

            v_cryptor = new Spartacus.Net.Cryptor("spartacus");

            v_dstname = v_cryptor.RandomString() + ".xlsx";
            v_dst = new System.IO.FileInfo(v_dstname);

            using (OfficeOpenXml.ExcelPackage v_package_dst = new OfficeOpenXml.ExcelPackage(v_dst))
            {
                for (int t = 0; t < p_templatenames.Count; t++)
                {
                    v_src = new System.IO.FileInfo((string)p_templatenames[t]);

                    using (OfficeOpenXml.ExcelPackage v_package_src = new OfficeOpenXml.ExcelPackage(v_src))
                    {
                        foreach (OfficeOpenXml.ExcelWorksheet v_worksheet_src in v_package_src.Workbook.Worksheets)
                        {
                            OfficeOpenXml.ExcelWorksheet v_worksheet = v_package_dst.Workbook.Worksheets.Add(v_worksheet_src.Name);
                            v_package_dst.Workbook.Styles.UpdateXml();

                            v_worksheet.View.ShowGridLines = v_worksheet_src.View.ShowGridLines;

                            v_table = this.v_set.Tables[v_worksheet.Name];
                            if (v_table != null && v_table.Rows.Count > 0)
                            {
                                using (System.IO.StringReader v_reader = new System.IO.StringReader(v_worksheet_src.Cells["A1"].Value.ToString()))
                                {
                                    /* EXEMPLO DE CONFIGURACAO DE MARKUP:
                                        TIPO|CAMPO|POSICAO|OPCIONAL
                                        CA||A1:AD12|
                                        ST|titulo|A6|
                                        ST|filtro|A8|
                                        ST|ano|E2:J2|
                                        ST|empresa|E4:J4|
                                        FO|U10/S10|V7|
                                        TO|SUM(#)|M9|M12
                                        TO|SUBTOTAL(9,#)|M10|M12
                                        TO|SUMIF(#0,$W12=2,#1)|N10|O12;N12
                                        CF|#DBE5F1|A11:AD11|
                                        TA|A:AD|11|30
                                        IM|imagem|0:0|80;240
                                        TD|metodo,margem;qtdetotal,custototal,ajustetotal|Método,Margem,Qtde Total,Custo Total,Ajuste Total|F6
                                        FC|$W2=2|#D3D3D3|A:AD
                                     */

                                    v_line = string.Empty;
                                    k = 0;

                                    do
                                    {
                                        v_line = v_reader.ReadLine();

                                        if (v_line != null && k > 0)
                                        {
                                            v_options = v_line.Split('|');

                                            switch (v_options[0])
                                            {
                                                case "CA":
                                                    foreach (OfficeOpenXml.ExcelRangeBase v_cell in v_worksheet_src.Cells[v_options[2]])
                                                    {
                                                        // valor
                                                        v_worksheet.Cells[v_cell.Address].Value = v_worksheet_src.Cells[v_cell.Address].Value;

                                                        // alinhamento
                                                        v_worksheet.Cells[v_cell.Address].Style.VerticalAlignment = v_worksheet_src.Cells[v_cell.Address].Style.VerticalAlignment;
                                                        v_worksheet.Cells[v_cell.Address].Style.HorizontalAlignment = v_worksheet_src.Cells[v_cell.Address].Style.HorizontalAlignment;

                                                        // bordas
                                                        v_worksheet.Cells[v_cell.Address].Style.Border.Top.Style = v_worksheet_src.Cells[v_cell.Address].Style.Border.Top.Style;
                                                        v_worksheet.Cells[v_cell.Address].Style.Border.Left.Style = v_worksheet_src.Cells[v_cell.Address].Style.Border.Left.Style;
                                                        v_worksheet.Cells[v_cell.Address].Style.Border.Right.Style = v_worksheet_src.Cells[v_cell.Address].Style.Border.Right.Style;
                                                        v_worksheet.Cells[v_cell.Address].Style.Border.Bottom.Style = v_worksheet_src.Cells[v_cell.Address].Style.Border.Bottom.Style;

                                                        // padrão de cor de fundo
                                                        v_worksheet.Cells[v_cell.Address].Style.Fill.PatternType = v_worksheet_src.Cells[v_cell.Address].Style.Fill.PatternType;

                                                        // fonte
                                                        v_worksheet.Cells[v_cell.Address].Style.Font.Bold = v_worksheet_src.Cells[v_cell.Address].Style.Font.Bold;
                                                        v_worksheet.Cells[v_cell.Address].Style.Font.Italic = v_worksheet_src.Cells[v_cell.Address].Style.Font.Italic;
                                                        v_worksheet.Cells[v_cell.Address].Style.Font.Size = v_worksheet_src.Cells[v_cell.Address].Style.Font.Size;
                                                        v_worksheet.Cells[v_cell.Address].Style.Font.Family = v_worksheet_src.Cells[v_cell.Address].Style.Font.Family;
                                                        if (v_worksheet_src.Cells[v_cell.Address].Style.Font.Color.Theme == "0")
                                                            v_worksheet.Cells[v_cell.Address].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                                        // formato numérico
                                                        v_worksheet.Cells[v_cell.Address].Style.Numberformat.Format = v_worksheet_src.Cells[v_cell.Address].Style.Numberformat.Format;

                                                        // quebrar texto automaticamente
                                                        v_worksheet.Cells[v_cell.Address].Style.WrapText = v_worksheet_src.Cells[v_cell.Address].Style.WrapText;

                                                        // comentários
                                                        if (v_worksheet_src.Cells[v_cell.Address].Comment != null)
                                                        {
                                                            v_worksheet.Cells[v_cell.Address].AddComment(v_worksheet_src.Cells[v_cell.Address].Comment.Text, v_worksheet_src.Cells[v_cell.Address].Comment.Author);
                                                            v_worksheet.Cells[v_cell.Address].Comment.AutoFit = true;
                                                        }
                                                    }
                                                    break;
                                                case "ST":
                                                    v_worksheet.Cells[v_options[2]].Value = System.Net.WebUtility.HtmlDecode(v_table.Rows[0][v_options[1]].ToString());
                                                    if (v_options[2].Contains(':'))
                                                        v_worksheet.Cells[v_options[2]].Merge = true;
                                                    break;
                                                case "FO":
                                                    v_worksheet.Cells[v_options[2]].Formula = v_options[1];
                                                    break;
                                                case "IM":
                                                    try
                                                    {
                                                        v_imagefilename = v_cryptor.Decrypt(v_table.Rows[0][v_options[1]].ToString());
                                                    }
                                                    catch (Spartacus.Net.Exception)
                                                    {
                                                        v_imagefilename = "";
                                                    }
                                                    if (v_imagefilename != "")
                                                    {
                                                        try
                                                        {
                                                            v_image = new System.Drawing.Bitmap(v_imagefilename);
                                                            v_picture = null;
                                                            if (v_image != null)
                                                            {
                                                                v_picture = v_worksheet.Drawings.AddPicture(v_imagefilename, v_image);
                                                                v_picture.SetPosition(int.Parse(v_options[2].Split(':')[0]), int.Parse(v_options[2].Split(':')[1]));

                                                                if (v_options[3].Split(';').Length > 1)
                                                                {
                                                                    v_height = int.Parse(v_options[3].Split(';')[0]);
                                                                    v_width = v_height * v_image.Width / v_image.Height;
                                                                    if (v_width > int.Parse(v_options[3].Split(';')[1]))
                                                                    {
                                                                        v_width = int.Parse(v_options[3].Split(';')[1]);
                                                                        v_height = v_width * v_image.Height / v_image.Width;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    v_height = int.Parse(v_options[3].Split(';')[0]);
                                                                    v_width = v_height * v_image.Width / v_image.Height;
                                                                }
                                                                v_picture.SetSize(v_width, v_height);
                                                            }
                                                        }
                                                        catch (System.Exception)
                                                        {
                                                        }
                                                    }
                                                    break;
                                                case "TO":
                                                    v_worksheet.Cells [v_options[2]].Value = "";
                                                    if (v_options[3].Split(';').Length > 1)
                                                    {
                                                        k = 0;
                                                        v_worksheet.Cells [v_options[2]].Formula = v_options[1];
                                                        foreach (string v_dest in v_options[3].Split(';'))
                                                        {
                                                            v_row = v_worksheet.Cells[v_dest].Start.Row;
                                                            v_col = v_worksheet.Cells[v_dest].Start.Column;
                                                            if (v_options[1] != "")
                                                                v_worksheet.Cells [v_options[2]].Formula = v_worksheet.Cells [v_options[2]].Formula.Replace("#" + k.ToString(), v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address);
                                                            else
                                                                v_worksheet.Cells [v_options[2]].Formula = "SUM(" + v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address + ")";
                                                            k++;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        v_row = v_worksheet.Cells[v_options[3]].Start.Row;
                                                        v_col = v_worksheet.Cells[v_options[3]].Start.Column;
                                                        if (v_options[1] != "")
                                                            v_worksheet.Cells [v_options[2]].Formula = v_options[1].Replace("#", v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address);
                                                        else
                                                            v_worksheet.Cells [v_options[2]].Formula = "SUM(" + v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address + ")";
                                                    }
                                                    break;
                                                case "CF":
                                                    v_worksheet.Cells[v_options[2]].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(v_options[1]));
                                                    break;
                                                case "TA":
                                                    v_row = int.Parse(v_options[2]);
                                                    for (int i = 1; i <= v_row; i++)
                                                        v_worksheet.Row(i).Height = v_worksheet_src.Row(i).Height;
                                                    v_col = int.Parse(v_options[3]);
                                                    for (int j = 1; j <= v_col; j++)
                                                        v_worksheet.Column(j).Width = v_worksheet_src.Column(j).Width;
                                                    v_worksheet.View.FreezePanes(v_row + 1, 1);
                                                    v_worksheet.Tables.Add(v_worksheet.Cells[v_options[1].Split(':')[0] + v_options[2] + ":" + v_options[1].Split(':')[1] + (v_table.Rows.Count + v_row).ToString()], v_worksheet_src.Name);
                                                    v_worksheet.Tables[0].TableStyle = OfficeOpenXml.Table.TableStyles.None;
                                                    v_worksheet.Tables[0].ShowFilter = true;
                                                    // passando informação para demais configurações
                                                    v_datastart = int.Parse(v_options[2]);
                                                    // passando informação para SejExcel
                                                    v_worksheet.Cells["A1"].Value = v_options[2];
                                                    break;
                                                case "TD":
                                                    v_worksheet.Cells[v_options[3]].LoadFromDataTable(this.CreatePivotTable(v_table, v_options[1], v_options[2]), true, OfficeOpenXml.Table.TableStyles.Medium23);
                                                    v_worksheet.Tables[v_table.TableName.Replace(' ', '_') + "_PIVOT"].ShowTotal = true;
                                                    v_offset = v_options[1].Split(';')[0].Split(',').Length;
                                                    for (int j = 0; j < v_options[1].Split(';')[1].Split(',').Length; j++)
                                                        v_worksheet.Tables[v_table.TableName.Replace(' ', '_') + "_PIVOT"].Columns[j + v_offset].TotalsRowFunction = OfficeOpenXml.Table.RowFunctions.Sum;
                                                    break;
                                                case "FC":
                                                    var v_rule = v_worksheet.ConditionalFormatting.AddExpression(new OfficeOpenXml.ExcelAddress(v_options[3].Split(':')[0] + (v_datastart + 1).ToString() + ":" + v_options[3].Split(':')[1] + (v_table.Rows.Count + v_datastart).ToString()));
                                                    v_rule.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                                    v_rule.Style.Fill.BackgroundColor.Color = System.Drawing.ColorTranslator.FromHtml(v_options[2]);
                                                    v_rule.Formula = v_options[1];
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        k++;
                                    }
                                    while (v_line != null);
                                }
                            }
                        }
                    }
                }

                v_package_dst.Save();
            }

            return v_dstname;
        }

        /// <summary>
        /// Baseado em um DataTable preenchido, cria um XLSX para ser usado como template.
        /// </summary>
        /// <returns>Nome do arquivo a ser usado como template.</returns>
        /// <param name="p_freezeheader">Se deve congelar ou não a primeira linha da planilha.</param>
        /// <param name="p_showfilter">Se deve mostrar ou não o filtro na primeira linha da planilha.</param>
        private string CreateTemplate(bool p_freezeheader, bool p_showfilter)
        {
            Spartacus.Net.Cryptor v_cryptor;
            System.IO.FileInfo v_dst;
            string v_dstname;
            string v_column;
            string v_prefix;

            v_cryptor = new Spartacus.Net.Cryptor("spartacus");

            v_dstname = v_cryptor.RandomString() + ".xlsx";
            v_dst = new System.IO.FileInfo(v_dstname);

            using (OfficeOpenXml.ExcelPackage v_package = new OfficeOpenXml.ExcelPackage(v_dst))
            {
                foreach (System.Data.DataTable v_table in this.v_set.Tables)
                {
                    OfficeOpenXml.ExcelWorksheet v_worksheet = v_package.Workbook.Worksheets.Add(v_table.TableName);

                    v_worksheet.View.ShowGridLines = true;

                    for (int k = 1; k <= v_table.Columns.Count; k++)
                    {
                        v_column = v_table.Columns[k-1].ColumnName.ToUpper();

                        if (v_column.Contains("_RE_"))
                            v_prefix = "*RE_";
                        else
                        {
                            if (v_column.Contains("_IN_"))
                                v_prefix = "*IN_";
                            else
                                v_prefix = "*ST_";
                        }

                        v_worksheet.Cells[1, k].Value = v_column;
                        v_worksheet.Cells[2, k].Value = v_prefix + v_column;
                    }

                    if (p_freezeheader)
                        v_worksheet.View.FreezePanes(2, 1);

                    if (p_showfilter)
                    {
                        v_worksheet.Tables.Add(v_worksheet.Cells["A1:" + OfficeOpenXml.ExcelCellBase.GetAddress(v_table.Rows.Count + 1, v_table.Columns.Count)], v_table.TableName);
                        v_worksheet.Tables[0].TableStyle = OfficeOpenXml.Table.TableStyles.None;
                        v_worksheet.Tables[0].ShowFilter = true;
                    }
                }

                v_package.Save();
            }

            return v_dstname;
        }

        /// <summary>
        /// Cria uma tabela dinâmica.
        /// </summary>
        /// <returns>Tabela dinâmica.</returns>
        /// <param name="p_table">Tabela original.</param>
        /// <param name="p_origcolumns">
        ///   Nomes originais das colunas, separados por vírgula.
        ///   Nomes de colunas de texto vem à esquerda, separadas dos nomes de colunas de valor por um ponto-e-vírgula.
        /// </param>
        /// <param name="p_fakecolumns">Nomes fantasia das colunas, separados por vírgula.</param>
        private System.Data.DataTable CreatePivotTable(System.Data.DataTable p_table, string p_origcolumns, string p_fakecolumns)
        {
            System.Data.DataTable v_pivot, v_table;
            string[] v_origtextcolumns;
            string[] v_origdatacolumns;
            string[] v_fakecolumns;
            string v_where;

            v_origtextcolumns = p_origcolumns.Split(';')[0].Split(',');
            v_origdatacolumns = p_origcolumns.Split(';')[1].Split(',');
            v_fakecolumns = p_fakecolumns.Split(',');

            // tratando colunas de valor
            v_table = p_table.Clone();
            for (int k = 0; k < v_origdatacolumns.Length; k++)
                v_table.Columns[v_origdatacolumns[k]].DataType = typeof(double);
            foreach (System.Data.DataRow v_row in p_table.Rows)
                v_table.ImportRow(v_row);

            // criando tabela dinamica
            v_pivot = p_table.DefaultView.ToTable(true, v_origtextcolumns);
            v_pivot.TableName = v_table.TableName.Replace(' ', '_') + "_PIVOT";

            // adicionando colunas de valor
            for (int k = 0; k < v_origdatacolumns.Length; k++)
                v_pivot.Columns.Add(v_origdatacolumns[k], typeof(double));

            // calculando valores sumarizados
            for (int i = 0; i < v_pivot.Rows.Count; i++)
            {
                v_where = "1 = 1";
                for (int k = 0; k < v_origtextcolumns.Length; k++)
                    v_where += " AND " + v_origtextcolumns[k] + " = '" + v_pivot.Rows[i][v_origtextcolumns[k]].ToString() + "'";

                for (int j = 0; j < v_origdatacolumns.Length; j++)
                    v_pivot.Rows[i][v_origdatacolumns[j]] = (double) v_table.Compute("Sum(" + v_origdatacolumns[j] + ")", v_where);
            }

            // renomeando colunas
            for (int k = 0; k < v_fakecolumns.Length; k++)
                v_pivot.Columns[k].ColumnName = v_fakecolumns[k];

            return v_pivot;
        }
    }
}

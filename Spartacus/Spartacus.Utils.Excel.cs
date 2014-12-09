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
            /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.Excel+Sheet"/>.
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
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.Excel"/>.
        /// </summary>
        public Excel()
        {
            this.v_set = new System.Data.DataSet();
            this.v_sheets = new System.Collections.ArrayList();
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
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser importado.</param>
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
                            this.ImportCSV(p_filename, ';', false, System.Text.Encoding.Default);
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
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser importado.</param>
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
        /// A lista pode conter arquivos XLSX ou CSV, e pode ser misturado.
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
                        if (v_line.Length != v_table.Columns.Count)
                        {
                            throw new Spartacus.Utils.Exception("Linha {0} contem {1} colunas, diferente do esperado.", i, v_line.Length);
                        }
                        else
                        {
                            v_row = v_table.NewRow();
                            for (j = 0; j < v_table.Columns.Count; j++)
                                v_row[j] = v_line[j];
                            v_table.Rows.Add(v_row);
                        }
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
        /// <remarks>No caso de arquivo CSV ou XLSX com template padrão, apenas a primeira tabela do DataSet será salva.</remarks>
        public void Export(string p_filename)
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
        /// <remarks>No caso de arquivo CSV, apenas a primeira tabela do DataSet será salva.</remarks>
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
        /// <param name="p_markupname">Se deve substituir o markup do cabeçalho ou não.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir escrever no arquivo de destino, ou quando ocorrer qualquer problema na SejExcel.</exception>
        /// <remarks>No caso de arquivo CSV, apenas a primeira tabela do DataSet será salva.</remarks>
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
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XLSX ou CSV a ser exportado.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_encoding">Codificação de escrita do arquivo CSV.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir escrever no arquivo de destino, ou quando ocorrer qualquer problema na SejExcel.</exception>
        /// <remarks>No caso de arquivo CSV, apenas a primeira tabela do DataSet será salva.</remarks>
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
            int i;

            if (p_separator == ',')
                v_notseparator = '.';
            else
                v_notseparator = ',';

            try
            {
                v_writer = new System.IO.StreamWriter(new System.IO.FileStream(p_filename, System.IO.FileMode.Create), p_encoding);

                v_text = this.v_set.Tables[0].Columns[0].ColumnName;
                for (i = 1; i < this.v_set.Tables[0].Columns.Count; i++)
                    v_text += p_separator + this.v_set.Tables[0].Columns[i].ColumnName;
                v_writer.WriteLine(v_text);

                foreach (System.Data.DataRow r in this.v_set.Tables[0].Rows)
                {
                    v_text = r[0].ToString();
                    for (i = 1; i < this.v_set.Tables[0].Columns.Count; i++)
                        v_text += p_separator + r[i].ToString().Replace(p_separator, v_notseparator);
                    v_writer.WriteLine(v_text);
                }

                v_writer.Flush();
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

                System.DateTime v_mid = System.DateTime.Now;
                v_package.Save(p_filename);

                System.DateTime v_fin = System.DateTime.Now;
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

            v_info.v_mapping = new System.Collections.Generic.Dictionary<int, string>();
            k = 0;
            do
            {
                v_cells = p_sheet.Row(k);
                if (v_cells != null)
                {
                    for (int i = 0; i < v_cells.Length; i++)
                    {
                        if (! string.IsNullOrEmpty(v_cells [i]) && v_cells [i].Length > 1)
                            v_value = v_cells [i].Substring(0, 1);
                        else
                            v_value = v_cells [i];
                        if (v_value == "*")
                            v_info.v_mapping [i] = v_cells [i].Replace("*", "");
                    }
                    if (v_info.v_mapping.Count > 0)
                        v_info.v_fixedrows = k;
                }
                k++;
            }
            while (v_cells != null && v_info.v_mapping.Count == 0);

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
                            if (double.TryParse(v_row [v_tail].ToString().Replace(",", "."), out v_re_value))
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
            }
        }

        /// <summary>
        /// Substitui valores de células conforme configuração do cabeçalho, que deve estar na célula A1 e seguir formato específico.
        /// </summary>
        /// <returns>Nome do arquivo XLSX com cabeçalho aplicado.</returns>
        /// <param name="p_templatename">Nome do arquivo XLSX usado como template.</param>
        private string ReplaceMarkup(string p_templatename)
        {
            Spartacus.Utils.File v_file;
            System.IO.FileInfo v_src;
            Spartacus.Net.Cryptor v_cryptor;
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

            v_cryptor = new Spartacus.Net.Cryptor("spartacus");

            v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_templatename);
            v_src = new System.IO.FileInfo(p_templatename);

            using (OfficeOpenXml.ExcelPackage v_package = new OfficeOpenXml.ExcelPackage(v_src))
            {
                foreach (OfficeOpenXml.ExcelWorksheet v_worksheet in v_package.Workbook.Worksheets)
                {
                    v_table = this.v_set.Tables [v_worksheet.Name];

                    using (System.IO.StringReader v_reader = new System.IO.StringReader(v_worksheet.Cells ["A1"].Value.ToString()))
                    {
                        // EXEMPLO DE CONFIGURACAO DE MARKUP:
                        //TIPO;CAMPO;POSICAO;OPCIONAL
                        //ST;titulo;A6;
                        //ST;ano;E2:J2;
                        //ST;empresa;E4:J4;
                        //IM;imagem;0:0;80
                        //TO;;L9;L11
                        //TO;;M9;M11
                        //TO;;N9;N11
                        //TO;;O9;O11
                        //TO;;P9;P11
                        //TO;;Q9;Q11
                        //TO;;R9;R11
                        //TO;;S9;S11
                        //TO;;T9;T11
                        //TO;;U9;U11
                        //TO;;V9;V11
                        //TO;;W9;W11

                        v_worksheet.Cells ["A1"].Value = "";

                        v_line = string.Empty;
                        k = 0;

                        do
                        {
                            v_line = v_reader.ReadLine();

                            if (v_line != null && k > 0)
                            {
                                v_options = v_line.Split(';');

                                switch (v_options[0])
                                {
                                    case "ST":
                                        v_worksheet.Cells [v_options[2]].Value = v_table.Rows [0] [v_options[1]].ToString();
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
                                            v_image = new System.Drawing.Bitmap(v_imagefilename);
                                            v_picture = null;
                                            if (v_image != null)
                                            {
                                                v_picture = v_worksheet.Drawings.AddPicture(v_imagefilename, v_image);
                                                v_picture.SetPosition(int.Parse(v_options[2].Split(':')[0]), int.Parse(v_options[2].Split(':')[1]));
                                                v_picture.SetSize(int.Parse(v_options[3]) * v_image.Width / v_image.Height, int.Parse(v_options[3]));
                                            }
                                        }
                                        break;
                                    case "TO":
                                        v_worksheet.Cells [v_options[2]].Value = "";
                                        v_row = v_worksheet.Cells[v_options[3]].Start.Row;
                                        v_col = v_worksheet.Cells[v_options[3]].Start.Column;
                                        v_worksheet.Cells [v_options[2]].Formula = "SUM(" + v_worksheet.Cells [v_row, v_col].Address + ":" + v_worksheet.Cells [v_table.Rows.Count + v_row - 1, v_col].Address + ")";
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

                v_dstname = v_cryptor.Encrypt(v_file.GetBaseNameNoExt()).Replace("/", "").Replace("=", "").Replace("+", "") + ".xlsx";
                v_dst = new System.IO.FileInfo(v_dstname);

                v_package.SaveAs(v_dst);
            }

            return v_dstname;
        }
    }
}

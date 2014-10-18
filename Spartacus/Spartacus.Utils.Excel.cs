using System;

//TODO: Mesclar classe CSVTable na classe Excel.

namespace Spartacus.Utils
{
    /// <summary>
    /// Classe Excel.
    /// Manipulação de arquivos CSV e XLSX.s
    /// </summary>
    public class Excel
    {
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
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir ler o arquivo de origem, ou quando ocorrer qualquer problema na SejExcel.</exception>
        public void Import(string p_filename)
        {
            Spartacus.ThirdyParty.SejExcel.OoXml v_package;
            Spartacus.ThirdyParty.SejExcel.gSheet v_sheet;
            System.IO.FileInfo v_fileinfo;

            v_fileinfo = new System.IO.FileInfo(p_filename);
            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Utils.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }

            try
            {
                v_package = new Spartacus.ThirdyParty.SejExcel.OoXml(p_filename);
                
                if (v_package != null && v_package.sheets != null && v_package.sheets.Count > 0)
                {
                    foreach (string v_key in v_package.sheets.Keys)
                    {
                        v_sheet = v_package.sheets[v_key];
                        if (v_sheet != null)
                            this.v_set.Tables.Add(this.SheetToDataTable(v_package, v_sheet));
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        private System.Data.DataTable SheetToDataTable(Spartacus.ThirdyParty.SejExcel.OoXml p_package, Spartacus.ThirdyParty.SejExcel.gSheet p_sheet)
        {
            System.Data.DataTable v_table;
            System.Data.DataRow v_row = null;
            bool v_firstrow = true;
            bool v_datanode = false;
            bool v_istext = false;
            string v_cellcontent;
            int v_col = -1;

            v_table = new System.Data.DataTable(p_sheet.Name);

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
                                                    v_col++;
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
                                    v_table.Columns.Add(v_cellcontent);
                                else
                                    v_row [v_col] = v_cellcontent;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return v_table;
        }
    }
}

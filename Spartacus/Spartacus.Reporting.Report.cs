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
using PDFjet;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Classe Report.
    /// Representa um relatório em PDF.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Código do Relatório.
        /// </summary>
        public int v_reportid;

        /// <summary>
        /// Objeto para comunicação com o banco de dados.
        /// </summary>
        private Spartacus.Database.Generic v_database;

        /// <summary>
        /// Comando SQL que pode possuir parâmetros entre #.
        /// </summary>
        public Spartacus.Database.Command v_cmd;

        /// <summary>
        /// Tabela com os dados temporários do relatório.
        /// </summary>
        public System.Data.DataTable v_tabletemp;

        /// <summary>
        /// Tabela com os dados do relatório, após tratar tipos de colunas.
        /// </summary>
        public System.Data.DataTable v_table;

        /// <summary>
        /// Configurações globais e de dados do relatório.
        /// </summary>
        public Spartacus.Reporting.Settings v_settings;

        /// <summary>
        /// Cabeçalho do relatório, mostrado em todas as páginas.
        /// </summary>
        public Spartacus.Reporting.Block v_header;

        /// <summary>
        /// Rodapé do relatório, mostrado em todas as páginas.
        /// </summary>
        public Spartacus.Reporting.Block v_footer;

        /// <summary>
        /// Lista de campos do relatório.
        /// </summary>
        public System.Collections.ArrayList v_fields;

        /// <summary>
        /// Lista de grupos do relatório.
        /// </summary>
        public System.Collections.ArrayList v_groups;

        /// <summary>
        /// Objeto usado para auxiliar renderização de texto.
        /// </summary>
        private System.Drawing.Graphics v_graphics;

        /// <summary>
        /// Se o gerador de relatórios deve calcular os valores agrupados.
        /// </summary>
        private bool v_calculate_groups;

        /// <summary>
        /// Objeto que gerencia eventos de progresso do processamento.
        /// </summary>
        public Spartacus.Utils.ProgressEventClass v_progress;

        /// <summary>
        /// Percentual de progresso do processamento.
        /// </summary>
        public double v_perc;

        /// <summary>
        /// Faixa de percentual de progresso do processamento usada pelo relatório atual.
        /// Se não for pacote de relatórios, é igual a 100.
        /// </summary>
        public double v_percstep;

        /// <summary>
        /// Valor do percentual a ser atualizado após renderizar a última página.
        /// Se não for pacote de relatórios, é igual a 100.
        /// </summary>
        public double v_lastperc;

        /// <summary>
        /// Incremento de percentual;
        /// </summary>
        private double v_inc;

        /// <summary>
        /// Número de linhas renderizadas até o momento.
        /// </summary>
        private int v_renderedrows;

        /// <summary>
        /// Número de linhas do detalhe.
        /// </summary>
        private int v_numrowsdetail;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_reportid">Código do Relatório.</param>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        public Report(int p_reportid, string p_filename)
        {
            this.v_reportid = p_reportid;

            this.v_graphics = (new System.Windows.Forms.Form()).CreateGraphics();

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.ArrayList();
            this.v_groups = new System.Collections.ArrayList();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = null;

            this.v_calculate_groups = false;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_filename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_reportid">Código do Relatório.</param>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_database">Objeto para conexão com o banco de dados.</param>
        public Report(int p_reportid, string p_filename, Spartacus.Database.Generic p_database)
        {
            this.v_reportid = p_reportid;

            this.v_graphics = (new System.Windows.Forms.Form()).CreateGraphics();

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.ArrayList();
            this.v_groups = new System.Collections.ArrayList();

            this.v_database = p_database;
            this.v_tabletemp = null;
            this.v_table = null;

            this.v_calculate_groups = false;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_filename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_reportid">Código do Relatório.</param>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_table">Tabela com os dados.</param>
        public Report(int p_reportid, string p_filename, System.Data.DataTable p_table)
        {
            this.v_reportid = p_reportid;

            this.v_graphics = (new System.Windows.Forms.Form()).CreateGraphics();

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.ArrayList();
            this.v_groups = new System.Collections.ArrayList();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = p_table;

            this.v_calculate_groups = false;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_filename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_reportid">Código do Relatório.</param>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        public Report(int p_reportid, string p_filename, bool p_calculate_groups)
        {
            this.v_reportid = p_reportid;

            this.v_graphics = (new System.Windows.Forms.Form()).CreateGraphics();

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.ArrayList();
            this.v_groups = new System.Collections.ArrayList();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_filename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_reportid">Código do Relatório.</param>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_database">Objeto para conexão com o banco de dados.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        public Report(int p_reportid, string p_filename, Spartacus.Database.Generic p_database, bool p_calculate_groups)
        {
            this.v_reportid = p_reportid;

            this.v_graphics = (new System.Windows.Forms.Form()).CreateGraphics();

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.ArrayList();
            this.v_groups = new System.Collections.ArrayList();

            this.v_database = p_database;
            this.v_tabletemp = null;
            this.v_table = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_filename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_reportid">Código do Relatório.</param>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_table">Tabela com os dados.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        public Report(int p_reportid, string p_filename, System.Data.DataTable p_table, bool p_calculate_groups)
        {
            this.v_reportid = p_reportid;

            this.v_graphics = (new System.Windows.Forms.Form()).CreateGraphics();

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.ArrayList();
            this.v_groups = new System.Collections.ArrayList();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_filename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Lê o arquivo XML que define o relatório.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        private void ReadXml(string p_filename)
        {
            System.Xml.XmlReader v_reader, v_item;
            System.Xml.XmlReaderSettings v_settings;

            v_settings = new System.Xml.XmlReaderSettings();
            v_settings.IgnoreComments = true;
            v_settings.ConformanceLevel = System.Xml.ConformanceLevel.Document;

            try
            {
                v_reader = System.Xml.XmlReader.Create(p_filename, v_settings);

                while (v_reader.Read())
                {
                    if (v_reader.IsStartElement())
                    {
                        switch(v_reader.Name)
                        {
                            case "connection":
                                v_item = v_reader.ReadSubtree();
                                this.ReadConnection(v_item);
                                v_item.Close();
                                break;
                            case "settings":
                                v_item = v_reader.ReadSubtree();
                                this.ReadSettings(v_item);
                                v_item.Close();
                                break;
                            case "command":
                                v_item = v_reader.ReadSubtree();
                                this.ReadCommand(v_item);
                                v_item.Close();
                                break;
                            case "header":
                                v_item = v_reader.ReadSubtree();
                                this.ReadHeader(v_item);
                                v_item.Close();
                                break;
                            case "footer":
                                v_item = v_reader.ReadSubtree();
                                this.ReadFooter(v_item);
                                v_item.Close();
                                break;
                            case "fields":
                                v_item = v_reader.ReadSubtree();
                                this.ReadFields(v_item);
                                v_item.Close();
                                break;
                            case "groups":
                                v_item = v_reader.ReadSubtree();
                                this.ReadGroups(v_item);
                                v_item.Close();
                                break;
                            default:
                                break;
                        }
                    }
                }

                v_reader.Close();
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Lê informações sobre a conexão.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadConnection(System.Xml.XmlReader p_reader)
        {
            string v_type = null;
            string v_provider = null;
            string v_host = null;
            string v_port = null;
            string v_service = null;
            string v_user = null;
            string v_password = null;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "type":
                            v_type = p_reader.ReadString();
                            break;
                        case "provider":
                            v_provider = p_reader.ReadString();
                            break;
                        case "host":
                            v_host = p_reader.ReadString();
                            break;
                        case "port":
                            v_port = p_reader.ReadString();
                            break;
                        case "service":
                            v_service = p_reader.ReadString();
                            break;
                        case "user":
                            v_user = p_reader.ReadString();
                            break;
                        case "password":
                            v_password = p_reader.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }

            switch (v_type)
            {
                case "odbc":
                    // instanciando uma nova conexão com banco de dados via ODBC
                    this.v_database = new Spartacus.Database.Odbc(
                        v_service,
                        v_user,
                        v_password
                    );
                    break;
                case "oledb":
                    // instanciando uma nova conexão com banco de dados via ODBC
                    this.v_database = new Spartacus.Database.Oledb(
                        v_provider,
                        v_host,
                        v_port,
                        v_service,
                        v_user,
                        v_password
                    );
                    break;
                case "mysql":
                    // instanciando uma nova conexao com banco de dados via MySQL
                    this.v_database = new Spartacus.Database.Mysql(
                        v_host,
                        v_port,
                        v_service,
                        v_user,
                        v_password
                    );
                    break;
                case "firebird":
                    // instanciando uma nova conexao com banco de dados via FirebirdSQL
                    this.v_database = new Spartacus.Database.Firebird(
                        v_host,
                        v_port,
                        v_service,
                        v_user,
                        v_password
                    );
                    break;
                case "sqlite":
                    // instanciando uma nova conexao com banco de dados via SQLite
                    this.v_database = new Spartacus.Database.Sqlite(
                        v_service
                    );
                    break;
                case "postgresql":
                    // instanciando uma nova conexao com banco de dados via PostgreSQL
                    this.v_database = new Spartacus.Database.Postgresql(
                        v_host,
                        v_port,
                        v_service,
                        v_user,
                        v_password
                    );
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Lê informações sobre as configurações do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadSettings(System.Xml.XmlReader p_reader)
        {
            this.v_settings = new Spartacus.Reporting.Settings();
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "layout":
                            if (p_reader.ReadString() == "PORTRAIT")
                                this.v_settings.v_layout = Spartacus.Reporting.PageLayout.PORTRAIT;
                            else
                                this.v_settings.v_layout = Spartacus.Reporting.PageLayout.LANDSCAPE;
                            break;
                        case "topmargin":
                            this.v_settings.SetMargin(Spartacus.Reporting.PageMargin.TOP, p_reader.ReadString());
                            break;
                        case "bottommargin":
                            this.v_settings.SetMargin(Spartacus.Reporting.PageMargin.BOTTOM, p_reader.ReadString());
                            break;
                        case "leftmargin":
                            this.v_settings.SetMargin(Spartacus.Reporting.PageMargin.LEFT, p_reader.ReadString());
                            break;
                        case "rightmargin":
                            this.v_settings.SetMargin(Spartacus.Reporting.PageMargin.RIGHT, p_reader.ReadString());
                            break;
                        case "dataheaderborder":
                            this.v_settings.v_dataheaderborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "datafieldborder":
                            this.v_settings.v_datafieldborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "groupheaderborder":
                            this.v_settings.v_groupheaderborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "groupfooterborder":
                            this.v_settings.v_groupfooterborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "reportheaderborder":
                            this.v_settings.v_reportheaderborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "reportfooterborder":
                            this.v_settings.v_reportfooterborder = new Spartacus.Reporting.Border(p_reader.ReadString());
                            break;
                        case "dataheadercolor":
                            this.v_settings.v_dataheadercolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "datafieldevencolor":
                            this.v_settings.v_datafieldevencolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "datafieldoddcolor":
                            this.v_settings.v_datafieldoddcolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "groupheadercolor":
                            this.v_settings.v_groupheadercolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "groupfootercolor":
                            this.v_settings.v_groupfootercolor = this.v_settings.GetColor(p_reader.ReadString());
                            break;
                        case "reportheaderfont":
                            this.v_settings.v_reportheaderfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_reportheaderfont, v_item);
                            v_item.Close();
                            break;
                        case "reportfooterfont":
                            this.v_settings.v_reportfooterfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_reportfooterfont, v_item);
                            v_item.Close();
                            break;
                        case "dataheaderfont":
                            this.v_settings.v_dataheaderfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_dataheaderfont, v_item);
                            v_item.Close();
                            break;
                        case "datafieldfont":
                            this.v_settings.v_datafieldfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_datafieldfont, v_item);
                            v_item.Close();
                            break;
                        case "groupheaderfont":
                            this.v_settings.v_groupheaderfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_groupheaderfont, v_item);
                            v_item.Close();
                            break;
                        case "groupfooterfont":
                            this.v_settings.v_groupfooterfont = new Spartacus.Reporting.Font();
                            v_item = p_reader.ReadSubtree();
                            this.ReadFont(this.v_settings.v_groupfooterfont, v_item);
                            v_item.Close();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Lê fonte.
        /// </summary>
        /// <param name="p_font">Objeto fonte, onde vai guardar as informações.</param>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadFont(Spartacus.Reporting.Font p_font, System.Xml.XmlReader p_reader)
        {
            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "family":
                            p_font.SetFamily(p_reader.ReadString());
                            break;
                        case "size":
                            p_font.SetSize(p_reader.ReadString());
                            break;
                        case "bold":
                            if (p_reader.ReadString() == "TRUE")
                                p_font.v_bold = true;
                            else
                                p_font.v_bold = false;
                            break;
                        case "italic":
                            if (p_reader.ReadString() == "TRUE")
                                p_font.v_italic = true;
                            else
                                p_font.v_italic = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Lê comando.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadCommand(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            this.v_cmd = new Spartacus.Database.Command();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "sql":
                            this.v_cmd.v_text = p_reader.ReadString();
                            break;
                        case "parameter":
                            v_item = p_reader.ReadSubtree();
                            this.ReadParameter(v_item);
                            v_item.Close();
                            break;
                            default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Lê parâmetro.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadParameter(System.Xml.XmlReader p_reader)
        {
            string v_name = null;
            Spartacus.Database.Type v_type = Spartacus.Database.Type.STRING;
            Spartacus.Database.Locale v_locale = Spartacus.Database.Locale.EUROPEAN;
            string v_dateformat = null;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "name":
                            v_name = p_reader.ReadString();
                            break;
                        case "type":
                            switch(p_reader.ReadString())
                            {
                                case "INTEGER":
                                    v_type = Spartacus.Database.Type.INTEGER;
                                    break;
                                case "REAL":
                                    v_type = Spartacus.Database.Type.REAL;
                                    break;
                                case "BOOLEAN":
                                    v_type = Spartacus.Database.Type.BOOLEAN;
                                    break;
                                case "CHAR":
                                    v_type = Spartacus.Database.Type.CHAR;
                                    break;
                                case "DATE":
                                    v_type = Spartacus.Database.Type.DATE;
                                    break;
                                case "STRING":
                                    v_type = Spartacus.Database.Type.STRING;
                                    break;
                                case "UNDEFINED":
                                    v_type = Spartacus.Database.Type.UNDEFINED;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "locale":
                            switch(p_reader.ReadString())
                            {
                                case "EUROPEAN":
                                    v_locale = Spartacus.Database.Locale.EUROPEAN;
                                    break;
                                case "AMERICAN":
                                    v_locale = Spartacus.Database.Locale.AMERICAN;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "dateformat":
                            v_dateformat = p_reader.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_cmd.AddParameter(v_name, v_type);
            if (v_type == Spartacus.Database.Type.REAL)
                this.v_cmd.SetLocale(v_name, v_locale);
            else
                if (v_type == Spartacus.Database.Type.DATE)
                    this.v_cmd.SetDateFormat(v_name, v_dateformat);
        }

        /// <summary>
        /// Lê cabeçalho do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadHeader(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "height":
                            this.v_header.SetHeight(p_reader.ReadString());
                            break;
                        case "object":
                            v_item = p_reader.ReadSubtree();
                            this.ReadHeaderObject(v_item);
                            v_item.Close();
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_header.v_border = this.v_settings.v_reportheaderborder;
        }

        /// <summary>
        /// Lê objeto do cabeçalho do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadHeaderObject(System.Xml.XmlReader p_reader)
        {
            Spartacus.Reporting.Object v_object;

            v_object = new Spartacus.Reporting.Object();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "type":
                            switch (p_reader.ReadString())
                            {
                                case "IMAGE":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.IMAGE;
                                    break;
                                case "TEXT":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.TEXT;
                                    break;
                                case "PAGENUMBER":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.PAGENUMBER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "column":
                            v_object.v_column = p_reader.ReadString();
                            break;
                        case "posx":
                            v_object.SetPosX(p_reader.ReadString());
                            break;
                        case "posy":
                            v_object.SetPosY(p_reader.ReadString());
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_header.v_objects.Add(v_object);
        }

        /// <summary>
        /// Lê rodapé do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadFooter(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "height":
                            this.v_footer.SetHeight(p_reader.ReadString());
                            break;
                        case "object":
                            v_item = p_reader.ReadSubtree();
                            this.ReadFooterObject(v_item);
                            v_item.Close();
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_footer.v_border = this.v_settings.v_reportfooterborder;
        }

        /// <summary>
        /// Lê objeto do rodapé do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadFooterObject(System.Xml.XmlReader p_reader)
        {
            Spartacus.Reporting.Object v_object;

            v_object = new Spartacus.Reporting.Object();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "type":
                            switch (p_reader.ReadString())
                            {
                                case "IMAGE":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.IMAGE;
                                    break;
                                case "TEXT":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.TEXT;
                                    break;
                                case "PAGENUMBER":
                                    v_object.v_type = Spartacus.Reporting.ObjectType.PAGENUMBER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "column":
                            v_object.v_column = p_reader.ReadString();
                            break;
                        case "posx":
                            v_object.SetPosX(p_reader.ReadString());
                            break;
                        case "posy":
                            v_object.SetPosY(p_reader.ReadString());
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_object.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_footer.v_objects.Add(v_object);
        }

        /// <summary>
        /// Lê campos do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadFields(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            this.v_numrowsdetail = 1;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement() && p_reader.Name == "field")
                {
                    v_item = p_reader.ReadSubtree();
                    this.ReadField(v_item);
                    v_item.Close();
                }
            }
        }

        /// <summary>
        /// Lê um único campo do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadField(System.Xml.XmlReader p_reader)
        {
            Spartacus.Reporting.Field v_field;

            v_field = new Spartacus.Reporting.Field();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "title":
                            v_field.v_title = p_reader.ReadString();
                            break;
                        case "column":
                            v_field.v_column = p_reader.ReadString();
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "fill":
                            v_field.v_fill = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "type":
                            v_field.SetType(p_reader.ReadString());
                            break;
                        case "row":
                            v_field.v_row = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        default:
                            break;
                    }
                }
            }

            if ((v_field.v_row + 1) > this.v_numrowsdetail)
                this.v_numrowsdetail = v_field.v_row + 1;

            this.v_fields.Add(v_field);
        }

        /// <summary>
        /// Lê grupos do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadGroups(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement() && p_reader.Name == "group")
                {
                    v_item = p_reader.ReadSubtree();
                    this.ReadGroup(v_item);
                    v_item.Close();
                }
            }
        }

        /// <summary>
        /// Lê um único grupo do relatório.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        private void ReadGroup(System.Xml.XmlReader p_reader)
        {
            System.Xml.XmlReader v_item;
            Spartacus.Reporting.Group v_group;

            v_group = new Spartacus.Reporting.Group();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "level":
                            v_group.v_level = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "column":
                            v_group.v_column = p_reader.ReadString();
                            break;
                        case "sort":
                            v_group.v_sort = p_reader.ReadString();
                            break;
                        case "showheader":
                            if (p_reader.ReadString() == "FALSE")
                                v_group.v_showheader = false;
                            else
                                v_group.v_showheader = true;
                            break;
                        case "showfooter":
                            if (p_reader.ReadString() == "FALSE")
                                v_group.v_showfooter = false;
                            else
                                v_group.v_showfooter = true;
                            break;
                        case "headerfields":
                            v_item = p_reader.ReadSubtree();
                            this.ReadGroupHeaderFields(v_item, v_group);
                            v_item.Close();
                            break;
                        case "footerfields":
                            v_item = p_reader.ReadSubtree();
                            this.ReadGroupFooterFields(v_item, v_group);
                            v_item.Close();
                            break;
                        default:
                            break;
                    }
                }
            }

            this.v_groups.Add(v_group);
        }

        /// <summary>
        /// Lê campos de um cabeçalho de grupo.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        /// <param name="p_group">Grupo do relatório.</param>
        private void ReadGroupHeaderFields(System.Xml.XmlReader p_reader, Spartacus.Reporting.Group p_group)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement() && p_reader.Name == "headerfield")
                {
                    v_item = p_reader.ReadSubtree();
                    this.ReadGroupHeaderField(v_item, p_group);
                    v_item.Close();
                }
            }
        }

        /// <summary>
        /// Lê um único campo de um cabeçalho de grupo.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        /// <param name="p_group">Grupo do relatório.</param>
        private void ReadGroupHeaderField(System.Xml.XmlReader p_reader, Spartacus.Reporting.Group p_group)
        {
            Spartacus.Reporting.Field v_field;

            v_field = new Spartacus.Reporting.Field();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "title":
                            v_field.v_title = p_reader.ReadString();
                            break;
                        case "column":
                            v_field.v_column = p_reader.ReadString();
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "fill":
                            v_field.v_fill = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "type":
                            v_field.SetType(p_reader.ReadString());
                            break;
                        case "groupedvalue":
                            if (p_reader.ReadString() == "FALSE")
                                v_field.v_groupedvalue = false;
                            else
                                v_field.v_groupedvalue = true;
                            break;
                        case "row":
                            v_field.v_row = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        default:
                            break;
                    }
                }
            }

            if ((v_field.v_row + 1) > p_group.v_numrowsheader)
                p_group.v_numrowsheader = v_field.v_row + 1;

            p_group.v_headerfields.Add(v_field);
        }

        /// <summary>
        /// Lê campos de um rodapé de grupo.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        /// <param name="p_group">Grupo do relatório.</param>
        private void ReadGroupFooterFields(System.Xml.XmlReader p_reader, Spartacus.Reporting.Group p_group)
        {
            System.Xml.XmlReader v_item;

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement() && p_reader.Name == "footerfield")
                {
                    v_item = p_reader.ReadSubtree();
                    this.ReadGroupFooterField(v_item, p_group);
                    v_item.Close();
                }
            }
        }

        /// <summary>
        /// Lê um único campo de rodapé de grupo.
        /// </summary>
        /// <param name="p_reader">Objeto XML.</param>
        /// <param name="p_group">Grupo do relatório.</param>
        private void ReadGroupFooterField(System.Xml.XmlReader p_reader, Spartacus.Reporting.Group p_group)
        {
            Spartacus.Reporting.Field v_field;

            v_field = new Spartacus.Reporting.Field();

            while (p_reader.Read())
            {
                if (p_reader.IsStartElement())
                {
                    switch (p_reader.Name)
                    {
                        case "title":
                            v_field.v_title = p_reader.ReadString();
                            break;
                        case "column":
                            v_field.v_column = p_reader.ReadString();
                            break;
                        case "align":
                            switch (p_reader.ReadString())
                            {
                                case "LEFT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
                                    break;
                                case "RIGHT":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.RIGHT;
                                    break;
                                case "CENTER":
                                    v_field.v_align = Spartacus.Reporting.FieldAlignment.CENTER;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "fill":
                            v_field.v_fill = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        case "type":
                            v_field.SetType(p_reader.ReadString());
                            break;
                        case "groupedvalue":
                            if (p_reader.ReadString() == "FALSE")
                                v_field.v_groupedvalue = false;
                            else
                                v_field.v_groupedvalue = true;
                            break;
                        case "row":
                            v_field.v_row = System.Convert.ToInt32(p_reader.ReadString());
                            break;
                        default:
                            break;
                    }
                }
            }

            if ((v_field.v_row + 1) > p_group.v_numrowsfooter)
                p_group.v_numrowsfooter = v_field.v_row + 1;

            p_group.v_footerfields.Add(v_field);
        }

        /// <summary>
        /// Executa o relatório.
        /// Se o relatório não possui tabela pré-definida, utiliza o comando SQL do relatório para buscar os dados no banco.
        /// Em seguida gera tabelas auxiliares para todos os grupos.
        /// </summary>
        public void Execute()
        {
            int k;

            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Executando relatorio " + this.v_reportid.ToString());
            this.v_perc = 0;
            this.v_percstep = 100.0;
            this.v_lastperc = 100.0;

            if (this.v_database != null && this.v_tabletemp == null)
            {
                this.v_cmd.UpdateText();

                try
                {
                    // buscando dados do banco
                    this.v_tabletemp = this.v_database.Query(this.v_cmd.v_text, "RESULTS");

                    // SE O GERADOR DE RELATORIOS DEVE CALCULAR OS VALORES DOS GRUPOS
                    if (this.v_calculate_groups)
                    {
                        // tratando colunas de valor
                        this.v_table = this.v_tabletemp.Clone();
                        for (k = 0; k < this.v_fields.Count; k++)
                        {
                            switch (((Spartacus.Reporting.Field)this.v_fields[k]).v_type)
                            {
                                case Spartacus.Database.Type.REAL:
                                    this.v_table.Columns[((Spartacus.Reporting.Field)this.v_fields[k]).v_column].DataType = typeof(double);
                                    break;
                                case Spartacus.Database.Type.INTEGER:
                                    this.v_table.Columns[((Spartacus.Reporting.Field)this.v_fields[k]).v_column].DataType = typeof(int);
                                    break;
                                default:
                                    break;
                            }
                        }
                        foreach (System.Data.DataRow v_row in this.v_tabletemp.Rows)
                            this.v_table.ImportRow(v_row);

                        // gerando tabelas auxiliares para todos os grupos
                        if (this.v_table != null && this.v_table.Rows.Count > 0 && this.v_groups.Count > 0)
                        {
                            for (k = 0; k < this.v_groups.Count; k++)
                                ((Spartacus.Reporting.Group)this.v_groups [k]).BuildCalculate(this.v_table);
                        }
                    }
                    else
                    {
                        this.v_table = this.v_tabletemp;

                        // gerando tabelas auxiliares para todos os grupos
                        if (this.v_table != null && this.v_table.Rows.Count > 0 && this.v_groups.Count > 0)
                        {
                            for (k = 0; k < this.v_groups.Count; k++)
                                ((Spartacus.Reporting.Group)this.v_groups [k]).Build(this.v_table);
                        }
                    }
                }
                catch (Spartacus.Database.Exception e)
                {
                    throw new Spartacus.Reporting.Exception("Erro ao buscar os dados do relatório.", e);
                }
            }
            else
            {
                // SE O GERADOR DE RELATORIOS DEVE CALCULAR OS VALORES DOS GRUPOS
                if (this.v_calculate_groups)
                {
                    // tratando colunas de valor
                    this.v_table = this.v_tabletemp.Clone();
                    for (k = 0; k < this.v_fields.Count; k++)
                    {
                        switch (((Spartacus.Reporting.Field)this.v_fields[k]).v_type)
                        {
                            case Spartacus.Database.Type.REAL:
                                this.v_table.Columns[((Spartacus.Reporting.Field)this.v_fields[k]).v_column].DataType = typeof(double);
                                break;
                            case Spartacus.Database.Type.INTEGER:
                                this.v_table.Columns[((Spartacus.Reporting.Field)this.v_fields[k]).v_column].DataType = typeof(int);
                                break;
                            default:
                                break;
                        }
                    }
                    foreach (System.Data.DataRow v_row in this.v_tabletemp.Rows)
                        this.v_table.ImportRow(v_row);

                    // gerando tabelas auxiliares para todos os grupos
                    if (this.v_table != null && this.v_table.Rows.Count > 0 && this.v_groups.Count > 0)
                    {
                        for (k = 0; k < this.v_groups.Count; k++)
                            ((Spartacus.Reporting.Group)this.v_groups [k]).BuildCalculate(this.v_table);
                    }
                }
                else
                {
                    this.v_table = this.v_tabletemp;

                    // gerando tabelas auxiliares para todos os grupos
                    if (this.v_table != null && this.v_table.Rows.Count > 0 && this.v_groups.Count > 0)
                    {
                        for (k = 0; k < this.v_groups.Count; k++)
                            ((Spartacus.Reporting.Group)this.v_groups [k]).Build(this.v_table);
                    }
                }
            }
        }

        /// <summary>
        /// Salva como PDF.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo PDF.</param>
        public void Save(string p_filename)
        {
            PDFjet.NET.PDF v_pdf;
            System.IO.BufferedStream v_buffer;
            System.IO.FileStream f;
            PDFjet.NET.Table v_dataheadertable, v_datatable;
            float[] v_layout;
            PDFjet.NET.Page v_page;
            System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_rendered;
            int v_numpages, v_currentpage;

            // se o relatório não tiver dados, não faz nada
            if (this.v_table.Rows.Count == 0)
                return;

            try
            {
                this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", this.v_perc, "Renderizando relatorio " + this.v_reportid.ToString() + " no arquivo " + p_filename);
                this.v_inc = this.v_percstep / (double) this.v_table.Rows.Count;
                this.v_renderedrows = 0;

                f = new System.IO.FileStream(p_filename, System.IO.FileMode.Create);
                v_buffer = new System.IO.BufferedStream(f);

                v_pdf = new PDFjet.NET.PDF(v_buffer);

                if (this.v_settings.v_layout == Spartacus.Reporting.PageLayout.LANDSCAPE)
                    v_layout = PDFjet.NET.A4.LANDSCAPE;
                else
                    v_layout = PDFjet.NET.A4.PORTRAIT;

                v_page = new PDFjet.NET.Page(v_pdf, v_layout);

                // tabela de cabecalho de dados

                v_dataheadertable = new PDFjet.NET.Table();
                v_dataheadertable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height);

                v_rendered = this.RenderDataHeader(
                    v_page.GetHeight(),
                    v_page.GetWidth(),
                    this.v_settings.v_dataheaderfont.GetFont(v_pdf)
                );

                v_dataheadertable.SetData(v_rendered, PDFjet.NET.Table.DATA_HAS_0_HEADER_ROWS);
                v_dataheadertable.SetCellBordersWidth(0.8f);

                // tabela de dados

                v_datatable = new PDFjet.NET.Table();
                v_datatable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height + ((this.v_settings.v_dataheaderfont.v_size + 2) * 1.8 * this.v_numrowsdetail));
                v_datatable.SetBottomMargin(this.v_settings.v_bottommargin + this.v_footer.v_height);

                v_rendered = this.RenderData(
                    v_page.GetHeight(),
                    v_page.GetWidth(),
                    this.v_settings.v_datafieldfont.GetFont(v_pdf),
                    this.v_settings.v_groupheaderfont.GetFont(v_pdf),
                    this.v_settings.v_groupfooterfont.GetFont(v_pdf)
                );

                v_datatable.SetData(v_rendered, PDFjet.NET.Table.DATA_HAS_0_HEADER_ROWS);
                v_datatable.SetCellBordersWidth(0.8f);

                this.v_header.SetValues(this.v_table);
                this.v_footer.SetValues(this.v_table);

                v_numpages = v_datatable.GetNumberOfPages(v_page);
                v_currentpage = 1;
                while (v_datatable.HasMoreData())
                {
                    this.v_header.SetPageNumber(v_currentpage, v_numpages);
                    this.v_footer.SetPageNumber(v_currentpage, v_numpages);

                    this.v_header.Render(
                        this.v_settings.v_reportheaderfont,
                        this.v_settings.v_leftmargin,
                        this.v_settings.v_topmargin,
                        this.v_settings.v_rightmargin,
                        v_pdf,
                        v_page
                    );

                    v_dataheadertable.DrawOn(v_page);
                    v_datatable.DrawOn(v_page);

                    this.v_footer.Render(
                        this.v_settings.v_reportfooterfont,
                        this.v_settings.v_leftmargin,
                        v_page.GetHeight() - v_settings.v_bottommargin - v_footer.v_height,
                        this.v_settings.v_rightmargin,
                        v_pdf,
                        v_page
                    );

                    if (v_datatable.HasMoreData())
                    {
                        v_dataheadertable.ResetRenderedPagesCount();

                        v_page = new PDFjet.NET.Page(v_pdf, v_layout);
                        v_currentpage++;
                    }
                }

                v_pdf.Flush();
                v_buffer.Close();

                this.v_perc = this.v_lastperc;
                this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", this.v_perc, "Relatorio " + this.v_reportid.ToString() + " renderizado no arquivo " + p_filename);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o arquivo PDF de saída.", e);
            }
        }

        /// <summary>
        /// Salva como PDF parcialmente.
        /// Usado para renderização de pacotes de arquivos PDF.
        /// </summary>
        /// <param name="p_pdf">Objeto PDF aberto.</param>
        public void SavePartial(PDFjet.NET.PDF p_pdf)
        {
            PDFjet.NET.Table v_dataheadertable, v_datatable;
            float[] v_layout;
            PDFjet.NET.Page v_page;
            System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_rendered;
            int v_numpages, v_currentpage;

            // se o relatório não tiver dados, não faz nada
            if (this.v_table.Rows.Count == 0)
                return;

            try
            {
                this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", this.v_perc, "Renderizando o relatorio " + this.v_reportid.ToString());
                this.v_inc = this.v_percstep / (double) this.v_table.Rows.Count;
                this.v_renderedrows = 0;

                if (this.v_settings.v_layout == Spartacus.Reporting.PageLayout.LANDSCAPE)
                    v_layout = PDFjet.NET.A4.LANDSCAPE;
                else
                    v_layout = PDFjet.NET.A4.PORTRAIT;

                v_page = new PDFjet.NET.Page(p_pdf, v_layout);

                // tabela de cabecalho de dados

                v_dataheadertable = new PDFjet.NET.Table();
                v_dataheadertable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height);

                v_rendered = this.RenderDataHeader(
                    v_page.GetHeight(),
                    v_page.GetWidth(),
                    this.v_settings.v_dataheaderfont.GetFont(p_pdf)
                );

                v_dataheadertable.SetData(v_rendered, PDFjet.NET.Table.DATA_HAS_0_HEADER_ROWS);
                v_dataheadertable.SetCellBordersWidth(0.8f);

                // tabela de dados

                v_datatable = new PDFjet.NET.Table();
                v_datatable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height + ((this.v_settings.v_dataheaderfont.v_size + 2) * 1.8 * this.v_numrowsdetail));
                v_datatable.SetBottomMargin(this.v_settings.v_bottommargin + this.v_footer.v_height);

                v_rendered = this.RenderData(
                    v_page.GetHeight(),
                    v_page.GetWidth(),
                    this.v_settings.v_datafieldfont.GetFont(p_pdf),
                    this.v_settings.v_groupheaderfont.GetFont(p_pdf),
                    this.v_settings.v_groupfooterfont.GetFont(p_pdf)
                );

                v_datatable.SetData(v_rendered, PDFjet.NET.Table.DATA_HAS_0_HEADER_ROWS);
                v_datatable.SetCellBordersWidth(0.8f);

                this.v_header.SetValues(this.v_table);
                this.v_footer.SetValues(this.v_table);

                v_numpages = v_datatable.GetNumberOfPages(v_page);
                v_currentpage = 1;
                while (v_datatable.HasMoreData())
                {
                    this.v_header.SetPageNumber(v_currentpage, v_numpages);
                    this.v_footer.SetPageNumber(v_currentpage, v_numpages);

                    this.v_header.Render(
                        this.v_settings.v_reportheaderfont,
                        this.v_settings.v_leftmargin,
                        this.v_settings.v_topmargin,
                        this.v_settings.v_rightmargin,
                        p_pdf,
                        v_page
                    );

                    v_dataheadertable.DrawOn(v_page);
                    v_datatable.DrawOn(v_page);

                    this.v_footer.Render(
                        this.v_settings.v_reportfooterfont,
                        this.v_settings.v_leftmargin,
                        v_page.GetHeight() - v_settings.v_bottommargin - v_footer.v_height,
                        this.v_settings.v_rightmargin,
                        p_pdf,
                        v_page
                    );

                    if (v_datatable.HasMoreData())
                    {
                        v_dataheadertable.ResetRenderedPagesCount();

                        v_page = new PDFjet.NET.Page(p_pdf, v_layout);
                        v_currentpage++;
                    }
                }

                this.v_perc = this.v_lastperc;
                this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", this.v_perc, "Relatorio " + this.v_reportid.ToString() + " renderizado.");
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o arquivo PDF de saída.", e);
            }
        }

        /// <summary>
        /// Renderiza cabeçalho de dados.
        /// </summary>
        /// <returns>Matriz representando o cabeçalho de dados.</returns>
        /// <param name="p_pageheight">Altura da página.</param>
        /// <param name="p_pagewidth">Largura da página.</param>
        /// <param name="p_dataheaderfont">Fonte do cabeçalho de dados.</param>
        private System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> RenderDataHeader(
            float p_pageheight,
            float p_pagewidth,
            PDFjet.NET.Font p_dataheaderfont
        )
        {
            System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_data;
            System.Collections.Generic.List<PDFjet.NET.Cell> v_row;
            PDFjet.NET.Cell v_cell;
            int k, v_sectionrow;

            v_data = new System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>>();

            // renderizando cabecalho da pagina
            for (v_sectionrow = 0; v_sectionrow < this.v_numrowsdetail; v_sectionrow++)
            {
                v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                for (k = 0; k < this.v_fields.Count; k++)
                {
                    if (((Spartacus.Reporting.Field)this.v_fields[k]).v_row == v_sectionrow)
                    {
                        v_cell = new PDFjet.NET.Cell(p_dataheaderfont);
                        v_cell.SetText(((Spartacus.Reporting.Field)this.v_fields[k]).v_title);
                        v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)this.v_fields[k]).v_fill) / 100);
                        switch (((Spartacus.Reporting.Field)this.v_fields[k]).v_align)
                        {
                            case Spartacus.Reporting.FieldAlignment.LEFT:
                                v_cell.SetTextAlignment(PDFjet.NET.Align.LEFT);
                                break;
                            case Spartacus.Reporting.FieldAlignment.RIGHT:
                                v_cell.SetTextAlignment(PDFjet.NET.Align.RIGHT);
                                break;
                            case Spartacus.Reporting.FieldAlignment.CENTER:
                                v_cell.SetTextAlignment(PDFjet.NET.Align.CENTER);
                                break;
                            default:
                                break;
                        }
                        v_cell.SetBgColor(this.v_settings.v_dataheadercolor);
                        v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_dataheaderborder.v_top);
                        v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_dataheaderborder.v_bottom);
                        v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_dataheaderborder.v_left);
                        v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_dataheaderborder.v_right);
                        v_row.Add(v_cell);
                    }
                }
                v_data.Add(v_row);
            }

            return v_data;
        }

        /// <summary>
        /// Renderiza matriz de dados.
        /// </summary>
        /// <returns>Matriz de dados.</returns>
        /// <param name="p_pageheight">Altura da página.</param>
        /// <param name="p_pagewidth">Largura da página.</param>
        /// <param name="p_datafieldfont">Fonte do campo de dados.</param>
        /// <param name="p_groupheaderfont">Fonte do cabeçalho de grupo.</param>
        /// <param name="p_groupfooterfont">Fonte do rodapé de grupo.</param>
        private System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> RenderData(
            float p_pageheight,
            float p_pagewidth,
            PDFjet.NET.Font p_datafieldfont,
            PDFjet.NET.Font p_groupheaderfont,
            PDFjet.NET.Font p_groupfooterfont
        )
        {
            System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_data;
            System.Collections.Generic.List<PDFjet.NET.Cell> v_row;
            PDFjet.NET.Cell v_cell;
            string v_text;
            int k, r, v_sectionrow;

            v_data = new System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>>();

            // se o relatorio possui grupos
            if (this.v_groups.Count > 0)
            {
                this.RenderGroup(
                    this.v_groups.Count - 1,
                    v_data,
                    p_pageheight,
                    p_pagewidth,
                    p_datafieldfont,
                    p_groupheaderfont,
                    p_groupfooterfont
                );
            }
            else // se o relatorio nao possui grupos
            {
                r = 0;
                foreach (System.Data.DataRow rb in this.v_table.Rows)
                {
                    for (v_sectionrow = 0; v_sectionrow < this.v_numrowsdetail; v_sectionrow++)
                    {
                        v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                        for (k = 0; k < this.v_fields.Count; k++)
                        {
                            if (((Spartacus.Reporting.Field)this.v_fields[k]).v_row == v_sectionrow)
                            {
                                v_cell = new PDFjet.NET.Cell(p_datafieldfont);
                                v_text = ((Spartacus.Reporting.Field)this.v_fields[k]).Format(rb[((Spartacus.Reporting.Field)this.v_fields[k]).v_column].ToString());
                                v_cell.SetText(Spartacus.Reporting.Field.Crop(v_text, this.v_graphics, this.v_settings.v_datafieldfont.v_nativefont, ((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)this.v_fields[k]).v_fill) / 100));
                                v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)this.v_fields[k]).v_fill) / 100);
                                switch (((Spartacus.Reporting.Field)this.v_fields[k]).v_align)
                                {
                                    case Spartacus.Reporting.FieldAlignment.LEFT:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.LEFT);
                                        break;
                                    case Spartacus.Reporting.FieldAlignment.RIGHT:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.RIGHT);
                                        break;
                                    case Spartacus.Reporting.FieldAlignment.CENTER:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.CENTER);
                                        break;
                                    default:
                                        break;
                                }
                                if (r % 2 == 0)
                                    v_cell.SetBgColor(this.v_settings.v_datafieldevencolor);
                                else
                                    v_cell.SetBgColor(this.v_settings.v_datafieldoddcolor);
                                v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_datafieldborder.v_top);
                                v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_datafieldborder.v_bottom);
                                v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_datafieldborder.v_left);
                                v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_datafieldborder.v_right);
                                v_row.Add(v_cell);
                            }
                        }
                        v_data.Add(v_row);
                    }
                    r++;

                    this.v_perc += v_inc;
                    this.v_renderedrows++;
                    this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", this.v_perc, "Relatorio " + this.v_reportid.ToString() + ": linha " + this.v_renderedrows.ToString() + " de " + this.v_table.Rows.Count.ToString());
                }
            }

            return v_data;
        }

        /// <summary>
        /// Renderiza um grupo.
        /// </summary>
        /// <param name="p_level">Nível do grupo atual.</param>
        /// <param name="p_data">Matriz de dados.</param>
        /// <param name="p_pageheight">Altura da página.</param>
        /// <param name="p_pagewidth">Largura da página.</param>
        /// <param name="p_datafieldfont">Fonte do campo de dados.</param>
        /// <param name="p_groupheaderfont">Fonte do cabeçalho de grupo.</param>
        /// <param name="p_groupfooterfont">Fonte do rodapé de grupo.</param>
        private void RenderGroup(
            int p_level,
            System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> p_data,
            float p_pageheight,
            float p_pagewidth,
            PDFjet.NET.Font p_datafieldfont,
            PDFjet.NET.Font p_groupheaderfont,
            PDFjet.NET.Font p_groupfooterfont
        )
        {
            Spartacus.Reporting.Group v_group;
            System.Collections.Generic.List<PDFjet.NET.Cell> v_row;
            PDFjet.NET.Cell v_cell;
            string v_text;
            int k, r, v_sectionrow;

            v_group = (Spartacus.Reporting.Group)this.v_groups [p_level];

            // para cada elemento do grupo
            foreach (System.Data.DataRow rg in v_group.v_table.Rows)
            {
                // renderizando campos do cabecalho
                if (v_group.v_showheader)
                {
                    for (v_sectionrow = 0; v_sectionrow < v_group.v_numrowsheader; v_sectionrow++)
                    {
                        v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                        for (k = 0; k < v_group.v_headerfields.Count; k++)
                        {
                            if (((Spartacus.Reporting.Field)v_group.v_headerfields[k]).v_row == v_sectionrow)
                            {
                                v_cell = new PDFjet.NET.Cell(p_groupheaderfont);
                                if (((Spartacus.Reporting.Field)v_group.v_headerfields[k]).v_column != "")
                                    v_text = ((Spartacus.Reporting.Field)v_group.v_headerfields[k]).Format(rg[((Spartacus.Reporting.Field)v_group.v_headerfields[k]).v_column].ToString());
                                else
                                    v_text = "";
                                v_cell.SetText(Spartacus.Reporting.Field.Crop(v_text, this.v_graphics, this.v_settings.v_groupheaderfont.v_nativefont, ((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)v_group.v_headerfields[k]).v_fill) / 100));
                                v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)v_group.v_headerfields[k]).v_fill) / 100);
                                switch (((Spartacus.Reporting.Field)v_group.v_headerfields[k]).v_align)
                                {
                                    case Spartacus.Reporting.FieldAlignment.LEFT:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.LEFT);
                                        break;
                                    case Spartacus.Reporting.FieldAlignment.RIGHT:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.RIGHT);
                                        break;
                                    case Spartacus.Reporting.FieldAlignment.CENTER:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.CENTER);
                                        break;
                                    default:
                                        break;
                                }
                                v_cell.SetBgColor(this.v_settings.v_groupheadercolor);
                                v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_groupheaderborder.v_top);
                                v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_groupheaderborder.v_bottom);
                                v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_groupheaderborder.v_left);
                                v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_groupheaderborder.v_right);
                                v_row.Add(v_cell);
                            }
                        }
                        p_data.Add(v_row);
                    }
                }

                if (v_group.v_level == 0)
                {
                    // renderizando dados do grupo
                    r = 0;
                    foreach (System.Data.DataRow rb in this.v_table.Select(v_group.v_column + " = '" + rg[v_group.v_column] + "'", v_group.v_sort))
                    {
                        for (v_sectionrow = 0; v_sectionrow < this.v_numrowsdetail; v_sectionrow++)
                        {
                            v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                            for (k = 0; k < this.v_fields.Count; k++)
                            {
                                if (((Spartacus.Reporting.Field)this.v_fields[k]).v_row == v_sectionrow)
                                {
                                    v_cell = new PDFjet.NET.Cell(p_datafieldfont);
                                    if (((Spartacus.Reporting.Field)this.v_fields[k]).v_column != "")
                                        v_text = ((Spartacus.Reporting.Field)this.v_fields[k]).Format(rb[((Spartacus.Reporting.Field)this.v_fields[k]).v_column].ToString());
                                    else
                                        v_text = "";
                                    v_cell.SetText(Spartacus.Reporting.Field.Crop(v_text, this.v_graphics, this.v_settings.v_datafieldfont.v_nativefont, ((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)this.v_fields[k]).v_fill) / 100));
                                    v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)this.v_fields[k]).v_fill) / 100);
                                    switch (((Spartacus.Reporting.Field)this.v_fields[k]).v_align)
                                    {
                                        case Spartacus.Reporting.FieldAlignment.LEFT:
                                            v_cell.SetTextAlignment(PDFjet.NET.Align.LEFT);
                                            break;
                                        case Spartacus.Reporting.FieldAlignment.RIGHT:
                                            v_cell.SetTextAlignment(PDFjet.NET.Align.RIGHT);
                                            break;
                                        case Spartacus.Reporting.FieldAlignment.CENTER:
                                            v_cell.SetTextAlignment(PDFjet.NET.Align.CENTER);
                                            break;
                                        default:
                                            break;
                                    }
                                    if (r % 2 == 0)
                                        v_cell.SetBgColor(this.v_settings.v_datafieldevencolor);
                                    else
                                        v_cell.SetBgColor(this.v_settings.v_datafieldoddcolor);
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_datafieldborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_datafieldborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_datafieldborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_datafieldborder.v_right);
                                    v_row.Add(v_cell);
                                }
                            }
                            p_data.Add(v_row);
                        }
                        r++;

                        this.v_perc += v_inc;
                        this.v_renderedrows++;
                        this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", this.v_perc, "Relatorio " + this.v_reportid.ToString() + ": linha " + this.v_renderedrows.ToString() + " de " + this.v_table.Rows.Count.ToString());
                    }
                }
                else
                {
                    this.RenderGroup(
                        p_level - 1,
                        p_data,
                        p_pageheight,
                        p_pagewidth,
                        p_datafieldfont,
                        p_groupheaderfont,
                        p_groupfooterfont
                    );
                }

                // renderizando campos do rodape
                if (v_group.v_showfooter)
                {
                    for (v_sectionrow = 0; v_sectionrow < v_group.v_numrowsfooter; v_sectionrow++)
                    {
                        v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                        for (k = 0; k < v_group.v_footerfields.Count; k++)
                        {
                            if (((Spartacus.Reporting.Field)v_group.v_footerfields[k]).v_row == v_sectionrow)
                            {
                                v_cell = new PDFjet.NET.Cell(p_groupfooterfont);
                                if (((Spartacus.Reporting.Field)v_group.v_footerfields[k]).v_column != "")
                                    v_text = ((Spartacus.Reporting.Field)v_group.v_footerfields[k]).Format(rg[((Spartacus.Reporting.Field)v_group.v_footerfields[k]).v_column].ToString());
                                else
                                    v_text = "";
                                v_cell.SetText(Spartacus.Reporting.Field.Crop(v_text, this.v_graphics, this.v_settings.v_groupfooterfont.v_nativefont, ((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)v_group.v_footerfields[k]).v_fill) / 100));
                                v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * ((Spartacus.Reporting.Field)v_group.v_footerfields[k]).v_fill) / 100);
                                switch (((Spartacus.Reporting.Field)v_group.v_footerfields[k]).v_align)
                                {
                                    case Spartacus.Reporting.FieldAlignment.LEFT:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.LEFT);
                                        break;
                                    case Spartacus.Reporting.FieldAlignment.RIGHT:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.RIGHT);
                                        break;
                                    case Spartacus.Reporting.FieldAlignment.CENTER:
                                        v_cell.SetTextAlignment(PDFjet.NET.Align.CENTER);
                                        break;
                                    default:
                                        break;
                                }
                                v_cell.SetBgColor(this.v_settings.v_groupfootercolor);
                                v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_groupfooterborder.v_top);
                                v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_groupfooterborder.v_bottom);
                                v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_groupfooterborder.v_left);
                                v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_groupfooterborder.v_right);
                                v_row.Add(v_cell);
                            }
                        }
                        p_data.Add(v_row);
                    }
                }
            }
        }
    }
}

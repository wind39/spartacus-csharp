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
using PDFjet;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Classe Report.
    /// Representa um relatório em PDF.
    /// </summary>
    public partial class Report
    {
        /// <summary>
        /// Código do Relatório.
        /// </summary>
        public int v_reportid;

        /// <summary>
        /// Objeto para comunicação com o banco de dados.
        /// </summary>
        public Spartacus.Database.Generic v_database;

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
        /// Tabela com os dados do relatório, ordenada de acordo com grupo de nível 0.
        /// Usada para renderização dos detalhes.
        /// </summary>
        public System.Data.DataTable v_rendertable;

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
        public System.Collections.Generic.List<Spartacus.Reporting.Field> v_fields;

        /// <summary>
        /// Lista de grupos do relatório.
        /// </summary>
        public System.Collections.Generic.List<Spartacus.Reporting.Group> v_groups;

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
        /// Modelo de renderização do detalhe ímpar do relatório.
        /// </summary>
        public System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_detailoddtemplate;

        /// <summary>
        /// Modelo de renderização do detalhe par do relatório.
        /// </summary>
        public System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_detaileventemplate;

        /// <summary>
        /// Arquivo de texto contendo a matriz de renderização.
        /// </summary>
        public System.IO.FileStream v_datafile;

        /// <summary>
        /// Número de linhas no cabeçalho de dados.
        /// </summary>
        public int v_numrowsdataheader;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo XML.</param>
        public Report(string p_filename)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

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
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_database">Objeto para conexão com o banco de dados.</param>
        public Report(string p_filename, Spartacus.Database.Generic p_database)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = p_database;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

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
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_table">Tabela com os dados.</param>
        public Report(string p_filename, System.Data.DataTable p_table)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;
            this.v_rendertable = null;

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
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        public Report(string p_filename, bool p_calculate_groups)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

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
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_database">Objeto para conexão com o banco de dados.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        public Report(string p_filename, Spartacus.Database.Generic p_database, bool p_calculate_groups)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = p_database;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

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
        /// <param name="p_filename">Nome do arquivo XML.</param>
        /// <param name="p_table">Tabela com os dados.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        public Report(string p_filename, System.Data.DataTable p_table, bool p_calculate_groups)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;
            this.v_rendertable = null;

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
        public Report(int p_reportid, string p_filename)
        {
            this.v_reportid = p_reportid;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

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

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = p_database;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

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

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;
            this.v_rendertable = null;

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

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

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

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = p_database;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

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

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;
            this.v_rendertable = null;

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
        /// <param name="p_content">Nome do arquivo XML ou conteúdo XML.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        public Report(string p_content, bool p_calculate_groups, bool p_isfilename)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_content, p_isfilename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_content">Nome do arquivo XML ou conteúdo XML.</param>
        /// <param name="p_database">Objeto para conexão com o banco de dados.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        public Report(string p_content, Spartacus.Database.Generic p_database, bool p_calculate_groups, bool p_isfilename)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = p_database;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_content, p_isfilename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_content">Nome do arquivo XML ou conteúdo XML.</param>
        /// <param name="p_table">Tabela com os dados.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        /// <param name="p_isfilename">Indica se <paramref name="p_content"/> representa nome de arquivo ou não.</param>
        public Report(string p_content, System.Data.DataTable p_table, bool p_calculate_groups, bool p_isfilename)
        {
            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;
            this.v_rendertable = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_content, p_isfilename);
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
        /// <param name="p_content">Nome do arquivo XML ou conteúdo XML.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        /// <param name="p_isfilename">Indica se <paramref name="p_content"/> representa nome de arquivo ou não.</param>
        public Report(int p_reportid, string p_content, bool p_calculate_groups, bool p_isfilename)
        {
            this.v_reportid = p_reportid;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_content, p_isfilename);
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
        /// <param name="p_filename">Nome do arquivo XML ou conteúdo XML.</param>
        /// <param name="p_database">Objeto para conexão com o banco de dados.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        /// <param name="p_isfilename">Indica se <paramref name="p_content"/> representa nome de arquivo ou não.</param>
        public Report(int p_reportid, string p_content, Spartacus.Database.Generic p_database, bool p_calculate_groups, bool p_isfilename)
        {
            this.v_reportid = p_reportid;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = p_database;
            this.v_tabletemp = null;
            this.v_table = null;
            this.v_rendertable = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_content, p_isfilename);
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
        /// <param name="p_content">Nome do arquivo XML ou conteúdo XML.</param>
        /// <param name="p_table">Tabela com os dados.</param>
        /// <param name="p_calculate_groups">Se o gerador de relatórios deve calcular os valores agrupados ou não.</param>
        /// <param name="p_isfilename">Indica se <paramref name="p_content"/> representa nome de arquivo ou não.</param>
        public Report(int p_reportid, string p_content, System.Data.DataTable p_table, bool p_calculate_groups, bool p_isfilename)
        {
            this.v_reportid = p_reportid;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;
            this.v_rendertable = null;

            this.v_calculate_groups = p_calculate_groups;

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(p_content, p_isfilename);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_table">Tabela com os dados.</param>
        public Report(System.Data.DataTable p_table)
        {
            string v_xml;
            Spartacus.Reporting.Field v_field;
            double v_fill;

            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;
            this.v_rendertable = null;

            this.v_calculate_groups = false;

            v_xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> " +
                "<report> " +
                "    <settings> " +
                "        <layout>LANDSCAPE</layout> " +
                "        <topmargin>30,0</topmargin> " +
                "        <bottommargin>20,0</bottommargin> " +
                "        <leftmargin>30,0</leftmargin> " +
                "        <rightmargin>20,0</rightmargin> " +
                "        <dataheaderborder>TOP,BOTTOM</dataheaderborder> " +
                "        <datafieldborder>NONE</datafieldborder> " +
                "        <groupheaderborder>NONE</groupheaderborder> " +
                "        <groupfooterborder>NONE</groupfooterborder> " +
                "        <reportheaderborder>NONE</reportheaderborder> " +
                "        <reportfooterborder>TOP</reportfooterborder> " +
                "        <dataheadercolor>WHITE</dataheadercolor> " +
                "        <datafieldevencolor>WHITE</datafieldevencolor> " +
                "        <datafieldoddcolor>SILVER</datafieldoddcolor> " +
                "        <groupheaderevencolor>SILVER</groupheaderevencolor> " +
                "        <groupheaderoddcolor>SILVER</groupheaderoddcolor> " +
                "        <groupfooterevencolor>SILVER</groupfooterevencolor> " +
                "        <groupfooteroddcolor>SILVER</groupfooteroddcolor> " +
                "        <reportheaderfont> " +
                "            <family>HELVETICA</family> " +
                "            <bold>FALSE</bold> " +
                "            <italic>FALSE</italic> " +
                "            <size>8,0</size> " +
                "        </reportheaderfont> " +
                "        <reportfooterfont> " +
                "            <family>HELVETICA</family> " +
                "            <bold>FALSE</bold> " +
                "            <italic>FALSE</italic> " +
                "            <size>8,0</size> " +
                "        </reportfooterfont> " +
                "        <dataheaderfont> " +
                "            <family>HELVETICA</family> " +
                "            <bold>FALSE</bold> " +
                "            <italic>FALSE</italic> " +
                "            <size>6,0</size> " +
                "        </dataheaderfont> " +
                "        <datafieldfont> " +
                "            <family>HELVETICA</family> " +
                "            <bold>FALSE</bold> " +
                "            <italic>FALSE</italic> " +
                "            <size>7,0</size> " +
                "        </datafieldfont> " +
                "        <groupheaderfont> " +
                "            <family>HELVETICA</family> " +
                "            <bold>TRUE</bold> " +
                "            <italic>FALSE</italic> " +
                "            <size>7,0</size> " +
                "        </groupheaderfont> " +
                "        <groupfooterfont> " +
                "            <family>HELVETICA</family> " +
                "            <bold>TRUE</bold> " +
                "            <italic>FALSE</italic> " +
                "            <size>7,0</size> " +
                "        </groupfooterfont> " +
                "    </settings> " +
                "    <header> " +
                "        <height>5,0</height> " +
                "    </header> " +
                "    <footer> " +
                "        <height>15,0</height> " +
                "        <object> " +
                "            <type>PAGENUMBER</type> " +
                "            <column></column> " +
                "            <posx>0,0</posx> " +
                "            <posy>10,0</posy> " +
                "            <align>RIGHT</align> " +
                "        </object> " +
                "    </footer> " +
                "    <fields> ";

            v_fill = 100.0 / (double)p_table.Columns.Count;
            foreach (System.Data.DataColumn c in p_table.Columns)
            {
                v_field = new Spartacus.Reporting.Field(
                    c.ColumnName,
                    c.ColumnName,
                    Spartacus.Reporting.FieldAlignment.LEFT,
                    v_fill,
                    Spartacus.Database.Type.STRING
                );
                v_xml += v_field.ToXML();
            }

            v_xml += "    </fields> " +
            "</report>";

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(v_xml, false);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Report"/>.
        /// </summary>
        /// <param name="p_table">Tabela com os dados.</param>
        /// <param name="p_titlecolumn">Coluna de título.</param>
        /// <param name="p_fields">Campos a serem exibidos no relatório.</param>
        public Report(System.Data.DataTable p_table, string p_titlecolumn, System.Collections.Generic.List<Spartacus.Reporting.Field> p_fields)
        {
            string v_xml;

            this.v_reportid = 0;

            this.v_header = new Spartacus.Reporting.Block();
            this.v_footer = new Spartacus.Reporting.Block();

            this.v_fields = new System.Collections.Generic.List<Spartacus.Reporting.Field>();
            this.v_groups = new System.Collections.Generic.List<Spartacus.Reporting.Group>();

            this.v_database = null;
            this.v_tabletemp = p_table;
            this.v_table = null;
            this.v_rendertable = null;

            this.v_calculate_groups = false;

            v_xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> " +
            "<report> " +
            "    <settings> " +
            "        <layout>LANDSCAPE</layout> " +
            "        <topmargin>30,0</topmargin> " +
            "        <bottommargin>20,0</bottommargin> " +
            "        <leftmargin>30,0</leftmargin> " +
            "        <rightmargin>20,0</rightmargin> " +
            "        <dataheaderborder>TOP,BOTTOM</dataheaderborder> " +
            "        <datafieldborder>NONE</datafieldborder> " +
            "        <groupheaderborder>NONE</groupheaderborder> " +
            "        <groupfooterborder>NONE</groupfooterborder> " +
            "        <reportheaderborder>NONE</reportheaderborder> " +
            "        <reportfooterborder>TOP</reportfooterborder> " +
            "        <dataheadercolor>WHITE</dataheadercolor> " +
            "        <datafieldevencolor>WHITE</datafieldevencolor> " +
            "        <datafieldoddcolor>SILVER</datafieldoddcolor> " +
            "        <groupheaderevencolor>SILVER</groupheaderevencolor> " +
            "        <groupheaderoddcolor>SILVER</groupheaderoddcolor> " +
            "        <groupfooterevencolor>SILVER</groupfooterevencolor> " +
            "        <groupfooteroddcolor>SILVER</groupfooteroddcolor> " +
            "        <reportheaderfont> " +
            "            <family>HELVETICA</family> " +
            "            <bold>FALSE</bold> " +
            "            <italic>FALSE</italic> " +
            "            <size>8,0</size> " +
            "        </reportheaderfont> " +
            "        <reportfooterfont> " +
            "            <family>HELVETICA</family> " +
            "            <bold>FALSE</bold> " +
            "            <italic>FALSE</italic> " +
            "            <size>8,0</size> " +
            "        </reportfooterfont> " +
            "        <dataheaderfont> " +
            "            <family>HELVETICA</family> " +
            "            <bold>FALSE</bold> " +
            "            <italic>FALSE</italic> " +
            "            <size>6,0</size> " +
            "        </dataheaderfont> " +
            "        <datafieldfont> " +
            "            <family>HELVETICA</family> " +
            "            <bold>FALSE</bold> " +
            "            <italic>FALSE</italic> " +
            "            <size>7,0</size> " +
            "        </datafieldfont> " +
            "        <groupheaderfont> " +
            "            <family>HELVETICA</family> " +
            "            <bold>TRUE</bold> " +
            "            <italic>FALSE</italic> " +
            "            <size>7,0</size> " +
            "        </groupheaderfont> " +
            "        <groupfooterfont> " +
            "            <family>HELVETICA</family> " +
            "            <bold>TRUE</bold> " +
            "            <italic>FALSE</italic> " +
            "            <size>7,0</size> " +
            "        </groupfooterfont> " +
            "    </settings> " +
            "    <header> " +
            "        <height>15,0</height> " +
            "        <object> " +
            "            <type>TEXT</type> " +
            "            <column>" + p_titlecolumn + "</column> " +
            "            <posx>0,0</posx> " +
            "            <posy>10,0</posy> " +
            "            <align>CENTER</align> " +
            "        </object> " +
            "    </header> " +
            "    <footer> " +
            "        <height>15,0</height> " +
            "        <object> " +
            "            <type>PAGENUMBER</type> " +
            "            <column></column> " +
            "            <posx>0,0</posx> " +
            "            <posy>10,0</posy> " +
            "            <align>RIGHT</align> " +
            "        </object> " +
            "    </footer> " +
            "    <fields> ";

            foreach (Spartacus.Reporting.Field f in p_fields)
                v_xml += f.ToXML();

            v_xml += "    </fields> " +
            "</report>";

            this.v_progress = new Spartacus.Utils.ProgressEventClass();
            this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", 0.0, "Lendo XML do relatorio " + this.v_reportid.ToString());

            try
            {
                this.ReadXml(v_xml, false);
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Executa o relatório.
        /// Se o relatório não possui tabela pré-definida, utiliza o comando SQL do relatório para buscar os dados no banco.
        /// Em seguida gera tabelas auxiliares para todos os grupos.
        /// </summary>
        public void Execute()
        {
            string v_parentgroupcolumn;
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
                            switch (this.v_fields[k].v_type)
                            {
                                case Spartacus.Database.Type.REAL:
                                    this.v_table.Columns[this.v_fields[k].v_column].DataType = typeof(double);
                                    break;
                                case Spartacus.Database.Type.INTEGER:
                                    this.v_table.Columns[this.v_fields[k].v_column].DataType = typeof(int);
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
                            v_parentgroupcolumn = null;
                            for (k = this.v_groups.Count-1; k >= 0; k--)
                            {
                                this.v_groups[k].BuildCalculate(this.v_table, v_parentgroupcolumn);
                                v_parentgroupcolumn = this.v_groups[k].v_column;
                            }
                        }
                    }
                    else
                    {
                        this.v_table = this.v_tabletemp;

                        // gerando tabelas auxiliares para todos os grupos
                        if (this.v_table != null && this.v_table.Rows.Count > 0 && this.v_groups.Count > 0)
                        {
                            v_parentgroupcolumn = null;
                            for (k = this.v_groups.Count-1; k >= 0; k--)
                            {
                                this.v_groups[k].Build(this.v_table, v_parentgroupcolumn);
                                v_parentgroupcolumn = this.v_groups[k].v_column;
                            }
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
                        switch (this.v_fields[k].v_type)
                        {
                            case Spartacus.Database.Type.REAL:
                                this.v_table.Columns[this.v_fields[k].v_column].DataType = typeof(double);
                                break;
                            case Spartacus.Database.Type.INTEGER:
                                this.v_table.Columns[this.v_fields[k].v_column].DataType = typeof(int);
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
                        v_parentgroupcolumn = null;
                        for (k = this.v_groups.Count-1; k >= 0; k--)
                        {
                            this.v_groups[k].BuildCalculate(this.v_table, v_parentgroupcolumn);
                            v_parentgroupcolumn = this.v_groups[k].v_column;
                        }
                    }
                }
                else
                {
                    this.v_table = this.v_tabletemp;

                    // gerando tabelas auxiliares para todos os grupos
                    if (this.v_table != null && this.v_table.Rows.Count > 0 && this.v_groups.Count > 0)
                    {
                        v_parentgroupcolumn = null;
                        for (k = this.v_groups.Count-1; k >= 0; k--)
                        {
                            this.v_groups[k].Build(this.v_table, v_parentgroupcolumn);
                            v_parentgroupcolumn = this.v_groups[k].v_column;
                        }
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
            PDFjet.NET.Table v_dataheadertable = null, v_datatable;
            float[] v_layout;
            PDFjet.NET.Page v_page;
            System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_rendered;
            int v_numpages, v_currentpage;
            Spartacus.Utils.Cryptor v_cryptor;
            string v_datafilename;
            System.IO.StreamReader v_reader;

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

                if (this.v_settings.v_showdataheader)
                {
                    v_dataheadertable = new PDFjet.NET.Table();
                    v_dataheadertable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height);

                    v_rendered = this.RenderDataHeader(
                        v_page.GetHeight(),
                        v_page.GetWidth(),
                        this.v_settings.v_dataheaderfont.GetFont(v_pdf)
                    );

                    v_dataheadertable.SetData(v_rendered, PDFjet.NET.Table.DATA_HAS_0_HEADER_ROWS);
                    //v_dataheadertable.SetCellBordersWidth(1.5f);
                }

                // tabela de dados

                v_datatable = new PDFjet.NET.Table();
                //v_datatable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height + ((this.v_settings.v_dataheaderfont.v_size + 2) * 1.8 * this.v_numrowsdetail));
                if (this.v_settings.v_showdataheader)
                    v_datatable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height + ((this.v_settings.v_dataheaderfont.v_size + 2) * 1.8 * this.v_numrowsdataheader));
                else
                    v_datatable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height);
                v_datatable.SetBottomMargin(this.v_settings.v_bottommargin + this.v_footer.v_height);

                this.BuildTemplates(
                    v_page.GetHeight(),
                    v_page.GetWidth(),
                    this.v_settings.v_datafieldfont.GetFont(v_pdf),
                    this.v_settings.v_groupheaderfont.GetFont(v_pdf),
                    this.v_settings.v_groupfooterfont.GetFont(v_pdf)
                );

                v_cryptor = new Spartacus.Utils.Cryptor("spartacus");
                v_datafilename = v_cryptor.RandomString() + ".tmp";
                this.v_datafile = System.IO.File.Open(
                    v_datafilename,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.ReadWrite
                );

                v_rendered = this.RenderData(
                    v_page.GetHeight(),
                    v_page.GetWidth(),
                    this.v_settings.v_datafieldfont.GetFont(v_pdf),
                    this.v_settings.v_groupheaderfont.GetFont(v_pdf),
                    this.v_settings.v_groupfooterfont.GetFont(v_pdf)
                );

                v_datatable.SetData(v_rendered, PDFjet.NET.Table.DATA_HAS_0_HEADER_ROWS);
                //v_datatable.SetCellBordersWidth(1.5f);

                this.v_datafile.Seek(0, System.IO.SeekOrigin.Begin);
                v_reader = new System.IO.StreamReader(this.v_datafile);

                // salvando PDF

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

                    if (this.v_settings.v_showdataheader)
                        v_dataheadertable.DrawOn(v_page);
                    v_datatable.ImprovedDrawOn(v_page, v_reader);

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
                        if (this.v_settings.v_showdataheader)
                            v_dataheadertable.ResetRenderedPagesCount();

                        v_page = new PDFjet.NET.Page(v_pdf, v_layout);
                        v_currentpage++;
                    }
                }

                v_pdf.Flush();
                v_buffer.Close();

                v_reader.Close();
                this.v_datafile.Close();
                (new System.IO.FileInfo(v_datafilename)).Delete();

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
            PDFjet.NET.Table v_dataheadertable = null, v_datatable;
            float[] v_layout;
            PDFjet.NET.Page v_page;
            System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> v_rendered;
            int v_numpages, v_currentpage;
            Spartacus.Utils.Cryptor v_cryptor;
            string v_datafilename;
            System.IO.StreamReader v_reader;

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

                if (this.v_settings.v_showdataheader)
                {
                    v_dataheadertable = new PDFjet.NET.Table();
                    v_dataheadertable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height);

                    v_rendered = this.RenderDataHeader(
                        v_page.GetHeight(),
                        v_page.GetWidth(),
                        this.v_settings.v_dataheaderfont.GetFont(p_pdf)
                    );

                    v_dataheadertable.SetData(v_rendered, PDFjet.NET.Table.DATA_HAS_0_HEADER_ROWS);
                    //v_dataheadertable.SetCellBordersWidth(1.5f);
                }

                // tabela de dados

                v_datatable = new PDFjet.NET.Table();
                //v_datatable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height + ((this.v_settings.v_dataheaderfont.v_size + 2) * 1.8 * this.v_numrowsdetail));
                if (this.v_settings.v_showdataheader)
                    v_datatable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height + ((this.v_settings.v_dataheaderfont.v_size + 2) * 1.8 * this.v_numrowsdataheader));
                else
                    v_datatable.SetPosition(this.v_settings.v_leftmargin, this.v_settings.v_topmargin  + this.v_header.v_height);
                v_datatable.SetBottomMargin(this.v_settings.v_bottommargin + this.v_footer.v_height);

                this.BuildTemplates(
                    v_page.GetHeight(),
                    v_page.GetWidth(),
                    this.v_settings.v_datafieldfont.GetFont(p_pdf),
                    this.v_settings.v_groupheaderfont.GetFont(p_pdf),
                    this.v_settings.v_groupfooterfont.GetFont(p_pdf)
                );

                v_cryptor = new Spartacus.Utils.Cryptor("spartacus");
                v_datafilename = v_cryptor.RandomString() + ".tmp";
                this.v_datafile = System.IO.File.Open(
                    v_datafilename,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.ReadWrite
                );

                v_rendered = this.RenderData(
                    v_page.GetHeight(),
                    v_page.GetWidth(),
                    this.v_settings.v_datafieldfont.GetFont(p_pdf),
                    this.v_settings.v_groupheaderfont.GetFont(p_pdf),
                    this.v_settings.v_groupfooterfont.GetFont(p_pdf)
                );

                v_datatable.SetData(v_rendered, PDFjet.NET.Table.DATA_HAS_0_HEADER_ROWS);
                //v_datatable.SetCellBordersWidth(1.5f);

                this.v_datafile.Seek(0, System.IO.SeekOrigin.Begin);
                v_reader = new System.IO.StreamReader(this.v_datafile);

                // salvando PDF

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

                    if (this.v_settings.v_showdataheader)
                        v_dataheadertable.DrawOn(v_page);
                    v_datatable.ImprovedDrawOn(v_page, v_reader);

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
                        if (this.v_settings.v_showdataheader)
                            v_dataheadertable.ResetRenderedPagesCount();

                        v_page = new PDFjet.NET.Page(p_pdf, v_layout);
                        v_currentpage++;
                    }
                }

                v_reader.Close();
                this.v_datafile.Close();
                (new System.IO.FileInfo(v_datafilename)).Delete();

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
            int g, k, v_sectionrow;

            v_data = new System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>>();
            this.v_numrowsdataheader = 0;

            // renderizando titulos de cabeçalhos de grupos
            for (g = this.v_groups.Count - 1; g >= 0; g--)
            {
                if (this.v_groups[g].v_showheadertitles)
                {
                    this.v_numrowsdataheader += this.v_groups[g].v_numrowsheader;

                    for (v_sectionrow = 0; v_sectionrow < this.v_groups[g].v_numrowsheader; v_sectionrow++)
                    {
                        v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                        for (k = 0; k < this.v_groups[g].v_headerfields.Count; k++)
                        {
                            if (this.v_groups[g].v_headerfields[k].v_row == v_sectionrow)
                            {
                                v_cell = new PDFjet.NET.Cell(p_dataheaderfont);
                                v_cell.SetText(this.v_groups[g].v_headerfields[k].v_title);
                                v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * this.v_groups[g].v_headerfields[k].v_fill) / 100);
                                switch (this.v_groups[g].v_headerfields[k].v_align)
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
                                if (this.v_groups[g].v_headerfields[k].v_border != null)
                                {
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_groups[g].v_headerfields[k].v_border.v_top || this.v_settings.v_dataheaderborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_groups[g].v_headerfields[k].v_border.v_bottom || this.v_settings.v_dataheaderborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_groups[g].v_headerfields[k].v_border.v_left || this.v_settings.v_dataheaderborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_groups[g].v_headerfields[k].v_border.v_right || this.v_settings.v_dataheaderborder.v_right);
                                }
                                else
                                {
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_dataheaderborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_dataheaderborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_dataheaderborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_dataheaderborder.v_right);
                                }
                                v_row.Add(v_cell);
                            }
                        }
                        v_data.Add(v_row);
                    }
                }
            }

            // renderizando titulos de campos de detalhe
            this.v_numrowsdataheader += this.v_numrowsdetail;
            for (v_sectionrow = 0; v_sectionrow < this.v_numrowsdetail; v_sectionrow++)
            {
                v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                for (k = 0; k < this.v_fields.Count; k++)
                {
                    if (this.v_fields[k].v_row == v_sectionrow)
                    {
                        v_cell = new PDFjet.NET.Cell(p_dataheaderfont);
                        v_cell.SetText(this.v_fields[k].v_title);
                        v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * this.v_fields[k].v_fill) / 100);
                        switch (this.v_fields[k].v_align)
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
                        if (this.v_fields[k].v_border != null)
                        {
                            v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_fields[k].v_border.v_top || this.v_settings.v_dataheaderborder.v_top);
                            v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_fields[k].v_border.v_bottom || this.v_settings.v_dataheaderborder.v_bottom);
                            v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_fields[k].v_border.v_left || this.v_settings.v_dataheaderborder.v_left);
                            v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_fields[k].v_border.v_right || this.v_settings.v_dataheaderborder.v_right);
                        }
                        else
                        {
                            v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_dataheaderborder.v_top);
                            v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_dataheaderborder.v_bottom);
                            v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_dataheaderborder.v_left);
                            v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_dataheaderborder.v_right);
                        }
                        v_row.Add(v_cell);
                    }
                }
                v_data.Add(v_row);
            }

            // renderizando titulos de rodapés de grupos
            for (g = this.v_groups.Count - 1; g >= 0; g--)
            {
                if (this.v_groups[g].v_showfootertitles)
                {
                    this.v_numrowsdataheader += this.v_groups[g].v_numrowsfooter;

                    for (v_sectionrow = 0; v_sectionrow < this.v_groups[g].v_numrowsfooter; v_sectionrow++)
                    {
                        v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                        for (k = 0; k < this.v_groups[g].v_footerfields.Count; k++)
                        {
                            if (this.v_groups[g].v_footerfields[k].v_row == v_sectionrow)
                            {
                                v_cell = new PDFjet.NET.Cell(p_dataheaderfont);
                                v_cell.SetText(this.v_groups[g].v_footerfields[k].v_title);
                                v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * this.v_groups[g].v_footerfields[k].v_fill) / 100);
                                switch (this.v_groups[g].v_footerfields[k].v_align)
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
                                if (this.v_groups[g].v_footerfields[k].v_border != null)
                                {
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_groups[g].v_footerfields[k].v_border.v_top || this.v_settings.v_dataheaderborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_groups[g].v_footerfields[k].v_border.v_bottom || this.v_settings.v_dataheaderborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_groups[g].v_footerfields[k].v_border.v_left || this.v_settings.v_dataheaderborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_groups[g].v_footerfields[k].v_border.v_right || this.v_settings.v_dataheaderborder.v_right);
                                }
                                else
                                {
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_dataheaderborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_dataheaderborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_dataheaderborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_dataheaderborder.v_right);
                                }
                                v_row.Add(v_cell);
                            }
                        }
                        v_data.Add(v_row);
                    }
                }
            }

            return v_data;
        }

        /// <summary>
        /// Contrói modelos de renderização em memória.
        /// </summary>
        /// <param name="p_pageheight">Altura da página.</param>
        /// <param name="p_pagewidth">Largura da página.</param>
        /// <param name="p_datafieldfont">Fonte do campo de dados.</param>
        /// <param name="p_groupheaderfont">Fonte do cabeçalho de grupo.</param>
        /// <param name="p_groupfooterfont">Fonte do rodapé de grupo.</param>
        private void BuildTemplates(
            float p_pageheight,
            float p_pagewidth,
            PDFjet.NET.Font p_datafieldfont,
            PDFjet.NET.Font p_groupheaderfont,
            PDFjet.NET.Font p_groupfooterfont
        )
        {
            System.Collections.Generic.List<PDFjet.NET.Cell> v_row;
            PDFjet.NET.Cell v_cell;
            Spartacus.Reporting.Group v_group;
            int k, r, v_sectionrow, v_level;

            // modelo do detalhe ímpar

            this.v_detailoddtemplate = new System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>>();

            for (v_sectionrow = 0; v_sectionrow < this.v_numrowsdetail; v_sectionrow++)
            {
                v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                for (k = 0; k < this.v_fields.Count; k++)
                {
                    if (this.v_fields[k].v_row == v_sectionrow)
                    {
                        v_cell = new PDFjet.NET.Cell(p_datafieldfont);
                        v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * this.v_fields[k].v_fill) / 100);
                        switch (this.v_fields[k].v_align)
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
                        v_cell.SetBgColor(this.v_settings.v_datafieldoddcolor);
                        if (this.v_fields[k].v_border != null)
                        {
                            v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_fields[k].v_border.v_top || this.v_settings.v_datafieldborder.v_top);
                            v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_fields[k].v_border.v_bottom || this.v_settings.v_datafieldborder.v_bottom);
                            v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_fields[k].v_border.v_left || this.v_settings.v_datafieldborder.v_left);
                            v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_fields[k].v_border.v_right || this.v_settings.v_datafieldborder.v_right);
                        }
                        else
                        {
                            v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_datafieldborder.v_top);
                            v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_datafieldborder.v_bottom);
                            v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_datafieldborder.v_left);
                            v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_datafieldborder.v_right);
                        }
                        v_row.Add(v_cell);
                    }
                }
                this.v_detailoddtemplate.Add(v_row);
            }

            // modelo do detalhe par

            this.v_detaileventemplate = new System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>>();

            for (v_sectionrow = 0; v_sectionrow < this.v_numrowsdetail; v_sectionrow++)
            {
                v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                for (k = 0; k < this.v_fields.Count; k++)
                {
                    if (this.v_fields[k].v_row == v_sectionrow)
                    {
                        v_cell = new PDFjet.NET.Cell(p_datafieldfont);
                        v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * this.v_fields[k].v_fill) / 100);
                        switch (this.v_fields[k].v_align)
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
                        v_cell.SetBgColor(this.v_settings.v_datafieldevencolor);
                        if (this.v_fields[k].v_border != null)
                        {
                            v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_fields[k].v_border.v_top || this.v_settings.v_datafieldborder.v_top);
                            v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_fields[k].v_border.v_bottom || this.v_settings.v_datafieldborder.v_bottom);
                            v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_fields[k].v_border.v_left || this.v_settings.v_datafieldborder.v_left);
                            v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_fields[k].v_border.v_right || this.v_settings.v_datafieldborder.v_right);
                        }
                        else
                        {
                            v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_datafieldborder.v_top);
                            v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_datafieldborder.v_bottom);
                            v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_datafieldborder.v_left);
                            v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_datafieldborder.v_right);
                        }
                        v_row.Add(v_cell);
                    }
                }
                this.v_detaileventemplate.Add(v_row);
            }

            for (v_level = 0; v_level < this.v_groups.Count; v_level++)
            {
                v_group = this.v_groups[v_level];

                // modelo do cabeçalho do grupo

                if (v_group.v_showheader)
                {
                    v_group.v_headertemplate = new System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>>();

                    r = 0;
                    for (v_sectionrow = 0; v_sectionrow < v_group.v_numrowsheader; v_sectionrow++)
                    {
                        v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                        for (k = 0; k < v_group.v_headerfields.Count; k++)
                        {
                            if (v_group.v_headerfields[k].v_row == v_sectionrow)
                            {
                                v_cell = new PDFjet.NET.Cell(p_groupheaderfont);
                                v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * v_group.v_headerfields[k].v_fill) / 100);
                                switch (v_group.v_headerfields[k].v_align)
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
                                    v_cell.SetBgColor(this.v_settings.v_groupheaderevencolor);
                                else
                                    v_cell.SetBgColor(this.v_settings.v_groupheaderoddcolor);
                                if (v_group.v_headerfields[k].v_border != null)
                                {
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, v_group.v_headerfields[k].v_border.v_top || this.v_settings.v_groupheaderborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, v_group.v_headerfields[k].v_border.v_bottom || this.v_settings.v_groupheaderborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, v_group.v_headerfields[k].v_border.v_left || this.v_settings.v_groupheaderborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, v_group.v_headerfields[k].v_border.v_right || this.v_settings.v_groupheaderborder.v_right);
                                }
                                else
                                {
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_groupheaderborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_groupheaderborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_groupheaderborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_groupheaderborder.v_right);
                                }
                                v_row.Add(v_cell);
                            }
                        }
                        v_group.v_headertemplate.Add(v_row);
                        r++;
                    }
                }

                // modelo do rodapé grupo

                if (v_group.v_showfooter)
                {
                    v_group.v_footertemplate = new System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>>();

                    r = 0;
                    for (v_sectionrow = 0; v_sectionrow < v_group.v_numrowsfooter; v_sectionrow++)
                    {
                        v_row = new System.Collections.Generic.List<PDFjet.NET.Cell>();
                        for (k = 0; k < v_group.v_footerfields.Count; k++)
                        {
                            if (v_group.v_footerfields[k].v_row == v_sectionrow)
                            {
                                v_cell = new PDFjet.NET.Cell(p_groupfooterfont);
                                v_cell.SetWidth(((p_pagewidth - (this.v_settings.v_leftmargin + this.v_settings.v_rightmargin)) * v_group.v_footerfields[k].v_fill) / 100);
                                switch (v_group.v_footerfields[k].v_align)
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
                                    v_cell.SetBgColor(this.v_settings.v_groupfooterevencolor);
                                else
                                    v_cell.SetBgColor(this.v_settings.v_groupfooteroddcolor);
                                if (v_group.v_footerfields[k].v_border != null)
                                {
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, v_group.v_footerfields[k].v_border.v_top || this.v_settings.v_groupfooterborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, v_group.v_footerfields[k].v_border.v_bottom || this.v_settings.v_groupfooterborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, v_group.v_footerfields[k].v_border.v_left || this.v_settings.v_groupfooterborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, v_group.v_footerfields[k].v_border.v_right || this.v_settings.v_groupfooterborder.v_right);
                                }
                                else
                                {
                                    v_cell.SetBorder(PDFjet.NET.Border.TOP, this.v_settings.v_groupfooterborder.v_top);
                                    v_cell.SetBorder(PDFjet.NET.Border.BOTTOM, this.v_settings.v_groupfooterborder.v_bottom);
                                    v_cell.SetBorder(PDFjet.NET.Border.LEFT, this.v_settings.v_groupfooterborder.v_left);
                                    v_cell.SetBorder(PDFjet.NET.Border.RIGHT, this.v_settings.v_groupfooterborder.v_right);
                                }
                                v_row.Add(v_cell);
                            }
                        }
                        v_group.v_footertemplate.Add(v_row);
                        r++;
                    }
                }
            }
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
            System.Data.DataView v_view;
            string v_textrow;
            System.IO.StreamWriter v_writer;
            string v_text;
            int k, r, v_sectionrow;
            string v_sort;

            v_data = new System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>>();
            v_writer = new System.IO.StreamWriter(this.v_datafile);

            // se o relatorio possui grupos
            if (this.v_groups.Count > 0)
            {
                // ordenando campos da tabela original para otimizar a renderização
                v_sort = "";
                for (k = this.v_groups.Count - 1; k >= 0; k--)
                    v_sort += this.v_groups[k].v_column + ", ";
                v_view = new System.Data.DataView(this.v_table);
                v_view.Sort = v_sort + this.v_groups[0].v_sort;
                this.v_rendertable = v_view.ToTable();

                this.RenderGroup(
                    this.v_groups.Count - 1,
                    null,
                    null,
                    v_data,
                    p_pageheight,
                    p_pagewidth,
                    p_datafieldfont,
                    p_groupheaderfont,
                    p_groupfooterfont,
                    v_writer
                );
            }
            else // se o relatorio nao possui grupos
            {
                r = 0;
                foreach (System.Data.DataRow rb in this.v_table.Rows)
                {
                    for (v_sectionrow = 0; v_sectionrow < this.v_numrowsdetail; v_sectionrow++)
                    {
                        v_textrow = "";
                        for (k = 0; k < this.v_fields.Count; k++)
                        {
                            if (this.v_fields[k].v_row == v_sectionrow)
                            {
                                v_text = this.v_fields[k].Format(rb[this.v_fields[k].v_column].ToString());
                                v_textrow += v_text.Replace(';', ',') + ";";
                            }
                        }
                        v_writer.WriteLine(v_textrow);
                    }

                    if (r % 2 == 0)
                        v_data.AddRange(this.v_detaileventemplate);
                    else
                        v_data.AddRange(this.v_detailoddtemplate);
                    r++;

                    this.v_perc += v_inc;
                    this.v_renderedrows++;
                    this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", this.v_perc, "Relatorio " + this.v_reportid.ToString() + ": linha " + this.v_renderedrows.ToString() + " de " + this.v_table.Rows.Count.ToString());
                }
            }

            v_writer.Flush();

            return v_data;
        }

        /// <summary>
        /// Renderiza um grupo.
        /// </summary>
        /// <param name="p_level">Nível do grupo atual.</param>
        /// <param name="p_parentgroupcolumn">Coluna do grupo pai.</param>
        /// <param name="p_parentgroupvalue">Valor do grupo pai.</param>
        /// <param name="p_data">Matriz de dados.</param>
        /// <param name="p_pageheight">Altura da página.</param>
        /// <param name="p_pagewidth">Largura da página.</param>
        /// <param name="p_datafieldfont">Fonte do campo de dados.</param>
        /// <param name="p_groupheaderfont">Fonte do cabeçalho de grupo.</param>
        /// <param name="p_groupfooterfont">Fonte do rodapé de grupo.</param>
        private void RenderGroup(
            int p_level,
            string p_parentgroupcolumn,
            string p_parentgroupvalue,
            System.Collections.Generic.List<System.Collections.Generic.List<PDFjet.NET.Cell>> p_data,
            float p_pageheight,
            float p_pagewidth,
            PDFjet.NET.Font p_datafieldfont,
            PDFjet.NET.Font p_groupheaderfont,
            PDFjet.NET.Font p_groupfooterfont,
            System.IO.StreamWriter p_writer
        )
        {
            Spartacus.Reporting.Group v_group;
            string v_textrow;
            string v_text;
            int k, r, v_sectionrow;
            System.Data.DataRow rg, rb;

            v_group = this.v_groups[p_level];

            // percorrendo elementos do grupo
            rg = v_group.v_table.Rows[v_group.v_renderedrows];
            while (v_group.v_renderedrows < v_group.v_table.Rows.Count &&
                   (v_group.v_level == this.v_groups.Count-1 ||
                    rg[p_parentgroupcolumn].ToString() == p_parentgroupvalue))
            {
                // renderizando campos do cabecalho
                if (v_group.v_showheader)
                {
                    for (v_sectionrow = 0; v_sectionrow < v_group.v_numrowsheader; v_sectionrow++)
                    {
                        v_textrow = "";
                        for (k = 0; k < v_group.v_headerfields.Count; k++)
                        {
                            if (v_group.v_headerfields[k].v_row == v_sectionrow)
                            {
                                if (v_group.v_headerfields[k].v_column != "")
                                    v_text = v_group.v_headerfields[k].Format(rg[v_group.v_headerfields[k].v_column].ToString());
                                else
                                    v_text = "";
                                v_textrow += v_text.Replace(';', ',').Replace('\n', ' ').Replace('\r', ' ') + ";";
                            }
                        }
                        p_writer.WriteLine(v_textrow);
                    }

                    p_data.AddRange(v_group.v_headertemplate);
                }

                if (v_group.v_level == 0)
                {
                    // renderizando dados do grupo
                    r = 0;
                    rb = this.v_rendertable.Rows[this.v_renderedrows];
                    while (this.v_renderedrows < this.v_rendertable.Rows.Count &&
                           rb[v_group.v_column].ToString() == rg[v_group.v_column].ToString() &&
                           (v_group.v_level == this.v_groups.Count-1 ||
                            rb[p_parentgroupcolumn].ToString() == p_parentgroupvalue))
                    {
                        for (v_sectionrow = 0; v_sectionrow < this.v_numrowsdetail; v_sectionrow++)
                        {
                            v_textrow = "";
                            for (k = 0; k < this.v_fields.Count; k++)
                            {
                                if (this.v_fields[k].v_row == v_sectionrow)
                                {
                                    v_text = this.v_fields[k].Format(rb[this.v_fields[k].v_column].ToString());
                                    v_textrow += v_text.Replace(';', ',').Replace('\n', ' ').Replace('\r', ' ') + ";";
                                }
                            }
                            p_writer.WriteLine(v_textrow);
                        }

                        if (r % 2 == 0)
                            p_data.AddRange(this.v_detaileventemplate);
                        else
                            p_data.AddRange(this.v_detailoddtemplate);
                        r++;

                        this.v_perc += v_inc;
                        this.v_renderedrows++;
                        this.v_progress.FireEvent("Spartacus.Reporting.Report", "ExportPDF", this.v_perc, "Relatorio " + this.v_reportid.ToString() + ": linha " + this.v_renderedrows.ToString() + " de " + this.v_table.Rows.Count.ToString());

                        if (this.v_renderedrows < this.v_rendertable.Rows.Count)
                            rb = this.v_rendertable.Rows[this.v_renderedrows];
                    }
                }
                else
                {
                    this.RenderGroup(
                        p_level - 1,
                        v_group.v_column,
                        rg[v_group.v_column].ToString(),
                        p_data,
                        p_pageheight,
                        p_pagewidth,
                        p_datafieldfont,
                        p_groupheaderfont,
                        p_groupfooterfont,
                        p_writer
                    );
                }

                // renderizando campos do rodape
                if (v_group.v_showfooter)
                {
                    for (v_sectionrow = 0; v_sectionrow < v_group.v_numrowsfooter; v_sectionrow++)
                    {
                        v_textrow = "";
                        for (k = 0; k < v_group.v_footerfields.Count; k++)
                        {
                            if (v_group.v_footerfields[k].v_row == v_sectionrow)
                            {
                                if (v_group.v_footerfields[k].v_column != "")
                                    v_text = v_group.v_footerfields[k].Format(rg[v_group.v_footerfields[k].v_column].ToString());
                                else
                                    v_text = "";
                                v_textrow += v_text.Replace(';', ',').Replace('\n', ' ').Replace('\r', ' ') + ";";
                            }
                        }
                        p_writer.WriteLine(v_textrow);
                    }

                    p_data.AddRange(v_group.v_footertemplate);
                }

                v_group.v_renderedrows++;
                if (v_group.v_renderedrows < v_group.v_table.Rows.Count)
                    rg = v_group.v_table.Rows[v_group.v_renderedrows];
            }
        }
    }
}

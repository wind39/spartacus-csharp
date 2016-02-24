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
using System.Data;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe abstrata Spartacus.Database.Generic.
    /// Armazena informações de conexão que são genéricas a qualquer SGBD.
    /// Provê polimorfismo, por ser uma classe abstrata.
    /// </summary>
    public abstract class Generic
    {
        /// <summary>
        /// Hostname ou IP onde o banco de dados está localizado.
        /// </summary>
        public string v_host;

        /// <summary>
        /// Porta TCP para conectar-se ao SGBG.
        /// </summary>
        public string v_port;

        /// <summary>
        /// Nome do serviço que representa o banco ao qual desejamos nos conectar.
        /// </summary>
        public string v_service;

        /// <summary>
        /// Usuário ou schema para se conectar ao banco de dados.
        /// </summary>
        public string v_user;

        /// <summary>
        /// A senha do usuário ou schema.
        /// </summary>
        public string v_password;

        /// <summary>
        /// Segurança integrada (suportada apenas na classe SqlServer).
        /// </summary>
        public bool v_integrated_security;

        /// <summary>
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// Timeout de execução de comandos, em segundos.
        /// </summary>
        public int v_timeout;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// Armazena informações de conexão que são genéricas a qualquer SGBD.
        /// </summary>
        /// <param name='p_host'>
        /// Hostname ou IP onde o banco de dados está localizado.
        /// </param>
        /// <param name='p_port'>
        /// Porta TCP para conectar-se ao SGBG.
        /// </param>
        /// <param name='p_service'>
        /// Nome do serviço que representa o banco ao qual desejamos nos conectar.
        /// </param>
        /// <param name='p_user'>
        /// Usuário ou schema para se conectar ao banco de dados.
        /// </param>
        /// <param name='p_password'>
        /// A senha do usuário ou schema.
        /// </param>
        public Generic(string p_host, string p_port, string p_service, string p_user, string p_password)
        {
            this.v_host = p_host;
            this.v_port = p_port;
            this.v_service = p_service;
            this.v_user = p_user;
            this.v_password = p_password;
            this.v_integrated_security = false;
            this.v_timeout = -1;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// Armazena informações de conexão que são genéricas a qualquer SGBD.
        /// </summary>
        /// <param name='p_host'>
        /// Hostname ou IP onde o banco de dados está localizado.
        /// </param>
        /// <param name='p_port'>
        /// Porta TCP para conectar-se ao SGBG.
        /// </param>
        /// <param name='p_service'>
        /// Nome do serviço que representa o banco ao qual desejamos nos conectar.
        /// </param>
        /// <param name='p_user'>
        /// Usuário ou schema para se conectar ao banco de dados.
        /// </param>
        /// <param name='p_password'>
        /// A senha do usuário ou schema.
        /// </param>
        /// <param name='p_integrated_security'>
        /// Segurança integrada (suportada apenas na classe SqlServer).
        /// </param>
        public Generic(string p_host, string p_port, string p_service, string p_user, string p_password, bool p_integrated_security)
        {
            this.v_host = p_host;
            this.v_port = p_port;
            this.v_service = p_service;
            this.v_user = p_user;
            this.v_password = p_password;
            this.v_integrated_security = p_integrated_security;
            this.v_timeout = -1;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// Armazena informações de conexão que são genéricas a qualquer SGBD.
        /// </summary>
        /// <param name='p_dsn'>
        /// DSN (Data Source Name).
        /// </param>
        /// <param name='p_user'>
        /// Usuário ou schema para se conectar ao banco de dados.
        /// </param>
        /// <param name='p_password'>
        /// A senha do usuário ou schema.
        /// </param>
        public Generic(string p_dsn, string p_user, string p_password)
        {
            this.v_service = p_dsn;
            this.v_user = p_user;
            this.v_password = p_password;
            this.v_integrated_security = false;
            this.v_timeout = -1;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// Armazena informações de conexão que são genéricas a qualquer SGBD.
        /// </summary>
        /// <param name='p_file'>
        /// Arquivo do banco de dados.
        /// </param>
        public Generic(string p_file)
        {
            this.v_service = p_file;
            this.v_integrated_security = false;
            this.v_timeout = -1;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// </summary>
        public Generic()
        {
            this.v_timeout = -1;
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do banco de dados a ser criado.</param>
        public abstract void CreateDatabase(string p_name);

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        public abstract void CreateDatabase();

        /// <summary>
        /// Abre a conexão com o banco de dados.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_tablename'>
        /// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
        /// </param>
        public abstract System.Data.DataTable Query(string p_sql, string p_tablename);

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_tablename'>
        /// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
        /// </param>
        /// <param name='p_progress'>
        /// Evento de progresso da execução da consulta.
        /// </param>
        public abstract System.Data.DataTable Query(string p_sql, string p_tablename, Spartacus.Utils.ProgressEventClass p_progress);

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
        /// Utiliza um DataReader para buscar em blocos.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_tablename'>
        /// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
        /// </param>
        /// <param name='p_startrow'>
        /// Número da linha inicial.
        /// </param>
        /// <param name='p_endrow'>
        /// Número da linha final.
        /// </param>
        /// <param name='p_hasmoredata'>
        /// Indica se ainda há mais dados a serem lidos.
        /// </param>
        public abstract System.Data.DataTable Query(string p_sql, string p_tablename, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma string HTML.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_id'>
        /// ID da tabela no HTML.
        /// </param>
        /// <param name='p_options'>
        /// Opções da tabela no HTML.
        /// </param>
        public abstract string QueryHtml(string p_sql, string p_id, string p_options);

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
        /// Utiliza um DataReader para buscar em blocos a partir do cursor de saída de uma Stored Procedure.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_tablename'>
        /// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
        /// </param>
        /// <param name='p_outparam'>
        /// Nome do parâmetro de saída que deve ser um REF CURSOR.
        /// </param>
        /// <remarks>Não suportado em todos os SGBDs.</remarks>
        public abstract System.Data.DataTable QueryStoredProc(string p_sql, string p_tablename, string p_outparam);

        /// <summary>
        /// Executa uma instrução SQL no banco de dados.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser executado no banco de dados.
        /// </param>
        public abstract void Execute(string p_sql);

        /// <summary>
        /// Insere um bloco de linhas em uma determinada tabela.
        /// </summary>
        /// <param name='p_table'>
        /// Nome da tabela a serem inseridas as linhas.
        /// </param>
        /// <param name='p_rows'>
        /// Lista de linhas a serem inseridas na tabela.
        /// </param>
        public abstract void InsertBlock(string p_table, System.Collections.Generic.List<string> p_rows);

        /// <summary>
        /// Insere um bloco de linhas em uma determinada tabela.
        /// </summary>
        /// <param name='p_table'>
        /// Nome da tabela a serem inseridas as linhas.
        /// </param>
        /// <param name='p_rows'>
        /// Lista de linhas a serem inseridas na tabela.
        /// </param>
        /// <param name='p_columnnames'>
        /// Nomes de colunas da tabela, entre parênteses, separados por vírgula.
        /// </param>
        public abstract void InsertBlock(string p_table, System.Collections.Generic.List<string> p_rows, string p_columnnames);

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando um único dado de retorno em uma string.
        /// </summary>
        /// <returns>
        /// string com o dado de retorno.
        /// </returns>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        public abstract string ExecuteScalar(string p_sql);

        /// <summary>
        /// Fecha a conexão com o banco de dados.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Deleta um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do banco de dados a ser deletado.</param>
        public abstract void DropDatabase(string p_name);

        /// <summary>
        /// Deleta o banco de dados conectado atualmente.
        /// </summary>
        public abstract void DropDatabase();

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        public abstract uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase);

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_log">Log de inserção.</param>
        public abstract uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, out string p_log);

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public abstract uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_log">Log de inserção.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public abstract uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_table">Nome da tabela de destino.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public abstract uint Transfer(string p_query, string p_table, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_table">Nome da tabela de destino.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_log">Log de inserção.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public abstract uint Transfer(string p_query, string p_table, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public abstract uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error);

        /// <summary>
        /// Lista os nomes de colunas de uma determinada consulta.
        /// </summary>
        /// <returns>Vetor com os nomes de colunas.</returns>
        /// <param name="p_query">Consulta SQL.</param>
        public abstract string[] GetColumnNames(string p_query);

        /// <summary>
        /// Lista os nomes e tipos de colunas de uma determinada consulta.
        /// </summary>
        /// <returns>Matriz com os nomes e tipos de colunas.</returns>
        /// <param name="p_query">Consulta SQL.</param>
        public abstract string[,] GetColumnNamesAndTypes(string p_query);

        /// <summary>
        /// Transfere dados de um arquivo Excel para o banco de dados atual.
        /// Conexão com o banco atual precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo de origem.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de dados atual.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            Spartacus.Utils.Excel v_excel = null;
            uint v_transfered = 0;
            string v_insert;

            try
            {
                v_excel = new Spartacus.Utils.Excel();
                v_excel.Import(p_filename);

                foreach (System.Data.DataRow r in v_excel.v_set.Tables[0].Rows)
                {
                    foreach (System.Data.DataColumn c in v_excel.v_set.Tables[0].Columns)
                    {
                        if (p_insert.Exists(c.ColumnName))
                            p_insert.SetValue(c.ColumnName, r[c].ToString());
                    }

                    v_insert = p_insert.GetUpdatedText();
                    try
                    {
                        this.Execute(v_insert);
                        v_transfered++;
                        p_progress.FireEvent(v_transfered);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_error.FireEvent(v_insert + "\n" + e.v_message);
                    }
                }

                return v_transfered;
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
            catch (Spartacus.Database.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_excel != null)
                {
                    v_excel.Clear();
                    v_excel = null;
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo Excel para o banco de dados atual.
        /// Conexão com o banco atual precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo de origem.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_header">Se deve considerar a primeira linha como cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação para leitura do arquivo CSV.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de dados atual.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, char p_separator, bool p_header, System.Text.Encoding p_encoding, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            Spartacus.Utils.Excel v_excel = null;
            uint v_transfered = 0;
            string v_insert;

            try
            {
                v_excel = new Spartacus.Utils.Excel();
                v_excel.Import(p_filename, p_separator, p_header, p_encoding);

                foreach (System.Data.DataRow r in v_excel.v_set.Tables[0].Rows)
                {
                    foreach (System.Data.DataColumn c in v_excel.v_set.Tables[0].Columns)
                    {
                        if (p_insert.Exists(c.ColumnName))
                            p_insert.SetValue(c.ColumnName, r[c].ToString());
                    }

                    v_insert = p_insert.GetUpdatedText();
                    try
                    {
                        this.Execute(v_insert);
                        v_transfered++;
                        p_progress.FireEvent(v_transfered);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_error.FireEvent(v_insert + "\n" + e.v_message);
                    }
                }

                return v_transfered;
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
            catch (Spartacus.Database.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_excel != null)
                {
                    v_excel.Clear();
                    v_excel = null;
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo Excel para o banco de dados atual.
        /// Conexão com o banco atual precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo de origem.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_delimitator">Delimitador de campos do arquivo CSV.</param>
        /// <param name="p_header">Se deve considerar a primeira linha como cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação para leitura do arquivo CSV.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de dados atual.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, char p_separator, char p_delimitator, bool p_header, System.Text.Encoding p_encoding, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            Spartacus.Utils.Excel v_excel = null;
            uint v_transfered = 0;
            string v_insert;

            try
            {
                v_excel = new Spartacus.Utils.Excel();
                v_excel.Import(p_filename, p_separator, p_delimitator, p_header, p_encoding);

                foreach (System.Data.DataRow r in v_excel.v_set.Tables[0].Rows)
                {
                    foreach (System.Data.DataColumn c in v_excel.v_set.Tables[0].Columns)
                    {
                        if (p_insert.Exists(c.ColumnName))
                            p_insert.SetValue(c.ColumnName, r[c].ToString());
                    }

                    v_insert = p_insert.GetUpdatedText();
                    try
                    {
                        this.Execute(v_insert);
                        v_transfered++;
                        p_progress.FireEvent(v_transfered);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_error.FireEvent(v_insert + "\n" + e.v_message);
                    }
                }

                return v_transfered;
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
            catch (Spartacus.Database.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_excel != null)
                {
                    v_excel.Clear();
                    v_excel = null;
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo Excel para o banco de dados atual.
        /// Conexão com o banco atual precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo de origem.</param>
        /// <param name="p_newtable">Nome da nova tabela a ser criada no banco de dados.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, string p_newtable, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            Spartacus.Database.Command v_cmd;
            Spartacus.Utils.Excel v_excel = null;
            uint v_transfered = 0;
            string v_createtable;
            string v_insert;

            try
            {
                v_excel = new Spartacus.Utils.Excel();
                v_excel.Import(p_filename);

                v_createtable = "create table " + p_newtable + " (";
                for (int k = 0; k < v_excel.v_set.Tables[0].Columns.Count; k++)
                {
                    if (k < v_excel.v_set.Tables[0].Columns.Count-1)
                        v_createtable += v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + " text,";
                    else
                        v_createtable += v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + " text)";
                }
                try
                {
                    this.Execute(v_createtable);
                }
                catch (Spartacus.Database.Exception e)
                {
                    p_error.FireEvent(v_createtable + "\n" + e.v_message);
                }

                v_cmd = new Spartacus.Database.Command();
                v_cmd.v_text = "insert into " + p_newtable + " values (";
                for (int k = 0; k < v_excel.v_set.Tables[0].Columns.Count; k++)
                {
                    if (k < v_excel.v_set.Tables[0].Columns.Count-1)
                        v_cmd.v_text += "#" + v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + "#,";
                    else
                        v_cmd.v_text += "#" + v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + "#)";
                    v_cmd.AddParameter(v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower(), Spartacus.Database.Type.QUOTEDSTRING);
                }

                foreach (System.Data.DataRow r in v_excel.v_set.Tables[0].Rows)
                {
                    foreach (System.Data.DataColumn c in v_excel.v_set.Tables[0].Columns)
                        v_cmd.SetValue(c.ColumnName, r[c].ToString());

                    v_insert = v_cmd.GetUpdatedText();
                    try
                    {
                        this.Execute(v_insert);
                        v_transfered++;
                        p_progress.FireEvent(v_transfered);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_error.FireEvent(v_insert + "\n" + e.v_message);
                    }
                }

                return v_transfered;
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
            catch (Spartacus.Database.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_excel != null)
                {
                    v_excel.Clear();
                    v_excel = null;
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo Excel para o banco de dados atual.
        /// Conexão com o banco atual precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo de origem.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_header">Se deve considerar a primeira linha como cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação para leitura do arquivo CSV.</param>
        /// <param name="p_newtable">Nome da nova tabela a ser criada no banco de dados.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, char p_separator, bool p_header, System.Text.Encoding p_encoding, string p_newtable, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            Spartacus.Database.Command v_cmd;
            Spartacus.Utils.Excel v_excel = null;
            uint v_transfered = 0;
            string v_createtable;
            string v_insert;

            try
            {
                v_excel = new Spartacus.Utils.Excel();
                v_excel.Import(p_filename, p_separator, p_header, p_encoding);

                v_createtable = "create table " + p_newtable + " (";
                for (int k = 0; k < v_excel.v_set.Tables[0].Columns.Count; k++)
                {
                    if (k < v_excel.v_set.Tables[0].Columns.Count-1)
                        v_createtable += v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + " text,";
                    else
                        v_createtable += v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + " text)";
                }
                try
                {
                    this.Execute(v_createtable);
                }
                catch (Spartacus.Database.Exception e)
                {
                    p_error.FireEvent(v_createtable + "\n" + e.v_message);
                }

                v_cmd = new Spartacus.Database.Command();
                v_cmd.v_text = "insert into " + p_newtable + " values (";
                for (int k = 0; k < v_excel.v_set.Tables[0].Columns.Count; k++)
                {
                    if (k < v_excel.v_set.Tables[0].Columns.Count-1)
                        v_cmd.v_text += "#" + v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + "#,";
                    else
                        v_cmd.v_text += "#" + v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + "#)";
                    v_cmd.AddParameter(v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower(), Spartacus.Database.Type.QUOTEDSTRING);
                }

                foreach (System.Data.DataRow r in v_excel.v_set.Tables[0].Rows)
                {
                    foreach (System.Data.DataColumn c in v_excel.v_set.Tables[0].Columns)
                        v_cmd.SetValue(c.ColumnName, r[c].ToString());

                    v_insert = v_cmd.GetUpdatedText();
                    try
                    {
                        this.Execute(v_insert);
                        v_transfered++;
                        p_progress.FireEvent(v_transfered);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_error.FireEvent(v_insert + "\n" + e.v_message);
                    }
                }

                return v_transfered;
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
            catch (Spartacus.Database.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_excel != null)
                {
                    v_excel.Clear();
                    v_excel = null;
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo Excel para o banco de dados atual.
        /// Conexão com o banco atual precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo de origem.</param>
        /// <param name="p_separator">Separador de campos do arquivo CSV.</param>
        /// <param name="p_delimitator">Delimitador de campos do arquivo CSV.</param>
        /// <param name="p_header">Se deve considerar a primeira linha como cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação para leitura do arquivo CSV.</param>
        /// <param name="p_newtable">Nome da nova tabela a ser criada no banco de dados.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, char p_separator, char p_delimitator, bool p_header, System.Text.Encoding p_encoding, string p_newtable, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            Spartacus.Database.Command v_cmd;
            Spartacus.Utils.Excel v_excel = null;
            uint v_transfered = 0;
            string v_createtable;
            string v_insert;

            try
            {
                v_excel = new Spartacus.Utils.Excel();
                v_excel.Import(p_filename, p_separator, p_delimitator, p_header, p_encoding);

                v_createtable = "create table " + p_newtable + " (";
                for (int k = 0; k < v_excel.v_set.Tables[0].Columns.Count; k++)
                {
                    if (k < v_excel.v_set.Tables[0].Columns.Count-1)
                        v_createtable += v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + " text,";
                    else
                        v_createtable += v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + " text)";
                }
                try
                {
                    this.Execute(v_createtable);
                }
                catch (Spartacus.Database.Exception e)
                {
                    p_error.FireEvent(v_createtable + "\n" + e.v_message);
                }

                v_cmd = new Spartacus.Database.Command();
                v_cmd.v_text = "insert into " + p_newtable + " values (";
                for (int k = 0; k < v_excel.v_set.Tables[0].Columns.Count; k++)
                {
                    if (k < v_excel.v_set.Tables[0].Columns.Count-1)
                        v_cmd.v_text += "#" + v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + "#,";
                    else
                        v_cmd.v_text += "#" + v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower() + "#)";
                    v_cmd.AddParameter(v_excel.v_set.Tables[0].Columns[k].ColumnName.ToLower(), Spartacus.Database.Type.QUOTEDSTRING);
                }

                foreach (System.Data.DataRow r in v_excel.v_set.Tables[0].Rows)
                {
                    foreach (System.Data.DataColumn c in v_excel.v_set.Tables[0].Columns)
                        v_cmd.SetValue(c.ColumnName, r[c].ToString());

                    v_insert = v_cmd.GetUpdatedText();
                    try
                    {
                        this.Execute(v_insert);
                        v_transfered++;
                        p_progress.FireEvent(v_transfered);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_error.FireEvent(v_insert + "\n" + e.v_message);
                    }
                }

                return v_transfered;
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
            catch (Spartacus.Database.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_excel != null)
                {
                    v_excel.Clear();
                    v_excel = null;
                }
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo do Excel.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual para obter os dados.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        public uint TransferToFile(string p_query, string p_filename)
        {
            Spartacus.Utils.Excel v_excel = null;
            System.Data.DataTable v_table;

            try
            {
                v_excel = new Spartacus.Utils.Excel();

                v_table = this.Query(p_query, "TRANSFER");

                if (v_table != null && v_table.Rows.Count > 0)
                {
                    v_excel.v_set.Tables.Add(v_table);
                    v_excel.Export(p_filename);

                    return (uint) v_table.Rows.Count;
                }
                else
                    return 0;
            }
            catch (Spartacus.Utils.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
            catch (Spartacus.Database.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_excel != null)
                {
                    v_excel.Clear();
                    v_excel = null;
                }
            }
        }

        /// <summary>
        /// Fix temporário para um problema de DataColumn.ColumnName que apareceu no Mono 4
        /// </summary>
        /// <returns>Nome da coluna corrigido.</returns>
        /// <param name="p_input">Nome da coluna com problema.</param>
        public string FixColumnName(string p_input)
        {
            string v_output;
            char[] v_array;
            int k;

            v_array = p_input.ToCharArray();

            v_output = "";
            k = 0;
            while (k < v_array.Length && ((uint)v_array[k]) != 0)
            {
                v_output += v_array[k];
                k++;
            }

            return v_output;
        }

        /// <summary>
        /// Configura CommandTimeout de todas as conexões feitas com a instância atual.
        /// </summary>
        /// <param name="p_timeout">Timeout em segundos.</param>
        public void SetTimeout(int p_timeout)
        {
            this.v_timeout = p_timeout;
        }
    }
}

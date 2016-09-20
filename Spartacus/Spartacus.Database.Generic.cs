/*
The MIT License (MIT)

Copyright (c) 2014-2016 William Ivanski

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
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// Timeout de execução de comandos, em segundos.
        /// </summary>
        public int v_timeout;

        /// <summary>
        /// Se deve tratar caracteres inseguros no Execute ou não.
        /// </summary>
        public bool v_execute_security;

        /// <summary>
        /// Tipo de dados padrão para string.
        /// </summary>
        public string v_default_string;

        /// <summary>
        /// Tamanho do bloco para transferências de dados.
        /// </summary>
        public int v_blocksize;


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
            this.v_timeout = -1;
            this.v_execute_security = true;
            this.v_blocksize = 100;
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
            this.v_timeout = -1;
            this.v_execute_security = true;
            this.v_blocksize = 100;
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
            this.v_timeout = -1;
            this.v_execute_security = true;
            this.v_blocksize = 100;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// </summary>
        public Generic()
        {
            this.v_timeout = -1;
            this.v_execute_security = true;
            this.v_blocksize = 100;
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
        public abstract System.Data.DataTable QueryBlock(string p_sql, string p_tablename, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

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
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma lista de objetos customizados.
        /// Utiliza um DataReader para buscar em blocos.
        /// </summary>
        /// <param name="p_sql">
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <typeparam name="T">
        /// Tipo do objeto customizado (classe com propriedades).
        /// </typeparam>
        public abstract System.Collections.Generic.List<T> QueryList<T>(string p_sql);

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma lista de listas de string.
        /// Utiliza um DataReader para buscar em blocos.
        /// </summary>
        /// <param name="p_sql">
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        public abstract System.Collections.Generic.List<System.Collections.Generic.List<string>> QuerySList(string p_sql);

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
        /// Configura CommandTimeout de todas as conexões feitas com a instância atual.
        /// </summary>
        /// <param name="p_timeout">Timeout em segundos.</param>
        public void SetTimeout(int p_timeout)
        {
            this.v_timeout = p_timeout;
        }

        /// <summary>
        /// Configura Execute Security de todas as conexões feitas com a instância atual.
        /// </summary>
        /// <param name="p_execute_security">Se deve tratar caracteres inseguros no Execute ou não.</param>
        public void SetExecuteSecurity(bool p_execute_security)
        {
            this.v_execute_security = p_execute_security;
        }

        /// <summary>
        /// Configura o tipo de dados padrão para string.
        /// </summary>
        /// <param name="p_default_string">Tipo de dados padrão para string.</param>
        public void SetDefaultString(string p_default_string)
        {
            this.v_default_string = p_default_string;
        }

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
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_table">Nome da tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas da tabela de destino.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public abstract uint Transfer(string p_query, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_table">Nome da tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas da tabela de destino.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_log">Log de inserção.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public abstract uint Transfer(string p_query, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

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
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino precisa existir.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            System.IO.FileInfo v_fileinfo;
            Spartacus.Utils.File v_file;

            v_fileinfo = new System.IO.FileInfo(p_filename);

            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Database.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }
            else
            {
                try
                {
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                    switch (v_file.v_extension.ToLower())
                    {
                        case "csv":
                            return this.TransferFromCSV(p_filename, ";", "\"", true, System.Text.Encoding.Default, p_table, p_columns, p_insert, p_progress, p_error);
                        case "xlsx":
                            return this.TransferFromXLSX(p_filename, p_table, p_columns, p_insert, p_progress, p_error);
                        case "dbf":
                            return this.TransferFromDBF(p_filename, p_table, p_columns, p_insert, p_progress, p_error);
                        default:
                            throw new Spartacus.Database.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                    }
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception("Erro ao transferir dados do arquivo {0}.", e, p_filename);
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino será criada.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, string p_table, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            System.IO.FileInfo v_fileinfo;
            Spartacus.Utils.File v_file;

            v_fileinfo = new System.IO.FileInfo(p_filename);

            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Database.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }
            else
            {
                try
                {
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                    switch (v_file.v_extension.ToLower())
                    {
                        case "csv":
                            return this.TransferFromCSV(p_filename, ";", "\"", true, System.Text.Encoding.Default, p_table, p_progress, p_error);
                        case "xlsx":
                            return this.TransferFromXLSX(p_filename, p_table, p_progress, p_error);
                        case "dbf":
                            return this.TransferFromDBF(p_filename, p_table, p_progress, p_error);
                        default:
                            throw new Spartacus.Database.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                    }
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception("Erro ao transferir dados do arquivo {0}.", e, p_filename);
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino precisa existir.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_separator">Separador de campos.</param>
        /// <param name="p_delimiter">Delimitador de string.</param>
        /// <param name="p_header">Se a primeira linha é cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            System.IO.FileInfo v_fileinfo;
            Spartacus.Utils.File v_file;

            v_fileinfo = new System.IO.FileInfo(p_filename);

            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Database.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }
            else
            {
                try
                {
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                    switch (v_file.v_extension.ToLower())
                    {
                        case "csv":
                            return this.TransferFromCSV(p_filename, p_separator, p_delimiter, p_header, p_encoding, p_table, p_columns, p_insert, p_progress, p_error);
                        case "xlsx":
                            return this.TransferFromXLSX(p_filename, p_table, p_columns, p_insert, p_progress, p_error);
                        case "dbf":
                            return this.TransferFromDBF(p_filename, p_table, p_columns, p_insert, p_progress, p_error);
                        default:
                            throw new Spartacus.Database.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                    }
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception("Erro ao transferir dados do arquivo {0}.", e, p_filename);
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino será criada.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_separator">Separador de campos.</param>
        /// <param name="p_delimiter">Delimitador de string.</param>
        /// <param name="p_header">Se a primeira linha é cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public uint TransferFromFile(string p_filename, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding, string p_table, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            System.IO.FileInfo v_fileinfo;
            Spartacus.Utils.File v_file;

            v_fileinfo = new System.IO.FileInfo(p_filename);

            if (! v_fileinfo.Exists)
            {
                throw new Spartacus.Database.Exception(string.Format("Arquivo {0} nao existe.", p_filename));
            }
            else
            {
                try
                {
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                    switch (v_file.v_extension.ToLower())
                    {
                        case "csv":
                            return this.TransferFromCSV(p_filename, p_separator, p_delimiter, p_header, p_encoding, p_table, p_progress, p_error);
                        case "xlsx":
                            return this.TransferFromXLSX(p_filename, p_table, p_progress, p_error);
                        case "dbf":
                            return this.TransferFromDBF(p_filename, p_table, p_progress, p_error);
                        default:
                            throw new Spartacus.Database.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                    }
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception("Erro ao transferir dados do arquivo {0}.", e, p_filename);
                }
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino precisa existir.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_separator">Separador de campos.</param>
        /// <param name="p_delimiter">Delimitador de string.</param>
        /// <param name="p_header">Se a primeira linha é cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        private uint TransferFromCSV(string p_filename, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            uint v_transfered;
            System.IO.StreamReader v_reader = null;
            System.Collections.Generic.List<string> v_block, v_columns;
            string[] v_row;
            string v_tmp;
            int i, j, k;

            try
            {
                v_reader = new System.IO.StreamReader(p_filename, p_encoding);

                v_block = new System.Collections.Generic.List<string>();
                v_columns = new System.Collections.Generic.List<string>();

                v_transfered = 0;
                i = 0;
                k = 0;
                while (! v_reader.EndOfStream)
                {
                    v_tmp = v_reader.ReadLine();
                    v_row = v_tmp.Split(new string[]{p_delimiter + p_separator + p_delimiter}, System.StringSplitOptions.None);

                    if (v_row.Length == 1)
                    {
                        if (i == 0)
                        {
                            if (p_header)
                                v_columns.Add(v_row[0].Substring(p_delimiter.Length, v_row[0].Length-(p_delimiter.Length*2)));
                            else
                            {
                                v_columns.Add("col0");

                                p_insert.SetValue(0, v_row[0].Substring(p_delimiter.Length, v_row[0].Length-(p_delimiter.Length*2)));

                                v_block.Add(p_insert.GetUpdatedText());
                                k++;

                                if (k == this.v_blocksize)
                                {
                                    this.InsertBlock(p_table, v_block, p_columns);
                                    v_transfered += (uint) v_block.Count;
                                    p_progress.FireEvent(v_transfered);

                                    v_block.Clear();
                                    k = 0;
                                }
                            }
                        }
                        else
                        {
                            if (v_row.Length == v_columns.Count)
                            {
                                p_insert.SetValue(0, v_row[0].Substring(p_delimiter.Length, v_row[0].Length-(p_delimiter.Length*2)));

                                v_block.Add(p_insert.GetUpdatedText());
                                k++;

                                if (k == this.v_blocksize)
                                {
                                    this.InsertBlock(p_table, v_block, p_columns);
                                    v_transfered += (uint) v_block.Count;
                                    p_progress.FireEvent(v_transfered);

                                    v_block.Clear();
                                    k = 0;
                                }
                            }
                            else
                                p_error.FireEvent("Número de colunas inesperado: " + v_row.Length + ", deveria ser " + v_columns.Count);
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            if (p_header)
                            {
                                v_columns.Add(v_row[0].Substring(p_delimiter.Length));
                                for (j = 1; j < v_row.Length-1; j++)
                                    v_columns.Add(v_row[j]);
                                v_columns.Add(v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length));
                            }
                            else
                            {
                                for (j = 0; j < v_row.Length; j++)
                                    v_columns.Add("col" + j.ToString());

                                p_insert.SetValue(v_columns[0], v_row[0].Substring(p_delimiter.Length));
                                for (j = 1; j < p_insert.v_parameters.Count-1; j++)
                                    p_insert.SetValue(v_columns[j], v_row[j]);
                                p_insert.SetValue(v_columns[v_row.Length-1], v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length));

                                v_block.Add(p_insert.GetUpdatedText());
                                k++;

                                if (k == this.v_blocksize)
                                {
                                    this.InsertBlock(p_table, v_block, p_columns);
                                    v_transfered += (uint) v_block.Count;
                                    p_progress.FireEvent(v_transfered);

                                    v_block.Clear();
                                    k = 0;
                                }
                            }
                        }
                        else
                        {
                            if (v_row.Length > 0)
                            {
                                if (v_row.Length == v_columns.Count)
                                {
                                    p_insert.SetValue(v_columns[0], v_row[0].Substring(p_delimiter.Length));
                                    for (j = 1; j < p_insert.v_parameters.Count-1; j++)
                                        p_insert.SetValue(v_columns[j], v_row[j]);
                                    p_insert.SetValue(v_columns[v_row.Length-1], v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length));

                                    v_block.Add(p_insert.GetUpdatedText());
                                    k++;

                                    if (k == this.v_blocksize)
                                    {
                                        this.InsertBlock(p_table, v_block, p_columns);
                                        v_transfered += (uint) v_block.Count;
                                        p_progress.FireEvent(v_transfered);

                                        v_block.Clear();
                                        k = 0;
                                    }
                                }
                                else
                                    p_error.FireEvent("Número de colunas inesperado: " + v_row.Length + ", deveria ser " + v_columns.Count);
                            }
                        }
                    }

                    i++;
                }

                if (v_block.Count > 0)
                {
                    this.InsertBlock(p_table, v_block, p_columns);
                    v_transfered += (uint) v_block.Count;
                    p_progress.FireEvent(v_transfered);
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_reader != null)
                    v_reader.Close();
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino será criada.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_separator">Separador de campos.</param>
        /// <param name="p_delimiter">Delimitador de string.</param>
        /// <param name="p_header">Se a primeira linha é cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        private uint TransferFromCSV(string p_filename, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding, string p_table, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            uint v_transfered;
            System.IO.StreamReader v_reader = null;
            System.Collections.Generic.List<string> v_block, v_columns;
            Spartacus.Database.Command v_insert;
            string[] v_row;
            string v_createtable, v_tmp;
            int i, j, k;

            try
            {
                v_reader = new System.IO.StreamReader(p_filename, p_encoding);

                v_block = new System.Collections.Generic.List<string>();
                v_columns = new System.Collections.Generic.List<string>();
                v_insert = new Spartacus.Database.Command();

                v_transfered = 0;
                i = 0;
                k = 0;
                while (! v_reader.EndOfStream)
                {
                    v_tmp = v_reader.ReadLine();
                    v_row = v_tmp.Split(new string[]{p_delimiter + p_separator + p_delimiter}, System.StringSplitOptions.None);

                    if (v_row.Length == 1)
                    {
                        if (i == 0)
                        {
                            if (p_header)
                            {
                                v_insert.v_text = "(#" + v_row[0].Substring(p_delimiter.Length, v_row[0].Length-(p_delimiter.Length*2)) + "#)";
                                v_insert.AddParameter(v_row[0].Substring(p_delimiter.Length, v_row[0].Length-(p_delimiter.Length*2)), Spartacus.Database.Type.QUOTEDSTRING);
                                v_createtable = "create table " + p_table + " (" + v_row[0].Substring(p_delimiter.Length, v_row[0].Length-(p_delimiter.Length*2)) + " " + this.v_default_string + ")";
                                v_columns.Add(v_row[0].Substring(p_delimiter.Length));

                                this.Execute(v_createtable);
                            }
                            else
                            {
                                v_insert.v_text = "(#col0#)";
                                v_insert.AddParameter("col0", Spartacus.Database.Type.QUOTEDSTRING);
                                v_createtable = "create table " + p_table + " (col0 " + this.v_default_string + ")";
                                v_columns.Add("col0");

                                this.Execute(v_createtable);

                                v_insert.SetValue(0, v_row[0].Substring(p_delimiter.Length, v_row[0].Length-(p_delimiter.Length*2)));

                                v_block.Add(v_insert.GetUpdatedText());
                                k++;

                                if (k == this.v_blocksize)
                                {
                                    this.InsertBlock(p_table, v_block);
                                    v_transfered += (uint) v_block.Count;
                                    p_progress.FireEvent(v_transfered);

                                    v_block.Clear();
                                    k = 0;
                                }
                            }
                        }
                        else
                        {
                            if (v_row.Length == v_columns.Count)
                            {
                                v_insert.SetValue(0, v_row[0].Substring(p_delimiter.Length, v_row[0].Length-(p_delimiter.Length*2)));

                                v_block.Add(v_insert.GetUpdatedText());
                                k++;

                                if (k == this.v_blocksize)
                                {
                                    this.InsertBlock(p_table, v_block);
                                    v_transfered += (uint) v_block.Count;
                                    p_progress.FireEvent(v_transfered);

                                    v_block.Clear();
                                    k = 0;
                                }
                            }
                            else
                                p_error.FireEvent("Número de colunas inesperado: " + v_row.Length + ", deveria ser " + v_columns.Count);
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            if (p_header)
                            {
                                v_insert.v_text = "(#" + v_row[0].Substring(p_delimiter.Length) + "#";
                                v_insert.AddParameter(v_row[0].Substring(p_delimiter.Length), Spartacus.Database.Type.QUOTEDSTRING);
                                v_createtable = "create table " + p_table + " (" + v_row[0].Substring(p_delimiter.Length) + " " + this.v_default_string;
                                v_columns.Add(v_row[0].Substring(p_delimiter.Length));

                                for (j = 1; j < v_row.Length-1; j++)
                                {
                                    v_insert.v_text += ",#" + v_row[j] + "#";
                                    v_insert.AddParameter(v_row[j], Spartacus.Database.Type.QUOTEDSTRING);
                                    v_createtable += "," + v_row[j] + " " + this.v_default_string;
                                    v_columns.Add(v_row[j]);
                                }

                                v_insert.v_text += ",#" + v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length) + "#)";
                                v_insert.AddParameter(v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length), Spartacus.Database.Type.QUOTEDSTRING);
                                v_createtable += "," + v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length) + " " + this.v_default_string + ")";
                                v_columns.Add(v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length));

                                this.Execute(v_createtable);
                            }
                            else
                            {
                                v_insert.v_text = "(#col0#";
                                v_insert.AddParameter("col0", Spartacus.Database.Type.QUOTEDSTRING);
                                v_createtable = "create table " + p_table + " (col0 " + this.v_default_string;
                                v_columns.Add("col0");

                                for (j = 1; j < v_row.Length-1; j++)
                                {
                                    v_insert.v_text += ",#col" + j.ToString() + "#";
                                    v_insert.AddParameter("col" + j.ToString(), Spartacus.Database.Type.QUOTEDSTRING);
                                    v_createtable += ",col" + j.ToString() + " " + this.v_default_string;
                                    v_columns.Add("col" + j.ToString());
                                }

                                v_insert.v_text += ",#col" + (v_row.Length-1).ToString() + "#)";
                                v_insert.AddParameter("col" + (v_row.Length-1).ToString(), Spartacus.Database.Type.QUOTEDSTRING);
                                v_createtable += ",col" + (v_row.Length-1).ToString() + " " + this.v_default_string + ")";
                                v_columns.Add("col" + (v_row.Length-1).ToString());

                                this.Execute(v_createtable);

                                v_insert.SetValue(v_columns[0], v_row[0].Substring(p_delimiter.Length));
                                for (j = 1; j < v_insert.v_parameters.Count-1; j++)
                                    v_insert.SetValue(v_columns[j], v_row[j]);
                                v_insert.SetValue(v_columns[v_row.Length-1], v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length));

                                v_block.Add(v_insert.GetUpdatedText());
                                k++;

                                if (k == this.v_blocksize)
                                {
                                    this.InsertBlock(p_table, v_block);
                                    v_transfered += (uint) v_block.Count;
                                    p_progress.FireEvent(v_transfered);

                                    v_block.Clear();
                                    k = 0;
                                }
                            }
                        }
                        else
                        {
                            if (v_row.Length > 0)
                            {
                                if (v_row.Length == v_columns.Count)
                                {
                                    v_insert.SetValue(v_columns[0], v_row[0].Substring(p_delimiter.Length));
                                    for (j = 1; j < v_insert.v_parameters.Count-1; j++)
                                        v_insert.SetValue(v_columns[j], v_row[j]);
                                    v_insert.SetValue(v_columns[v_row.Length-1], v_row[v_row.Length-1].Substring(0, v_row[v_row.Length-1].Length-p_delimiter.Length));

                                    v_block.Add(v_insert.GetUpdatedText());
                                    k++;

                                    if (k == this.v_blocksize)
                                    {
                                        this.InsertBlock(p_table, v_block);
                                        v_transfered += (uint) v_block.Count;
                                        p_progress.FireEvent(v_transfered);

                                        v_block.Clear();
                                        k = 0;
                                    }
                                }
                                else
                                    p_error.FireEvent("Número de colunas inesperado: " + v_row.Length + ", deveria ser " + v_columns.Count);
                            }
                        }
                    }

                    i++;
                }

                if (v_block.Count > 0)
                {
                    this.InsertBlock(p_table, v_block);
                    v_transfered += (uint) v_block.Count;
                    p_progress.FireEvent(v_transfered);
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                if (v_reader != null)
                    v_reader.Close();
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino precisa existir.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        private uint TransferFromXLSX(string p_filename, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            uint v_transfered;
            Spartacus.ThirdParty.SejExcel.OoXml v_package = null;
            Spartacus.ThirdParty.SejExcel.gSheet v_sheet;
            System.Collections.Generic.List<string> v_columns, v_block;
            string[] v_row = null;
            bool v_firstrow = true;
            bool v_datanode = false;
            bool v_istext = false;
            string v_cellcontent, v_key = "";
            double v_value;
            int v_col = -1;
            int j, k;

            try
            {
                v_package = new Spartacus.ThirdParty.SejExcel.OoXml(p_filename);

                if (v_package != null && v_package.sheets != null && v_package.sheets.Count == 1)
                {
                    foreach (string s in v_package.sheets.Keys)
                        v_key = s;
                    v_sheet = v_package.sheets[v_key];

                    v_columns = new System.Collections.Generic.List<string>();
                    v_block = new System.Collections.Generic.List<string>();

                    v_transfered = 0;
                    k = 0;
                    using (System.Xml.XmlReader v_reader = System.Xml.XmlReader.Create(v_sheet.GetStream()))
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
                                                v_col = -1;
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
                                        {
                                            for (j = 0; j < p_insert.v_parameters.Count; j++)
                                                p_insert.SetValue(j, v_row[v_columns.IndexOf(p_insert.v_parameters[j].v_name)]);

                                            v_block.Add(p_insert.GetUpdatedText());
                                            k++;

                                            if (k == this.v_blocksize)
                                            {
                                                this.InsertBlock(p_table, v_block, p_columns);
                                                v_transfered += (uint) v_block.Count;
                                                p_progress.FireEvent(v_transfered);

                                                v_block.Clear();
                                                k = 0;
                                            }
                                        }
                                    }
                                    break;
                                case System.Xml.XmlNodeType.Text:
                                    if (v_datanode)
                                    {
                                        if (v_istext)
                                            v_cellcontent = v_package.words[int.Parse(v_reader.Value)];
                                        else
                                            v_cellcontent = v_reader.Value;
                                        if (v_firstrow)
                                        {
                                            v_columns.Add(v_cellcontent);
                                            v_row = new string[v_columns.Count];
                                        }
                                        else
                                        {
                                            if (double.TryParse(v_cellcontent, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out v_value))
                                                v_row[v_col] = System.Math.Round(v_value, 8).ToString();
                                            else
                                                v_row[v_col] = v_cellcontent;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                    throw new Spartacus.Database.Exception("Arquivo {0} nao pode ser aberto, nao contem planilhas com dados ou contem mais de uma planilha.", p_filename);

                if (v_block.Count > 0)
                {
                    this.InsertBlock(p_table, v_block, p_columns);
                    v_transfered += (uint) v_block.Count;
                    p_progress.FireEvent(v_transfered);
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw e;
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
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino será criada.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        private uint TransferFromXLSX(string p_filename, string p_table, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            uint v_transfered;
            Spartacus.ThirdParty.SejExcel.OoXml v_package = null;
            Spartacus.ThirdParty.SejExcel.gSheet v_sheet;
            System.Collections.Generic.List<string> v_columns, v_block;
            Spartacus.Database.Command v_insert;
            string[] v_row = null;
            bool v_firstrow = true;
            bool v_datanode = false;
            bool v_istext = false;
            string v_cellcontent, v_key = "", v_createtable;
            double v_value;
            int v_col = -1;
            int j, k;

            try
            {
                v_package = new Spartacus.ThirdParty.SejExcel.OoXml(p_filename);

                if (v_package != null && v_package.sheets != null && v_package.sheets.Count == 1)
                {
                    foreach (string s in v_package.sheets.Keys)
                        v_key = s;
                    v_sheet = v_package.sheets[v_key];

                    v_columns = new System.Collections.Generic.List<string>();
                    v_block = new System.Collections.Generic.List<string>();
                    v_insert = new Spartacus.Database.Command();

                    v_transfered = 0;
                    k = 0;
                    using (System.Xml.XmlReader v_reader = System.Xml.XmlReader.Create(v_sheet.GetStream()))
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
                                                v_col = -1;
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
                                        {
                                            if (v_columns.Count == 1)
                                            {
                                                v_insert.v_text = "(#" + v_columns[0] + "#)";
                                                v_insert.AddParameter(v_columns[0], Spartacus.Database.Type.QUOTEDSTRING);
                                                v_createtable = "create table " + p_table + "(" + v_columns[0] + " " + this.v_default_string + ")";
                                            }
                                            else
                                            {
                                                v_insert.v_text = "(#" + v_columns[0] + "#";
                                                v_insert.AddParameter(v_columns[0], Spartacus.Database.Type.QUOTEDSTRING);
                                                v_createtable = "create table " + p_table + "(" + v_columns[0] + " " + this.v_default_string;

                                                for (j = 1; j < v_columns.Count-1; j++)
                                                {
                                                    v_insert.v_text += ",#" + v_columns[j] + "#";
                                                    v_insert.AddParameter(v_columns[j], Spartacus.Database.Type.QUOTEDSTRING);
                                                    v_createtable += "," + v_columns[j] + " " + this.v_default_string;
                                                }

                                                v_insert.v_text += ",#" + v_columns[v_columns.Count-1] + "#)";
                                                v_insert.AddParameter(v_columns[v_columns.Count-1], Spartacus.Database.Type.QUOTEDSTRING);
                                                v_createtable += "," + v_columns[v_columns.Count-1] + " " + this.v_default_string + ")";
                                            }

                                            this.Execute(v_createtable);

                                            v_firstrow = false;
                                        }
                                        else
                                        {
                                            for (j = 0; j < v_insert.v_parameters.Count; j++)
                                                v_insert.SetValue(j, v_row[v_columns.IndexOf(v_insert.v_parameters[j].v_name)]);

                                            v_block.Add(v_insert.GetUpdatedText());
                                            k++;

                                            if (k == this.v_blocksize)
                                            {
                                                this.InsertBlock(p_table, v_block);
                                                v_transfered += (uint) v_block.Count;
                                                p_progress.FireEvent(v_transfered);

                                                v_block.Clear();
                                                k = 0;
                                            }
                                        }
                                    }
                                    break;
                                case System.Xml.XmlNodeType.Text:
                                    if (v_datanode)
                                    {
                                        if (v_istext)
                                            v_cellcontent = v_package.words[int.Parse(v_reader.Value)];
                                        else
                                            v_cellcontent = v_reader.Value;
                                        if (v_firstrow)
                                        {
                                            v_columns.Add(v_cellcontent);
                                            v_row = new string[v_columns.Count];
                                        }
                                        else
                                        {
                                            if (double.TryParse(v_cellcontent, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out v_value))
                                                v_row[v_col] = System.Math.Round(v_value, 8).ToString();
                                            else
                                                v_row[v_col] = v_cellcontent;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                    throw new Spartacus.Database.Exception("Arquivo {0} nao pode ser aberto, nao contem planilhas com dados ou contem mais de uma planilha.", p_filename);

                if (v_block.Count > 0)
                {
                    this.InsertBlock(p_table, v_block);
                    v_transfered += (uint) v_block.Count;
                    p_progress.FireEvent(v_transfered);
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw e;
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
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino precisa existir.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        private uint TransferFromDBF(string p_filename, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            uint v_transfered;
            System.Collections.Generic.List<string> v_block, v_columns;
            SocialExplorer.IO.FastDBF.DbfFile v_dbf;
            SocialExplorer.IO.FastDBF.DbfRecord v_record;
            int j, k;

            try
            {
                v_dbf = new SocialExplorer.IO.FastDBF.DbfFile(System.Text.Encoding.UTF8);
                v_dbf.Open(p_filename, System.IO.FileMode.Open);

                v_block = new System.Collections.Generic.List<string>();
                v_columns = new System.Collections.Generic.List<string>();

                for (j = 0; j < v_dbf.Header.ColumnCount; j++)
                {
                    if (v_dbf.Header[j].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Binary &&
                        v_dbf.Header[j].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Memo)
                        v_columns.Add(v_dbf.Header[j].Name);
                }

                v_transfered = 0;
                k = 0;
                v_record = new SocialExplorer.IO.FastDBF.DbfRecord(v_dbf.Header);
                while (v_dbf.ReadNext(v_record))
                {
                    if (! v_record.IsDeleted)
                    {
                        for (j = 0; j < v_record.ColumnCount; j++)
                        {
                            if (v_dbf.Header[j].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Binary &&
                                v_dbf.Header[j].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Memo)
                                p_insert.SetValue(v_columns[j], v_record[j].Trim());
                        }

                        v_block.Add(p_insert.GetUpdatedText());
                        k++;

                        if (k == this.v_blocksize)
                        {
                            this.InsertBlock(p_table, v_block, p_columns);
                            v_transfered += (uint) v_block.Count;
                            p_progress.FireEvent(v_transfered);

                            v_block.Clear();
                            k = 0;
                        }
                    }
                }

                v_dbf.Close();

                if (v_block.Count > 0)
                {
                    this.InsertBlock(p_table, v_block, p_columns);
                    v_transfered += (uint) v_block.Count;
                    p_progress.FireEvent(v_transfered);
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Transfere dados de um arquivo para o banco de dados atual.
        /// Conexão com o banco de dados atual precisa estar aberta. Tabela de destino será criada.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_filename">Nome do arquivo.</param>
        /// <param name="p_table">Tabela de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        private uint TransferFromDBF(string p_filename, string p_table, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            uint v_transfered;
            System.Collections.Generic.List<string> v_block, v_columns;
            Spartacus.Database.Command v_insert;
            SocialExplorer.IO.FastDBF.DbfFile v_dbf;
            SocialExplorer.IO.FastDBF.DbfRecord v_record;
            string v_createtable;
            int j, k;
            bool v_first;

            try
            {
                v_dbf = new SocialExplorer.IO.FastDBF.DbfFile(System.Text.Encoding.UTF8);
                v_dbf.Open(p_filename, System.IO.FileMode.Open);

                v_block = new System.Collections.Generic.List<string>();
                v_columns = new System.Collections.Generic.List<string>();
                v_insert = new Spartacus.Database.Command();

                v_insert.v_text = "(";
                v_createtable = "create table " + p_table + " (";
                v_first = true;
                for (j = 0; j < v_dbf.Header.ColumnCount; j++)
                {
                    if (v_dbf.Header[j].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Binary &&
                        v_dbf.Header[j].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Memo)
                    {
                        if (v_first)
                        {
                            v_insert.v_text += "#" + v_dbf.Header[j].Name + "#";
                            v_insert.AddParameter(v_dbf.Header[j].Name, Spartacus.Database.Type.QUOTEDSTRING);
                            v_createtable += v_dbf.Header[j].Name + " " + this.v_default_string;
                            v_columns.Add(v_dbf.Header[j].Name);
                            v_first = false;
                        }
                        else
                        {
                            v_insert.v_text += ",#" + v_dbf.Header[j].Name + "#";
                            v_insert.AddParameter(v_dbf.Header[j].Name, Spartacus.Database.Type.QUOTEDSTRING);
                            v_createtable += "," + v_dbf.Header[j].Name + " " + this.v_default_string;
                            v_columns.Add(v_dbf.Header[j].Name);
                        }
                    }
                }
                v_insert.v_text += ")";
                v_createtable += ")";
                this.Execute(v_createtable);

                v_transfered = 0;
                k = 0;
                v_record = new SocialExplorer.IO.FastDBF.DbfRecord(v_dbf.Header);
                while (v_dbf.ReadNext(v_record))
                {
                    if (! v_record.IsDeleted)
                    {
                        for (j = 0; j < v_record.ColumnCount; j++)
                        {
                            if (v_dbf.Header[j].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Binary &&
                                v_dbf.Header[j].ColumnType != SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Memo)
                                v_insert.SetValue(v_columns[j], v_record[j].Trim());
                        }

                        v_block.Add(v_insert.GetUpdatedText());
                        k++;

                        if (k == this.v_blocksize)
                        {
                            this.InsertBlock(p_table, v_block);
                            v_transfered += (uint) v_block.Count;
                            p_progress.FireEvent(v_transfered);

                            v_block.Clear();
                            k = 0;
                        }
                    }
                }

                v_dbf.Close();

                if (v_block.Count > 0)
                {
                    this.InsertBlock(p_table, v_block);
                    v_transfered += (uint) v_block.Count;
                    p_progress.FireEvent(v_transfered);
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        public uint TransferToFile(string p_query, string p_filename)
        {
            Spartacus.Utils.File v_file;

            try
            {
                v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                switch (v_file.v_extension.ToLower())
                {
                    case "csv":
                        return this.TransferToCSV(p_query, p_filename, ";", "\"", true, System.Text.Encoding.Default);
                    case "xlsx":
                        return this.TransferToXLSX(p_query, p_filename);
                    case "dbf":
                        return this.TransferToDBF(p_query, p_filename);
                    default:
                        throw new Spartacus.Database.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                }
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception("Erro ao transferir dados para o arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        /// <param name="p_separator">Separador de campos.</param>
        /// <param name="p_delimiter">Delimitador de string.</param>
        /// <param name="p_header">Se a primeira linha é cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação do arquivo.</param>
        public uint TransferToFile(string p_query, string p_filename, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding)
        {
            Spartacus.Utils.File v_file;

            try
            {
                v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);

                switch (v_file.v_extension.ToLower())
                {
                    case "csv":
                        return this.TransferToCSV(p_query, p_filename, p_separator, p_delimiter, p_header, p_encoding);
                    case "xlsx":
                        return this.TransferToXLSX(p_query, p_filename);
                    case "dbf":
                        return this.TransferToDBF(p_query, p_filename);
                    default:
                        throw new Spartacus.Database.Exception("Extensao {0} desconhecida.", v_file.v_extension.ToLower());
                }
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception("Erro ao transferir dados para o arquivo {0}.", e, p_filename);
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo CSV.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        /// <param name="p_separator">Separador de campos.</param>
        /// <param name="p_delimiter">Delimitador de string.</param>
        /// <param name="p_header">Se a primeira linha é cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação do arquivo.</param>
        public abstract uint TransferToCSV(string p_query, string p_filename, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding);

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo XLSX.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        public abstract uint TransferToXLSX(string p_query, string p_filename);

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo DBF.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        public abstract uint TransferToDBF(string p_query, string p_filename);
    }
}

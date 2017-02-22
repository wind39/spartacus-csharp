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
using System.Data;

namespace SpartacusMin.Database
{
    /// <summary>
    /// Classe abstrata SpartacusMin.Database.Generic.
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
        /// Inicializa uma nova instância da classe <see cref="SpartacusMin.Database.Generic"/>.
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
        /// Inicializa uma nova instância da classe <see cref="SpartacusMin.Database.Generic"/>.
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
        /// Inicializa uma nova instância da classe <see cref="SpartacusMin.Database.Generic"/>.
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
        /// Inicializa uma nova instância da classe <see cref="SpartacusMin.Database.Generic"/>.
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
        public abstract System.Data.DataTable Query(string p_sql, string p_tablename, SpartacusMin.Utils.ProgressEventClass p_progress);

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
		/// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma lista de listas de string.
		/// Utiliza um DataReader para buscar em blocos.
		/// </summary>
		/// <param name="p_sql">
		/// Código SQL a ser consultado no banco de dados.
		/// </param>
		/// <param name="p_header">
		/// Lista de nomes de colunas.
		/// </param>
		public abstract System.Collections.Generic.List<System.Collections.Generic.List<string>> QuerySList(string p_sql, out System.Collections.Generic.List<string> p_header);

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
		/// Lista os campos (ou colunas) de uma determinada consulta.
		/// </summary>
		/// <returns>Vetor de campos.</returns>
		/// <param name="p_query">Consulta SQL.</param>
		public abstract System.Collections.Generic.List<SpartacusMin.Database.Field> GetFields(string p_query);

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
        public abstract uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase);

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
        public abstract uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, out string p_log);

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
        public abstract uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

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
        public abstract uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

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
        public abstract uint Transfer(string p_query, string p_table, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

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
        public abstract uint Transfer(string p_query, string p_table, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

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
        public abstract uint Transfer(string p_query, string p_table, string p_columns, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

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
        public abstract uint Transfer(string p_query, string p_table, string p_columns, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata);

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
        public abstract uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, SpartacusMin.Utils.ProgressEventClass p_progress, SpartacusMin.Utils.ErrorEventClass p_error);
    }
}

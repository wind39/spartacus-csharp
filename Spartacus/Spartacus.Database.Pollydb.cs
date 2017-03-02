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
using Mono.Data.Sqlite;

namespace Spartacus.Database
{
	/// <summary>
	/// Classe Spartacus.Database.PollyDB.
	/// Herda da classe <see cref="Spartacus.Database.Generic"/>.
	/// Utiliza o Mono.Data.Sqlite para acessar arquivos CSV, XLSX e DBF.
	/// </summary>
	public class Pollydb : Spartacus.Database.Generic
	{
		/// <summary>
		/// Limiar para saber se a cache deve ficar em memória ou em disco.
		/// </summary>
		private long v_cachethreshold;

		/// <summary>
		/// Lista de arquivos em cache.
		/// </summary>
		private System.Collections.Generic.List<string> v_tables;

		/// <summary>
		/// Separador de campos.
		/// </summary>
		private string v_separator;

		/// <summary>
		/// Delimitador de campos.
		/// </summary>
		private string v_delimiter;

		/// <summary>
		/// Se a primeira linha do arquivo é cabeçalho ou não.
		/// </summary>
		private bool v_header;

		/// <summary>
		/// Codificação do arquivo.
		/// </summary>
		private System.Text.Encoding v_encoding;

		/// <summary>
		/// Banco de dados SQLite ou Memory temporário.
		/// </summary>
		private Spartacus.Database.Generic v_database;

		/// <summary>
		/// Nome do banco de dados temporário (se for SQLite).
		/// </summary>
		private string v_tempdatabase;

		/// <summary>
		/// Nome da tabela que será excluída.
		/// </summary>
		private string v_tabletodrop;


		/// <summary>
		/// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Pollydb"/>.
		/// </summary>
		/// <param name='p_directory'>
		/// Caminho para o diretório onde estão os arquivos CSV, DBF e XLSX.
		/// </param>
		public Pollydb(string p_directory)
			: base(p_directory)
		{
			this.v_separator = ";";
			this.v_delimiter = "\"";
			this.v_header = true;
			this.v_encoding = System.Text.Encoding.Default;
			this.v_tables = new System.Collections.Generic.List<string>();
			this.v_tempdatabase = "";

			this.v_connectionstring = p_directory;
			this.v_default_string = "text";
			this.v_cachethreshold = 268435456; // 250 MB
		}

		/// <summary>
		/// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Pollydb"/>.
		/// </summary>
		/// <param name='p_directory'>
		/// Caminho para o diretório onde estão os arquivos CSV, DBF e XLSX.
		/// </param>
		/// <param name="p_cachethreshold">
		/// Limiar para saber se a cache deve ser construída em memória ou em disco.
		/// </param>
		public Pollydb(string p_directory, long p_cachethreshold)
			: base(p_directory)
		{
			this.v_separator = ";";
			this.v_delimiter = "\"";
			this.v_header = true;
			this.v_encoding = System.Text.Encoding.Default;
			this.v_tables = new System.Collections.Generic.List<string>();
			this.v_tempdatabase = "";

			this.v_connectionstring = p_directory;
			this.v_default_string = "text";
			this.v_cachethreshold = p_cachethreshold;
		}

		/// <summary>
		/// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Pollydb"/>.
		/// </summary>
		/// <param name='p_directory'>
		/// Caminho para o diretório onde estão os arquivos CSV, DBF e XLSX.
		/// </param>
		/// <param name="p_cachethreshold">
		/// Limiar para saber se a cache deve ser construída em memória ou em disco.
		/// </param>
		/// <param name="p_separator">
		/// Separador de campos.
		/// </param>
		/// <param name="p_delimiter">
		/// Delimitador de string.
		/// </param>
		/// <param name="p_header">
		/// Se a primeira linha é cabeçalho ou não.
		/// </param>
		/// <param name="p_encoding">
		/// Codificação do arquivo.
		/// </param>
		public Pollydb(string p_directory, long p_cachethreshold, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding)
			: base(p_directory)
		{
			this.v_separator = p_separator;
			this.v_delimiter = p_delimiter;
			this.v_header = p_header;
			this.v_encoding = p_encoding;
			this.v_tables = new System.Collections.Generic.List<string>();
			this.v_tempdatabase = "";

			this.v_connectionstring = p_directory;
			this.v_default_string = "text";
			this.v_cachethreshold = p_cachethreshold;
		}

		/// <summary>
		/// Cria um banco de dados.
		/// </summary>
		/// <param name="p_name">Nome do arquivo de banco de dados a ser criado.</param>
		public override void CreateDatabase(string p_name)
		{
			throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Pollydb.CreateDatabase");
		}

		/// <summary>
		/// Cria um banco de dados.
		/// </summary>
		public override void CreateDatabase()
		{
			throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Pollydb.CreateDatabase");
		}

		/// <summary>
		/// Abre a conexão com o banco de dados.
		/// </summary>
		public override void Open()
		{
		}

		/// <summary>
		/// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name='p_sql'>
		/// Código SQL a ser consultado no banco de dados.
		/// </param>
		/// <param name='p_tablename'>
		/// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
		/// </param>
		/// <returns>Retorna uma <see cref="System.Data.DataTable"/> com os dados de retorno da consulta.</returns>
		public override System.Data.DataTable Query(string p_sql, string p_tablename)
		{
			System.Data.DataTable v_table;
			this.BuildCache(p_sql);
			v_table = this.v_database.Query(p_sql, p_tablename);
			this.DestroyCache();
			return v_table;
		}

		/// <summary>
		/// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see cref="System.Data.DataTable"/>.
		/// </summary>
		/// <param name='p_sql'>
		/// Código SQL a ser consultado no banco de dados.
		/// </param>
		/// <param name='p_tablename'>
		/// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
		/// </param>
		/// <param name='p_progress'>Evento de progresso da execução da consulta.</param>
		/// <returns>Retorna uma <see cref="System.Data.DataTable"/> com os dados de retorno da consulta.</returns>
		public override System.Data.DataTable Query(string p_sql, string p_tablename, Spartacus.Utils.ProgressEventClass p_progress)
		{
			System.Data.DataTable v_table;
			this.BuildCache(p_sql);
			v_table = this.v_database.Query(p_sql, p_tablename, p_progress);
			this.DestroyCache();
			return v_table;
		}

		/// <summary>
		/// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
		/// Utiliza um DataReader para buscar em blocos. Conexão com o banco precisa estar aberta.
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
		public override System.Data.DataTable QueryBlock(string p_sql, string p_tablename, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
		{
			System.Data.DataTable v_table;
			this.BuildCache(p_sql);
			v_table = this.v_database.QueryBlock(p_sql, p_tablename, p_startrow, p_endrow, out p_hasmoredata);
			this.DestroyCache();
			return v_table;
		}

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
		public override string QueryHtml(string p_sql, string p_id, string p_options)
		{
			string v_html;
			this.BuildCache(p_sql);
			v_html = this.v_database.QueryHtml(p_sql, p_id, p_options);
			this.DestroyCache();
			return v_html;
		}

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
		public override System.Data.DataTable QueryStoredProc(string p_sql, string p_tablename, string p_outparam)
		{
			throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Pollydb.QueryStoredProc");
		}

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
		public override System.Collections.Generic.List<T> QueryList<T>(string p_sql)
		{
			System.Collections.Generic.List<T> v_list;
			this.BuildCache(p_sql);
			v_list = this.v_database.QueryList<T>(p_sql);
			this.DestroyCache();
			return v_list;
		}

		/// <summary>
		/// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma lista de listas de string.
		/// Utiliza um DataReader para buscar em blocos.
		/// </summary>
		/// <param name="p_sql">
		/// Código SQL a ser consultado no banco de dados.
		/// </param>
		public override System.Collections.Generic.List<System.Collections.Generic.List<string>> QuerySList(string p_sql)
		{
			System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list;
			this.BuildCache(p_sql);
			v_list = this.v_database.QuerySList(p_sql);
			this.DestroyCache();
			return v_list;
		}

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
		public override System.Collections.Generic.List<System.Collections.Generic.List<string>> QuerySList(string p_sql, out System.Collections.Generic.List<string> p_header)
		{
			System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list;
			this.BuildCache(p_sql);
			v_list = this.v_database.QuerySList(p_sql, out p_header);
			this.DestroyCache();
			return v_list;
		}

		/// <summary>
		/// Executa um código SQL no banco de dados.
		/// </summary>
		/// <param name='p_sql'>
		/// Código SQL a ser executado no banco de dados.
		/// </param>
		public override void Execute(string p_sql)
		{
			this.BuildCache(p_sql);
			this.v_database.Execute(p_sql);
			this.UpdateFiles();
			this.DestroyCache();
		}

		/// <summary>
		/// Insere um bloco de linhas em uma determinada tabela.
		/// </summary>
		/// <param name='p_table'>
		/// Nome da tabela a serem inseridas as linhas.
		/// </param>
		/// <param name='p_rows'>
		/// Lista de linhas a serem inseridas na tabela.
		/// </param>
		public override void InsertBlock(string p_table, System.Collections.Generic.List<string> p_rows)
		{
			this.BuildCache(p_table);
			this.v_database.InsertBlock(p_table, p_rows);
			this.UpdateFiles();
			this.DestroyCache();
		}

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
		public override void InsertBlock(string p_table, System.Collections.Generic.List<string> p_rows, string p_columnnames)
		{
			this.BuildCache(p_table);
			this.v_database.InsertBlock(p_table, p_rows, p_columnnames);
			this.UpdateFiles();
			this.DestroyCache();
		}

		/// <summary>
		/// Realiza uma consulta no banco de dados, armazenando um único dado de retorno em uma string.
		/// </summary>
		/// <returns>
		/// string com o dado de retorno.
		/// </returns>
		/// <param name='p_sql'>
		/// Código SQL a ser consultado no banco de dados.
		/// </param>
		public override string ExecuteScalar(string p_sql)
		{
			this.BuildCache(p_sql);
			return this.v_database.ExecuteScalar(p_sql);
			this.DestroyCache();
		}

		/// <summary>
		/// Fecha a conexão com o banco de dados.
		/// </summary>
		public override void Close()
		{
		}

		/// <summary>
		/// Deleta um banco de dados.
		/// </summary>
		/// <param name="p_name">Nome do banco de dados a ser deletado.</param>
		public override void DropDatabase(string p_name)
		{
			throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Pollydb.DropDatabase");
		}

		/// <summary>
		/// Deleta o banco de dados conectado atualmente.
		/// </summary>
		public override void DropDatabase()
		{
			throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Pollydb.DropDatabase");
		}

		/// <summary>
		/// Lista os nomes de colunas de uma determinada consulta.
		/// </summary>
		/// <returns>Vetor com os nomes de colunas.</returns>
		/// <param name="p_sql">Consulta SQL.</param>
		public override string[] GetColumnNames(string p_sql)
		{
			string[] v_array;

			this.BuildCache(p_sql);
			v_array = this.v_database.GetColumnNames(p_sql);
			this.DestroyCache();

			return v_array;
		}

		/// <summary>
		/// Lista os nomes e tipos de colunas de uma determinada consulta.
		/// </summary>
		/// <returns>Matriz com os nomes e tipos de colunas.</returns>
		/// <param name="p_sql">Consulta SQL.</param>
		public override string[,] GetColumnNamesAndTypes(string p_sql)
		{
			string[,] v_matrix;

			this.BuildCache(p_sql);
			v_matrix = this.v_database.GetColumnNamesAndTypes(p_sql);
			this.DestroyCache();

			return v_matrix;
		}

		/// <summary>
		/// Lista os campos (ou colunas) de uma determinada consulta.
		/// </summary>
		/// <returns>Vetor de campos.</returns>
		/// <param name="p_sql">Consulta SQL.</param>
		public override System.Collections.Generic.List<Spartacus.Database.Field> GetFields(string p_sql)
		{
			System.Collections.Generic.List<Spartacus.Database.Field> v_list;

			this.BuildCache(p_sql);
			v_list = this.v_database.GetFields(p_sql);
			this.DestroyCache();

			return v_list;
		}

		/// <summary>
		/// Transfere dados do banco de dados atual para um banco de dados de destino.
		/// Conexão com o banco de destino precisa estar aberta.
		/// </summary>
		/// <returns>Número de linhas transferidas.</returns>
		/// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
		/// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
		/// <param name="p_destdatabase">Conexão com o banco de destino.</param>
		public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_insert, p_destdatabase);
			this.DestroyCache();
			return v_transfered;
		}

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
		public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, out string p_log)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_insert, p_destdatabase, out p_log);
			this.DestroyCache();
			return v_transfered;
		}

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
		public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_insert, p_destdatabase, p_startrow, p_endrow, out p_hasmoredata);
			this.DestroyCache();
			return v_transfered;
		}

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
		public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_insert, p_destdatabase, ref p_log, p_startrow, p_endrow, out p_hasmoredata);
			this.DestroyCache();
			return v_transfered;
		}

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
		public override uint Transfer(string p_query, string p_table, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_table, p_insert, p_destdatabase, p_startrow, p_endrow, out  p_hasmoredata);
			this.DestroyCache();
			return v_transfered;
		}

		/// <summary>
		/// Transfere dados do banco de dados atual para um banco de dados de destino.
		/// Conexão com o banco de destino precisa estar aberta.
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
		public override uint Transfer(string p_query, string p_table, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_table, p_insert, p_destdatabase, ref p_log, p_startrow, p_endrow, out p_hasmoredata);
			this.DestroyCache();
			return v_transfered;
		}

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
		public override uint Transfer(string p_query, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_table, p_columns, p_insert, p_destdatabase, p_startrow, p_endrow, out p_hasmoredata);
			this.DestroyCache();
			return v_transfered;
		}

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
		/// <param name="p_log">Log de inserção.</param>
		/// <param name='p_startrow'>Número da linha inicial.</param>
		/// <param name='p_endrow'>Número da linha final.</param>
		/// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
		public override uint Transfer(string p_query, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_table, p_columns, p_insert, p_destdatabase, ref p_log, p_startrow, p_endrow, out p_hasmoredata);
			this.DestroyCache();
			return v_transfered;
		}

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
		public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.Transfer(p_query, p_insert, p_destdatabase, p_progress, p_error);
			this.DestroyCache();
			return v_transfered;
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
		public override uint TransferToCSV(string p_query, string p_filename, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.TransferToCSV(p_query, p_filename, p_separator, p_delimiter, p_header, p_encoding);
			this.DestroyCache();
			return v_transfered;
		}

		/// <summary>
		/// Transfere dados do banco de dados atual para um arquivo XLSX.
		/// Não pára a execução se der um problema num comando de inserção específico.
		/// </summary>
		/// <returns>Número de linhas transferidas.</returns>
		/// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
		/// <param name="p_filename">Nome do arquivo de destino.</param>
		public override uint TransferToXLSX(string p_query, string p_filename)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.TransferToXLSX(p_query, p_filename);
			this.DestroyCache();
			return v_transfered;
		}

		/// <summary>
		/// Transfere dados do banco de dados atual para um arquivo DBF.
		/// Não pára a execução se der um problema num comando de inserção específico.
		/// </summary>
		/// <returns>Número de linhas transferidas.</returns>
		/// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
		/// <param name="p_filename">Nome do arquivo de destino.</param>
		public override uint TransferToDBF(string p_query, string p_filename)
		{
			uint v_transfered;
			this.BuildCache(p_query);
			v_transfered = this.v_database.TransferToDBF(p_query, p_filename);
			this.DestroyCache();
			return v_transfered;
		}

		/// <summary>
		/// Constrói o banco de dados temporário.
		/// </summary>
		/// <param name="p_sql">Texto SQL.</param>
		private void BuildCache(string p_sql)
		{
			Spartacus.Utils.ProgressEventClass v_progress;
			Spartacus.Utils.ErrorEventClass v_error;
			Spartacus.Utils.Cryptor v_cryptor;
			System.IO.FileInfo v_info;
			long v_totalsize;

			v_totalsize = 0;
			foreach (string s in this.ExtractFromString(p_sql, "[", "]"))
			{
				if (!s.StartsWith("."))
				{
					v_info = new System.IO.FileInfo(this.v_service + "/" + s);

					if (v_info.Exists)
					{
						if (p_sql.ToLower().Contains("drop table [" + s.ToLower() + "]"))
							this.v_tabletodrop = s.ToLower();
						
						v_totalsize += v_info.Length;
						this.v_tables.Add(s);
					}
					else
					{
						if (p_sql.ToLower().Contains("create table [" + s.ToLower() + "]"))
							this.v_tables.Add(s.ToLower());
						else
							throw new Spartacus.Database.Exception("File '{0}' does not exist and is not going to be created.", s);
					}
				}
			}

			if (v_totalsize > this.v_cachethreshold)
			{
				v_cryptor = new Spartacus.Utils.Cryptor("spartacus");
				this.v_tempdatabase = v_cryptor.RandomString() + ".db";
				this.v_database = new Spartacus.Database.Sqlite(this.v_tempdatabase);
			}
			else
			{
				this.v_tempdatabase = "";
				this.v_database = new Spartacus.Database.Memory();
			}

			this.v_database.SetTimeout(-1);
			this.v_database.SetExecuteSecurity(false);
			this.v_database.Open();
			this.v_database.Execute("PRAGMA synchronous=OFF");

			v_progress = new Spartacus.Utils.ProgressEventClass();
			v_progress.ProgressEvent += new Spartacus.Utils.ProgressEventClass.ProgressEventHandler(OnProgress);
			v_error = new Spartacus.Utils.ErrorEventClass();
			v_error.ErrorEvent += new Spartacus.Utils.ErrorEventClass.ErrorEventHandler(OnError);

			foreach (string t in this.v_tables)
			{
				v_info = new System.IO.FileInfo(this.v_service + "/" + t);
				if (v_info.Exists)
					this.v_database.TransferFromFile(this.v_service + "/" + t, this.v_separator, this.v_delimiter, this.v_header, this.v_encoding, "[" + t + "]", v_progress, v_error);
			}
		}

		/// <summary>
		/// Extrai uma lista de texto entre os delimitadores especificados.
		/// </summary>
		/// <returns>Lista de texto entre delimitadores especificados.</returns>
		/// <param name="p_text">Texto original completo.</param>
		/// <param name="p_start">Delimitador inicial.</param>
		/// <param name="p_end">Delimitador final.</param>
		private System.Collections.Generic.List<string> ExtractFromString(string p_text, string p_start, string p_end)
		{            
			System.Collections.Generic.List<string> v_matched;
			string v_text;
			int s, e;
			bool v_exit = false;

			v_matched = new System.Collections.Generic.List<string>();

			v_text = p_text;
			s = 0;
			e = 0;
			v_exit = false;
			while(! v_exit)
			{
				s = v_text.IndexOf(p_start);
				e = v_text.IndexOf(p_end);
				if (s != -1 && e != -1)
				{
					v_matched.Add(v_text.Substring(s + p_start.Length, e - s - p_start.Length));
					v_text = v_text.Substring(e + p_end.Length);
				}
				else
					v_exit = true;
			}

			return v_matched;
		}

		/// <summary>
		/// Atualiza arquivos do banco de dados.
		/// </summary>
		private void UpdateFiles()
		{
			System.IO.FileInfo v_info;

			if (this.v_tabletodrop != "")
			{
				v_info = new System.IO.FileInfo(this.v_service + "/" + this.v_tabletodrop);
				if (v_info.Exists)
					v_info.Delete();
			}

			foreach (string t in this.v_tables)
			{
				if (t != this.v_tabletodrop)
					this.v_database.TransferToFile("select * from [" + t + "]", this.v_service + "/" + t, this.v_separator, this.v_delimiter, this.v_header, this.v_encoding);
			}
		}

		/// <summary>
		/// Destrói o banco de dados temporário.
		/// </summary>
		private void DestroyCache()
		{
			System.IO.FileInfo v_info;

			this.v_database.Close();
			this.v_tables.Clear();
			this.v_tabletodrop = "";

			if (this.v_tempdatabase != "")
			{
				v_info = new System.IO.FileInfo(this.v_tempdatabase);
				if (v_info.Exists)
					v_info.Delete();
			}
		}

		/// <summary>
		/// Evento de progresso de transferência de um arquivo para o banco de dados temporário.
		/// </summary>
		private static void OnProgress(Spartacus.Utils.ProgressEventClass obj, Spartacus.Utils.ProgressEventArgs e)
		{
		}

		/// <summary>
		/// Evento de erro de transferência de um arquivo para o banco de dados temporário.
		/// </summary>
		private static void OnError(Spartacus.Utils.ErrorEventClass obj, Spartacus.Utils.ErrorEventArgs e)
		{
			throw new Spartacus.Database.Exception(e.v_message);
		}
	}
}

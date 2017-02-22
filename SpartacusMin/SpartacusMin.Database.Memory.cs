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

namespace SpartacusMin.Database
{
    /// <summary>
    /// Classe SpartacusMin.Database.Memory.
    /// Herda da classe <see cref="SpartacusMin.Database.Generic"/>.
    /// Utiliza o Mono.Data.Sqlite para acessar um SGBD Sqlite em memória.
    /// </summary>
    public class Memory : SpartacusMin.Database.Generic
    {
        /// <summary>
        /// Conexão com o banco de dados.
        /// </summary>
        private Mono.Data.Sqlite.SqliteConnection v_con;

        /// <summary>
        /// Comando para conexão com o banco de dados.
        /// </summary>
        private Mono.Data.Sqlite.SqliteCommand v_cmd;

        /// <summary>
        /// Leitor de dados do banco de dados.
        /// </summary>
        private Mono.Data.Sqlite.SqliteDataReader v_reader;

        /// <summary>
        /// Linha atual da QueryBlock.
        /// </summary>
        private uint v_currentrow;

        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="SpartacusMin.Database.Memory"/>.
        /// </summary>
        public Memory()
            : base()
        {
            this.v_connectionstring = "URI=file::memory:,version=3";

            this.v_con = null;
            this.v_cmd = null;
            this.v_reader = null;
            this.v_default_string = "text";
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do arquivo de banco de dados a ser criado.</param>
        public override void CreateDatabase(string p_name)
        {
            throw new SpartacusMin.Utils.NotSupportedException("SpartacusMin.Database.Memory.CreateDatabase");
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        public override void CreateDatabase()
        {
            throw new SpartacusMin.Utils.NotSupportedException("SpartacusMin.Database.Memory.CreateDatabase");
        }

        /// <summary>
        /// Abre a conexão com o banco de dados.
        /// </summary>
        public override void Open()
        {
            try
            {
                this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                this.v_con.Open();
                this.v_cmd = new Mono.Data.Sqlite.SqliteCommand();
                this.v_cmd.Connection = this.v_con;
                if (this.v_timeout > -1)
                    this.v_cmd.CommandTimeout = this.v_timeout;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
            System.Data.DataTable v_table = null;
            System.Data.DataRow v_row;

            #if DEBUG
            Console.WriteLine("SpartacusMin.Database.Memory.Query: " + p_sql);
            #endif

            try
            {
                this.v_cmd.CommandText = p_sql;
                this.v_reader = this.v_cmd.ExecuteReader();

                v_table = new System.Data.DataTable(p_tablename);
                for (int i = 0; i < this.v_reader.FieldCount; i++)
                    v_table.Columns.Add(this.v_reader.GetName(i), typeof(string));

                while (this.v_reader.Read())
                {
                    v_row = v_table.NewRow();
                    for (int i = 0; i < this.v_reader.FieldCount; i++)
                        v_row[i] = this.v_reader[i].ToString();
                    v_table.Rows.Add(v_row);
                }

                return v_table;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
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
        public override System.Data.DataTable Query(string p_sql, string p_tablename, SpartacusMin.Utils.ProgressEventClass p_progress)
        {
            System.Data.DataTable v_table = null;
            System.Data.DataRow v_row;
            uint v_counter = 0;

            #if DEBUG
            Console.WriteLine("SpartacusMin.Database.Memory.Query: " + p_sql);
            #endif

            p_progress.FireEvent(v_counter);

            try
            {
                this.v_cmd.CommandText = p_sql;
                this.v_reader = this.v_cmd.ExecuteReader();

                v_table = new System.Data.DataTable(p_tablename);
                for (int i = 0; i < this.v_reader.FieldCount; i++)
                    v_table.Columns.Add(this.v_reader.GetName(i), typeof(string));

                while (this.v_reader.Read())
                {
                    v_row = v_table.NewRow();
                    for (int i = 0; i < this.v_reader.FieldCount; i++)
                        v_row[i] = this.v_reader[i].ToString();
                    v_table.Rows.Add(v_row);

                    v_counter++;
                    p_progress.FireEvent(v_counter);
                }

                return v_table;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
            System.Data.DataTable v_table = null;
            System.Data.DataRow v_row;

            #if DEBUG
            Console.WriteLine("SpartacusMin.Database.Memory.QueryBlock: " + p_sql);
            #endif

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_sql;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                v_table = new System.Data.DataTable(p_tablename);
                for (int i = 0; i < this.v_reader.FieldCount; i++)
                    v_table.Columns.Add(this.v_reader.GetName(i), typeof(string));

                p_hasmoredata = false;
                while (this.v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        v_row = v_table.NewRow();
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            v_row[i] = this.v_reader[i].ToString();
                        v_table.Rows.Add(v_row);
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }

                return v_table;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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

            #if DEBUG
            Console.WriteLine("SpartacusMin.Database.Memory.QueryHtml: " + p_sql);
            #endif

            try
            {
                this.v_cmd.CommandText = p_sql;
                this.v_reader = this.v_cmd.ExecuteReader();

                v_html = "<table id='" + p_id + "' " + p_options + "><thead><tr>";

                for (int i = 0; i < this.v_reader.FieldCount; i++)
                    v_html += "<th>" + this.v_reader.GetName(i) + "</th>";

                v_html += "</tr></thead><tbody>";

                while (this.v_reader.Read())
                {
                    v_html += "<tr>";
                    for (int i = 0; i < this.v_reader.FieldCount; i++)
                        v_html += "<td>" + this.v_reader[i].ToString() + "</td>";
                    v_html += "</tr>";
                }

                v_html += "</tbody></table>";

                return v_html;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
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
            throw new SpartacusMin.Utils.NotSupportedException("SpartacusMin.Database.Memory.QueryStoredProc");
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
            System.Type v_type;
            T v_obj;
            System.Reflection.PropertyInfo v_prop;

            #if DEBUG
            Console.WriteLine("SpartacusMin.Database.Memory.QueryList<T>: " + p_sql);
            #endif

            try
            {
                this.v_reader = this.v_cmd.ExecuteReader();

                v_list = new System.Collections.Generic.List<T>();
                v_type = typeof(T);

                while (this.v_reader.Read())
                {
                    v_obj = System.Activator.CreateInstance<T>();

                    for (int i = 0; i < this.v_reader.FieldCount; i++)
                    {
                        v_prop = v_type.GetProperty(this.v_reader.GetName(i));
                        if (v_prop != null && this.v_reader.GetName(i) == v_prop.Name && this.v_reader[i] != System.DBNull.Value)
                            v_prop.SetValue(v_obj, System.Convert.ChangeType(this.v_reader[i], v_prop.PropertyType), null);
                    }

                    v_list.Add(v_obj);
                }

                return v_list;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
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
            System.Collections.Generic.List<string> v_row;

            #if DEBUG
            Console.WriteLine("SpartacusMin.Database.Memory.QuerySList: " + p_sql);
            #endif

            try
            {
                this.v_reader = this.v_cmd.ExecuteReader();

                v_list = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();

                while (this.v_reader.Read())
                {
                    v_row = new System.Collections.Generic.List<string>();

                    for (int i = 0; i < this.v_reader.FieldCount; i++)
                        v_row.Add(this.v_reader[i].ToString());

                    v_list.Add(v_row);
                }

                return v_list;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
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
			System.Collections.Generic.List<string> v_row;

			#if DEBUG
				Console.WriteLine("SpartacusMin.Database.Memory.QuerySList: " + p_sql);
            #endif

			try
			{
				this.v_reader = this.v_cmd.ExecuteReader();

				p_header = new System.Collections.Generic.List<string>();
				for (int i = 0; i < this.v_reader.FieldCount; i++)
					p_header.Add(this.v_reader.GetName(i));

				v_list = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();

				while (this.v_reader.Read())
				{
					v_row = new System.Collections.Generic.List<string>();

					for (int i = 0; i < this.v_reader.FieldCount; i++)
						v_row.Add(this.v_reader[i].ToString());

					v_list.Add(v_row);
				}

				return v_list;
			}
			catch (System.Exception e)
			{
				throw new SpartacusMin.Database.Exception(e);
			}
			finally
			{
				if (this.v_reader != null)
				{
					this.v_reader.Close();
					this.v_reader = null;
				}
			}
		}

        /// <summary>
        /// Executa um código SQL no banco de dados.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser executado no banco de dados.
        /// </param>
        public override void Execute(string p_sql)
        {
            #if DEBUG
            Console.WriteLine("SpartacusMin.Database.Memory.Execute: " + p_sql);
            #endif

            try
            {
                if (this.v_execute_security)
                    this.v_cmd.CommandText = SpartacusMin.Database.Command.RemoveUnwantedCharsExecute(p_sql);
                else
                    this.v_cmd.CommandText = p_sql;
                this.v_cmd.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
            string v_block;

            try
            {
                v_block = "begin;\n";
                for (int k = 0; k < p_rows.Count; k++)
                    v_block += "insert into " + p_table + " values " + p_rows[k] + ";\n";
                v_block += "commit;";

                #if DEBUG
                Console.WriteLine("SpartacusMin.Database.Memory.InsertBlock: " + v_block);
                #endif

                if (this.v_execute_security)
                    this.v_cmd.CommandText = SpartacusMin.Database.Command.RemoveUnwantedCharsExecute(v_block);
                else
                    this.v_cmd.CommandText = v_block;
                this.v_cmd.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
            string v_block;

            try
            {
                v_block = "begin;\n";
                for (int k = 0; k < p_rows.Count; k++)
                    v_block += "insert into " + p_table + " " + p_columnnames + " values " + p_rows[k] + ";\n";
                v_block += "commit;";

                #if DEBUG
                Console.WriteLine("SpartacusMin.Database.Memory.InsertBlock: " + v_block);
                #endif

                if (this.v_execute_security)
                    this.v_cmd.CommandText = SpartacusMin.Database.Command.RemoveUnwantedCharsExecute(v_block);
                else
                    this.v_cmd.CommandText = v_block;
                this.v_cmd.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
            object v_tmp;

            #if DEBUG
            Console.WriteLine("SpartacusMin.Database.Memory.ExecuteScalar: " + p_sql);
            #endif

            try
            {
                if (this.v_execute_security)
                    this.v_cmd.CommandText = SpartacusMin.Database.Command.RemoveUnwantedCharsExecute(p_sql);
                else
                    this.v_cmd.CommandText = p_sql;
                v_tmp = this.v_cmd.ExecuteScalar();
                if (v_tmp != null)
                    return v_tmp.ToString();
                else
                    return "";
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados.
        /// </summary>
        public override void Close()
        {
            if (this.v_cmd != null)
            {
                try { this.v_cmd.Cancel(); } catch {}
                this.v_cmd.Dispose();
                this.v_cmd = null;
            }
            if (this.v_reader != null)
            {
                this.v_reader.Close();
                this.v_reader = null;
            }
            if (this.v_con != null)
            {
                this.v_con.Close();
                this.v_con = null;
            }
        }

        /// <summary>
        /// Deleta um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do banco de dados a ser deletado.</param>
        public override void DropDatabase(string p_name)
        {
            throw new SpartacusMin.Utils.NotSupportedException("SpartacusMin.Database.Memory.DropDatabase");
        }

        /// <summary>
        /// Deleta o banco de dados conectado atualmente.
        /// </summary>
        public override void DropDatabase()
        {
            throw new SpartacusMin.Utils.NotSupportedException("SpartacusMin.Database.Memory.DropDatabase");
        }

        /// <summary>
        /// Lista os nomes de colunas de uma determinada consulta.
        /// </summary>
        /// <returns>Vetor com os nomes de colunas.</returns>
        /// <param name="p_sql">Consulta SQL.</param>
        public override string[] GetColumnNames(string p_sql)
        {
            string[] v_array;

            try
            {
                this.v_cmd.CommandText = p_sql;
                this.v_reader = this.v_cmd.ExecuteReader();

                v_array = new string[this.v_reader.FieldCount];
                for (int i = 0; i < this.v_reader.FieldCount; i++)
                    v_array[i] = this.v_reader.GetName(i);

                return v_array;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
        }

        /// <summary>
        /// Lista os nomes e tipos de colunas de uma determinada consulta.
        /// </summary>
        /// <returns>Matriz com os nomes e tipos de colunas.</returns>
        /// <param name="p_sql">Consulta SQL.</param>
        public override string[,] GetColumnNamesAndTypes(string p_sql)
        {
            string[,] v_matrix;

            try
            {
                this.v_cmd.CommandText = p_sql;
                this.v_reader = this.v_cmd.ExecuteReader();

                v_matrix = new string[this.v_reader.FieldCount, 2];
                for (int i = 0; i < this.v_reader.FieldCount; i++)
                {
                    v_matrix[i, 0] = this.v_reader.GetName(i);
                    v_matrix[i, 1] = this.v_reader.GetDataTypeName(i);
                }

                return v_matrix;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
        }

		/// <summary>
		/// Lista os campos (ou colunas) de uma determinada consulta.
		/// </summary>
		/// <returns>Vetor de campos.</returns>
		/// <param name="p_sql">Consulta SQL.</param>
		public override System.Collections.Generic.List<SpartacusMin.Database.Field> GetFields(string p_sql)
		{
			System.Collections.Generic.List<SpartacusMin.Database.Field> v_list;

			try
			{
				this.v_cmd.CommandText = p_sql;
				this.v_reader = this.v_cmd.ExecuteReader();

				v_list = new System.Collections.Generic.List<SpartacusMin.Database.Field>();
				for (int i = 0; i < this.v_reader.FieldCount; i++)
					v_list.Add(new SpartacusMin.Database.Field(this.v_reader.GetName(i), this.v_reader.GetFieldType(i)));

				return v_list;
			}
			catch (System.Exception e)
			{
				throw new SpartacusMin.Database.Exception(e);
			}
			finally
			{
				if (this.v_reader != null)
				{
					this.v_reader.Close();
					this.v_reader = null;
				}
			}
		}

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        public override uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase)
        {
            uint v_transfered = 0;

            try
            {
                this.v_cmd.CommandText = p_query;
                this.v_reader = this.v_cmd.ExecuteReader();

                while (this.v_reader.Read())
                {
                    for (int i = 0; i < this.v_reader.FieldCount; i++)
                        p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                    p_destdatabase.Execute(p_insert.GetUpdatedText());
                    v_transfered++;
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
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
        public override uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, out string p_log)
        {
            uint v_transfered = 0;
            string v_insert;

            p_log = "";

            try
            {
                this.v_cmd.CommandText = p_query;
                this.v_reader = this.v_cmd.ExecuteReader();

                while (this.v_reader.Read())
                {
                    for (int i = 0; i < this.v_reader.FieldCount; i++)
                        p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                    v_insert = p_insert.GetUpdatedText();
                    try
                    {
                        p_destdatabase.Execute(v_insert);
                        v_transfered++;
                    }
                    catch (SpartacusMin.Database.Exception e)
                    {
                        p_log += v_insert + "\n" + e.v_message + "\n";
                    }
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
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
        public override uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            uint v_transfered = 0;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                p_hasmoredata = false;
                while (this.v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                        p_destdatabase.Execute(p_insert.GetUpdatedText());
                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
        public override uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            uint v_transfered = 0;
            string v_insert;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                p_hasmoredata = false;
                while (this.v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                        }
                        catch (SpartacusMin.Database.Exception e)
                        {
                            p_log += v_insert + "\n" + e.v_message + "\n";
                        }
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
        public override uint Transfer(string p_query, string p_table, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            uint v_transfered = 0;
            System.Collections.Generic.List<string> v_rows = new System.Collections.Generic.List<string>();
            string v_columnnames;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                v_columnnames = "(" + this.v_reader.GetName(0);
                for (int i = 1; i < this.v_reader.FieldCount; i++)
                    v_columnnames += "," + this.v_reader.GetName(i);
                v_columnnames += ")";

                p_hasmoredata = false;
                while (this.v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                        v_rows.Add(p_insert.GetUpdatedText());

                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
                else
                    p_destdatabase.InsertBlock(p_table, v_rows, v_columnnames);

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
        public override uint Transfer(string p_query, string p_table, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            uint v_transfered = 0;
            System.Collections.Generic.List<string> v_rows = new System.Collections.Generic.List<string>();
            string v_columnnames;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                v_columnnames = "(" + this.v_reader.GetName(0);
                for (int i = 1; i < this.v_reader.FieldCount; i++)
                    v_columnnames += "," + this.v_reader.GetName(i);
                v_columnnames += ")";

                p_hasmoredata = false;
                while (this.v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                        v_rows.Add(p_insert.GetUpdatedText());

                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
                else
                {
                    try
                    {
                        p_destdatabase.InsertBlock(p_table, v_rows, v_columnnames);
                    }
                    catch (SpartacusMin.Database.Exception e)
                    {
                        p_log += e.v_message + "\n";
                    }
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
        public override uint Transfer(string p_query, string p_table, string p_columns, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            uint v_transfered = 0;
            System.Collections.Generic.List<string> v_rows = new System.Collections.Generic.List<string>();

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                p_hasmoredata = false;
                while (this.v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                        v_rows.Add(p_insert.GetUpdatedText());

                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
                else
                    p_destdatabase.InsertBlock(p_table, v_rows, p_columns);

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
        public override uint Transfer(string p_query, string p_table, string p_columns, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            uint v_transfered = 0;
            System.Collections.Generic.List<string> v_rows = new System.Collections.Generic.List<string>();

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                p_hasmoredata = false;
                while (this.v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                        v_rows.Add(p_insert.GetUpdatedText());

                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
                else
                {
                    try
                    {
                        p_destdatabase.InsertBlock(p_table, v_rows, p_columns);
                    }
                    catch (SpartacusMin.Database.Exception e)
                    {
                        p_log += e.v_message + "\n";
                    }
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
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
        public override uint Transfer(string p_query, SpartacusMin.Database.Command p_insert, SpartacusMin.Database.Generic p_destdatabase, SpartacusMin.Utils.ProgressEventClass p_progress, SpartacusMin.Utils.ErrorEventClass p_error)
        {
            uint v_transfered = 0;
            string v_insert;

            p_progress.FireEvent(v_transfered);

            try
            {
                this.v_cmd.CommandText = p_query;
                this.v_reader = this.v_cmd.ExecuteReader();

                while (this.v_reader.Read())
                {
                    for (int i = 0; i < this.v_reader.FieldCount; i++)
                        p_insert.SetValue(this.v_reader.GetName(i).ToLower(), this.v_reader[i].ToString(), this.v_execute_security);

                    v_insert = p_insert.GetUpdatedText();
                    try
                    {
                        p_destdatabase.Execute(v_insert);
                        v_transfered++;
                        p_progress.FireEvent(v_transfered);
                    }
                    catch (SpartacusMin.Database.Exception e)
                    {
                        p_error.FireEvent(v_insert + "\n" + e.v_message);
                    }
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new SpartacusMin.Database.Exception(e);
            }
            finally
            {
                if (this.v_reader != null)
                {
                    this.v_reader.Close();
                    this.v_reader = null;
                }
            }
        }
    }
}

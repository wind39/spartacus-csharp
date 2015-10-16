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
using Mono.Data.Sqlite;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Sqlite.
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza o Mono.Data.Sqlite para acessar um SGBD Sqlite.
    /// </summary>
    public class Sqlite : Spartacus.Database.Generic
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
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Sqlite"/>.
        /// </summary>
        /// <param name='p_file'>
        /// Caminho para o arquivo DB.
        /// </param>
        public Sqlite(string p_file)
            : base(p_file)
        {
            this.v_connectionstring = "Data Source=" + p_file + ";Version=3;Synchronous=Full;Journal Mode=Off;";

            this.v_con = null;
            this.v_cmd = null;
            this.v_reader = null;
        }

        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Sqlite"/>.
        /// </summary>
        public Sqlite()
            : base()
        {
            this.v_con = null;
            this.v_cmd = null;
            this.v_reader = null;
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do arquivo de banco de dados a ser criado.</param>
        public override void CreateDatabase(string p_name)
        {
            Mono.Data.Sqlite.SqliteConnection.CreateFile(p_name);
            this.v_service = p_name;
            this.v_connectionstring = "Data Source=" + p_name + ";Version=3;Synchronous=Full;Journal Mode=Off;";
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        public override void CreateDatabase()
        {
            Mono.Data.Sqlite.SqliteConnection.CreateFile(this.v_service);
            this.v_connectionstring = "Data Source=" + this.v_service + ";Version=3;Synchronous=Full;Journal Mode=Off;";
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
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
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

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, this.v_con);
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_table = new System.Data.DataTable(p_tablename);
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_table.Columns.Add(this.FixColumnName(this.v_reader.GetName(i)), typeof(string));

                    while (this.v_reader.Read())
                    {
                        v_row = v_table.NewRow();
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            v_row[i] = this.v_reader[i].ToString();
                        v_table.Rows.Add(v_row);
                    }

                    return v_table;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.Close();
                        this.v_reader = null;
                    }
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = p_sql;
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_table = new System.Data.DataTable(p_tablename);
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_table.Columns.Add(this.FixColumnName(this.v_reader.GetName(i)), typeof(string));

                    while (this.v_reader.Read())
                    {
                        v_row = v_table.NewRow();
                        for (int i = 0; i < this.v_reader.FieldCount; i++)
                            v_row[i] = this.v_reader[i].ToString();
                        v_table.Rows.Add(v_row);
                    }

                    return v_table;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
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
            System.Data.DataTable v_table = null;
            System.Data.DataRow v_row;
            uint v_counter = 0;

            p_progress.FireEvent(v_counter);

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, this.v_con);
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_table = new System.Data.DataTable(p_tablename);
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_table.Columns.Add(this.FixColumnName(this.v_reader.GetName(i)), typeof(string));

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
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.Close();
                        this.v_reader = null;
                    }
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = p_sql;
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_table = new System.Data.DataTable(p_tablename);
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_table.Columns.Add(this.FixColumnName(this.v_reader.GetName(i)), typeof(string));

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
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
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
        public override System.Data.DataTable Query(string p_sql, string p_tablename, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            System.Data.DataTable v_table = null;
            System.Data.DataRow v_row;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_sql;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                v_table = new System.Data.DataTable(p_tablename);
                for (int i = 0; i < v_reader.FieldCount; i++)
                    v_table.Columns.Add(this.FixColumnName(this.v_reader.GetName(i)), typeof(string));

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
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
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

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, this.v_con);
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_html = "<table id='" + p_id + "' " + p_options + "><thead><tr>";

                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_html += "<th>" + this.FixColumnName(this.v_reader.GetName(i)) + "</th>";

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
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.Close();
                        this.v_reader = null;
                    }
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = p_sql;
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_html = "<table id='" + p_id + "' " + p_options + "><thead><tr>";

                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_html += "<th>" + this.FixColumnName(this.v_reader.GetName(i)) + "</th>";

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
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
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
            throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Sqlite.QueryStoredProc");
        }

        /// <summary>
        /// Executa um código SQL no banco de dados.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser executado no banco de dados.
        /// </param>
        public override void Execute(string p_sql)
        {
            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_con);
                    this.v_cmd.ExecuteNonQuery();
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);
                    this.v_cmd.ExecuteNonQuery();
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
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
        public override void InsertBlock(string p_table, System.Collections.ArrayList p_rows)
        {
            string v_block;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();

                    v_block = "begin;\n";
                    for (int k = 0; k < p_rows.Count; k++)
                        v_block += "insert into " + p_table + " values " + (string)p_rows[k] + ";\n";
                    v_block += "commit;";

                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(v_block), this.v_con);
                    this.v_cmd.ExecuteNonQuery();
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    v_block = "begin;\n";
                    for (int k = 0; k < p_rows.Count; k++)
                        v_block += "insert into " + p_table + " values " + (string)p_rows[k] + ";\n";
                    v_block += "commit;";

                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(v_block), this.v_con);
                    this.v_cmd.ExecuteNonQuery();
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
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

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_con);
                    v_tmp = this.v_cmd.ExecuteScalar();
                    if (v_tmp != null)
                        return v_tmp.ToString();
                    else
                        return "";
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);
                    v_tmp = this.v_cmd.ExecuteScalar();
                    if (v_tmp != null)
                        return v_tmp.ToString();
                    else
                        return "";
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados.
        /// </summary>
        public override void Close()
        {
            if (this.v_reader != null)
            {
                this.v_reader.Close();
                this.v_reader = null;
            }
            if (this.v_cmd != null)
            {
                this.v_cmd.Dispose();
                this.v_cmd = null;
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
            throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Sqlite.DropDatabase");
        }

        /// <summary>
        /// Deleta o banco de dados conectado atualmente.
        /// </summary>
        public override void DropDatabase()
        {
            throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Sqlite.DropDatabase");
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
            uint v_transfered = 0;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(p_query, this.v_con);
                    this.v_reader = this.v_cmd.ExecuteReader();

                    while (v_reader.Read())
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

                        p_destdatabase.Execute(p_insert.GetUpdatedText());
                        v_transfered++;
                    }

                    return v_transfered;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.Close();
                        this.v_reader = null;
                    }
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();

                    while (v_reader.Read())
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

                        p_destdatabase.Execute(p_insert.GetUpdatedText());
                        v_transfered++;
                    }

                    return v_transfered;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
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
            uint v_transfered = 0;
            string v_insert;

            p_log = "";

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(p_query, this.v_con);
                    this.v_reader = this.v_cmd.ExecuteReader();

                    while (v_reader.Read())
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                        }
                        catch (Spartacus.Database.Exception e)
                        {
                            p_log += v_insert + "\n" + e.v_message + "\n";
                        }
                    }

                    return v_transfered;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.Close();
                        this.v_reader = null;
                    }
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();

                    while (v_reader.Read())
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                        }
                        catch (Spartacus.Database.Exception e)
                        {
                            p_log += v_insert + "\n" + e.v_message + "\n";
                        }
                    }

                    return v_transfered;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
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
                while (v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

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
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
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
        public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
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
                while (v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                        }
                        catch (Spartacus.Database.Exception e)
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
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
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
        public override uint Transfer(string p_query, string p_table, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            uint v_transfered = 0;
            System.Collections.ArrayList v_rows = new System.Collections.ArrayList();

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                p_hasmoredata = false;
                while (v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

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
                    p_destdatabase.InsertBlock(p_table, v_rows);

                return v_transfered;
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
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
        public override uint Transfer(string p_query, string p_table, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            uint v_transfered = 0;
            System.Collections.ArrayList v_rows = new System.Collections.ArrayList();

            try
            {
                if (this.v_reader == null)
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();
                    this.v_currentrow = 0;
                }

                p_hasmoredata = false;
                while (v_reader.Read())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

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
                        p_destdatabase.InsertBlock(p_table, v_rows);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_log += e.v_message + "\n";
                    }
                }

                return v_transfered;
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
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
        public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            uint v_transfered = 0;
            string v_insert;

            p_progress.FireEvent(v_transfered);

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(p_query, this.v_con);
                    this.v_reader = this.v_cmd.ExecuteReader();

                    while (v_reader.Read())
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
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
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.Close();
                        this.v_reader = null;
                    }
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = p_query;
                    this.v_reader = this.v_cmd.ExecuteReader();

                    while (v_reader.Read())
                    {
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            p_insert.SetValue(this.FixColumnName(v_reader.GetName(i)).ToLower(), v_reader[i].ToString());

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
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
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
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
        public override uint TransferFromFile(string p_filename, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
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
        public override uint TransferFromFile(string p_filename, char p_separator, bool p_header, System.Text.Encoding p_encoding, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
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
        public override uint TransferFromFile(string p_filename, char p_separator, char p_delimitator, bool p_header, System.Text.Encoding p_encoding, Spartacus.Database.Command p_insert, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
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
        public override uint TransferFromFile(string p_filename, string p_newtable, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
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
        public override uint TransferFromFile(string p_filename, char p_separator, bool p_header, System.Text.Encoding p_encoding, string p_newtable, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
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
        public override uint TransferFromFile(string p_filename, char p_separator, char p_delimitator, bool p_header, System.Text.Encoding p_encoding, string p_newtable, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
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
        public override uint TransferToFile(string p_query, string p_filename)
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
        /// Lista os nomes de colunas de uma determinada consulta.
        /// </summary>
        /// <returns>Vetor com os nomes de colunas.</returns>
        /// <param name="p_sql">Consulta SQL.</param>
        public override string[] GetColumnNames(string p_sql)
        {
            string[] v_array;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, this.v_con);
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_array = new string[v_reader.FieldCount];
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_array[i] = this.FixColumnName(this.v_reader.GetName(i));

                    return v_array;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.Close();
                        this.v_reader = null;
                    }
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = p_sql;
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_array = new string[v_reader.FieldCount];
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_array[i] = this.FixColumnName(this.v_reader.GetName(i));

                    return v_array;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
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

        /// <summary>
        /// Lista os nomes e tipos de colunas de uma determinada consulta.
        /// </summary>
        /// <returns>Matriz com os nomes e tipos de colunas.</returns>
        /// <param name="p_sql">Consulta SQL.</param>
        public override string[,] GetColumnNamesAndTypes(string p_sql)
        {
            string[,] v_matrix;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, this.v_con);
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_matrix = new string[v_reader.FieldCount, 2];
                    for (int i = 0; i < v_reader.FieldCount; i++)
                    {
                        v_matrix[i, 0] = this.FixColumnName(this.v_reader.GetName(i));
                        v_matrix[i, 1] = this.v_reader.GetDataTypeName(i);
                    }

                    return v_matrix;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.Close();
                        this.v_reader = null;
                    }
                    if (this.v_cmd != null)
                    {
                        this.v_cmd.Dispose();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.Close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_cmd.CommandText = p_sql;
                    this.v_reader = this.v_cmd.ExecuteReader();

                    v_matrix = new string[v_reader.FieldCount, 2];
                    for (int i = 0; i < v_reader.FieldCount; i++)
                    {
                        v_matrix[i, 0] = this.FixColumnName(this.v_reader.GetName(i));
                        v_matrix[i, 1] = this.v_reader.GetDataTypeName(i);
                    }

                    return v_matrix;
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    throw new Spartacus.Database.Exception(e);
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
}

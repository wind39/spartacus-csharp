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
using System.Data.OleDb;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Oledb.
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza a implementação OLE DB para acessar qualquer SGBD.
    /// </summary>
    public class Oledb : Spartacus.Database.Generic
    {
        /// <summary>
        /// String de conexão para acessar o banco.
        /// </summary>
        private string v_connectionstring;

        /// <summary>
        /// Conexão com o banco de dados.
        /// </summary>
        private System.Data.OleDb.OleDbConnection v_con;

        /// <summary>
        /// Comando para conexão com o banco de dados.
        /// </summary>
        private System.Data.OleDb.OleDbCommand v_cmd;

        /// <summary>
        /// Leitor de dados do banco de dados.
        /// </summary>
        private System.Data.OleDb.OleDbDataReader v_reader;

        /// <summary>
        /// Linha atual da QueryBlock.
        /// </summary>
        private uint v_currentrow;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Oledb"/>.
        /// Cria a string de conexão ao banco.
        /// </summary>
        /// <param name='p_provider'>
        /// SGBD que fornece o banco de dados.
        /// </param>
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
        public Oledb (string p_provider, string p_host, string p_port, string p_service, string p_user, string p_password)
            : base(p_host, p_port, p_service, p_user, p_password)
        {
            Spartacus.Utils.File v_file;

            switch (p_provider)
            {
                case "Oracle":
                    this.v_connectionstring = "Provider=OraOLEDB.Oracle;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST="
                        + this.v_host + ")(PORT="
                        + this.v_port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME="
                        + this.v_service + ")));User Id="
                        + this.v_user + ";Password="
                        + this.v_password;
                    break;
                case "Access":
                    v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, v_service);
                    if (v_file.v_extension.ToLower() == "accdb")
                        this.v_connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.v_service + ";Persist Security Info=False;";
                    else
                        this.v_connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.v_service + ";User Id=admin;Password=;";
                    break;
                default:
                    this.v_connectionstring = "Provider="
                        + p_provider + ";Addr="
                        + this.v_host + ";Port="
                        + this.v_port + ";Database="
                        + this.v_service + ";User Id="
                        + this.v_user + ";Password="
                        + this.v_password;
                    break;
            }

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
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        public override void CreateDatabase()
        {
        }

        /// <summary>
        /// Abre a conexão com o banco de dados.
        /// </summary>
        public override void Open()
        {
            try
            {
                this.v_con = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
                this.v_con.Open();
                this.v_cmd = new System.Data.OleDb.OleDbCommand();
                this.v_cmd.Connection = this.v_con;
            }
            catch (System.Data.OleDb.OleDbException e)
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
                    this.v_con = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new System.Data.OleDb.OleDbCommand(p_sql, this.v_con);
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
                catch (System.Data.OleDb.OleDbException e)
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
                catch (System.Data.OleDb.OleDbException e)
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
                    this.v_con = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new System.Data.OleDb.OleDbCommand(p_sql, this.v_con);
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
                catch (System.Data.OleDb.OleDbException e)
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
                catch (System.Data.OleDb.OleDbException e)
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
            catch (System.Data.OleDb.OleDbException e)
            {
                throw new Spartacus.Database.Exception(e);
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
            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new System.Data.OleDb.OleDbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_con);
                    this.v_cmd.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException e)
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
                catch (System.Data.OleDb.OleDbException e)
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
            if (this.v_con == null)
            {
                try
                {
                    this.v_con = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new System.Data.OleDb.OleDbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_con);
                    return (string) this.v_cmd.ExecuteScalar();
                }
                catch (System.Data.OleDb.OleDbException e)
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
                    return (string) this.v_cmd.ExecuteScalar();
                }
                catch (System.Data.OleDb.OleDbException e)
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
        }

        /// <summary>
        /// Deleta o banco de dados conectado atualmente.
        /// </summary>
        public override void DropDatabase()
        {
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
                    this.v_con = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new System.Data.OleDb.OleDbCommand(p_query, this.v_con);
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
                catch (System.Data.OleDb.OleDbException e)
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
                catch (System.Data.OleDb.OleDbException e)
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
                    this.v_con = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new System.Data.OleDb.OleDbCommand(p_query, this.v_con);
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
                catch (System.Data.OleDb.OleDbException e)
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
                catch (System.Data.OleDb.OleDbException e)
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
            catch (System.Data.OleDb.OleDbException e)
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
            catch (System.Data.OleDb.OleDbException e)
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
                    this.v_con = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
                    this.v_con.Open();
                    this.v_cmd = new System.Data.OleDb.OleDbCommand(p_query, this.v_con);
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
                catch (System.Data.OleDb.OleDbException e)
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
                catch (System.Data.OleDb.OleDbException e)
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

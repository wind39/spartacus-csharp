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
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// Conexão persistente com o banco de dados.
        /// </summary>
        private Mono.Data.Sqlite.SqliteConnection v_sqlcon;


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
        }

        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Sqlite"/>.
        /// </summary>
        public Sqlite()
            : base()
        {
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
        /// Abra a conexão com o banco de dados, se esta for persistente (por exemplo, SQLite).
        /// </summary>
        public override void Open()
        {
            this.v_sqlcon = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring);
            this.v_sqlcon.Open();
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
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar a consulta.</exception>
        public override System.Data.DataTable Query(string p_sql, string p_tablename)
        {
            System.Data.DataTable v_table = null;
            Mono.Data.Sqlite.SqliteDataReader v_reader;
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            System.Data.DataRow v_row;

            try
            {
                v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, this.v_sqlcon);
                v_reader = v_sqlcmd.ExecuteReader();

                v_table = new System.Data.DataTable(p_tablename);
                for (int i = 0; i < v_reader.FieldCount; i++)
                    v_table.Columns.Add(this.FixColumnName(v_reader.GetName(i)), typeof(string));

                while (v_reader.Read())
                {
                    v_row = v_table.NewRow();
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_row[i] = v_reader[i].ToString();
                    v_table.Rows.Add(v_row);
                }

                v_reader.Close();
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
            }

            return v_table;
        }

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
        public override System.Data.DataTable Query(string p_sql, string p_tablename, uint p_startrow, uint p_endrow)
        {
            System.Data.DataTable v_table = null;
            Mono.Data.Sqlite.SqliteDataReader v_reader;
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            System.Data.DataRow v_row;
            uint v_currentrow;

            try
            {
                v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, this.v_sqlcon);
                v_reader = v_sqlcmd.ExecuteReader();

                v_table = new System.Data.DataTable(p_tablename);
                for (int i = 0; i < v_reader.FieldCount; i++)
                    v_table.Columns.Add(this.FixColumnName(v_reader.GetName(i)), typeof(string));

                v_currentrow = 0;
                while (v_reader.Read())
                {
                    if (v_currentrow >= p_startrow && v_currentrow <= p_endrow)
                    {
                        v_row = v_table.NewRow();
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            v_row[i] = v_reader[i].ToString();
                        v_table.Rows.Add(v_row);
                    }

                    if (v_currentrow > p_endrow)
                        break;

                    v_currentrow++;
                }

                v_reader.Close();
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
            }

            return v_table;
        }

        /// <summary>
        /// Executa um código SQL no banco de dados.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser executado no banco de dados.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override void Execute(string p_sql)
        {
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;

            try
            {
                v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_sqlcon);
                v_sqlcmd.ExecuteNonQuery();
            }
            catch (Mono.Data.Sqlite.SqliteException e)
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
        /// <param name='p_verbose'>
        /// Se deve ser mostrado o código SQL no console.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override void Execute(string p_sql, bool p_verbose)
        {
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            string v_sql;

            try
            {
                v_sql = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);

                if (p_verbose)
                {
                    System.Console.WriteLine("Spartacus [{0}] - Spartacus.Database.Sqlite.Execute:", System.DateTime.UtcNow);
                    System.Console.WriteLine(v_sql);
                    System.Console.WriteLine("--------------------------------------------------");
                }

                v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(v_sql, this.v_sqlcon);
                v_sqlcmd.ExecuteNonQuery();
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
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
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override string ExecuteScalar(string p_sql)
        {
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            string v_ret;

            try
            {
                v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_sqlcon);
                v_ret = v_sqlcmd.ExecuteScalar().ToString();
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
            }

            return v_ret;
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
        /// <param name='p_verbose'>
        /// Se deve ser mostrado o código SQL no console.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override string ExecuteScalar(string p_sql, bool p_verbose)
        {
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            string v_sql, v_ret;

            try
            {
                v_sql = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);

                if (p_verbose)
                {
                    System.Console.WriteLine("Spartacus [{0}] - Spartacus.Database.Sqlite.ExecuteScalar:", System.DateTime.UtcNow);
                    System.Console.WriteLine(v_sql);
                    System.Console.WriteLine("--------------------------------------------------");
                }

                v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(v_sql, this.v_sqlcon);
                v_ret = v_sqlcmd.ExecuteScalar().ToString();
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
            }

            return v_ret;
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados, se esta for persistente (por exemplo, SQLite).
        /// </summary>
        public override void Close()
        {
            this.v_sqlcon.Close();
            this.v_sqlcon.Dispose();
            this.v_sqlcon = null;
        }

        /// <summary>
        /// Deleta um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do banco de dados a ser deletado.</param>
        public override void DropDatabase(string p_name)
        {
            (new System.IO.FileInfo(p_name)).Delete();
        }

        /// <summary>
        /// Deleta o banco de dados conectado atualmente.
        /// </summary>
        public override void DropDatabase()
        {
            (new System.IO.FileInfo(this.v_service)).Delete();
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        public override int Transfer(string p_query, string p_insert, Spartacus.Database.Generic p_destdatabase)
        {
            Mono.Data.Sqlite.SqliteDataReader v_reader;
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            int v_transfered = 0;
            string v_insert;

            try
            {
                v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(p_query, this.v_sqlcon);
                v_reader = v_sqlcmd.ExecuteReader();

                while (v_reader.Read())
                {
                    v_insert = p_insert;
                    for (int i = 0; i < v_reader.FieldCount; i++)
                        v_insert.Replace("#" + this.FixColumnName(v_reader.GetName(i)) + "#", v_reader[i].ToString());

                    p_destdatabase.Execute(v_insert);
                    v_transfered++;
                }

                v_reader.Close();
            }
            catch (Mono.Data.Sqlite.SqliteException e)
            {
                throw new Spartacus.Database.Exception(e);
            }

            return v_transfered;
        }
    }
}

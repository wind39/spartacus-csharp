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
using FirebirdSql;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Firebird.
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza o Firebird .NET Provider para acessar um SGBD Firebird.
    /// </summary>
    public class Firebird : Spartacus.Database.Generic
    {
        /// <summary>
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Firebird"/>.
        /// </summary>
        /// <param name='p_source'>
        /// IP do servidor Firebird.
        /// </param>
        /// <param name='p_port'>
        /// Porta de conexão.
        /// </param>
        /// <param name='p_file'>
        /// Caminho completo para o arquivo FDB ou GDB.
        /// </param>
        /// <param name='p_user'>
        /// Usuário do Firebird.
        /// </param>
        /// <param name='p_password'>
        /// Senha do Firebird.
        /// </param>
        public Firebird(string p_source, string p_port, string p_file, string p_user, string p_password)
            : base(p_source, p_port, p_file, p_user, p_password)
        {
            this.v_connectionstring = "DataSource=" + p_source + ";"
                + "Port=" + p_port + ";"
                + "Database=" + p_file + ";"
                + "User=" + p_user + ";"
                + "Password=" + p_password + ";"
                + "Dialect=3;Charset=NONE;Role=;";
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
            FirebirdSql.Data.FirebirdClient.FbDataReader v_reader;
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;
            System.Data.DataRow v_row;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();
                    v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(p_sql, v_fbcon);
                    v_reader = v_fbcmd.ExecuteReader();

                    while (v_reader.Read())
                    {
                        if (v_table == null)
                        {
                            v_table = new System.Data.DataTable(p_tablename);
                            for (int i = 0; i < v_reader.FieldCount; i++)
                                v_table.Columns.Add(this.FixColumnName(v_reader.GetName(i)), typeof(string));
                        }

                        v_row = v_table.NewRow();
                        for (int i = 0; i < v_reader.FieldCount; i++)
                            v_row[i] = v_reader.GetString(i);
                        v_table.Rows.Add(v_row);
                    }

                    v_reader.Close();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }

            return v_table;

            /*
            System.Data.DataTable v_table = null;
            FirebirdSql.Data.FirebirdClient.FbDataAdapter v_fbadp;
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();

                    v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(p_sql, v_fbcon);
                    v_fbadp = new FirebirdSql.Data.FirebirdClient.FbDataAdapter(v_fbcmd);
                    v_table = new System.Data.DataTable(p_tablename);
                    v_fbadp.Fill(v_table);
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }

            return v_table;
            */
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
            FirebirdSql.Data.FirebirdClient.FbDataReader v_reader;
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;
            System.Data.DataRow v_row;
            uint v_currentrow;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();
                    v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(p_sql, v_fbcon);
                    v_reader = v_fbcmd.ExecuteReader();

                    v_currentrow = 0;
                    while (v_reader.Read())
                    {
                        if (v_currentrow >= p_startrow && v_currentrow <= p_endrow)
                        {
                            if (v_table == null)
                            {
                                v_table = new System.Data.DataTable(p_tablename);
                                for (int i = 0; i < v_reader.FieldCount; i++)
                                    v_table.Columns.Add(this.FixColumnName(v_reader.GetName(i)), typeof(string));
                            }

                            v_row = v_table.NewRow();
                            for (int i = 0; i < v_reader.FieldCount; i++)
                                v_row[i] = v_reader.GetString(i);
                            v_table.Rows.Add(v_row);
                        }

                        v_currentrow++;
                    }

                    v_reader.Close();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
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
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();

                    v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_fbcon);
                    v_fbcmd.ExecuteNonQuery();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
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
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;
            string v_sql;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();

                    v_sql = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);

                    if (p_verbose)
                    {
                        System.Console.WriteLine("Spartacus [{0}] - Spartacus.Database.Firebird.Execute:", System.DateTime.Now);
                        System.Console.WriteLine(v_sql);
                        System.Console.WriteLine("--------------------------------------------------");
                    }

                    v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(v_sql, v_fbcon);
                    v_fbcmd.ExecuteNonQuery();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
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
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar o código SQL.</exception>
        public override string ExecuteScalar(string p_sql)
        {
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;
            string v_ret;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();

                    v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_fbcon);
                    v_ret = v_fbcmd.ExecuteScalar().ToString();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
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
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;
            string v_sql, v_ret;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();

                    v_sql = Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql);

                    if (p_verbose)
                    {
                        System.Console.WriteLine("Spartacus [{0}] - Spartacus.Database.Firebird.ExecuteScalar:", System.DateTime.Now);
                        System.Console.WriteLine(v_sql);
                        System.Console.WriteLine("--------------------------------------------------");
                    }

                    v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(v_sql, v_fbcon);
                    v_ret = v_fbcmd.ExecuteScalar().ToString();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }

            return v_ret;
        }

        /// <summary>
        /// Insere uma massa de dados.
        /// <paramref name="p_table"/> precisa ter o nome igual ao nome da tabela onde será inserido.
        /// Os nomes das colunas também precisam ser os mesmos.
        /// </summary>
        /// <param name="p_table">Tabela com os dados e definições para inserção em massa.</param>
        public override void BulkInsert(System.Data.DataTable p_table)
        {
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;
            string v_sqlheader, v_sql;
            int k;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();

                    v_sqlheader = "insert into " + p_table.TableName + " (" + p_table.Columns[0].ColumnName + ", ";
                    for (k = 1; k < p_table.Columns.Count; k++)
                        v_sqlheader += ", " + p_table.Columns[k].ColumnName;
                    v_sqlheader += ") values (";

                    foreach (System.Data.DataRow r in p_table.Rows)
                    {
                        v_sql = v_sqlheader + r[0].ToString() + ", ";
                        for (k = 1; k < p_table.Columns.Count; k++)
                            v_sql += ", " + r[k].ToString();
                        v_sql += ")";

                        v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(v_sql), v_fbcon);
                        v_fbcmd.ExecuteNonQuery();
                    }
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }
        }
    }
}

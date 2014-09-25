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
            FirebirdSql.Data.FirebirdClient.FbDataAdapter v_fbadp;
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;
            string v_context;

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
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Database.Exception(v_context, e);
                }
            }

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
        /// <param name='p_table'>
        /// Tabela que contém definições de nomes e tipos de colunas.
        /// </param>
        /// <returns>Retorna uma <see cref="System.Data.DataTable"/> com os dados de retorno da consulta.</returns>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar a consulta.</exception>
        public override System.Data.DataTable Query(string p_sql, string p_tablename, System.Data.DataTable p_table)
        {
            System.Data.DataTable v_table = null, v_tabletmp = null;
            FirebirdSql.Data.FirebirdClient.FbDataAdapter v_fbadp;
            FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;
            System.Data.DataRow v_row;
            string v_context;
            int k;

            using (FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring))
            {
                try
                {
                    v_fbcon.Open();

                    v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(p_sql, v_fbcon);
                    v_fbadp = new FirebirdSql.Data.FirebirdClient.FbDataAdapter(v_fbcmd);
                    v_tabletmp = new System.Data.DataTable(p_tablename);
                    v_fbadp.Fill(v_tabletmp);

                    v_table = v_tabletmp.Clone();

                    for (k = 0; k < v_table.Columns.Count && k < p_table.Columns.Count; k++)
                    {
                        v_table.Columns [k].ColumnName = p_table.Columns [k].ColumnName;
                        v_table.Columns [k].DataType = p_table.Columns [k].DataType;
                    }

                    foreach (System.Data.DataRow r in v_tabletmp.Rows)
                    {
                        v_row = v_table.NewRow();

                        for (k = 0; k < v_table.Columns.Count; k++)
                            v_row [k] = r [k];

                        v_table.Rows.Add(v_row);
                    }
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException e)
                {
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Database.Exception(v_context, e);
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
            string v_context;

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
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Database.Exception(v_context, e);
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
            string v_context;
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
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Database.Exception(v_context, e);
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
            string v_context;
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
                catch (System.Data.Odbc.OdbcException e)
                {
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Database.Exception(v_context, e);
                }
            }
        }
    }
}

using System;
using System.Data;
using MySql;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Mysql.
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza o MySQL .NET Provider para acessar um SGBD MySQL.
    /// </summary>
    public class Mysql : Spartacus.Database.Generic
    {
        /// <summary>
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Mysql"/>.
        /// </summary>
        /// <param name='p_server'>
        /// IP do servidor MySQL.
        /// </param>
        /// <param name='p_port'>
        /// Porta de conexão.
        /// </param>
        /// <param name='p_database'>
        /// Nome da base de dados ou schema.
        /// </param>
        /// <param name='p_user'>
        /// Usuário do MySQL.
        /// </param>
        /// <param name='p_password'>
        /// Senha do MySQL.
        /// </param>
        public Mysql(string p_server, string p_port, string p_database, string p_user, string p_password)
            : base(p_server, p_port, p_database, p_user, p_password)
        {
            this.v_connectionstring = "Persist Security Info=False;"
                + "Server=" + p_server + ";"
                + "Port=" + p_port + ";"
                + "Database=" + p_database + ";"
                + "Uid=" + p_user + ";"
                + "Pwd=" + p_password;
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
            MySql.Data.MySqlClient.MySqlDataAdapter v_myadp;
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            string v_context;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();

                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(p_sql, v_mycon);
                    v_myadp = new MySql.Data.MySqlClient.MySqlDataAdapter(v_mycmd);
                    v_table = new System.Data.DataTable(p_tablename);
                    v_myadp.Fill(v_table);
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
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
            MySql.Data.MySqlClient.MySqlDataAdapter v_myadp;
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            System.Data.DataRow v_row;
            string v_context;
            int k;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();

                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(p_sql, v_mycon);
                    v_myadp = new MySql.Data.MySqlClient.MySqlDataAdapter(v_mycmd);
                    v_tabletmp = new System.Data.DataTable(p_tablename);
                    v_myadp.Fill(v_tabletmp);

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
                catch (MySql.Data.MySqlClient.MySqlException e)
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
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            string v_context;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();

                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_mycon);
                    v_mycmd.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Database.Exception(v_context, "Não conseguiu se conectar a {0}/{1}@{2}:{3}/{4}.", e, this.v_user, this.v_password, this.v_host, this.v_port, this.v_service);
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
            MySql.Data.MySqlClient.MySqlCommand v_mycmd;
            string v_context;
            string v_ret;

            using (MySql.Data.MySqlClient.MySqlConnection v_mycon = new MySql.Data.MySqlClient.MySqlConnection(this.v_connectionstring))
            {
                try
                {
                    v_mycon.Open();

                    v_mycmd = new MySql.Data.MySqlClient.MySqlCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_mycon);
                    v_ret = v_mycmd.ExecuteScalar().ToString();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Database.Exception(v_context, "Não conseguiu se conectar a {0}/{1}@{2}:{3}/{4}.", e, this.v_user, this.v_password, this.v_host, this.v_port, this.v_service);
                }
            }

            return v_ret;
        }
    }
}

using System;
using System.Data;
using Npgsql;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Postgresql.
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza o Npgsql .NET Provider para acessar um SGBD PostgreSQL.
    /// </summary>
    public class Postgresql : Spartacus.Database.Generic
    {
        /// <summary>
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// PostgreSQL Connection.
        /// </summary>
        Npgsql.NpgsqlConnection v_pgcon;

        /// <summary>
        /// PostgreSQL Command.
        /// </summary>
        Npgsql.NpgsqlCommand v_pgcmd;

        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Postgresql"/>.
        /// </summary>
        /// <param name='p_server'>
        /// IP do servidor PostgreSQL.
        /// </param>
        /// <param name='p_port'>
        /// Porta de conexão.
        /// </param>
        /// <param name='p_database'>
        /// Nome da base de dados ou schema.
        /// </param>
        /// <param name='p_user'>
        /// Usuário do PostgreSQL.
        /// </param>
        /// <param name='p_password'>
        /// Senha do PostgreSQL.
        /// </param>
        public Postgresql(string p_server, string p_port, string p_database, string p_user, string p_password)
            : base(p_server, p_port, p_database, p_user, p_password)
        {
            this.v_connectionstring = "Server=" + p_server + ";"
                    + "Port=" + p_port + ";"
                    + "Database=" + p_database + ";"
                    + "User ID=" + p_user + ";"
                    + "Password=" + p_password;

            this.v_pgcon = new Npgsql.NpgsqlConnection(this.v_connectionstring);
        }

        /// <summary>
        /// Conectar ao banco de dados.
        /// </summary>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível se conectar ao banco de dados.</exception>
        public override void Connect ()
        {
            string v_context;

            try
            {
                this.v_pgcon.Open();
            }
            catch (Npgsql.NpgsqlException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, "Não conseguiu se conectar a {0}/{1}@{2}:{3}/{4}.", e, this.v_user, this.v_password, this.v_host, this.v_port, this.v_service);
            }
        }

        /// <summary>
        /// Desconectar do banco de dados.
        /// </summary>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível se desconectar do banco de dados.</exception>
        public override void Disconnect()
        {
            string v_context;

            try
            {
                this.v_pgcon.Close();
                this.v_pgcon.Dispose();
                this.v_pgcon = null;
                this.v_pgcmd.Dispose();
                this.v_pgcmd = null;
            }
            catch (Npgsql.NpgsqlException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, e);
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
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando não for possível executar a consulta.</exception>
        public override System.Data.DataTable Query(string p_sql, string p_tablename)
        {
            System.Data.DataTable v_table = null;
            Npgsql.NpgsqlDataAdapter v_pgadp;
            string v_context;

            try
            {
                this.v_pgcmd = new Npgsql.NpgsqlCommand(p_sql, this.v_pgcon);
                v_pgadp = new Npgsql.NpgsqlDataAdapter(this.v_pgcmd);

                v_table = new System.Data.DataTable(p_tablename);
                v_pgadp.Fill(v_table);

                v_pgadp.Dispose();
                v_pgadp = null;
            }
            catch (Npgsql.NpgsqlException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, e);
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
            Npgsql.NpgsqlDataAdapter v_pgadp;
            System.Data.DataRow v_row;
            string v_context;
            int k;

            try
            {
                this.v_pgcmd = new Npgsql.NpgsqlCommand(p_sql, this.v_pgcon);
                v_pgadp = new Npgsql.NpgsqlDataAdapter(this.v_pgcmd);
                v_tabletmp = new System.Data.DataTable(p_tablename);
                v_pgadp.Fill(v_tabletmp);

                v_pgadp.Dispose();
                v_pgadp = null;

                v_table = v_tabletmp.Clone();

                for (k = 0; k < v_table.Columns.Count && k < p_table.Columns.Count; k++)
                {
                    v_table.Columns[k].ColumnName = p_table.Columns[k].ColumnName;
                    v_table.Columns[k].DataType = p_table.Columns[k].DataType;
                }

                foreach (System.Data.DataRow r in v_tabletmp.Rows)
                {
                    v_row = v_table.NewRow();

                    for (k = 0; k < v_table.Columns.Count; k++)
                        v_row[k] = r[k];

                    v_table.Rows.Add(v_row);
                }
            }
            catch (Npgsql.NpgsqlException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, e);
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
            string v_context;

            try
            {
                this.v_pgcmd = new Npgsql.NpgsqlCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_pgcon);
                this.v_pgcmd.ExecuteNonQuery();
            }
            catch (Npgsql.NpgsqlException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, "Não conseguiu se conectar a {0}/{1}@{2}:{3}/{4}.", e, this.v_user, this.v_password, this.v_host, this.v_port, this.v_service);
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
            string v_context;
            string v_ret;

            try
            {
                this.v_pgcmd = new Npgsql.NpgsqlCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_pgcon);
                v_ret = this.v_pgcmd.ExecuteScalar().ToString();
            }
            catch (Npgsql.NpgsqlException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, "Não conseguiu se conectar a {0}/{1}@{2}:{3}/{4}.", e, this.v_user, this.v_password, this.v_host, this.v_port, this.v_service);
            }

            return v_ret;
        }
    }
}

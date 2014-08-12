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
        /// Firebird Connection.
        /// </summary>
        FirebirdSql.Data.FirebirdClient.FbConnection v_fbcon;

        /// <summary>
        /// Firebird Command.
        /// </summary>
        FirebirdSql.Data.FirebirdClient.FbCommand v_fbcmd;

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

            this.v_fbcon = new FirebirdSql.Data.FirebirdClient.FbConnection(this.v_connectionstring);
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
                this.v_fbcon.Open();
            }
            catch (FirebirdSql.Data.FirebirdClient.FbException e)
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
                this.v_fbcon.Close();
                this.v_fbcon.Dispose();
                this.v_fbcon = null;
                this.v_fbcmd.Dispose();
                this.v_fbcmd = null;
            }
            catch (FirebirdSql.Data.FirebirdClient.FbException e)
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
            FirebirdSql.Data.FirebirdClient.FbDataAdapter v_fbadp;
            string v_context;

            try
            {
                this.v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(p_sql, this.v_fbcon);
                v_fbadp = new FirebirdSql.Data.FirebirdClient.FbDataAdapter(this.v_fbcmd);
                v_table = new System.Data.DataTable(p_tablename);
                v_fbadp.Fill(v_table);

                v_fbadp.Dispose();
                v_fbadp = null;
            }
            catch (FirebirdSql.Data.FirebirdClient.FbException e)
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
            FirebirdSql.Data.FirebirdClient.FbDataAdapter v_fbadp;
            System.Data.DataRow v_row;
            string v_context;
            int k;

            try
            {
                this.v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(p_sql, this.v_fbcon);
                v_fbadp = new FirebirdSql.Data.FirebirdClient.FbDataAdapter(this.v_fbcmd);
                v_tabletmp = new System.Data.DataTable(p_tablename);
                v_fbadp.Fill(v_tabletmp);

                v_fbadp.Dispose();
                v_fbadp = null;

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
            catch (FirebirdSql.Data.FirebirdClient.FbException e)
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
                this.v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_fbcon);
                this.v_fbcmd.ExecuteNonQuery();
            }
            catch (FirebirdSql.Data.FirebirdClient.FbException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, e);
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
                this.v_fbcmd = new FirebirdSql.Data.FirebirdClient.FbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_fbcon);
                v_ret = this.v_fbcmd.ExecuteScalar().ToString();
            }
            catch (FirebirdSql.Data.FirebirdClient.FbException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, e);
            }

            return v_ret;
        }
    }
}

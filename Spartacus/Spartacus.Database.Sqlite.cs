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
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Sqlite"/>.
        /// </summary>
        /// <param name='p_file'>
        /// Caminho para o arquivo DB.
        /// </param>
        public Sqlite(string p_file)
            : base(p_file)
        {
            this.v_connectionstring = "URI=file:" + p_file;
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
            Mono.Data.Sqlite.SqliteDataAdapter v_sqladp;
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            string v_context;

            using (Mono.Data.Sqlite.SqliteConnection v_sqlcon = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring))
            {
                try
                {
                    v_sqlcon.Open();

                    v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, v_sqlcon);
                    v_sqladp = new Mono.Data.Sqlite.SqliteDataAdapter(v_sqlcmd);
                    v_table = new System.Data.DataTable(p_tablename);
                    v_sqladp.Fill(v_table);
                }
                catch (Mono.Data.Sqlite.SqliteException e)
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
            Mono.Data.Sqlite.SqliteDataAdapter v_sqladp;
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            System.Data.DataRow v_row;
            string v_context;
            int k;

            using (Mono.Data.Sqlite.SqliteConnection v_sqlcon = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring))
            {
                try
                {
                    v_sqlcon.Open();

                    v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(p_sql, v_sqlcon);
                    v_sqladp = new Mono.Data.Sqlite.SqliteDataAdapter(v_sqlcmd);
                    v_tabletmp = new System.Data.DataTable(p_tablename);
                    v_sqladp.Fill(v_tabletmp);

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
                catch (Mono.Data.Sqlite.SqliteException e)
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
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            string v_context;

            using (Mono.Data.Sqlite.SqliteConnection v_sqlcon = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring))
            {
                try
                {
                    v_sqlcon.Open();

                    v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_sqlcon);
                    v_sqlcmd.ExecuteNonQuery();
                }
                catch (Mono.Data.Sqlite.SqliteException e)
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
            Mono.Data.Sqlite.SqliteCommand v_sqlcmd;
            string v_context;
            string v_ret;

            using (Mono.Data.Sqlite.SqliteConnection v_sqlcon = new Mono.Data.Sqlite.SqliteConnection(this.v_connectionstring))
            {
                try
                {
                    v_sqlcon.Open();

                    v_sqlcmd = new Mono.Data.Sqlite.SqliteCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), v_sqlcon);
                    v_ret = v_sqlcmd.ExecuteScalar().ToString();
                }
                catch (Mono.Data.Sqlite.SqliteException e)
                {
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Database.Exception(v_context, e);
                }
            }

            return v_ret;
        }
    }
}

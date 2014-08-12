using System;
using System.Data;
using System.Data.OleDb;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Oledb;
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza a implementação OLE DB para acessar qualquer SGBD.
    /// </summary>
    public class Oledb : Spartacus.Database.Generic
    {
        /// <summary>
        /// String de conexão para acessar o banco.
        /// </summary>
        public string v_connectionstring;

        /// <summary>
        /// OLE DB Connection.
        /// </summary>
        System.Data.OleDb.OleDbConnection v_olecon;

        /// <summary>
        /// OLE DB Command.
        /// </summary>
        System.Data.OleDb.OleDbCommand v_olecmd;

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
            if (p_provider == "Oracle")
            {
                this.v_connectionstring = "Provider=OraOLEDB.Oracle;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST="
                    + this.v_host + ")(PORT="
                        + this.v_port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME="
                        + this.v_service + ")));User Id="
                        + this.v_user + ";Password="
                        + this.v_password;
            }
            else
            {
                this.v_connectionstring = "Provider="
                    + p_provider + ";Addr="
                        + this.v_host + ";Port="
                        + this.v_port + ";Database="
                        + this.v_service + ";User Id="
                        + this.v_user + ";Password="
                        + this.v_password;
            }

            this.v_olecon = new System.Data.OleDb.OleDbConnection(this.v_connectionstring);
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
                this.v_olecon.Open();
            }
            catch (System.Data.OleDb.OleDbException e)
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
                this.v_olecon.Close();
                this.v_olecon.Dispose();
                this.v_olecon = null;
                this.v_olecmd.Dispose();
                this.v_olecmd = null;
            }
            catch (System.Data.OleDb.OleDbException e)
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
            System.Data.OleDb.OleDbDataAdapter v_oleadp;
            string v_context;

            try
            {
                this.v_olecmd = new System.Data.OleDb.OleDbCommand(p_sql, this.v_olecon);
                v_oleadp = new System.Data.OleDb.OleDbDataAdapter(this.v_olecmd);
                v_table = new System.Data.DataTable(p_tablename);
                v_oleadp.Fill(v_table);

                v_oleadp.Dispose();
                v_oleadp = null;
            }
            catch (System.Data.OleDb.OleDbException e)
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
            System.Data.OleDb.OleDbDataAdapter v_oleadp;
            System.Data.DataRow v_row;
            string v_context;
            int k;

            try
            {
                this.v_olecmd = new System.Data.OleDb.OleDbCommand(p_sql, this.v_olecon);
                v_oleadp = new System.Data.OleDb.OleDbDataAdapter(this.v_olecmd);
                v_tabletmp = new System.Data.DataTable(p_tablename);
                v_oleadp.Fill(v_tabletmp);

                v_oleadp.Dispose();
                v_oleadp = null;

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
            catch (System.Data.OleDb.OleDbException e)
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
                this.v_olecmd = new System.Data.OleDb.OleDbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_olecon);
                this.v_olecmd.ExecuteNonQuery();
            }
            catch (System.Data.OleDb.OleDbException e)
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
                this.v_olecmd = new System.Data.OleDb.OleDbCommand(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql), this.v_olecon);
                v_ret = this.v_olecmd.ExecuteScalar().ToString();
            }
            catch (System.Data.OleDb.OleDbException e)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Database.Exception(v_context, e);
            }

            return v_ret;
        }
    }
}


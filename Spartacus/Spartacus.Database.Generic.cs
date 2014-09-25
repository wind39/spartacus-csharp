using System;
using System.Data;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe abstrata Spartacus.Database.Generic.
    /// Armazena informações de conexão que são genéricas a qualquer SGBD.
    /// Provê polimorfismo, por ser uma classe abstrata.
    /// </summary>
    public abstract class Generic
    {
        /// <summary>
        /// Hostname ou IP onde o banco de dados está localizado.
        /// </summary>
        public string v_host;

        /// <summary>
        /// Porta TCP para conectar-se ao SGBG.
        /// </summary>
        public string v_port;

        /// <summary>
        /// Nome do serviço que representa o banco ao qual desejamos nos conectar.
        /// </summary>
        public string v_service;

        /// <summary>
        /// Usuário ou schema para se conectar ao banco de dados.
        /// </summary>
        public string v_user;

        /// <summary>
        /// A senha do usuário ou schema.
        /// </summary>
        public string v_password;

        /// <summary>
        /// DSN (Data Source Name)
        /// </summary>
        public string v_dsn;

        /// <summary>
        /// Arquivo do banco de dados.
        /// </summary>
        public string v_file;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// Armazena informações de conexão que são genéricas a qualquer SGBD.
        /// </summary>
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
        public Generic(string p_host, string p_port, string p_service, string p_user, string p_password)
        {
            this.v_host = p_host;
            this.v_port = p_port;
            this.v_service = p_service;
            this.v_user = p_user;
            this.v_password = p_password;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// Armazena informações de conexão que são genéricas a qualquer SGBD.
        /// </summary>
        /// <param name='p_file'>
        /// DSN (Data Source Name).
        /// </param>
        /// <param name='p_user'>
        /// Usuário ou schema para se conectar ao banco de dados.
        /// </param>
        /// <param name='p_password'>
        /// A senha do usuário ou schema.
        /// </param>
        public Generic(string p_dsn, string p_user, string p_password)
        {
            this.v_dsn = p_dsn;
            this.v_user = p_user;
            this.v_password = p_password;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Generic"/>.
        /// Armazena informações de conexão que são genéricas a qualquer SGBD.
        /// </summary>
        /// <param name='p_file'>
        /// Arquivo do banco de dados.
        /// </param>
        public Generic(string p_file)
        {
            this.v_file = p_file;
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_tablename'>
        /// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
        /// </param>
        public abstract System.Data.DataTable Query(string p_sql, string p_tablename);

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
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
        public abstract System.Data.DataTable Query(string p_sql, string p_tablename, System.Data.DataTable p_table);

        /// <summary>
        /// Executa uma instrução SQL no banco de dados.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser executado no banco de dados.
        /// </param>
        public abstract void Execute(string p_sql);

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando um único dado de retorno em uma string.
        /// </summary>
        /// <returns>
        /// string com o dado de retorno.
        /// </returns>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        public abstract string ExecuteScalar(string p_sql);

        /// <summary>
        /// Insere uma massa de dados.
        /// <paramref name="p_table"/> precisa ter o nome igual ao nome da tabela onde será inserido.
        /// Os nomes das colunas também precisam ser os mesmos.
        /// </summary>
        /// <param name="p_table">Tabela com os dados e definições para inserção em massa.</param>
        public abstract void BulkInsert(System.Data.DataTable p_table);
    }
}

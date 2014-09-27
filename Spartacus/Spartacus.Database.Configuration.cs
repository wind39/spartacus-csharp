using System;

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Configuration.
    /// Permite instanciar um banco de dados a partir de uma configuração.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Objeto de conexão com o banco de dados.
        /// </summary>
        public Spartacus.Database.Generic v_database;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Configuration"/>.
        /// </summary>
        /// <param name="p_type">Tipo.</param>
        /// <param name="p_provider">Provider.</param>
        /// <param name="p_host">Servidor.</param>
        /// <param name="p_port">Porta.</param>
        /// <param name="p_service">Serviço.</param>
        /// <param name="p_user">Usuário.</param>
        /// <param name="p_password">Senha.</param>
        public Configuration(
            string p_type,
            string p_provider,
            string p_host,
            string p_port,
            string p_service,
            string p_user,
            string p_password
        )
        {
            this.v_database = null;

            switch (p_type.ToLower())
            {
                case "firebird":
                    this.v_database = new Spartacus.Database.Firebird(p_host, p_port, p_service, p_user, p_password);
                    break;
                case "mysql":
                    this.v_database = new Spartacus.Database.Mysql(p_host, p_port, p_service, p_user, p_password);
                    break;
                case "odbc":
                    this.v_database = new Spartacus.Database.Odbc(p_service, p_user, p_password);
                    break;
                case "oledb":
                    this.v_database = new Spartacus.Database.Oledb(p_provider, p_host, p_port, p_service, p_user, p_password);
                    break;
                case "postgresql":
                    this.v_database = new Spartacus.Database.Postgresql(p_host, p_port, p_service, p_user, p_password);
                    break;
                case "sqlite":
                    this.v_database = new Spartacus.Database.Sqlite(p_service);
                    break;
                default:
                    this.v_database = null;
                    break;
            }
        }
    }
}


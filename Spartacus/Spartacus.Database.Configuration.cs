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
        /// <param name="p_integrated_security">Segurança integrada.</param>
        public Configuration(
            string p_type,
            string p_provider,
            string p_host,
            string p_port,
            string p_service,
            string p_user,
            string p_password,
            string p_integrated_security
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
                    if (p_service != null)
                        this.v_database = new Spartacus.Database.Sqlite(p_service);
                    else
                        this.v_database = new Spartacus.Database.Sqlite();
                    break;
                case "oracle":
                    this.v_database = new Spartacus.Database.Oracle(p_host, p_port, p_service, p_user, p_password);
                    break;
                case "memory":
                    this.v_database = new Spartacus.Database.Memory();
                    break;
                case "sqlserver":
                    this.v_database = new Spartacus.Database.SqlServer(p_host, p_port, p_service, p_user, p_password, bool.Parse(p_integrated_security));
                    break;
                case "access":
                    this.v_database = new Spartacus.Database.Access(p_service);
                    break;
                default:
                    this.v_database = null;
                    break;
            }
        }
    }
}

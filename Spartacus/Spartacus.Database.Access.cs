/*
The MIT License (MIT)

Copyright (c) 2014-2017 William Ivanski

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

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Spartacus.Database.Access.
    /// Herda da classe <see cref="Spartacus.Database.Generic"/>.
    /// Utiliza o driver JDBC UCanConnect (juntamente com Jackcess, HSQLDB, todos emulados por IKVM) para acessar um arquivo MDB ou ACCBD Microsoft Access.
    /// </summary>
    public class Access : Spartacus.Database.Generic
    {
        /// <summary>
        /// Conexão com o banco de dados.
        /// </summary>
        private java.sql.Connection v_con;

        /// <summary>
        /// Comando para conexão com o banco de dados.
        /// </summary>
        private java.sql.Statement v_cmd;

        /// <summary>
        /// Leitor de dados do banco de dados.
        /// </summary>
        private java.sql.ResultSet v_reader;

        /// <summary>
        /// Linha atual da QueryBlock.
        /// </summary>
        private uint v_currentrow;


        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Spartacus.Database.Access"/>.
        /// </summary>
        /// <param name='p_file'>
        /// Caminho para o arquivo MDB ou ACCDB.
        /// </param>
        public Access(string p_file)
            : base(p_file)
        {
            ikvm.runtime.Startup.addBootClassPathAssemby(System.Reflection.Assembly.Load("UCanAccess"));
            java.lang.Class.forName("net.ucanaccess.jdbc.UcanaccessDriver, UCanAccess, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

            this.v_con = null;
            this.v_cmd = null;
            this.v_reader = null;
            this.v_default_string = "text";
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do arquivo de banco de dados a ser criado.</param>
        public override void CreateDatabase(string p_name)
        {
            throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Access.CreateDatabase");
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        public override void CreateDatabase()
        {
            throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Access.CreateDatabase");
        }

        /// <summary>
        /// Abre a conexão com o banco de dados.
        /// </summary>
        public override void Open()
        {
            try
            {
                this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                this.v_cmd = this.v_con.createStatement();
                if (this.v_timeout > -1)
                    this.v_cmd.setQueryTimeout(this.v_timeout);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
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
        public override System.Data.DataTable Query(string p_sql, string p_tablename)
        {
            java.sql.ResultSetMetaData v_resmd;
            System.Data.DataTable v_table = null;
            System.Data.DataRow v_row;

            #if DEBUG
            Console.WriteLine("Spartacus.Database.Access.Query: " + p_sql);
            #endif

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_table = new System.Data.DataTable(p_tablename);
                    v_resmd = this.v_reader.getMetaData();
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        v_table.Columns.Add(v_resmd.getColumnLabel(i), typeof(string));

                    while (this.v_reader.next())
                    {
                        v_row = v_table.NewRow();
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_row[i-1] = this.v_reader.getString(i);
                        v_table.Rows.Add(v_row);
                    }

                    return v_table;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_table = new System.Data.DataTable(p_tablename);
                    v_resmd = this.v_reader.getMetaData();
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        v_table.Columns.Add(v_resmd.getColumnLabel(i), typeof(string));

                    while (this.v_reader.next())
                    {
                        v_row = v_table.NewRow();
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_row[i-1] = this.v_reader.getString(i);
                        v_table.Rows.Add(v_row);
                    }

                    return v_table;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
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
        /// <param name='p_progress'>Evento de progresso da execução da consulta.</param>
        /// <returns>Retorna uma <see cref="System.Data.DataTable"/> com os dados de retorno da consulta.</returns>
        public override System.Data.DataTable Query(string p_sql, string p_tablename, Spartacus.Utils.ProgressEventClass p_progress)
        {
            java.sql.ResultSetMetaData v_resmd;
            System.Data.DataTable v_table = null;
            System.Data.DataRow v_row;
            uint v_counter = 0;

            #if DEBUG
            Console.WriteLine("Spartacus.Database.Access.Query: " + p_sql);
            #endif

            p_progress.FireEvent(v_counter);

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_table = new System.Data.DataTable(p_tablename);
                    v_resmd = this.v_reader.getMetaData();
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        v_table.Columns.Add(v_resmd.getColumnLabel(i), typeof(string));

                    while (this.v_reader.next())
                    {
                        v_row = v_table.NewRow();
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_row[i-1] = this.v_reader.getString(i);
                        v_table.Rows.Add(v_row);

                        v_counter++;
                        p_progress.FireEvent(v_counter);
                    }

                    return v_table;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_table = new System.Data.DataTable(p_tablename);
                    v_resmd = this.v_reader.getMetaData();
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        v_table.Columns.Add(v_resmd.getColumnLabel(i), typeof(string));

                    while (this.v_reader.next())
                    {
                        v_row = v_table.NewRow();
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_row[i-1] = this.v_reader.getString(i);
                        v_table.Rows.Add(v_row);

                        v_counter++;
                        p_progress.FireEvent(v_counter);
                    }

                    return v_table;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
        /// Utiliza um DataReader para buscar em blocos. Conexão com o banco precisa estar aberta.
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
        /// <param name='p_hasmoredata'>
        /// Indica se ainda há mais dados a serem lidos.
        /// </param>
        public override System.Data.DataTable QueryBlock(string p_sql, string p_tablename, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            java.sql.ResultSetMetaData v_resmd;
            System.Data.DataTable v_table = null;
            System.Data.DataRow v_row;

            #if DEBUG
            Console.WriteLine("Spartacus.Database.Access.QueryBlock: " + p_sql);
            #endif

            try
            {
                if (this.v_reader == null)
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);
                    this.v_currentrow = 0;
                }

                v_table = new System.Data.DataTable(p_tablename);
                v_resmd = this.v_reader.getMetaData();
                for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                    v_table.Columns.Add(v_resmd.getColumnLabel(i), typeof(string));

                p_hasmoredata = false;
                while (this.v_reader.next())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        v_row = v_table.NewRow();
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_row[i-1] = this.v_reader.getString(i);
                        v_table.Rows.Add(v_row);
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.close();
                    this.v_reader = null;
                }

                return v_table;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma string HTML.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_id'>
        /// ID da tabela no HTML.
        /// </param>
        /// <param name='p_options'>
        /// Opções da tabela no HTML.
        /// </param>
        public override string QueryHtml(string p_sql, string p_id, string p_options)
        {
            java.sql.ResultSetMetaData v_resmd;
            string v_html;

            #if DEBUG
            Console.WriteLine("Spartacus.Database.Access.QueryHtml: " + p_sql);
            #endif

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_html = "<table id='" + p_id + "' " + p_options + "><thead><tr>";

                    v_resmd = this.v_reader.getMetaData();
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        v_html += "<th>" + v_resmd.getColumnLabel(i) + "</th>";

                    v_html += "</tr></thead><tbody>";

                    while (this.v_reader.next())
                    {
                        v_html += "<tr>";
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_html += "<td>" + this.v_reader.getString(i) + "</td>";
                        v_html += "</tr>";
                    }

                    v_html += "</tbody></table>";

                    return v_html;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_html = "<table id='" + p_id + "' " + p_options + "><thead><tr>";

                    v_resmd = this.v_reader.getMetaData();
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        v_html += "<th>" + v_resmd.getColumnLabel(i) + "</th>";

                    v_html += "</tr></thead><tbody>";

                    while (this.v_reader.next())
                    {
                        v_html += "<tr>";
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_html += "<td>" + this.v_reader.getString(i) + "</td>";
                        v_html += "</tr>";
                    }

                    v_html += "</tbody></table>";

                    return v_html;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em um <see creg="System.Data.DataTable"/>.
        /// Utiliza um DataReader para buscar em blocos a partir do cursor de saída de uma Stored Procedure.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <param name='p_tablename'>
        /// Nome virtual da tabela onde deve ser armazenado o resultado, para fins de cache.
        /// </param>
        /// <param name='p_outparam'>
        /// Nome do parâmetro de saída que deve ser um REF CURSOR.
        /// </param>
        /// <remarks>Não suportado em todos os SGBDs.</remarks>
        public override System.Data.DataTable QueryStoredProc(string p_sql, string p_tablename, string p_outparam)
        {
            throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Access.QueryStoredProc");
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma lista de objetos customizados.
        /// Utiliza um DataReader para buscar em blocos.
        /// </summary>
        /// <param name="p_sql">
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        /// <typeparam name="T">
        /// Tipo do objeto customizado (classe com propriedades).
        /// </typeparam>
        public override System.Collections.Generic.List<T> QueryList<T>(string p_sql)
        {
            java.sql.ResultSetMetaData v_resmd;
            System.Collections.Generic.List<T> v_list;
            System.Type v_type;
            T v_obj;
            System.Reflection.PropertyInfo v_prop;

            #if DEBUG
            Console.WriteLine("Spartacus.Database.Access.QueryList<T>: " + p_sql);
            #endif

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_resmd = this.v_reader.getMetaData();

                    v_list = new System.Collections.Generic.List<T>();
                    v_type = typeof(T);

                    while (this.v_reader.next())
                    {
                        v_obj = System.Activator.CreateInstance<T>();

                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        {
                            v_prop = v_type.GetProperty(v_resmd.getColumnLabel(i));
                            if (v_prop != null && v_resmd.getColumnLabel(i) == v_prop.Name && this.v_reader.getObject(i) != System.DBNull.Value)
                                v_prop.SetValue(v_obj, System.Convert.ChangeType(this.v_reader.getObject(i), v_prop.PropertyType), null);
                        }

                        v_list.Add(v_obj);
                    }

                    return v_list;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_resmd = this.v_reader.getMetaData();

                    v_list = new System.Collections.Generic.List<T>();
                    v_type = typeof(T);

                    while (this.v_reader.next())
                    {
                        v_obj = System.Activator.CreateInstance<T>();

                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        {
                            v_prop = v_type.GetProperty(v_resmd.getColumnLabel(i));
                            if (v_prop != null && v_resmd.getColumnLabel(i) == v_prop.Name && this.v_reader.getObject(i) != System.DBNull.Value)
                                v_prop.SetValue(v_obj, System.Convert.ChangeType(this.v_reader.getObject(i), v_prop.PropertyType), null);
                        }

                        v_list.Add(v_obj);
                    }

                    return v_list;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma lista de listas de string.
        /// Utiliza um DataReader para buscar em blocos.
        /// </summary>
        /// <param name="p_sql">
        /// Código SQL a ser consultado no banco de dados.
        /// </param>
        public override System.Collections.Generic.List<System.Collections.Generic.List<string>> QuerySList(string p_sql)
        {
            java.sql.ResultSetMetaData v_resmd;
            System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list;
            System.Collections.Generic.List<string> v_row;

            #if DEBUG
            Console.WriteLine("Spartacus.Database.Access.QuerySList: " + p_sql);
            #endif

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_resmd = this.v_reader.getMetaData();

                    v_list = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();

                    while (this.v_reader.next())
                    {
                        v_row = new System.Collections.Generic.List<string>();

                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_row.Add(this.v_reader.getString(i));

                        v_list.Add(v_row);
                    }

                    return v_list;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_resmd = this.v_reader.getMetaData();

                    v_list = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();

                    while (this.v_reader.next())
                    {
                        v_row = new System.Collections.Generic.List<string>();

                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            v_row.Add(this.v_reader.getString(i));

                        v_list.Add(v_row);
                    }

                    return v_list;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

		/// <summary>
		/// Realiza uma consulta no banco de dados, armazenando os dados de retorno em uma lista de listas de string.
		/// Utiliza um DataReader para buscar em blocos.
		/// </summary>
		/// <param name="p_sql">
		/// Código SQL a ser consultado no banco de dados.
		/// </param>
		/// <param name="p_header">
		/// Lista de nomes de colunas.
		/// </param>
		public override System.Collections.Generic.List<System.Collections.Generic.List<string>> QuerySList(string p_sql, out System.Collections.Generic.List<string> p_header)
		{
			java.sql.ResultSetMetaData v_resmd;
			System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list;
			System.Collections.Generic.List<string> v_row;

			#if DEBUG
			    Console.WriteLine("Spartacus.Database.Access.QuerySList: " + p_sql);
            #endif

			if (this.v_con == null)
			{
				try
				{
					this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
					this.v_cmd = this.v_con.createStatement();
					if (this.v_timeout > -1)
						this.v_cmd.setQueryTimeout(this.v_timeout);
					this.v_reader = this.v_cmd.executeQuery(p_sql);

					p_header = new System.Collections.Generic.List<string>();

					v_resmd = this.v_reader.getMetaData();
					for (int i = 1; i <= v_resmd.getColumnCount(); i++)
						p_header.Add(v_resmd.getColumnLabel(i));

					v_list = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();

					while (this.v_reader.next())
					{
						v_row = new System.Collections.Generic.List<string>();

						for (int i = 1; i <= v_resmd.getColumnCount(); i++)
							v_row.Add(this.v_reader.getString(i));

						v_list.Add(v_row);
					}

					return v_list;
				}
				catch (System.Exception e)
				{
					throw new Spartacus.Database.Exception(e);
				}
				finally
				{
					if (this.v_cmd != null)
					{
						try { this.v_cmd.cancel(); } catch {}
						this.v_cmd.close();
						this.v_cmd = null;
					}
					if (this.v_reader != null)
					{
						this.v_reader.close();
						this.v_reader = null;
					}
					if (this.v_con != null)
					{
						this.v_con.close();
						this.v_con = null;
					}
				}
			}
			else
			{
				try
				{
					this.v_reader = this.v_cmd.executeQuery(p_sql);

					p_header = new System.Collections.Generic.List<string>();

					v_resmd = this.v_reader.getMetaData();
					for (int i = 1; i <= v_resmd.getColumnCount(); i++)
						p_header.Add(v_resmd.getColumnLabel(i));

					v_list = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();

					while (this.v_reader.next())
					{
						v_row = new System.Collections.Generic.List<string>();

						for (int i = 1; i <= v_resmd.getColumnCount(); i++)
							v_row.Add(this.v_reader.getString(i));

						v_list.Add(v_row);
					}

					return v_list;
				}
				catch (System.Exception e)
				{
					throw new Spartacus.Database.Exception(e);
				}
				finally
				{
					if (this.v_reader != null)
					{
						this.v_reader.close();
						this.v_reader = null;
					}
				}
			}
		}

        /// <summary>
        /// Executa um código SQL no banco de dados.
        /// </summary>
        /// <param name='p_sql'>
        /// Código SQL a ser executado no banco de dados.
        /// </param>
        public override void Execute(string p_sql)
        {
            #if DEBUG
            Console.WriteLine("Spartacus.Database.Access.Execute: " + p_sql);
            #endif

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    if (this.v_execute_security)
                        this.v_cmd.execute(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql));
                    else
                        this.v_cmd.execute(p_sql);
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    if (this.v_execute_security)
                        this.v_cmd.execute(Spartacus.Database.Command.RemoveUnwantedCharsExecute(p_sql));
                    else
                        this.v_cmd.execute(p_sql);
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }
        }

        /// <summary>
        /// Insere um bloco de linhas em uma determinada tabela.
        /// </summary>
        /// <param name='p_table'>
        /// Nome da tabela a serem inseridas as linhas.
        /// </param>
        /// <param name='p_rows'>
        /// Lista de linhas a serem inseridas na tabela.
        /// </param>
        public override void InsertBlock(string p_table, System.Collections.Generic.List<string> p_rows)
        {
            string v_block;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);

                    v_block = "insert into " + p_table + " values \n";
                    v_block += p_rows[0];
                    for (int k = 1; k < p_rows.Count; k++)
                        v_block += ", \n" + p_rows[k];

                    #if DEBUG
                    Console.WriteLine("Spartacus.Database.Access.InsertBlock: " + v_block);
                    #endif

                    if (this.v_execute_security)
                        this.v_cmd.execute(Spartacus.Database.Command.RemoveUnwantedCharsExecute(v_block));
                    else
                        this.v_cmd.execute(v_block);
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    v_block = "insert into " + p_table + " values \n";
                    v_block += p_rows[0];
                    for (int k = 1; k < p_rows.Count; k++)
                        v_block += ", \n" + p_rows[k];

                    #if DEBUG
                    Console.WriteLine("Spartacus.Database.Access.InsertBlock: " + v_block);
                    #endif

                    if (this.v_execute_security)
                        this.v_cmd.execute(Spartacus.Database.Command.RemoveUnwantedCharsExecute(v_block));
                    else
                        this.v_cmd.execute(v_block);
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }
        }

        /// <summary>
        /// Insere um bloco de linhas em uma determinada tabela.
        /// </summary>
        /// <param name='p_table'>
        /// Nome da tabela a serem inseridas as linhas.
        /// </param>
        /// <param name='p_rows'>
        /// Lista de linhas a serem inseridas na tabela.
        /// </param>
        /// <param name='p_columnnames'>
        /// Nomes de colunas da tabela, entre parênteses, separados por vírgula.
        /// </param>
        public override void InsertBlock(string p_table, System.Collections.Generic.List<string> p_rows, string p_columnnames)
        {
            string v_block;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);

                    v_block = "insert into " + p_table + " values \n";
                    v_block += p_rows[0];
                    for (int k = 1; k < p_rows.Count; k++)
                        v_block += ", \n" + p_rows[k];

                    #if DEBUG
                    Console.WriteLine("Spartacus.Database.Access.InsertBlock: " + v_block);
                    #endif

                    if (this.v_execute_security)
                        this.v_cmd.execute(Spartacus.Database.Command.RemoveUnwantedCharsExecute(v_block));
                    else
                        this.v_cmd.execute(v_block);
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    v_block = "insert into " + p_table + " values \n";
                    v_block += p_rows[0];
                    for (int k = 1; k < p_rows.Count; k++)
                        v_block += ", \n" + p_rows[k];

                    #if DEBUG
                    Console.WriteLine("Spartacus.Database.Access.InsertBlock: " + v_block);
                    #endif

                    if (this.v_execute_security)
                        this.v_cmd.execute(Spartacus.Database.Command.RemoveUnwantedCharsExecute(v_block));
                    else
                        this.v_cmd.execute(v_block);
                }
                catch (System.Exception e)
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
        public override string ExecuteScalar(string p_sql)
        {
            object v_tmp;

            #if DEBUG
            Console.WriteLine("Spartacus.Database.Access.ExecuteScalar: " + p_sql);
            #endif

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);

                    this.v_reader = this.v_cmd.executeQuery(p_sql);
                    this.v_reader.next();

                    v_tmp = this.v_reader.getString(1);
                    if (v_tmp != null)
                        return v_tmp.ToString();
                    else
                        return "";
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);
                    this.v_reader.next();

                    v_tmp = this.v_reader.getString(1);
                    if (v_tmp != null)
                        return v_tmp.ToString();
                    else
                        return "";
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
            }
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados.
        /// </summary>
        public override void Close()
        {
            if (this.v_cmd != null)
            {
                try { this.v_cmd.cancel(); } catch {}
                this.v_cmd.close();
                this.v_cmd = null;
            }
            if (this.v_reader != null)
            {
                this.v_reader.close();
                this.v_reader = null;
            }
            if (this.v_con != null)
            {
                this.v_con.close();
                this.v_con = null;
            }
        }

        /// <summary>
        /// Deleta um banco de dados.
        /// </summary>
        /// <param name="p_name">Nome do banco de dados a ser deletado.</param>
        public override void DropDatabase(string p_name)
        {
            throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Access.DropDatabase");
        }

        /// <summary>
        /// Deleta o banco de dados conectado atualmente.
        /// </summary>
        public override void DropDatabase()
        {
            throw new Spartacus.Utils.NotSupportedException("Spartacus.Database.Access.DropDatabase");
        }

        /// <summary>
        /// Lista os nomes de colunas de uma determinada consulta.
        /// </summary>
        /// <returns>Vetor com os nomes de colunas.</returns>
        /// <param name="p_sql">Consulta SQL.</param>
        public override string[] GetColumnNames(string p_sql)
        {
            java.sql.ResultSetMetaData v_resmd;
            string[] v_array;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_resmd = this.v_reader.getMetaData();

                    v_array = new string[v_resmd.getColumnCount()];
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        v_array[i] = v_resmd.getColumnLabel(i);

                    return v_array;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_resmd = this.v_reader.getMetaData();

                    v_array = new string[v_resmd.getColumnCount()];
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                        v_array[i] = v_resmd.getColumnLabel(i);

                    return v_array;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Lista os nomes e tipos de colunas de uma determinada consulta.
        /// </summary>
        /// <returns>Matriz com os nomes e tipos de colunas.</returns>
        /// <param name="p_sql">Consulta SQL.</param>
        public override string[,] GetColumnNamesAndTypes(string p_sql)
        {
            java.sql.ResultSetMetaData v_resmd;
            string[,] v_matrix;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_resmd = this.v_reader.getMetaData();

                    v_matrix = new string[v_resmd.getColumnCount(), 2];
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                    {
                        v_matrix[i, 0] = v_resmd.getColumnLabel(i);
                        v_matrix[i, 1] = v_resmd.getColumnTypeName(i);
                    }

                    return v_matrix;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_sql);

                    v_resmd = this.v_reader.getMetaData();

                    v_matrix = new string[v_resmd.getColumnCount(), 2];
                    for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                    {
                        v_matrix[i, 0] = v_resmd.getColumnLabel(i);
                        v_matrix[i, 1] = v_resmd.getColumnTypeName(i);
                    }

                    return v_matrix;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

		/// <summary>
		/// Lista os campos (ou colunas) de uma determinada consulta.
		/// </summary>
		/// <returns>Vetor de campos.</returns>
		/// <param name="p_sql">Consulta SQL.</param>
		public override System.Collections.Generic.List<Spartacus.Database.Field> GetFields(string p_sql)
		{
			java.sql.ResultSetMetaData v_resmd;
			System.Collections.Generic.List<Spartacus.Database.Field> v_list;

			if (this.v_con == null)
			{
				try
				{
					this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
					this.v_cmd = this.v_con.createStatement();
					if (this.v_timeout > -1)
						this.v_cmd.setQueryTimeout(this.v_timeout);
					this.v_reader = this.v_cmd.executeQuery(p_sql);

					v_resmd = this.v_reader.getMetaData();

					v_list = new System.Collections.Generic.List<Spartacus.Database.Field>();
					for (int i = 1; i <= v_resmd.getColumnCount(); i++)
						//TODO: usar java.sql.Types para definir o tipo correto
						v_list.Add(new Spartacus.Database.Field(v_resmd.getColumnLabel(i), Spartacus.Database.Type.STRING));

					return v_list;
				}
				catch (System.Exception e)
				{
					throw new Spartacus.Database.Exception(e);
				}
				finally
				{
					if (this.v_cmd != null)
					{
						try { this.v_cmd.cancel(); } catch {}
						this.v_cmd.close();
						this.v_cmd = null;
					}
					if (this.v_reader != null)
					{
						this.v_reader.close();
						this.v_reader = null;
					}
					if (this.v_con != null)
					{
						this.v_con.close();
						this.v_con = null;
					}
				}
			}
			else
			{
				try
				{
					this.v_reader = this.v_cmd.executeQuery(p_sql);

					v_resmd = this.v_reader.getMetaData();

					v_list = new System.Collections.Generic.List<Spartacus.Database.Field>();
					for (int i = 1; i <= v_resmd.getColumnCount(); i++)
						//TODO: usar java.sql.Types para definir o tipo correto
						v_list.Add(new Spartacus.Database.Field(v_resmd.getColumnLabel(i), Spartacus.Database.Type.STRING));

					return v_list;
				}
				catch (System.Exception e)
				{
					throw new Spartacus.Database.Exception(e);
				}
				finally
				{
					if (this.v_reader != null)
					{
						this.v_reader.close();
						this.v_reader = null;
					}
				}
			}
		}

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    while (this.v_reader.next())
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        p_destdatabase.Execute(p_insert.GetUpdatedText());
                        v_transfered++;
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    while (this.v_reader.next())
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        p_destdatabase.Execute(p_insert.GetUpdatedText());
                        v_transfered++;
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_log">Log de inserção.</param>
        public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, out string p_log)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            string v_insert;

            p_log = "";

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    while (this.v_reader.next())
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                        }
                        catch (Spartacus.Database.Exception e)
                        {
                            p_log += v_insert + "\n" + e.v_message + "\n";
                        }
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    while (this.v_reader.next())
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                        }
                        catch (Spartacus.Database.Exception e)
                        {
                            p_log += v_insert + "\n" + e.v_message + "\n";
                        }
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);
                    this.v_currentrow = 0;
                }

                v_resmd = this.v_reader.getMetaData();

                p_hasmoredata = false;
                while (this.v_reader.next())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        p_destdatabase.Execute(p_insert.GetUpdatedText());
                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.close();
                    this.v_reader = null;
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_log">Log de inserção.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            string v_insert;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);
                    this.v_currentrow = 0;
                }

                v_resmd = this.v_reader.getMetaData();

                p_hasmoredata = false;
                while (this.v_reader.next())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                        }
                        catch (Spartacus.Database.Exception e)
                        {
                            p_log += v_insert + "\n" + e.v_message + "\n";
                        }
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.close();
                    this.v_reader = null;
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_table">Nome da tabela de destino.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public override uint Transfer(string p_query, string p_table, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            System.Collections.Generic.List<string> v_rows = new System.Collections.Generic.List<string>();
            string v_columnnames;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);
                    this.v_currentrow = 0;
                }

                v_resmd = this.v_reader.getMetaData();

                v_columnnames = "(" + v_resmd.getColumnLabel(1);
                for (int i = 2; i <= v_resmd.getColumnCount(); i++)
                    v_columnnames += "," + v_resmd.getColumnLabel(i);
                v_columnnames += ")";

                p_hasmoredata = false;
                while (this.v_reader.next())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_rows.Add(p_insert.GetUpdatedText());

                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.close();
                    this.v_reader = null;
                }
                else
                    p_destdatabase.InsertBlock(p_table, v_rows, v_columnnames);

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_table">Nome da tabela de destino.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_log">Log de inserção.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public override uint Transfer(string p_query, string p_table, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            System.Collections.Generic.List<string> v_rows = new System.Collections.Generic.List<string>();
            string v_columnnames;

            try
            {
                if (this.v_reader == null)
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);
                    this.v_currentrow = 0;
                }

                v_resmd = this.v_reader.getMetaData();

                v_columnnames = "(" + v_resmd.getColumnLabel(1);
                for (int i = 2; i <= v_resmd.getColumnCount(); i++)
                    v_columnnames += "," + v_resmd.getColumnLabel(i);
                v_columnnames += ")";

                p_hasmoredata = false;
                while (this.v_reader.next())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_rows.Add(p_insert.GetUpdatedText());

                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.close();
                    this.v_reader = null;
                }
                else
                {
                    try
                    {
                        p_destdatabase.InsertBlock(p_table, v_rows, v_columnnames);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_log += e.v_message + "\n";
                    }
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_table">Nome da tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas da tabela de destino.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public override uint Transfer(string p_query, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            System.Collections.Generic.List<string> v_rows = new System.Collections.Generic.List<string>();

            try
            {
                if (this.v_reader == null)
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);
                    this.v_currentrow = 0;
                }

                v_resmd = this.v_reader.getMetaData();

                p_hasmoredata = false;
                while (this.v_reader.next())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_rows.Add(p_insert.GetUpdatedText());

                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.close();
                    this.v_reader = null;
                }
                else
                    p_destdatabase.InsertBlock(p_table, v_rows, p_columns);

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_table">Nome da tabela de destino.</param>
        /// <param name="p_columns">Lista de colunas da tabela de destino.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_log">Log de inserção.</param>
        /// <param name='p_startrow'>Número da linha inicial.</param>
        /// <param name='p_endrow'>Número da linha final.</param>
        /// <param name='p_hasmoredata'>Indica se ainda há mais dados a serem lidos.</param>
        public override uint Transfer(string p_query, string p_table, string p_columns, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, ref string p_log, uint p_startrow, uint p_endrow, out bool p_hasmoredata)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            System.Collections.Generic.List<string> v_rows = new System.Collections.Generic.List<string>();

            try
            {
                if (this.v_reader == null)
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);
                    this.v_currentrow = 0;
                }

                v_resmd = this.v_reader.getMetaData();

                p_hasmoredata = false;
                while (this.v_reader.next())
                {
                    p_hasmoredata = true;

                    if (this.v_currentrow >= p_startrow && this.v_currentrow <= p_endrow)
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_rows.Add(p_insert.GetUpdatedText());

                        v_transfered++;
                    }

                    this.v_currentrow++;

                    if (this.v_currentrow > p_endrow)
                        break;
                }

                if (! p_hasmoredata)
                {
                    this.v_reader.close();
                    this.v_reader = null;
                }
                else
                {
                    try
                    {
                        p_destdatabase.InsertBlock(p_table, v_rows, p_columns);
                    }
                    catch (Spartacus.Database.Exception e)
                    {
                        p_log += e.v_message + "\n";
                    }
                }

                return v_transfered;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Database.Exception(e);
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um banco de dados de destino.
        /// Conexão com o banco de destino precisa estar aberta.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta SQL para buscar os dados no banco atual.</param>
        /// <param name="p_insert">Comando de inserção para inserir cada linha no banco de destino.</param>
        /// <param name="p_destdatabase">Conexão com o banco de destino.</param>
        /// <param name="p_progress">Evento de progresso.</param>
        /// <param name="p_error">Evento de erro.</param>
        public override uint Transfer(string p_query, Spartacus.Database.Command p_insert, Spartacus.Database.Generic p_destdatabase, Spartacus.Utils.ProgressEventClass p_progress, Spartacus.Utils.ErrorEventClass p_error)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            string v_insert;

            p_progress.FireEvent(v_transfered);

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    while (this.v_reader.next())
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                            p_progress.FireEvent(v_transfered);
                        }
                        catch (Spartacus.Database.Exception e)
                        {
                            p_error.FireEvent(v_insert + "\n" + e.v_message);
                        }
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    while (this.v_reader.next())
                    {
                        for (int i = 1; i <= v_resmd.getColumnCount(); i++)
                            p_insert.SetValue(v_resmd.getColumnLabel(i).ToLower(), this.v_reader.getString(i), this.v_execute_security);

                        v_insert = p_insert.GetUpdatedText();
                        try
                        {
                            p_destdatabase.Execute(v_insert);
                            v_transfered++;
                            p_progress.FireEvent(v_transfered);
                        }
                        catch (Spartacus.Database.Exception e)
                        {
                            p_error.FireEvent(v_insert + "\n" + e.v_message);
                        }
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo CSV.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        /// <param name="p_separator">Separador de campos.</param>
        /// <param name="p_delimiter">Delimitador de string.</param>
        /// <param name="p_header">Se a primeira linha é cabeçalho ou não.</param>
        /// <param name="p_encoding">Codificação do arquivo.</param>
        public override uint TransferToCSV(string p_query, string p_filename, string p_separator, string p_delimiter, bool p_header, System.Text.Encoding p_encoding)
        {
            java.sql.ResultSetMetaData v_resmd;
            System.IO.StreamWriter v_writer = null;
            uint v_transfered = 0;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    v_writer = new System.IO.StreamWriter(new System.IO.FileStream(p_filename, System.IO.FileMode.Create), p_encoding);
                    if (p_header)
                    {
                        v_writer.Write(p_delimiter + v_resmd.getColumnLabel(1).ToUpper() + p_delimiter);
                        for (int i = 2; i <= v_resmd.getColumnCount(); i++)
                            v_writer.Write(p_separator + p_delimiter + v_resmd.getColumnLabel(i).ToUpper() + p_delimiter);
                        v_writer.WriteLine();
                    }

                    while (this.v_reader.next())
                    {
                        v_writer.Write(p_delimiter + this.v_reader.getString(0) + p_delimiter);
                        for (int i = 2; i <= v_resmd.getColumnCount(); i++)
                            v_writer.Write(p_separator + p_delimiter + this.v_reader.getString(i) + p_delimiter);
                        v_writer.WriteLine();

                        v_transfered++;
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (v_writer != null)
                    {
                        v_writer.Close();
                        v_writer = null;
                    }
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    v_writer = new System.IO.StreamWriter(new System.IO.FileStream(p_filename, System.IO.FileMode.Create), p_encoding);
                    if (p_header)
                    {
                        v_writer.Write(p_delimiter + v_resmd.getColumnLabel(1).ToUpper() + p_delimiter);
                        for (int i = 2; i <= v_resmd.getColumnCount(); i++)
                            v_writer.Write(p_separator + p_delimiter + v_resmd.getColumnLabel(i).ToUpper() + p_delimiter);
                        v_writer.WriteLine();
                    }

                    while (this.v_reader.next())
                    {
                        v_writer.Write(p_delimiter + this.v_reader.getString(0) + p_delimiter);
                        for (int i = 2; i <= v_resmd.getColumnCount(); i++)
                            v_writer.Write(p_separator + p_delimiter + this.v_reader.getString(i) + p_delimiter);
                        v_writer.WriteLine();

                        v_transfered++;
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (v_writer != null)
                    {
                        v_writer.Close();
                        v_writer = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo XLSX.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        public override uint TransferToXLSX(string p_query, string p_filename)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            int i, j;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    using (OfficeOpenXml.ExcelPackage v_package = new OfficeOpenXml.ExcelPackage(new System.IO.FileInfo(p_filename)))
                    {
                        using (OfficeOpenXml.ExcelWorksheet v_worksheet = v_package.Workbook.Worksheets.Add("RESULT"))
                        {
                            v_worksheet.View.ShowGridLines = true;

                            for (j = 1; j <= v_resmd.getColumnCount(); j++)
                                v_worksheet.Cells[1, j+1].Value = v_resmd.getColumnLabel(j).ToUpper();

                            i = 2;
                            while (this.v_reader.next())
                            {
                                for (j = 1; j <= v_resmd.getColumnCount(); j++)
                                    v_worksheet.Cells[i, j+1].Value = this.v_reader.getString(j);
                                i++;

                                v_transfered++;
                            }

                            v_package.Save();
                        }
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    using (OfficeOpenXml.ExcelPackage v_package = new OfficeOpenXml.ExcelPackage(new System.IO.FileInfo(p_filename)))
                    {
                        using (OfficeOpenXml.ExcelWorksheet v_worksheet = v_package.Workbook.Worksheets.Add("RESULT"))
                        {
                            v_worksheet.View.ShowGridLines = true;

                            for (j = 1; j <= v_resmd.getColumnCount(); j++)
                                v_worksheet.Cells[1, j+1].Value = v_resmd.getColumnLabel(j).ToUpper();

                            i = 2;
                            while (this.v_reader.next())
                            {
                                for (j = 1; j <= v_resmd.getColumnCount(); j++)
                                    v_worksheet.Cells[i, j+1].Value = this.v_reader.getString(j);
                                i++;

                                v_transfered++;
                            }

                            v_package.Save();
                        }
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Transfere dados do banco de dados atual para um arquivo DBF.
        /// Não pára a execução se der um problema num comando de inserção específico.
        /// </summary>
        /// <returns>Número de linhas transferidas.</returns>
        /// <param name="p_query">Consulta a ser executada no banco de dados atual.</param>
        /// <param name="p_filename">Nome do arquivo de destino.</param>
        public override uint TransferToDBF(string p_query, string p_filename)
        {
            java.sql.ResultSetMetaData v_resmd;
            uint v_transfered = 0;
            SocialExplorer.IO.FastDBF.DbfFile v_dbf = null;
            SocialExplorer.IO.FastDBF.DbfRecord v_record;
            int j;

            if (this.v_con == null)
            {
                try
                {
                    this.v_con = java.sql.DriverManager.getConnection("jdbc:ucanaccess://" + this.v_service);
                    this.v_cmd = this.v_con.createStatement();
                    if (this.v_timeout > -1)
                        this.v_cmd.setQueryTimeout(this.v_timeout);
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    v_dbf = new SocialExplorer.IO.FastDBF.DbfFile(System.Text.Encoding.UTF8);
                    v_dbf.Open(p_filename, System.IO.FileMode.Create);

                    for (j = 1; j <= v_resmd.getColumnCount(); j++)
                        v_dbf.Header.AddColumn(new SocialExplorer.IO.FastDBF.DbfColumn(v_resmd.getColumnLabel(j).ToUpper(), SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Character, 254, 0));

                    while (this.v_reader.next())
                    {
                        v_record = new SocialExplorer.IO.FastDBF.DbfRecord(v_dbf.Header);
                        for (j = 1; j <= v_resmd.getColumnCount(); j++)
                            v_record[j] = this.v_reader.getString(j);
                        v_dbf.Write(v_record);

                        v_transfered++;
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (v_dbf != null)
                        v_dbf.Close();
                    if (this.v_cmd != null)
                    {
                        try { this.v_cmd.cancel(); } catch {}
                        this.v_cmd.close();
                        this.v_cmd = null;
                    }
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                    if (this.v_con != null)
                    {
                        this.v_con.close();
                        this.v_con = null;
                    }
                }
            }
            else
            {
                try
                {
                    this.v_reader = this.v_cmd.executeQuery(p_query);

                    v_resmd = this.v_reader.getMetaData();

                    v_dbf = new SocialExplorer.IO.FastDBF.DbfFile(System.Text.Encoding.UTF8);
                    v_dbf.Open(p_filename, System.IO.FileMode.Create);

                    for (j = 1; j <= v_resmd.getColumnCount(); j++)
                        v_dbf.Header.AddColumn(new SocialExplorer.IO.FastDBF.DbfColumn(v_resmd.getColumnLabel(j).ToUpper(), SocialExplorer.IO.FastDBF.DbfColumn.DbfColumnType.Character, 254, 0));

                    while (this.v_reader.next())
                    {
                        v_record = new SocialExplorer.IO.FastDBF.DbfRecord(v_dbf.Header);
                        for (j = 1; j <= v_resmd.getColumnCount(); j++)
                            v_record[j] = this.v_reader.getString(j);
                        v_dbf.Write(v_record);

                        v_transfered++;
                    }

                    return v_transfered;
                }
                catch (System.Exception e)
                {
                    throw new Spartacus.Database.Exception(e);
                }
                finally
                {
                    if (v_dbf != null)
                        v_dbf.Close();
                    if (this.v_reader != null)
                    {
                        this.v_reader.close();
                        this.v_reader = null;
                    }
                }
            }
        }
    }
}

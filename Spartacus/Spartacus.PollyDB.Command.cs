/*
The MIT License (MIT)

Copyright (c) 2014-2016 William Ivanski

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

namespace Spartacus.PollyDB
{
    /// <summary>
    /// Classe Command.
    /// Representa um comando a ser executado no SGBD, pela interface ADO.NET.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Conexão usada pelo comando.
        /// </summary>
        public Spartacus.PollyDB.Connection Connection;

        /// <summary>
        /// Texto usado pelo comando.
        /// </summary>
        public string CommandText;

        /// <summary>
        /// Timeout do comando em segundos (0 = sem timeout).
        /// </summary>
        public int CommandTimeout;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.PollyDB.Command"/>.
        /// </summary>
        public Command()
        {
            this.Connection = null;
            this.CommandText = "";
            this.CommandTimeout = 300;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.PollyDB.Command"/>.
        /// </summary>
        /// <param name="p_sql">Texto SQL.</param>
        public Command(string p_sql)
        {
            this.Connection = null;
            this.CommandText = p_sql;
            this.CommandTimeout = 300;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.PollyDB.Command"/>.
        /// </summary>
        /// <param name="p_sql">Texto SQL.</param>
        /// <param name="p_connection">Conexão a ser usada pelo comando.</param>
        public Command(string p_sql, Spartacus.PollyDB.Connection p_connection)
        {
            this.Connection = p_connection;
            this.CommandText = p_sql;
            this.CommandTimeout = 300;
        }

        /// <summary>
        /// Constrói e aplica o plano de execução, retornando um objeto da classe <see cref="Spartacus.PollyDB.DataReader"/>.
        /// </summary>
        /// <returns>Objeto de leitura sequencial do resultado da consulta.</returns>
        public Spartacus.PollyDB.DataReader ExecuteReader()
        {
            Spartacus.PollyDB.DataReader v_reader;
            Spartacus.PollyDB.Parser v_parser;
            Spartacus.PollyDB.Scan v_scan;
            Spartacus.PollyDB.NestedLoop v_loop;

            v_parser = new Spartacus.PollyDB.Parser(this.Connection);

            //TODO: suportar outros tipos de comando
            if (v_parser.Parse(this.CommandText) != Spartacus.PollyDB.CommandType.SELECT)
                throw new Spartacus.PollyDB.Exception("Wrong type of command.");

            v_reader = new Spartacus.PollyDB.DataReader(this.Connection);

            foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Relation> kvp in v_parser.v_query.v_relations)
            {
                v_scan = new Spartacus.PollyDB.CsvScan(kvp.Value.v_name, kvp.Value.v_alias, this.Connection);
                v_scan.Open(kvp.Value.v_columns);
                v_reader.AddScan(v_scan);
            }

            v_loop = new Spartacus.PollyDB.NestedLoop(v_reader, v_parser.v_query.v_conditions_expression);

            v_reader.SetIndex(v_loop.GetIndex());
            v_reader.SetProjection(v_parser.v_query.v_projection);

            return v_reader;
        }

        /// <summary>
        /// Constrói e aplica o plano de execução, retornando apenas a primeira coluna da primeira linha do resultado.
        /// </summary>
        /// <returns>Primeira coluna da primeira linha do resultado.</returns>
        public object ExecuteScalar()
        {
            throw new Spartacus.Utils.NotImplementedException("Spartacus.PollyDB.Command.ExecuteScalar");
        }

        /// <summary>
        /// Executa um comando DML ou DDL no banco de dados.
        /// </summary>
        public void ExecuteNonQuery()
        {
            throw new Spartacus.Utils.NotImplementedException("Spartacus.PollyDB.Command.ExecuteNonQuery");
        }
    }
}

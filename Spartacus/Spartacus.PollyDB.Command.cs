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
    public class Command
    {
        public Spartacus.PollyDB.Connection Connection;
        public string CommandText;
        public int CommandTimeout;

        public Command()
        {
            this.Connection = null;
            this.CommandText = "";
            this.CommandTimeout = 300;
        }

        public Command(string p_sql)
        {
            this.Connection = null;
            this.CommandText = p_sql;
            this.CommandTimeout = 300;
        }

        public Command(string p_sql, Spartacus.PollyDB.Connection p_connection)
        {
            this.Connection = p_connection;
            this.CommandText = p_sql;
            this.CommandTimeout = 300;
        }

        public Spartacus.PollyDB.DataReader ExecuteReader()
        {
            Spartacus.PollyDB.DataReader v_reader;
            Spartacus.PollyDB.Scan v_scan;

            v_reader = new Spartacus.PollyDB.DataReader(this.Connection);

            v_scan = new Spartacus.PollyDB.CsvScan(this.CommandText.Replace("select * from ", ""), this.Connection);
            v_scan.Select(null);
            //TODO: tratar lista de colunas
            v_scan.StartRead(v_scan.v_all_columns);
            v_reader.AddScan(v_scan);

            return v_reader;
        }

        public object ExecuteScalar()
        {
            return null;
        }

        public void ExecuteNonQuery()
        {
        }
    }
}

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
    public class CsvScan : Spartacus.PollyDB.Scan
    {
        private System.IO.StreamReader r = null;

        public CsvScan(string p_tablename, Spartacus.PollyDB.Connection p_connection)
            : base(p_tablename, p_connection)
        {
        }

        public override void Select(string p_filter)
        {
            string[] v_line;
            uint i, j;

            try
            {
                this.r = new System.IO.StreamReader(this.v_tablename, this.v_connection.v_encoding);

                i = 0;
                while (! this.r.EndOfStream)
                {
                    v_line = this.r.ReadLine().Split(this.v_connection.v_separator);

                    if (i == 0)
                    {
                        if (this.v_connection.v_header)
                        {
                            for (j = 0; j < v_line.Length; j++)
                                this.v_all_columns.Add(v_line[j]);
                        }
                        else
                        {
                            for (j = 0; j < v_line.Length; j++)
                                this.v_all_columns.Add("col" + j.ToString());

                            //TODO: tratar filtro
                            this.v_rowids.Add(i);
                        }
                    }
                    else
                    {
                        //TODO: tratar filtro
                        if (v_line.Length == this.v_all_columns.Count)
                            this.v_rowids.Add(i);
                    }

                    i++;
                }
            }
            catch (System.Exception e)
            {
                throw new Spartacus.PollyDB.Exception("Spartacus.PollyDB.CsvScan.Select: Read error on file {0}.", e, this.v_tablename);
            }
            finally
            {
                if (this.r != null)
                {
                    this.r.Close();
                    this.r = null;
                }
            }
        }

        public override void StartRead(System.Collections.Generic.List<string> p_columns)
        {
            try
            {
                //TODO: tratar colunas requeridas que não existem na tabela
                for (int i = 0; i < p_columns.Count; i++)
                {
                    for (int j = 0; j < this.v_all_columns.Count; j++)
                    {
                        if (p_columns[i] == this.v_all_columns[j])
                        {
                            this.v_colids.Add((uint) j);
                            this.v_columns.Add(p_columns[i]);
                        }
                    }
                }

                this.r = new System.IO.StreamReader(this.v_tablename, this.v_connection.v_encoding);

                if (this.v_connection.v_header)
                {
                    this.r.ReadLine();
                    this.v_currentfilerowid = 1;
                }
                else
                    this.v_currentfilerowid = 0;
                
                this.v_currentrowid = 0;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.PollyDB.Exception("Spartacus.PollyDB.CsvScan.StartRead: Read error on file {0}.", e, this.v_tablename);
            }
        }

        public override System.Collections.Generic.List<string> Read()
        {
            uint v_row;
            string v_tmp = "";
            string[] v_line;
            int j;

            v_row = this.v_rowids[(int) this.v_currentrowid];

            try
            {
                if (this.r == null || this.v_currentfilerowid > v_row)
                {
                    if (this.r != null)
                    {
                        this.r.Close();
                        this.r = null;
                    }

                    this.r = new System.IO.StreamReader(this.v_tablename, this.v_connection.v_encoding);

                    if (this.v_connection.v_header)
                    {
                        this.v_currentfilerowid = 1;
                        this.r.ReadLine();
                    }
                    else
                        this.v_currentfilerowid = 0;

                    this.v_currentrowid = 0;
                }

                if (this.v_currentfilerowid <= v_row)
                {
                    do
                    {
                        v_tmp = this.r.ReadLine();
                        this.v_currentfilerowid++;
                    }
                    while (this.v_currentfilerowid <= v_row);

                    v_line = v_tmp.Split(this.v_connection.v_separator);

                    if (v_line.Length == this.v_all_columns.Count)
                    {
                        this.v_currentrow = new System.Collections.Generic.List<string>();

                        for (j = 0; j < this.v_colids.Count; j++)
                            this.v_currentrow.Add(v_line[this.v_colids[j]]);
                    }
                    else
                    {
                        if (v_line.Length > 1)
                            throw new Spartacus.PollyDB.Exception("Spartacus.PollyDB.CsvScan.Read: Unexpected number of columns. It is {0} and should be {1}. Row: {2}", v_line.Length, this.v_colids.Count, v_tmp);
                    }
                }

                return this.v_currentrow;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.PollyDB.Exception("Spartacus.PollyDB.CsvScan.Read: Read error on file {0}.", e, this.v_tablename);
            }
        }

        public override void Next()
        {
            this.v_currentrowid++;
        }

        public override void StopRead()
        {
            if (this.r != null)
            {
                this.r.Close();
                this.r = null;
            }
        }
    }
}

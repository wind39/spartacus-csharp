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

        public CsvScan(string p_relationname, string p_relationalias, Spartacus.PollyDB.Connection p_connection)
            : base(p_relationname, p_relationalias, p_connection)
        {
        }

        public override void Open(System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Column> p_columns)
        {
            string[] v_line;
            uint i;
            int j;

            try
            {
                this.r = new System.IO.StreamReader(this.v_relationname, this.v_connection.v_encoding);

                i = 0;
                while (! this.r.EndOfStream)
                {
                    v_line = this.r.ReadLine().Split(this.v_connection.v_separator);

                    if (i == 0)
                    {
                        if (this.v_connection.v_header)
                        {
                            for (j = 0; j < v_line.Length; j++)
                                this.v_all_columns.Add(v_line[j].ToLower());
                        }
                        else
                        {
                            for (j = 0; j < v_line.Length; j++)
                                this.v_all_columns.Add("col" + j.ToString());
                            
                            this.v_rowids.Add(i);
                        }

                        foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Column> kvp in p_columns)
                        {
                            if (!this.v_all_columns.Contains(kvp.Value.v_name))
                                throw new Spartacus.PollyDB.Exception("Column '{0}' does not exist in relation '{1}'.", kvp.Value.v_name, kvp.Value.v_relationalias);
                        }

                        for (j = 0; j < this.v_all_columns.Count; j++)
                        {
                            foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Column> kvp in p_columns)
                            {
                                if (kvp.Value.v_name == this.v_all_columns[j])
                                {
                                    this.v_colids.Add(j);
                                    this.v_columns.Add(kvp.Value.v_name);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (v_line.Length == this.v_all_columns.Count)
                            this.v_rowids.Add(i);
                    }

                    i++;
                }

                this.r = new System.IO.StreamReader(this.v_relationname, this.v_connection.v_encoding);

                if (this.v_connection.v_header)
                {
                    this.r.ReadLine();
                    this.v_currentfilerowid = 1;
                }
                else
                    this.v_currentfilerowid = 0;

                this.v_currentrowid = 0;
            }
            catch (Spartacus.PollyDB.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.PollyDB.Exception("Read error on file {0}.", e, this.v_relationname);
            }
        }

        public override System.Collections.Generic.List<string> Read(uint p_row)
        {
            uint v_row;
            string v_tmp = "";
            string[] v_line;
            int j;

            v_row = this.v_rowids[(int) p_row];

            try
            {
                if (this.r == null || this.v_currentfilerowid > v_row)
                {
                    if (this.r != null)
                    {
                        this.r.Close();
                        this.r = null;
                    }

                    this.r = new System.IO.StreamReader(this.v_relationname, this.v_connection.v_encoding);

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
                            throw new Spartacus.PollyDB.Exception("Unexpected number of columns. It is {0} and should be {1}. Row: {2}", v_line.Length, this.v_colids.Count, v_tmp);
                    }
                }

                return this.v_currentrow;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.PollyDB.Exception("Read error on file {0}.", e, this.v_relationname);
            }
        }

        public override void Close()
        {
            if (this.r != null)
            {
                this.r.Close();
                this.r = null;
            }
        }
    }
}

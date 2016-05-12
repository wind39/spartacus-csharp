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
    public class DataReader
    {
        public Spartacus.PollyDB.Connection v_connection;

        public System.Collections.Generic.List<Spartacus.PollyDB.Scan> v_scanlist;
        public System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Column> v_columns;

        public int FieldCount
        {
            get
            {
                return this.v_columns.Count;
            }
        }

        private int v_currentrowid;

        private System.Collections.Generic.List<string> v_currentrow;
        private System.Collections.Generic.List<int[]> v_index;

        public DataReader(Spartacus.PollyDB.Connection p_connection)
        {
            this.v_connection = p_connection;
            this.v_scanlist = new System.Collections.Generic.List<Scan>();
            this.v_currentrowid = 0;
        }

        public void AddScan(Spartacus.PollyDB.Scan p_scan)
        {
            this.v_scanlist.Add(p_scan);
        }

        public void SetIndex(System.Collections.Generic.List<int[]> p_index)
        {
            this.v_index = p_index;
        }

        public void SetProjection(System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Column> p_columns)
        {
            if (p_columns.Count > 0)
                this.v_columns = p_columns;
            else
            {
                this.v_columns = new System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Column>();

                foreach (Spartacus.PollyDB.Scan v_scan in this.v_scanlist)
                {
                    foreach (string v_column in v_scan.v_columns)
                    {
                        try
                        {
                            this.v_columns.Add(v_column, new Spartacus.PollyDB.Column(v_scan.v_relationalias, v_column));
                        }
                        catch (System.Exception)
                        {
                            throw new Spartacus.PollyDB.Exception("Ambiguous definition for column {0}.", v_column);
                        }
                    }
                }
            }
        }

        public bool Read()
        {
            System.Collections.Generic.List<string> v_row;

            if (this.v_currentrowid < this.v_index.Count)
            {
                this.v_currentrow = new System.Collections.Generic.List<string>();

                for (int i = 0; i < this.v_scanlist.Count; i++)
                {
                    v_row = this.v_scanlist[i].Read(this.v_index[this.v_currentrowid][i]);

                    for (int j = 0; j < v_row.Count; j++)
                    {
                        foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Column> kvp in this.v_columns)
                        {
                            if (kvp.Value.v_relationalias == this.v_scanlist[i].v_relationalias &&
                                kvp.Value.v_name == this.v_scanlist[i].v_columns[j])
                                this.v_currentrow.Add(v_row[j]);
                        }
                    }
                }

                this.v_currentrowid++;

                return true;
            }
            else
                return false;
        }

        public string GetValue(int p_column)
        {
            return this.v_currentrow[p_column];
        }

        public string GetName(int p_column)
        {
            int k = 0;
            foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Column> kvp in this.v_columns)
            {
                if (k == p_column)
                    return kvp.Value.v_name;
                else
                    k++;
            }
            return "";
        }

        public string GetDataTypeName(int p_column)
        {
            return "text";
        }

        public void Close()
        {
            foreach (Spartacus.PollyDB.Scan v_scan in this.v_scanlist)
                v_scan.Close();
        }
    }
}

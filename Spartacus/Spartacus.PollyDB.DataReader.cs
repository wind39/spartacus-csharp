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

        public int FieldCount
        {
            get
            {
                return this.v_columns.Count;
            }
        }

        private uint v_currentrowid;
        private uint v_totalrows;

        private System.Collections.Generic.List<string> v_currentrow;
        private System.Collections.Generic.List<string> v_columns;

        public DataReader(Spartacus.PollyDB.Connection p_connection)
        {
            this.v_connection = p_connection;
            this.v_scanlist = new System.Collections.Generic.List<Scan>();
            this.v_columns = new System.Collections.Generic.List<string>();
            this.v_currentrowid = 0;
            this.v_totalrows = 0;
        }

        public void AddScan(Spartacus.PollyDB.Scan p_scan)
        {
            this.v_scanlist.Add(p_scan);
            this.v_columns.AddRange(p_scan.v_columns);
            if (p_scan.v_rowids.Count > this.v_totalrows)
                this.v_totalrows = (uint) p_scan.v_rowids.Count;
        }

        public bool Read()
        {
            if (this.v_currentrowid < this.v_totalrows)
            {
                this.v_currentrow = new System.Collections.Generic.List<string>();

                for (int k = 0; k < this.v_scanlist.Count; k++)
                {
                    if (this.v_currentrowid < this.v_scanlist[k].v_rowids.Count)
                    {
                        this.v_currentrow.AddRange(this.v_scanlist[k].Read());
                        this.v_scanlist[k].Next();
                    }
                }

                //TODO: reordenar colunas para atender ordem de colunas da query

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
            return this.v_columns[p_column];
        }

        public string GetDataTypeName(int p_column)
        {
            return "text";
        }

        public void Close()
        {
            foreach (Spartacus.PollyDB.Scan v_scan in this.v_scanlist)
                v_scan.StopRead();
        }
    }
}

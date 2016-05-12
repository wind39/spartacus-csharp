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
    public abstract class Scan
    {
        public Spartacus.PollyDB.Connection v_connection;
        public string v_relationname;
        public string v_relationalias;

        public System.Collections.Generic.List<string> v_all_columns;
        public System.Collections.Generic.List<string> v_columns;
        public System.Collections.Generic.List<int> v_colids;

        public System.Collections.Generic.List<int> v_rowids;
        public System.Collections.Generic.List<string> v_currentrow;

        public int v_currentrowid;
        public int v_currentfilerowid;

        public Scan(string p_relationname, string p_relationalias, Spartacus.PollyDB.Connection p_connection)
        {
            this.v_connection = p_connection;
            this.v_relationname = p_relationname;
            this.v_relationalias = p_relationalias;
            this.v_all_columns = new System.Collections.Generic.List<string>();
            this.v_columns = new System.Collections.Generic.List<string>();
            this.v_rowids = new System.Collections.Generic.List<int>();
            this.v_colids = new System.Collections.Generic.List<int>();
        }

        public abstract void Open(System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Column> p_columns);

        public abstract System.Collections.Generic.List<string> Read(int p_row);

        public abstract void Close();
    }
}

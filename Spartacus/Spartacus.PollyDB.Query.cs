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
    public class Query
    {
        public System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Column> v_projection;
        public System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Relation> v_relations;

        public Query()
        {
            this.v_projection = new System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Column>();
            this.v_relations = new System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Relation>();
        }

        public void AddProjectionColumn(string p_relationalias, string p_name)
        {
            Spartacus.PollyDB.Column c;

            try
            {
                c = new Spartacus.PollyDB.Column(p_relationalias, p_name);
                this.v_projection.Add(p_name, c);
            }
            catch (System.Exception)
            {
                throw new Spartacus.PollyDB.Exception("Ambiguous definition for column {0}.", p_name);
            }
        }

        public void AddProjectionColumn(string p_relationalias, string p_name, string p_alias)
        {
            Spartacus.PollyDB.Column c;

            try
            {
                c = new Spartacus.PollyDB.Column(p_relationalias, p_name, p_alias);
                this.v_projection.Add(p_alias, c);
            }
            catch (System.Exception)
            {
                throw new Spartacus.PollyDB.Exception("Ambiguous definition for column {0}.", p_alias);
            }
        }

        public void AddRelation(string p_name, string p_alias)
        {
            Spartacus.PollyDB.Relation r;

            try
            {
                r = new Spartacus.PollyDB.Relation(p_name, p_alias);
                this.v_relations.Add(p_alias, r);
            }
            catch (System.Exception)
            {
                throw new Spartacus.PollyDB.Exception("Ambiguous definition for relation {0}.", p_alias);
            }
        }
    }
}

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
    public class NestedLoop
    {
        private System.Collections.Generic.List<Spartacus.PollyDB.Scan> v_scanlist;
        private NCalc.Expression v_condition;

        private uint[] v_recvetor;

        private System.Collections.Generic.List<uint[]> v_index;


        public NestedLoop(Spartacus.PollyDB.DataReader p_reader, NCalc.Expression p_condition)
        {
            this.v_scanlist = p_reader.v_scanlist;
            this.v_condition = p_condition;

            this.v_recvetor = new uint[this.v_scanlist.Count];
            this.v_index = new System.Collections.Generic.List<uint[]>();

            this.NestedLoopRec(0);
        }

        public System.Collections.Generic.List<uint[]> GetIndex()
        {
            return this.v_index;
        }

        private void NestedLoopRec(int p_nivel)
        {
            uint[] v_candidate;

            if (p_nivel == this.v_scanlist.Count)
            {
                v_candidate = new uint[this.v_recvetor.Length];
                for (int i = 0; i < this.v_recvetor.Length; i++)
                    v_candidate[i] = this.v_recvetor[i];

                if (this.Filter(v_candidate))
                    this.v_index.Add(v_candidate);
            }
            else
            {
                foreach (uint r in this.v_scanlist[p_nivel].v_rowids)
                {
                    this.NestedLoopRec(p_nivel + 1);
                    this.v_recvetor[p_nivel]++;

                    if (p_nivel < this.v_scanlist.Count - 1)
                        this.v_recvetor[p_nivel + 1] = 0;
                }
            }
        }

        private bool Filter(uint[] p_candidate)
        {
            System.Collections.Generic.List<string> v_row;

            for (int i = 0; i < this.v_scanlist.Count; i++)
            {
                v_row = this.v_scanlist[i].Read(p_candidate[i]);

                for (int j = 0; j < v_row.Count; j++)
                {
                    try
                    {
                        this.v_condition.Parameters[this.v_scanlist[i].v_relationalias + "." + this.v_scanlist[i].v_columns[j]] = v_row[j];
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }

            return (bool) this.v_condition.Evaluate();
        }
    }
}

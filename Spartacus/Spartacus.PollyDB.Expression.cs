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
    public class Expression
    {
        private string[] v_start;
        private string[] v_end;

        public Expression(string[] p_start, string[] p_end)
        {
            this.v_start = p_start;
            this.v_end = p_end;
        }

        public System.Collections.Generic.List<string> Parse(System.Collections.Generic.List<string> p_current, out System.Collections.Generic.List<string> p_next)
        {
            System.Collections.Generic.List<string> v_expression = null;
            bool v_achou;
            int k;

            if (this.v_start.Length == 1)
            {
                if (p_current[0].ToLower() != this.v_start[0])
                    throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[0]);
                else
                {
                    v_expression = new System.Collections.Generic.List<string>();
                    v_expression.Add(this.v_start[0]);

                    v_achou = false;
                    k = 1;

                    while (k < p_current.Count && !v_achou)
                    {
                        if (this.MatchEnd(p_current[k].ToLower()))
                            v_achou = true;
                        else
                        {
                            v_expression.Add(p_current[k]);
                            k++;
                        }
                    }
                }
            }
            else
            {
                if (p_current[0].ToLower() != this.v_start[0] || p_current[1].ToLower() != this.v_start[1])
                    throw new Spartacus.PollyDB.Exception("Syntax error near '{0} {1}'.", p_current[0], p_current[1]);
                else
                {
                    v_expression = new System.Collections.Generic.List<string>();
                    v_expression.Add(this.v_start[0]);
                    v_expression.Add(this.v_start[1]);

                    v_achou = false;
                    k = 2;

                    while (k < p_current.Count && !v_achou)
                    {
                        if (this.MatchEnd(p_current[k].ToLower()))
                            v_achou = true;
                        else
                        {
                            v_expression.Add(p_current[k]);
                            k++;
                        }
                    }
                }
            }

            if (v_achou)
            {
                p_next = new System.Collections.Generic.List<string>();
                for (int i = k; i < p_current.Count; i++)
                    p_next.Add(p_current[i]);
            }
            else
                p_next = null;

            return v_expression;
        }

        private bool MatchEnd(string p_word)
        {
            bool v_achou = false;
            int k = 0;

            while (k < this.v_end.Length && !v_achou)
            {
                if (this.v_end[k] == p_word)
                    v_achou = true;
                else
                    k++;
            }

            return v_achou;
        }
    }
}

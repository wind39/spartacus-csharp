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
    public class Parser
    {
        public Spartacus.PollyDB.Connection v_connection;
        public Spartacus.PollyDB.Query v_query;

        private System.Collections.Generic.Dictionary<string, string[][]> v_syntax;

        public Parser(Spartacus.PollyDB.Connection p_connection)
        {
            this.v_connection = p_connection;

            this.v_syntax = new System.Collections.Generic.Dictionary<string, string[][]>();

            this.v_syntax.Add("select", new string[2][] {new string[]{"select"}, new string[]{"from"}});
            this.v_syntax.Add("from", new string[2][] {new string[]{"from"}, new string[]{"inner", "left", "right", "full", "where"}});
            this.v_syntax.Add("inner", new string[2][] {new string[]{"inner", "join"}, new string[]{"inner", "left", "right", "full", "where"}});
            this.v_syntax.Add("left", new string[2][] {new string[]{"left", "join"}, new string[]{"inner", "left", "right", "full", "where"}});
            this.v_syntax.Add("right", new string[2][] {new string[]{"right", "join"}, new string[]{"inner", "left", "right", "full", "where"}});
            this.v_syntax.Add("full", new string[2][] {new string[]{"full", "join"}, new string[]{"inner", "left", "right", "full", "where"}});
            this.v_syntax.Add("where", new string[2][] {new string[]{"where"}, new string[]{}});

            this.v_query = new Spartacus.PollyDB.Query();
        }

        public void Parse(string p_sql)
        {
            System.Collections.Generic.List<string> v_sql, v_current, v_next;
            Spartacus.PollyDB.Expression v_expression = null;

            v_sql = this.Split(p_sql.Replace(",", " , ").Replace("=", " = ").Replace("!=", " != ").Replace(">", " > ").Replace("<", " < ").Replace(">=", " >= ").Replace("<=", " <= "));

            v_current = v_sql;
            while (v_current != null && v_current.Count > 0)
            {
                try
                {
                    v_expression = new Spartacus.PollyDB.Expression(
                        this.v_syntax[v_current[0].ToLower()][0],
                        this.v_syntax[v_current[0].ToLower()][1]
                    );
                }
                catch (System.Exception)
                {
                    throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", v_current[0]);
                }

                v_current = v_expression.Parse(v_current, out v_next);

                switch (v_current[0].ToLower())
                {
                    case "select":
                        this.Select(v_current);
                        break;
                    case "from":
                        this.From(v_current);
                        break;
                    case "inner":
                        this.InnerJoin(v_current);
                        break;
                    case "left":
                        this.LeftJoin(v_current);
                        break;
                    case "right":
                        this.RightJoin(v_current);
                        break;
                    case "full":
                        this.FullJoin(v_current);
                        break;
                    case "where":
                        this.Where(v_current);
                        break;
                    default:
                        throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", v_current[0]);
                }

                Console.WriteLine(string.Join(" ", v_current));

                v_current = v_next;
            }
        }

        private System.Collections.Generic.List<string> Split(string p_text)
        {
            System.Collections.Generic.List<string> v_ret;
            string[] v_tmp;

            v_tmp = p_text.Split((char[]) null, System.StringSplitOptions.RemoveEmptyEntries);

            v_ret = new System.Collections.Generic.List<string>();

            foreach (string s in v_tmp)
                v_ret.Add(s);

            return v_ret;
        }

        private System.Collections.Generic.List<string> Split(string p_text, string p_split)
        {
            System.Collections.Generic.List<string> v_ret;
            string[] v_tmp;

            v_tmp = p_text.Split(new string[] {p_split}, System.StringSplitOptions.RemoveEmptyEntries);

            v_ret = new System.Collections.Generic.List<string>();

            foreach (string s in v_tmp)
                v_ret.Add(s);

            return v_ret;
        }

        private void Select(System.Collections.Generic.List<string> p_current)
        {
            System.Collections.Generic.List<string> v_select;
            System.Collections.Generic.List<string> v_tmp, v_tmp2;
            string v_text, v_token;
            bool v_error;
            int k;

            v_text = string.Join(" ", p_current).Replace("select", "");
            v_select = this.Split(v_text, ",");

            if (v_select.Count != v_text.Split(',').Length)
                throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", v_select[v_select.Count-1].Trim());

            k = 0;
            v_error = false;
            while (k < v_select.Count && !v_error)
            {
                v_token = v_select[k].Trim().ToLower();

                if (v_token.Contains(" as "))
                {
                    if (v_token.Contains("."))
                    {
                        v_tmp = this.Split(v_token, ".");
                        if (v_tmp.Count == 2 && v_tmp[0].Length > 0 && v_tmp[1].Length > 0)
                        {
                            v_tmp2 = this.Split(v_tmp[1], " as ");
                            if (v_tmp2.Count == 2 && v_tmp2[0].Length > 0 && v_tmp2[1].Length > 0)
                            {
                                this.v_query.AddProjectionColumn(v_tmp[0], v_tmp2[0], v_tmp2[1]);
                                k++;
                            }
                            else
                                v_error = true;
                        }
                        else
                            v_error = true;
                    }
                    else
                        v_error = true;
                }
                else
                {
                    if (v_token.Contains("."))
                    {
                        v_tmp = this.Split(v_token, ".");
                        if (v_tmp.Count == 2 && v_tmp[0].Length > 0 && v_tmp[1].Length > 0)
                        {
                            this.v_query.AddProjectionColumn(v_tmp[0], v_tmp[1]);
                            k++;
                        }
                        else
                            v_error = true;
                    }
                    else
                        v_error = true;
                }
            }

            if (v_error == true)
                throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", v_select[k].Trim());
        }

        private void From(System.Collections.Generic.List<string> p_current)
        {
            if (p_current.Count != 3)
                throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[0]);
            else
            {
                if (!this.v_connection.Exists(p_current[1].ToLower()))
                    throw new Spartacus.PollyDB.Exception("File '{0}' not found or format not supported.", p_current[1]);
                else
                    this.v_query.AddRelation(p_current[1].ToLower(), p_current[2].ToLower());
            }
        }

        private void InnerJoin(System.Collections.Generic.List<string> p_current)
        {
        }

        private void LeftJoin(System.Collections.Generic.List<string> p_current)
        {
        }

        private void RightJoin(System.Collections.Generic.List<string> p_current)
        {
        }

        private void FullJoin(System.Collections.Generic.List<string> p_current)
        {
        }

        private void Where(System.Collections.Generic.List<string> p_current)
        {
        }
    }
}

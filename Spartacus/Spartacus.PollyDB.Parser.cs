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
    public enum CommandType
    {
        SELECT,
        DELETE,
        UPDATE,
        INSERT
    }

    public class Parser
    {
        public Spartacus.PollyDB.Query v_query;

        private System.Collections.Generic.Dictionary<string, string[][]> v_syntax;
        private Spartacus.PollyDB.Connection v_connection;

        public Parser(Spartacus.PollyDB.Connection p_connection)
        {
            this.v_connection = p_connection;

            this.v_syntax = new System.Collections.Generic.Dictionary<string, string[][]>();

            this.v_syntax.Add("select", new string[2][] {new string[]{"select"}, new string[]{"from"}});
            this.v_syntax.Add("from", new string[2][] {new string[]{"from"}, new string[]{"inner", "left", "right", "full", "where"}});
            this.v_syntax.Add("inner", new string[2][] {new string[]{"inner", "join"}, new string[]{"inner", "left", "right", "full", "where"}});
            this.v_syntax.Add("full", new string[2][] {new string[]{"full", "join"}, new string[]{"inner", "left", "right", "full", "where"}});
            this.v_syntax.Add("where", new string[2][] {new string[]{"where"}, new string[]{}});

            this.v_query = new Spartacus.PollyDB.Query();
        }

        public Spartacus.PollyDB.CommandType Parse(string p_sql)
        {
            System.Collections.Generic.List<string> v_sql, v_current, v_next;
            Spartacus.PollyDB.Expression v_expression = null;

            v_sql = this.Split(p_sql.Replace("*", " * ").Replace(",", " , ").Replace("==", " == ").Replace("!=", " != "));

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
                    case "full":
                        this.FullJoin(v_current);
                        break;
                    case "where":
                        this.Where(v_current);
                        break;
                    default:
                        throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", v_current[0]);
                }

                v_current = v_next;
            }

            this.Validate();

            return Spartacus.PollyDB.CommandType.SELECT;
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
            System.Collections.Generic.List<string> v_tmp;
            string v_text, v_token;
            bool v_error, v_fullprojection;
            int k;

            v_text = string.Join(" ", p_current).Replace("select", "");
            v_select = this.Split(v_text, ",");

            if (v_select.Count != v_text.Split(',').Length)
                throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", v_select[v_select.Count-1].Trim());

            k = 0;
            v_error = false;
            v_fullprojection = false;
            while (k < v_select.Count && !v_error && !v_fullprojection)
            {
                v_token = v_select[k].Trim().ToLower();

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
                {
                    if (v_token == "*")
                        v_fullprojection = true;
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
                {
                    this.v_query.AddRelation(Spartacus.PollyDB.JoinType.FROM, p_current[1].ToLower(), p_current[2].ToLower());

                    foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Column> kvp in this.v_query.v_projection)
                    {
                        if (kvp.Value.v_relationalias == p_current[2].ToLower())
                            this.v_query.v_relations[p_current[2].ToLower()].AddColumn(kvp.Value.v_name);
                    }
                }
            }
        }

        private void InnerJoin(System.Collections.Generic.List<string> p_current)
        {
            if (p_current.Count < 5)
                throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[0]);
            else
            {
                if (!this.v_connection.Exists(p_current[2].ToLower()))
                    throw new Spartacus.PollyDB.Exception("File '{0}' not found or format not supported.", p_current[2]);
                else
                {
                    this.v_query.AddRelation(Spartacus.PollyDB.JoinType.INNER, p_current[2].ToLower(), p_current[3].ToLower());

                    foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Column> kvp in this.v_query.v_projection)
                    {
                        if (kvp.Value.v_relationalias == p_current[3].ToLower())
                            this.v_query.v_relations[p_current[3].ToLower()].AddColumn(kvp.Value.v_name);
                    }
                }

                this.Conditions(p_current, 5);
            }
        }

        private void FullJoin(System.Collections.Generic.List<string> p_current)
        {
            if (p_current.Count != 4)
                throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[0]);
            else
            {
                if (!this.v_connection.Exists(p_current[2].ToLower()))
                    throw new Spartacus.PollyDB.Exception("File '{0}' not found or format not supported.", p_current[2]);
                else
                {
                    this.v_query.AddRelation(Spartacus.PollyDB.JoinType.FULL, p_current[2].ToLower(), p_current[3].ToLower());

                    foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Column> kvp in this.v_query.v_projection)
                    {
                        if (kvp.Value.v_relationalias == p_current[3].ToLower())
                            this.v_query.v_relations[p_current[3].ToLower()].AddColumn(kvp.Value.v_name);
                    }
                }
            }
        }

        private void Where(System.Collections.Generic.List<string> p_current)
        {
            this.Conditions(p_current, 1);
        }

        private void Conditions(System.Collections.Generic.List<string> p_current, int p_index)
        {
            Spartacus.PollyDB.LogicOperator v_logic_operator = Spartacus.PollyDB.LogicOperator.AND;
            string v_leftoperand_relation = "";
            string v_leftoperand_column = "";
            Spartacus.PollyDB.Operator v_operator = Spartacus.PollyDB.Operator.EQ;
            Spartacus.PollyDB.OperandType v_rightoperand_type = Spartacus.PollyDB.OperandType.CONSTANT_STRING;
            string v_rightoperand_relation = "";
            string v_rightoperand_column = "";
            string v_rightoperand_constant = "";
            int v_type = 1;
            bool v_error = false, v_achou;
            string v_token, v_tokentmp;
            string[] v_tmp;
            double v_dtmp;
            int k = p_index;

            while (k < p_current.Count && !v_error)
            {
                v_token = p_current[k];

                switch (v_type)
                {
                    case 0:
                        if (k == p_current.Count-1)
                            throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                        switch (v_token.ToLower())
                        {
                            case "and":
                                v_logic_operator = Spartacus.PollyDB.LogicOperator.AND;
                                break;
                            case "or":
                                v_logic_operator = Spartacus.PollyDB.LogicOperator.OR;
                                break;
                            default:
                                throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                        }
                        v_type++;
                        break;
                    case 1:
                        if (k == p_current.Count - 1)
                            throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                        v_token = v_token.ToLower();
                        if (v_token.Contains("."))
                        {
                            v_tmp = v_token.Split('.');
                            if (v_tmp.Length > 0 && v_tmp[0].Length > 0 && v_tmp[1].Length > 0)
                            {
                                try
                                {
                                    this.v_query.v_relations[v_tmp[0]].AddColumn(v_tmp[1]);
                                    v_leftoperand_relation = v_tmp[0];
                                    v_leftoperand_column = v_tmp[1];
                                }
                                catch (System.Exception)
                                {
                                    throw new Spartacus.PollyDB.Exception("Unknown relation '{0}'.", v_tmp[0]);
                                }
                            }
                            else
                                throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                        }
                        else
                            throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                        v_type++;
                        break;
                    case 2:
                        if (k == p_current.Count-1)
                            throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                        switch (v_token)
                        {
                            case "==":
                                v_operator = Spartacus.PollyDB.Operator.EQ;
                                break;
                            case "!=":
                                v_operator = Spartacus.PollyDB.Operator.NE;
                                break;
                            default:
                                throw new Spartacus.PollyDB.Exception("Unknown operator '{0}'.", p_current[k]);
                        }
                        v_type++;
                        break;
                    case 3:
                        if (!v_token.StartsWith("'") && !v_token.EndsWith("'") && double.TryParse(v_token, out v_dtmp))
                        {
                            v_rightoperand_type = Spartacus.PollyDB.OperandType.CONSTANT_NUMBER;
                            v_rightoperand_constant = v_token;
                        }
                        else
                        {
                            if (!v_token.StartsWith("'") && !v_token.EndsWith("'"))
                            {
                                if (v_token.Contains("."))
                                {
                                    v_tmp = v_token.Split('.');
                                    if (v_tmp.Length > 0 && v_tmp[0].Length > 0 && v_tmp[1].Length > 0)
                                    {
                                        try
                                        {
                                            this.v_query.v_relations[v_tmp[0]].AddColumn(v_tmp[1]);
                                            v_rightoperand_type = Spartacus.PollyDB.OperandType.COLUMN;
                                            v_rightoperand_relation = v_tmp[0];
                                            v_rightoperand_column = v_tmp[1];
                                        }
                                        catch (System.Exception)
                                        {
                                            throw new Spartacus.PollyDB.Exception("Unknown relation '{0}'.", v_tmp[0]);
                                        }
                                    }
                                    else
                                        throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                                }
                                else
                                    throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                            }
                            else
                            {
                                if (v_token.StartsWith("'") && v_token.EndsWith("'"))
                                {
                                    v_rightoperand_type = Spartacus.PollyDB.OperandType.CONSTANT_STRING;
                                    v_rightoperand_constant = v_token;
                                }
                                else
                                {
                                    if (v_token.StartsWith("'") && !v_token.EndsWith("'"))
                                    {
                                        v_tokentmp = v_token;
                                        v_achou = false;
                                        k++;
                                        while (k < p_current.Count && !v_achou)
                                        {
                                            v_tokentmp += " " + p_current[k];
                                            if (p_current[k].EndsWith("'"))
                                                v_achou = true;
                                            else
                                                k++;
                                        }
                                        if (v_achou)
                                        {
                                            v_rightoperand_type = Spartacus.PollyDB.OperandType.CONSTANT_STRING;
                                            v_rightoperand_constant = v_tokentmp;
                                        }
                                        else
                                            throw new Spartacus.PollyDB.Exception("Syntax error: Unexpected end of string literal.");
                                    }
                                    else
                                        throw new Spartacus.PollyDB.Exception("Syntax error near '{0}'.", p_current[k]);
                                }
                                    
                            }
                        }

                        if (v_rightoperand_type == Spartacus.PollyDB.OperandType.COLUMN)
                            this.v_query.v_conditions.Add(new Spartacus.PollyDB.Condition(
                                v_logic_operator,
                                v_leftoperand_relation,
                                v_leftoperand_column,
                                v_operator,
                                v_rightoperand_type,
                                v_rightoperand_relation,
                                v_rightoperand_column
                            ));
                        else
                            this.v_query.v_conditions.Add(new Spartacus.PollyDB.Condition(
                                v_logic_operator,
                                v_leftoperand_relation,
                                v_leftoperand_column,
                                v_operator,
                                v_rightoperand_type,
                                v_rightoperand_constant
                            ));

                        v_type = 0;
                        break;
                    default:
                        break;
                }
                k++;
            }
        }

        private void Validate()
        {
            Spartacus.PollyDB.Relation r;

            // verificando se todos os alias de tabelas existem na lista de tabelas
            foreach (System.Collections.Generic.KeyValuePair<string, Spartacus.PollyDB.Column> kvp in this.v_query.v_projection)
            {
                try
                {
                    r = this.v_query.v_relations[kvp.Value.v_relationalias];
                }
                catch (System.Exception)
                {
                    throw new Spartacus.PollyDB.Exception("Unknown relation '{0}'.", kvp.Value.v_relationalias);
                }
            }

            // criando expressao de condicoes
            this.v_query.BuildConditions();
        }
    }
}

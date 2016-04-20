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

        public System.Collections.Generic.List<Spartacus.PollyDB.Condition> v_conditions;
        public NCalc.Expression v_conditions_expression;

        public Query()
        {
            this.v_projection = new System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Column>();
            this.v_relations = new System.Collections.Generic.Dictionary<string, Spartacus.PollyDB.Relation>();
            this.v_conditions = new System.Collections.Generic.List<Spartacus.PollyDB.Condition>();
            this.v_conditions_expression = null;
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

        public void AddRelation(Spartacus.PollyDB.JoinType p_type, string p_name, string p_alias)
        {
            Spartacus.PollyDB.Relation r;

            try
            {
                r = new Spartacus.PollyDB.Relation(p_type, p_name, p_alias);
                this.v_relations.Add(p_alias, r);
            }
            catch (System.Exception)
            {
                throw new Spartacus.PollyDB.Exception("Ambiguous definition for relation {0}.", p_alias);
            }
        }

        public void BuildConditions()
        {
            string s;

            s = "1 = 1 ";
            foreach (Spartacus.PollyDB.Condition c in this.v_conditions)
            {
                switch (c.v_logic_operator)
                {
                    case Spartacus.PollyDB.LogicOperator.AND:
                        s += "and ";
                        break;
                    case Spartacus.PollyDB.LogicOperator.OR:
                        s += "or ";
                        break;
                    default:
                        break;
                }

                s += "[" + c.v_leftoperand_relation + "." + c.v_leftoperand_column + "]";

                switch (c.v_operator)
                {
                    case Spartacus.PollyDB.Operator.EQ:
                        s += " = ";
                        break;
                    case Spartacus.PollyDB.Operator.NE:
                        s += " != ";
                        break;
                    default:
                        break;
                }

                if (c.v_rightoperand_type == Spartacus.PollyDB.OperandType.COLUMN)
                    s += "[" + c.v_rightoperand_relation + "." + c.v_rightoperand_column + "]";
                else
                    s += c.v_rightoperand_constant;
            }

            this.v_conditions_expression = new NCalc.Expression(s);
        }
    }
}

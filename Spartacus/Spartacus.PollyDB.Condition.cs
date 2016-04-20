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
    public enum LogicOperator
    {
        AND,
        OR
    }

    public enum Operator
    {
        EQ,
        NE
    }

    public enum OperandType
    {
        CONSTANT_NUMBER,
        CONSTANT_STRING,
        COLUMN
    }

    public class Condition
    {
        public Spartacus.PollyDB.LogicOperator v_logic_operator;

        public string v_leftoperand_relation;
        public string v_leftoperand_column;

        public Spartacus.PollyDB.Operator v_operator;

        public Spartacus.PollyDB.OperandType v_rightoperand_type;
        public string v_rightoperand_relation;
        public string v_rightoperand_column;
        public string v_rightoperand_constant;

        public Condition(
            Spartacus.PollyDB.LogicOperator p_logic_operator,
            string p_leftoperand_relation,
            string p_leftoperand_column,
            Spartacus.PollyDB.Operator p_operator,
            Spartacus.PollyDB.OperandType p_rightoperand_type,
            string p_rightoperand_constant
        )
        {
            this.v_logic_operator = p_logic_operator;
            this.v_leftoperand_relation = p_leftoperand_relation;
            this.v_leftoperand_column = p_leftoperand_column;
            this.v_operator = p_operator;
            this.v_rightoperand_type = p_rightoperand_type;
            this.v_rightoperand_relation = null;
            this.v_rightoperand_column = null;
            this.v_rightoperand_constant = p_rightoperand_constant;
        }

        public Condition(
            Spartacus.PollyDB.LogicOperator p_logic_operator,
            string p_leftoperand_relation,
            string p_leftoperand_column,
            Spartacus.PollyDB.Operator p_operator,
            Spartacus.PollyDB.OperandType p_rightoperand_type,
            string p_rightoperand_relation,
            string p_rightoperand_column
        )
        {
            this.v_logic_operator = p_logic_operator;
            this.v_leftoperand_relation = p_leftoperand_relation;
            this.v_leftoperand_column = p_leftoperand_column;
            this.v_operator = p_operator;
            this.v_rightoperand_type = p_rightoperand_type;
            this.v_rightoperand_relation = p_rightoperand_relation;
            this.v_rightoperand_column = p_rightoperand_column;
            this.v_rightoperand_constant = null;
        }
    }
}

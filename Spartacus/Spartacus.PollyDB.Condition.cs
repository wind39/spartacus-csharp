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
    /// <summary>
    /// Operador Lógico.
    /// </summary>
    public enum LogicOperator
    {
        AND,
        OR
    }

    /// <summary>
    /// Operador de Comparação.
    /// </summary>
    public enum Operator
    {
        EQ,
        NE
    }

    /// <summary>
    /// Tipo do Operando.
    /// </summary>
    public enum OperandType
    {
        CONSTANT_NUMBER,
        CONSTANT_STRING,
        COLUMN
    }

    /// <summary>
    /// Classe Condition.
    /// Representa uma Condição de join ou where.
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// Operador Lógico da Condição.
        /// </summary>
        public Spartacus.PollyDB.LogicOperator v_logic_operator;

        /// <summary>
        /// Relação do operando da esquerda.
        /// </summary>
        public string v_leftoperand_relation;

        /// <summary>
        /// Coluna do operando da esquerda.
        /// </summary>
        public string v_leftoperand_column;

        /// <summary>
        /// Operador de Comparação.
        /// </summary>
        public Spartacus.PollyDB.Operator v_operator;

        /// <summary>
        /// Tipo do operando da direita.
        /// </summary>
        public Spartacus.PollyDB.OperandType v_rightoperand_type;

        /// <summary>
        /// Relação do operando da direita.
        /// </summary>
        public string v_rightoperand_relation;

        /// <summary>
        /// Coluna do operando da direita.
        /// </summary>
        public string v_rightoperand_column;

        /// <summary>
        /// Constante usada como operando da direita.
        /// </summary>
        public string v_rightoperand_constant;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.PollyDB.Condition"/>.
        /// </summary>
        /// <param name="p_logic_operator">Operador Lógico.</param>
        /// <param name="p_leftoperand_relation">Relação do operando da esquerda.</param>
        /// <param name="p_leftoperand_column">Coluna do operando da esquerda.</param>
        /// <param name="p_operator">Operador de Comparação.</param>
        /// <param name="p_rightoperand_type">Tipo do operando da direita.</param>
        /// <param name="p_rightoperand_constant">Constante usada como operando da direita.</param>
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

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.PollyDB.Condition"/>.
        /// </summary>
        /// <param name="p_logic_operator">Operador Lógico.</param>
        /// <param name="p_leftoperand_relation">Relação do operando da esquerda.</param>
        /// <param name="p_leftoperand_column">Coluna do operando da esquerda.</param>
        /// <param name="p_operator">Operador de Comparação.</param>
        /// <param name="p_rightoperand_type">Tipo do operando da direita.</param>
        /// <param name="p_rightoperand_relation">Relação do operando da direita.</param>
        /// <param name="p_rightoperand_column">Coluna do operando da direita.</param>
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

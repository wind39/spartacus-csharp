/*
The MIT License (MIT)

Copyright (c) 2014,2015 William Ivanski

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

namespace Spartacus.Reporting
{
    /// <summary>
    /// Alinhamento do Campo.
    /// </summary>
    public enum FieldAlignment
    {
        LEFT,
        RIGHT,
        CENTER
    }

    /// <summary>
    /// Classe Field.
    /// Representa um campo de dados do relatório.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Título do campo (aparece no cabeçalho de dados).
        /// </summary>
        public string v_title;

        /// <summary>
        /// Coluna da tabela do relatório associada ao campo.
        /// </summary>
        public string v_column;

        /// <summary>
        /// Alinhamento do campo.
        /// </summary>
        public Spartacus.Reporting.FieldAlignment v_align;

        /// <summary>
        /// Percentual indicando quanto da largura da página o campo ocupa.
        /// </summary>
        public int v_fill;

        /// <summary>
        /// Tipo de dados do campo.
        /// </summary>
        public Spartacus.Database.Type v_type;

        /// <summary>
        /// Se o campo é um valor totalizado dentro de um grupo.
        /// </summary>
        public bool v_groupedvalue;

        /// <summary>
        /// Em qual linha dentro do detalhe o campo aparece.
        /// </summary>
        public int v_row;

        /// <summary>
        /// Formato de impressão do campo numérico.
        /// </summary>
        public string v_format;

        /// <summary>
        /// Borda do campo.
        /// </summary>
        public Spartacus.Reporting.Border v_border;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Field"/>.
        /// </summary>
        public Field()
        {
            this.v_groupedvalue = false;
            this.v_row = 0;
            this.v_format = "###,###,###,###,##0.00";
            this.v_border = null;
        }

        /// <summary>
        /// Configura o tipo de dados do campo.
        /// </summary>
        /// <param name="p_text">Texto representando o tipo de dados.</param>
        public void SetType(string p_text)
        {
            switch (p_text)
            {
                case "INTEGER":
                    this.v_type = Spartacus.Database.Type.INTEGER;
                    break;
                case "REAL":
                    this.v_type = Spartacus.Database.Type.REAL;
                    break;
                case "BOOLEAN":
                    this.v_type = Spartacus.Database.Type.BOOLEAN;
                    break;
                case "CHAR":
                    this.v_type = Spartacus.Database.Type.CHAR;
                    break;
                case "DATE":
                    this.v_type = Spartacus.Database.Type.DATE;
                    break;
                case "STRING":
                    this.v_type = Spartacus.Database.Type.STRING;
                    break;
                default:
                    this.v_type = Spartacus.Database.Type.STRING;
                    break;
            }
        }

        /// <summary>
        /// Formata o valor do campo.
        /// </summary>
        /// <param name="p_text">Texto representando o valor do campo.</param>
        public string Format(string p_text)
        {
            string v_ret;
            double v_tmpdouble;
            int v_tmpint;

            switch (this.v_type)
            {
                case Spartacus.Database.Type.INTEGER:
                    if (int.TryParse(p_text, out v_tmpint))
                        v_ret = p_text;
                    else
                        v_ret = "0";
                    break;
                case Spartacus.Database.Type.REAL:
                    if (double.TryParse(p_text.Replace('.', ','), out v_tmpdouble))
                        v_ret = string.Format("{0:" + this.v_format + "}", v_tmpdouble);
                    else
                        v_ret = string.Format("{0:" + this.v_format + "}", (double) 0.0);
                    break;
                case Spartacus.Database.Type.DATE:
                    if (p_text.Length >= 8 && int.TryParse(p_text, out v_tmpint))
                        v_ret = string.Format("{0}/{1}/{2}", p_text.Substring(6, 2), p_text.Substring(4, 2), p_text.Substring(0, 4));
                    else
                        v_ret = "01/01/1900";
                    break;
                default:
                    v_ret = p_text;
                    break;
            }

            return v_ret;
        }
    }
}

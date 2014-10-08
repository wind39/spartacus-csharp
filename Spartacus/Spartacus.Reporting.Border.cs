/*
The MIT License (MIT)

Copyright (c) 2014 William Ivanski

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
    /// Classe Border.
    /// Armazena informações sobre quais bordas devem ser ativadas.
    /// </summary>
    public class Border
    {
        /// <summary>
        /// Borda superior.
        /// </summary>
        public bool v_top;

        /// <summary>
        /// Borda inferior.
        /// </summary>
        public bool v_bottom;

        /// <summary>
        /// Borda esquerda.
        /// </summary>
        public bool v_left;

        /// <summary>
        /// Borda direita.
        /// </summary>
        public bool v_right;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Border"/>.
        /// </summary>
        public Border()
        {
            this.v_top = false;
            this.v_bottom = false;
            this.v_left = false;
            this.v_right = false;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Border"/>.
        /// </summary>
        /// <param name="p_text">Texto representando quais bordas devem ser ativadas.</param>
        public Border(string p_text)
        {
            char[] v_ch;

            v_ch = new char[1];
            v_ch[0] = ',';

            foreach (string s in p_text.Split(v_ch))
            {
                switch (s)
                {
                    case "TOP":
                        this.v_top = true;
                        break;
                    case "BOTTOM":
                        this.v_bottom = true;
                        break;
                    case "LEFT":
                        this.v_left = true;
                        break;
                    case "RIGHT":
                        this.v_right = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

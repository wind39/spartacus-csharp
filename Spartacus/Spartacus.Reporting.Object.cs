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
    /// Tipo do Objeto.
    /// </summary>
    public enum ObjectType
    {
        IMAGE,
        TEXT,
        PAGENUMBER
    }

    /// <summary>
    /// Classe Object.
    /// Representa um objeto que pode ser renderizado dentro de um <see cref="Spartacus.Reporting.Block"/>.
    /// </summary>
    public class Object
    {
        /// <summary>
        /// Tipo do objeto.
        /// </summary>
        public Spartacus.Reporting.ObjectType v_type;

        /// <summary>
        /// Coluna associada ao Objeto.
        /// </summary>
        public string v_column;

        /// <summary>
        /// Valor do Objeto.
        /// </summary>
        public string v_value;

        /// <summary>
        /// Posição X onde o Objeto será renderizado.
        /// </summary>
        public double v_posx;

        /// <summary>
        /// Posição Y onde o Objeto será renderizado.
        /// </summary>
        public double v_posy;

        /// <summary>
        /// Alinhamento do Objeto dentro do Bloco.
        /// </summary>
        public Spartacus.Reporting.FieldAlignment v_align;

        /// <summary>
        /// Objeto PDF nativo já configurado dentro do Bloco.
        /// </summary>
        public object v_pdfobject;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Object"/>.
        /// </summary>
        public Object()
        {
            this.v_type = Spartacus.Reporting.ObjectType.TEXT;
            this.v_column = "";
            this.v_value = "";
            this.v_posx = 0.0;
            this.v_posy = 0.0;
            this.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
            this.v_pdfobject = null;
        }

        /// <summary>
        /// Configura a posição X do Objeto.
        /// </summary>
        /// <param name="p_text">Texto representando a posição.</param>
        public void SetPosX(string p_text)
        {
            double v_temp;

            if (System.Double.TryParse(p_text, out v_temp))
                this.v_posx = v_temp;
        }

        /// <summary>
        /// Configura a posição Y do Objeto.
        /// </summary>
        /// <param name="p_text">Texto representando a posição.</param>
        public void SetPosY(string p_text)
        {
            double v_temp;

            if (System.Double.TryParse(p_text, out v_temp))
                this.v_posy = v_temp;
        }
    }
}

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
using PDFjet;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Família de fonte.
    /// </summary>
    public enum FontFamily
    {
        COURIER,
        HELVETICA,
        TIMES
    }

    /// <summary>
    /// Classe Fonte.
    /// </summary>
    public class Font
    {
        /// <summary>
        /// Família.
        /// </summary>
        public Spartacus.Reporting.FontFamily v_family;

        /// <summary>
        /// Negrito.
        /// </summary>
        public bool v_bold;

        /// <summary>
        /// Itálico.
        /// </summary>
        public bool v_italic;

        /// <summary>
        /// Tamanho.
        /// </summary>
        public double v_size;

        /// <summary>
        /// Fonte nativa.
        /// </summary>
        //public System.Drawing.Font v_nativefont;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Font"/>.
        /// </summary>
        public Font()
        {
            this.v_family = Spartacus.Reporting.FontFamily.HELVETICA;
            this.v_size = 8.0;
            this.v_bold = false;
            this.v_italic = false;
        }

        /// <summary>
        /// Configura família da fonte.
        /// </summary>
        /// <param name="p_text">Texto representando a família.</param>
        public void SetFamily(string p_text)
        {
            switch (p_text)
            {
                case "COURIER":
                    this.v_family = Spartacus.Reporting.FontFamily.COURIER;
                    break;
                case "HELVETICA":
                    this.v_family = Spartacus.Reporting.FontFamily.HELVETICA;
                    break;
                case "TIMES":
                    this.v_family = Spartacus.Reporting.FontFamily.TIMES;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Configura o tamanho da fonte.
        /// </summary>
        /// <param name="p_text">Texto representando o tamanho da fonte.</param>
        public void SetSize(string p_text)
        {
            double v_temp;

            if (System.Double.TryParse(p_text, out v_temp))
                this.v_size = v_temp;
        }

        /// <summary>
        /// Converte a fonte em uma opção específica da PDFjet.
        /// </summary>
        /// <returns>Fonte conforme PDFjet.</returns>
        /// <param name="p_pdf">Objeto PDF.</param>
        public PDFjet.NET.Font GetFont(PDFjet.NET.PDF p_pdf)
        {
            PDFjet.NET.Font v_font = null;

            switch (this.v_family)
            {
                case Spartacus.Reporting.FontFamily.COURIER:
                    if (this.v_bold && this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.COURIER_BOLD_OBLIQUE);
                        //this.v_nativefont = new System.Drawing.Font("Courier New", (float) this.v_size, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                    }
                    if (this.v_bold && !this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.COURIER_BOLD);
                        //this.v_nativefont = new System.Drawing.Font("Courier New", (float) this.v_size, System.Drawing.FontStyle.Bold);
                    }
                    if (!this.v_bold && this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.COURIER_OBLIQUE);
                        //this.v_nativefont = new System.Drawing.Font("Courier New", (float) this.v_size, System.Drawing.FontStyle.Italic);
                    }
                    if (!this.v_bold && !this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.COURIER);
                        //this.v_nativefont = new System.Drawing.Font("Courier New", (float) this.v_size, System.Drawing.FontStyle.Regular);
                    }
                    break;
                case Spartacus.Reporting.FontFamily.HELVETICA:
                    if (this.v_bold && this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.HELVETICA_BOLD_OBLIQUE);
                        //this.v_nativefont = new System.Drawing.Font("Helvetica", (float) this.v_size, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                    }
                    if (this.v_bold && !this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.HELVETICA_BOLD);
                        //this.v_nativefont = new System.Drawing.Font("Helvetica", (float) this.v_size, System.Drawing.FontStyle.Bold);
                    }
                    if (!this.v_bold && this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.HELVETICA_OBLIQUE);
                        //this.v_nativefont = new System.Drawing.Font("Helvetica", (float) this.v_size, System.Drawing.FontStyle.Italic);
                    }
                    if (!this.v_bold && !this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.HELVETICA);
                        //this.v_nativefont = new System.Drawing.Font("Helvetica", (float) this.v_size, System.Drawing.FontStyle.Regular);
                    }
                    break;
                case Spartacus.Reporting.FontFamily.TIMES:
                    if (this.v_bold && this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.TIMES_BOLD_ITALIC);
                        //this.v_nativefont = new System.Drawing.Font("Times New Roman", (float) this.v_size, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                    }
                    if (this.v_bold && !this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.TIMES_BOLD);
                        //this.v_nativefont = new System.Drawing.Font("Times New Roman", (float) this.v_size, System.Drawing.FontStyle.Bold);
                    }
                    if (!this.v_bold && this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.TIMES_ITALIC);
                        //this.v_nativefont = new System.Drawing.Font("Times New Roman", (float) this.v_size, System.Drawing.FontStyle.Italic);
                    }
                    if (!this.v_bold && !this.v_italic)
                    {
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.TIMES_ROMAN);
                        //this.v_nativefont = new System.Drawing.Font("Times New Roman", (float) this.v_size, System.Drawing.FontStyle.Regular);
                    }
                    break;
                default:
                    break;
            }

            v_font.SetSize(this.v_size);

            return v_font;
        }
    }
}

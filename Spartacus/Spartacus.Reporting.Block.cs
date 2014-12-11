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
using PDFjet;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Classe Block.
    /// Representa um bloco que pode conter qualquer informação.
    /// O cabeçalho do relatório e o rodapé do relatório são blocos.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Altura do Bloco.
        /// </summary>
        public double v_height;

        /// <summary>
        /// Lista de objetos contidos no bloco.
        /// </summary>
        public System.Collections.ArrayList v_objects;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Block"/>.
        /// </summary>
        public Block()
        {
            this.v_height = 0.0;
            this.v_objects = new System.Collections.ArrayList();
        }

        /// <summary>
        /// Configura a altura do bloco.
        /// </summary>
        /// <param name="p_text">Texto representando a altura.</param>
        public void SetHeight(string p_text)
        {
            double v_temp;

            if (System.Double.TryParse(p_text, out v_temp))
                this.v_height = v_temp;
        }

        /// <summary>
        /// Configura os valores dos objetos do bloco.
        /// Esses valores vem de um DataTable.
        /// </summary>
        /// <param name="p_table">Tabela do relatório.</param>
        public void SetValues(System.Data.DataTable p_table)
        {
            int k;

            for (k = 0; k < this.v_objects.Count; k++)
            {
                if (((Spartacus.Reporting.Object)this.v_objects[k]).v_type != Spartacus.Reporting.ObjectType.PAGENUMBER)
                    ((Spartacus.Reporting.Object)this.v_objects[k]).v_value = p_table.Rows[0][((Spartacus.Reporting.Object)this.v_objects[k]).v_column].ToString();
            }
        }

        /// <summary>
        /// Configura número da página.
        /// </summary>
        /// <param name="p_current">Página atual.</param>
        /// <param name="p_total">Número total de páginas.</param>
        public void SetPageNumber(int p_current, int p_total)
        {
            int k;

            for (k = 0; k < this.v_objects.Count; k++)
            {
                if (((Spartacus.Reporting.Object)this.v_objects [k]).v_type == Spartacus.Reporting.ObjectType.PAGENUMBER)
                    ((Spartacus.Reporting.Object)this.v_objects [k]).v_value = string.Format("Pagina: {0} de {1}", p_current, p_total);
            }
        }

        /// <summary>
        /// Renderiza o Bloco em uma Página.
        /// </summary>
        /// <param name="p_font">Fonte.</param>
        /// <param name="p_posx">Posição X.</param>
        /// <param name="p_posy">Posição Y.</param>
        /// <param name="p_rightmargin">Margem direita.</param>
        /// <param name="p_pdf">Objeto PDF.</param>
        /// <param name="p_page">Página onde o Bloco será renderizado.</param>
        public void Render(Spartacus.Reporting.Font p_font, double p_posx, double p_posy, double p_rightmargin, PDFjet.NET.PDF p_pdf, PDFjet.NET.Page p_page)
        {
            int k;

            for (k = 0; k < this.v_objects.Count; k++)
            {
                if (((Spartacus.Reporting.Object)this.v_objects[k]).v_type == Spartacus.Reporting.ObjectType.IMAGE)
                    this.RenderImage((Spartacus.Reporting.Object)this.v_objects[k], p_posx, p_posy, p_rightmargin, p_pdf, p_page);
                else
                    this.RenderText((Spartacus.Reporting.Object)this.v_objects[k], p_posx, p_posy, p_rightmargin, p_font, p_pdf, p_page);
            }
        }

        /// <summary>
        /// Renderiza uma imagem no Bloco.
        /// Essa imagem precisa vir de um arquivo em disco.
        /// </summary>
        /// <param name="p_object">Objeto a ser renderizado.</param>
        /// <param name="p_posx">Posição X.</param>
        /// <param name="p_posy">Posição Y.</param>
        /// <param name="p_rightmargin">Margem direita.</param>
        /// <param name="p_pdf">Objeto PDF.</param>
        /// <param name="p_page">Página onde o objeto será renderizado.</param>
        private void RenderImage(Spartacus.Reporting.Object p_object, double p_posx, double p_posy, double p_rightmargin, PDFjet.NET.PDF p_pdf, PDFjet.NET.Page p_page)
        {
            PDFjet.NET.Image v_image;
            char[] v_ch;
            string[] v_temp;
            Spartacus.Net.Cryptor v_cryptor;
            string v_path;

            v_ch = new char[1];
            v_ch[0] = '.';

            v_cryptor = new Spartacus.Net.Cryptor("spartacus");
            try
            {
                v_path = v_cryptor.Decrypt(p_object.v_value);
            }
            catch (System.Exception)
            {
                v_path = p_object.v_value;
            }

            v_temp = v_path.Split(v_ch);
            switch (v_temp[v_temp.Length - 1].ToUpper())
            {
                case "BMP":
                    v_image = new PDFjet.NET.Image(p_pdf, new System.IO.FileStream(v_path, System.IO.FileMode.Open, System.IO.FileAccess.Read), PDFjet.NET.ImageType.BMP);
                    break;
                case "JPG":
                    v_image = new PDFjet.NET.Image(p_pdf, new System.IO.FileStream(v_path, System.IO.FileMode.Open, System.IO.FileAccess.Read), PDFjet.NET.ImageType.JPG);
                    break;
                case "JPEG":
                    v_image = new PDFjet.NET.Image(p_pdf, new System.IO.FileStream(v_path, System.IO.FileMode.Open, System.IO.FileAccess.Read), PDFjet.NET.ImageType.JPG);
                    break;
                case "PDF":
                    v_image = new PDFjet.NET.Image(p_pdf, new System.IO.FileStream(v_path, System.IO.FileMode.Open, System.IO.FileAccess.Read), PDFjet.NET.ImageType.PDF);
                    break;
                case "PNG":
                    v_image = new PDFjet.NET.Image(p_pdf, new System.IO.FileStream(v_path, System.IO.FileMode.Open, System.IO.FileAccess.Read), PDFjet.NET.ImageType.PNG);
                    break;
                default:
                    v_image = null;
                    break;
            }

            if (v_image != null)
            {
                if (p_object.v_align == Spartacus.Reporting.FieldAlignment.LEFT)
                    v_image.SetPosition(p_posx + p_object.v_posx, p_posy + p_object.v_posy);
                else
                    v_image.SetPosition(p_page.GetWidth() - p_rightmargin - v_image.GetWidth(), p_posy + p_object.v_posy);
                v_image.DrawOn(p_page);
            }
        }

        /// <summary>
        /// Renderiza um rótulo de texto no Bloco.
        /// </summary>
        /// <param name="p_object">Objeto a ser renderizado.</param>
        /// <param name="p_posx">Posição X.</param>
        /// <param name="p_posy">Posição Y.</param>
        /// <param name="p_rightmargin">Margem direita.</param>
        /// <param name="p_font">Fonte.</param>
        /// <param name="p_pdf">Objeto PDF.</param>
        /// <param name="p_page">Página onde será renderizado.</param>
        private void RenderText(Spartacus.Reporting.Object p_object, double p_posx, double p_posy, double p_rightmargin, Spartacus.Reporting.Font p_font, PDFjet.NET.PDF p_pdf, PDFjet.NET.Page p_page)
        {
            PDFjet.NET.TextLine v_text;

            v_text = new PDFjet.NET.TextLine(p_font.GetFont(p_pdf));

            v_text.SetText(p_object.v_value);
            if (p_object.v_align == Spartacus.Reporting.FieldAlignment.LEFT)
                v_text.SetPosition(p_posx + p_object.v_posx, p_posy + p_object.v_posy);
            else
                v_text.SetPosition(p_page.GetWidth() - p_rightmargin - v_text.GetWidth(), p_posy + p_object.v_posy);

            v_text.DrawOn(p_page);
        }
    }
}

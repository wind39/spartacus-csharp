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
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.COURIER_BOLD_OBLIQUE);
                    if (this.v_bold && ! this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.COURIER_BOLD);
                    if (! this.v_bold && this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.COURIER_OBLIQUE);
                    if (! this.v_bold && ! this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.COURIER);
                    break;
                case Spartacus.Reporting.FontFamily.HELVETICA:
                    if (this.v_bold && this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.HELVETICA_BOLD_OBLIQUE);
                    if (this.v_bold && ! this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.HELVETICA_BOLD);
                    if (! this.v_bold && this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.HELVETICA_OBLIQUE);
                    if (! this.v_bold && ! this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.HELVETICA);
                    break;
                case Spartacus.Reporting.FontFamily.TIMES:
                    if (this.v_bold && this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.TIMES_BOLD_ITALIC);
                    if (this.v_bold && ! this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.TIMES_BOLD);
                    if (! this.v_bold && this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.TIMES_ITALIC);
                    if (! this.v_bold && ! this.v_italic)
                        v_font = new PDFjet.NET.Font(p_pdf, PDFjet.NET.CoreFont.TIMES_ROMAN);
                    break;
                default:
                    break;
            }

            v_font.SetSize(this.v_size);

            return v_font;
        }
    }
}

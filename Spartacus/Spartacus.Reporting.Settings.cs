using System;
using PDFjet;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Layout da Página.
    /// </summary>
    public enum PageLayout
    {
        PORTRAIT,
        LANDSCAPE
    }

    /// <summary>
    /// Tipo de margem.
    /// </summary>
    public enum PageMargin
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }

    /// <summary>
    /// Classe Settings.
    /// Armazena informações gerais sobre o relatório.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Layout da Página.
        /// </summary>
        public Spartacus.Reporting.PageLayout v_layout;

        /// <summary>
        /// Margem superior.
        /// </summary>
        public double v_topmargin;

        /// <summary>
        /// Margem inferior.
        /// </summary>
        public double v_bottommargin;

        /// <summary>
        /// Margem esquerda.
        /// </summary>
        public double v_leftmargin;

        /// <summary>
        /// Margem direita.
        /// </summary>
        public double v_rightmargin;

        /// <summary>
        /// Borda do cabeçalho de dados.
        /// </summary>
        public Spartacus.Reporting.Border v_dataheaderborder;

        /// <summary>
        /// Borda do campo de dados.
        /// </summary>
        public Spartacus.Reporting.Border v_datafieldborder;

        /// <summary>
        /// Borda do cabeçalho de grupo.
        /// </summary>
        public Spartacus.Reporting.Border v_groupheaderborder;

        /// <summary>
        /// Borda do rodapé de grupo.
        /// </summary>
        public Spartacus.Reporting.Border v_groupfooterborder;

        /// <summary>
        /// Fonte do cabeçalho do relatório.
        /// </summary>
        public Spartacus.Reporting.Font v_reportheaderfont;

        /// <summary>
        /// Fonte do rodapé do relatório.
        /// </summary>
        public Spartacus.Reporting.Font v_reportfooterfont;

        /// <summary>
        /// Fonte do cabeçalho de dados.
        /// </summary>
        public Spartacus.Reporting.Font v_dataheaderfont;

        /// <summary>
        /// Fonte do campo de dados.
        /// </summary>
        public Spartacus.Reporting.Font v_datafieldfont;

        /// <summary>
        /// Fonte do cabeçalho de grupo.
        /// </summary>
        public Spartacus.Reporting.Font v_groupheaderfont;

        /// <summary>
        /// Fonte do rodapé de grupo.
        /// </summary>
        public Spartacus.Reporting.Font v_groupfooterfont;

        /// <summary>
        /// Cor de fundo do cabeçalho de dados.
        /// </summary>
        public int v_dataheadercolor;

        /// <summary>
        /// Cor de fundo do campo de dados em linhas pares.
        /// </summary>
        public int v_datafieldevencolor;

        /// <summary>
        /// Cor de fundo do campo de dados em linhas ímpares.
        /// </summary>
        public int v_datafieldoddcolor;

        /// <summary>
        /// Cor de fundo do cabeçalho de grupo.
        /// </summary>
        public int v_groupheadercolor;

        /// <summary>
        /// Cor de fundo do rodapé de grupo.
        /// </summary>
        public int v_groupfootercolor;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Settings"/>.
        /// </summary>
        public Settings()
        {
            this.v_topmargin = 20.0f;
            this.v_bottommargin = 20.0f;
            this.v_leftmargin = 20.0f;
            this.v_rightmargin = 20.0f;
        }

        /// <summary>
        /// Configura margem.
        /// </summary>
        /// <param name="p_margin">Tipo de margem.</param>
        /// <param name="p_text">Valor da margem.</param>
        public void SetMargin(Spartacus.Reporting.PageMargin p_margin, string p_text)
        {
            double v_temp;

            if (System.Double.TryParse(p_text, out v_temp))
            {
                switch (p_margin)
                {
                    case Spartacus.Reporting.PageMargin.TOP:
                        this.v_topmargin = v_temp;
                        break;
                    case Spartacus.Reporting.PageMargin.BOTTOM:
                        this.v_bottommargin = v_temp;
                        break;
                    case Spartacus.Reporting.PageMargin.LEFT:
                        this.v_leftmargin = v_temp;
                        break;
                    case Spartacus.Reporting.PageMargin.RIGHT:
                        this.v_rightmargin = v_temp;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Converte a string representando a cor para a opção correta da PDFjet.
        /// </summary>
        /// <returns>Cor conforme a PDFjet.</returns>
        /// <param name="p_text">Texto representando a cor.</param>
        public int GetColor(string p_text)
        {
            int v_color = PDFjet.NET.Color.white;

            switch (p_text)
            {
                case "ALICEBLUE":
                    v_color = PDFjet.NET.Color.aliceblue;
                    break;
                case "ANTIQUEWHITE":
                    v_color = PDFjet.NET.Color.antiquewhite;
                    break;
                case "AQUA":
                    v_color = PDFjet.NET.Color.aqua;
                    break;
                case "AQUAMARINE":
                    v_color = PDFjet.NET.Color.aquamarine;
                    break;
                case "AZURE":
                    v_color = PDFjet.NET.Color.azure;
                    break;
                case "BEIGE":
                    v_color = PDFjet.NET.Color.beige;
                    break;
                case "BISQUE":
                    v_color = PDFjet.NET.Color.bisque;
                    break;
                case "BLACK":
                    v_color = PDFjet.NET.Color.black;
                    break;
                case "BLANCHEDALMOND":
                    v_color = PDFjet.NET.Color.blanchedalmond;
                    break;
                case "BLUE":
                    v_color = PDFjet.NET.Color.blue;
                    break;
                case "BLUEVIOLET":
                    v_color = PDFjet.NET.Color.blueviolet;
                    break;
                case "BROWN":
                    v_color = PDFjet.NET.Color.brown;
                    break;
                case "BURLYWOOD":
                    v_color = PDFjet.NET.Color.burlywood;
                    break;
                case "CADETBLUE":
                    v_color = PDFjet.NET.Color.cadetblue;
                    break;
                case "CHARTREUSE":
                    v_color = PDFjet.NET.Color.chartreuse;
                    break;
                case "CHOCOLATE":
                    v_color = PDFjet.NET.Color.chocolate;
                    break;
                case "CORAL":
                    v_color = PDFjet.NET.Color.coral;
                    break;
                case "CORNFLOWERBLUE":
                    v_color = PDFjet.NET.Color.cornflowerblue;
                    break;
                case "CORNSILK":
                    v_color = PDFjet.NET.Color.cornsilk;
                    break;
                case "CRIMSON":
                    v_color = PDFjet.NET.Color.crimson;
                    break;
                case "CYAN":
                    v_color = PDFjet.NET.Color.cyan;
                    break;
                case "DARKBLUE":
                    v_color = PDFjet.NET.Color.darkblue;
                    break;
                case "DARKCYAN":
                    v_color = PDFjet.NET.Color.darkcyan;
                    break;
                case "DARKGOLDENROD":
                    v_color = PDFjet.NET.Color.darkgoldenrod;
                    break;
                case "DARKGRAY":
                    v_color = PDFjet.NET.Color.darkgray;
                    break;
                case "DARKGREEN":
                    v_color = PDFjet.NET.Color.darkgreen;
                    break;
                case "DARKKHAKI":
                    v_color = PDFjet.NET.Color.darkkhaki;
                    break;
                case "DARKMAGENTA":
                    v_color = PDFjet.NET.Color.darkmagenta;
                    break;
                case "DARKOLIVEGREEN":
                    v_color = PDFjet.NET.Color.darkolivegreen;
                    break;
                case "DARKORANGE":
                    v_color = PDFjet.NET.Color.darkorange;
                    break;
                case "DARKORCHID":
                    v_color = PDFjet.NET.Color.darkorchid;
                    break;
                case "DARKRED":
                    v_color = PDFjet.NET.Color.darkred;
                    break;
                case "DARKSALMON":
                    v_color = PDFjet.NET.Color.darksalmon;
                    break;
                case "DARKSEAGREEN":
                    v_color = PDFjet.NET.Color.darkseagreen;
                    break;
                case "DARKSLATEBLUE":
                    v_color = PDFjet.NET.Color.darkslateblue;
                    break;
                case "DARKSLATEGRAY":
                    v_color = PDFjet.NET.Color.darkslategray;
                    break;
                case "DARKTURQUOISE":
                    v_color = PDFjet.NET.Color.darkturquoise;
                    break;
                case "DARKVIOLET":
                    v_color = PDFjet.NET.Color.darkviolet;
                    break;
                case "DEEPPINK":
                    v_color = PDFjet.NET.Color.deeppink;
                    break;
                case "DEEPSLYBLUE":
                    v_color = PDFjet.NET.Color.deepskyblue;
                    break;
                case "DIMGRAY":
                    v_color = PDFjet.NET.Color.dimgray;
                    break;
                case "DODGERBLUE":
                    v_color = PDFjet.NET.Color.dodgerblue;
                    break;
                case "FIREBRICK":
                    v_color = PDFjet.NET.Color.firebrick;
                    break;
                case "FLORALWHTE":
                    v_color = PDFjet.NET.Color.floralwhite;
                    break;
                case "FORESTGREEN":
                    v_color = PDFjet.NET.Color.forestgreen;
                    break;
                case "FUCHSIA":
                    v_color = PDFjet.NET.Color.fuchsia;
                    break;
                case "GAINSBORO":
                    v_color = PDFjet.NET.Color.gainsboro;
                    break;
                case "GHOSTWHITE":
                    v_color = PDFjet.NET.Color.ghostwhite;
                    break;
                case "GOLD":
                    v_color = PDFjet.NET.Color.gold;
                    break;
                case "GOLDENROD":
                    v_color = PDFjet.NET.Color.goldenrod;
                    break;
                case "GRAY":
                    v_color = PDFjet.NET.Color.gray;
                    break;
                case "GREEN":
                    v_color = PDFjet.NET.Color.green;
                    break;
                case "GREENYELLOW":
                    v_color = PDFjet.NET.Color.greenyellow;
                    break;
                case "HONEYDEW":
                    v_color = PDFjet.NET.Color.honeydew;
                    break;
                case "HOTPINK":
                    v_color = PDFjet.NET.Color.hotpink;
                    break;
                case "INDIANRED":
                    v_color = PDFjet.NET.Color.indianred;
                    break;
                case "INDIGO":
                    v_color = PDFjet.NET.Color.indigo;
                    break;
                case "IVORY":
                    v_color = PDFjet.NET.Color.ivory;
                    break;
                case "KHAKI":
                    v_color = PDFjet.NET.Color.khaki;
                    break;
                case "LAVENDER":
                    v_color = PDFjet.NET.Color.lavender;
                    break;
                case "LAVENDERBLUSH":
                    v_color = PDFjet.NET.Color.lavenderblush;
                    break;
                case "LAWNGREEN":
                    v_color = PDFjet.NET.Color.lawngreen;
                    break;
                case "LEMONCHIFFON":
                    v_color = PDFjet.NET.Color.lemonchiffon;
                    break;
                case "LIGHTBLUE":
                    v_color = PDFjet.NET.Color.lightblue;
                    break;
                case "LIGHTCORAL":
                    v_color = PDFjet.NET.Color.lightcoral;
                    break;
                case "LIGHTCYAN":
                    v_color = PDFjet.NET.Color.lightcyan;
                    break;
                case "LIGHTGOLDENRODYELLOW":
                    v_color = PDFjet.NET.Color.lightgoldenrodyellow;
                    break;
                case "LIGHTGRAY":
                    v_color = PDFjet.NET.Color.lightgray;
                    break;
                case "LIGHTGREEN":
                    v_color = PDFjet.NET.Color.lightgreen;
                    break;
                case "LIGHTPINK":
                    v_color = PDFjet.NET.Color.lightpink;
                    break;
                case "LIGHTSALMON":
                    v_color = PDFjet.NET.Color.lightsalmon;
                    break;
                case "LIGHTSEAGREEN":
                    v_color = PDFjet.NET.Color.lightseagreen;
                    break;
                case "LIGHTSKYBLUE":
                    v_color = PDFjet.NET.Color.lightskyblue;
                    break;
                case "LIGHTSLATEGRAY":
                    v_color = PDFjet.NET.Color.lightslategray;
                    break;
                case "LIGHTSTEELBLUE":
                    v_color = PDFjet.NET.Color.lightsteelblue;
                    break;
                case "LIGHTYELLOW":
                    v_color = PDFjet.NET.Color.lightyellow;
                    break;
                case "LIME":
                    v_color = PDFjet.NET.Color.lime;
                    break;
                case "LIMEGREEN":
                    v_color = PDFjet.NET.Color.limegreen;
                    break;
                case "LINEN":
                    v_color = PDFjet.NET.Color.linen;
                    break;
                case "MAGENTA":
                    v_color = PDFjet.NET.Color.magenta;
                    break;
                case "MAROON":
                    v_color = PDFjet.NET.Color.maroon;
                    break;
                case "MEDIUMAQUAMARINE":
                    v_color = PDFjet.NET.Color.mediumaquamarine;
                    break;
                case "MEDIUMBLUE":
                    v_color = PDFjet.NET.Color.mediumblue;
                    break;
                case "MEDIUMORCHID":
                    v_color = PDFjet.NET.Color.mediumorchid;
                    break;
                case "MEDIUMPURPLE":
                    v_color = PDFjet.NET.Color.mediumpurple;
                    break;
                case "MEDIUMSEAGREEN":
                    v_color = PDFjet.NET.Color.mediumseagreen;
                    break;
                case "MEDIUMSLATEBLUE":
                    v_color = PDFjet.NET.Color.mediumslateblue;
                    break;
                case "MEDIUMSPRINGGREEN":
                    v_color = PDFjet.NET.Color.mediumspringgreen;
                    break;
                case "MEDIUMTURQUOISE":
                    v_color = PDFjet.NET.Color.mediumturquoise;
                    break;
                case "MEDIUMVIOLETRED":
                    v_color = PDFjet.NET.Color.mediumvioletred;
                    break;
                case "MIDNIGHTBLUE":
                    v_color = PDFjet.NET.Color.midnightblue;
                    break;
                case "MINTCREAM":
                    v_color = PDFjet.NET.Color.mintcream;
                    break;
                case "MISTYROSE":
                    v_color = PDFjet.NET.Color.mistyrose;
                    break;
                case "MOCCASIN":
                    v_color = PDFjet.NET.Color.moccasin;
                    break;
                case "NAVAJOWHITE":
                    v_color = PDFjet.NET.Color.navajowhite;
                    break;
                case "NAVY":
                    v_color = PDFjet.NET.Color.navy;
                    break;
                case "OLDGLORYBLUE":
                    v_color = PDFjet.NET.Color.oldgloryblue;
                    break;
                case "OLDGLORYRED":
                    v_color = PDFjet.NET.Color.oldgloryred;
                    break;
                case "OLDLACE":
                    v_color = PDFjet.NET.Color.oldlace;
                    break;
                case "OLIVE":
                    v_color = PDFjet.NET.Color.olive;
                    break;
                case "OLIVEDRAB":
                    v_color = PDFjet.NET.Color.olivedrab;
                    break;
                case "ORANGE":
                    v_color = PDFjet.NET.Color.orange;
                    break;
                case "ORANGERED":
                    v_color = PDFjet.NET.Color.orangered;
                    break;
                case "ORCHID":
                    v_color = PDFjet.NET.Color.orchid;
                    break;
                case "PALEGOLDENRED":
                    v_color = PDFjet.NET.Color.palegoldenrod;
                    break;
                case "PALEGREEN":
                    v_color = PDFjet.NET.Color.palegreen;
                    break;
                case "PALETURQUOISE":
                    v_color = PDFjet.NET.Color.paleturquoise;
                    break;
                case "PALEVIOLETRED":
                    v_color = PDFjet.NET.Color.palevioletred;
                    break;
                case "PAPAYAWHIP":
                    v_color = PDFjet.NET.Color.papayawhip;
                    break;
                case "PEACHPUFF":
                    v_color = PDFjet.NET.Color.peachpuff;
                    break;
                case "PERU":
                    v_color = PDFjet.NET.Color.peru;
                    break;
                case "PINK":
                    v_color = PDFjet.NET.Color.pink;
                    break;
                case "PLUM":
                    v_color = PDFjet.NET.Color.plum;
                    break;
                case "POWDERBLUE":
                    v_color = PDFjet.NET.Color.powderblue;
                    break;
                case "PURPLE":
                    v_color = PDFjet.NET.Color.purple;
                    break;
                case "RED":
                    v_color = PDFjet.NET.Color.red;
                    break;
                case "ROSYBROWN":
                    v_color = PDFjet.NET.Color.rosybrown;
                    break;
                case "ROYALBLUE":
                    v_color = PDFjet.NET.Color.royalblue;
                    break;
                case "SADDLEBROWN":
                    v_color = PDFjet.NET.Color.saddlebrown;
                    break;
                case "SALMON":
                    v_color = PDFjet.NET.Color.salmon;
                    break;
                case "SANDYBROWN":
                    v_color = PDFjet.NET.Color.sandybrown;
                    break;
                case "SEAGREEN":
                    v_color = PDFjet.NET.Color.seagreen;
                    break;
                case "SEASHELL":
                    v_color = PDFjet.NET.Color.seashell;
                    break;
                case "SIENNA":
                    v_color = PDFjet.NET.Color.sienna;
                    break;
                case "SILVER":
                    v_color = PDFjet.NET.Color.silver;
                    break;
                case "SKYBLUE":
                    v_color = PDFjet.NET.Color.skyblue;
                    break;
                case "SLATEBLUE":
                    v_color = PDFjet.NET.Color.slateblue;
                    break;
                case "SLATEGRAY":
                    v_color = PDFjet.NET.Color.slategray;
                    break;
                case "SNOW":
                    v_color = PDFjet.NET.Color.snow;
                    break;
                case "SPRINGGREEN":
                    v_color = PDFjet.NET.Color.springgreen;
                    break;
                case "STEELBLUE":
                    v_color = PDFjet.NET.Color.steelblue;
                    break;
                case "TAN":
                    v_color = PDFjet.NET.Color.tan;
                    break;
                case "TEAL":
                    v_color = PDFjet.NET.Color.teal;
                    break;
                case "THISTLE":
                    v_color = PDFjet.NET.Color.thistle;
                    break;
                case "TOMATO":
                    v_color = PDFjet.NET.Color.tomato;
                    break;
                case "TURQUOISE":
                    v_color = PDFjet.NET.Color.turquoise;
                    break;
                case "VIOLET":
                    v_color = PDFjet.NET.Color.violet;
                    break;
                case "WHEAT":
                    v_color = PDFjet.NET.Color.wheat;
                    break;
                case "WHITE":
                    v_color = PDFjet.NET.Color.white;
                    break;
                case "WHITESMOKE":
                    v_color = PDFjet.NET.Color.whitesmoke;
                    break;
                case "YELLOW":
                    v_color = PDFjet.NET.Color.yellow;
                    break;
                case "YELLOWGREEN":
                    v_color = PDFjet.NET.Color.yellowgreen;
                    break;
                default:
                    v_color = PDFjet.NET.Color.white;
                    break;
            }

            return v_color;
        }
    }
}

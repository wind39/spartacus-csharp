/**
 *  Font.cs
 *
Copyright (c) 2014, Innovatics Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
      this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and / or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace PDFjet.NET {
public class Font {

    internal String name;
    internal int objNumber;

    // The object number of the embedded font file
    internal int fileObjNumber = -1;

    // Font attributes
    internal int unitsPerEm = 1000;
    internal float size = 12f;
    internal float ascent;
    internal float descent;
    internal float capHeight;
    internal float body_height;

    // Font metrics
    internal int[][] metrics = null;

    // Don't change the following default values!
    internal bool isStandard = true;
    internal bool kernPairs = false;
    internal bool isComposite = false;
    internal int firstChar = 32;
    internal int lastChar = 255;
    internal bool skew15 = false;
    internal bool isCJK = false;

    // Font bounding box
    internal float bBoxLLx;
    internal float bBoxLLy;
    internal float bBoxURx;
    internal float bBoxURy;
    internal float underlinePosition;
    internal float underlineThickness;

    internal int compressed_size;
    internal int uncompressed_size;

    internal int[] advanceWidth = null;
    internal int[] glyphWidth = null;
    internal int[] unicodeToGID;

    internal String fontID;

    private int fontDescriptorObjNumber = -1;
    private int cMapObjNumber = -1;
    private int cidFontDictObjNumber = -1;
    private int toUnicodeCMapObjNumber = -1;
    private int widthsArrayObjNumber = -1;
    private int encodingObjNumber = -1;
    private int codePage = CodePage.CP1252;
    private int fontUnderlinePosition = 0;
    private int fontUnderlineThickness = 0;


    /**
     *  Constructor for the 14 standard fonts.
     *  Creates a font object and adds it to the PDF.
     *
     *  <pre>
     *  Examples:
     *      Font font1 = new Font(pdf, CoreFont.HELVETICA);
     *      Font font2 = new Font(pdf, CoreFont.TIMES_ITALIC);
     *      Font font3 = new Font(pdf, CoreFont.ZAPF_DINGBATS);
     *      ...
     *  </pre>
     *
     *  @param pdf the PDF to add this font to.
     *  @param coreFont the core font. Must be one the names defined in the CoreFont class.
     */
    public Font(PDF pdf, CoreFont coreFont) {
        StandardFont font = StandardFont.GetInstance(coreFont);
        this.name = font.name;
        this.bBoxLLx = font.bBoxLLx;
        this.bBoxLLy = font.bBoxLLy;
        this.bBoxURx = font.bBoxURx;
        this.bBoxURy = font.bBoxURy;
        this.fontUnderlinePosition = font.underlinePosition;
        this.fontUnderlineThickness = font.underlineThickness;
        this.metrics = font.metrics;
        this.ascent = bBoxURy * size / unitsPerEm;
        this.descent = bBoxLLy * size / unitsPerEm;
        this.body_height = ascent - descent;
        this.underlineThickness = fontUnderlineThickness * size / unitsPerEm;
        this.underlinePosition = fontUnderlinePosition * size / -unitsPerEm + underlineThickness / 2.0f;

        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Font\n");
        pdf.Append("/Subtype /Type1\n");
        pdf.Append("/BaseFont /");
        pdf.Append(this.name);
        pdf.Append('\n');
        if (!this.name.Equals("Symbol") && !this.name.Equals("ZapfDingbats")) {
            pdf.Append("/Encoding /WinAnsiEncoding\n");
        }
        pdf.Append(">>\n");
        pdf.Endobj();
        objNumber = pdf.objNumber;

        pdf.fonts.Add(this);
    }


    // Used by PDFobj
    internal Font(CoreFont coreFont) {
        StandardFont font = StandardFont.GetInstance(coreFont);
        this.name = font.name;
        this.bBoxLLx = font.bBoxLLx;
        this.bBoxLLy = font.bBoxLLy;
        this.bBoxURx = font.bBoxURx;
        this.bBoxURy = font.bBoxURy;
        this.metrics = font.metrics;
        this.ascent = bBoxURy * size / unitsPerEm;
        this.descent = bBoxLLy * size / unitsPerEm;
        this.body_height = ascent - descent;
        this.fontUnderlinePosition = font.underlinePosition;
        this.fontUnderlineThickness = font.underlineThickness;
        this.underlineThickness = fontUnderlineThickness * size / unitsPerEm;
        this.underlinePosition = fontUnderlinePosition * size / -unitsPerEm + underlineThickness / 2.0f;
    }


    // Constructor for CJK fonts
    public Font(PDF pdf, String fontName, int codePage) {
        this.name = fontName;
        this.codePage = codePage;
        isCJK = true;
        isStandard = false;
        isComposite = true;

        firstChar = 0x0020;
        lastChar = 0xFFEE;

        // Font Descriptor
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /FontDescriptor\n");
        pdf.Append("/FontName /");
        pdf.Append(fontName);
        pdf.Append('\n');
        pdf.Append("/Flags 4\n");
        pdf.Append("/FontBBox [0 0 0 0]\n");
        pdf.Append(">>\n");
        pdf.Endobj();

        // CIDFont Dictionary
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Font\n");
        pdf.Append("/Subtype /CIDFontType0\n");
        pdf.Append("/BaseFont /");
        pdf.Append(fontName);
        pdf.Append('\n');
        pdf.Append("/FontDescriptor ");
        pdf.Append(pdf.objNumber - 1);
        pdf.Append(" 0 R\n");
        pdf.Append("/CIDSystemInfo <<\n");
        pdf.Append("/Registry (Adobe)\n");
        if (fontName.StartsWith("AdobeMingStd")) {
            pdf.Append("/Ordering (CNS1)\n");
            pdf.Append("/Supplement 4\n");
        } else if (fontName.StartsWith("AdobeSongStd")
                || fontName.StartsWith("STHeitiSC")) {
            pdf.Append("/Ordering (GB1)\n");
            pdf.Append("/Supplement 4\n");
        } else if (fontName.StartsWith("KozMinPro")) {
            pdf.Append("/Ordering (Japan1)\n");
            pdf.Append("/Supplement 4\n");
        } else if (fontName.StartsWith("AdobeMyungjoStd")) {
            pdf.Append("/Ordering (Korea1)\n");
            pdf.Append("/Supplement 1\n");
        } else {
            throw new Exception("Unsupported font: " + fontName);
        }
        pdf.Append(">>\n");
        pdf.Append(">>\n");
        pdf.Endobj();

        // Type0 Font Dictionary
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Font\n");
        pdf.Append("/Subtype /Type0\n");
        pdf.Append("/BaseFont /");
        if (fontName.StartsWith("AdobeMingStd")) {
            pdf.Append(fontName + "-UniCNS-UTF16-H\n");
            pdf.Append("/Encoding /UniCNS-UTF16-H\n");
        } else if (fontName.StartsWith("AdobeSongStd")
                || fontName.StartsWith("STHeitiSC")) {
            pdf.Append(fontName + "-UniGB-UTF16-H\n");
            pdf.Append("/Encoding /UniGB-UTF16-H\n");
        } else if (fontName.StartsWith("KozMinPro")) {
            pdf.Append(fontName + "-UniJIS-UCS2-H\n");
            pdf.Append("/Encoding /UniJIS-UCS2-H\n");
        } else if (fontName.StartsWith("AdobeMyungjoStd")) {
            pdf.Append(fontName + "-UniKS-UCS2-H\n");
            pdf.Append("/Encoding /UniKS-UCS2-H\n");
        } else {
            throw new Exception("Unsupported font: " + fontName);
        }
        pdf.Append("/DescendantFonts [");
        pdf.Append(pdf.objNumber - 1);
        pdf.Append(" 0 R]\n");
        pdf.Append(">>\n");
        pdf.Endobj();
        objNumber = pdf.objNumber;

        ascent = size;
        descent = -ascent/4;
        body_height = ascent - descent;

        pdf.fonts.Add(this);
    }


    // Constructor for the DejaVuLGCSerif.ttf font.
    public Font(PDF pdf, Stream inputStream) {
        this.isStandard = false;
        this.isComposite = true;
        this.codePage = CodePage.UNICODE;

        FastFont.Register(pdf, this, inputStream);

        this.ascent = bBoxURy * size / unitsPerEm;
        this.descent = bBoxLLy * size / unitsPerEm;
        this.body_height = ascent - descent;
        this.underlineThickness = fontUnderlineThickness * size / unitsPerEm;
        this.underlinePosition = fontUnderlinePosition * size / -unitsPerEm + underlineThickness / 2f;

        pdf.fonts.Add(this);
    }


    internal int GetFontDescriptorObjNumber() {
        return fontDescriptorObjNumber;
    }


    internal int GetCMapObjNumber() {
        return cMapObjNumber;
    }


    internal int GetCidFontDictObjNumber() {
        return cidFontDictObjNumber;
    }


    internal int GetToUnicodeCMapObjNumber() {
        return toUnicodeCMapObjNumber;
    }


    internal int GetWidthsArrayObjNumber() {
        return widthsArrayObjNumber;
    }


    internal int GetEncodingObjNumber() {
        return encodingObjNumber;
    }


    internal float GetUnderlinePosition() {
        return underlinePosition;
    }


    internal float GetUnderlineThickness() {
        return underlineThickness;
    }


    internal void SetFontDescriptorObjNumber(int objNumber) {
        this.fontDescriptorObjNumber = objNumber;
    }


    internal void SetCMapObjNumber(int objNumber) {
        this.cMapObjNumber = objNumber;
    }


    internal void SetCidFontDictObjNumber(int objNumber) {
        this.cidFontDictObjNumber = objNumber;
    }


    internal void SetToUnicodeCMapObjNumber(int objNumber) {
        this.toUnicodeCMapObjNumber = objNumber;
    }


    internal void SetWidthsArrayObjNumber(int objNumber) {
        this.widthsArrayObjNumber = objNumber;
    }


    internal void SetEncodingObjNumber(int objNumber) {
        this.encodingObjNumber = objNumber;
    }


    public void SetSize(double fontSize) {
        SetSize((float) fontSize);
    }


    public void SetSize(float fontSize) {
        size = fontSize;
        if (isCJK) {
            ascent = size;
            descent = -ascent/4;
            return;
        }
        ascent = bBoxURy * size / unitsPerEm;
        descent = bBoxLLy * size / unitsPerEm;
        body_height = ascent - descent;

        underlineThickness = fontUnderlineThickness * size / unitsPerEm;
        underlinePosition = fontUnderlinePosition * size / -unitsPerEm + underlineThickness / 2.0f;
    }


    public float GetSize() {
        return size;
    }


    public void SetKernPairs(bool kernPairs) {
        this.kernPairs = kernPairs;
    }


    public float StringWidth(String str) {
        if (str == null) {
            return 0.0f;
        }

        int width = 0;
        if (isCJK) {
            return str.Length * ascent;
        }

        for (int i = 0; i < str.Length; i++) {
            int c1 = str[i];
            if (isStandard) {
                if (c1 < firstChar || c1 > lastChar) {
                    c1 = MapUnicodeChar(c1);
                }
                c1 -= 32;
                width += metrics[c1][1];

                if (kernPairs && i < (str.Length - 1)) {
                    int c2 = str[i + 1];
                    if (c2 < firstChar || c2 > lastChar) {
                        c2 = 32;
                    }
                    for (int j = 2; j < metrics[c1].Length; j += 2) {
                        if (metrics[c1][j] == c2) {
                            width += metrics[c1][j + 1];
                            break;
                        }
                    }
                }
            }
            else {
                if (c1 < firstChar || c1 > lastChar) {
                    width += advanceWidth[0];
                }
                else {
                    width += nonStandardFontGlyphWidth(c1);
                }
            }
        }

        return width * size / unitsPerEm;
    }


    private int nonStandardFontGlyphWidth(int c1) {
        int width = 0;

        if (isComposite) {
            width = glyphWidth[c1];
        }
        else {
            if (c1 < 127) {
                width = glyphWidth[c1];
            }
            else {
                if (codePage == 0) {
                    width = glyphWidth[CP1250.codes[c1 - 127]];
                }
                else if (codePage == 1) {
                    width = glyphWidth[CP1251.codes[c1 - 127]];
                }
                else if (codePage == 2) {
                    width = glyphWidth[CP1252.codes[c1 - 127]];
                }
                else if (codePage == 3) {
                    width = glyphWidth[CP1253.codes[c1 - 127]];
                }
                else if (codePage == 4) {
                    width = glyphWidth[CP1254.codes[c1 - 127]];
                }
                else if (codePage == 7) {
                    width = glyphWidth[CP1257.codes[c1 - 127]];
                }
            }
        }

        return width;
    }


    public float GetAscent() {
        return ascent;
    }


    public float GetDescent() {
        return -descent;
    }


    public float GetHeight() {
        return ascent - descent;
    }


    public float GetBodyHeight() {
        return body_height;
    }


    public int GetFitChars(String str, double width) {
        return GetFitChars(str, (float) width);
    }


    public int GetFitChars(String str, float width) {

        float w = width * unitsPerEm / size;

        if (isCJK) {
            return (int) (w / ascent);
        }

        if (isStandard) {
            return GetStandardFontFitChars(str, w);
        }

        int i;
        for (i = 0; i < str.Length; i++) {

            int c1 = str[i];

            if (c1 < firstChar || c1 > lastChar) {
                w -= advanceWidth[0];
            }
            else {
                w -= nonStandardFontGlyphWidth(c1);
            }

            if (w < 0) break;
        }

        return i;
    }


    private int GetStandardFontFitChars(String str, double width) {
        return GetStandardFontFitChars(str, (float) width);
    }


    private int GetStandardFontFitChars(String str, float width) {
        float w = width;

        int i = 0;
        while (i < str.Length) {

            int c1 = str[i];

            if (c1 < firstChar || c1 > lastChar) {
                c1 = 32;
            }

            c1 -= 32;
            w -= metrics[c1][1];

            if (w < 0) {
                return i;
            }

            if (kernPairs && i < (str.Length - 1)) {
                int c2 = str[i + 1];
                if (c2 < firstChar || c2 > lastChar) {
                    c2 = 32;
                }

                for (int j = 2; j < metrics[c1].Length; j += 2) {
                    if (metrics[c1][j] == c2) {
                        w -= metrics[c1][j + 1];
                        if (w < 0) {
                            return i;
                        }
                        break;
                    }
                }
            }

            i += 1;
        }

        return i;
    }


    public int MapUnicodeChar(int c1) {

        int[] codes = null;

        if (codePage == CodePage.CP1250) {
            codes = CP1250.codes;
        }
        else if (codePage == CodePage.CP1251) {
            codes = CP1251.codes;
        }
        else if (codePage == CodePage.CP1252) {
            codes = CP1252.codes;
        }
        else if (codePage == CodePage.CP1253) {
            codes = CP1253.codes;
        }
        else if (codePage == CodePage.CP1254) {
            codes = CP1254.codes;
        }
        else if (codePage == CodePage.CP1257) {
            codes = CP1257.codes;
        }

        if (codes != null) {
            for (int i = 0; i < codes.Length; i++) {
                if (codes[i] == c1) {
                    return 127 + i;
                }
            }
        }

        return 0x0020;
    }


   /**
    * Sets the skew15 private variable.
    * When the variable is set to 'true' all glyphs in the font are skewed on 15 degrees.
    * This makes a regular font look like an italic type font.
    * Use this method when you don't have real italic font in the font family,
    * or when you want to generate smaller PDF files.
    * For example you could embed only the Regular and Bold fonts and synthesize the RegularItalic and BoldItalic.
    * 
    * @param skew15 the skew flag.
    */
    public void SetItalic(bool skew15) {
        this.skew15 = skew15;
    }


    /**
     * Returns the width of a string drawn using two fonts.
     * 
     * @param font2 the fallback font.
     * @param str the string.
     * @return the width.
     */
    public float StringWidth(Font font2, String str) {
        if (font2 == null) {
            return StringWidth(str);
        }
        float width = 0f;

        Font activeFont = this;
        StringBuilder buf = new StringBuilder();
        for (int i = 0; i < str.Length; i++) {
            int ch = str[i];
            if ((isCJK && ch >= 0x4E00 && ch <= 0x9FCC)
                    || (!isCJK && unicodeToGID[ch] != 0)) {
                if (this != activeFont) {
                    width += activeFont.StringWidth(buf.ToString());
                    buf.Length = 0;
                    activeFont = this;
                }
            }
            else {
                if (font2 != activeFont) {
                    width += activeFont.StringWidth(buf.ToString());
                    buf.Length = 0;
                    activeFont = font2;
                }
            }
            buf.Append((char) ch);
        }
        width += activeFont.StringWidth(buf.ToString());

        return width;
    }

}   // End of Font.cs
}   // End of namespace PDFjet.NET

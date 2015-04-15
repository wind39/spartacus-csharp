/**
 *  FastFont.cs
 *
Copyright (c) 2015, Innovatics Inc.
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
class FastFont {

    internal static void Register(
            PDF pdf,
            Font font,
            Stream inputStream) {

        int len = inputStream.ReadByte();
        byte[] fontName = new byte[len];
        inputStream.Read(fontName, 0, len);
        font.name = System.Text.Encoding.UTF8.GetString(fontName);
        // Console.WriteLine(font.name);

        font.unitsPerEm = GetInt32(inputStream);
        font.bBoxLLx = GetInt32(inputStream);
        font.bBoxLLy = GetInt32(inputStream);
        font.bBoxURx = GetInt32(inputStream);
        font.bBoxURy = GetInt32(inputStream);
        font.ascent = GetInt32(inputStream);
        font.descent = GetInt32(inputStream);
        font.firstChar = GetInt32(inputStream);
        font.lastChar = GetInt32(inputStream);
        font.capHeight = GetInt32(inputStream);
        font.underlinePosition = GetInt32(inputStream);
        font.underlineThickness = GetInt32(inputStream);

        len = GetInt32(inputStream);
        font.advanceWidth = new int[len];
        for (int i = 0; i < len; i++) {
            font.advanceWidth[i] = GetInt16(inputStream);
        }

        len = GetInt32(inputStream);
        font.glyphWidth = new int[len];
        for (int i = 0; i < len; i++) {
            font.glyphWidth[i] = GetInt16(inputStream);
        }

        len = GetInt32(inputStream);
        font.unicodeToGID = new int[len];
        for (int i = 0; i < len; i++) {
            font.unicodeToGID[i] = GetInt16(inputStream);
        }

        font.uncompressed_size = GetInt32(inputStream);
        font.compressed_size = GetInt32(inputStream);

        EmbedFontFile(pdf, font, inputStream);
        AddFontDescriptorObject(pdf, font);
        AddCIDFontDictionaryObject(pdf, font);
        AddToUnicodeCMapObject(pdf, font);

        // Type0 Font Dictionary
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Font\n");
        pdf.Append("/Subtype /Type0\n");
        pdf.Append("/BaseFont /");
        pdf.Append(font.name);
        pdf.Append('\n');
        pdf.Append("/Encoding /Identity-H\n");
        pdf.Append("/DescendantFonts [");
        pdf.Append(font.GetCidFontDictObjNumber());
        pdf.Append(" 0 R]\n");
        pdf.Append("/ToUnicode ");
        pdf.Append(font.GetToUnicodeCMapObjNumber());
        pdf.Append(" 0 R\n");
        pdf.Append(">>\n");
        pdf.Endobj();

        font.objNumber = pdf.objNumber;
    }


    private static void EmbedFontFile(PDF pdf, Font font, Stream inputStream) {

        // Check if the font file is already embedded
        for (int i = 0; i < pdf.fonts.Count; i++) {
            Font f = pdf.fonts[i];
            if (f.name.Equals(font.name) && f.fileObjNumber != -1) {
                font.fileObjNumber = f.fileObjNumber;
                return;
            }
        }
/*
        int metadataObjNumber = pdf.AddMetadataObject(DejaVu.FONT_LICENSE, true);
*/
        pdf.Newobj();
        pdf.Append("<<\n");
/*
        pdf.Append("/Metadata ");
        pdf.Append(metadataObjNumber);
        pdf.Append(" 0 R\n");
*/
        pdf.Append("/Filter /FlateDecode\n");
        pdf.Append("/Length ");
        pdf.Append(font.compressed_size);
        pdf.Append("\n");

        pdf.Append("/Length1 ");
        pdf.Append(font.uncompressed_size);
        pdf.Append('\n');

        pdf.Append(">>\n");
        pdf.Append("stream\n");
        byte[] buf = new byte[2048];
        int len;
        while ((len = inputStream.Read(buf, 0, buf.Length)) > 0) {
            pdf.Append(buf, 0, len);
        }
        inputStream.Dispose();
        pdf.Append("\nendstream\n");
        pdf.Endobj();

        font.fileObjNumber = pdf.objNumber;
    }


    private static void AddFontDescriptorObject(PDF pdf, Font font) {

        float factor = 1000f / font.unitsPerEm;

        for (int i = 0; i < pdf.fonts.Count; i++) {
            Font f = pdf.fonts[i];
            if (f.name.Equals(font.name) && f.GetFontDescriptorObjNumber() != -1) {
                font.SetFontDescriptorObjNumber(f.GetFontDescriptorObjNumber());
                return;
            }
        }

        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /FontDescriptor\n");
        pdf.Append("/FontName /");
        pdf.Append(font.name);
        pdf.Append('\n');
        pdf.Append("/FontFile2 ");
        pdf.Append(font.fileObjNumber);
        pdf.Append(" 0 R\n");
        pdf.Append("/Flags 32\n");
        pdf.Append("/FontBBox [");
        pdf.Append(font.bBoxLLx * factor);
        pdf.Append(' ');
        pdf.Append(font.bBoxLLy * factor);
        pdf.Append(' ');
        pdf.Append(font.bBoxURx * factor);
        pdf.Append(' ');
        pdf.Append(font.bBoxURy * factor);
        pdf.Append("]\n");
        pdf.Append("/Ascent ");
        pdf.Append(font.ascent * factor);
        pdf.Append('\n');
        pdf.Append("/Descent ");
        pdf.Append(font.descent * factor);
        pdf.Append('\n');
        pdf.Append("/ItalicAngle 0\n");
        pdf.Append("/CapHeight ");
        pdf.Append(font.capHeight * factor);
        pdf.Append('\n');
        pdf.Append("/StemV 79\n");
        pdf.Append(">>\n");
        pdf.Endobj();

        font.SetFontDescriptorObjNumber(pdf.objNumber);
    }


    private static void AddToUnicodeCMapObject(PDF pdf, Font font) {

        for (int i = 0; i < pdf.fonts.Count; i++) {
            Font f = pdf.fonts[i];
            if (f.name.Equals(font.name) && f.GetToUnicodeCMapObjNumber() != -1) {
                font.SetToUnicodeCMapObjNumber(f.GetToUnicodeCMapObjNumber());
                return;
            }
        }

        StringBuilder sb = new StringBuilder();

        sb.Append("/CIDInit /ProcSet findresource begin\n");
        sb.Append("12 dict begin\n");
        sb.Append("begincmap\n");
        sb.Append("/CIDSystemInfo <</Registry (Adobe) /Ordering (Identity) /Supplement 0>> def\n");
        sb.Append("/CMapName /Adobe-Identity def\n");
        sb.Append("/CMapType 2 def\n");

        sb.Append("1 begincodespacerange\n");
        sb.Append("<0000> <FFFF>\n");
        sb.Append("endcodespacerange\n");

        List<String> list = new List<String>();
        StringBuilder buf = new StringBuilder();
        for (int cid = 0; cid <= 0xffff; cid++) {
            int gid = font.unicodeToGID[cid];
            if (gid > 0) {
                buf.Append('<');
                buf.Append(ToHexString(gid));
                buf.Append("> <");
                buf.Append(ToHexString(cid));
                buf.Append(">\n");
                list.Add(buf.ToString());
                buf.Length = 0;
                if (list.Count == 100) {
                    WriteListToBuffer(list, sb);
                }
            }
        }
        if (list.Count > 0) {
            WriteListToBuffer(list, sb);
        }

        sb.Append("endcmap\n");
        sb.Append("CMapName currentdict /CMap defineresource pop\n");
        sb.Append("end\nend");

        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Length ");
        pdf.Append(sb.Length);
        pdf.Append("\n");
        pdf.Append(">>\n");
        pdf.Append("stream\n");
        pdf.Append(sb.ToString());
        pdf.Append("\nendstream\n");
        pdf.Endobj();

        font.SetToUnicodeCMapObjNumber(pdf.objNumber);
    }


    private static void AddCIDFontDictionaryObject(PDF pdf, Font font) {

        for (int i = 0; i < pdf.fonts.Count; i++) {
            Font f = pdf.fonts[i];
            if (f.name.Equals(font.name) && f.GetCidFontDictObjNumber() != -1) {
                font.SetCidFontDictObjNumber(f.GetCidFontDictObjNumber());
                return;
            }
        }

        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Font\n");
        pdf.Append("/Subtype /CIDFontType2\n");
        pdf.Append("/BaseFont /");
        pdf.Append(font.name);
        pdf.Append('\n');
        pdf.Append("/CIDSystemInfo <</Registry (Adobe) /Ordering (Identity) /Supplement 0>>\n");
        pdf.Append("/FontDescriptor ");
        pdf.Append(font.GetFontDescriptorObjNumber());
        pdf.Append(" 0 R\n");
        pdf.Append("/DW ");
        pdf.Append((int)
                ((1000f / font.unitsPerEm) * font.advanceWidth[0]));
        pdf.Append('\n');
        pdf.Append("/W [0[\n");
        for (int i = 0; i < font.advanceWidth.Length; i++) {
            pdf.Append((int)
                    ((1000f / font.unitsPerEm) * font.advanceWidth[i]));
            if ((i + 1) % 10 == 0) {
                pdf.Append('\n');
            }
            else {
                pdf.Append(' ');
            }
        }
        pdf.Append("]]\n");
        pdf.Append("/CIDToGIDMap /Identity\n");
        pdf.Append(">>\n");
        pdf.Endobj();

        font.SetCidFontDictObjNumber(pdf.objNumber);
    }


    private static String ToHexString(int code) {
        String str = Convert.ToString(code, 16);
        if (str.Length == 1) {
            return "000" + str;
        }
        else if (str.Length == 2) {
            return "00" + str;
        }
        else if (str.Length == 3) {
            return "0" + str;
        }
        return str;
    }


    private static void WriteListToBuffer(List<String> list, StringBuilder sb) {
        sb.Append(list.Count);
        sb.Append(" beginbfchar\n");
        foreach (String str in list) {
            sb.Append(str);
        }
        sb.Append("endbfchar\n");
        list.Clear();
    }


    private static int GetInt16(Stream inputStream) {
        byte[] buf = new byte[2];
        inputStream.Read(buf, 0, 2);
        int val = 0;
        val |= buf[0] & 0xff;
        val <<= 8;
        val |= buf[1] & 0xff;
        return val;
    }


    private static int GetInt32(Stream inputStream) {
        byte[] buf = new byte[4];
        inputStream.Read(buf, 0, 4);
        int val = 0;
        val |= buf[0] & 0xff;
        val <<= 8;
        val |= buf[1] & 0xff;
        val <<= 8;
        val |= buf[2] & 0xff;
        val <<= 8;
        val |= buf[3] & 0xff;
        return val;
    }

}   // End of FastFont.cs
}   // End of namespace PDFjet.NET

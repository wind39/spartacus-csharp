/**
 *  OpenTypeFont.cs
 *
Copyright (c) 2014, Innovatics Inc.
All rights reserved.
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.IO.Compression;


namespace PDFjet.NET {
class OpenTypeFont {

    internal OTF otf = null;


    internal void RegisterAsSimple(
            PDF pdf,
            Font font,
            Stream inputStream,
            int codePage,
            bool embed) {

    	otf = new OTF(inputStream);
    	if (embed) {
            EmbedFontFile(pdf, font, otf, true);
        }

        AddFontDescriptorObject(pdf, font, otf, embed);
        AddWidthsArrayObject(pdf, font, otf, codePage);
        AddEncodingObject(pdf, font, codePage);

        // Simple font object
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Font\n");
        if (otf.cff) {
            pdf.Append("/Subtype /Type1\n");
        } else {
            pdf.Append("/Subtype /TrueType\n");
        }
        pdf.Append("/BaseFont /");
        pdf.Append(otf.fontName);
        pdf.Append('\n');
        pdf.Append("/FirstChar ");
        pdf.Append(otf.firstChar);
        pdf.Append('\n');
        pdf.Append("/LastChar ");
        pdf.Append(255);
        pdf.Append('\n');
        pdf.Append("/Encoding ");
        pdf.Append(font.GetEncodingObjNumber());
        pdf.Append(" 0 R\n");
        pdf.Append("/Widths ");
        pdf.Append(font.GetWidthsArrayObjNumber());
        pdf.Append(" 0 R\n");
        pdf.Append("/FontDescriptor ");
        pdf.Append(font.GetFontDescriptorObjNumber());
        pdf.Append(" 0 R\n");
        pdf.Append(">>\n");
        pdf.Endobj();

        font.objNumber = pdf.objNumber;

    }


    internal void RegisterAsComposite(
            PDF pdf,
            Font font,
            Stream inputStream,
            bool embed) {

    	otf = new OTF(inputStream);
        if (embed) {
            EmbedFontFile(pdf, font, otf, false);
        }

        AddFontDescriptorObject(pdf, font, otf, embed);
        AddCIDFontDictionaryObject(pdf, font, otf);
        AddToUnicodeCMapObject(pdf, font, otf);

        // Type0 Font Dictionary
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Font\n");
        pdf.Append("/Subtype /Type0\n");
        pdf.Append("/BaseFont /");
        pdf.Append(otf.fontName);
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


    private void EmbedFontFile(
            PDF pdf,
            Font font,
            OTF otf,
            bool simpleFont) {

        // Check if the font file is already embedded
        for (int i = 0; i < pdf.fonts.Count; i++) {
            Font f = pdf.fonts[i];
            if (f.name.Equals(otf.fontName) && f.fileObjNumber != -1) {
                font.fileObjNumber = f.fileObjNumber;
                return;
            }
        }

        int metadataObjNumber = -1;
        if (otf.fontName.ToLower().IndexOf("droid") != -1
                || otf.fontName.ToLower().IndexOf("roboto") != -1) {
            metadataObjNumber = pdf.AddMetadataObject(AndroidFontsCopyright.NOTICE, true);
        }

        pdf.Newobj();
        pdf.Append("<<\n");
        if (otf.cff) {
            if (simpleFont) {
                pdf.Append("/Subtype /Type1C\n");
            }
            else {
                pdf.Append("/Subtype /CIDFontType0C\n");
            }
        }
        pdf.Append("/Filter /FlateDecode\n");

        pdf.Append("/Length ");
        pdf.Append(otf.baos.Length);
        pdf.Append("\n");

        if (!otf.cff) {
            pdf.Append("/Length1 ");
            pdf.Append(otf.buf.Length);
            pdf.Append('\n');
        }

        if (metadataObjNumber != -1) {
            pdf.Append("/Metadata ");
            pdf.Append(metadataObjNumber);
            pdf.Append(" 0 R\n");
        }

        pdf.Append(">>\n");
        pdf.Append("stream\n");
        pdf.Append(otf.baos);
        pdf.Append("\nendstream\n");
        pdf.Endobj();

        font.fileObjNumber = pdf.objNumber;

    }


    private void AddFontDescriptorObject(
            PDF pdf,
            Font font,
            OTF otf,
            bool embed) {

        float factor = 1000f / otf.unitsPerEm;

        for (int i = 0; i < pdf.fonts.Count; i++) {
            Font f = pdf.fonts[i];
            if (f.name.Equals(otf.fontName) && f.GetFontDescriptorObjNumber() != -1) {
                font.SetFontDescriptorObjNumber(f.GetFontDescriptorObjNumber());
                return;
            }
        }

        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /FontDescriptor\n");
        pdf.Append("/FontName /");
        pdf.Append(otf.fontName);
        pdf.Append('\n');
        if (embed) {
            if (otf.cff) {
                pdf.Append("/FontFile3 ");
            } else {
                pdf.Append("/FontFile2 ");
            }
            pdf.Append(font.fileObjNumber);
            pdf.Append(" 0 R\n");
        }
        pdf.Append("/Flags 32\n");
        pdf.Append("/FontBBox [");
        pdf.Append(otf.bBoxLLx * factor);
        pdf.Append(' ');
        pdf.Append(otf.bBoxLLy * factor);
        pdf.Append(' ');
        pdf.Append(otf.bBoxURx * factor);
        pdf.Append(' ');
        pdf.Append(otf.bBoxURy * factor);
        pdf.Append("]\n");
        pdf.Append("/Ascent ");
        pdf.Append(otf.ascent * factor);
        pdf.Append('\n');
        pdf.Append("/Descent ");
        pdf.Append(otf.descent * factor);
        pdf.Append('\n');
        pdf.Append("/ItalicAngle 0\n");
        pdf.Append("/CapHeight ");
        pdf.Append(otf.capHeight * factor);
        pdf.Append('\n');
        pdf.Append("/StemV 79\n");
        pdf.Append(">>\n");
        pdf.Endobj();

        font.SetFontDescriptorObjNumber(pdf.objNumber);

    }


    private void AddWidthsArrayObject(
            PDF pdf,
            Font font,
            OTF otf,
            int codePage) {

        pdf.Newobj();
        pdf.Append("[ ");
        int n = 1;
        for (int c = otf.firstChar; c < 256; c++) {
            if (c < 127) {
                pdf.Append((int)
                        ((1000.0f / otf.unitsPerEm) * otf.glyphWidth[c]));
            } else {
                if (codePage == 0) {
                    pdf.Append((int) ((1000.0f / otf.unitsPerEm)
                            * otf.glyphWidth[CP1250.codes[c - 127]]));
                } else if (codePage == 1) {
                    pdf.Append((int) ((1000.0f / otf.unitsPerEm)
                            * otf.glyphWidth[CP1251.codes[c - 127]]));
                } else if (codePage == 2) {
                    pdf.Append((int) ((1000.0f / otf.unitsPerEm)
                            * otf.glyphWidth[CP1252.codes[c - 127]]));
                } else if (codePage == 3) {
                    pdf.Append((int) ((1000.0f / otf.unitsPerEm)
                            * otf.glyphWidth[CP1253.codes[c - 127]]));
                } else if (codePage == 4) {
                    pdf.Append((int) ((1000.0f / otf.unitsPerEm)
                            * otf.glyphWidth[CP1254.codes[c - 127]]));
                } else if (codePage == 7) {
                    pdf.Append((int) ((1000.0f / otf.unitsPerEm)
                            * otf.glyphWidth[CP1257.codes[c - 127]]));
                }
            }
            if (n < 10) {
                pdf.Append(' ');
                ++n;
            }
            else {
                pdf.Append('\n');
                n = 1;
            }
        }
        pdf.Append("]\n");
        pdf.Endobj();

        font.SetWidthsArrayObjNumber(pdf.objNumber);

    }


    private void AddEncodingObject(
            PDF pdf,
            Font font,
            int codePage) {

        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Encoding\n");
        pdf.Append("/BaseEncoding /WinAnsiEncoding\n");
        pdf.Append("/Differences [127\n");
        for (int i = 0; i < 129; i++) {
            if (codePage == 0) {
                pdf.Append(CP1250.names[i]);
            }
            else if (codePage == 1) {
                pdf.Append(CP1251.names[i]);
            }
            else if (codePage == 2) {
                pdf.Append(CP1252.names[i]);
            }
            else if (codePage == 3) {
                pdf.Append(CP1253.names[i]);
            }
            else if (codePage == 4) {
                pdf.Append(CP1254.names[i]);
            }
            else if (codePage == 7) {
                pdf.Append(CP1257.names[i]);
            }
            pdf.Append(' ');
        }
        pdf.Append("]\n");
        pdf.Append(">>\n");
        pdf.Endobj();

        font.SetEncodingObjNumber(pdf.objNumber);

    }


    private void AddToUnicodeCMapObject(
            PDF pdf,
            Font font,
            OTF otf) {

        for (int i = 0; i < pdf.fonts.Count; i++) {
            Font f = pdf.fonts[i];
            if (f.name.Equals(otf.fontName) && f.GetToUnicodeCMapObjNumber() != -1) {
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
            int gid = otf.unicodeToGID[cid];
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


    private void AddCIDFontDictionaryObject(
            PDF pdf,
            Font font,
            OTF otf) {

        for (int i = 0; i < pdf.fonts.Count; i++) {
            Font f = pdf.fonts[i];
            if (f.name.Equals(otf.fontName) && f.GetCidFontDictObjNumber() != -1) {
                font.SetCidFontDictObjNumber(f.GetCidFontDictObjNumber());
                return;
            }
        }

        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /Font\n");
        if (otf.cff) {
            pdf.Append("/Subtype /CIDFontType0\n");
        } else {
            pdf.Append("/Subtype /CIDFontType2\n");
        }
        pdf.Append("/BaseFont /");
        pdf.Append(otf.fontName);
        pdf.Append('\n');
        pdf.Append("/CIDSystemInfo <</Registry (Adobe) /Ordering (Identity) /Supplement 0>>\n");
        pdf.Append("/FontDescriptor ");
        pdf.Append(font.GetFontDescriptorObjNumber());
        pdf.Append(" 0 R\n");
        pdf.Append("/DW ");
        pdf.Append((int)
                ((1000f / otf.unitsPerEm) * otf.advanceWidth[0]));
        pdf.Append('\n');
        pdf.Append("/W [0[\n");
        for (int i = 0; i < otf.advanceWidth.Length; i++) {
            pdf.Append((int)
                    ((1000f / otf.unitsPerEm) * otf.advanceWidth[i]));
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


    private String ToHexString(int code) {
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


    private void WriteListToBuffer(List<String> list, StringBuilder sb) {
        sb.Append(list.Count);
        sb.Append(" beginbfchar\n");
        foreach (String str in list) {
            sb.Append(str);
        }
        sb.Append("endbfchar\n");
        list.Clear();
    }

}   // End of OpenTypeFont.cs
}   // End of namespace PDFjet.NET

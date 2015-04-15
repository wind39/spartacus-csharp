/**
 *  OTF.cs
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
class OTF {

    internal String fontName;
    internal bool cff = false;
    internal MemoryStream baos;
    internal byte[] buf;
    internal int unitsPerEm;
    internal short bBoxLLx;
    internal short bBoxLLy;
    internal short bBoxURx;
    internal short bBoxURy;
    internal short ascent;
    internal short descent;
    internal int[] advanceWidth;
    internal int firstChar;
    internal int lastChar;
    internal int capHeight;
    internal int[] glyphWidth;
    internal long postVersion;
    internal long italicAngle;
    internal short underlinePosition;
    internal short underlineThickness;

    private Stream stream;
    private DeflaterOutputStream dos;
    private int cff_off;
    private int cff_len;
    private int index = 0;

    internal int[] unicodeToGID = new int[0x10000];
    internal int[] halfForm = new int[36*36];


    public OTF(Stream stream) {
        this.stream = stream;
        this.baos = new MemoryStream();

    	byte[] buffer = new byte[0x10000];
    	int count;
    	while ((count = stream.Read(buffer, 0, buffer.Length)) > 0) {
    		baos.Write(buffer, 0, count);
    	}
        stream.Dispose();
        buf = baos.ToArray();

        baos = new MemoryStream();
        dos = new DeflaterOutputStream(baos);

        // Extract OTF metadata
        long version = ReadUInt32();
        if (version == 0x00010000L ||   // Win OTF
            version == 0x74727565L ||   // Mac TTF
            version == 0x4F54544FL) {   // CFF OTF
            // We should be able to read this font
        }
        else {
            throw new Exception(
                    "OTF version == " + version + " is not supported.");
        }

        int numOfTables   = ReadUInt16();
        int searchRange   = ReadUInt16();
        int entrySelector = ReadUInt16();
        int rangeShift    = ReadUInt16();

        FontTable cmapTable = null;
        for (int i = 0; i < numOfTables; i++) {
            char[] name = new char[4];
            for (int j = 0; j < 4; j++) {
                name[j] = (char) ReadByte();
            }
            FontTable table = new FontTable();
            table.name     = new String(name);
            table.checkSum = ReadUInt32();
            table.offset = (int) ReadUInt32();
            table.length = (int) ReadUInt32();

            int k = index;  // Save the current index
            if      (table.name.Equals("head")) { Head(table); }
            else if (table.name.Equals("hhea")) { Hhea(table); }
            else if (table.name.Equals("OS/2")) { OS_2(table); }
            else if (table.name.Equals("name")) { Name(table); }
            else if (table.name.Equals("hmtx")) { Hmtx(table); }
            else if (table.name.Equals("post")) { Post(table); }
            else if (table.name.Equals("CFF ")) { CFF_(table); }
            else if (table.name.Equals("GSUB")) { GSUB(table); }
            else if (table.name.Equals("cmap")) { cmapTable = table; }
            index = k;      // Restore the index
        }

        // This table must be processed last
        Cmap(cmapTable);

        if (cff) {
            dos.Write(buf, cff_off, cff_len);
        }
        else {
            dos.Write(buf, 0, buf.Length);
        }
        dos.Finish();
    }

    private void Head(FontTable table) {
        index = table.offset + 16;
        int flags  = ReadUInt16();
        unitsPerEm = ReadUInt16();
        index += 16;
        bBoxLLx = (short) ReadUInt16();
        bBoxLLy = (short) ReadUInt16();
        bBoxURx = (short) ReadUInt16();
        bBoxURy = (short) ReadUInt16();
    }

    private void Hhea(FontTable table) {
        index = table.offset + 4;
        ascent  = (short) ReadUInt16();
        descent = (short) ReadUInt16();
        index += 26;
        advanceWidth = new int[ReadUInt16()];
    }

    private void OS_2(FontTable table) {
        index = table.offset + 64;
        firstChar = ReadUInt16();
        lastChar  = ReadUInt16();
        index += 20;
        capHeight = (short) ReadUInt16();
    }

    private void Name(FontTable table) {
        index = table.offset;
        int tableOffset = index;
        int format = ReadUInt16();
        int count  = ReadUInt16();
        int stringOffset = ReadUInt16();
        for (int r = 0; r < count; r++) {
            int platformID = ReadUInt16();
            int encodingID = ReadUInt16();
            int languageID = ReadUInt16();
            int nameID = ReadUInt16();
            int length = ReadUInt16();
            int offset = ReadUInt16();
            int fontNameOffset = tableOffset + stringOffset + offset;
            if (platformID == 1) {      // Macintosh
                if (encodingID == 0 && languageID == 0 && nameID == 6) {
                    index = fontNameOffset;
                    char[] buf = new char[length];
                    for (int i = 0; i < length; i++) {
                        buf[i] = (char) ReadByte();
                    }
                    fontName = new String(buf);
                    break;
                }
            }
            else if (platformID == 3) { // Windows
                if (encodingID == 1 && languageID == 0x409 && nameID == 6) {
                    index = fontNameOffset;
                    int len = length/2;
                    char[] buf = new char[len];
                    for (int i = 0; i < len; i++) {
                        byte b1 = ReadByte();
                        byte b2 = ReadByte();
                        buf[i] = (char) b2;
                    }
                    fontName = new String(buf);
                    break;
                }
            }
        }
    }

    private void Cmap(FontTable table) {
        index = table.offset;
        int tableOffset = index;
        index += 2;
        int numRecords = ReadUInt16();

        // Process the encoding records
        bool format4subtable = false;
        int subtableOffset = 0;
        for (int i = 0; i < numRecords; i++) {
            int platformID = ReadUInt16();
            int encodingID = ReadUInt16();
            subtableOffset = (int) ReadUInt32();
            if (platformID == 3 && encodingID == 1) {
                format4subtable = true;
                break;
            }
        }
        if (!format4subtable) {
            throw new Exception("Format 4 subtable not found in this font.");
        }

        index = tableOffset + subtableOffset;

        int format   = ReadUInt16();
        int tableLen = ReadUInt16();
        int language = ReadUInt16();
        int segCount = ReadUInt16() / 2;

        index += 6; // Skip to the endCount[]
        int[] endCount = new int[segCount];
        for (int j = 0; j < segCount; j++) {
            endCount[j] = ReadUInt16();
        }

        index += 2; // Skip the reservedPad
        int[] startCount = new int[segCount];
        for (int j = 0; j < segCount; j++) {
            startCount[j] = ReadUInt16();
        }

        short[] idDelta = new short[segCount];
        for (int j = 0; j < segCount; j++) {
            idDelta[j] = (short) ReadUInt16();
        }

        int[] idRangeOffset = new int[segCount];
        for (int j = 0; j < segCount; j++) {
            idRangeOffset[j] = ReadUInt16();
        }

        int[] glyphIdArray = new int[(tableLen - (16 + 8*segCount)) / 2];
        for (int j = 0; j < glyphIdArray.Length; j++) {
            glyphIdArray[j] = ReadUInt16();
        }

        glyphWidth = new int[lastChar + 1];
        for (int i = 0; i < glyphWidth.Length; i++) { glyphWidth[i] = advanceWidth[0]; }

        for (int c = firstChar; c <= lastChar; c++) {
            int seg = GetSegmentFor(c, startCount, endCount, segCount);
            if (seg != -1) {
                int gid;
                int offset = idRangeOffset[seg];
                if (offset == 0) {
                    gid = (idDelta[seg] + c) % 65536;
                }
                else {
                    offset /= 2;
                    offset -= segCount - seg;
                    gid = glyphIdArray[offset + (c - startCount[seg])];
                    if (gid != 0) {
                        gid += idDelta[seg] % 65536;
                    }
                }
    
                if (gid < advanceWidth.Length) {
                    glyphWidth[c] = advanceWidth[gid];
                }

                unicodeToGID[c] = gid;
            }
        }
    }

    private void Hmtx(FontTable table) {
        index = table.offset;
        for (int j = 0; j < advanceWidth.Length; j++) {
            advanceWidth[j] = ReadUInt16();
            index += 2;
        }
    }

    private void Post(FontTable table) {
        index = table.offset;
        postVersion = ReadUInt32();
        italicAngle = ReadUInt32();
        underlinePosition  = (short) ReadUInt16();
        underlineThickness = (short) ReadUInt16();
    }

    private void CFF_(FontTable table) {
        this.cff = true;
        this.cff_off = table.offset;
        this.cff_len = table.length;
    }

    private void GSUB(FontTable table) {
        index = table.offset;
        int tableOffset = index;
        /*
        Type    Name        Description
        Fixed   Version     Version of the GSUB table-initially set to 0x00010000
        Offset  ScriptList  Offset to ScriptList table-from beginning of GSUB table
        Offset  FeatureList Offset to FeatureList table-from beginning of GSUB table
        Offset  LookupList  Offset to LookupList table-from beginning of GSUB table
        */
        long version = ReadUInt32();
        int scriptList  = ReadUInt16();
        int featureList = ReadUInt16();
        int lookupList  = ReadUInt16();
/*
Console.WriteLine();
Console.WriteLine("GSUB");
Console.WriteLine("version == " + version.ToString("x"));
*/
        index = tableOffset + scriptList;
        int scriptCount = ReadUInt16();
// Console.WriteLine("scriptCount == " + scriptCount);
        for (int i = 0; i < scriptCount; i++) {

        }

        index = tableOffset + featureList;
        int featureCount = ReadUInt16();
// Console.WriteLine("featureCount == " + featureCount);
        for (int i = 0; i < featureCount; i++) {

        }

        index = tableOffset + lookupList;
        int lookupCount = ReadUInt16();
// Console.WriteLine("lookupCount == " + lookupCount);
        int[] lookupTable = new int[lookupCount];
        for (int i = 0; i < lookupCount; i++) {
            lookupTable[i] = ReadUInt16();
        }

        for (int i = 0; i < lookupTable.Length; i++) {
            index = tableOffset + lookupList + lookupTable[i];
            int lookupType = ReadUInt16();
            int lookupFlag = ReadUInt16();
            int subTableCount = ReadUInt16();
/*
Console.WriteLine("lookupType == " + lookupType);
Console.WriteLine("lookupFlag == " + lookupFlag);
Console.WriteLine("subTableCount == " + subTableCount);
*/
        }
    }

    private int GetSegmentFor(
            int c, int[] startCount, int[] endCount, int segCount) {
        int segment = -1;
        for (int i = 0; i < segCount; i++) {
            if (c <= endCount[i] && c >= startCount[i]) {
                segment = i;
                break;
            }
        }
        return segment;
    }

    private byte ReadByte() {
        return buf[index++];
    }

    private int ReadUInt16() {
        int val = 0;
        val |= (buf[index++] <<  8) & 0x0000FF00;
        val |= (buf[index++])       & 0x000000FF;
        return val;
    }

    private long ReadUInt32() {
        long val = 0L;
        val |= (buf[index++] << 24) & 0xFF000000L;
        val |= (buf[index++] << 16) & 0x00FF0000L;
        val |= (buf[index++] <<  8) & 0x0000FF00L;
        val |= (buf[index++])       & 0x000000FFL;
        return val;
    }

    internal void SetHalfForm(char ch1, char ch2, int gid) {
        halfForm[36*(ch1 - 0x0915) + (ch2 - 0x0915)] = gid;
    }

    internal int GetHalfForm(char ch1, char ch2) {
        return halfForm[36*(ch1 - 0x0915) + (ch2 - 0x0915)];
    }

}   // End of OTF.cs
}   // End of namespace PDFjet.NET

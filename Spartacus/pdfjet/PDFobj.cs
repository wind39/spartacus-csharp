/**
 *  PDFobj.cs
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
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace PDFjet.NET {
/**
 *  Used to create Java or .NET objects that represent the objects in PDF document. 
 *  See the PDF specification for more information.
 *
 */
public class PDFobj {

    internal int offset;           // The object offset
    internal int number;           // The object number
    internal List<String> dict;
    internal int stream_offset;
    internal byte[] stream;        // The compressed stream
    internal byte[] data;          // The decompressed data


    /**
     *  Used to create Java or .NET objects that represent the objects in PDF document. 
     *  See the PDF specification for more information.
     *  Also see Example_19.
     *
     *  @param offset the object offset in the offsets table.
     */
    public PDFobj(int offset) {
        this.offset = offset;
        this.dict = new List<String>();
    }


    internal PDFobj() {
        this.dict = new List<String>();
    }


    public List<String> GetDict() {
        return this.dict;
    }


    public byte[] GetData() {
        return this.data;
    }


    internal void SetStream(byte[] pdf, int length) {
        stream = new byte[length];
        Array.Copy(pdf, this.stream_offset, stream, 0, length);
    }


    internal void SetStream(byte[] stream) {
        this.stream = stream;
    }


    internal int GetNumber() {
        return this.number;
    }


    internal void SetNumber(int number) {
        this.number = number;
    }


    /**
     *  Returns the parameter value given the specified key.
     *
     *  @param key the specified key.
     *
     *  @return the value.
     */
    public String GetValue(String key) {
        for (int i = 0; i < dict.Count; i++) {
            String token = dict[i];
            if (token.Equals(key)) {
                return dict[i + 1];
            }
        }
        return "";
    }


    internal List<Int32> GetObjectNumbers(String key) {
        List<Int32> numbers = new List<Int32>();
        for (int i = 0; i < dict.Count; i++) {
            String token = dict[i];
            if (token.Equals(key)) {
                String str = dict[++i];
                if (str.Equals("[")) {
                    for (;;) {
                        str = dict[++i];
                        if (str.Equals("]")) {
                            break;
                        }
                        numbers.Add(Int32.Parse(str));
                        ++i;    // 0
                        ++i;    // R
                    }
                }
                else {
                    numbers.Add(Int32.Parse(str));
                }
                break;
            }
        }
        return numbers;
    }


    public void AddContentObject(int number) {
        int index = -1;
        for (int i = 0; i < dict.Count; i++) {
            if (dict[i].Equals("/Contents")) {
                String str = dict[++i];
                if (str.Equals("[")) {
                    for (;;) {
                        str = dict[++i];
                        if (str.Equals("]")) {
                            index = i;
                            break;
                        }
                        ++i;    // 0
                        ++i;    // R
                    }
                }
                break;
            }
        }
        dict.Insert(index, "R");
        dict.Insert(index, "0");
        dict.Insert(index, number.ToString());
    }


    public void AddContent(byte[] content, SortedDictionary<Int32, PDFobj> objects) {
        PDFobj obj = new PDFobj();

        int maxObjNumber = -1;
        foreach (int number in objects.Keys) {
            if (number > maxObjNumber) { maxObjNumber = number; }
        }
        obj.SetNumber(maxObjNumber + 1);

        obj.SetStream(content);
        objects.Add(obj.GetNumber(), obj);

        int index = -1;
        bool single = false;
        for (int i = 0; i < dict.Count; i++) {
            if (dict[i].Equals("/Contents")) {
                String str = dict[++i];
                if (str.Equals("[")) {
                    for (;;) {
                        str = dict[++i];
                        if (str.Equals("]")) {
                            index = i;
                            break;
                        }
                        ++i;    // 0
                        ++i;    // R
                    }
                }
                else {
                    // Single content object
                    index = i;
                    single = true;
                }
                break;
            }
        }

        if (single) {
            dict.Insert(index, "[");
            dict.Insert(index + 4, "]");
            dict.Insert(index + 4, "R");
            dict.Insert(index + 4, "0");
            dict.Insert(index + 4, obj.number.ToString());
        }
        else {
            dict.Insert(index, "R");
            dict.Insert(index, "0");
            dict.Insert(index, obj.number.ToString());
        }
    }


    public float[] GetPageSize() {
        for (int i = 0; i < dict.Count; i++) {
            if (dict[i].Equals("/MediaBox")) {
                return new float[] {
                        Convert.ToSingle(dict[i + 4]),
                        Convert.ToSingle(dict[i + 5]) };
            }
        }
        return Letter.PORTRAIT;
    }


    internal int GetLength(List<PDFobj> objects) {
        for (int i = 0; i < dict.Count; i++) {
            String token = dict[i];
            if (token.Equals("/Length")) {
                int number = Int32.Parse(dict[i + 1]);
                if (dict[i + 2].Equals("0") &&
                        dict[i + 3].Equals("R")) {
                    return GetLength(objects, number);
                }
                else {
                    return number;
                }
            }
        }
        return 0;
    }


    internal int GetLength(List<PDFobj> objects, int number) {
        foreach (PDFobj obj in objects) {
            if (obj.number == number) {
                return Int32.Parse(obj.dict[3]);
            }
        }
        return 0;
    }


    public Font AddFontResource(CoreFont coreFont, SortedDictionary<Int32, PDFobj> objects) {
        Font font = new Font(coreFont);
        font.fontID = font.name.Replace('-', '_').ToUpper();

        PDFobj obj = new PDFobj();

        int maxObjNumber = -1;
        foreach (int number in objects.Keys) {
            if (number > maxObjNumber) { maxObjNumber = number; }
        }
        obj.number = maxObjNumber + 1;

        obj.dict.Add("<<");
        obj.dict.Add("/Type");
        obj.dict.Add("/Font");
        obj.dict.Add("/Subtype");
        obj.dict.Add("/Type1");
        obj.dict.Add("/BaseFont");
        obj.dict.Add("/" + font.name);
        if (!font.name.Equals("Symbol") && !font.name.Equals("ZapfDingbats")) {
            obj.dict.Add("/Encoding");
            obj.dict.Add("/WinAnsiEncoding");
        }
        obj.dict.Add(">>");

        objects.Add(obj.number, obj);

        for (int i = 0; i < dict.Count; i++) {
            if (dict[i].Equals("/Resources")) {
                String token = dict[++i];
                if (token.Equals("<<")) {                   // Direct resources object
                    AddFontResource(this, objects, font.fontID, obj.number);
                }
                else if (Char.IsDigit(token[0])) {          // Indirect resources object
                    AddFontResource(objects[Int32.Parse(token)], objects, font.fontID, obj.number);
                }
            }
        }

        return font;
    }


    private void AddFontResource(
            PDFobj obj, SortedDictionary<Int32, PDFobj> objects, String fontID, int number) {
        for (int i = 0; i < obj.dict.Count; i++) {
            String token = null;
            if (obj.dict[i].Equals("/Font")) {
                token = obj.dict[++i];
                if (token.Equals("<<")) {
                    obj.dict.Insert(++i, "/" + fontID);
                    obj.dict.Insert(++i, number.ToString());
                    obj.dict.Insert(++i, "0");
                    obj.dict.Insert(++i, "R");
                    break;
                }
                else if (Char.IsDigit(token[0])) {
                    PDFobj o2 = objects[Int32.Parse(token)];
                    for (int j = 0; j < o2.dict.Count; j++) {
                        token = o2.dict[j];
                        if (token.Equals("<<")) {
                            o2.dict.Insert(++j, "/" + fontID);
                            o2.dict.Insert(++j, number.ToString());
                            o2.dict.Insert(++j, "0");
                            o2.dict.Insert(++j, "R");
                            break;
                        }
                    }
                }
            }
        }
    }

}
}   // End of namespace PDFjet.NET

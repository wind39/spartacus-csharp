/**
 *  BarCode.cs
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
using System.Text;
using System.Collections.Generic;


namespace PDFjet.NET {
/**
 *  Used to create one dimentional barcodes - UPC, Code 39 and Code 128.
 *
 *  Please see Example_11.
 */
public class BarCode : IDrawable {

    public static readonly int UPC = 0;
    public static readonly int CODE128 = 1;
    public static readonly int CODE39 = 2;

    public static readonly int LEFT_TO_RIGHT = 0;
    public static readonly int TOP_TO_BOTTOM = 1;
    public static readonly int BOTTOM_TO_TOP = 2;
    
    private int[] tableA = {
        3211,2221,2122,1411,1132,1231,1114,1312,1213,3112
    };

    private float x1 = 0.0f;
    private float y1 = 0.0f;

    private float m1 = 0.75f;   // Module length
    private float barHeightFactor = 50.0f;

    private int type = 0;
    private int direction = LEFT_TO_RIGHT;
    private String str = null;
    private Font font = null;

    private Dictionary<Char, String> map = new Dictionary<Char, String>();


    /**
     *  The constructor.
     *
     *  @param type the type of the barcode.
     *  @param str the content string of the barcode.
     */
    public BarCode(int type, String str) {
        this.type = type;
        this.str = str;

        map.Add( '*', "bWbwBwBwb" );
        map.Add( '-', "bWbwbwBwB" );
        map.Add( '$', "bWbWbWbwb" );
        map.Add( '%', "bwbWbWbWb" );
        map.Add( ' ', "bWBwbwBwb" );
        map.Add( '.', "BWbwbwBwb" );
        map.Add( '/', "bWbWbwbWb" );
        map.Add( '+', "bWbwbWbWb" );
        map.Add( '0', "bwbWBwBwb" );
        map.Add( '1', "BwbWbwbwB" );
        map.Add( '2', "bwBWbwbwB" );
        map.Add( '3', "BwBWbwbwb" );
        map.Add( '4', "bwbWBwbwB" );
        map.Add( '5', "BwbWBwbwb" );
        map.Add( '6', "bwBWBwbwb" );
        map.Add( '7', "bwbWbwBwB" );
        map.Add( '8', "BwbWbwBwb" );
        map.Add( '9', "bwBWbwBwb" );
        map.Add( 'A', "BwbwbWbwB" );
        map.Add( 'B', "bwBwbWbwB" );
        map.Add( 'C', "BwBwbWbwb" );
        map.Add( 'D', "bwbwBWbwB" );
        map.Add( 'E', "BwbwBWbwb" );
        map.Add( 'F', "bwBwBWbwb" );
        map.Add( 'G', "bwbwbWBwB" );
        map.Add( 'H', "BwbwbWBwb" );
        map.Add( 'I', "bwBwbWBwb" );
        map.Add( 'J', "bwbwBWBwb" );
        map.Add( 'K', "BwbwbwbWB" );
        map.Add( 'L', "bwBwbwbWB" );
        map.Add( 'M', "BwBwbwbWb" );
        map.Add( 'N', "bwbwBwbWB" );
        map.Add( 'O', "BwbwBwbWb" );
        map.Add( 'P', "bwBwBwbWb" );
        map.Add( 'Q', "bwbwbwBWB" );
        map.Add( 'R', "BwbwbwBWb" );
        map.Add( 'S', "bwBwbwBWb" );
        map.Add( 'T', "bwbwBwBWb" );
        map.Add( 'U', "BWbwbwbwB" );
        map.Add( 'V', "bWBwbwbwB" );
        map.Add( 'W', "BWBwbwbwb" );
        map.Add( 'X', "bWbwBwbwB" );
        map.Add( 'Y', "BWbwBwbwb" );
        map.Add( 'Z', "bWBwBwbwb" );

    }


    /**
     *  Sets the position where this barcode will be drawn on the page.
     *
     *  @param x1 the x coordinate of the top left corner of the barcode.
     *  @param y1 the y coordinate of the top left corner of the barcode.
     */
    public void SetPosition(double x1, double y1) {
        SetPosition((float) x1, (float) y1);
    }


    /**
     *  Sets the position where this barcode will be drawn on the page.
     *
     *  @param x1 the x coordinate of the top left corner of the barcode.
     *  @param y1 the y coordinate of the top left corner of the barcode.
     */
    public void SetPosition(float x1, float y1) {
        SetLocation(x1, y1);
    }


    /**
     *  Sets the location where this barcode will be drawn on the page.
     *
     *  @param x1 the x coordinate of the top left corner of the barcode.
     *  @param y1 the y coordinate of the top left corner of the barcode.
     */
    public void SetLocation(float x1, float y1) {
        this.x1 = x1;
        this.y1 = y1;
    }


    /**
     *  Sets the module length of this barcode.
     *  The default value is 0.75
     *
     *  @param moduleLength the specified module length.
     */
    public void SetModuleLength(double moduleLength) {
        this.m1 = (float) moduleLength;
    }


    /**
     *  Sets the module length of this barcode.
     *  The default value is 0.75f
     *
     *  @param moduleLength the specified module length.
     */
    public void SetModuleLength(float moduleLength) {
        this.m1 = moduleLength;
    }


    /**
     *  Sets the bar height factor.
     *  The height of the bars is the moduleLength * barHeightFactor
     *  The default value is 50.0
     *
     *  @param barHeightFactor the specified bar height factor.
     */
    public void SetBarHeightFactor(double barHeightFactor) {
        this.barHeightFactor = (float) barHeightFactor;
    }


    /**
     *  Sets the bar height factor.
     *  The height of the bars is the moduleLength * barHeightFactor
     *  The default value is 50.0
     *
     *  @param barHeightFactor the specified bar height factor.
     */
    public void SetBarHeightFactor(float barHeightFactor) {
        this.barHeightFactor = barHeightFactor;
    }


    /**
     *  Sets the drawing direction for this font.
     *
     *  @param direction the specified direction.
     */
    public void SetDirection(int direction) {
        this.direction = direction;
    }


    /**
     *  Sets the font to be used with this barcode.
     *
     *  @param font the specified font.
     */
    public void SetFont(Font font) {
        this.font = font;
    }


    /**
     *  Draws this barcode on the specified page.
     *
     *  @param page the scecified page.
     */
    public void DrawOn(Page page) {
        if (type == BarCode.UPC) {
            drawCodeUPC(page);
        }
        else if (type == BarCode.CODE128) {
            drawCode128(page);
        }
        else if (type == BarCode.CODE39) {
            drawCode39(page);
        }
    }


    private void drawCodeUPC(Page page) {
        float x = x1;
        float y = y1;
        float h = m1 * barHeightFactor; // Barcode height when drawn horizontally

        // Calculate the check digit:
        // 1. Add the digits in the odd-numbered positions (first, third, fifth, etc.)
        // together and multiply by three.
        // 2. Add the digits in the even-numbered positions (second, fourth, sixth, etc.)
        // to the result.
        // 3. Subtract the result modulo 10 from ten.
        // 4. The answer modulo 10 is the check digit.
        int sum = 0;
        for (int i = 0; i < 11; i += 2) {
            sum += str[i] - 48;
        }
        sum *= 3;
        for (int i = 1; i < 11; i += 2) {
            sum += str[i] - 48;
        }
        int reminder = sum % 10;
        int check_digit = (10 - reminder) % 10;
        str += check_digit.ToString();

        x = drawEGuard(page, x, y, m1, h + 8);
        for (int i = 0; i < 6; i++) {
            int digit = str[i] - 0x30;
            // page.DrawString(digit.ToString(), x + 1, y + h + 12);
            String symbol = tableA[digit].ToString();
            int n;
            for (int j = 0; j < symbol.Length; j++) {
                n = symbol[j] - 0x30;
                if (j%2 != 0) {
                    drawVertBar(page, x, y, n*m1, h);
                }
                x += n*m1;
            }
        }
        x = drawMGuard(page, x, y, m1, h + 8);
        for (int i = 6; i < 12; i++) {
            int digit = str[i] - 0x30;
            // page.DrawString(digit.ToString(), x + 1, y + h + 12);
            String symbol = tableA[digit].ToString();
            int n;
            for (int j = 0; j < symbol.Length; j++) {
                n = symbol[j] - 0x30;
                if (j%2 == 0) {
                    drawVertBar(page, x, y, n*m1, h);
                }
                x += n*m1;
            }
        }
        x = drawEGuard(page, x, y, m1, h + 8);
    
        if (font != null) {
            String label =
            		str[0] +
            		"  " +
            		str[1] +
            		str[2] +
            		str[3] +
            		str[4] +
            		str[5] +
            		"   " +
            		str[6] +
            		str[7] +
            		str[8] +
            		str[9] +
            		str[10] +
            		"  " +
            		str[11];
            float fontSize = font.GetSize();
            font.SetSize(10.0);
            page.DrawString(
                    font,
                    label,
                    x1 + ((x - x1) - font.StringWidth(label))/2,
                    y1 + h + font.body_height);
            font.SetSize(fontSize);
        }
    }


    private float drawEGuard(
            Page page,
            float x,
            float y,
            float m1,
            float h) {
        // 101
        drawBar(page, x + (0.5f * m1), y, m1, h);
        drawBar(page, x + (2.5f * m1), y, m1, h);
        return (x + (3.0f * m1));
    }


    private float drawMGuard(
            Page page,
            float x,
            float y,
            float m1,
            float h) {
        // 01010
        drawBar(page, x + (1.5f * m1), y, m1, h);
        drawBar(page, x + (3.5f * m1), y, m1, h);
        return (x + (5.0f * m1));
    }


    private void drawBar(
            Page page,
            float x,
            float y,
            float m1,   // Single bar width
            float h) {
        page.SetPenWidth(m1);
        page.MoveTo(x, y);
        page.LineTo(x, y + h);
        page.StrokePath();
    }


    private void drawCode128(Page page) {
        float x = x1;
        float y = y1;
        float w = m1;
        float h = m1;

        if (direction == TOP_TO_BOTTOM) {
            w *= barHeightFactor;
        }
        else if (direction == LEFT_TO_RIGHT) {
            h *= barHeightFactor;
        }

        List<Int32> list = new List<Int32>();
        for (int i = 0; i < str.Length; i++) {
            char symchar = str[i];
            if (symchar < 32) {
                list.Add(GS1_128.SHIFT);
                list.Add(symchar + 0x40);   // Decimal 64
            }
            else {
                list.Add(symchar - 0x20);   // Decimal 32
            }
            if (list.Count == 48) break;   // Maximum number of data characters is 48
        }

        StringBuilder buf = new StringBuilder();
        int check_digit = GS1_128.START_B;
        buf.Append((char) check_digit);
        for (int i = 0; i < list.Count; i++) {
            int codeword = list[i];
            buf.Append((char) codeword);
            check_digit +=  codeword * (i + 1);
        }
        check_digit %= GS1_128.START_A;
        buf.Append((char) check_digit);
        buf.Append((char) GS1_128.STOP);

        for (int i = 0; i < buf.Length; i++) {
            int si = buf[i];
            String symbol = GS1_128.TABLE[si].ToString();
            for (int j = 0; j < symbol.Length; j++) {
                int n = symbol[j] - 0x30;
                if (j%2 == 0) {
                    if (direction == LEFT_TO_RIGHT) {
                        drawVertBar(page, x, y, m1 * n, h);
                    } 
                    else if (direction == TOP_TO_BOTTOM) {
                        drawHorzBar(page, x, y, m1 * n, w);
                    }
                }
                if (direction == LEFT_TO_RIGHT) {
                    x += n * m1;
                }
                else if (direction == TOP_TO_BOTTOM) {
                    y += n * m1;
                }
            }
        }

        if (font != null) {
            if (direction == LEFT_TO_RIGHT) {
                page.DrawString(
                        font,
                        str,
                        x1 + ((x - x1) - font.StringWidth(str))/2,
                        y1 + h + font.body_height);
            } else if (direction == TOP_TO_BOTTOM) {
                page.SetTextDirection(90);
                page.DrawString(
                        font,
                        str,
                        x + w + font.body_height,
                        y - ((y - y1) - font.StringWidth(str))/2);
                page.SetTextDirection(0);
            }
        }
    }
    
    
    private void drawCode39(Page page) {
        str = "*" + str + "*";
        
        float x = x1;
        float y = y1;
        float w = m1 * barHeightFactor; // Barcode width when drawn vertically
        float h = m1 * barHeightFactor; // Barcode height when drawn horizontally

        if (direction == LEFT_TO_RIGHT) {

            for (int i = 0; i < str.Length; i++) {
                String code = map[str[i]];

                if ( code == null ) {
                    throw new Exception("The input string '" + str +
                            "' contains characters that are invalid in a Code39 barcode.");
                }

                for (int j = 0; j < 9; j++) {
                    char ch = code[j];
                    if (ch == 'w') {
                        x += m1;                    
                    }
                    else if (ch == 'W') {
                        x += m1 * 3;
                    }
                    else if (ch == 'b') {
                        drawVertBar(page, x, y, m1, h);
                        x += m1;
                    }
                    else if (ch == 'B') {
                        drawVertBar(page, x, y, m1 * 3, h);
                        x += m1 * 3;
                    }
                }
    
                x += m1;
            }

            if (font != null) {
                page.DrawString(
                        font,
                        str,
                        x1 + ((x - x1) - font.StringWidth(str))/2,
                        y1 + h + font.body_height);
            }

        }
        else if (direction == TOP_TO_BOTTOM) {

            for (int i = 0; i < str.Length; i++) {
                String code = map[str[i]];
    
                if ( code == null ) {
                    throw new Exception("The input string '" + str +
                            "' contains characters that are invalid in a Code39 barcode.");
                }

                for (int j = 0; j < 9; j++) {
                    char ch = code[j];
                    if (ch == 'w') {
                        y += m1;                    
                    }
                    else if (ch == 'W') {
                        y += 3 * m1;
                    }
                    else if (ch == 'b') {
                        drawHorzBar(page, x, y, m1, h);
                        y += m1;
                    }
                    else if (ch == 'B') {
                        drawHorzBar(page, x, y, 3 * m1, h);
                        y += 3 * m1;
                    }
                }
    
                y += m1;
            }
        
            if (font != null) {
                page.SetTextDirection(270);
                page.DrawString(
                        font,
                        str,
                        x - font.body_height,
                        y1 + ((y - y1) - font.StringWidth(str))/2);
                page.SetTextDirection(0);
            }

        }
        else if (direction == BOTTOM_TO_TOP) {

            float height = 0.0f;

            for (int i = 0; i < str.Length; i++) {
                String code = map[str[i]];
    
                if ( code == null ) {
                    throw new Exception("The input string '" + str +
                            "' contains characters that are invalid in a Code39 barcode.");
                }

                for (int j = 0; j < 9; j++) {
                    char ch = code[j];
                    if (ch == 'w' || ch == 'b') {
                        height += m1;                   
                    }
                    else if (ch == 'W' || ch == 'B') {
                        height += 3 * m1;
                    }
                }
    
                height += m1;
            }

            y += height - m1;

            for (int i = 0; i < str.Length; i++) {
                String code = map[str[i]];
    
                for (int j = 0; j < 9; j++) {
                    char ch = code[j];
                    if (ch == 'w') {
                        y -= m1;                    
                    }
                    else if (ch == 'W') {
                        y -= 3 * m1;
                    }
                    else if (ch == 'b') {
                        drawHorzBar2(page, x, y, m1, h);
                        y -= m1;
                    }
                    else if (ch == 'B') {
                        drawHorzBar2(page, x, y, 3 * m1, h);
                        y -= 3 * m1;
                    }
                }
    
                y -= m1;
            }
        
            if (font != null) {
                y = y1 + ( height - m1);
    
                page.SetTextDirection(90);
                page.DrawString(
                        font,
                        str,
                        x + w + font.body_height,
                        y - ((y - y1) - font.StringWidth(str))/2);
                page.SetTextDirection(0);
            }
        }
    }


    private void drawVertBar(
            Page page,
            float x,
            float y,
            float m1,   // Module length
            float h) {
        page.SetPenWidth(m1);
        page.MoveTo(x + m1/2, y);
        page.LineTo(x + m1/2, y + h);
        page.StrokePath();
    }


    private void drawHorzBar(
            Page page,
            float x,
            float y,
            float m1,   // Module length
            float w) {
        page.SetPenWidth(m1);
        page.MoveTo(x, y + m1/2);
        page.LineTo(x + w, y + m1/2);
        page.StrokePath();
    }


    private void drawHorzBar2(
            Page page,
            float x,
            float y,
            float m1,   // Module length
            float w) {
        page.SetPenWidth(m1);
        page.MoveTo(x, y - m1/2);
        page.LineTo(x + w, y - m1/2);
        page.StrokePath();
    }


}   // End of BarCode.cs
}   // End of namespace PDFjet.NET

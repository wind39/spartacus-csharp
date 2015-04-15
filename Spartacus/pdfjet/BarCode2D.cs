/**
 *  BarCode2D.cs
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
 *  Used to create PDF417 2D barcodes.
 *
 *  Please see Example_12.
 */
public class BarCode2D : IDrawable {

    private const int ALPHA = 0x08;
    private const int LOWER = 0x04;
    private const int MIXED = 0x02;
    private const int PUNCT = 0x01;

    private const int LATCH_TO_LOWER = 27;
    private const int SHIFT_TO_ALPHA = 27;
    private const int LATCH_TO_MIXED = 28;
    private const int LATCH_TO_ALPHA = 28;
    private const int SHIFT_TO_PUNCT = 29;

    private float x1 = 0.0f;
    private float y1 = 0.0f;

    // Critical defaults!
    private float w1 = 0.75f;
    private float h1 = 0.0f;

    private int rows = 50;
    private int cols = 18;

    private int[] codewords = null;

    private String str = null;


    /**
     *  Constructor for 2D barcodes.
     *
     *  @param str the specified string.
     */
    public BarCode2D(String str) {
        this.str = str;
        this.h1 = 3 * w1;
        this.codewords = new int[rows * (cols + 2)];

        int[] lf_buffer = new int[rows];
        int[] lr_buffer = new int[rows];
        int[] buffer = new int[rows * cols];

        // Left and right row indicators - see page 34 of the ISO specification
        int compression = 5;    // Compression Level
        int k = 1;
        for (int i = 0; i < rows; i++) {
            int lf = 0;
            int lr = 0;
            int cf = 30 * (i/3);
            if (k == 1) {
                lf = cf + (rows - 1) / 3;
                lr = cf + (cols - 1);
            } else if (k == 2) {
                lf = cf + 3*compression + (rows - 1) % 3;
                lr = cf + (rows - 1) / 3;
            } else if (k == 3) {
                lf = cf + (cols - 1);
                lr = cf + 3*compression + (rows - 1) % 3;
            }
            lf_buffer[i] = lf;
            lr_buffer[i] = lr;
            k++;
            if (k == 4) k = 1;
        }

        int data_len = (rows * cols) - ECC_L5.table.Length;
        for (int i = 0; i < data_len; i++) {
            buffer[i] = 900;    // The default pad codeword
        }
        buffer[0] = data_len;

        addData(buffer, data_len);
        addECC(buffer);

        for (int i = 0; i < rows; i++) {
            int index = (cols + 2) * i;
            codewords[index] = lf_buffer[i];
            for (int j = 0; j < cols; j++) {
                codewords[index + j + 1] = buffer[cols*i + j];
            }
            codewords[index + cols + 1] = lr_buffer[i];
        }
    }


    /**
     *  Sets the position of this barcode on the page.
     *
     *  @param x the x coordinate of the top left corner of the barcode.
     *  @param y the y coordinate of the top left corner of the barcode.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Sets the position of this barcode on the page.
     *
     *  @param x the x coordinate of the top left corner of the barcode.
     *  @param y the y coordinate of the top left corner of the barcode.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Sets the location of this barcode on the page.
     *
     *  @param x the x coordinate of the top left corner of the barcode.
     *  @param y the y coordinate of the top left corner of the barcode.
     */
    public void SetLocation(float x, float y) {
        this.x1 = x;
        this.y1 = y;
    }


    /**
     *  Draws this barcode on the specified page.
     *
     *  @param page the page to draw this barcode on.
     */
    public void DrawOn(Page page) {
        DrawPdf417(page);
    }


    private List<Int32> convertTheStringToListOfValues() {
        List<Int32> list = new List<Int32>();

        int value = 0;
        int current_mode = ALPHA;
        int mode = 0;
        int ch = 0;
        for (int i = 0; i < str.Length; i++) {
            ch = str[i];
            if (ch == 0x20) {
                list.Add(26);   // The codeword for space
                continue;
            }

            value = TextCompact.TABLE[ch,1];
            mode = TextCompact.TABLE[ch,2];
            if (mode == current_mode) {
                list.Add(value);
            }
            else {
                if (mode == ALPHA && current_mode == LOWER) {
                    list.Add(SHIFT_TO_ALPHA);
                    list.Add(value);
                }
                else if (mode == ALPHA && current_mode == MIXED) {
                    list.Add(LATCH_TO_ALPHA);
                    list.Add(value);
                    current_mode = mode;
                }
                else if (mode == LOWER && current_mode == ALPHA) {
                    list.Add(LATCH_TO_LOWER);
                    list.Add(value);
                    current_mode = mode;
                }
                else if (mode == LOWER && current_mode == MIXED) {
                    list.Add(LATCH_TO_LOWER);
                    list.Add(value);
                    current_mode = mode;
                }
                else if (mode == MIXED && current_mode == ALPHA) {
                    list.Add(LATCH_TO_MIXED);
                    list.Add(value);
                    current_mode = mode;
                }
                else if (mode == MIXED && current_mode == LOWER) {
                    list.Add(LATCH_TO_MIXED);
                    list.Add(value);
                    current_mode = mode;
                }
                else if (mode == PUNCT && current_mode == ALPHA) {
                    list.Add(SHIFT_TO_PUNCT);
                    list.Add(value);
                }
                else if (mode == PUNCT && current_mode == LOWER) {
                    list.Add(SHIFT_TO_PUNCT);
                    list.Add(value);
                }   
                else if (mode == PUNCT && current_mode == MIXED) {
                    list.Add(SHIFT_TO_PUNCT);
                    list.Add(value);
                }
            }
        }

        return list;
    }


    private void addData(int[] buffer, int data_len) {
        List<Int32> list = convertTheStringToListOfValues();
        int bi = 1; // buffer index = 1 to skip the Symbol Length Descriptor
        int hi = 0;
        int lo = 0;
        for (int i = 0; i < list.Count; i += 2) {
            hi = list[i];
            if (i + 1 == list.Count) {
                lo = SHIFT_TO_PUNCT;    // Pad
            }
            else {
                lo = list[i + 1];
            }

            bi++;
            if (bi == data_len) break;
            buffer[bi] = 30*hi + lo;
        }
    }


    private void addECC(int[] buf) {
        int[] ecc = new int[ECC_L5.table.Length];
        int t1 = 0;
        int t2 = 0;
        int t3 = 0;

        int data_len = buf.Length - ecc.Length;
        for (int i = 0; i < data_len; i++) {
            t1 = (buf[i] + ecc[ecc.Length - 1]) % 929;
            for (int j = ecc.Length - 1; j > 0; j--) {
                t2 = (t1 * ECC_L5.table[j]) % 929;
                t3 = 929 - t2;
                ecc[j] = (ecc[j - 1] + t3) % 929;
            }
            t2 = (t1 * ECC_L5.table[0]) % 929;
            t3 = 929 - t2;
            ecc[0] = t3 % 929;
        }

        for (int i = 0; i < ecc.Length; i++) {
            if (ecc[i] != 0) {
                buf[(buf.Length - 1) - i] = 929 - ecc[i];
            }
        }
    }


    private void DrawPdf417(Page page) {
        float x = x1;
        float y = y1;

        int[] start_symbol = {8, 1, 1, 1, 1, 1, 1, 3};
        for (int i = 0; i < start_symbol.Length; i++) {
            int n = start_symbol[i];
            if (i%2 == 0) {
                drawBar(page, x, y, n * w1, rows * h1);
            }
            x += n * w1;
        }
        x1 = x;

        int k = 1;  // Cluster index
        for (int i = 0; i < codewords.Length; i++) {
            int row = codewords[i];
            String symbol = PDF417.TABLE[row,k].ToString();
            for (int j = 0; j < 8; j++) {
                int n = symbol[j] - 0x30;
                if (j%2 == 0) {
                    drawBar(page, x, y, n * w1, h1);
                }
                x += n * w1;
            }
            if (i == codewords.Length - 1) break;
            if ((i + 1) % (cols + 2) == 0) {
                x = x1;
                y += h1;
                k++;
                if (k == 4) k = 1;
            }
        }

        y = y1;
        int[] end_symbol =   {7, 1, 1, 3, 1, 1, 1, 2, 1};
        for (int i = 0; i < end_symbol.Length; i++) {
            int n = end_symbol[i];
            if (i%2 == 0) {
                drawBar(page, x, y, n * w1, rows * h1);
            }
            x += n * w1;
        }
    }


    private void drawBar(
            Page page,
            float x,
            float y,
            float w,    // Bar width
            float h) {
        page.SetPenWidth(w);
        page.MoveTo(x + w/2, y);
        page.LineTo(x + w/2, y + h);
        page.StrokePath();
    }

}   // End of BarCode2D.cs
}   // End of namespace PDFjet.NET

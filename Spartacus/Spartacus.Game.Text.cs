/*
The MIT License (MIT)

Copyright (c) 2014-2017 William Ivanski

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

namespace Spartacus.Game
{
    public class Text
    {
        public string v_message;

        public int v_x;

        public int v_y;

        private System.Drawing.Font v_font;

        private System.Drawing.SolidBrush v_brush;


        public Text(int p_x, int p_y, string p_font, int p_size, int p_a, int p_r, int p_g, int p_b)
        {
            this.v_message = "";
            this.v_x = p_x;
            this.v_y = p_y;
            this.v_font = new System.Drawing.Font(p_font, p_size);
            this.v_brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(p_a, p_r, p_g, p_b));
        }

        public void SetMessage(string p_message)
        {
            this.v_message = p_message;
        }

        public void SetPosition(int p_x, int p_y)
        {
            this.v_x = p_x;
            this.v_y = p_y;
        }

        public void Render(System.Drawing.Graphics p_graphics)
        {
            p_graphics.DrawString(this.v_message, this.v_font, this.v_brush, this.v_x, this.v_y);
        }
    }
}

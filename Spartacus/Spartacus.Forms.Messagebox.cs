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

namespace Spartacus.Forms
{
    public class Messagebox
    {
        public enum Icon
        {
            ASTERISK,
            ERROR,
            EXCLAMATION,
            HAND,
            INFORMATION,
            NONE,
            QUESTION,
            STOP,
            WARNING
        }

        public Messagebox()
        {
        }

        public static void Show(string p_text, string p_caption, Spartacus.Forms.Messagebox.Icon p_icon)
        {
            switch (p_icon)
            {
                case Spartacus.Forms.Messagebox.Icon.ASTERISK:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Asterisk);
                    break;
                case Spartacus.Forms.Messagebox.Icon.ERROR:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    break;
                case Spartacus.Forms.Messagebox.Icon.EXCLAMATION:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    break;
                case Spartacus.Forms.Messagebox.Icon.HAND:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Hand);
                    break;
                case Spartacus.Forms.Messagebox.Icon.INFORMATION:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    break;
                case Spartacus.Forms.Messagebox.Icon.NONE:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.None);
                    break;
                case Spartacus.Forms.Messagebox.Icon.QUESTION:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Question);
                    break;
                case Spartacus.Forms.Messagebox.Icon.STOP:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                    break;
                case Spartacus.Forms.Messagebox.Icon.WARNING:
                    System.Windows.Forms.MessageBox.Show(p_text, p_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    break;
                default:
                    break;
            }
        }
    }
}

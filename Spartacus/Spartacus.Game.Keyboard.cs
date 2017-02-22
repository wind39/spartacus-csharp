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
	public class Keyboard
    {
        private System.Collections.Generic.List<Spartacus.Game.Keys> v_keysdown;

        private readonly object v_keysdown_lock;

        private System.Windows.Forms.Timer v_timer;

        public delegate void KeyEvent(Spartacus.Game.Keys p_key);

        public event KeyEvent KeyDown;
        public event KeyEvent KeyUp;
        public event KeyEvent KeyPress;


        public Keyboard(Spartacus.Forms.Window p_window)
        {
            this.v_keysdown = new System.Collections.Generic.List<Spartacus.Game.Keys>();
            this.v_keysdown_lock = new object();

            this.v_timer = new System.Windows.Forms.Timer();
            this.v_timer.Tick += new System.EventHandler(this.OnTimer);

            ((System.Windows.Forms.Form) p_window.v_control).KeyPreview = true;
            ((System.Windows.Forms.Form) p_window.v_control).PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
            ((System.Windows.Forms.Form) p_window.v_control).KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            ((System.Windows.Forms.Form) p_window.v_control).KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            ((System.Windows.Forms.Form) p_window.v_control).KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPressed);
        }

        public Keyboard(System.Windows.Forms.Form p_screen)
        {
            this.v_keysdown = new System.Collections.Generic.List<Spartacus.Game.Keys>();
            this.v_keysdown_lock = new object();

            this.v_timer = new System.Windows.Forms.Timer();
            this.v_timer.Tick += new System.EventHandler(this.OnTimer);

            p_screen.KeyPreview = true;
            p_screen.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
            p_screen.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            p_screen.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            p_screen.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPressed);
        }

        public void Start(int p_framerate)
        {
            this.v_timer.Interval = 1000 / p_framerate;
            this.v_timer.Start();
        }

        private void OnPreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
			if (!this.v_keysdown.Contains((Spartacus.Game.Keys)e.KeyCode))
            {
                lock (this.v_keysdown_lock)
                {
					this.v_keysdown.Add((Spartacus.Game.Keys)e.KeyCode);
                }
            }
        }

        private void OnTimer(object sender, System.EventArgs e)
        {
            foreach (Spartacus.Game.Keys k in this.v_keysdown)
                this.KeyDown(k);
        }

        private void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
			if (this.v_keysdown.Contains((Spartacus.Game.Keys)e.KeyCode))
            {
                lock (this.v_keysdown_lock)
                {
					this.v_keysdown.Remove((Spartacus.Game.Keys)e.KeyCode);
                }
            }

            e.Handled = true;
        }

        private void OnKeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            this.KeyPress((Spartacus.Game.Keys)char.ToUpper(e.KeyChar));
        }
    }
}
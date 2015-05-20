/*
The MIT License (MIT)

Copyright (c) 2014,2015 William Ivanski

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
    public class Level
    {
        private System.Collections.ArrayList v_layers;

        private System.Windows.Forms.Form v_screen;

        private System.Drawing.BufferedGraphicsContext v_context;

        private System.Drawing.BufferedGraphics v_bufferedgraphics;

        private System.Windows.Forms.Timer v_timer;


        public Level(System.Windows.Forms.Form p_screen)
        {
            this.v_layers = new System.Collections.ArrayList();

            this.v_screen = p_screen;

            this.v_context = System.Drawing.BufferedGraphicsManager.Current;
            this.v_context.MaximumBuffer = new System.Drawing.Size(this.v_screen.Width + 1, this.v_screen.Height + 1);

            this.v_bufferedgraphics = v_context.Allocate(this.v_screen.CreateGraphics(), new System.Drawing.Rectangle(0, 0, this.v_screen.Width, this.v_screen.Height));

            this.v_timer = new System.Windows.Forms.Timer();
            this.v_timer.Enabled = true;
            this.v_timer.Tick += new System.EventHandler(this.OnTimer);
        }

        public void AddLayer(Spartacus.Game.Layer p_layer)
        {
            this.v_layers.Add(p_layer);
        }

        public void Start(int p_framerate)
        {
            this.v_timer.Interval = 1000 / p_framerate;
            this.v_timer.Start();
        }

        private void OnTimer(object sender, System.EventArgs e)
        {
            System.Drawing.Graphics v_graphics = this.v_bufferedgraphics.Graphics;
            v_graphics.Clear(System.Drawing.Color.Black);

            for (int k = 0; k < this.v_layers.Count; k++)
                ((Spartacus.Game.Layer)this.v_layers[k]).Render(v_graphics);

            this.v_bufferedgraphics.Render(System.Drawing.Graphics.FromHwnd(this.v_screen.Handle));
        }
    }
}

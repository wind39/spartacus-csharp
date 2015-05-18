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

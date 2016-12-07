/*
The MIT License (MIT)

Copyright (c) 2014-2016 William Ivanski

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
    public class Object
    {
        public string v_name;

        public System.Drawing.Rectangle v_rectangle;

		public System.Collections.Generic.List<System.Drawing.Image> v_images;

		public System.Drawing.Image v_currentimage;

		public System.Collections.Generic.List<Spartacus.Game.Animation> v_animations;

        public Spartacus.Game.Layer v_layer;

		public System.Drawing.Pen v_border;

		private bool v_ismoving;
		private int v_mov_numframes;
		private int v_mov_curframe;
		private int v_mov_stepx;
		private int v_mov_stepy;
		private int v_mov_prevx;
		private int v_mov_prevy;
		private int v_mov_offsetx;
		private int v_mov_offsety;


		public Object(int p_x, int p_y, int p_width, int p_height)
		{
			this.v_name = null;
			this.v_rectangle = new System.Drawing.Rectangle(p_x, p_y, p_width, p_height);
			this.v_images = new System.Collections.Generic.List<System.Drawing.Image>();
			this.v_currentimage = null;
			this.v_animations = new System.Collections.Generic.List<Spartacus.Game.Animation>();
			this.v_ismoving = false;
			this.v_border = null;
		}

        public Object(string p_name, int p_x, int p_y, int p_width, int p_height)
        {
            this.v_name = p_name;
            this.v_rectangle = new System.Drawing.Rectangle(p_x, p_y, p_width, p_height);
			this.v_images = new System.Collections.Generic.List<System.Drawing.Image>();
            this.v_currentimage = null;
			this.v_animations = new System.Collections.Generic.List<Spartacus.Game.Animation>();
			this.v_ismoving = false;
			this.v_border = null;
        }

        public void AddImage(string p_filename)
        {
            this.v_images.Add(System.Drawing.Image.FromFile(p_filename));
            if (this.v_currentimage == null)
                this.v_currentimage = this.v_images[0];
        }

        public void AddImage(System.Drawing.Image p_image)
        {
            this.v_images.Add(p_image);
            if (this.v_currentimage == null)
                this.v_currentimage = this.v_images[0];
        }

		public void AddImage(string p_filename, bool p_transparent)
		{
			if (p_transparent)
			{
				System.Drawing.Bitmap v_image = new System.Drawing.Bitmap(p_filename);
				System.Drawing.Color v_color;
				for (int i = 0; i < v_image.Width; i++)
				{
					for (int j = 0; j < v_image.Height; j++)
					{
						v_color = v_image.GetPixel(i, j);
						if (v_color.R >= 150 && v_color.G <= 100 && v_color.B >= 150)
							v_image.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0, 0));
					}
				}
				this.v_images.Add(v_image);
				if (this.v_currentimage == null)
					this.v_currentimage = this.v_images[0];
			}
			else
				this.AddImage(p_filename);
		}

        public void AddAnimation(Spartacus.Game.Animation p_animation)
        {
            this.v_animations.Add(p_animation);
        }

        public void SetLayer(Spartacus.Game.Layer p_layer)
        {
            this.v_layer = p_layer;
        }

        public void SetPosition(int p_x, int p_y)
        {
            this.v_rectangle.X = p_x;
            this.v_rectangle.Y = p_y;
        }

        public void Move(int p_offsetx, int p_offsety)
        {
            this.v_rectangle.X += p_offsetx;
            this.v_rectangle.Y += p_offsety;
        }

        public void Move(int p_offsetx, int p_offsety, bool p_collide)
        {
            this.v_rectangle.X += p_offsetx;
            this.v_rectangle.Y += p_offsety;

            if (p_collide)
            {
                int k = 0;
                bool v_achou = false;
                while (k < this.v_layer.v_objects.Count && !v_achou)
                {
                    if (this.v_name != this.v_layer.v_objects[k].v_name &&
                        this.v_rectangle.IntersectsWith(this.v_layer.v_objects[k].v_rectangle))
                        v_achou = true;
                    else
                        k++;
                }
                if (v_achou)
                {
                    this.v_rectangle.X -= p_offsetx;
                    this.v_rectangle.Y -= p_offsety;
                }
            }
        }

		public void Move(int p_offsetx, int p_offsety, int p_numframes)
		{
			this.v_mov_numframes = p_numframes;
			this.v_mov_stepx = p_offsetx / p_numframes;
			this.v_mov_stepy = p_offsety / p_numframes;
			this.v_mov_curframe = 0;
			this.v_mov_prevx = this.v_rectangle.X;
			this.v_mov_prevy = this.v_rectangle.Y;
			this.v_mov_offsetx = p_offsetx;
			this.v_mov_offsety = p_offsety;
			this.v_ismoving = true;
		}

        public void Render(System.Drawing.Graphics p_graphics)
        {
			if (this.v_ismoving)
			{
				this.Move(this.v_mov_stepx, this.v_mov_stepy);
				this.v_mov_curframe++;
				if (this.v_mov_curframe == this.v_mov_numframes)
				{
					this.v_ismoving = false;
					this.SetPosition(this.v_mov_prevx+this.v_mov_offsetx, this.v_mov_prevy+this.v_mov_offsety);
				}
			}

            for (int k = 0; k < this.v_animations.Count; k++)
            {
                if (this.v_animations[k].v_isrunning)
                {
                    this.v_currentimage = this.v_images[this.v_animations[k].v_steps[this.v_animations[k].v_currentstep].v_imageindex];
                    this.v_animations[k].IncFrame();
                    break;
                }
            }

            if (this.v_currentimage != null)
                p_graphics.DrawImage(this.v_currentimage, this.v_rectangle);
			this.DrawBorder(p_graphics);
        }

		public void SetBorder(System.Drawing.Color p_color, int p_width)
		{
			this.v_border = new System.Drawing.Pen(p_color, p_width);
		}

		public void RemoveBorder()
		{
			this.v_border = null;
		}

		private void DrawBorder(System.Drawing.Graphics p_graphics)
		{
			if (this.v_border != null)
			{
				System.Drawing.Rectangle r = new System.Drawing.Rectangle(
					this.v_rectangle.X,
					this.v_rectangle.Y,
					this.v_rectangle.Width,
					this.v_rectangle.Height
				);
				p_graphics.DrawRectangle(this.v_border, r);
			}
		}
    }
}
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


        public Object(string p_name, int p_x, int p_y, int p_width, int p_height)
        {
            this.v_name = p_name;
            this.v_rectangle = new System.Drawing.Rectangle(p_x, p_y, p_width, p_height);
			this.v_images = new System.Collections.Generic.List<System.Drawing.Image>();
            this.v_currentimage = null;
			this.v_animations = new System.Collections.Generic.List<Spartacus.Game.Animation>();
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

        public void Render(System.Drawing.Graphics p_graphics)
        {
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
        }
    }
}
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
    public class Layer
    {
		public System.Collections.Generic.List<Spartacus.Game.Object> v_objects;

        public System.Collections.Generic.List<Spartacus.Game.Text> v_texts;

        public delegate void CollisionEvent(Spartacus.Game.Object p_object1, Spartacus.Game.Object p_object2);

        public event CollisionEvent Collision;

		public delegate void MouseEvent(Spartacus.Game.Object p_object);

		public event MouseEvent MouseClick;


        public Layer()
        {
			this.v_objects = new System.Collections.Generic.List<Spartacus.Game.Object>();
            this.v_texts = new System.Collections.Generic.List<Spartacus.Game.Text>();
        }

        public void AddObject(Spartacus.Game.Object p_object)
        {
            p_object.SetLayer(this);
            this.v_objects.Add(p_object);
        }

        public void AddText(Spartacus.Game.Text p_text)
        {
            this.v_texts.Add(p_text);
        }

		public void Render(System.Drawing.Graphics p_graphics)
        {
            for (int i = 0; i < this.v_objects.Count - 1; i++)
            {
                for (int j = i + 1; j < this.v_objects.Count; j++)
                    if (this.v_objects[i].v_rectangle.IntersectsWith(this.v_objects[j].v_rectangle))
                        this.Collision(this.v_objects[i], this.v_objects[j]);
            }

            for (int k = 0; k < this.v_objects.Count; k++)
                this.v_objects[k].Render(p_graphics);

            for (int k = 0; k < this.v_texts.Count; k++)
                this.v_texts[k].Render(p_graphics);
        }

		public void FireMouseClick(Spartacus.Game.Object p_object)
		{
			this.MouseClick(p_object);
		}
    }
}
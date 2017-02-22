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

namespace Spartacus.Tools.OverLord
{
	public class Tile
	{
		public int v_x;
		public int v_y;
		public int v_size;
		public bool v_canstep;
		public bool v_block;
		public System.Collections.Generic.List<string> v_images;

		public Tile(int p_x, int p_y, int p_size)
		{
			this.v_x = p_x;
			this.v_y = p_y;
			this.v_size = p_size;
			this.v_canstep = true;
			this.v_images = new System.Collections.Generic.List<string>();
		}

		public Tile(int p_x, int p_y, int p_size, bool p_canstep)
		{
			this.v_x = p_x;
			this.v_y = p_y;
			this.v_size = p_size;
			this.v_canstep = p_canstep;
			this.v_images = new System.Collections.Generic.List<string>();
		}

		public Tile(int p_x, int p_y, int p_size, bool p_canstep, string p_image)
		{
			this.v_x = p_x;
			this.v_y = p_y;
			this.v_size = p_size;
			this.v_canstep = p_canstep;
			this.v_images = new System.Collections.Generic.List<string>();
			this.v_images.Add(p_image);
		}

		public System.Drawing.Bitmap GetImage()
		{
			System.Drawing.Bitmap v_tile_image, v_terrain_image, v_object_image;
			System.Drawing.Color v_color;

			v_tile_image = new System.Drawing.Bitmap(this.v_size, this.v_size);

			v_terrain_image = new System.Drawing.Bitmap(this.v_images[0]);
			for (int x = 0; x < this.v_size; x++)
				for (int y = 0; y < this.v_size; y++)
					v_tile_image.SetPixel(x, y, v_terrain_image.GetPixel(x, y));

			if (this.v_images.Count == 2)
			{
				v_object_image = new System.Drawing.Bitmap(this.v_images[1]);
				for (int x = 0; x < this.v_size; x++)
				{
					for (int y = 0; y < this.v_size; y++)
					{
						v_color = v_object_image.GetPixel(x, y);
						if (!(v_color.R >= 150 && v_color.G <= 100 && v_color.B >= 150))
							v_tile_image.SetPixel(x, y, v_color);
					}
				}
			}

			return v_tile_image;
		}
	}
}

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

namespace Spartacus.Tools.OverLord
{
	class Terrain
	{
		public int id {get; set;}
		public string name {get; set;}
		public string image {get; set;}
		public char canstep {get; set;}

		public Terrain()
		{
		}

		public Terrain(int p_id, string p_name, string p_image, char p_canstep)
		{
			this.id = p_id;
			this.name = p_name;
			this.image = p_image;
			this.canstep = p_canstep;
		}
	}

	class Object
	{
		public int id {get; set;}
		public string name {get; set;}
		public string image {get; set;}
		public char block {get; set;}

		public Object()
		{
		}

		public Object(int p_id, string p_name, string p_image, char p_block)
		{
			this.id = p_id;
			this.name = p_name;
			this.image = p_image;
			this.block = p_block;
		}
	}

	public class MapGenerator
	{
		public MapGenerator()
		{
		}

		public Tile[,] Generate(
			Spartacus.Database.Generic p_database,
			int p_map_size,
			int p_mapcell_size,
			int p_tree_chance,
			string p_terrain_filter,
			string p_object_filter
		)
		{
			Tile[,] v_map;
			bool[,] v_skel;
			System.Collections.Generic.List<Terrain> v_all_terrain, v_terrain;
			System.Collections.Generic.List<Object> v_all_object, v_object;
			System.Random v_random;
			int k, v_tmp;
			int v_limit;

			v_random = new System.Random();
			v_limit = v_random.Next(1, p_map_size/2);

			v_skel = new bool[p_map_size, p_map_size];
			v_map = new Tile[p_map_size, p_map_size];

			v_all_terrain = p_database.QueryList<Terrain>(
				"select distinct id, " +
				"       name, " +
				"       'terrain/' || image as image, " +
				"       canstep " +
				"from terrain " +
				"where 1=1 " + p_terrain_filter + " " +
				"order by 1"
			);
			v_all_object = p_database.QueryList<Object>(
				"select distinct id, " +
				"       name, " +
				"       'trees/' || image as image, " +
				"       block " +
				"from objects " +
				"where 1=1 " + p_object_filter + " " +
				"order by 1"
			);

			v_terrain = new System.Collections.Generic.List<Terrain>();
			v_terrain.Add(new Terrain());
			do
			{
				k = v_random.Next(v_all_terrain.Count);
				v_terrain[0].id = v_all_terrain[k].id;
				v_terrain[0].name = v_all_terrain[k].name;
				v_terrain[0].image = v_all_terrain[k].image;
				v_terrain[0].canstep = v_all_terrain[k].canstep;
			}
			while (v_terrain[0].canstep == 'N');
			v_tmp = v_random.Next(1, System.Math.Min(5, v_all_terrain.Count));
			for (int x = 1; x <= v_tmp; x++)
			{
				k = v_random.Next(v_all_terrain.Count);
				v_terrain.Add(new Terrain(v_all_terrain[k].id, v_all_terrain[k].name, v_all_terrain[k].image, v_all_terrain[k].canstep));
			}

			v_object = new System.Collections.Generic.List<Object>();
			v_tmp = v_random.Next(1, System.Math.Min(5, v_all_object.Count));
			for (int x = 0; x <= v_tmp; x++)
			{
				k = v_random.Next(v_all_object.Count);
				v_object.Add(new Object(v_all_object[k].id, v_all_object[k].name, v_all_object[k].image, v_all_object[k].block));
			}

			for (int i = 0; i < p_map_size; i++)
			{
				for (int j = 0; j < p_map_size; j++)
				{
					v_map[i, j] = new Tile(i, j, p_mapcell_size, true, v_terrain[0].image);
					v_skel[i, j] = false;

					if (i < v_limit || i > (p_map_size-v_limit) ||
					    j < v_limit || j > (p_map_size-v_limit))
					{
						if (i == j || (i+j) == (p_map_size-1))
							v_skel[i, j] = true;
					}
					else
					{
						if (i == v_limit || i == (p_map_size-v_limit) ||
						    j == v_limit || j == (p_map_size-v_limit))
							v_skel[i, j] = true;
					}

					if (! v_skel[i, j])
					{
						k = v_random.Next(v_terrain.Count);
						v_map[i, j].v_images[0] = v_terrain[k].image;
						v_map[i, j].v_canstep = v_terrain[k].canstep == 'Y';

						if (v_random.Next(100) > (100-p_tree_chance))
						{
							k = v_random.Next(v_object.Count);
							v_map[i, j].v_images.Add(v_object[k].image);
							v_map[i, j].v_canstep = false;
							v_map[i, j].v_block = true;
						}
					}
				}
			}

			return v_map;
		}
	}
}

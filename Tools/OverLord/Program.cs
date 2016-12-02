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
	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			(new MainClass()).Initialize();
		}

		int v_window_width = 1000;
		int v_window_height = 700;
		int v_map_size = 50;
		int v_tile_size = 96;
		int v_mapview_width = 7;
		int v_mapview_height = 7;
		int v_mapview_x = 0;
		int v_mapview_y = 0;
		int v_tree_chance = 20;

		Spartacus.Database.Generic v_database;
		Tile[,] v_tileset;
		Spartacus.Forms.Window v_window;
		Spartacus.Game.Layer v_mapview;
		Spartacus.Game.Layer v_soldiersview;
		Spartacus.Game.Level v_level;
		Spartacus.Game.Keyboard v_keyboard;

		public void Initialize()
		{
			this.v_window = new Spartacus.Forms.Window("Spartacus OverLord", this.v_window_width, this.v_window_height);

			this.v_database = new Spartacus.Database.Sqlite("overlord.db");

			MapGenerator v_generator = new MapGenerator();
			this.v_tileset = v_generator.Generate(
				this.v_database,
				this.v_map_size,
				this.v_tile_size,
				this.v_tree_chance,
				"and name like 'jungle%'",
				"and name not like 'autumn%'"
			);

			this.v_mapview = new Spartacus.Game.Layer();
			this.BuildMapView();

			// teste
			this.v_soldiersview = new Spartacus.Game.Layer();
			Spartacus.Game.Object v_soldier = new Spartacus.Game.Object("G000", 96, 96, 96, 96);
			v_soldier.AddImage("soldiers/green/stopped0000.png", true);
			v_soldier.AddImage("soldiers/green/walking_s0000.png", true);
			v_soldier.AddImage("soldiers/green/walking_s0001.png", true);
			v_soldier.AddImage("soldiers/green/walking_s0002.png", true);
			v_soldier.AddImage("soldiers/green/walking_s0003.png", true);
			v_soldier.AddImage("soldiers/green/walking_s0004.png", true);
			v_soldier.AddImage("soldiers/green/walking_s0005.png", true);
			v_soldier.AddImage("soldiers/green/walking_s0006.png", true);
			v_soldier.AddImage("soldiers/green/walking_s0007.png", true);
			Spartacus.Game.Animation v_animation = new Spartacus.Game.Animation("S_Walking", false);
			v_animation.AddStep(1, 2);
			v_animation.AddStep(2, 4);
			v_animation.AddStep(3, 6);
			v_animation.AddStep(4, 8);
			v_animation.AddStep(5, 9);
			v_animation.AddStep(6, 10);
			v_animation.AddStep(7, 12);
			v_animation.AddStep(8, 14);
			v_soldier.AddAnimation(v_animation);
			this.v_soldiersview.AddObject(v_soldier);
			// fim teste

			this.v_keyboard = new Spartacus.Game.Keyboard(this.v_window);
			this.v_keyboard.KeyDown += this.OnKeyDown;
			this.v_keyboard.KeyPress += this.OnKeyPress;
			this.v_keyboard.Start(15);

			this.v_level = new Spartacus.Game.Level(this.v_window);
			this.v_level.AddLayer(this.v_mapview);
			this.v_level.AddLayer(this.v_soldiersview);
			v_level.Time += this.OnTime;
			this.v_level.Start(15);

			this.v_window.Run();
		}

		private void BuildMapView()
		{
			Spartacus.Game.Object v_object;

			this.v_mapview.v_objects.Clear();

			for (int x = 0; x < this.v_mapview_width; x++)
			{
				for (int y = 0; y < this.v_mapview_height; y++)
				{
					v_object = new Spartacus.Game.Object(this.v_tile_size*x, this.v_tile_size*y, this.v_tile_size, this.v_tile_size);
					v_object.AddImage(this.v_tileset[this.v_mapview_x+x, this.v_mapview_y+y].GetImage());
					this.v_mapview.AddObject(v_object);
				}
			}
		}

		private void OnKeyDown(Spartacus.Game.Keys p_key)
		{
			switch(p_key)
			{
				case Spartacus.Game.Keys.Up:
					if (this.v_mapview_y > 0)
					{
						this.v_mapview_y--;
						this.BuildMapView();
					}
					break;
				case Spartacus.Game.Keys.Down:
					if (this.v_mapview_y < (this.v_map_size-this.v_mapview_height))
					{
						this.v_mapview_y++;
						this.BuildMapView();
					}
					break;
				case Spartacus.Game.Keys.Left:
					if (this.v_mapview_x > 0)
					{
						this.v_mapview_x--;
						this.BuildMapView();
					}
					break;
				case Spartacus.Game.Keys.Right:
					if (this.v_mapview_x < (this.v_map_size-this.v_mapview_width))
					{
						this.v_mapview_x++;
						this.BuildMapView();
					}
					break;
				case Spartacus.Game.Keys.Space:
					this.v_soldiersview.v_objects[0].v_animations[0].Start();
					break;
				default:
					break;
			}
		}

		private void OnKeyPress(Spartacus.Game.Keys p_key)
		{
			switch(p_key)
			{
				case Spartacus.Game.Keys.Space:
					this.v_soldiersview.v_objects[0].v_animations[0].Start();
					this.v_soldiersview.v_objects[0].Move(0, 96, 16);
					break;
				default:
				break;
			}
		}

		private void OnTime()
		{
		}
	}
}

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
		Spartacus.Forms.Window v_window;
		Spartacus.Game.Layer v_mapview;
		Spartacus.Game.Layer v_soldiersview;
		Spartacus.Game.Layer v_guiview;
		Spartacus.Game.Level v_level;
		Spartacus.Game.Keyboard v_keyboard;

		string v_player;
		System.Drawing.Color v_playercolor;
		Tile[,] v_tileset;
		Soldier[] v_soldiers;
		int v_selected;


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

			this.v_player = "green";
			this.v_playercolor = System.Drawing.Color.Green;

			this.v_soldiers = new Soldier[1];
			this.v_soldiers[0] = new Soldier(
				this.v_database,
				TeamColor.GREEN,
				"green_1",
				2, 3
			);
			this.v_selected = -1;

			this.v_soldiersview = new Spartacus.Game.Layer();
			this.v_soldiersview.MouseClick += this.OnSoldierMouseClick;
			this.v_soldiersview.Collision += this.OnCollision;

			this.v_mapview = new Spartacus.Game.Layer();
			this.v_mapview.MouseClick += this.OnMapMouseClick;
			this.v_mapview.Collision += this.OnCollision;
			this.BuildMapView();

			this.v_guiview = new Spartacus.Game.Layer();
			this.v_guiview.MouseClick += this.OnGuiMouseClick;
			this.v_guiview.Collision += this.OnCollision;
			this.BuidGuiView();

			this.v_keyboard = new Spartacus.Game.Keyboard(this.v_window);
			this.v_keyboard.KeyDown += this.OnKeyDown;
			this.v_keyboard.Start(15);

			this.v_level = new Spartacus.Game.Level(this.v_window);
			this.v_level.AddLayer(this.v_mapview);
			this.v_level.AddLayer(this.v_soldiersview);
			this.v_level.AddLayer(this.v_guiview);
			v_level.Time += this.OnTime;
			this.v_level.Start(15);

			this.v_window.Run();
		}

		private void BuildMapView()
		{
			Spartacus.Game.Object v_object;

			this.v_mapview.v_objects.Clear();
			this.v_soldiersview.v_objects.Clear();

			for (int x = 0; x < this.v_mapview_width; x++)
			{
				for (int y = 0; y < this.v_mapview_height; y++)
				{
					v_object = new Spartacus.Game.Object(this.v_tile_size*x, this.v_tile_size*y, this.v_tile_size, this.v_tile_size);
					v_object.AddImage(this.v_tileset[this.v_mapview_x+x, this.v_mapview_y+y].GetImage());
					this.v_mapview.AddObject(v_object);

					for (int w = 0; w < this.v_soldiers.Length; w++)
					{
						if (this.v_soldiers[w].v_mapx == (this.v_mapview_x+x) &&
						    this.v_soldiers[w].v_mapy == (this.v_mapview_y+y))
						{
							this.v_soldiers[w].v_object.SetPosition(this.v_tile_size*x, this.v_tile_size*y);
							this.v_soldiersview.AddObject(this.v_soldiers[w].v_object);
						}
					}
				}
			}
		}

		private void BuidGuiView()
		{
			Spartacus.Game.Object v_object;
			Spartacus.Game.Text v_text;

			// 0 - Soldier Name
			v_text = new Spartacus.Game.Text(700, 100, "Courier New", 16, 255, 255, 255, 255);
			v_text.SetMessage("Soldier 1");
			this.v_guiview.AddText(v_text);

			// 1 - Soldier Health
			v_text = new Spartacus.Game.Text(880, 155, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("100/100");
			this.v_guiview.AddText(v_text);

			// 2 - Soldier Actions
			v_text = new Spartacus.Game.Text(920, 205, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("3/3");
			this.v_guiview.AddText(v_text);

			// 3 - Soldier Stamina
			v_text = new Spartacus.Game.Text(880, 355, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("100/100");
			this.v_guiview.AddText(v_text);

			// 4 - Soldier Ammo
			v_text = new Spartacus.Game.Text(900, 405, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("20/20");
			this.v_guiview.AddText(v_text);

			// 5 - Soldier Grenades
			v_text = new Spartacus.Game.Text(920, 455, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("2/2");
			this.v_guiview.AddText(v_text);

			v_object = new Spartacus.Game.Object("health", 700, 150, 32, 32);
			v_object.AddImage("gui/health.png");
			this.v_guiview.AddObject(v_object);

			v_text = new Spartacus.Game.Text(750, 155, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("Health");
			this.v_guiview.AddText(v_text);

			v_object = new Spartacus.Game.Object("actions", 700, 200, 32, 32);
			v_object.AddImage("gui/actions.png");
			this.v_guiview.AddObject(v_object);

			v_text = new Spartacus.Game.Text(750, 205, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("Actions");
			this.v_guiview.AddText(v_text);

			v_object = new Spartacus.Game.Object("stop", 700, 250, 32, 32);
			v_object.AddImage("gui/stop.png");
			v_object.AddImage("gui/stop_" + this.v_player + ".png");
			this.v_guiview.AddObject(v_object);

			v_text = new Spartacus.Game.Text(750, 255, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("Stop");
			this.v_guiview.AddText(v_text);

			v_object = new Spartacus.Game.Object("walk", 700, 300, 32, 32);
			v_object.AddImage("gui/walk.png");
			v_object.AddImage("gui/walk_" + this.v_player + ".png");
			this.v_guiview.AddObject(v_object);

			v_text = new Spartacus.Game.Text(750, 305, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("Walk");
			this.v_guiview.AddText(v_text);

			v_object = new Spartacus.Game.Object("run", 700, 350, 32, 32);
			v_object.AddImage("gui/run.png");
			v_object.AddImage("gui/run_" + this.v_player + ".png");
			this.v_guiview.AddObject(v_object);

			v_text = new Spartacus.Game.Text(750, 355, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("Run");
			this.v_guiview.AddText(v_text);

			v_object = new Spartacus.Game.Object("shoot", 700, 400, 32, 32);
			v_object.AddImage("gui/shoot.png");
			v_object.AddImage("gui/shoot_" + this.v_player + ".png");
			this.v_guiview.AddObject(v_object);

			v_text = new Spartacus.Game.Text(750, 405, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("Shoot");
			this.v_guiview.AddText(v_text);

			v_object = new Spartacus.Game.Object("throw", 700, 450, 32, 32);
			v_object.AddImage("gui/throw.png");
			v_object.AddImage("gui/throw_" + this.v_player + ".png");
			this.v_guiview.AddObject(v_object);

			v_text = new Spartacus.Game.Text(750, 455, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("Throw");
			this.v_guiview.AddText(v_text);

			v_object = new Spartacus.Game.Object("endturn", 700, 500, 32, 32);
			v_object.AddImage("gui/endturn.png");
			this.v_guiview.AddObject(v_object);

			v_text = new Spartacus.Game.Text(750, 505, "Courier New", 12, 255, 255, 255, 255);
			v_text.SetMessage("End Turn");
			this.v_guiview.AddText(v_text);
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
				default:
					break;
			}
		}

		private void OnSoldierMouseClick(Spartacus.Game.Object p_object)
		{
			if (this.v_selected < 0 &&
			    ! string.IsNullOrWhiteSpace(p_object.v_name) &&
			    p_object.v_name.StartsWith(this.v_player))
			{
				for (int w = 0; w < this.v_soldiers.Length; w++)
				{
					if (this.v_soldiers[w].v_object.v_name == p_object.v_name)
					{
						this.v_soldiers[w].v_object.SetBorder(this.v_playercolor, 5);
						this.v_soldiers[w].Greet();
						this.v_selected = w;
					}
				}
			}
		}

		private void OnMapMouseClick(Spartacus.Game.Object p_object)
		{
			if (this.v_selected >= 0 && string.IsNullOrWhiteSpace(p_object.v_name))
			{
				this.v_soldiers[this.v_selected].v_object.RemoveBorder();
				this.v_soldiers[this.v_selected].Walk(p_object.v_rectangle.X/96, p_object.v_rectangle.Y/96, this.v_mapview_x, this.v_mapview_y);
				this.v_soldiers[this.v_selected].Stop();
				this.v_selected = -1;
			}
		}

		private void OnGuiMouseClick(Spartacus.Game.Object p_object)
		{
			
		}

		private void OnTime()
		{
		}

		private void OnCollision(Spartacus.Game.Object p_object1, Spartacus.Game.Object p_object2)
		{
		}
	}
}

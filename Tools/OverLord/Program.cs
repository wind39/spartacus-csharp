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
		int v_border_width = 10;
		int v_tree_chance = 20;
		int v_maxhealth = 100;
		int v_maxactions = 5;
		int v_maxstamina = 100;
		int v_maxammo = 50;
		int v_maxgrenades = 5;

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
		string v_action;
		TeamColor v_turn;
		Range v_range;


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

			this.v_soldiers = new Soldier[4];
			this.v_soldiers[0] = new Soldier(
				this.v_database,
				TeamColor.GREEN,
				"green_1",
				2, 3
			);
			this.v_soldiers[1] = new Soldier(
				this.v_database,
				TeamColor.GREEN,
				"green_2",
				2, 4
			);
			this.v_soldiers[2] = new Soldier(
				this.v_database,
				TeamColor.RED,
				"red_1",
				6, 1
			);
			this.v_soldiers[3] = new Soldier(
				this.v_database,
				TeamColor.BLUE,
				"blue_1",
				1, 6
			);

			this.v_selected = -1;
			this.v_action = "";
			this.v_turn = TeamColor.GREEN;

			this.v_range = new Range(this.v_tileset, this.v_map_size, this.v_mapview_width, this.v_mapview_height);

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

							if (this.v_soldiers[w].v_color == this.v_turn)
							{
								if (this.v_soldiers[w].v_actions > 0)
									this.v_soldiers[w].v_object.SetBorder(System.Drawing.Color.White, this.v_border_width);
								else
									this.v_soldiers[w].v_object.SetBorder(System.Drawing.Color.Black, this.v_border_width);
							}
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
			//v_text.SetMessage("Soldier 1");
			this.v_guiview.AddText(v_text);

			// 1 - Soldier Health
			v_text = new Spartacus.Game.Text(880, 155, "Courier New", 12, 255, 255, 255, 255);
			//v_text.SetMessage("100/100");
			this.v_guiview.AddText(v_text);

			// 2 - Soldier Actions
			v_text = new Spartacus.Game.Text(920, 205, "Courier New", 12, 255, 255, 255, 255);
			//v_text.SetMessage("3/3");
			this.v_guiview.AddText(v_text);

			// 3 - Soldier Stamina
			v_text = new Spartacus.Game.Text(880, 355, "Courier New", 12, 255, 255, 255, 255);
			//v_text.SetMessage("100/100");
			this.v_guiview.AddText(v_text);

			// 4 - Soldier Ammo
			v_text = new Spartacus.Game.Text(900, 405, "Courier New", 12, 255, 255, 255, 255);
			//v_text.SetMessage("20/20");
			this.v_guiview.AddText(v_text);

			// 5 - Soldier Grenades
			v_text = new Spartacus.Game.Text(920, 455, "Courier New", 12, 255, 255, 255, 255);
			//v_text.SetMessage("2/2");
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
					if (this.v_soldiers[w].v_object.v_name == p_object.v_name &&
					    this.v_soldiers[w].v_health > 0 &&
					    this.v_soldiers[w].v_actions > 0 &&
					    this.v_action != "")
					{
						switch (this.v_action)
						{
							case "stop":
								this.v_soldiers[w].v_object.SetBorder(System.Drawing.Color.Black, this.v_border_width);
								this.v_soldiers[w].Greet();
								this.v_soldiers[w].Stop();
								break;
							case "walk":
								this.v_soldiers[w].v_object.SetBorder(this.v_playercolor, this.v_border_width);
								this.v_soldiers[w].Greet();
								for (int x = 0; x < this.v_mapview_width; x++)
								{
									for (int y = 0; y < this.v_mapview_height; y++)
									{
										if (this.v_range.CanWalk(this.v_soldiers, this.v_soldiers[w], this.v_mapview_x+x, this.v_mapview_y+y))
											this.v_mapview.v_objects[x*this.v_mapview_width+y].SetBorder(this.v_playercolor, this.v_border_width);
									}
								}
								break;
							case "run":
								if (this.v_soldiers[w].v_stamina >= 10)
								{
									this.v_soldiers[w].v_object.SetBorder(this.v_playercolor, this.v_border_width);
									this.v_soldiers[w].Greet();
									for (int x = 0; x < this.v_mapview_width; x++)
									{
										for (int y = 0; y < this.v_mapview_height; y++)
										{
											if (this.v_range.CanRun(this.v_soldiers, this.v_soldiers[w], this.v_mapview_x+x, this.v_mapview_y+y))
												this.v_mapview.v_objects[x*this.v_mapview_width+y].SetBorder(this.v_playercolor, this.v_border_width);
										}
									}
								}
								break;
							case "shoot":
								if (this.v_soldiers[w].v_ammo > 0)
								{
									this.v_soldiers[w].v_object.SetBorder(this.v_playercolor, this.v_border_width);
									this.v_soldiers[w].Greet();
									for (int x = 0; x < this.v_mapview_width; x++)
									{
										for (int y = 0; y < this.v_mapview_height; y++)
										{
											if (this.v_range.CanShoot(this.v_soldiers, this.v_soldiers[w], this.v_mapview_x+x, this.v_mapview_y+y))
												this.v_mapview.v_objects[x*this.v_mapview_width+y].SetBorder(this.v_playercolor, this.v_border_width);
										}
									}
								}
								break;
							case "throw":
								if (this.v_soldiers[w].v_grenades > 0)
								{
									this.v_soldiers[w].v_object.SetBorder(this.v_playercolor, this.v_border_width);
									this.v_soldiers[w].Greet();
									for (int x = 0; x < this.v_mapview_width; x++)
									{
										for (int y = 0; y < this.v_mapview_height; y++)
										{
											if (this.v_range.CanThrow(this.v_soldiers, this.v_soldiers[w], this.v_mapview_x+x, this.v_mapview_y+y))
												this.v_mapview.v_objects[x*this.v_mapview_width+y].SetBorder(this.v_playercolor, this.v_border_width);
										}
									}
								}
								break;
							default:
								break;
						}

						this.v_guiview.v_texts[0].SetMessage(this.v_soldiers[w].v_name);
						this.v_guiview.v_texts[1].SetMessage(string.Format("{0:000}/{1:000}", this.v_soldiers[w].v_health, this.v_maxhealth));
						this.v_guiview.v_texts[2].SetMessage(string.Format("{0:0}/{1:0}", this.v_soldiers[w].v_actions, this.v_maxactions));
						this.v_guiview.v_texts[3].SetMessage(string.Format("{0:000}/{1:000}", this.v_soldiers[w].v_stamina, this.v_maxstamina));
						this.v_guiview.v_texts[4].SetMessage(string.Format("{0:00}/{1:00}", this.v_soldiers[w].v_ammo, this.v_maxammo));
						this.v_guiview.v_texts[5].SetMessage(string.Format("{0:0}/{1:0}", this.v_soldiers[w].v_grenades, this.v_maxgrenades));

						this.v_selected = w;
					}
				}
			}
			else if (this.v_selected >= 0 &&
			         ! string.IsNullOrWhiteSpace(p_object.v_name) &&
			         ! p_object.v_name.StartsWith(this.v_player))
			{
				for (int w = 0; w < this.v_soldiers.Length; w++)
				{
					if (this.v_soldiers[w].v_object.v_name == p_object.v_name &&
					    this.v_soldiers[w].v_health > 0 &&
					    this.v_soldiers[w].v_actions > 0 &&
					    this.v_action != "")
					{
						if (this.v_action == "shoot")
						{
							this.v_soldiers[this.v_selected].Shoot(p_object.v_rectangle.X/this.v_tile_size, p_object.v_rectangle.Y/this.v_tile_size, this.v_mapview_x, this.v_mapview_y);


							/*this.DrawLine(
								this.v_soldiers[this.v_selected].v_object.v_rectangle.X/this.v_tile_size,
								this.v_soldiers[this.v_selected].v_object.v_rectangle.Y/this.v_tile_size,
								//p_object.v_rectangle.X/this.v_tile_size,
								this.v_soldiers[w].v_object.v_rectangle.X/this.v_tile_size,
								//p_object.v_rectangle.Y/this.v_tile_size
								this.v_soldiers[w].v_object.v_rectangle.Y/this.v_tile_size
							);*/
						}
					}
				}
			}
		}

		private void OnMapMouseClick(Spartacus.Game.Object p_object)
		{
			if (this.v_selected >= 0 &&
			    string.IsNullOrWhiteSpace(p_object.v_name) &&
			    this.v_action != "" &&
			    p_object.v_border != null)
			{
				switch (this.v_action)
				{
					case "walk":
						this.v_soldiers[this.v_selected].Walk(p_object.v_rectangle.X/this.v_tile_size, p_object.v_rectangle.Y/this.v_tile_size, this.v_mapview_x, this.v_mapview_y);
						break;
					case "run":
						this.v_soldiers[this.v_selected].Run(p_object.v_rectangle.X/this.v_tile_size, p_object.v_rectangle.Y/this.v_tile_size, this.v_mapview_x, this.v_mapview_y);
						break;
					case "throw":
						break;
					default:
						break;
				}

				if (this.v_soldiers[this.v_selected].v_actions > 0)
					this.v_soldiers[this.v_selected].v_object.SetBorder(System.Drawing.Color.White, this.v_border_width);
				else
					this.v_soldiers[this.v_selected].v_object.SetBorder(System.Drawing.Color.Black, this.v_border_width);

				this.v_guiview.v_texts[0].SetMessage(this.v_soldiers[this.v_selected].v_name);
				this.v_guiview.v_texts[1].SetMessage(string.Format("{0:000}/{1:000}", this.v_soldiers[this.v_selected].v_health, this.v_maxhealth));
				this.v_guiview.v_texts[2].SetMessage(string.Format("{0:0}/{1:0}", this.v_soldiers[this.v_selected].v_actions, this.v_maxactions));
				this.v_guiview.v_texts[3].SetMessage(string.Format("{0:000}/{1:000}", this.v_soldiers[this.v_selected].v_stamina, this.v_maxstamina));
				this.v_guiview.v_texts[4].SetMessage(string.Format("{0:00}/{1:00}", this.v_soldiers[this.v_selected].v_ammo, this.v_maxammo));
				this.v_guiview.v_texts[5].SetMessage(string.Format("{0:0}/{1:0}", this.v_soldiers[this.v_selected].v_grenades, this.v_maxgrenades));

				if (this.v_action != "shoot")
					this.v_selected = -1;

				for (int x = 0; x < this.v_mapview_width; x++)
				{
					for (int y = 0; y < this.v_mapview_height; y++)
						this.v_mapview.v_objects[x*this.v_mapview_width+y].RemoveBorder();
				}
			}
		}

		private void OnGuiMouseClick(Spartacus.Game.Object p_object)
		{
			for (int k = 0; k < this.v_guiview.v_objects.Count; k++)
			{
				if (p_object.v_name == "stop" ||
				    p_object.v_name == "walk" ||
				    p_object.v_name == "run" ||
				    p_object.v_name == "shoot" ||
				    p_object.v_name == "throw")
				{
					this.v_action = p_object.v_name;

					if (p_object.v_name == this.v_guiview.v_objects[k].v_name)
						this.v_guiview.v_objects[k].v_currentimage = this.v_guiview.v_objects[k].v_images[1];
					else
						this.v_guiview.v_objects[k].v_currentimage = this.v_guiview.v_objects[k].v_images[0];
				}
				else if (p_object.v_name == "endturn")
					this.v_action = p_object.v_name;
			}

			if (this.v_selected >= 0)
			{
				if (this.v_soldiers[this.v_selected].v_actions > 0)
					this.v_soldiers[this.v_selected].v_object.SetBorder(System.Drawing.Color.White, this.v_border_width);
				else
					this.v_soldiers[this.v_selected].v_object.SetBorder(System.Drawing.Color.Black, this.v_border_width);
			}

			this.v_selected = -1;

			for (int x = 0; x < this.v_mapview_width; x++)
			{
				for (int y = 0; y < this.v_mapview_height; y++)
					this.v_mapview.v_objects[x*this.v_mapview_width+y].RemoveBorder();
			}
		}

		private void DrawLine(int x, int y, int x2, int y2)
		{
			int w = x2 - x;
			int h = y2 - y;
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
			if (w<0) dx1 = -1 ; else if (w>0) dx1 = 1;
			if (h<0) dy1 = -1 ; else if (h>0) dy1 = 1;
			if (w<0) dx2 = -1 ; else if (w>0) dx2 = 1;
			int longest = Math.Abs(w);
			int shortest = Math.Abs(h);
			if (!(longest>shortest))
			{
				longest = Math.Abs(h);
				shortest = Math.Abs(w);
				if (h<0) dy2 = -1; else if (h>0) dy2 = 1;
				dx2 = 0;
			}
			int numerator = longest >> 1;
			for (int i=0;i<=longest;i++)
			{
				this.v_mapview.v_objects[x*this.v_mapview_width+y].SetBorder(System.Drawing.Color.Orange, this.v_border_width);
				numerator += shortest;
				if (!(numerator<longest))
				{
					numerator -= longest;
					x += dx1;
					y += dy1;
				} else
				{
					x += dx2;
					y += dy2;
				}
			}
		}

		private void OnTime()
		{
		}

		private void OnCollision(Spartacus.Game.Object p_object1, Spartacus.Game.Object p_object2)
		{
		}
	}
}

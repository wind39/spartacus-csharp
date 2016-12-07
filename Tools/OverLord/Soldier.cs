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
	public enum TeamColor
	{
		GREEN,
		RED,
		BLUE,
		YELLOW
	}

	public enum Direction
	{
		NORTH,
		NORTHEAST,
		EAST,
		SOUTHEAST,
		SOUTH,
		SOUTHWEST,
		WEST,
		NORTHWEST
	}

	public class Soldier
	{
		public TeamColor v_color;

		public string v_prefix;

		public Spartacus.Game.Object v_object;

		public int v_mapx, v_mapy;

		public Direction v_direction;

		public string v_name;
		public int v_health;
		public int v_actions;
		public int v_stamina;
		public int v_ammo;
		public int v_grenades;

		private System.Random v_random;

		/*
		switch(this.v_direction)
		{
			case Direction.SOUTH:
			break;
			case Direction.SOUTHWEST:
			break;
			case Direction.WEST:
			break;
			case Direction.NORTHWEST:
			break;
			case Direction.NORTH:
			break;
			case Direction.NORTHEAST:
			break;
			case Direction.EAST:
			break;
			case Direction.SOUTHEAST:
			break;
		}
		*/

		public Soldier(
			Spartacus.Database.Generic p_database,
			TeamColor p_color,
			string p_name,
			int p_mapx,
			int p_mapy
		)
		{
			Spartacus.Game.Animation v_animation;

			Direction[] v_directions = new Direction[]
			{
				Direction.SOUTH,
				Direction.SOUTHWEST,
				Direction.WEST,
				Direction.NORTHWEST,
				Direction.NORTH,
				Direction.NORTHEAST,
				Direction.EAST,
				Direction.SOUTHEAST
			};
			int k, f;

			this.v_color = p_color;
			switch (p_color)
			{
				case TeamColor.GREEN:
					this.v_prefix = "soldiers/green/";
					break;
				case TeamColor.RED:
					this.v_prefix = "soldiers/red/";
					break;
				case TeamColor.BLUE:
					this.v_prefix = "soldiers/blue/";
					break;
				case TeamColor.YELLOW:
					this.v_prefix = "soldiers/yellow/";
					break;
				default:
					break;
			}

			this.v_mapx = p_mapx;
			this.v_mapy = p_mapy;
			this.v_name = p_name;
			this.v_health = 100;
			this.v_actions = 3;
			this.v_stamina = 100;
			this.v_ammo = 20;
			this.v_grenades = 2;

			this.v_object = new Spartacus.Game.Object(p_name, 0, 0, 96, 96);

			foreach(System.Data.DataRow r in p_database.Query("select * from animations order by id", "ANIMATIONS").Rows)
			{
				if (r["id"].ToString() == "1")
				{
					k = 0;
					foreach (Direction d in v_directions)
					{
						this.v_object.AddImage(this.v_prefix + r["name"].ToString() + string.Format("{0:0000}", k) + ".png", true);
						k++;
					}
				}
				else
				{
					foreach (Direction d in v_directions)
					{
						v_animation = new Spartacus.Game.Animation(r["name"].ToString(), false);
						k = 0;
						f = 2;
						for (int s = 0; s < int.Parse(r["steps"].ToString()); s++)
						{
							this.v_object.AddImage(this.v_prefix + r["name"].ToString() + "_" + this.DirectionToString(d) + string.Format("{0:0000}", k) + ".png", true);
							v_animation.AddStep(this.v_object.v_images.Count-1, f);
							k++;
							f += 2;
						}
						switch (d)
						{
							case Direction.SOUTH:
								v_animation.AddStep(0, f);
								break;
							case Direction.SOUTHWEST:
								v_animation.AddStep(1, f);
								break;
							case Direction.WEST:
								v_animation.AddStep(2, f);
								break;
							case Direction.NORTHWEST:
								v_animation.AddStep(3, f);
								break;
							case Direction.NORTH:
								v_animation.AddStep(4, f);
								break;
							case Direction.NORTHEAST:
								v_animation.AddStep(5, f);
								break;
							case Direction.EAST:
								v_animation.AddStep(6, f);
								break;
							case Direction.SOUTHEAST:
								v_animation.AddStep(7, f);
								break;
						}
						this.v_object.AddAnimation(v_animation);
					}
				}
			}

			this.v_random = new System.Random();

			this.v_direction = v_directions[this.v_random.Next(v_directions.Length)];
			this.Start();
		}

		private string DirectionToString(Direction p_direction)
		{
			switch(p_direction)
			{
				case Direction.EAST:
					return "e";
				case Direction.NORTH:
					return "n";
				case Direction.NORTHEAST:
					return "ne";
				case Direction.NORTHWEST:
					return "nw";
				case Direction.SOUTH:
					return "s";
				case Direction.SOUTHEAST:
					return "se";
				case Direction.SOUTHWEST:
					return "sw";
				case Direction.WEST:
					return "w";
				default:
					return null;
			}
		}

		private void Start()
		{
			switch(this.v_direction)
			{
				case Direction.SOUTH:
					this.v_object.v_currentimage = this.v_object.v_images[0];
					break;
				case Direction.SOUTHWEST:
					this.v_object.v_currentimage = this.v_object.v_images[1];
					break;
				case Direction.WEST:
					this.v_object.v_currentimage = this.v_object.v_images[2];
					break;
				case Direction.NORTHWEST:
					this.v_object.v_currentimage = this.v_object.v_images[3];
					break;
				case Direction.NORTH:
					this.v_object.v_currentimage = this.v_object.v_images[4];
					break;
				case Direction.NORTHEAST:
					this.v_object.v_currentimage = this.v_object.v_images[5];
					break;
				case Direction.EAST:
					this.v_object.v_currentimage = this.v_object.v_images[6];
					break;
				case Direction.SOUTHEAST:
					this.v_object.v_currentimage = this.v_object.v_images[7];
					break;
			}
		}

		public void Greet()
		{
			switch(this.v_direction)
			{
				case Direction.SOUTH:
					this.v_object.v_animations[0].Start();
					break;
				case Direction.SOUTHWEST:
					this.v_object.v_animations[1].Start();
					break;
				case Direction.WEST:
					this.v_object.v_animations[2].Start();
					break;
				case Direction.NORTHWEST:
					this.v_object.v_animations[3].Start();
					break;
				case Direction.NORTH:
					this.v_object.v_animations[4].Start();
					break;
				case Direction.NORTHEAST:
					this.v_object.v_animations[5].Start();
					break;
				case Direction.EAST:
					this.v_object.v_animations[6].Start();
					break;
				case Direction.SOUTHEAST:
					this.v_object.v_animations[7].Start();
					break;
			}
		}

		public void Stop()
		{
			this.v_actions = 0;
		}

		public void Walk(int p_x, int p_y, int p_mapview_x, int p_mapview_y)
		{
			//Direction.SOUTH
			if (this.v_mapx == (p_mapview_x + p_x) && this.v_mapy < (p_mapview_y + p_y))
			{
				this.v_object.v_animations[8].Start();
				this.v_object.Move(0, 96, 16);
				this.v_mapy++;
				this.v_direction = Direction.SOUTH;
			}
			//Direction.SOUTHWEST
			else if (this.v_mapx > (p_mapview_x + p_x) && this.v_mapy < (p_mapview_y + p_y))
			{
				this.v_object.v_animations[9].Start();
				this.v_object.Move(-96, 96, 16);
				this.v_mapx--;
				this.v_mapy++;
				this.v_direction = Direction.SOUTHWEST;
			}
			//Direction.WEST
			else if (this.v_mapx > (p_mapview_x + p_x) && this.v_mapy == (p_mapview_y + p_y))
			{
				this.v_object.v_animations[10].Start();
				this.v_object.Move(-96, 0, 16);
				this.v_mapx--;
				this.v_direction = Direction.WEST;
			}
			//Direction.NORTHWEST
			else if (this.v_mapx > (p_mapview_x + p_x) && this.v_mapy > (p_mapview_y + p_y))
			{
				this.v_object.v_animations[11].Start();
				this.v_object.Move(-96, -96, 16);
				this.v_mapx--;
				this.v_mapy--;
				this.v_direction = Direction.NORTHWEST;
			}
			//Direction.NORTH
			else if (this.v_mapx == (p_mapview_x + p_x) && this.v_mapy > (p_mapview_y + p_y))
			{
				this.v_object.v_animations[12].Start();
				this.v_object.Move(0, -96, 16);
				this.v_mapy--;
				this.v_direction = Direction.NORTH;
			}
			//Direction.NORTHEAST
			else if (this.v_mapx < (p_mapview_x + p_x) && this.v_mapy > (p_mapview_y + p_y))
			{
				this.v_object.v_animations[13].Start();
				this.v_object.Move(96, -96, 16);
				this.v_mapx++;
				this.v_mapy--;
				this.v_direction = Direction.NORTHEAST;
			}
			//Direction.EAST
			else if (this.v_mapx < (p_mapview_x + p_x) && this.v_mapy == (p_mapview_y + p_y))
			{
				this.v_object.v_animations[14].Start();
				this.v_object.Move(96, 0, 16);
				this.v_mapx++;
				this.v_direction = Direction.EAST;
			}
			//Direction.SOUTHEAST
			else if (this.v_mapx < (p_mapview_x + p_x) && this.v_mapy < (p_mapview_y + p_y))
			{
				this.v_object.v_animations[15].Start();
				this.v_object.Move(96, 96, 16);
				this.v_mapx++;
				this.v_mapy++;
				this.v_direction = Direction.SOUTHEAST;
			}

			this.v_actions--;
			//this.EndMovement();
		}

		//TODO: melhorar este método para não ir somente reto, respeitando obstáculos
		public void Run(int p_x, int p_y, int p_mapview_x, int p_mapview_y)
		{
			//Direction.SOUTH
			if (this.v_mapx == (p_mapview_x + p_x) && this.v_mapy < (p_mapview_y + p_y))
			{
				this.v_object.v_animations[16].Start();
				this.v_object.Move(0, 192, 16);
				this.v_mapy++;
				this.v_direction = Direction.SOUTH;
			}
			//Direction.SOUTHWEST
			else if (this.v_mapx > (p_mapview_x + p_x) && this.v_mapy < (p_mapview_y + p_y))
			{
				this.v_object.v_animations[17].Start();
				this.v_object.Move(-192, 192, 16);
				this.v_mapx--;
				this.v_mapy++;
				this.v_direction = Direction.SOUTHWEST;
			}
			//Direction.WEST
			else if (this.v_mapx > (p_mapview_x + p_x) && this.v_mapy == (p_mapview_y + p_y))
			{
				this.v_object.v_animations[18].Start();
				this.v_object.Move(-192, 0, 192);
				this.v_mapx--;
				this.v_direction = Direction.WEST;
			}
			//Direction.NORTHWEST
			else if (this.v_mapx > (p_mapview_x + p_x) && this.v_mapy > (p_mapview_y + p_y))
			{
				this.v_object.v_animations[19].Start();
				this.v_object.Move(-192, -192, 16);
				this.v_mapx--;
				this.v_mapy--;
				this.v_direction = Direction.NORTHWEST;
			}
			//Direction.NORTH
			else if (this.v_mapx == (p_mapview_x + p_x) && this.v_mapy > (p_mapview_y + p_y))
			{
				this.v_object.v_animations[20].Start();
				this.v_object.Move(0, -192, 16);
				this.v_mapy--;
				this.v_direction = Direction.NORTH;
			}
			//Direction.NORTHEAST
			else if (this.v_mapx < (p_mapview_x + p_x) && this.v_mapy > (p_mapview_y + p_y))
			{
				this.v_object.v_animations[21].Start();
				this.v_object.Move(192, -192, 16);
				this.v_mapx++;
				this.v_mapy--;
				this.v_direction = Direction.NORTHEAST;
			}
			//Direction.EAST
			else if (this.v_mapx < (p_mapview_x + p_x) && this.v_mapy == (p_mapview_y + p_y))
			{
				this.v_object.v_animations[22].Start();
				this.v_object.Move(192, 0, 16);
				this.v_mapx++;
				this.v_direction = Direction.EAST;
			}
			//Direction.SOUTHEAST
			else if (this.v_mapx < (p_mapview_x + p_x) && this.v_mapy < (p_mapview_y + p_y))
			{
				this.v_object.v_animations[23].Start();
				this.v_object.Move(192, 192, 16);
				this.v_mapx++;
				this.v_mapy++;
				this.v_direction = Direction.SOUTHEAST;
			}

			this.v_actions--;
			this.v_stamina -= 10;
			//this.EndMovement();
		}
	}
}

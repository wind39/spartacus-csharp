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
	public class Range
	{
		public Tile[,] v_tileset;

		public int v_map_size;

		public int v_mapview_width;

		public int v_mapview_height;


		public Range(Tile[,] p_tileset, int p_map_size, int p_mapview_width, int p_mapview_height)
		{
			this.v_tileset = p_tileset;
			this.v_map_size = p_map_size;
			this.v_mapview_width = p_mapview_width;
			this.v_mapview_height = p_mapview_height;
		}

		private bool HasSoldier(Soldier[] p_soldiers, int p_x, int p_y)
		{
			bool v_achou = false;
			int k = 0;
			while (k < p_soldiers.Length && ! v_achou)
			{
				if (p_soldiers[k].v_mapx == p_x &&
				    p_soldiers[k].v_mapy == p_y &&
				    p_soldiers[k].v_health > 0)
					v_achou = true;
				else
					k++;
			}
			return v_achou;
		}

		private bool HasEnemySoldier(Soldier[] p_soldiers, Soldier p_soldier, int p_x, int p_y)
		{
			bool v_achou = false;
			int k = 0;
			while (k < p_soldiers.Length && ! v_achou)
			{
				if (p_soldiers[k].v_mapx == p_x &&
				    p_soldiers[k].v_mapy == p_y &&
				    p_soldiers[k].v_health > 0 &&
				    p_soldiers[k].v_color != p_soldier.v_color)
					v_achou = true;
				else
					k++;
			}
			return v_achou;
		}

		private bool HasFriendSoldier(Soldier[] p_soldiers, Soldier p_soldier, int p_x, int p_y)
		{
			bool v_achou = false;
			int k = 0;
			while (k < p_soldiers.Length && ! v_achou)
			{
				if (p_soldiers[k].v_mapx == p_x &&
				    p_soldiers[k].v_mapy == p_y &&
				    p_soldiers[k].v_health > 0 &&
				    p_soldiers[k].v_color == p_soldier.v_color)
					v_achou = true;
				else
					k++;
			}
			return v_achou;
		}

		private bool HasCurrentSoldier(Soldier p_soldier, int p_x, int p_y)
		{
			return p_soldier.v_mapx == p_x && p_soldier.v_mapy == p_y;
		}

		private bool FreeStraightPath(Soldier[] p_soldiers, Soldier p_soldier, int x, int y, int x2, int y2)
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
				if (! this.HasCurrentSoldier(p_soldier, x, y) &&
				    (this.v_tileset[x, y].v_block || this.HasFriendSoldier(p_soldiers, p_soldier, x, y)))
					return false;
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
			return true;
		}

		public bool CanWalk(Soldier[] p_soldiers, Soldier p_soldier, int p_x, int p_y)
		{
			int v_distance_x = p_x - p_soldier.v_mapx;
			int v_distance_y = p_y - p_soldier.v_mapy;

			return (! this.HasSoldier(p_soldiers, p_x, p_y) &&
			        this.v_tileset[p_x, p_y].v_canstep &&
			        ! this.v_tileset[p_x, p_y].v_block &&
			        v_distance_x >= -1 &&
			        v_distance_x <= 1 &&
			        v_distance_y >= -1 &&
			        v_distance_y <= 1);
		}

		public bool CanRun(Soldier[] p_soldiers, Soldier p_soldier, int p_x, int p_y)
		{
			int v_distance_x = p_x - p_soldier.v_mapx;
			int v_distance_y = p_y - p_soldier.v_mapy;

			return (! this.HasSoldier(p_soldiers, p_x, p_y) &&
			        this.v_tileset[p_x, p_y].v_canstep &&
			        ! this.v_tileset[p_x, p_y].v_block &&
			        v_distance_x >= -1 &&
			        v_distance_x <= 1 &&
			        v_distance_y >= -1 &&
			        v_distance_y <= 1);
		}

		public bool CanShoot(Soldier[] p_soldiers, Soldier p_soldier, int p_x, int p_y)
		{
			if (this.HasEnemySoldier(p_soldiers, p_soldier, p_x, p_y) &&
			    this.FreeStraightPath(p_soldiers, p_soldier, p_soldier.v_mapx, p_soldier.v_mapy, p_x, p_y))
				return true;
			else
				return false;
		}

		public bool CanThrow(Soldier[] p_soldiers, Soldier p_soldier, int p_x, int p_y)
		{
			int v_distance_x = p_x - p_soldier.v_mapx;
			int v_distance_y = p_y - p_soldier.v_mapy;

			return (v_distance_x >= -3 &&
			        v_distance_x <= 3 &&
			        v_distance_y >= -3 &&
			        v_distance_y <= 3);
		}
	}
}


/* Bresenham algorithm
 public void line(int x,int y,int x2, int y2, int color) {
    int w = x2 - x ;
    int h = y2 - y ;
    int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0 ;
    if (w<0) dx1 = -1 ; else if (w>0) dx1 = 1 ;
    if (h<0) dy1 = -1 ; else if (h>0) dy1 = 1 ;
    if (w<0) dx2 = -1 ; else if (w>0) dx2 = 1 ;
    int longest = Math.abs(w) ;
    int shortest = Math.abs(h) ;
    if (!(longest>shortest)) {
        longest = Math.abs(h) ;
        shortest = Math.abs(w) ;
        if (h<0) dy2 = -1 ; else if (h>0) dy2 = 1 ;
        dx2 = 0 ;            
    }
    int numerator = longest >> 1 ;
    for (int i=0;i<=longest;i++) {
        putpixel(x,y,color) ;
        numerator += shortest ;
        if (!(numerator<longest)) {
            numerator -= longest ;
            x += dx1 ;
            y += dy1 ;
        } else {
            x += dx2 ;
            y += dy2 ;
        }
    }
}*/
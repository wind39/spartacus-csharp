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

namespace Spartacus.Tools.Pong
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            (new MainClass()).Initialize();
        }

        Spartacus.Forms.Window v_window;
        Spartacus.Game.Object v_racket_left, v_racket_right, v_ball;
        int v_ball_x, v_ball_y;
        Spartacus.Game.Object v_bound_up, v_bound_down, v_bound_left, v_bound_right;
        Spartacus.Game.Layer v_layer;
        Spartacus.Game.Level v_level;
        Spartacus.Game.Keyboard v_keyboard;
        Spartacus.Game.Text v_score_left, v_score_right, v_text_paused;
        bool v_paused;

        public void Initialize()
        {
            v_window = new Spartacus.Forms.Window("Spartacus Pong", 800, 600);

            v_racket_left = new Spartacus.Game.Object("RL", 20, (v_window.v_height-100)/2, 20, 100);
            v_racket_left.AddImage("racket.png");

            v_racket_right = new Spartacus.Game.Object("RR", v_window.v_width-40, (v_window.v_height-100)/2, 20, 100);
            v_racket_right.AddImage("racket.png");

            v_ball = new Spartacus.Game.Object("B", v_window.v_width/2, v_window.v_height/2, 15, 15);
            v_ball.AddImage("ball.png");
            v_ball_x = 10;
            v_ball_y = 10;

            v_bound_up = new Spartacus.Game.Object("BU", 0, 0, v_window.v_width, 10);
            v_bound_down = new Spartacus.Game.Object("BD", 0, v_window.v_height-10, v_window.v_width, 10);
            v_bound_left = new Spartacus.Game.Object("BL", 0, 0, 10, v_window.v_height);
            v_bound_right = new Spartacus.Game.Object("BR", v_window.v_width-10, 0, 10, v_window.v_height);

            v_score_left = new Spartacus.Game.Text(80, 10, "Courier New", 14, 255, 255, 255, 255);
            v_score_left.SetMessage("0");

            v_score_right = new Spartacus.Game.Text(v_window.v_width-100, 10, "Courier New", 14, 255, 255, 255, 255);
            v_score_right.SetMessage("0");

            v_text_paused = new Spartacus.Game.Text((v_window.v_width-50)/2, 10, "Courier New", 14, 255, 255, 255, 255);
            v_text_paused.SetMessage("");
            v_paused = false;

            v_layer = new Spartacus.Game.Layer();
            v_layer.AddObject(v_racket_left);
            v_layer.AddObject(v_racket_right);
            v_layer.AddObject(v_ball);
            v_layer.AddObject(v_bound_up);
            v_layer.AddObject(v_bound_down);
            v_layer.AddObject(v_bound_left);
            v_layer.AddObject(v_bound_right);
            v_layer.AddText(v_score_left);
            v_layer.AddText(v_score_right);
            v_layer.AddText(v_text_paused);
            v_layer.Collision += this.OnCollision;

            v_keyboard = new Spartacus.Game.Keyboard(v_window);
            v_keyboard.KeyDown += this.OnKeyDown;
            v_keyboard.KeyPress += this.OnKeyPress;
            v_keyboard.Start(60);

            v_level = new Spartacus.Game.Level(v_window);
            v_level.AddLayer(v_layer);
            v_level.Time += this.OnTime;
            v_level.Start(30);

            v_window.Run();
        }

        private void OnKeyDown(System.Windows.Forms.Keys p_key)
        {
            if (!v_paused)
            {
                switch (p_key)
                {
                    case System.Windows.Forms.Keys.Up:
                        v_racket_right.Move(0, -10, true);
                        break;
                    case System.Windows.Forms.Keys.Down:
                        v_racket_right.Move(0, 10, true);
                        break;
                    case System.Windows.Forms.Keys.Q:
                        v_racket_left.Move(0, -10, true);
                        break;
                    case System.Windows.Forms.Keys.X:
                        v_racket_left.Move(0, 10, true);
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnKeyPress(System.Windows.Forms.Keys p_key)
        {
            if (p_key == System.Windows.Forms.Keys.Space)
                v_paused = ! v_paused;

            if (v_paused)
                v_text_paused.SetMessage("PAUSED");
            else
                v_text_paused.SetMessage("");
        }

        private void OnTime()
        {
            if (!v_paused)
                v_ball.Move(v_ball_x, v_ball_y);
        }

        private void OnCollision(Spartacus.Game.Object p_object1, Spartacus.Game.Object p_object2)
        {
            if ((p_object1.v_name == "B" && p_object2.v_name == "BU") ||
                (p_object1.v_name == "BU" && p_object2.v_name == "B"))
                v_ball_y = 10;
            else if ((p_object1.v_name == "B" && p_object2.v_name == "BD") ||
                (p_object1.v_name == "BD" && p_object2.v_name == "B"))
                v_ball_y = -10;
            else if ((p_object1.v_name == "B" && p_object2.v_name == "RL") ||
                (p_object1.v_name == "RL" && p_object2.v_name == "B"))
                v_ball_x = 10;
            else if ((p_object1.v_name == "B" && p_object2.v_name == "RR") ||
                (p_object1.v_name == "RR" && p_object2.v_name == "B"))
                v_ball_x = -10;
            else if ((p_object1.v_name == "B" && p_object2.v_name == "BL") ||
                (p_object1.v_name == "BL" && p_object2.v_name == "B"))
            {
                v_score_right.v_message = (int.Parse(v_score_right.v_message)+1).ToString();
                v_ball.SetPosition(v_window.v_width/2, v_window.v_height/2);
            }
            else if ((p_object1.v_name == "B" && p_object2.v_name == "BR") ||
                (p_object1.v_name == "BR" && p_object2.v_name == "B"))
            {
                v_score_left.v_message = (int.Parse(v_score_left.v_message)+1).ToString();
                v_ball.SetPosition(v_window.v_width/2, v_window.v_height/2);
            }
        }
    }
}

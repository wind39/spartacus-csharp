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
    public struct AnimationStep
    {
        public int v_imageindex;
        public int v_relativeframe;
    }

    public class Animation
    {
        public bool v_circular;

		public System.Collections.Generic.List<Spartacus.Game.AnimationStep> v_steps;

        public bool v_isrunning;

        public int v_currentstep;

        public int v_currentframe;


        public Animation(bool p_circular)
        {
            this.v_circular = p_circular;
            this.v_steps = new System.Collections.Generic.List<Spartacus.Game.AnimationStep>();
            this.v_isrunning = false;
        }

        public void AddStep(int p_imageindex, int p_relativeframe)
        {
            Spartacus.Game.AnimationStep v_step;

            v_step.v_imageindex = p_imageindex;
            v_step.v_relativeframe = p_relativeframe;

            this.v_steps.Add(v_step);
        }

        public void AddStep(Spartacus.Game.AnimationStep p_step)
        {
            this.v_steps.Add(p_step);
        }

        public void Start()
        {
            this.v_isrunning = true;
            this.v_currentstep = 0;
            this.v_currentframe = 0;
        }

        public void IncFrame()
        {
            if (this.v_isrunning)
            {
                this.v_currentframe++;

                if (this.v_currentframe == this.v_steps[this.v_currentstep].v_relativeframe)
                {
                    if (this.v_currentstep == this.v_steps.Count - 1)
                    {
                        if (this.v_circular)
                        {
                            this.v_currentstep = 0;
                            this.v_currentframe = 0;
                        }
                        else
                            this.v_isrunning = false;
                    }
                    else
                        this.v_currentstep++;
                }
            }
        }

        public void Stop()
        {
            this.v_isrunning = false;
        }
    }
}
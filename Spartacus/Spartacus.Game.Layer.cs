using System;

namespace Spartacus.Game
{
    public class Layer
    {
        public System.Collections.ArrayList v_objects;


        public Layer()
        {
            this.v_objects = new System.Collections.ArrayList();
        }

        public void AddObject(Spartacus.Game.Object p_object)
        {
            this.v_objects.Add(p_object);
        }

        public void Render(System.Drawing.Graphics p_graphics)
        {
            for (int k = 0; k < this.v_objects.Count; k++)
                ((Spartacus.Game.Object)this.v_objects[k]).Render(p_graphics);
        }
    }
}

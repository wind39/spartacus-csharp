using System;

namespace Spartacus.Game
{
    public class Object
    {
        System.Drawing.Rectangle v_rectangle;

        System.Collections.ArrayList v_images;

        System.Drawing.Image v_current;


        public Object(int p_x, int p_y, int p_width, int p_height)
        {
            this.v_rectangle = new System.Drawing.Rectangle(p_x, p_y, p_width, p_height);
            this.v_images = new System.Collections.ArrayList();
            this.v_current = null;
        }

        public void AddImage(string p_filename)
        {
            this.v_images.Add(System.Drawing.Image.FromFile(p_filename));
            if (this.v_current == null)
                this.v_current = (System.Drawing.Image) this.v_images[0];
        }

        public void AddImage(System.Drawing.Image p_image)
        {
            this.v_images.Add(p_image);
            if (this.v_current == null)
                this.v_current = (System.Drawing.Image) this.v_images[0];
        }

        public void Render(System.Drawing.Graphics p_graphics)
        {
            p_graphics.DrawImage(this.v_current, this.v_rectangle);
        }
    }
}

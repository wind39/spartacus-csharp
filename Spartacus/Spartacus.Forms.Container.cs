using System;

namespace Spartacus.Forms
{
    public enum ContainerType
    {
        FORM,
        PANEL
    }

    public class Container
    {
        public Spartacus.Forms.Container v_parent;

        public Spartacus.Forms.ContainerType v_type;

        public System.Windows.Forms.Control v_control;

        public string v_title;
        public int v_width;
        public int v_height;

        public System.Collections.ArrayList v_containers;
        public System.Collections.ArrayList v_components;

        public int v_offsety;

        public int v_frozenheight;


        public Container(Spartacus.Forms.ContainerType p_type)
        {
            this.v_parent = null;
            this.v_type = p_type;

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    v_control = new System.Windows.Forms.Form();
                    ((System.Windows.Forms.Form)this.v_control).Resize += new System.EventHandler(this.OnResize);
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    v_control = new System.Windows.Forms.Panel();
                    ((System.Windows.Forms.Panel)this.v_control).Resize += new System.EventHandler(this.OnResize);
                    break;
            }

            this.SetWidth(300);
            this.SetHeight(300);

            this.v_containers = new System.Collections.ArrayList();
            this.v_components = new System.Collections.ArrayList();

            this.v_offsety = 0;

            this.v_frozenheight = 0;
        }

        public Container(Spartacus.Forms.ContainerType p_type, Spartacus.Forms.Container p_parent)
        {
            this.v_parent = p_parent;
            this.v_type = p_type;

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    v_control = new System.Windows.Forms.Form();
                    ((System.Windows.Forms.Form)this.v_control).Resize += new System.EventHandler(this.OnResize);
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    v_control = new System.Windows.Forms.Panel();
                    ((System.Windows.Forms.Panel)this.v_control).Resize += new System.EventHandler(this.OnResize);
                    break;
            }

            this.SetWidth(300);
            this.SetHeight(300);

            this.v_containers = new System.Collections.ArrayList();
            this.v_components = new System.Collections.ArrayList();

            this.v_offsety = 0;

            this.v_frozenheight = 0;

            //TODO: tratar posicao do container dentro do pai
        }

        public void SetWidth(int p_width)
        {
            this.v_width = p_width;

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    ((System.Windows.Forms.Form)this.v_control).Width = p_width;
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    ((System.Windows.Forms.Panel)this.v_control).Width = p_width;
                    break;
            }
        }

        public void SetHeight(int p_height)
        {
            this.v_height = p_height;

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    ((System.Windows.Forms.Form)this.v_control).Height = p_height;
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    ((System.Windows.Forms.Panel)this.v_control).Height = p_height;
                    break;
            }
        }

        public void SetTitle(string p_title)
        {
            this.v_title = p_title;

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    ((System.Windows.Forms.Form)this.v_control).Text = p_title;
                    break;
                default:
                    break;
            }
        }

        public void Add(Spartacus.Forms.Container p_container)
        {
            this.v_containers.Add(p_container);

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    switch (p_container.v_type)
                    {
                        case Spartacus.Forms.ContainerType.FORM:
                            // nao tem como incluir um form dentro de outro container
                            break;
                        case Spartacus.Forms.ContainerType.PANEL:
                            ((System.Windows.Forms.Panel)p_container.v_control).Parent = ((System.Windows.Forms.Form)this.v_control);
                            break;
                    }
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    switch (p_container.v_type)
                    {
                        case Spartacus.Forms.ContainerType.FORM:
                            // nao tem como incluir um form dentro de outro container
                            break;
                        case Spartacus.Forms.ContainerType.PANEL:
                            ((System.Windows.Forms.Panel)p_container.v_control).Parent = ((System.Windows.Forms.Panel)this.v_control);
                            break;
                    }
                    break;
            }

            //TODO: tratar como fica o offsety
        }

        public void Add(Spartacus.Forms.Component p_component)
        {
            this.v_components.Add(p_component);

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    p_component.v_panel.Parent = ((System.Windows.Forms.Form)this.v_control);
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    p_component.v_panel.Parent = ((System.Windows.Forms.Panel)this.v_control);
                    break;
            }

            this.v_offsety += p_component.v_height;

            if (p_component.v_frozenheight)
                this.v_frozenheight += p_component.v_height;
        }

        private void Resize(int p_newwidth, int p_newheight)
        {
            Spartacus.Forms.Container v_container;
            Spartacus.Forms.Component v_component;
            int k, posy;

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    ((System.Windows.Forms.Form)this.v_control).SuspendLayout();
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    ((System.Windows.Forms.Panel)this.v_control).SuspendLayout();
                    break;
            }

            // redimensionando containers
            for (k = 0; k < this.v_containers.Count; k++)
            {
                v_container = (Spartacus.Forms.Container)this.v_containers[k];
            
                v_container.Resize(
                    (int) ((double) p_newwidth * (double) v_container.v_width / (double) this.v_width),
                    (int) ((double) p_newheight * (double) v_container.v_height / (double) this.v_height)
                );
            }

            // redimensionando componentes
            posy = 0;
            for (k = 0; k < this.v_components.Count; k++)
            {
                v_component = (Spartacus.Forms.Component)this.v_components[k];

                v_component.Resize(
                    (int) ((double) p_newwidth * (double) v_component.v_width / (double) this.v_width),
                    (int) (((double) (p_newheight - this.v_frozenheight) * (double) v_component.v_height) / (double) (this.v_height - this.v_frozenheight)),
                    0,
                    posy
                );

                posy += v_component.v_height;
            }

            this.v_width = p_newwidth;
            this.v_height = p_newheight;

            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    ((System.Windows.Forms.Form)this.v_control).ResumeLayout();
                    ((System.Windows.Forms.Form)this.v_control).Refresh();
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    ((System.Windows.Forms.Panel)this.v_control).ResumeLayout();
                    ((System.Windows.Forms.Panel)this.v_control).Refresh();
                    break;
            }
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            switch (this.v_type)
            {
                case Spartacus.Forms.ContainerType.FORM:
                    this.Resize(
                        ((System.Windows.Forms.Form)sender).Width,
                        ((System.Windows.Forms.Form)sender).Height
                    );
                    break;
                case Spartacus.Forms.ContainerType.PANEL:
                    this.Resize(
                        ((System.Windows.Forms.Panel)sender).Width,
                        ((System.Windows.Forms.Panel)sender).Height
                    );
                    break;
            }
        }
    }
}

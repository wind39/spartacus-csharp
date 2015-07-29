/*
The MIT License (MIT)

Copyright (c) 2014,2015 William Ivanski

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

namespace Spartacus.Web
{
    /// <summary>
    /// Classe Window.
    /// Representa uma Janela.
    /// </summary>
    public class Window : Spartacus.Web.Container
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Window"/>.
        /// </summary>
        public Window()
            :base(null)
        {
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            Spartacus.Web.Container v_container;

            for (int k = 0; k < this.v_containers.Count; k++)
            {
                v_container = (Spartacus.Web.Container)this.v_containers [k];
                v_container.Clear();
            }
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
            Spartacus.Web.Container v_container;

            for (int k = 0; k < this.v_containers.Count; k++)
            {
                v_container = (Spartacus.Web.Container)this.v_containers [k];
                v_container.Refresh();
            }
        }

        /// <summary>
        /// Informa o texto ou valor a ser mostrado no Container.
        /// Usado para mostrar ao usuário um formulário já preenchido.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Container.</param>
        public override void SetValue(string p_text)
        {
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Container.
        /// </summary>
        /// <returns>Texto ou valor atual do Container.</returns>
        public override string GetValue()
        {
            return null;
        }

        /// <summary>
        /// Procura e retorna o Container filho que possua o ID solicitado.
        /// </summary>
        /// <returns>Container filho..</returns>
        /// <param name="p_id">Identificador.</param>
        public Spartacus.Web.Container GetChildById(string p_id)
        {
            Spartacus.Web.Container v_container = null;
            bool v_achou;
            int k;

            v_achou = false;
            k = 0;
            while (k < this.v_containers.Count && !v_achou)
            {
                v_container = (Spartacus.Web.Container)this.v_containers [k];
                if (v_container.v_id == p_id)
                    v_achou = true;
                else
                    k++;
            }

            if (v_achou)
                return v_container;
            else
                return null;
        }

        /// <summary>
        /// Renderiza o HTML do Container.
        /// </summary>
        public override string Render()
        {
            string v_html;
            Spartacus.Web.Container v_container;

            v_html = "<form class='pure-form pure-form-aligned' id='spartacus_window' runat='server'><fieldset>";
            for (int k = 0; k < this.v_containers.Count; k++)
            {
                v_container = (Spartacus.Web.Container)this.v_containers [k];
                v_html += v_container.Render();
            }
            v_html += "</fieldset></form>";

            return v_html;
        }
    }
}

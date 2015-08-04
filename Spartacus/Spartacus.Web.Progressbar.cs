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
    public enum ProgressbarSize
    {
        BIG,
        SMALL
    }

    /// <summary>
    /// Classe Progressbar.
    /// Representa um componente que mostra o progresso de execução de alguma tarefa.
    /// </summary>
    public class Progressbar : Spartacus.Web.Container
    {
        /// <summary>
        /// Percentual atual do Progressbar.
        /// </summary>
        public int v_percent;

        /// <summary>
        /// Texto atual do Progressbar.
        /// </summary>
        public string v_text;

        /// <summary>
        /// Tamanho do Progressbar.
        /// </summary>
        public Spartacus.Web.ProgressbarSize v_size;

        /// <summary>
        /// Intervalo de atualização do Progressbar.
        /// </summary>
        public int v_interval;

        /// <summary>
        /// Webmethod utilizado em conjunto com o Progressbar.
        /// </summary>
        public string v_webmethod;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Progressbar"/>.
        /// </summary>
        /// <param name="p_id">Identificador do Container atual.</param>
        /// <param name="p_size">Tamanho do Progressbar.</param>
        /// <param name="p_interval">Intervalo de atualização do Progressbar.</param>
        /// <param name="p_webmethod">Webmethod utilizado em conjunto com o Progressbar.</param>
        /// <param name="p_parent">Container pai.</param>
        public Progressbar(string p_id, Spartacus.Web.ProgressbarSize p_size, int p_interval, string p_webmethod, Spartacus.Web.Container p_parent)
            : base(p_id, p_parent)
        {
            this.v_type = Spartacus.Web.ContainerType.PROGRESSBAR;

            this.v_percent = 0;
            this.v_text = "Aguardando início";
            this.v_size = p_size;
            this.v_interval = p_interval;
            this.v_webmethod = p_webmethod;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            this.v_text = "";
            this.v_percent = 0;
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
        }

        /// <summary>
        /// Informa o texto a ser mostrado no Label.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Label.</param>
        public override void SetValue(string p_text)
        {
            this.v_text = p_text;
        }

        /// <summary>
        /// Informa o texto a ser mostrado no Label e valor a ser mostrado no Progressbar.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Label.</param>
        /// <param name="p_value">Valor a ser mostrado no Progressbar</param>
        public void SetValue(string p_text, int p_value)
        {
            this.v_text = p_text;
            this.v_percent = p_value;
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Textbox.
        /// </summary>
        /// <returns>Texto ou valor atual do Textbox.</returns>
        public override string GetValue()
        {
            return this.v_text;
        }

        /// <summary>
        /// Renderiza o HTML do Container.
        /// </summary>
        public override string Render()
        {
            return "<center><div id='" + this.v_id + "'></div></center>";
        }

        /// <summary>
        /// Renderiza interior do Progressbar.
        /// </summary>
        public string RenderInner()
        {
            string v_html;
            string v_sizetext;

            if (this.v_size == Spartacus.Web.ProgressbarSize.BIG)
                v_sizetext = "big";
            else
                v_sizetext = "small";

            v_html = "<img style='margin-right: 0px;' src='images/progress_border.png'/>";

            if (this.v_percent == 100)
            {
                for (int k = 0; k < this.v_percent; k++)
                    v_html += "<img style='margin-right: 0px;' src='images/progress_green_" + v_sizetext + ".png'/>";
            }
            else
            {
                for (int k = 0; k < this.v_percent; k++)
                    v_html += "<img style='margin-right: 0px;' src='images/progress_yellow_" + v_sizetext + ".png'/>";

                for (int k = this.v_percent; k < 100; k++)
                    v_html += "<img style='margin-right: 0px;' src='images/progress_empty_" + v_sizetext + ".png'/>";
            }

            v_html += "<img style='margin-right: 0px;' src='images/progress_border.png'/>";
            v_html += "</br>" + this.v_percent.ToString() + "%</br>" + this.v_text;

            return v_html;
        }
    }
}

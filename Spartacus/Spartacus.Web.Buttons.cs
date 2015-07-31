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
    /// Classe Buttons.
    /// Herda da classe <see cref="Spartacus.Web.Container"/> 
    /// Representa um componente com um ou mais botões.
    /// </summary>
    public class Buttons : Spartacus.Web.Container
    {
        /// <summary>
        /// Lista de botões.
        /// </summary>
        public System.Collections.ArrayList v_list;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Buttons"/>.
        /// </summary>
        /// <param name="p_parent">Container pai.</param>
        public Buttons(Spartacus.Web.Container p_parent)
            : base(null, p_parent)
        {
            this.v_type = Spartacus.Web.ContainerType.BUTTONS;

            this.v_list = new System.Collections.ArrayList();
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
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
        /// Adiciona um botão à lista de botões.
        /// </summary>
        /// <param name="p_id">Identificador do botão.</param>
        /// <param name="p_text">Texto do botão.</param>
        /// <param name="p_onclick">Função JavaScript chamada quando o usuário clicar no botão.</param>
        public void AddButton(string p_id, string p_text, string p_onclick)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl v_button;

            v_button = new System.Web.UI.HtmlControls.HtmlGenericControl("button");
            v_button.ID = p_id;
            v_button.Attributes.Add("class", "pure-button pure-button-primary");
            v_button.Attributes.Add("onclick", p_onclick);
            v_button.InnerHtml = p_text;

            this.v_list.Add(v_button);
        }

        /// <summary>
        /// Adiciona um botão à lista de botões.
        /// </summary>
        /// <param name="p_id">Identificador do botão.</param>
        /// <param name="p_text">Texto do botão.</param>
        /// <param name="p_icon">Ícone do botão.</param>
        /// <param name="p_onclick">Função JavaScript chamada quando o usuário clicar no botão.</param>
        public void AddButton(string p_id, string p_text, string p_icon, string p_onclick)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl v_button;

            v_button = new System.Web.UI.HtmlControls.HtmlGenericControl("button");
            v_button.ID = p_id;
            v_button.Attributes.Add("class", "pure-button pure-button-primary");
            v_button.Attributes.Add("onclick", p_onclick);
            v_button.InnerHtml = "<i class='" + p_icon + "'></i>&nbsp;&nbsp;" + p_text;

            this.v_list.Add(v_button);
        }

        /// <summary>
        /// Renderiza o HTML do Container.
        /// </summary>
        public override string Render()
        {
            string v_html;
            System.Text.StringBuilder v_builder;
            System.Web.UI.HtmlTextWriter v_writer;

            v_builder = new System.Text.StringBuilder();
            v_writer = new System.Web.UI.HtmlTextWriter(new System.IO.StringWriter(v_builder));

            for (int k = 0; k < this.v_list.Count; k++)
                ((System.Web.UI.HtmlControls.HtmlGenericControl)this.v_list[k]).RenderControl(v_writer);

            v_html = "<div class='pure-controls'>";
            v_html += v_builder.ToString();
            v_html += "</div>";

            return v_html;
        }
    }
}

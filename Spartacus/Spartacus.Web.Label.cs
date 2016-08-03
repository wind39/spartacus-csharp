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
    /// Classe Label.
    /// Representa um componente que apenas mostra texto para o usuário.
    /// </summary>
    public class Label : Spartacus.Web.Container
    {
        /// <summary>
        /// Texto a ser exibido para o usuário.
        /// </summary>
        public string v_text;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Label"/>.
        /// </summary>
        /// <param name="p_id">Identificador do Container atual.</param>
        /// <param name="p_parent">Container pai.</param>
        public Label(string p_id, Spartacus.Web.Container p_parent)
            : base(p_id, p_parent)
        {
            this.v_type = Spartacus.Web.ContainerType.LABEL;
            this.v_text = "";
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Label"/>.
        /// </summary>
        /// <param name="p_id">Identificador do Container atual.</param>
        /// <param name="p_label">Texto a ser exibido para o usuário.</param>
        /// <param name="p_parent">Container pai.</param>
        public Label(string p_id, string p_text, Spartacus.Web.Container p_parent)
            : base(p_id, p_parent)
        {
            this.v_type = Spartacus.Web.ContainerType.LABEL;
            this.v_text = p_text;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            this.v_text = "";
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
        }

        /// <summary>
        /// Informa o texto ou valor a ser mostrado no Label.
        /// Usado para mostrar ao usuário um formulário já preenchido.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Label.</param>
        public override void SetValue(string p_text)
        {
            this.v_text = p_text;
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Label.
        /// </summary>
        /// <returns>Texto ou valor atual do Label.</returns>
        public override string GetValue()
        {
            return this.v_text;
        }

        /// <summary>
        /// Renderiza o HTML do Container.
        /// </summary>
        public override string Render()
        {
            string v_html;

            v_html = "<div class='pure-control-group'>";
            v_html += this.v_text.Replace("\n", "</br>");
            v_html += "</div>";

            return v_html;
        }
    }
}

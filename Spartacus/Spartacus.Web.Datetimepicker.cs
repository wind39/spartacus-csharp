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
    /// Classe Datetimepicker.
    /// RRepresenta um componente em que o usuário pode selecionar uma data.
    /// </summary>
    public class Datetimepicker : Spartacus.Web.Container
    {
        /// <summary>
        /// Rótulo do Datetimepicker.
        /// </summary>
        public System.Web.UI.HtmlControls.HtmlGenericControl v_label;

        /// <summary>
        /// Controle nativo que representa o Datetimepicker.
        /// </summary>
        public System.Web.UI.HtmlControls.HtmlGenericControl v_datetimepicker;

        /// <summary>
        /// Formato de Data.
        /// </summary>
        public string v_format;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Datetimepicker"/>.
        /// </summary>
        /// <param name="p_id">Identificador do Container atual.</param>
        /// <param name="p_label">Texto exibido no rótulo.</param>
        /// <param name="p_parent">Container pai.</param>
        public Datetimepicker(string p_id, string p_label, Spartacus.Web.Container p_parent)
            : base(p_id, p_parent)
        {
            this.v_label = new System.Web.UI.HtmlControls.HtmlGenericControl("label");
            this.v_label.Attributes.Add("for", p_id);
            this.v_label.InnerText = p_label;

            this.v_datetimepicker = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
            this.v_datetimepicker.ID = p_id;
            this.v_datetimepicker.Attributes.Add("runat", "server");
            this.v_datetimepicker.Attributes.Add("type", "text");
            this.v_datetimepicker.Attributes.Add("value", "");

            this.v_format = "YYYY-MM-DD";
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Datetimepicker"/>.
        /// </summary>
        /// <param name="p_id">Identificador do Container atual.</param>
        /// <param name="p_label">Texto exibido no rótulo.</param>
        /// <param name="p_format">Formato de data.</param>
        /// <param name="p_parent">Container pai.</param>
        public Datetimepicker(string p_id, string p_label, string p_format, Spartacus.Web.Container p_parent)
            : base(p_id, p_parent)
        {
            this.v_label = new System.Web.UI.HtmlControls.HtmlGenericControl("label");
            this.v_label.Attributes.Add("for", p_id);
            this.v_label.InnerText = p_label;

            this.v_datetimepicker = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
            this.v_datetimepicker.ID = p_id;
            this.v_datetimepicker.Attributes.Add("runat", "server");
            this.v_datetimepicker.Attributes.Add("type", "text");
            this.v_datetimepicker.Attributes.Add("value", "");

            this.v_format = p_format;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            this.v_datetimepicker.Attributes["value"] = "";
        }

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public override void Refresh()
        {
        }

        /// <summary>
        /// Informa o texto ou valor a ser mostrado no Datetimepicker.
        /// Usado para mostrar ao usuário um formulário já preenchido.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Datetimepicker.</param>
        public override void SetValue(string p_text)
        {
            this.v_datetimepicker.Attributes["value"] = p_text;
        }

        /// <summary>
        /// Retorna o texto ou valor atual do Datetimepicker.
        /// </summary>
        /// <returns>Texto ou valor atual do Datetimepicker.</returns>
        public override string GetValue()
        {
            return this.v_datetimepicker.Attributes["value"];
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

            this.v_label.RenderControl(v_writer);
            this.v_datetimepicker.RenderControl(v_writer);

            v_html = "<div class='pure-control-group'>";
            v_html += v_builder.ToString();
            v_html += "<script src='../js/moment.min.js'></script><script src='../js/pikaday.js'></script><script>var picker_" + this.v_id + " = new Pikaday({field: document.getElementById('" + this.v_id + "'), onSelect: function() {document.getElementById('" + this.v_id + "').value = picker_" + this.v_id + ".toString('" + this.v_format + "');}});</script>";
            v_html += "</div>";

            return v_html;
        }
    }
}

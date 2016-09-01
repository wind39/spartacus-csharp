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
        /// Nome da página ASPX.
        /// </summary>
        public string v_aspx;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Window"/>.
        /// </summary>
        /// <param name="p_aspx">Nome da página ASPX.</param>
        public Window(string p_aspx)
            :base(null)
        {
            this.v_type = Spartacus.Web.ContainerType.WINDOW;
            this.v_aspx = p_aspx;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Window"/>.
        /// </summary>
        /// <param name="p_title">Título da Janela</param>
        /// <param name="p_aspx">Nome da página ASPX.</param>
        public Window(string p_title, string p_aspx)
            :base(p_title)
        {
            this.v_type = Spartacus.Web.ContainerType.WINDOW;
            this.v_aspx = p_aspx;
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public override void Clear()
        {
            Spartacus.Web.Container v_container;

            for (int k = 0; k < this.v_containers.Count; k++)
            {
                v_container = this.v_containers[k];
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
                v_container = this.v_containers[k];
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
                v_container = this.v_containers[k];
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
            return "<!DOCTYPE html><html><head>" + this.RenderHead() + "</head><body>" + this.RenderBody() + "</body></html>";
        }

        /// <summary>
        /// Renderiza o HTML head.
        /// </summary>
        /// <returns>String com o HTML head.</returns>
        public string RenderHead()
        {
            string v_html;
            Spartacus.Web.Container v_container;
            Spartacus.Web.Buttons v_buttons;
            Spartacus.Web.Progressbar v_progressbar;
            System.Web.UI.HtmlControls.HtmlGenericControl v_button;
            string v_onclick, v_function, v_parameters;
            bool v_has_buttons = false;
            bool v_has_datetimepicker = false;
            bool v_has_grid = false;
            bool v_has_progressbar = false;
            string[] v_tmp;

            for (int i = 0; i < this.v_containers.Count; i++)
            {
                v_container = this.v_containers[i];
                switch (v_container.v_type)
                {
                    case Spartacus.Web.ContainerType.BUTTONS:
                        v_has_buttons = true;
                        break;
                    case Spartacus.Web.ContainerType.DATETIMEPICKER:
                        v_has_datetimepicker = true;
                        break;
                    case Spartacus.Web.ContainerType.GRID:
                        v_has_grid = true;
                        break;
                    case Spartacus.Web.ContainerType.PROGRESSBAR:
                        v_has_progressbar = true;
                        break;
                    default:
                        break;
                }
            }

            v_html = "<title>" + this.v_id + "</title>";

            if (v_has_datetimepicker)
                v_html += "<link rel='stylesheet' type='text/css' media='screen' href='../css/pikaday.css' />";

            if (v_has_grid)
                v_html += "<link rel='stylesheet' type='text/css' media='screen' href='../css/jquery.dataTables.min.css' />";

            v_html += "<link rel='stylesheet' type='text/css' media='screen' href='../css/font-awesome.min.css' />";
            v_html += "<link rel='stylesheet' type='text/css' media='screen' href='../css/pure-min.css' />";
            v_html += "<script type='text/javascript' src='../js/jquery.min.js'></script>";
            v_html += "<script type='text/javascript' src='../js/notify.min.js'></script>";

            if (v_has_datetimepicker)
            {
                v_html += "<script type='text/javascript' src='../js/moment.min.js'></script>";
                v_html += "<script type='text/javascript' src='../js/pikaday.js'></script>";
            }

            if (v_has_grid || v_has_progressbar)
            {
                if (v_has_grid)
                    v_html += "<script type='text/javascript' src='../js/jquery.dataTables.min.js'></script>";

                v_html += "<script type='text/javascript' class='init'> $(document).ready(function () { ";
                for (int i = 0; i < this.v_containers.Count; i++)
                {
                    v_container = this.v_containers[i];
                    switch (v_container.v_type)
                    {
                        case Spartacus.Web.ContainerType.GRID:
                            v_html += "var table_" + v_container.v_id + " = $('#grid_" + v_container.v_id + "').DataTable({stateSave: true, language: {decimal: ',', url: 'lang/portuguese_brasil.json'}}); ";
                            v_html += "$('#grid_" + v_container.v_id + " tbody').on('click', 'tr', function () { ";
                            v_html += "if ($(this).hasClass('selected')) { $(this).removeClass('selected'); $('#" + v_container.v_id + "').attr('value', ''); } ";
                            v_html += "else { table_" + v_container.v_id + ".$('tr.selected').removeClass('selected'); $(this).addClass('selected'); $('#" + v_container.v_id + "').attr('value', $(this).children('td:first').text()); }}); ";
                            break;
                        case Spartacus.Web.ContainerType.PROGRESSBAR:
                            v_progressbar = (Spartacus.Web.Progressbar)v_container;
                            v_html += "update_" + v_progressbar.v_id + "(); ";
                            v_html += "setInterval(update_" + v_progressbar.v_id + ", " + v_progressbar.v_interval.ToString() + "); ";
                            break;
                        default:
                            break;
                    }
                }
                v_html += "}); </script>";
            }

            if (v_has_progressbar)
            {
                v_html += "<script type='text/javascript'>";

                for (int i = 0; i < this.v_containers.Count; i++)
                {
                    v_container = this.v_containers[i];
                    if (v_container.v_type == Spartacus.Web.ContainerType.PROGRESSBAR)
                    {
                        v_progressbar = (Spartacus.Web.Progressbar)v_container;
                        v_html += " function update_" + v_progressbar.v_id + "() {";
                        v_html += "$.ajax({type: 'POST', url: '" + this.v_aspx + "/" + v_progressbar.v_webmethod + "', data: null, ";
                        //TODO: melhorar success e failure e implementar notificações
                        v_html += "contentType: 'application/json; charset=utf-8', dataType: 'json', ";
                        v_html += "success: function(response) { document.getElementById('" + v_progressbar.v_id +"').innerHTML = response.d; }, ";
                        v_html += "failure: function(response) { alert(response.d); } }); }";
                    }
                }

                v_html += "</script>";
            }

            if (v_has_buttons)
            {
                v_html += "<script type='text/javascript'>";

                for (int i = 0; i < this.v_containers.Count; i++)
                {
                    v_container = this.v_containers[i];
                    if (v_container.v_type == Spartacus.Web.ContainerType.BUTTONS)
                    {
                        v_buttons = (Spartacus.Web.Buttons)v_container;
                        for (int j = 0; j < v_buttons.v_list.Count; j++)
                        {
                            v_button = (System.Web.UI.HtmlControls.HtmlGenericControl)v_buttons.v_list[j];

                            v_onclick = v_button.Attributes["onclick"].Replace(" ", "");
                            v_tmp = v_onclick.Split('(');
                            v_function = v_tmp[0];

                            v_html += " function " + v_function + "() {";
                            v_html += "$.ajax({type: 'POST', url: '" + this.v_aspx + "/" + v_function + "', ";

                            v_parameters = v_tmp[1].Replace(")", "");
                            if (v_parameters != null && v_parameters != "")
                            {
                                v_tmp = v_parameters.Split(',');
                                if (v_buttons.v_arrayparams)
                                {
                                    v_html += "data: JSON.stringify({p_array: [document.getElementById('" + v_tmp[0] + "').value";
                                    for (int k = 1; k < v_tmp.Length; k++)
                                        v_html += ", document.getElementById('" + v_tmp[k] + "').value";
                                    v_html += "]}), ";
                                }
                                else
                                {
                                    v_html += "data: JSON.stringify({p_" + v_tmp[0] + ": document.getElementById('" + v_tmp[0] + "').value";
                                    for (int k = 1; k < v_tmp.Length; k++)
                                        v_html += ", p_" + v_tmp[k] + ": document.getElementById('" + v_tmp[k] + "').value";
                                    v_html += "}), ";
                                }
                            }
                            else
                                v_html += "data: null, ";

                            //TODO: melhorar success e failure e implementar notificações
                            v_html += "contentType: 'application/json; charset=utf-8', dataType: 'json', ";
                            v_html += "success: function(response) { if (response.d.length > 0) window.location = response.d; }, ";
                            v_html += "failure: function(response) { alert(response.d); } }); }";
                        }
                    }
                }

                v_html += "</script>";
            }

            return v_html;
        }

        /// <summary>
        /// Renderiza o HTML body.
        /// </summary>
        /// <returns>String com o HTML body.</returns>
        public string RenderBody()
        {
            string v_html;
            Spartacus.Web.Container v_container;

            v_html = "<form class='pure-form pure-form-aligned' id='spartacus_window' runat='server'><fieldset>";
            for (int k = 0; k < this.v_containers.Count; k++)
            {
                v_container = this.v_containers[k];
                v_html += v_container.Render();
            }
            v_html += "</fieldset></form>";

            return v_html;
        }
    }
}

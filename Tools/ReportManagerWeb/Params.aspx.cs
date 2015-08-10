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
using System.Web;
using System.Web.UI;
using Spartacus;

namespace Spartacus.Tools.ReportManagerWeb
{
    public partial class Params : System.Web.UI.Page
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time.
        }

        public string Render()
        {
            Spartacus.Database.Generic v_database;
            Spartacus.Reporting.Report v_report;
            Spartacus.Web.Window v_window;
            Spartacus.Web.Textbox v_textbox;
            Spartacus.Web.Datetimepicker v_datetimepicker;
            Spartacus.Web.Label v_label;
            Spartacus.Web.Grid v_grid;
            Spartacus.Web.Buttons v_buttons;
            string v_paramnames;

            // conectando-se ao banco de dados, se necessário
            v_database = (Spartacus.Database.Sqlite)this.Session["DATABASE"];
            if (v_database == null)
            {
                v_database = new Spartacus.Database.Sqlite(System.Web.Configuration.WebConfigurationManager.AppSettings["database"].ToString());
                this.Session["DATABASE"] = v_database;
            }

            // criando relatório, se necessário
            v_report = (Spartacus.Reporting.Report)this.Session["REPORT"];
            if (v_report == null)
            {
                this.Session["XML"] = v_database.ExecuteScalar("select xmlfile from reports where code = " + this.Session["ID"]);
                v_report = new Spartacus.Reporting.Report(
                    int.Parse(this.Session["ID"].ToString()),
                    this.Session["XML"].ToString()
                );
                this.Session["REPORT"] = v_report;
            }

            // criando janela de parâmetros, se necessário
            v_window = (Spartacus.Web.Window)this.Session["PARAMWINDOW"];
            if (v_window == null)
            {
                v_window = new Spartacus.Web.Window(System.Web.Configuration.WebConfigurationManager.AppSettings["param.title"].ToString(), "Params.aspx");

                // percorrendo todos os parâmetros do relatório
                v_paramnames = "";
                for (int k = 0; k < v_report.v_cmd.v_parameters.Count; k++)
                {
                    if (((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_type == Spartacus.Database.Type.DATE)
                    {
                        v_datetimepicker = new Spartacus.Web.Datetimepicker(
                            ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_name.ToLower(),
                            ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_description,
                            v_window
                        );
                        v_window.Add(v_datetimepicker);
                    }
                    else
                    {
                        if (((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_lookup != null &&
                            ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_lookup != "")
                        {
                            v_label = new Spartacus.Web.Label("label_" + ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_name.ToLower(), ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_description, v_window);
                            v_window.Add(v_label);

                            v_grid = new Spartacus.Web.Grid(
                                ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_name.ToLower(),
                                v_window
                            );
                            v_grid.Populate(v_report.v_database, ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_lookup);
                            v_window.Add(v_grid);
                        }
                        else
                        {
                            v_textbox = new Spartacus.Web.Textbox(
                                ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_name.ToLower(),
                                ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_description,
                                v_window
                            );
                            v_window.Add(v_textbox);
                        }
                    }

                    v_paramnames += ((Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k]).v_name.ToLower() + ",";
                }
                v_paramnames = v_paramnames.Substring(0, v_paramnames.Length - 1);

                v_buttons = new Spartacus.Web.Buttons(v_window);
                v_buttons.AddButton("back", System.Web.Configuration.WebConfigurationManager.AppSettings["param.back"].ToString(), "fa fa-reply", "OnBackClick()");
                v_buttons.AddButton("execute", System.Web.Configuration.WebConfigurationManager.AppSettings["param.execute"].ToString(), "fa fa-bolt", "OnExecuteClick(" + v_paramnames + ")", true);
                v_window.Add(v_buttons);

                this.Session["PARAMWINDOW"] = v_window;
            }

            return v_window.Render();
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string OnExecuteClick(System.Collections.Generic.List<string> p_array)
        {
            Spartacus.Reporting.Report v_report;

            v_report = (Spartacus.Reporting.Report)System.Web.HttpContext.Current.Session["REPORT"];

            for (int k = 0; k < v_report.v_cmd.v_parameters.Count; k++)
                v_report.v_cmd.SetValue(k, p_array[k]);

            System.Web.HttpContext.Current.Session["REPORT"] = v_report;

            return "Execute.aspx";
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string OnBackClick()
        {
            System.Web.HttpContext.Current.Session["PARAMWINDOW"] = null;
            System.Web.HttpContext.Current.Session["REPORT"] = null;
            return "Default.aspx";
        }
    }
}

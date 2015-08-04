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
    public partial class Default : System.Web.UI.Page
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time.
        }

        public string Render()
        {
            Spartacus.Database.Generic v_database;
            Spartacus.Web.Window v_window;
            Spartacus.Web.Label v_label;
            Spartacus.Web.Grid v_grid;
            Spartacus.Web.Buttons v_buttons;

            // criando banco de dados, se necessário, e conectando-se
            v_database = (Spartacus.Database.Sqlite)this.Session["DATABASE"];
            if (v_database == null)
            {
                if (new System.IO.FileInfo(System.Web.Configuration.WebConfigurationManager.AppSettings["database"].ToString()).Exists)
                    v_database = new Spartacus.Database.Sqlite(System.Web.Configuration.WebConfigurationManager.AppSettings["database"].ToString());
                else
                {
                    v_database = new Spartacus.Database.Sqlite();
                    v_database.CreateDatabase(System.Web.Configuration.WebConfigurationManager.AppSettings["database"].ToString());
                    v_database.Execute("create table reports (code integer primary key autoincrement, name text, xmlfile text not null);");
                }
                this.Session["DATABASE"] = v_database;
            }

            // construindo janela principal, se necessário
            v_window = (Spartacus.Web.Window)this.Session["MAINWINDOW"];
            if (v_window == null)
            {
                v_window = new Spartacus.Web.Window(System.Web.Configuration.WebConfigurationManager.AppSettings["main.title"].ToString(), "Default.aspx");

                v_label = new Spartacus.Web.Label("label", "Escolha um relatório na tabela abaixo:", v_window);
                v_window.Add(v_label);

                v_grid = new Spartacus.Web.Grid("report", v_window);
                v_grid.Populate(v_database, "select * from reports");
                v_window.Add(v_grid);

                v_buttons = new Spartacus.Web.Buttons(v_window);
                v_buttons.AddButton("execute", System.Web.Configuration.WebConfigurationManager.AppSettings["main.execute"].ToString(), "fa fa-cog", "OnExecuteClick(report)");
                v_window.Add(v_buttons);

                this.Session["MAINWINDOW"] = v_window;
            }

            return v_window.Render();
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string OnExecuteClick(string p_report)
        {
            Spartacus.Web.Window v_window;

            v_window = (Spartacus.Web.Window) System.Web.HttpContext.Current.Session["MAINWINDOW"];
            v_window.GetChildById("report").SetValue(p_report);

            System.Web.HttpContext.Current.Session["MAINWINDOW"] = v_window;
            System.Web.HttpContext.Current.Session["ID"] = p_report;

            return "Params.aspx";
        }
    }
}


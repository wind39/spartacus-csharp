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
    public partial class ErrorMessage : System.Web.UI.Page
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time.
        }

        public string Render()
        {
            Spartacus.Web.Window v_window;
            Spartacus.Web.Label v_label;
            Spartacus.Web.Buttons v_buttons;

            // construindo janela principal, se necessário
            v_window = (Spartacus.Web.Window)this.Session["ERRORWINDOW"];
            if (v_window == null)
            {
                v_window = new Spartacus.Web.Window(System.Web.Configuration.WebConfigurationManager.AppSettings["error.title"].ToString(), "ErrorMessage.aspx");

                v_label = new Spartacus.Web.Label("label", this.Session["ERRORMESSAGE"].ToString(), v_window);
                v_window.Add(v_label);

                v_buttons = new Spartacus.Web.Buttons(v_window);
                v_buttons.AddButton("back", System.Web.Configuration.WebConfigurationManager.AppSettings["error.back"].ToString(), "fa fa-thumbs-o-down", "OnBackClick()");
                v_window.Add(v_buttons);

                this.Session["ERRORWINDOW"] = v_window;
            }

            return v_window.Render();
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string OnBackClick()
        {
            System.Web.HttpContext.Current.Session["ERRORWINDOW"] = null;
            System.Web.HttpContext.Current.Session["ERRORMESSAGE"] = null;

            return "Default.aspx";
        }
    }
}


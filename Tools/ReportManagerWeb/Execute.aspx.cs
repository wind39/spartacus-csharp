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
    public partial class Execute : System.Web.UI.Page
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time.
        }

        public string Render()
        {
            Spartacus.Web.Window v_window;
            Spartacus.Web.Label v_label;
            Spartacus.Web.Progressbar v_progressbar;
            Spartacus.Web.Buttons v_buttons;
            //System.Threading.Thread v_thread;
            Spartacus.Reporting.Report v_report;
            Spartacus.Database.Parameter v_parameter;
            Spartacus.Net.Cryptor v_cryptor;
            Spartacus.Net.Client v_client;
            Spartacus.Net.Packet v_packet;
            string v_options;

            // criando janela de parâmetros, se necessário
            v_window = (Spartacus.Web.Window)this.Session["EXECUTEWINDOW"];
            if (v_window == null)
            {
                v_window = new Spartacus.Web.Window(System.Web.Configuration.WebConfigurationManager.AppSettings["execute.title"].ToString(), "Execute.aspx");

                v_label = new Spartacus.Web.Label("label", "Renderizando relatório em PDF. Aguarde...", v_window);
                v_window.Add(v_label);

                v_progressbar = new Spartacus.Web.Progressbar("progress", Spartacus.Web.ProgressbarSize.BIG, 1000, "OnProgress", v_window);
                v_window.Add(v_progressbar);

                v_buttons = new Spartacus.Web.Buttons(v_window);
                v_buttons.AddButton("back", System.Web.Configuration.WebConfigurationManager.AppSettings["execute.back"].ToString(), "fa fa-reply", "OnBackClick()");
                v_buttons.AddButton("download", System.Web.Configuration.WebConfigurationManager.AppSettings["execute.download"].ToString(), "fa fa-download", "OnDownloadClick()");
                v_window.Add(v_buttons);

                this.Session["EXECUTEWINDOW"] = v_window;
            }

            //v_thread = new System.Threading.Thread(this.Thread);
            //v_thread.Start();

            //this.Session["THREAD"] = v_thread;
            //this.Session["CANRENDER"] = "true";

            v_report = (Spartacus.Reporting.Report)this.Session["REPORT"];

            v_cryptor = new Spartacus.Net.Cryptor("spartacus");
            this.Session["FILENAME"] = "tmp/" + v_cryptor.RandomString() + ".pdf";

            v_options = "R;" + this.Session["ID"].ToString() + ";../" + this.Session["XML"].ToString() + ";../" + this.Session["FILENAME"].ToString() + ";";
            if (v_report.v_cmd.v_parameters.Count > 0)
            {
                v_parameter = (Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[0];
                v_options += v_parameter.v_name + "=" + v_parameter.v_value;
                for (int k = 1; k < v_report.v_cmd.v_parameters.Count; k++)
                {
                    v_parameter = (Spartacus.Database.Parameter)v_report.v_cmd.v_parameters[k];
                    v_options += "," + v_parameter.v_name + "=" + v_parameter.v_value;
                }
            }

            v_client = new Spartacus.Net.Client(
                System.Web.Configuration.WebConfigurationManager.AppSettings["serverip"].ToString(),
                int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["serverport"].ToString()),
                System.Web.Configuration.WebConfigurationManager.AppSettings["clientip"].ToString(),
                int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["clientport"].ToString())
            );
            v_client.Connect();
            v_client.SendString(v_options);
            this.Session["CLIENT"] = v_client;

            v_packet = v_client.Recv();
            if (v_packet.v_type != Spartacus.Net.PacketType.ACK)
            {
                this.Session["ERRORMESSAGE"] = "Impossível renderizar relatório!</br>Servidor de relatórios não reconheceu o relatório " + this.Session["ID"].ToString() + "!";
                this.Response.Redirect("ErrorMessage.aspx");
            }

            return v_window.Render();
        }

        /*public void Thread()
        {
            Spartacus.Reporting.Report v_report;
            Spartacus.Web.Window v_window;
            Spartacus.Net.Cryptor v_cryptor;
            string v_canrender;

            v_canrender = this.Session["CANRENDER"].ToString();
            if (v_canrender == null || v_canrender == "true")
            {
                v_report = (Spartacus.Reporting.Report)this.Session["REPORT"];
                v_report.v_progress.ProgressEvent += new Spartacus.Utils.ProgressEventClass.ProgressEventHandler(this.OnReportProgress);
                v_report.Execute();

                v_cryptor = new Spartacus.Net.Cryptor("spartacus");

                v_window = (Spartacus.Web.Window)this.Session["EXECUTEWINDOW"];

                if (v_report.v_table.Rows.Count == 0)
                    ((Spartacus.Web.Progressbar)v_window.GetChildById("progress")).SetValue("O relatório não foi salvo em PDF porque não tem dados.", 100);
                else
                {
                    this.Session["FILENAME"] = "tmp/" + v_cryptor.RandomString() + ".pdf";
                    v_report.Save(this.Session["FILENAME"].ToString());
                    ((Spartacus.Web.Progressbar)v_window.GetChildById("progress")).SetValue("Pronto! Clique em Download para baixar o PDF.", 100);
                }

                this.Session["EXECUTEWINDOW"] = v_window;
            }
        }

        public void OnReportProgress(Spartacus.Utils.ProgressEventClass sender, Spartacus.Utils.ProgressEventArgs e)
        {
            Spartacus.Web.Window v_window;

            v_window = (Spartacus.Web.Window)this.Session["EXECUTEWINDOW"];
            ((Spartacus.Web.Progressbar)v_window.GetChildById("progress")).SetValue(e.v_message, (int)e.v_percentage);
            this.Session["EXECUTEWINDOW"] = v_window;
        }*/

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string OnProgress()
        {
            Spartacus.Web.Window v_window;
            Spartacus.Net.Client v_client;
            string v_message;
            string[] v_messageparts;

            v_window = (Spartacus.Web.Window)System.Web.HttpContext.Current.Session["EXECUTEWINDOW"];
            v_client = (Spartacus.Net.Client)System.Web.HttpContext.Current.Session["CLIENT"];

            v_client.SendString("P");
            v_message = v_client.RecvString();

            v_messageparts = v_message.Split(';');
            if (v_messageparts.Length == 3)
            {
                ((Spartacus.Web.Progressbar)v_window.GetChildById("progress")).SetValue(
                    v_messageparts[1],
                    int.Parse(v_messageparts[0]),
                    bool.Parse(v_messageparts[2])
                );
            }

            System.Web.HttpContext.Current.Session["EXECUTEWINDOW"] = v_window;

            return ((Spartacus.Web.Progressbar)v_window.GetChildById("progress")).RenderInner();
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string OnBackClick()
        {
            Spartacus.Net.Client v_client;

            v_client = (Spartacus.Net.Client) System.Web.HttpContext.Current.Session["CLIENT"];
            v_client.Stop();

            System.Web.HttpContext.Current.Session["EXECUTEWINDOW"] = null;
            System.Web.HttpContext.Current.Session["REPORT"] = null;
            System.Web.HttpContext.Current.Session["CLIENT"] = null;
            return "Params.aspx";
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string OnDownloadClick()
        {
            //System.Threading.Thread v_thread;

            //v_thread = (System.Threading.Thread)System.Web.HttpContext.Current.Session["THREAD"];
            //v_thread.Abort();

            return "Download.aspx";
        }
    }
}

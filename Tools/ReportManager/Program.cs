/*
The MIT License (MIT)

Copyright (c) 2014-2016 William Ivanski

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

namespace Spartacus.Tools.ReportManager
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            (new MainClass()).Initialize();
        }

        private Spartacus.Database.Generic v_database;
        private Spartacus.Reporting.Report v_report;

        private Spartacus.Forms.Window v_mainwindow;
        private Spartacus.Forms.Grid v_maingrid;
        private Spartacus.Forms.Buttons v_mainbuttons;

        private Spartacus.Forms.Window v_reportwindow;
        private Spartacus.Forms.Textbox v_reportname;
        private Spartacus.Forms.Filepicker v_reportxmlfile;
        private Spartacus.Forms.Buttons v_reportbuttons;

        private Spartacus.Forms.Window v_paramwindow;
        private Spartacus.Forms.Buttons v_parambuttons;
        private Spartacus.Forms.Filepicker v_reportpdffile;
        private Spartacus.Forms.Progressbar v_progressbar;

        private int v_currentcode;
        private string v_currentfile;

        private bool v_editable;


        public void Initialize()
        {
            try
            {
                if (new System.IO.FileInfo(System.Configuration.ConfigurationManager.AppSettings["database"].ToString()).Exists)
                    this.v_database = new Spartacus.Database.Sqlite(System.Configuration.ConfigurationManager.AppSettings["database"].ToString());
                else
                {
                    this.v_database = new Spartacus.Database.Sqlite();
                    this.v_database.CreateDatabase(System.Configuration.ConfigurationManager.AppSettings["database"].ToString());
                    this.v_database.Execute("create table reports (code integer primary key autoincrement, name text, xmlfile text not null);");
                }

                this.v_mainwindow = new Spartacus.Forms.Window(
                    System.Configuration.ConfigurationManager.AppSettings["main.title"].ToString(),
                    600,
                    400
                );

                this.v_maingrid = new Spartacus.Forms.Grid(this.v_mainwindow, 600, 300);
                this.v_maingrid.Populate(this.v_database, "select * from reports");
                this.v_mainwindow.Add(this.v_maingrid);

                this.v_editable = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["editable"].ToString());

                this.v_mainbuttons = new Spartacus.Forms.Buttons(this.v_mainwindow);
                this.v_mainbuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["main.execute"].ToString(), this.OnMainExecuteClick);
                this.v_mainbuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["main.insert"].ToString(), this.OnMainInsertClick, this.v_editable);
                this.v_mainbuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["main.update"].ToString(), this.OnMainUpdateClick, this.v_editable);
                this.v_mainbuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["main.delete"].ToString(), this.OnMainDeleteClick, this.v_editable);
                this.v_mainwindow.Add(this.v_mainbuttons);

                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.Run((System.Windows.Forms.Form) this.v_mainwindow.v_control);
            }
            catch(Spartacus.Database.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Database.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (Spartacus.Forms.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Forms.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (System.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.Message, exc.GetType().ToString(), Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
        }

        private void OnMainInsertClick(object sender, System.EventArgs e)
        {
            try
            {
                this.v_reportwindow = new Spartacus.Forms.Window(
                    System.Configuration.ConfigurationManager.AppSettings["reportinsert.title"].ToString(),
                    600,
                    150,
                    this.v_mainwindow
                );

                this.v_reportname = new Spartacus.Forms.Textbox(this.v_reportwindow, System.Configuration.ConfigurationManager.AppSettings["reportinsert.name"].ToString());
                this.v_reportwindow.Add(this.v_reportname);

                this.v_reportxmlfile = new Spartacus.Forms.Filepicker(
                    this.v_reportwindow,
                    Spartacus.Forms.Filepicker.Type.OPEN,
                    System.Configuration.ConfigurationManager.AppSettings["reportinsert.xmlfile"].ToString(),
                    System.Configuration.ConfigurationManager.AppSettings["reportinsert.filter"].ToString()
                );
                this.v_reportwindow.Add(this.v_reportxmlfile);

                this.v_reportbuttons = new Spartacus.Forms.Buttons(this.v_reportwindow);
                this.v_reportbuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["reportinsert.save"].ToString(), this.OnReportInsertSaveClick);
                this.v_reportbuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["reportinsert.cancel"].ToString(), this.OnReportInsertCancelClick);
                this.v_reportwindow.Add(this.v_reportbuttons);

                this.v_reportwindow.Show();
            }
            catch (Spartacus.Forms.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Forms.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (System.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.Message, exc.GetType().ToString(), Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
        }

        private void OnReportInsertSaveClick(object sender, System.EventArgs e)
        {
            try
            {
                this.v_database.Execute("insert into reports (name, xmlfile) values ('" + this.v_reportname.GetValue() + "', '" + this.v_reportxmlfile.GetValue() + "');");
                this.v_reportwindow.Hide();
                this.v_maingrid.Refresh();
            }
            catch(Spartacus.Database.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Database.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
        }

        private void OnReportInsertCancelClick(object sender, System.EventArgs e)
        {
            this.v_reportwindow.Hide();
        }

        private void OnMainUpdateClick(object sender, System.EventArgs e)
        {
            System.Data.DataRow v_row = this.v_maingrid.CurrentRow();

            this.v_reportwindow = new Spartacus.Forms.Window(
                System.Configuration.ConfigurationManager.AppSettings["reportupdate.title"].ToString(),
                600,
                150,
                this.v_mainwindow
            );

            this.v_reportname = new Spartacus.Forms.Textbox(this.v_reportwindow, System.Configuration.ConfigurationManager.AppSettings["reportupdate.name"].ToString());
            this.v_reportname.SetValue(v_row["name"].ToString());
            this.v_reportwindow.Add(this.v_reportname);

            this.v_reportxmlfile = new Spartacus.Forms.Filepicker(
                this.v_reportwindow,
                Spartacus.Forms.Filepicker.Type.OPEN,
                System.Configuration.ConfigurationManager.AppSettings["reportupdate.xmlfile"].ToString(),
                System.Configuration.ConfigurationManager.AppSettings["reportupdate.filter"].ToString()
            );
            this.v_reportxmlfile.SetValue(v_row["xmlfile"].ToString());
            this.v_reportwindow.Add(this.v_reportxmlfile);

            this.v_reportbuttons = new Spartacus.Forms.Buttons(this.v_reportwindow);
            this.v_reportbuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["reportupdate.save"].ToString(), this.OnReportUpdateSaveClick);
            this.v_reportbuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["reportupdate.cancel"].ToString(), this.OnReportUpdateCancelClick);
            this.v_reportwindow.Add(this.v_reportbuttons);

            this.v_reportwindow.Show();
        }

        private void OnReportUpdateSaveClick(object sender, System.EventArgs e)
        {
            try
            {
                this.v_database.Execute("update reports set name = '" + this.v_reportname.GetValue() + "', xmlfile = '" + this.v_reportxmlfile.GetValue() + "';");
                this.v_reportwindow.Hide();
                this.v_maingrid.Refresh();
            }
            catch(Spartacus.Database.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Database.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
        }

        private void OnReportUpdateCancelClick(object sender, System.EventArgs e)
        {
            this.v_reportwindow.Hide();
        }

        private void OnMainDeleteClick(object sender, System.EventArgs e)
        {
            System.Data.DataRow v_row = this.v_maingrid.CurrentRow();

            try
            {
                this.v_database.Execute("delete from reports where code = " + v_row["code"].ToString());
                this.v_reportwindow.Hide();
                this.v_maingrid.Refresh();
            }
            catch(Spartacus.Database.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Database.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
        }

        private void OnMainExecuteClick(object sender, System.EventArgs e)
        {
            System.Data.DataRow v_row = this.v_maingrid.CurrentRow();

            try
            {
                this.v_currentcode = int.Parse(v_row["code"].ToString());
                this.v_currentfile = v_row["xmlfile"].ToString();
                this.v_report = new Spartacus.Reporting.Report(this.v_currentcode, this.v_currentfile);

                this.v_paramwindow = new Spartacus.Forms.Window(
                    System.Configuration.ConfigurationManager.AppSettings["param.title"].ToString(),
                    500,
                    500,
                    this.v_mainwindow
                );

                for (int k = 0; k < this.v_report.v_cmd.v_parameters.Count; k++)
                {
                    if (((Spartacus.Database.Parameter)this.v_report.v_cmd.v_parameters[k]).v_type == Spartacus.Database.Type.DATE)
                    {
                        Spartacus.Forms.Datetimepicker v_param = new Spartacus.Forms.Datetimepicker(
                            this.v_paramwindow,
                            ((Spartacus.Database.Parameter)this.v_report.v_cmd.v_parameters[k]).v_description,
                            "dd/mm/yyyy"
                        );
                        this.v_paramwindow.Add(v_param);
                    }
                    else
                    {
                        if (((Spartacus.Database.Parameter)this.v_report.v_cmd.v_parameters[k]).v_lookup != null &&
                            ((Spartacus.Database.Parameter)this.v_report.v_cmd.v_parameters[k]).v_lookup != "")
                        {
                            Spartacus.Forms.Lookup v_param = new Spartacus.Forms.Lookup(
                                this.v_paramwindow,
                                ((Spartacus.Database.Parameter)this.v_report.v_cmd.v_parameters[k]).v_description
                            );
                            v_param.Populate(this.v_report.v_database, ((Spartacus.Database.Parameter)this.v_report.v_cmd.v_parameters[k]).v_lookup, "80;150");
                            this.v_paramwindow.Add(v_param);
                        }
                        else
                        {
                            Spartacus.Forms.Textbox v_param = new Spartacus.Forms.Textbox(
                                this.v_paramwindow,
                                ((Spartacus.Database.Parameter)this.v_report.v_cmd.v_parameters[k]).v_description
                            );
                            this.v_paramwindow.Add(v_param);
                        }
                    }
                }

                this.v_reportpdffile = new Spartacus.Forms.Filepicker(
                    this.v_paramwindow,
                    Spartacus.Forms.Filepicker.Type.SAVE,
                    System.Configuration.ConfigurationManager.AppSettings["param.pdffile"].ToString(),
                    System.Configuration.ConfigurationManager.AppSettings["param.filter"].ToString()
                );
                this.v_paramwindow.Add(this.v_reportpdffile);

                this.v_parambuttons = new Spartacus.Forms.Buttons(this.v_paramwindow);
                this.v_parambuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["param.execute"].ToString(), this.OnParamExecuteClick);
                this.v_parambuttons.AddButton(System.Configuration.ConfigurationManager.AppSettings["param.cancel"].ToString(), this.OnParamCancelClick);
                this.v_paramwindow.Add(this.v_parambuttons);

                this.v_progressbar = new Spartacus.Forms.Progressbar(this.v_paramwindow);
                this.v_progressbar.SetValue("Aguardando início", 0);
                this.v_paramwindow.Add(this.v_progressbar);

                this.v_paramwindow.Show();
            }
            catch (Spartacus.Database.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Database.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (Spartacus.Reporting.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Reporting.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (Spartacus.Forms.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Forms.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (System.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.Message, exc.GetType().ToString(), Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
        }

        private void OnParamExecuteClick(object sender, System.EventArgs e)
        {
            try
            {
                this.v_report = new Spartacus.Reporting.Report(this.v_currentcode, this.v_currentfile);

                for (int k = 0; k < this.v_report.v_cmd.v_parameters.Count; k++)
                    this.v_report.v_cmd.SetValue(k, ((Spartacus.Forms.Container)this.v_paramwindow.v_containers[k]).GetValue());

                this.v_report.v_progress.ProgressEvent += new Spartacus.Utils.ProgressEventClass.ProgressEventHandler(this.OnProgress);

                this.v_report.Execute();

                if (this.v_report.v_table.Rows.Count == 0)
                    Spartacus.Forms.Messagebox.Show("O relatorio nao foi salvo em PDF porque nao tem dados.", "Sem dados!", Spartacus.Forms.Messagebox.Icon.HAND);
                else
                    this.v_report.Save(this.v_reportpdffile.GetValue());

                this.v_progressbar.SetValue("Pronto!", 100);
            }
            catch (Spartacus.Database.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Database.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (Spartacus.Reporting.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Reporting.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (Spartacus.Forms.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.v_message, "Spartacus.Forms.Exception", Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
            catch (System.Exception exc)
            {
                Spartacus.Forms.Messagebox.Show(exc.Message, exc.GetType().ToString(), Spartacus.Forms.Messagebox.Icon.ERROR);
                System.Environment.Exit(-1);
            }
        }

        private void OnParamCancelClick(object sender, System.EventArgs e)
        {
            this.v_paramwindow.Hide();
            ((System.Windows.Forms.Button)this.v_mainbuttons.v_list[1]).Enabled = this.v_editable;
            ((System.Windows.Forms.Button)this.v_mainbuttons.v_list[2]).Enabled = this.v_editable;
            ((System.Windows.Forms.Button)this.v_mainbuttons.v_list[3]).Enabled = this.v_editable;
        }

        private void OnProgress(Spartacus.Utils.ProgressEventClass sender, Spartacus.Utils.ProgressEventArgs e)
        {
            this.v_progressbar.SetValue(e.v_message, (int)e.v_percentage);
        }
    }
}

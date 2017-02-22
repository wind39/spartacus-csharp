/*
The MIT License (MIT)

Copyright (c) 2014-2017 William Ivanski

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
using PDFjet;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Classe Package.
    /// Pode conter um ou mais relatórios em PDF, e salvá-los em arquivos separados ou no mesmo arquivo.
    /// </summary>
    public class Package
    {
        /// <summary>
        /// Lista de Relatórios.
        /// </summary>
        public System.Collections.Generic.List<Spartacus.Reporting.Report> v_reports;

        /// <summary>
        /// Lista de nomes de arquivos (opcional).
        /// </summary>
        public System.Collections.Generic.List<string> v_filenames;

        /// <summary>
        /// Nome do arquivo PDF de saída (opcional).
        /// </summary>
        public string v_filename;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Package"/>.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo PDF de saída.</param>
        public Package(string p_filename)
        {
            this.v_reports = new System.Collections.Generic.List<Spartacus.Reporting.Report>();
            this.v_filenames = new System.Collections.Generic.List<string>();
            this.v_filename = p_filename;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Package"/>.
        /// O arquivo PDF de saída é considerado opcionalmente como "output.pdf".
        /// </summary>
        public Package()
        {
            this.v_reports = new System.Collections.Generic.List<Spartacus.Reporting.Report>();
            this.v_filenames = new System.Collections.Generic.List<string>();
            this.v_filename = "output.pdf";
        }

        /// <summary>
        /// Adiciona um relatório associado a um nome de arquivo PDF de saída.
        /// </summary>
        /// <param name="p_report">Relatório.</param>
        /// <param name="p_filename">Nome do arquivo PDF de saída.</param>
        public void Add(Spartacus.Reporting.Report p_report, string p_filename)
        {
            this.v_reports.Add(p_report);
            this.v_filenames.Add(p_filename);
        }

        /// <summary>
        /// Adiciona um relatório.
        /// </summary>
        /// <param name="p_report">Relatório.</param>
        public void Add(Spartacus.Reporting.Report p_report)
        {
            this.v_reports.Add(p_report);
        }

        /// <summary>
        /// Processa todos os relatórios do pacote.
        /// </summary>
        public void Execute()
        {
            for (int k = 0; k < this.v_reports.Count; k++)
                this.v_reports[k].Execute();
        }

        /// <summary>
        /// Salva todos os relatórios em arquivos separados.
        /// É necessário que o atributo "v_filenames" tenha sido alimentado e possua o mesmo número de elementos.
        /// </summary>
        public void SaveSplitted()
        {
            Spartacus.Reporting.Report v_report;
            double v_perc, v_percstep, v_lastperc;

            try
            {
                v_perc = 0.0;
                v_percstep = 100.0 / (double) this.v_reports.Count;
                v_lastperc = v_percstep;

                for (int k = 0; k < this.v_reports.Count; k++)
                {
                    v_report = this.v_reports[k];

                    v_report.v_perc = v_perc;
                    v_report.v_percstep = v_percstep;
                    v_report.v_lastperc = v_lastperc;

                    v_report.Save(this.v_filenames[k]);

                    v_perc = v_lastperc;
                    v_lastperc += v_percstep;
                }
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o pacote PDF de saída.", e);
            }
        }

        /// <summary>
        /// Salva todos os relatórios em arquivos separados.
        /// </summary>
        /// <param name="p_filenames">Lista de nomes de arquivos, que deve possuir o mesmo número de elementos.</param>
        public void SaveSplitted(System.Collections.Generic.List<string> p_filenames)
        {
            Spartacus.Reporting.Report v_report;
            double v_perc, v_percstep, v_lastperc;

            try
            {
                v_perc = 0.0;
                v_percstep = 100.0 / (double) this.v_reports.Count;
                v_lastperc = v_percstep;

                for (int k = 0; k < this.v_reports.Count; k++)
                {
                    v_report = this.v_reports[k];

                    v_report.v_perc = v_perc;
                    v_report.v_percstep = v_percstep;
                    v_report.v_lastperc = v_lastperc;

                    v_report.Save(p_filenames[k]);

                    v_perc = v_lastperc;
                    v_lastperc += v_percstep;
                }
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o pacote PDF de saída.", e);
            }
        }

        /// <summary>
        /// Salva todos os relatórios em arquivos separados.
        /// É necessário que o atributo "v_filenames" tenha sido alimentado e possua o mesmo número de elementos.
        /// </summary>
        /// <param name="p_compress">Gerar arquivo ZIP com todos os arquivos PDF.</param>
        public void SaveSplitted(bool p_compress)
        {
            Spartacus.ThirdParty.ZipStorer v_zipstorer;
            Spartacus.Utils.Cryptor v_cryptor;
            string v_encrypted;
            Spartacus.Reporting.Report v_report;
            double v_perc, v_percstep, v_lastperc;

            try
            {
                if (p_compress)
                {
                    v_cryptor = new Spartacus.Utils.Cryptor("spartacus");

                    v_zipstorer = Spartacus.ThirdParty.ZipStorer.Create(this.v_filename.Replace(".pdf", ".zip"), "Generated with ZipStorer (by Jaime Olivares) embedded in Spartacus (by William Ivanski)");
                    v_zipstorer.EncodeUTF8 = true;

                    v_perc = 0.0;
                    v_percstep = 100.0 / (double) this.v_reports.Count;
                    v_lastperc = v_percstep;

                    for (int k = 0; k < this.v_reports.Count; k++)
                    {
                        v_encrypted = v_cryptor.RandomString() + ".pdf";

                        v_report = this.v_reports[k];

                        v_report.v_perc = v_perc;
                        v_report.v_percstep = v_percstep;
                        v_report.v_lastperc = v_lastperc;

                        v_report.Save(v_encrypted);
                        if (v_report.v_table.Rows.Count > 0)
                        {
                            v_zipstorer.AddFile(Spartacus.ThirdParty.ZipStorer.Compression.Deflate, v_encrypted, this.v_filenames[k], "");
                            (new System.IO.FileInfo(v_encrypted)).Delete();
                        }

                        v_perc = v_lastperc;
                        v_lastperc += v_percstep;
                    }

                    v_zipstorer.Close();
                }
                else
                {
                    v_perc = 0.0;
                    v_percstep = 100.0 / (double) this.v_reports.Count;
                    v_lastperc = v_percstep;

                    for (int k = 0; k < this.v_reports.Count; k++)
                    {
                        v_report = this.v_reports[k];

                        v_report.v_perc = v_perc;
                        v_report.v_percstep = v_percstep;
                        v_report.v_lastperc = v_lastperc;

                        v_report.Save(this.v_filenames[k]);

                        v_perc = v_lastperc;
                        v_lastperc += v_percstep;
                    }
                }
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o pacote PDF de saída.", e);
            }
        }

        /// <summary>
        /// Salva todos os relatórios em arquivos separados.
        /// </summary>
        /// <param name="p_filenames">Lista de nomes de arquivos, que deve possuir o mesmo número de elementos.</param>
        /// <param name="p_compress">Gerar arquivo ZIP com todos os arquivos PDF.</param>
        public void SaveSplitted(System.Collections.Generic.List<string> p_filenames, bool p_compress)
        {
            Spartacus.ThirdParty.ZipStorer v_zipstorer;
            Spartacus.Utils.Cryptor v_cryptor;
            string v_encrypted;
            Spartacus.Reporting.Report v_report;
            double v_perc, v_percstep, v_lastperc;

            try
            {
                if (p_compress)
                {
                    v_cryptor = new Spartacus.Utils.Cryptor("spartacus");

                    v_zipstorer = Spartacus.ThirdParty.ZipStorer.Create(this.v_filename.Replace(".pdf", ".zip"), "Generated with ZipStorer (by Jaime Olivares) embedded in Spartacus (by William Ivanski)");
                    v_zipstorer.EncodeUTF8 = true;

                    v_perc = 0.0;
                    v_percstep = 100.0 / (double) this.v_reports.Count;
                    v_lastperc = v_percstep;

                    for (int k = 0; k < this.v_reports.Count; k++)
                    {
                        v_encrypted = v_cryptor.RandomString() + ".pdf";

                        v_report = this.v_reports[k];

                        v_report.v_perc = v_perc;
                        v_report.v_percstep = v_percstep;
                        v_report.v_lastperc = v_lastperc;

                        v_report.Save(v_encrypted);
                        if (v_report.v_table.Rows.Count > 0)
                        {
                            v_zipstorer.AddFile(Spartacus.ThirdParty.ZipStorer.Compression.Deflate, v_encrypted, p_filenames[k], "");
                            (new System.IO.FileInfo(v_encrypted)).Delete();
                        }

                        v_perc = v_lastperc;
                        v_lastperc += v_percstep;
                    }

                    v_zipstorer.Close();
                }
                else
                {
                    v_perc = 0.0;
                    v_percstep = 100.0 / (double) this.v_reports.Count;
                    v_lastperc = v_percstep;

                    for (int k = 0; k < this.v_reports.Count; k++)
                    {
                        v_report = this.v_reports[k];

                        v_report.v_perc = v_perc;
                        v_report.v_percstep = v_percstep;
                        v_report.v_lastperc = v_lastperc;

                        v_report.Save(p_filenames[k]);

                        v_perc = v_lastperc;
                        v_lastperc += v_percstep;
                    }
                }
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o pacote PDF de saída.", e);
            }
        }

        /// <summary>
        /// Salva todos os relatórios em arquivos separados.
        /// É necessário que o atributo "v_filenames" tenha sido alimentado e possua o mesmo número de elementos.
        /// </summary>
        /// <param name="p_compress">Gerar arquivo ZIP com todos os arquivos PDF.</param>
        /// <param name="p_outfilename">Nome do arquivo ZIP de saída.</param>
        public void SaveSplitted(bool p_compress, string p_outfilename)
        {
            Spartacus.ThirdParty.ZipStorer v_zipstorer;
            Spartacus.Utils.Cryptor v_cryptor;
            string v_encrypted;
            Spartacus.Reporting.Report v_report;
            double v_perc, v_percstep, v_lastperc;

            try
            {
                if (p_compress)
                {
                    v_cryptor = new Spartacus.Utils.Cryptor("spartacus");

                    v_zipstorer = Spartacus.ThirdParty.ZipStorer.Create(p_outfilename, "Generated with ZipStorer (by Jaime Olivares) embedded in Spartacus (by William Ivanski)");
                    v_zipstorer.EncodeUTF8 = true;

                    v_perc = 0.0;
                    v_percstep = 100.0 / (double) this.v_reports.Count;
                    v_lastperc = v_percstep;

                    for (int k = 0; k < this.v_reports.Count; k++)
                    {
                        v_encrypted = v_cryptor.RandomString() + ".pdf";

                        v_report = this.v_reports[k];

                        v_report.v_perc = v_perc;
                        v_report.v_percstep = v_percstep;
                        v_report.v_lastperc = v_lastperc;

                        v_report.Save(v_encrypted);
                        if (v_report.v_table.Rows.Count > 0)
                        {
                            v_zipstorer.AddFile(Spartacus.ThirdParty.ZipStorer.Compression.Deflate, v_encrypted, this.v_filenames[k], "");
                            (new System.IO.FileInfo(v_encrypted)).Delete();
                        }

                        v_perc = v_lastperc;
                        v_lastperc += v_percstep;
                    }

                    v_zipstorer.Close();
                }
                else
                {
                    v_perc = 0.0;
                    v_percstep = 100.0 / (double) this.v_reports.Count;
                    v_lastperc = v_percstep;

                    for (int k = 0; k < this.v_reports.Count; k++)
                    {
                        v_report = this.v_reports[k];

                        v_report.v_perc = v_perc;
                        v_report.v_percstep = v_percstep;
                        v_report.v_lastperc = v_lastperc;

                        v_report.Save(this.v_filenames[k]);

                        v_perc = v_lastperc;
                        v_lastperc += v_percstep;
                    }
                }
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o pacote PDF de saída.", e);
            }
        }

        /// <summary>
        /// Salva todos os relatórios em arquivos separados.
        /// </summary>
        /// <param name="p_filenames">Lista de nomes de arquivos, que deve possuir o mesmo número de elementos.</param>
        /// <param name="p_compress">Gerar arquivo ZIP com todos os arquivos PDF.</param>
        /// <param name="p_outfilename">Nome do arquivo ZIP de saída.</param>
        public void SaveSplitted(System.Collections.Generic.List<string> p_filenames, bool p_compress, string p_outfilename)
        {
            Spartacus.ThirdParty.ZipStorer v_zipstorer;
            Spartacus.Utils.Cryptor v_cryptor;
            string v_encrypted;
            Spartacus.Reporting.Report v_report;
            double v_perc, v_percstep, v_lastperc;

            try
            {
                if (p_compress)
                {
                    v_cryptor = new Spartacus.Utils.Cryptor("spartacus");

                    v_zipstorer = Spartacus.ThirdParty.ZipStorer.Create(p_outfilename, "Generated with ZipStorer (by Jaime Olivares) embedded in Spartacus (by William Ivanski)");
                    v_zipstorer.EncodeUTF8 = true;

                    v_perc = 0.0;
                    v_percstep = 100.0 / (double) this.v_reports.Count;
                    v_lastperc = v_percstep;

                    for (int k = 0; k < this.v_reports.Count; k++)
                    {
                        v_encrypted = v_cryptor.RandomString() + ".pdf";

                        v_report = this.v_reports[k];

                        v_report.v_perc = v_perc;
                        v_report.v_percstep = v_percstep;
                        v_report.v_lastperc = v_lastperc;

                        v_report.Save(v_encrypted);
                        if (v_report.v_table.Rows.Count > 0)
                        {
                            v_zipstorer.AddFile(Spartacus.ThirdParty.ZipStorer.Compression.Deflate, v_encrypted, p_filenames[k], "");
                            (new System.IO.FileInfo(v_encrypted)).Delete();
                        }

                        v_perc = v_lastperc;
                        v_lastperc += v_percstep;
                    }

                    v_zipstorer.Close();
                }
                else
                {
                    v_perc = 0.0;
                    v_percstep = 100.0 / (double) this.v_reports.Count;
                    v_lastperc = v_percstep;

                    for (int k = 0; k < this.v_reports.Count; k++)
                    {
                        v_report = this.v_reports[k];

                        v_report.v_perc = v_perc;
                        v_report.v_percstep = v_percstep;
                        v_report.v_lastperc = v_lastperc;

                        v_report.Save(p_filenames[k]);

                        v_perc = v_lastperc;
                        v_lastperc += v_percstep;
                    }
                }
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o pacote PDF de saída.", e);
            }
        }

        /// <summary>
        /// Salva todos os relatórios em um único PDF.
        /// </summary>
        public void SaveMerged()
        {
            PDFjet.NET.PDF v_pdf;
            System.IO.BufferedStream v_buffer;
            System.IO.FileStream f;
            Spartacus.Reporting.Report v_report;
            double v_perc, v_percstep, v_lastperc;

            try
            {
                f = new System.IO.FileStream(this.v_filename, System.IO.FileMode.Create);
                v_buffer = new System.IO.BufferedStream(f);

                v_pdf = new PDFjet.NET.PDF(v_buffer);

                v_perc = 0.0;
                v_percstep = 100.0 / (double) this.v_reports.Count;
                v_lastperc = v_percstep;

                for (int k = 0; k < this.v_reports.Count; k++)
                {
                    v_report = this.v_reports[k];

                    v_report.v_perc = v_perc;
                    v_report.v_percstep = v_percstep;
                    v_report.v_lastperc = v_lastperc;

                    v_report.SavePartial(v_pdf);

                    v_perc = v_lastperc;
                    v_lastperc += v_percstep;
                }

                v_pdf.Flush();
                v_buffer.Close();
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o pacote PDF de saída.", e);
            }
        }

        /// <summary>
        /// Salva todos os relatórios em um único PDF.
        /// </summary>
        /// <param name="p_filename">Nome do arquivo PDF a ser salvo.</param>
        public void SaveMerged(string p_filename)
        {
            PDFjet.NET.PDF v_pdf;
            System.IO.BufferedStream v_buffer;
            System.IO.FileStream f;
            Spartacus.Reporting.Report v_report;
            double v_perc, v_percstep, v_lastperc;

            try
            {
                f = new System.IO.FileStream(p_filename, System.IO.FileMode.Create);
                v_buffer = new System.IO.BufferedStream(f);

                v_pdf = new PDFjet.NET.PDF(v_buffer);

                v_perc = 0.0;
                v_percstep = 100.0 / (double) this.v_reports.Count;
                v_lastperc = v_percstep;

                for (int k = 0; k < this.v_reports.Count; k++)
                {
                    v_report = this.v_reports[k];

                    v_report.v_perc = v_perc;
                    v_report.v_percstep = v_percstep;
                    v_report.v_lastperc = v_lastperc;

                    v_report.SavePartial(v_pdf);

                    v_perc = v_lastperc;
                    v_lastperc += v_percstep;
                }

                v_pdf.Flush();
                v_buffer.Close();
            }
            catch (Spartacus.Reporting.Exception e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Reporting.Exception("Erro ao gerar o pacote PDF de saída.", e);
            }
        }
    }
}

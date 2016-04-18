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

namespace Spartacus.PollyDB
{
    public class Connection
    {
        public string v_directory;
        public System.Collections.Generic.List<string> v_files;

        private bool v_open;

        public char v_separator;
        public char v_delimitator;
        public bool v_header;
        public System.Text.Encoding v_encoding;

        public Connection(string p_directory)
        {
            this.v_directory = p_directory;
            this.v_open = false;
            this.v_files = new System.Collections.Generic.List<string>();

            this.v_separator = ';';
            this.v_delimitator = ' ';
            this.v_header = true;
            this.v_encoding = System.Text.Encoding.Default;
        }

        public Connection(string p_directory, char p_separator, char p_delimitator, bool p_header, System.Text.Encoding p_encoding)
        {
            this.v_directory = p_directory;
            this.v_open = false;
            this.v_files = new System.Collections.Generic.List<string>();

            this.v_separator = ';';
            this.v_delimitator = ' ';
            this.v_header = true;
            this.v_encoding = System.Text.Encoding.Default;
        }

        public void Open()
        {
            System.IO.DirectoryInfo v_info;

            v_info = new System.IO.DirectoryInfo(this.v_directory);

            if (v_info.Exists)
            {
                v_files.AddRange(System.IO.Directory.GetFiles(this.v_directory, "*.csv", System.IO.SearchOption.TopDirectoryOnly));
                v_files.AddRange(System.IO.Directory.GetFiles(this.v_directory, "*.CSV", System.IO.SearchOption.TopDirectoryOnly));
                v_files.AddRange(System.IO.Directory.GetFiles(this.v_directory, "*.dbf", System.IO.SearchOption.TopDirectoryOnly));
                v_files.AddRange(System.IO.Directory.GetFiles(this.v_directory, "*.DBF", System.IO.SearchOption.TopDirectoryOnly));
                v_files.AddRange(System.IO.Directory.GetFiles(this.v_directory, "*.xlsx", System.IO.SearchOption.TopDirectoryOnly));
                v_files.AddRange(System.IO.Directory.GetFiles(this.v_directory, "*.XLSX", System.IO.SearchOption.TopDirectoryOnly));

                if (v_files.Count > 0)
                    this.v_open = true;
                else
                    throw new Spartacus.PollyDB.Exception("Spartacus.PollyDB.Connection.Open: Directory {0} does not contain any supported files.", this.v_directory);
            }
            else
                throw new Spartacus.PollyDB.Exception("Spartacus.PollyDB.Connection.Open: Directory {0} does not exist.", this.v_directory);
        }

        public void Close()
        {
            this.v_files.Clear();
            this.v_open = false;
        }

        public bool Exists(string p_file)
        {
            Spartacus.Utils.File v_file;
            bool v_achou = false;
            int k;

            if (this.v_open)
            {
                k = 0;
                while (k < this.v_files.Count && !v_achou)
                {
                    v_file = new Spartacus.Utils.File(Spartacus.Utils.FileType.FILE, this.v_files[k].ToLower());
                    if (v_file.v_name == p_file)
                        v_achou = true;
                    else
                        k++;
                }
            }

            return v_achou;
        }
    }
}

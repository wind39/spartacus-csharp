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

namespace Spartacus.Utils
{
    public class FileArray
    {
        /// <summary>
        /// Lista de arquivos e/ou diretórios contidos no FileArray.
        /// </summary>
        public System.Collections.Generic.List<Spartacus.Utils.File> v_files;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.FileArray"/>.
        /// </summary>
        /// <param name="p_filenames">Lista de nomes de arquivos ou diretórios.</param>
        /// <param name="p_filetype">Tipo dos nomes, se são arquivos ou diretórios.</param>
        public FileArray(System.Collections.Generic.List<string> p_filenames, Spartacus.Utils.FileType p_filetype)
        {
            Spartacus.Utils.File v_file;
            int k;

            this.v_files = new System.Collections.Generic.List<Spartacus.Utils.File>();

            k = 1;
            foreach (string v_filename in p_filenames)
            {
                v_file = new Spartacus.Utils.File(k, 1, p_filetype, v_filename);

                this.v_files.Add(v_file);

                k++;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.FileArray"/>.
        /// </summary>
        /// <param name="p_filenames">String contendo nomes de arquivos ou diretórios, separados por ponto-e-vírgula.</param>
        /// <param name="p_filetype">Tipo dos nomes, se são arquivos ou diretórios.</param>
        public FileArray(string p_filenames, Spartacus.Utils.FileType p_filetype)
        {
            Spartacus.Utils.File v_file;
            string[] v_filenames;
            int k;

            this.v_files = new System.Collections.Generic.List<Spartacus.Utils.File>();

            v_filenames = p_filenames.Split(';');

            k = 1;
            foreach (string v_filename in v_filenames)
            {
                v_file = new Spartacus.Utils.File(k, 1, p_filetype, v_filename);

                this.v_files.Add(v_file);

                k++;
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.FileArray"/>.
        /// </summary>
        /// <param name="p_directorynames">Lista de nomes de diretórios.</param>
        /// <param name="p_filter">Filtro de extensão de arquivos.</param>
        public FileArray(System.Collections.Generic.List<string> p_directorynames, string p_filter)
        {
            Spartacus.Utils.File v_file;
            string[] v_filenames;
            int k;

            this.v_files = new System.Collections.Generic.List<Spartacus.Utils.File>();

            k = 1;
            foreach (string v_directoryname in p_directorynames)
            {
                v_filenames = this.FilterList(v_directoryname, p_filter, System.IO.SearchOption.TopDirectoryOnly);

                foreach (string v_filename in v_filenames)
                {
                    v_file = new Spartacus.Utils.File(k, 1, Spartacus.Utils.FileType.FILE, v_filename);

                    this.v_files.Add(v_file);

                    k++;
                }
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.FileArray"/>.
        /// </summary>
        /// <param name="p_directorynames">String contendo nomes de diretórios separados por ponto-e-vírgula.</param>
        /// <param name="p_filter">Filtro de extensão de arquivos.</param>
        public FileArray(string p_directorynames, string p_filter)
        {
            Spartacus.Utils.File v_file;
            string[] v_directorynames;
            string[] v_filenames;
            int k;

            this.v_files = new System.Collections.Generic.List<Spartacus.Utils.File>();

            v_directorynames = p_directorynames.Split(';');

            k = 1;
            foreach (string v_directoryname in v_directorynames)
            {
                v_filenames = this.FilterList(v_directoryname, p_filter, System.IO.SearchOption.TopDirectoryOnly);

                foreach (string v_filename in v_filenames)
                {
                    v_file = new Spartacus.Utils.File(k, 1, Spartacus.Utils.FileType.FILE, v_filename);

                    this.v_files.Add(v_file);

                    k++;
                }
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.FileArray"/>.
        /// </summary>
        /// <param name="p_directorynames">Lista de nomes de diretórios.</param>
        /// <param name="p_filter">Filtro de extensão de arquivos.</param>
        /// <param name="p_searchoption">Opção de busca no diretório.</param>
        public FileArray(System.Collections.Generic.List<string> p_directorynames, string p_filter, System.IO.SearchOption p_searchoption)
        {
            Spartacus.Utils.File v_file;
            string[] v_filenames;
            int k;

            this.v_files = new System.Collections.Generic.List<Spartacus.Utils.File>();

            k = 1;
            foreach (string v_directoryname in p_directorynames)
            {
                v_filenames = this.FilterList(v_directoryname, p_filter, p_searchoption);

                foreach (string v_filename in v_filenames)
                {
                    v_file = new Spartacus.Utils.File(k, 1, Spartacus.Utils.FileType.FILE, v_filename);

                    this.v_files.Add(v_file);

                    k++;
                }
            }
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.FileArray"/>.
        /// </summary>
        /// <param name="p_directorynames">String contendo nomes de diretórios separados por ponto-e-vírgula.</param>
        /// <param name="p_filter">Filtro de extensão de arquivos.</param>
        /// <param name="p_searchoption">Opção de busca no diretório.</param>
        public FileArray(string p_directorynames, string p_filter, System.IO.SearchOption p_searchoption)
        {
            Spartacus.Utils.File v_file;
            string[] v_directorynames;
            string[] v_filenames;
            int k;

            this.v_files = new System.Collections.Generic.List<Spartacus.Utils.File>();

            v_directorynames = p_directorynames.Split(';');

            k = 1;
            foreach (string v_directoryname in v_directorynames)
            {
                v_filenames = this.FilterList(v_directoryname, p_filter, p_searchoption);

                foreach (string v_filename in v_filenames)
                {
                    v_file = new Spartacus.Utils.File(k, 1, Spartacus.Utils.FileType.FILE, v_filename);

                    this.v_files.Add(v_file);

                    k++;
                }
            }
        }

        /// <summary>
        /// Lista, dentro de um diretório, todos os arquivos cujo nome corresponde ao filtro.
        /// O filtro é uma string que pode conter vários filtros separados por '|'.
        /// </summary>
        /// <returns>Lista com o nome completo de todos os arquivos que correspondem ao filtro.</returns>
        /// <param name="p_directoryname">Nome do diretório.</param>
        /// <param name="p_filter">String que pode conter vários filtros separados por '|'.</param>
        /// <param name="p_searchoption">Opção de busca.</param>
        private string[] FilterList(string p_directoryname, string p_filter, System.IO.SearchOption p_searchoption)
        {
            System.Collections.Generic.List<string> v_tempfiles;
            string[] v_filters;

            v_tempfiles = new System.Collections.Generic.List<string>();

            v_filters = p_filter.Split('|');

            foreach (string v_filter in v_filters)
                v_tempfiles.AddRange(System.IO.Directory.GetFiles(p_directoryname, v_filter, p_searchoption));

            return (string[]) v_tempfiles.ToArray();
        }
    }
}

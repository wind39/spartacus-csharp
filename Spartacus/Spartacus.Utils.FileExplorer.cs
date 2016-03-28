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

namespace Spartacus.Utils
{
    /// <summary>
    /// Enumera os atributos de arquivos apresentados no grid de arquivos.
    /// </summary>
    public enum FileAttributes
    {
        ID,
        TYPE,
        NAME,
        EXTENSION,
        LASTWRITEDATE,
        SIZE
    }

    /// <summary>
    /// Enumera as possibilidades de exibição de arquivos baseado no padrão.
    /// </summary>
    public enum ShowPatternType
    {
        SHOWALL,              // mostra todos os arquivos
        SHOWALLEXCEPTPATTERN, // mostra todos os arquivos menos o padrão
        SHOWONLYPATTERN,      // mostra apenas o padrão
        SHOWALLPROTECTED      // mostra todos os arquivos, mas tudo é read-only
    }

    /// <summary>
    /// Classe FileExplorer.
    /// Representa um explorador de arquivos genérico, que pode ser usado em qualquer interface (prompt de comando, desktop ou web).
    /// </summary>
    public class FileExplorer
    {
        /// <summary>
        /// Pasta raiz do explorador de arquivos.
        /// </summary>
        public string v_root;

        /// <summary>
        /// Nome da pasta atual.
        /// </summary>
        public Spartacus.Utils.File v_current;

        /// <summary>
        /// Nível atual dentro da estrutura de diretórios.
        /// </summary>
        public int v_currentlevel;

        /// <summary>
        /// Lista de arquivos e diretórios da pasta atual.
        /// </summary>
        public System.Collections.ArrayList v_files;

        /// <summary>
        /// Lista original de arquivos e diretórios da pasta atual.
        /// </summary>
        public System.Collections.ArrayList v_filesorig;

        /// <summary>
        /// Separador de diretórios.
        /// </summary>
        public Spartacus.Utils.PathSeparator v_pathseparator;

        /// <summary>
        /// Número de arquivos por página.
        /// </summary>
        public int v_numfilesperpage;

        /// <summary>
        /// Número de páginas.
        /// </summary>
        public int v_numpages;

        /// <summary>
        /// Padrão de nome de arquivo ou diretório que deve ser protegido contra escrita (fictício e não permissões reais do arquivo).
        /// </summary>
        public string v_protectpattern;

        /// <summary>
        /// Informa o tipo de exibição de arquivos baseado no padrão de proteção.
        /// </summary>
        //public bool v_showprotectpattern;
        public Spartacus.Utils.ShowPatternType v_showpatterntype;

        /// <summary>
        /// Nível mínimo de proteção de escrita dentro da estrutura de diretórios.
        /// </summary>
        public int v_protectedminlevel;

        /// <summary>
        /// Informa se arquivos e diretórios ocultos devem ou não ser mostrados.
        /// </summary>
        public bool v_showhiddenfiles;

        /// <summary>
        /// Armazena um vetor com o histórico de pastas pai.
        /// </summary>
        public System.Collections.ArrayList v_returnhistory;

        /// <summary>
        /// Fonte usada para renderizar o histórico de pastas pai no aplicativo cliente.
        /// </summary>
        public PDFjet.NET.Font v_returnhistory_font;

        /// <summary>
        /// Texto a ser exibido como pasta raiz ao renderizar o histórico de pastas pai.
        /// </summary>
        public string v_returnhistory_root;

        /// <summary>
        /// String usada para separar pastas no histórico de pastas pai.
        /// </summary>
        public string v_returnhistory_sep;

        /// <summary>
        /// String usada como agregação, para quando o texto renderizado do histórico de pastas pai estourou o limite de tamanho.
        /// </summary>
        public string v_returnhistory_first;

        /// <summary>
        /// Largura máxima para mostrar o texto renderizado do histórico de pastas pai.
        /// </summary>
        public double v_returnhistory_maxwidth;


        /// <summary>
        /// Initializa uma nova instância da classe <see cref="Spartacus.Utils.FileExplorer"/>.
        /// </summary>
        public FileExplorer()
        {
            this.v_root = null;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_currentlevel = 0;
            this.v_protectedminlevel = -1; // proteção a princípio está desabilitada

            this.v_showpatterntype = Spartacus.Utils.ShowPatternType.SHOWALL; // padrão é mostrar todos os arquivos
            this.v_protectpattern = "";
            this.v_showhiddenfiles = false; // padrão é não mostrar arquivos e pastas ocultos

            this.v_files = new System.Collections.ArrayList();

            this.v_returnhistory = new System.Collections.ArrayList();
            this.v_returnhistory_font = new PDFjet.NET.Font(PDFjet.NET.CoreFont.HELVETICA);
            this.v_returnhistory_font.SetSize(12.0);
            this.v_returnhistory_root = "Diretorio Raiz";
            this.v_returnhistory_sep = " > ";
            this.v_returnhistory_first = "...";
            this.v_returnhistory_maxwidth = 800.0;
        }

        /// <summary>
        /// Initializa uma nova instância da classe <see cref="Spartacus.Utils.FileExplorer"/>.
        /// </summary>
        /// <param name='p_root'>
        /// Pasta raiz do explorador de arquivos.
        /// </param>
        public FileExplorer(string p_root)
        {
            this.v_root = p_root;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_current = new Spartacus.Utils.File(0, 0, Spartacus.Utils.FileType.DIRECTORY, this.v_root, this.v_pathseparator);
            this.v_currentlevel = 0;
            this.v_protectedminlevel = -1; // proteção a princípio está desabilitada

            this.v_showpatterntype = Spartacus.Utils.ShowPatternType.SHOWALL; // padrão é mostrar todos os arquivos
            this.v_protectpattern = "";
            this.v_showhiddenfiles = false; // padrão é não mostrar arquivos e pastas ocultos

            this.v_current.v_protected = true; // raiz sempre é protegida

            this.v_files = new System.Collections.ArrayList();

            this.v_returnhistory = new System.Collections.ArrayList();
            this.v_returnhistory_font = new PDFjet.NET.Font(PDFjet.NET.CoreFont.HELVETICA);
            this.v_returnhistory_font.SetSize(12.0);
            this.v_returnhistory_root = "Diretorio Raiz";
            this.v_returnhistory_sep = " > ";
            this.v_returnhistory_first = "...";
            this.v_returnhistory_maxwidth = 800.0;

            this.v_returnhistory.Add(p_root);
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.FileExplorer"/>.
        /// </summary>
        /// <param name='p_root'>
        /// Pasta raiz do explorador de arquivos.
        /// </param>
        /// <param name='p_pathseparator'>
        /// Separador de diretórios.
        /// </param>
        public FileExplorer(string p_root, Spartacus.Utils.PathSeparator p_pathseparator)
        {
            this.v_root = p_root;
            this.v_pathseparator = p_pathseparator;
            this.v_current = new Spartacus.Utils.File(0, 0, Spartacus.Utils.FileType.DIRECTORY, this.v_root, this.v_pathseparator);
            this.v_currentlevel = 0;
            this.v_protectedminlevel = -1; // proteção a princípio está desabilitada

            this.v_showpatterntype = Spartacus.Utils.ShowPatternType.SHOWALL; // padrão é mostrar todos os arquivos
            this.v_protectpattern = "";
            this.v_showhiddenfiles = false; // padrão é não mostrar arquivos e pastas ocultos

            this.v_current.v_protected = true; // raiz sempre é protegida

            this.v_files = new System.Collections.ArrayList();

            this.v_returnhistory = new System.Collections.ArrayList();
            this.v_returnhistory_font = new PDFjet.NET.Font(PDFjet.NET.CoreFont.HELVETICA);
            this.v_returnhistory_font.SetSize(12.0);
            this.v_returnhistory_root = "Diretorio Raiz";
            this.v_returnhistory_sep = " > ";
            this.v_returnhistory_first = "...";
            this.v_returnhistory_maxwidth = 800.0;

            this.v_returnhistory.Add(p_root);
        }

        /// <summary>
        /// Seta a pasta raiz do explorador de arquivos.
        /// </summary>
        /// <param name='p_root'>
        /// Pasta raiz do explorador de arquivos.
        /// </param>
        public void SetRoot(string p_root)
        {
            this.v_root = p_root;
            this.v_current = new Spartacus.Utils.File(0, 0, Spartacus.Utils.FileType.DIRECTORY, this.v_root, this.v_pathseparator);
            this.v_currentlevel = 0;

            this.v_current.v_protected = true; // raiz sempre é protegida

            this.v_returnhistory.Add(p_root);
        }

        /// <summary>
        /// Constrói lista de arquivos e diretorios contidos no diretório atual.
        /// </summary>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir listar o conteúdo do diretório atual.</exception>
        public void List()
        {
            Spartacus.Utils.File v_file, v_directory;
            System.IO.DirectoryInfo v_directoryinfo;
            System.IO.FileInfo v_fileinfo;
            int k;

            try
            {
                this.v_files.Clear();
                k = 1;

                foreach (string v_item in System.IO.Directory.GetDirectories(this.v_current.CompleteFileName()))
                {
                    switch (this.v_showpatterntype)
                    {
                        case Spartacus.Utils.ShowPatternType.SHOWALL:
                            v_directoryinfo = new System.IO.DirectoryInfo(v_item);

                            v_directory = new Spartacus.Utils.File(k, 0, Spartacus.Utils.FileType.DIRECTORY, v_item, this.v_pathseparator, v_directoryinfo.LastWriteTime);

                            if (v_item.Contains(this.v_protectpattern) || this.v_currentlevel < this.v_protectedminlevel)
                                v_directory.v_protected = true;

                            if (v_directory.v_hidden)
                            {
                                if (this.v_showhiddenfiles)
                                {
                                    this.v_files.Add(v_directory);
                                    k++;
                                }
                            }
                            else
                            {
                                this.v_files.Add(v_directory);
                                k++;
                            }
                            break;
                        case Spartacus.Utils.ShowPatternType.SHOWALLEXCEPTPATTERN:
                            if (! v_item.Contains(this.v_protectpattern)) // só vai mostrar se não contiver o padrão
                            {
                                v_directoryinfo = new System.IO.DirectoryInfo(v_item);

                                v_directory = new Spartacus.Utils.File(k, 0, Spartacus.Utils.FileType.DIRECTORY, v_item, this.v_pathseparator, v_directoryinfo.LastWriteTime);

                                if (this.v_currentlevel < this.v_protectedminlevel)
                                    v_directory.v_protected = true;

                                if (v_directory.v_hidden)
                                {
                                    if (this.v_showhiddenfiles)
                                    {
                                        this.v_files.Add(v_directory);
                                        k++;
                                    }
                                }
                                else
                                {
                                    this.v_files.Add(v_directory);
                                    k++;
                                }
                            }
                            break;
                        case Spartacus.Utils.ShowPatternType.SHOWONLYPATTERN:
                            // só vai mostrar se contiver o padrão ou se o nivel atual for menor que o nivel minimo protegido
                            if (v_item.Contains(this.v_protectpattern) || this.v_currentlevel < this.v_protectedminlevel)
                            {
                                v_directoryinfo = new System.IO.DirectoryInfo(v_item);

                                v_directory = new Spartacus.Utils.File(k, 0, Spartacus.Utils.FileType.DIRECTORY, v_item, this.v_pathseparator, v_directoryinfo.LastWriteTime);

                                if (this.v_currentlevel < this.v_protectedminlevel)
                                    v_directory.v_protected = true;

                                if (v_directory.v_hidden)
                                {
                                    if (this.v_showhiddenfiles)
                                    {
                                        this.v_files.Add(v_directory);
                                        k++;
                                    }
                                }
                                else
                                {
                                    this.v_files.Add(v_directory);
                                    k++;
                                }
                            }
                            break;
                        case Spartacus.Utils.ShowPatternType.SHOWALLPROTECTED:
                            // mostra todos os arquivos, mas todos estao protegidos
                            v_directoryinfo = new System.IO.DirectoryInfo(v_item);

                            v_directory = new Spartacus.Utils.File(k, 0, Spartacus.Utils.FileType.DIRECTORY, v_item, this.v_pathseparator, v_directoryinfo.LastWriteTime);
                            v_directory.v_protected = true;

                            if (v_directory.v_hidden)
                            {
                                if (this.v_showhiddenfiles)
                                {
                                    this.v_files.Add(v_directory);
                                    k++;
                                }
                            }
                            else
                            {
                                this.v_files.Add(v_directory);
                                k++;
                            }
                            break;
                    }
                }

                foreach (string v_item in System.IO.Directory.GetFiles(this.v_current.CompleteFileName()))
                {
                    v_fileinfo = new System.IO.FileInfo(v_item);

                    v_file = new Spartacus.Utils.File(k, 0, Spartacus.Utils.FileType.FILE, v_item, this.v_pathseparator, v_fileinfo.LastWriteTime, v_fileinfo.Length);

                    switch (this.v_showpatterntype)
                    {
                        case Spartacus.Utils.ShowPatternType.SHOWALL:
                            if (v_item.Contains(this.v_protectpattern) || this.v_currentlevel < this.v_protectedminlevel)
                                v_file.v_protected = true;
                            break;
                        case Spartacus.Utils.ShowPatternType.SHOWALLEXCEPTPATTERN:
                            if (this.v_currentlevel < this.v_protectedminlevel)
                                v_file.v_protected = true;
                            break;
                        case Spartacus.Utils.ShowPatternType.SHOWONLYPATTERN:
                            if (this.v_currentlevel < this.v_protectedminlevel)
                                v_file.v_protected = true;
                            break;
                        case Spartacus.Utils.ShowPatternType.SHOWALLPROTECTED:
                            v_file.v_protected = true;
                            break;
                        default:
                            break;
                    }

                    if (v_file.v_hidden)
                    {
                        if (this.v_showhiddenfiles)
                        {
                            this.v_files.Add(v_file);
                            k++;
                        }
                    }
                    else
                    {
                        this.v_files.Add(v_file);
                        k++;
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Lista todos os arquivos cujo nome corresponde ao filtro.
        /// O filtro é uma string que pode conter vários filtros separados por '|'.
        /// </summary>
        /// <returns>Lista com o nome completo de todos os arquivos que correspondem ao filtro.</returns>
        /// <param name="p_filter">String que pode conter vários filtros separados por '|'.</param>
        /// <param name="p_searchoption">Opção de busca.</param>
        public string[] FilterList(string p_filter, System.IO.SearchOption p_searchoption)
        {
            string[] v_filters;

            this.v_files.Clear();

            v_filters = p_filter.Split('|');

            foreach (string v_filter in v_filters)
                this.v_files.AddRange(System.IO.Directory.GetFiles(this.v_root, v_filter, p_searchoption));

            return (string[]) this.v_files.ToArray(typeof(string));
        }

        /// <summary>
        /// Entra no diretório especificado.
        /// </summary>
        /// <param name='p_id'>
        /// Código do diretório dentro da lista de arquivos e diretórios.
        /// </param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando o item referenciado não é um diretório.</exception>
        public void Enter(int p_id)
        {
            Spartacus.Utils.File v_file;

            try
            {
                v_file = (Spartacus.Utils.File) this.v_files[p_id-1];

                if (v_file.v_filetype != Spartacus.Utils.FileType.DIRECTORY)
                {
                    throw new Spartacus.Utils.Exception("{0} não é um diretório.", v_file.CompleteFileName());
                }
                else
                {
                    this.v_current = new Spartacus.Utils.File(0, 0, Spartacus.Utils.FileType.DIRECTORY, v_file.CompleteFileName());
                    this.v_current.v_protected = v_file.v_protected;
                    this.v_currentlevel++;

                    this.v_returnhistory.Add(this.v_current.CompleteFileName());
                }
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Entra no diretório especificado.
        /// </summary>
        /// <param name='p_completename'>
        /// Caminho completo do diretório.
        /// </param>
        /// <param name='p_protected'>
        /// Se o diretório é protegido ou não.
        /// </param>
        public void Enter(string p_completename, bool p_protected)
        {
            string[] v_path;
            string v_tmp;
            char v_ch;

            try
            {
                this.v_current = new Spartacus.Utils.File(0, 0, Spartacus.Utils.FileType.DIRECTORY, p_completename, Spartacus.Utils.PathSeparator.SLASH);
                this.v_current.v_protected = p_protected;

                v_ch = '/';

                v_path = p_completename.Replace(this.v_root, this.v_returnhistory_root).Split(v_ch);

                this.v_returnhistory.Clear();

                v_tmp = v_path[0];
                this.v_returnhistory.Add(v_tmp);
                for (int k = 1; k < v_path.Length; k++)
                {
                    v_tmp += v_ch + v_path[k];
                    this.v_returnhistory.Add(v_tmp);
                }

                this.v_currentlevel = v_path.Length - 1;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Entra no diretório especificado.
        /// </summary>
        /// <param name='p_completename'>
        /// Caminho completo do diretório.
        /// </param>
        /// <param name='p_protected'>
        /// Se o diretório é protegido ou não.
        /// </param>
        /// <param name="p_pathseparator">
        /// Separador de diretórios.
        /// </param>
        public void Enter(string p_completename, bool p_protected, Spartacus.Utils.PathSeparator p_pathseparator)
        {
            string[] v_path;
            string v_tmp;
            char v_ch;

            try
            {
                this.v_current = new Spartacus.Utils.File(0, 0, Spartacus.Utils.FileType.DIRECTORY, p_completename, p_pathseparator);
                this.v_current.v_protected = p_protected;

                if (p_pathseparator == Spartacus.Utils.PathSeparator.SLASH)
                    v_ch = '/';
                else
                    v_ch = '\\';

                v_path = p_completename.Replace(this.v_root, this.v_returnhistory_root).Split(v_ch);

                this.v_returnhistory.Clear();

                v_tmp = v_path[0];
                this.v_returnhistory.Add(v_tmp);
                for (int k = 1; k < v_path.Length; k++)
                {
                    v_tmp += v_ch + v_path[k];
                    this.v_returnhistory.Add(v_tmp);
                }

                this.v_currentlevel = v_path.Length - 1;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Retorna para o diretório anterior, ou seja, o diretório pai do diretório atual.
        /// </summary>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando o diretório atual também é a raiz do explorador de arquivos.</exception>
        public void Return()
        {
            string v_parent;

            if (this.v_current.CompleteFileName() == this.v_root)
            {
                throw new Spartacus.Utils.Exception("Já está no diretório raiz.");
            }
            else
            {
                v_parent = this.v_current.v_path;
                this.v_current = new Spartacus.Utils.File(0, 0, Spartacus.Utils.FileType.DIRECTORY, v_parent);

                this.v_currentlevel--;

                if (v_parent.Contains(this.v_protectpattern) || this.v_currentlevel <= this.v_protectedminlevel)
                    this.v_current.v_protected = true;

                this.v_returnhistory.RemoveAt(this.v_returnhistory.Count-1);
            }
        }

        /// <summary>
        /// Retorna para um diretório anterior qualquer no histórico.
        /// </summary>
        /// <param name="p_history">Índice do histórico.</param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando o diretório atual também é a raiz do explorador de arquivos.</exception>
        public void Return(int p_history)
        {
            string v_parent;

            if (this.v_current.CompleteFileName() == this.v_root)
            {
                throw new Spartacus.Utils.Exception("Já está no diretório raiz.");
            }
            else
            {
                v_parent = this.v_current.v_path;
                this.v_current = new Spartacus.Utils.File(0, 0, Spartacus.Utils.FileType.DIRECTORY, v_parent);

                this.v_currentlevel--;

                if (v_parent.Contains(this.v_protectpattern) || this.v_currentlevel <= this.v_protectedminlevel)
                    this.v_current.v_protected = true;

                this.v_returnhistory.RemoveAt(this.v_returnhistory.Count-1);

                if (p_history < this.v_returnhistory.Count - 1)
                    this.Return(p_history);
            }
        }

        /// <summary>
        /// Configura as variáveis necessárias para tratar a renderização do texto do histórico de pastas pai no aplicativo cliente.
        /// </summary>
        /// <param name="p_font">Nome da fonte.</param>
        /// <param name="p_size">Tamanho da fonte.</param>
        /// <param name="p_italic">Se a fonte deve ser renderizada com estilo itálico ou não.</param>
        /// <param name="p_fakeroot">Nome falso da raiz.</param>
        /// <param name="p_sep">Texto separador entre pastas.</param>
        /// <param name="p_first">Texto do primeiro nível, caso o texto total do histórico de pastas pai estoure o limite.</param>
        /// <param name="p_maxwidth">Largura máxima do texto a ser renderizado.</param>
        public void SetupReturnHistory(string p_font, float p_size, bool p_italic, string p_fakeroot, string p_sep, string p_first, int p_maxwidth)
        {
            if (p_italic)
                this.v_returnhistory_font.SetItalic(true);
            this.v_returnhistory_font.SetSize(p_size);

            this.v_returnhistory_root = p_fakeroot;
            this.v_returnhistory_sep = p_sep;
            this.v_returnhistory_first = p_first;
            this.v_returnhistory_maxwidth = p_maxwidth;
        }

        /// <summary>
        /// Trata o histórico de retorno para pastas pai, conforme fonte e tamanho máximo do texto a ser renderizado.
        /// </summary>
        /// <returns>Vetor de histórico de retorno.</returns>
        /// <param name="p_overflow">.</param>
        public System.Collections.ArrayList GetReturnHistory(out bool p_overflow, out int p_start)
        {
            System.Collections.ArrayList v_handledhistory;
            Spartacus.Utils.File v_directory;
            string v_text;
            int k;

            v_handledhistory = new System.Collections.ArrayList();

            p_overflow = false;
            k = this.v_returnhistory.Count - 1;
            v_text = "";

            while (k > 0 && !p_overflow)
            {
                v_directory = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.DIRECTORY, (string)this.v_returnhistory [k]);

                v_text += this.v_returnhistory_sep + (v_directory.v_name);

                if (this.v_returnhistory_font.StringWidth(this.v_returnhistory_root + v_text) > this.v_returnhistory_maxwidth)
                {
                    v_handledhistory.Insert(0, this.v_returnhistory_first);
                    p_overflow = true;
                }
                else
                {
                    v_handledhistory.Insert(0, v_directory.v_name);
                    k--;
                }
            }
            
            v_handledhistory.Insert(0, this.v_returnhistory_root);
            p_start = k - 1;

            return v_handledhistory;
        }

        /// <summary>
        /// Retorna o arquivo ou diretório solicitado de acordo com seu código dentro da lista de arquivos e diretórios.
        /// </summary>
        /// <returns>
        /// Arquivo ou diretório especificado.
        /// </returns>
        /// <param name='p_id'>
        /// Código do arquivo ou diretório dentro da lista de arquivos e diretórios.
        /// </param>
        public Spartacus.Utils.File Get(int p_id)
        {
            try
            {
                return (Spartacus.Utils.File)this.v_files[p_id-1];
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Cria um arquivo.
        /// </summary>
        /// <returns>
        /// Nome completo do novo arquivo a ser criado.
        /// </returns>
        /// <param name='p_file'>
        /// Nome do novo arquivo a ser criado.
        /// </param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não é possível criar o arquivo.</exception>
        public string Put(string p_file)
        {
            string v_separator;
            string v_completename;

            if (this.v_pathseparator == Spartacus.Utils.PathSeparator.SLASH)
                v_separator = "/";
            else
                v_separator = "\\";

            try
            {
                v_completename = this.v_current.CompleteFileName() + v_separator + p_file;

                System.IO.File.Create(v_completename);
                System.IO.File.Delete(v_completename);

                return v_completename;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Cria um diretório.
        /// </summary>
        /// <returns>
        /// Nome completo do novo diretório a ser criado.
        /// </returns>
        /// <param name='p_directory'>
        /// Nome do novo diretório a ser criado.
        /// </param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não é possível criar o diretório.</exception>
        public string Mkdir(string p_directory)
        {
            string v_separator;
            string v_completename;

            if (this.v_pathseparator == Spartacus.Utils.PathSeparator.SLASH)
                v_separator = "/";
            else
                v_separator = "\\";

            try
            {
                v_completename = this.v_current.CompleteFileName() + v_separator + p_directory;

                System.IO.Directory.CreateDirectory(v_completename);

                return v_completename;
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Deleta o arquivo ou diretório.
        /// </summary>
        /// <param name='p_id'>
        /// Código do arquivo dentro da lista de arquivos e diretórios.
        /// </param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não é possível remover o arquivo ou diretório.</exception>
        public void Delete(int p_id)
        {
            Spartacus.Utils.File v_file;

            try
            {
                v_file = (Spartacus.Utils.File)this.v_files[p_id-1];

                if (v_file.v_filetype == Spartacus.Utils.FileType.DIRECTORY)
                    System.IO.Directory.Delete(v_file.CompleteFileName(), true);
                else
                    System.IO.File.Delete(v_file.CompleteFileName());
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }
        }

        /// <summary>
        /// Aplica o filtro do grid AJAX na lista de arquivos.
        /// </summary>
        /// <param name="p_completefilter">Filtro completo com valores separados por '&'.</param>
        public void FilterAllAttributes(string p_completefilter)
        {
            char[] v_separator;
            string[] v_filters;

            v_separator = new char[1];
            v_separator[0] = '&';

            v_filters = p_completefilter.Split(v_separator);

            // id
            if (v_filters[0] != null && v_filters[0] != "")
                this.FilterAttribute(Spartacus.Utils.FileAttributes.ID, v_filters[0]);

            // tipo
            if (v_filters[1] != null && v_filters[1] != "")
                this.FilterAttribute(Spartacus.Utils.FileAttributes.TYPE, v_filters[1]);

            // nome
            if (v_filters[2] != null && v_filters[2] != "")
                this.FilterAttribute(Spartacus.Utils.FileAttributes.NAME, v_filters[2]);

            // extensao
            if (v_filters[3] != null && v_filters[3] != "")
                this.FilterAttribute(Spartacus.Utils.FileAttributes.EXTENSION, v_filters[3]);

            // ultima data de alteracao
            if (v_filters[4] != null && v_filters[4] != "")
                this.FilterAttribute(Spartacus.Utils.FileAttributes.LASTWRITEDATE, v_filters[4]);

            // tamanho
            if (v_filters[5] != null && v_filters[5] != "")
                this.FilterAttribute(Spartacus.Utils.FileAttributes.SIZE, v_filters[5]);
        }

        /// <summary>
        /// Filtra a lista de arquivos por um único atributo.
        /// </summary>
        /// <param name="p_attribute">Atributo pelo qual a lista de arquivos vai ser filtrada.</param>
        /// <param name="p_filter">Valor do filtro.</param>
        private void FilterAttribute(Spartacus.Utils.FileAttributes p_attribute, string p_filter)
        {
            System.Text.RegularExpressions.Regex v_regex;
            int k;

            // clonando a lista de arquivos
            this.v_filesorig = (System.Collections.ArrayList) this.v_files.Clone();
            this.v_files.Clear();

            switch (p_attribute)
            {
                case Spartacus.Utils.FileAttributes.ID:
                    if (p_filter.Contains("%")) // expressao regular
                    {
                        v_regex = this.CreateRegex(p_filter);

                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (v_regex.IsMatch(((Spartacus.Utils.File)this.v_filesorig [k]).v_id.ToString()))
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    else // busca direta
                    {
                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (((Spartacus.Utils.File)this.v_filesorig [k]).v_id.ToString() == p_filter)
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    break;
                case Spartacus.Utils.FileAttributes.TYPE: // busca direta
                    for (k = 0; k < this.v_filesorig.Count; k++)
                    {
                        if (((Spartacus.Utils.File)this.v_filesorig [k]).v_filetype == Spartacus.Utils.FileType.DIRECTORY && p_filter == "D")
                            this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        if (((Spartacus.Utils.File)this.v_filesorig [k]).v_filetype == Spartacus.Utils.FileType.FILE && p_filter == "A")
                            this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                    }
                    break;
                case Spartacus.Utils.FileAttributes.NAME:
                    if (p_filter.Contains("%")) // expressao regular
                    {
                        v_regex = this.CreateRegex(p_filter);

                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (v_regex.IsMatch(((Spartacus.Utils.File)this.v_filesorig [k]).v_name))
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    else // busca direta
                    {
                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (((Spartacus.Utils.File)this.v_filesorig [k]).v_name == p_filter)
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    break;
                case Spartacus.Utils.FileAttributes.EXTENSION:
                    if (p_filter.Contains("%")) // expressao regular
                    {
                        v_regex = this.CreateRegex(p_filter);

                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (v_regex.IsMatch(((Spartacus.Utils.File)this.v_filesorig [k]).v_extension))
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    else // busca direta
                    {
                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (((Spartacus.Utils.File)this.v_filesorig [k]).v_extension == p_filter)
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    break;
                case Spartacus.Utils.FileAttributes.LASTWRITEDATE:
                    if (p_filter.Contains("%")) // expressao regular
                    {
                        v_regex = this.CreateRegex(p_filter);

                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (v_regex.IsMatch(((Spartacus.Utils.File)this.v_filesorig [k]).v_lastwritedate.ToString()))
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    else // busca direta
                    {
                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (((Spartacus.Utils.File)this.v_filesorig [k]).v_lastwritedate.ToString() == p_filter)
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    break;
                case Spartacus.Utils.FileAttributes.SIZE:
                    if (p_filter.Contains("%")) // expressao regular
                    {
                        v_regex = this.CreateRegex(p_filter);

                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (v_regex.IsMatch(((Spartacus.Utils.File)this.v_filesorig [k]).GetSize()))
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    else // busca direta
                    {
                        for (k = 0; k < this.v_filesorig.Count; k++)
                        {
                            if (((Spartacus.Utils.File)this.v_filesorig [k]).GetSize() == p_filter)
                                this.v_files.Add((Spartacus.Utils.File)this.v_filesorig [k]);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Constrói um objeto "expressão regular" baseado em um filtro com '%'.
        /// </summary>
        /// <returns>Objeto "expressão regular".</returns>
        /// <param name="p_filter">Filtro com '%'.</param>
        private System.Text.RegularExpressions.Regex CreateRegex(string p_filter)
        {
            System.Text.RegularExpressions.Regex v_regex;

            if (p_filter [0] != '%' && p_filter [p_filter.Length - 1] != '%') // filtro tem % no meio
                v_regex = new System.Text.RegularExpressions.Regex("^" + p_filter.Replace("%", ".*") + "$");
            else
            {
                if (p_filter [0] != '%' && p_filter [p_filter.Length - 1] == '%') // filtro termina com %
                    v_regex = new System.Text.RegularExpressions.Regex("^" + p_filter.Replace("%", ".*"));
                else
                {
                    if (p_filter [0] == '%' && p_filter [p_filter.Length - 1] != '%') // filtro comeca com %
                        v_regex = new System.Text.RegularExpressions.Regex(p_filter.Replace("%", ".*") + "$");
                    else
                        v_regex = new System.Text.RegularExpressions.Regex(p_filter.Replace("%", ".*"));
                }
            }

            return v_regex;
        }

        /// <summary>
        /// Ordena a lista de arquivos pela coluna passada como argumento, em ordem ascendente ou descendente.
        /// </summary>
        /// <param name="p_attribute">Coluna a ser ordenada (1, 2, 3, 4, 5 ou 6).</param>
        /// <param name="p_sorttype">Ordem (1: Ascendente ou 2: Descendente).</param>
        public void SortAttribute(int p_attribute, int p_sorttype)
        {
            switch (p_attribute)
            {
                case 1:
                    if (p_sorttype == 1)
                        this.v_files.Sort(new IdComparerAsc());
                    else
                        this.v_files.Sort(new IdComparerDesc());
                    break;
                case 2:
                    if (p_sorttype == 1)
                        this.v_files.Sort(new TypeComparerAsc());
                    else
                        this.v_files.Sort(new TypeComparerDesc());
                    break;
                case 3:
                    if (p_sorttype == 1)
                        this.v_files.Sort(new NameComparerAsc());
                    else
                        this.v_files.Sort(new NameComparerDesc());
                    break;
                case 4:
                    if (p_sorttype == 1)
                        this.v_files.Sort(new ExtensionComparerAsc());
                    else
                        this.v_files.Sort(new ExtensionComparerDesc());
                    break;
                case 5:
                    if (p_sorttype == 1)
                        this.v_files.Sort(new LastWriteDateComparerAsc());
                    else
                        this.v_files.Sort(new LastWriteDateComparerDesc());
                    break;
                case 6:
                    if (p_sorttype == 1)
                        this.v_files.Sort(new SizeComparerAsc());
                    else
                        this.v_files.Sort(new SizeComparerDesc());
                    break;
                default:
                    break;
            }
        }

        private class IdComparerAsc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((Spartacus.Utils.File)x).v_id.CompareTo(((Spartacus.Utils.File)y).v_id);
            }
        }

        private class IdComparerDesc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return -1 * ((Spartacus.Utils.File)x).v_id.CompareTo(((Spartacus.Utils.File)y).v_id);
            }
        }

        private class TypeComparerAsc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((Spartacus.Utils.File)x).v_filetype.CompareTo(((Spartacus.Utils.File)y).v_filetype);
            }
        }

        private class TypeComparerDesc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return -1 * ((Spartacus.Utils.File)x).v_filetype.CompareTo(((Spartacus.Utils.File)y).v_filetype);
            }
        }

        private class NameComparerAsc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((Spartacus.Utils.File)x).v_name.CompareTo(((Spartacus.Utils.File)y).v_name);
            }
        }

        private class NameComparerDesc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return -1 * ((Spartacus.Utils.File)x).v_name.CompareTo(((Spartacus.Utils.File)y).v_name);
            }
        }

        private class ExtensionComparerAsc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((Spartacus.Utils.File)x).v_extension.CompareTo(((Spartacus.Utils.File)y).v_extension);
            }
        }

        private class ExtensionComparerDesc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return -1 * ((Spartacus.Utils.File)x).v_extension.CompareTo(((Spartacus.Utils.File)y).v_extension);
            }
        }

        private class LastWriteDateComparerAsc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((Spartacus.Utils.File)x).v_lastwritedate.CompareTo(((Spartacus.Utils.File)y).v_lastwritedate);
            }
        }

        private class LastWriteDateComparerDesc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return -1 * ((Spartacus.Utils.File)x).v_lastwritedate.CompareTo(((Spartacus.Utils.File)y).v_lastwritedate);
            }
        }

        private class SizeComparerAsc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((Spartacus.Utils.File)x).v_size.CompareTo(((Spartacus.Utils.File)y).v_size);
            }
        }

        private class SizeComparerDesc : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return -1 * ((Spartacus.Utils.File)x).v_size.CompareTo(((Spartacus.Utils.File)y).v_size);
            }
        }

        /// <summary>
        /// Atribui um número de página a cada arquivo da lista de arquivos e conta o número de páginas.
        /// </summary>
        /// <param name="p_numfilesperpage">Número de arquivos por página.</param>
        public void SplitIntoPages(int p_numfilesperpage)
        {
            int v_currentpage, i, j;

            this.v_numfilesperpage = p_numfilesperpage;

            v_currentpage = 1;
            i = 0;
            while (i < this.v_files.Count)
            {
                j = 0;
                while (j < this.v_numfilesperpage && i < this.v_files.Count)
                {
                    ((Spartacus.Utils.File)this.v_files [i]).v_pagenumber = v_currentpage;

                    j++;
                    i++;
                }

                v_currentpage++;
            }

            this.v_numpages = v_currentpage - 1;
        }

        /// <summary>
        /// Cria um arquivo ZIP a partir de um diretório, no diretório pai do mesmo diretório.
        /// </summary>
        /// <returns>Arquivo ZIP.</returns>
        /// <param name="p_zipfilename">Nome do arquivo ZIP a ser criado.</param>
        /// <param name="p_directory">Diretório a ser compactado.</param>
        public Spartacus.Utils.File CompressDirectory(string p_zipfilename, Spartacus.Utils.File p_directory)
        {
            Spartacus.ThirdParty.ZipStorer v_zipstorer;
            Spartacus.Utils.File v_zipfiletmp, v_zipfile;
            Spartacus.Utils.FileArray v_filearray;
            System.IO.FileInfo v_fileinfo;

            if (p_directory.v_pathseparator == Spartacus.Utils.PathSeparator.SLASH)
                v_zipfiletmp = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_directory.v_path + "/" + p_zipfilename);
            else
                v_zipfiletmp = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_directory.v_path + "\\" + p_zipfilename);

            try
            {
                v_zipstorer = Spartacus.ThirdParty.ZipStorer.Create(v_zipfiletmp.CompleteFileName(), "Generated with ZipStorer (by Jaime Olivares) embedded in Spartacus (by William Ivanski)");
                v_zipstorer.EncodeUTF8 = true;
                
                v_filearray = new Spartacus.Utils.FileArray(p_directory.CompleteFileName(), "*", System.IO.SearchOption.AllDirectories);
                foreach (Spartacus.Utils.File v_file in v_filearray.v_files)
                    v_zipstorer.AddFile(Spartacus.ThirdParty.ZipStorer.Compression.Deflate, v_file.CompleteFileName(), v_file.CompleteFileName().Replace(p_directory.v_path, ""), "");
                v_zipstorer.Close();

                v_fileinfo = new System.IO.FileInfo(v_zipfiletmp.CompleteFileName());

                v_zipfile = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, v_zipfiletmp.CompleteFileName(), v_fileinfo.LastWriteTime, v_fileinfo.Length);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }

            return v_zipfile;
        }
    }
}

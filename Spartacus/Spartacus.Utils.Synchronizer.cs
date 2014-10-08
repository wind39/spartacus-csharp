/*
The MIT License (MIT)

Copyright (c) 2014 William Ivanski

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
    /// A classe <see cref="Spartacus.Utils.Synchronizer"/> pode obter a árvore de arquivos automaticamente ou através de arquivo.
    /// </summary>
    public enum TreeType
    {
        AUTOMATIC,
        FROMFILE
    }

    /// <summary>
    /// Ação a ser tomada ao ser detectada a diferença.
    /// </summary>
    public enum SyncAction
    {
        DONOTHING,
        CREATE,
        COPY,
        DELETE
    }

    /// <summary>
    /// Localidade da pasta.
    /// </summary>
    public enum Locality
    {
        LOCAL,
        REMOTE
    }

    /// <summary>
    /// Direção da pasta.
    /// </summary>
    public enum Direction
    {
        LEFT,
        RIGHT
    }

    /// <summary>
    /// Classe Synchronizer.
    /// Sincroniza duas pastas.
    /// </summary>
    public class Synchronizer
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.Synchronizer"/>.
        /// </summary>
        public Synchronizer()
        {
        }

        /// <summary>
        /// Executa a comparação de diretórios e a criação de script de sincronização.
        /// </summary>
        /// <param name='p_treetype'>
        /// Forma de obtenção da árvore de arquivos.
        /// </param>
        /// <param name='p_leftfilename'>
        /// Nome do arquivo que contém a árvore de arquivos da esquerda.
        /// </param>
        /// <param name='p_rightfilename'>
        /// Nome do arquivo que contém a árvore de arquivos da direita.
        /// </param>
        /// <param name='p_existleft_directory_action'>
        /// Ação a ser tomada ao ser identificado um diretório que existe apenas do lado esquerdo.
        /// </param>
        /// <param name='p_existleft_file_action'>
        /// Ação a ser tomada ao ser identificado um arquivo que existe apenas do lado esquerdo.
        /// </param>
        /// <param name='p_existright_directory_action'>
        /// Ação a ser tomada ao ser identificado um diretório que existe apenas do lado direito.
        /// </param>
        /// <param name='p_existright_file_action'>
        /// Ação a ser tomada ao ser identificado um arquivo que existe apenas do lado direito.
        /// </param>
        /// <param name='p_newerleft_file_action'>
        /// Ação a ser tomada ao ser identificado um arquivo mais recente no lado esquerdo.
        /// </param>
        /// <param name='p_newerright_file_action'>
        /// Ação a ser tomada ao ser identificado um arquivo mais recente no lado direito.
        /// </param>
        public void Execute(
            Spartacus.Utils.TreeType p_treetype,
            string p_leftfilename,
            string p_rightfilename,
            Spartacus.Utils.SyncAction p_existleft_directory_action,
            Spartacus.Utils.SyncAction p_existleft_file_action,
            Spartacus.Utils.SyncAction p_existright_directory_action,
            Spartacus.Utils.SyncAction p_existright_file_action,
            Spartacus.Utils.SyncAction p_newerleft_file_action,
            Spartacus.Utils.SyncAction p_newerright_file_action
            )
        {
            System.Collections.ArrayList v_left_directorylist;
            System.Collections.ArrayList v_left_filelist;
            System.Collections.ArrayList v_right_directorylist;
            System.Collections.ArrayList v_right_filelist;
            System.Collections.ArrayList v_existleft_directorylist;
            System.Collections.ArrayList v_existleft_filelist;
            System.Collections.ArrayList v_existright_directorylist;
            System.Collections.ArrayList v_existright_filelist;
            System.Collections.ArrayList v_newerleft_directorylist;
            System.Collections.ArrayList v_newerleft_filelist;
            System.Collections.ArrayList v_newerright_directorylist;
            System.Collections.ArrayList v_newerright_filelist;
            System.IO.FileStream v_file;
            System.IO.StreamReader v_reader;

            v_left_directorylist = new System.Collections.ArrayList();
            v_left_filelist = new System.Collections.ArrayList();
            v_right_directorylist = new System.Collections.ArrayList();
            v_right_filelist = new System.Collections.ArrayList();
            v_existleft_directorylist = new System.Collections.ArrayList();
            v_existleft_filelist = new System.Collections.ArrayList();
            v_existright_directorylist = new System.Collections.ArrayList();
            v_existright_filelist = new System.Collections.ArrayList();
            v_newerleft_directorylist = new System.Collections.ArrayList();
            v_newerleft_filelist = new System.Collections.ArrayList();
            v_newerright_directorylist = new System.Collections.ArrayList();
            v_newerright_filelist = new System.Collections.ArrayList();

            // construindo arvore da esquerda
            v_file = new System.IO.FileStream(p_leftfilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            v_reader = new System.IO.StreamReader(v_file);
            this.BuildTree(v_reader, v_left_directorylist, v_left_filelist);

            // construindo arvore da direita
            v_file = new System.IO.FileStream(p_rightfilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            v_reader = new System.IO.StreamReader(v_file);
            this.BuildTree(v_reader, v_right_directorylist, v_right_filelist);

            // comparando duas arvores
            this.CompareTrees(
                v_left_directorylist,
                v_left_filelist,
                v_right_directorylist, v_right_filelist,
                v_existleft_directorylist,
                v_existleft_filelist,
                v_existright_directorylist,
                v_existright_filelist,
                v_newerleft_directorylist,
                v_newerleft_filelist,
                v_newerright_directorylist,
                v_newerright_filelist);

            // escrevendo a lista de diferencas em arquivo

            System.Console.WriteLine("LEFTDIR=");
            System.Console.WriteLine("SSHUSER=");
            System.Console.WriteLine("SSHHOST=");
            System.Console.WriteLine("SSHPORT=");
            System.Console.WriteLine("RIGHTDIR=");
            System.Console.WriteLine();

            System.Console.WriteLine("# DIRETÓRIOS E ARQUIVOS QUE EXISTEM NA ESQUERDA E NAO EXISTEM NA DIREITA:");
            this.WriteSyncScript(v_existleft_directorylist, v_existleft_filelist, p_existleft_directory_action, p_existleft_file_action, Spartacus.Utils.Locality.REMOTE, Spartacus.Utils.Direction.RIGHT);
            System.Console.WriteLine();

            System.Console.WriteLine("# DIRETÓRIOS E ARQUIVOS QUE SÃO MAIS NOVOS NA ESQUERDA DO QUE NA DIREITA:");
            this.WriteSyncScript(v_newerleft_directorylist, v_newerleft_filelist, Spartacus.Utils.SyncAction.DONOTHING, p_newerleft_file_action, Spartacus.Utils.Locality.REMOTE, Spartacus.Utils.Direction.RIGHT);
            System.Console.WriteLine();

            System.Console.WriteLine("# DIRETÓRIOS E ARQUIVOS QUE EXISTEM NA DIREITA E NAO EXISTEM NA ESQUERDA:");
            this.WriteSyncScript(v_existright_directorylist, v_existright_filelist, p_existright_directory_action, p_existright_file_action, Spartacus.Utils.Locality.REMOTE, Spartacus.Utils.Direction.RIGHT);
            System.Console.WriteLine();

            System.Console.WriteLine("# DIRETÓRIOS E ARQUIVOS QUE SÃO MAIS NOVOS NA DIREITA DO QUE NA ESQUERDA:");
            this.WriteSyncScript(v_newerright_directorylist, v_newerright_filelist, Spartacus.Utils.SyncAction.DONOTHING, p_newerright_file_action, Spartacus.Utils.Locality.REMOTE, Spartacus.Utils.Direction.RIGHT);
            System.Console.WriteLine();
        }

        /// <summary>
        /// Constroi uma árvore de arquivos baseada na lista de arquivos.
        /// </summary>
        /// <param name='p_left_directorylist'>
        /// Lista de diretórios da esquerda.
        /// </param>
        /// <param name='p_left_filelist'>
        /// Lista de arquivos da esquerda.
        /// </param>
        /// <param name='p_right_directorylist'>
        /// Lista de diretórios da direita.
        /// </param>
        /// <param name='p_right_filelist'>
        /// Lista de arquivos da direita.
        /// </param>
        private void BuildTree(System.IO.StreamReader p_reader, System.Collections.ArrayList p_directorylist, System.Collections.ArrayList p_filelist)
        {
            string v_line;
            Spartacus.Utils.File v_directory;
            Spartacus.Utils.File v_file;
            int v_directoryid;
            int v_fileid;
            System.Text.RegularExpressions.RegexOptions v_options;
            System.Text.RegularExpressions.Regex v_regex;
            char [] v_ch;
            string [] v_array;
            string v_newline;
            string v_filename;
            System.DateTime v_date;
            long v_size;
            int k;

            v_directoryid = 1;
            while (! p_reader.EndOfStream)
            {
                v_line = p_reader.ReadLine();

                // se começa com ponto, significa que é diretório
                if (v_line.StartsWith("."))
                {
                    //System.Console.WriteLine("Diretório: " + v_line);

                    // criando um diretório novo
                    v_directory = new Spartacus.Utils.File(
                        v_directoryid,
                        0,
                        Spartacus.Utils.FileType.DIRECTORY,
                        v_line.Replace(":", ""),
                        Spartacus.Utils.PathSeparator.SLASH,
                        System.DateTime.Now,
                        0
                        );

                    // construindo lista de arquivos e diretórios que tem dentro do diretorio atual

                    v_line = p_reader.ReadLine(); // ignorando linha "total"
                    v_fileid = 1;
                    v_line = p_reader.ReadLine();
                    while (! p_reader.EndOfStream && v_line != "")
                    {
                        // quebrando linha
                        v_options = System.Text.RegularExpressions.RegexOptions.Compiled;
                        v_regex = new System.Text.RegularExpressions.Regex(@"[ ]{2,}", v_options);
                        v_newline = v_regex.Replace(v_line, @" ");
                        v_ch = new char[1];
                        v_ch[0] = ' ';
                        v_array = v_newline.Split(v_ch);

                        // pegando nome
                        v_filename = "";
                        for (k = 8; k < v_array.Length; k++)
                        {
                            v_filename += v_array[k];
                            if (k < v_array.Length-1)
                                v_filename += " ";
                        }

                        // pegando ultima data de alteracao
                        v_date = this.GetLastWriteDateTime(v_array[6], v_array[5], v_array[7]);

                        // pegando tamanho
                        v_size = System.Int64.Parse(v_array[4]);

                        //System.Console.WriteLine("Nova Linha: " + v_newline);
                        //System.Console.WriteLine("Arquivo: " + v_filename);
                        //System.Console.WriteLine("Data: " + v_date.ToString());
                        //System.Console.WriteLine("Tamanho: " + v_size.ToString());

                        // se começa com d, significa que é diretório
                        /*if (v_line.StartsWith("d"))
                        {
                            v_file = new Spartacus.Utils.File(
                                v_fileid,
                                v_directoryid,
                                Spartacus.Utils.FileType.DIRECTORY,
                                v_directory.CompleteFileName() + "/" + v_filename,
                                Spartacus.Utils.PathSeparator.SLASH,
                                System.DateTime.Now,
                                0
                            );
                        }
                        else // senao, é arquivo
                        {
                            v_file = new Spartacus.Utils.File(
                                v_fileid,
                                v_directoryid,
                                Spartacus.Utils.FileType.FILE,
                                v_directory.CompleteFileName() + "/" + v_filename,
                                Spartacus.Utils.PathSeparator.SLASH,
                                v_date,
                                v_size
                            );
                        }

                        p_filelist.Add(v_file);
                        v_fileid++;
                        */

                        // se começa com d, significa que é diretório
                        if (! v_line.StartsWith("d"))
                        {
                            v_file = new Spartacus.Utils.File(
                                v_fileid,
                                v_directoryid,
                                Spartacus.Utils.FileType.FILE,
                                v_directory.CompleteFileName() + "/" + v_filename,
                                Spartacus.Utils.PathSeparator.SLASH,
                                v_date,
                                v_size
                                );

                            p_filelist.Add(v_file);
                            v_fileid++;
                        }

                        v_line = p_reader.ReadLine();
                    }

                    p_directorylist.Add(v_directory);
                    v_directoryid++;
                }
            }
        }

        /// <summary>
        /// Compara duas árvores de arquivos.
        /// </summary>
        /// <param name='p_left_directorylist'>
        /// Lista de diretórios da esquerda.
        /// </param>
        /// <param name='p_left_filelist'>
        /// Lista de arquivos da esquerda.
        /// </param>
        /// <param name='p_right_directorylist'>
        /// Lista de diretórios da direita.
        /// </param>
        /// <param name='p_right_filelist'>
        /// Lista de arquivos da direita.
        /// </param>
        /// <param name='p_existleft_directorylist'>
        /// Lista de diretórios que estão apenas na esquerda.
        /// </param>
        /// <param name='p_existleft_filelist'>
        /// Lista de arquivos que estão apenas na esquerda.
        /// </param>
        /// <param name='p_existright_directorylist'>
        /// Lista de diretórios que estão apenas na direita.
        /// </param>
        /// <param name='p_existright_filelist'>
        /// Lista de arquivos que estão apenas na direita.
        /// </param>
        /// <param name='p_newerleft_directorylist'>
        /// Lista de diretórios que estão apenas na esquerda.
        /// </param>
        /// <param name='p_newerleft_filelist'>
        /// Lista de arquivos que estão apenas na esquerda.
        /// </param>
        /// <param name='p_newerright_directorylist'>
        /// Lista de diretórios que estão apenas na direita.
        /// </param>
        /// <param name='p_newerright_filelist'>
        /// Lista de arquivos que estão apenas na direita.
        /// </param>
        private void CompareTrees(
            System.Collections.ArrayList p_left_directorylist,
            System.Collections.ArrayList p_left_filelist,
            System.Collections.ArrayList p_right_directorylist,
            System.Collections.ArrayList p_right_filelist,
            System.Collections.ArrayList p_existleft_directorylist,
            System.Collections.ArrayList p_existleft_filelist,
            System.Collections.ArrayList p_existright_directorylist,
            System.Collections.ArrayList p_existright_filelist,
            System.Collections.ArrayList p_newerleft_directorylist,
            System.Collections.ArrayList p_newerleft_filelist,
            System.Collections.ArrayList p_newerright_directorylist,
            System.Collections.ArrayList p_newerright_filelist
            )
        {
            Spartacus.Utils.File v_directoryleft = null, v_directoryright = null;
            Spartacus.Utils.File v_fileleft = null, v_fileright = null;
            int di, dj, fi, fj;
            bool v_directorynotfound;
            bool v_filenotfound, v_filefoundbutnewer;
            bool v_inseriufilenotfound, v_inseriufilefoundbutnewer;
            System.Collections.ArrayList v_templeft_filelist, v_tempright_filelist;

            v_templeft_filelist = new System.Collections.ArrayList();
            v_tempright_filelist = new System.Collections.ArrayList();

            // primeira passada: da esquerda para a direita
            for (di = 0; di < p_left_directorylist.Count; di++)
            {
                v_directoryleft = (Spartacus.Utils.File) p_left_directorylist[di];

                // procurando diretorio da esquerda na lista da direita
                dj = 0;
                v_directorynotfound = true;
                while (dj < p_right_directorylist.Count && v_directorynotfound)
                {
                    v_directoryright = (Spartacus.Utils.File) p_right_directorylist[dj];

                    if (v_directoryleft.CompleteFileName() == v_directoryright.CompleteFileName())
                        v_directorynotfound = false;
                    else
                        dj++;
                }

                // se o diretorio da esquerda nao existe no lado direito
                if (v_directorynotfound)
                {
                    p_existleft_directorylist.Add(v_directoryleft);
                    for (fi = 0; fi < p_left_filelist.Count; fi++)
                    {
                        v_fileleft = (Spartacus.Utils.File) p_left_filelist[fi];

                        if (v_fileleft.v_parentid == v_directoryleft.v_id)
                            p_existleft_filelist.Add(v_fileleft);
                    }
                }
                else // se os diretorios existem em ambos os lados
                {
                    // colocando arquivos do diretorio da esquerda numa lista temporaria
                    v_templeft_filelist.Clear();
                    for (fi = 0; fi < p_left_filelist.Count; fi++)
                    {
                        v_fileleft = (Spartacus.Utils.File) p_left_filelist[fi];

                        if (v_fileleft.v_parentid == v_directoryleft.v_id)
                            v_templeft_filelist.Add(v_fileleft);
                    }

                    // colocando arquivos do diretorio da direita numa lista temporaria
                    v_tempright_filelist.Clear();
                    for (fi = 0; fi < p_right_filelist.Count; fi++)
                    {
                        v_fileright = (Spartacus.Utils.File) p_right_filelist[fi];

                        if (v_fileright.v_parentid == v_directoryright.v_id)
                            v_tempright_filelist.Add(v_fileright);
                    }

                    // comparando arquivos da esquerda para a direita
                    v_inseriufilenotfound = false;
                    v_inseriufilefoundbutnewer = false;
                    for (fi = 0; fi < v_templeft_filelist.Count; fi++)
                    {
                        v_fileleft = (Spartacus.Utils.File) v_templeft_filelist[fi];

                        fj = 0;
                        v_filenotfound = true;
                        v_filefoundbutnewer = false;
                        while (fj < v_tempright_filelist.Count && v_filenotfound)
                        {
                            v_fileright = (Spartacus.Utils.File)v_tempright_filelist[fj];

                            if (v_fileleft.v_name == v_fileright.v_name)
                            {
                                v_filenotfound = false;
                                if (v_fileleft.v_lastwritedate > v_fileright.v_lastwritedate && v_fileleft.v_size != v_fileright.v_size)
                                    v_filefoundbutnewer = true;
                            } else
                                fj++;
                        }

                        // se o arquivo existe apenas na esquerda
                        if (v_filenotfound)
                        {
                            p_existleft_filelist.Add(v_fileleft);
                            if (! v_inseriufilenotfound)
                            {
                                p_existleft_directorylist.Add(v_directoryleft);
                                v_inseriufilenotfound = true;
                            }
                        }

                        // se o arquivo eh mais novo na esquerda
                        if (v_filefoundbutnewer)
                        {
                            p_newerleft_filelist.Add(v_fileleft);
                            if (! v_inseriufilefoundbutnewer)
                            {
                                p_newerleft_directorylist.Add(v_directoryleft);
                                v_inseriufilefoundbutnewer = true;
                            }
                        }
                    }

                    // comparando arquivos da direita para a esquerda
                    v_inseriufilenotfound = false;
                    v_inseriufilefoundbutnewer = false;
                    for (fi = 0; fi < v_tempright_filelist.Count; fi++)
                    {
                        v_fileright = (Spartacus.Utils.File) v_tempright_filelist[fi];

                        fj = 0;
                        v_filenotfound = true;
                        v_filefoundbutnewer = false;
                        while (fj < v_templeft_filelist.Count && v_filenotfound)
                        {
                            v_fileleft = (Spartacus.Utils.File) v_templeft_filelist[fj];

                            if (v_fileright.v_name == v_fileleft.v_name)
                            {
                                v_filenotfound = false;
                                if (v_fileright.v_lastwritedate > v_fileleft.v_lastwritedate && v_fileright.v_size != v_fileleft.v_size)
                                    v_filefoundbutnewer = true;
                            }
                            else
                                fj++;
                        }

                        // se o arquivo existe apenas na direita
                        if (v_filenotfound)
                        {
                            p_existright_filelist.Add(v_fileright);
                            if (! v_inseriufilenotfound)
                            {
                                p_existright_directorylist.Add(v_directoryright);
                                v_inseriufilenotfound = true;
                            }
                        }

                        // se o arquivo eh mais novo na direita
                        if (v_filefoundbutnewer)
                        {
                            p_newerright_filelist.Add(v_fileright);
                            if (! v_inseriufilefoundbutnewer)
                            {
                                p_newerright_directorylist.Add(v_directoryright);
                                v_inseriufilefoundbutnewer = true;
                            }
                        }
                    }
                }
            }

            // segunda passada: da direita para a esquerda
            for (di = 0; di < p_right_directorylist.Count; di++)
            {
                v_directoryright = (Spartacus.Utils.File) p_right_directorylist[di];

                // procurando diretorio da direita na lista da esquerda
                dj = 0;
                v_directorynotfound = true;
                while (dj < p_left_directorylist.Count && v_directorynotfound)
                {
                    v_directoryleft = (Spartacus.Utils.File) p_left_directorylist[dj];

                    if (v_directoryright.CompleteFileName() == v_directoryleft.CompleteFileName())
                        v_directorynotfound = false;
                    else
                        dj++;
                }

                // se o diretorio da direita nao existe no lado esquerdo
                if (v_directorynotfound)
                {
                    p_existright_directorylist.Add(v_directoryright);
                    for (fi = 0; fi < p_right_filelist.Count; fi++)
                    {
                        v_fileright = (Spartacus.Utils.File) p_right_filelist[fi];

                        if (v_fileright.v_parentid == v_directoryright.v_id)
                            p_existright_filelist.Add(v_fileright);
                    }
                }
            }
        }

        /// <summary>
        /// Pega a última data de alteração.
        /// </summary>
        /// <returns>
        /// A última data de alteração.
        /// </returns>
        /// <param name='p_day'>
        /// Dia.
        /// </param>
        /// <param name='p_month'>
        /// Mês.
        /// </param>
        /// <param name='p_year'>
        /// Ano.
        /// </param>
        private System.DateTime GetLastWriteDateTime(string p_day, string p_month, string p_year)
        {
            System.DateTime v_datetime;
            string v_day, v_month, v_year, v_time;

            if (System.Int32.Parse(p_day) < 10)
                v_day = "0" + p_day;
            else
                v_day = p_day;

            switch (p_month)
            {
                case "Jan":
                    v_month = "01";
                    break;
                    case "Fev":
                    v_month = "02";
                    break;
                    case "Feb":
                    v_month = "02";
                    break;
                    case "Mar":
                    v_month = "03";
                    break;
                    case "Abr":
                    v_month = "04";
                    break;
                    case "Apr":
                    v_month = "04";
                    break;
                    case "Mai":
                    v_month = "05";
                    break;
                    case "May":
                    v_month = "05";
                    break;
                    case "Jun":
                    v_month = "06";
                    break;
                    case "Jul":
                    v_month = "07";
                    break;
                    case "Ago":
                    v_month = "08";
                    break;
                    case "Aug":
                    v_month = "08";
                    break;
                    case "Set":
                    v_month = "09";
                    break;
                    case "Sep":
                    v_month = "09";
                    break;
                    case "Out":
                    v_month = "10";
                    break;
                    case "Oct":
                    v_month = "10";
                    break;
                    case "Nov":
                    v_month = "11";
                    break;
                    case "Dez":
                    v_month = "12";
                    break;
                    case "Dec":
                    v_month = "12";
                    break;
                    default:
                    v_month = "00";
                    break;
            }

            if (p_year.Contains(":"))
            {
                v_year = System.DateTime.Now.Year.ToString();
                v_time = p_year + ":00";
            }
            else
            {
                v_year = p_year;
                v_time = "00:00:00";
            }

            try
            {
                v_datetime = System.DateTime.Parse(v_day + "/" + v_month + "/" + v_year + " " + v_time);
            }
            catch (System.FormatException)
            {
                System.Console.WriteLine("ERRO: Não conseguiu formatar a string de data e hora.");
                System.Console.WriteLine("Dia = {0}, Mês = {1}, Ano = {2}", p_day, p_month, p_year);
                v_datetime = System.DateTime.Now;
            }

            return v_datetime;
        }

        /// <summary>
        /// Escreve a lista de arquivos.
        /// </summary>
        /// <param name='p_directorylist'>
        /// Lista de diretórios.
        /// </param>
        /// <param name='p_filelist'>
        /// Lista de arquivos.
        /// </param>
        /// <param name='p_locality'>
        /// Localidade da pasta analisada.
        /// </param>
        /// <param name='p_direction'>
        /// Direção da pasta analisada.
        /// </param>
        private void WriteSyncScript(
            System.Collections.ArrayList p_directorylist,
            System.Collections.ArrayList p_filelist,
            Spartacus.Utils.SyncAction p_directory_action,
            Spartacus.Utils.SyncAction p_file_action,
            Spartacus.Utils.Locality p_locality,
            Spartacus.Utils.Direction p_direction
            )
        {
            Spartacus.Utils.File v_directory, v_file;
            int i, j;
            long v_totalsize;
            string v_name, v_basename;

            v_totalsize = 0;
            for (i = 0; i < p_directorylist.Count; i++)
            {
                v_directory = (Spartacus.Utils.File) p_directorylist[i];

                System.Console.WriteLine("echo \"Diretório {0} ({1}/{2})\"", v_directory.CompleteFileName(), i+1, p_directorylist.Count);

                v_name = v_directory.CompleteFileName().Substring(1).Replace(" ", "\\ ");

                if (p_locality == Spartacus.Utils.Locality.LOCAL)
                {
                    switch (p_directory_action)
                    {
                        case Spartacus.Utils.SyncAction.CREATE:
                            if (p_direction == Spartacus.Utils.Direction.LEFT)
                            {
                                //System.Console.WriteLine("echo \"mkdir -p $LEFTDIR" + v_name + "\"");
                                System.Console.WriteLine("mkdir -p \"$LEFTDIR" + v_name + "\"");
                            }
                            else
                            {
                                //System.Console.WriteLine("echo \"mkdir -p $RIGHTDIR" + v_name + "\"");
                                System.Console.WriteLine("mkdir -p \"$RIGHTDIR" + v_name + "\"");
                            }
                            break;
                            case Spartacus.Utils.SyncAction.DELETE:
                            if (p_direction == Spartacus.Utils.Direction.LEFT)
                            {
                                //System.Console.WriteLine("echo \"rm -f $LEFTDIR" + v_name + "\"");
                                System.Console.WriteLine("rm -rf \"$LEFTDIR" + v_name + "\"");
                            }
                            else
                            {
                                //System.Console.WriteLine("echo \"rm -f $RIGHTDIR" + v_name + "\"");
                                System.Console.WriteLine("rm -rf \"$RIGHTDIR" + v_name + "\"");
                            }
                            break;
                            case Spartacus.Utils.SyncAction.DONOTHING:
                            break;
                            default:
                            break;
                    }
                }
                else
                {
                    switch (p_directory_action)
                    {
                        case Spartacus.Utils.SyncAction.CREATE:
                            if (p_direction == Spartacus.Utils.Direction.LEFT)
                            {
                                //System.Console.WriteLine("echo \"ssh $SSHUSER@$SSHHOST -p $SSHPORT mkdir -p $LEFTDIR" + v_name + "\"");
                                System.Console.WriteLine("ssh $SSHUSER@$SSHHOST -p $SSHPORT mkdir -p \"$LEFTDIR" + v_name + "\"");
                            }
                            else
                            {
                                //System.Console.WriteLine("echo \"ssh $SSHUSER@$SSHHOST -p $SSHPORT mkdir -p $RIGHTDIR" + v_name + "\"");
                                System.Console.WriteLine("ssh $SSHUSER@$SSHHOST -p $SSHPORT mkdir -p \"$RIGHTDIR" + v_name + "\"");
                            }
                            break;
                            case Spartacus.Utils.SyncAction.DELETE:
                            if (p_direction == Spartacus.Utils.Direction.LEFT)
                            {
                                //System.Console.WriteLine("echo \"ssh $SSHUSER@$SSHHOST -p $SSHPORT rm -f $LEFTDIR" + v_name + "\"");
                                System.Console.WriteLine("ssh $SSHUSER@$SSHHOST -p $SSHPORT rm -rf \"$LEFTDIR" + v_name + "\"");
                            }
                            else
                            {
                                //System.Console.WriteLine("echo \"ssh $SSHUSER@$SSHHOST -p $SSHPORT rm -f $RIGHTDIR" + v_name + "\"");
                                System.Console.WriteLine("ssh $SSHUSER@$SSHHOST -p $SSHPORT rm -rf \"$RIGHTDIR" + v_name + "\"");
                            }
                            break;
                            case Spartacus.Utils.SyncAction.DONOTHING:
                            break;
                            default:
                            break;
                    }
                }

                for (j = 0; j < p_filelist.Count; j++)
                {
                    v_file = (Spartacus.Utils.File) p_filelist[j];

                    if (v_file.v_filetype == Spartacus.Utils.FileType.FILE && v_file.v_parentid == v_directory.v_id)
                    {
                        System.Console.WriteLine("echo \"Arquivo {0} ({1}/{2}) [{3}, {4} bytes]\"", v_file.CompleteFileName(), i+1, p_directorylist.Count, v_file.v_lastwritedate.ToString(), v_file.v_size.ToString());

                        //v_name = v_file.CompleteFileName().Substring(1).Replace(" ", "\\ ");
                        //v_basename = v_file.v_path.Substring(1).Replace(" ", "\\ ");
                        v_name = v_file.CompleteFileName().Substring(1);
                        v_basename = v_file.v_path.Substring(1);

                        if (p_locality == Spartacus.Utils.Locality.LOCAL)
                        {
                            switch (p_file_action)
                            {
                                case Spartacus.Utils.SyncAction.COPY:
                                    if (p_direction == Spartacus.Utils.Direction.LEFT)
                                    {
                                        //System.Console.WriteLine("echo \"cp -f $RIGHTDIR" + v_name + " $LEFTDIR" + v_basename + "\"");
                                        System.Console.WriteLine("cp -f \"$RIGHTDIR" + v_name + "\" \"$LEFTDIR" + v_basename + "\"");
                                    }
                                    else
                                    {
                                        //System.Console.WriteLine("echo \"cp -f $LEFTDIR" + v_name + " $RIGHTDIR" + v_basename + "\"");
                                        System.Console.WriteLine("cp -f \"$LEFTDIR" + v_name + "\" \"$RIGHTDIR" + v_basename + "\"");
                                    }
                                    break;
                                    case Spartacus.Utils.SyncAction.DELETE:
                                    if (p_direction == Spartacus.Utils.Direction.LEFT)
                                    {
                                        //System.Console.WriteLine("echo \"rm -f $RIGHTDIR" + v_name + " $LEFTDIR" + v_basename + "\"");
                                        System.Console.WriteLine("rm -rf \"$LEFTDIR" + v_name + "\"");
                                    }
                                    else
                                    {
                                        //System.Console.WriteLine("echo \"rm -f $LEFTDIR" + v_name + " $RIGHTDIR" + v_basename + "\"");
                                        System.Console.WriteLine("rm -rf \"$RIGHTDIR" + v_name + "\"");
                                    }
                                    break;
                                    case Spartacus.Utils.SyncAction.DONOTHING:
                                    break;
                                    default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (p_file_action)
                            {
                                case Spartacus.Utils.SyncAction.COPY:
                                    if (p_direction == Spartacus.Utils.Direction.LEFT)
                                    {
                                        //System.Console.WriteLine("echo \"scp -P $SSHPORT $RIGHTDIR" + v_name + " $SSHUSER@$SSHHOST:$LEFTDIR" + v_basename + "\"");
                                        System.Console.WriteLine("scp -P $SSHPORT \"$RIGHTDIR" + v_name + "\" $SSHUSER@$SSHHOST:\"'$LEFTDIR" + v_basename + "'\"");
                                    }
                                    else
                                    {
                                        //System.Console.WriteLine("echo \"scp -P $SSHPORT $LEFTDIR" + v_name + " $SSHUSER@$SSHHOST:$RIGHTDIR" + v_basename + "\"");
                                        System.Console.WriteLine("scp -P $SSHPORT \"$LEFTDIR" + v_name + "\" $SSHUSER@$SSHHOST:\"'$RIGHTDIR" + v_basename + "'\"");
                                    }
                                    break;
                                    case Spartacus.Utils.SyncAction.DELETE:
                                    if (p_direction == Spartacus.Utils.Direction.LEFT)
                                    {
                                        //System.Console.WriteLine("echo \"ssh $SSHUSER@$SSHHOST -p $SSHPORT rm -f $LEFTDIR" + v_name + "\"");
                                        System.Console.WriteLine("ssh $SSHUSER@$SSHHOST -p $SSHPORT rm -rf \"$LEFTDIR" + v_name + "\"");
                                    }
                                    else
                                    {
                                        //System.Console.WriteLine("echo \"ssh $SSHUSER@$SSHHOST -p $SSHPORT rm -f $RIGHTDIR" + v_name + "\"");
                                        System.Console.WriteLine("ssh $SSHUSER@$SSHHOST -p $SSHPORT rm -rf \"$RIGHTDIR" + v_name + "\"");
                                    }
                                    break;
                                    case Spartacus.Utils.SyncAction.DONOTHING:
                                    break;
                                    default:
                                    break;
                            }
                        }

                        v_totalsize += v_file.v_size;
                    }
                }
            }

            System.Console.WriteLine("echo \"TOTAL: {0} diretórios, {1} arquivos, {2} bytes.\"", p_directorylist.Count, p_filelist.Count, v_totalsize);
        }
    }
}

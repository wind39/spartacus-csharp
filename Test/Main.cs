using System;
using Spartacus;

namespace Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            FileTest();
        }

        //#region DATABASE
        //
        //private static void DatabaseTest()
        //{
            //v_database = new Spartacus.Database.Odbc("tpprodu", "planning", "password");
            //v_database = new Spartacus.Database.Firebird("localhost", "3050", "/work/clientes/escriba/sanmod.gdb", "SYSDBA", "masterkey");
            //v_database = new Spartacus.Database.Mysql("localhost", "3306", "wassina3_00555", "root", "password");
            //v_database = new Spartacus.Database.Oledb("Oracle", "127.0.0.1", "1521", "XE", "pscore", "password");
            //v_database = new Spartacus.Database.Sqlite("teste.db");
            //v_database = new Spartacus.Database.Postgresql("localhost", "5432", "test", "postgres", "password");
        //}
        //
        //#endregion

        #region FILE EXPLORER

        private static void FileTest()
        {
            Spartacus.Utils.FileExplorer v_explorer;
            string v_line;
            char[] v_separator;
            int k;

            v_explorer = new Spartacus.Utils.FileExplorer("/mnt/customers/ARAG");
            v_explorer.v_protectedminlevel = 1;
            v_explorer.v_protectpattern = "RESULTADOS FINAIS";
            v_explorer.v_showpatterntype = Spartacus.Utils.ShowPatternType.SHOWONLYPATTERN;

            try
            {
                System.Console.WriteLine(v_explorer.v_current.CompleteFileName());
                System.Console.WriteLine();

                v_explorer.List();

                foreach (Spartacus.Utils.File v_file in v_explorer.v_files)
                {
                    if (v_file.v_filetype == Spartacus.Utils.FileType.DIRECTORY)
                        System.Console.Write("D\t");
                    else
                        System.Console.Write("A\t");

                    System.Console.WriteLine("{0}\t{1}", v_file.v_id, v_file.v_name);
                }
                System.Console.WriteLine();
            }
            catch(Spartacus.Utils.Exception e)
            {
                System.Console.WriteLine(e.v_message);
            }

            v_separator = new char[1];
            v_separator[0] = ' ';

            v_line = System.Console.ReadLine();
            while (v_line != "exit")
            {
                switch(v_line.Substring(0, 1))
                {
                    case "e":
                        try
                        {
                            v_explorer.Enter(System.Convert.ToInt32(v_line.Split(v_separator)[1]));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "r":
                        try
                        {
                            if (v_line.Split(v_separator).Length > 1)
                                v_explorer.Return(System.Convert.ToInt32(v_line.Split(v_separator)[1]));
                            else
                                v_explorer.Return();
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "g":
                        try
                        {
                            System.Console.WriteLine("Arquivo recebido: " + v_explorer.Get(System.Convert.ToInt32(v_line.Split(v_separator)[1])));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "p":
                        try
                        {
                            System.Console.WriteLine("Arquivo recebido: " + v_explorer.Put(v_line.Split(v_separator)[1]));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "m":
                        try
                        {
                            System.Console.WriteLine("Diretório criado: " + v_explorer.Mkdir(v_line.Split(v_separator)[1]));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "d":
                        try
                        {
                            v_explorer.Delete(System.Convert.ToInt32(v_line.Split(v_separator)[1]));
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    case "z":
                        try
                        {
                            Spartacus.Utils.File v_zipfile = v_explorer.CompressDirectory("teste.zip", v_explorer.Get(System.Convert.ToInt32(v_line.Split(v_separator)[1])));
                            System.Console.WriteLine("Arquivo " + v_zipfile.v_name + " comprimido com sucesso!");
                        }
                        catch (Spartacus.Utils.Exception e)
                        {
                            System.Console.WriteLine(e.v_message);
                        }
                        break;
                    default:
                        System.Console.WriteLine("[" + v_line + "]: Comando não encontrado.");
                        break;
                }

                try
                {
                    //System.Console.WriteLine(v_explorer.v_current.CompleteFileName());
                    //System.Console.WriteLine();

                    System.Console.WriteLine();
                    System.Console.WriteLine("Histórico:");
                    for (k = 0; k < v_explorer.v_returnhistory.Count; k++)
                        System.Console.WriteLine("{0} - {1}", k, v_explorer.v_returnhistory[k].ToString());
                    System.Console.WriteLine();

                    v_explorer.List();

                    foreach (Spartacus.Utils.File v_file in v_explorer.v_files)
                    {
                        if (v_file.v_filetype == Spartacus.Utils.FileType.DIRECTORY)
                            System.Console.Write("D\t");
                        else
                            System.Console.Write("A\t");

                        System.Console.WriteLine("{0}\t{1}", v_file.v_id, v_file.v_name);
                    }
                    System.Console.WriteLine();
                }
                catch(Spartacus.Utils.Exception e)
                {
                    System.Console.WriteLine(e.v_message);
                }

                v_line = System.Console.ReadLine();
            }
        }

        #endregion

        #region REPORT

        private static void ReportTest()
        {
            // REPORT TEST
            Spartacus.Reporting.Report v_report;

            try
            {
                v_report = new Spartacus.Reporting.Report(1, "teste3.xml");
                v_report.v_cmd.SetValue("EMID", "181");
                v_report.v_cmd.SetValue("ANO", "2013");
                v_report.Execute();
                v_report.Save("output.pdf");

                System.Console.WriteLine("Pronto!");
            }
            catch (Spartacus.Reporting.Exception e)
            {
                System.Console.WriteLine(e.v_message);
            }
        }

        #endregion

        #region SYNCHRONIZER

        private static void SyncTest(string[] args)
        {
            Spartacus.Utils.Synchronizer v_sync;

            v_sync = new Spartacus.Utils.Synchronizer();
            v_sync.Execute(
                Spartacus.Utils.TreeType.FROMFILE,
                args[0],
                args[1],
                Spartacus.Utils.SyncAction.CREATE,
                Spartacus.Utils.SyncAction.COPY,
                Spartacus.Utils.SyncAction.DELETE,
                Spartacus.Utils.SyncAction.DELETE,
                Spartacus.Utils.SyncAction.COPY,
                Spartacus.Utils.SyncAction.DELETE);
        }

        #endregion

        #region EXCEL

        private static void ExcelTest()
        {
            Spartacus.Utils.Excel v_excel;

            v_excel = new Spartacus.Utils.Excel();
            v_excel.Import("TYCO2014 Faturas Layout Excel.xlsx");

            /*
            foreach (System.Data.DataTable v_table in v_excel.v_set.Tables)
            {
                System.Console.WriteLine("Planilha [{0}]:", v_table.TableName);

                foreach (System.Data.DataColumn v_column in v_table.Columns)
                    System.Console.Write("[{0}]\t", v_column.ColumnName);
                System.Console.WriteLine("");

                foreach (System.Data.DataRow v_row in v_table.Rows)
                {
                    foreach (System.Data.DataColumn v_column in v_table.Columns)
                        System.Console.Write("[{0}]\t", v_row[v_column]);
                    System.Console.WriteLine("");
                }
            }
            */
        }

        #endregion

        #region CRYPTO

        private static void CryptoTest()
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_plaintext;
            string v_encryptedtext, v_decryptedtext;
            int k;
            System.IO.StreamWriter v_fw;
            System.IO.StreamReader v_fr;
            string v_senha;

            v_senha = "spartacus";

            v_cryptor = new Spartacus.Net.Cryptor(v_senha);

            //v_plaintext = "program=Aplicativo Teste;version=0.1.0;release=20140702;customer=Adsistem Sistemas Administrativos;cnpj=00000000000;contact=William Ivanski;licenses=5;purchase=20140801;expiration=20140930";

            //v_plaintext = "/home/william/Público/planning/Transfer/LEGISLAÇÃO/Preços de Transferência/Preços de Transferência - Legislação/comentarios a mp 478/";

            v_plaintext = "/home/william/tmp/arag/produtos.txt";

            System.Console.WriteLine("Texto puro: [{0}]\n", v_plaintext);

            v_fw = new System.IO.StreamWriter("teste.txt", false, System.Text.Encoding.UTF8);
            for (k = 0; k < 10; k++)
            {
                v_encryptedtext = v_cryptor.Encrypt(v_plaintext);
                v_decryptedtext = v_cryptor.Decrypt(v_encryptedtext);

                v_fw.WriteLine(v_encryptedtext);

                System.Console.WriteLine("Texto criptografado {0}: [{1}]\tDescriptografado: [{2}]", k, v_encryptedtext, v_decryptedtext);
            }
            v_fw.Flush();
            v_fw.Close();

            System.Console.WriteLine("");

            v_cryptor = new Spartacus.Net.Cryptor(v_senha);

            v_fr = new System.IO.StreamReader("teste.txt", System.Text.Encoding.UTF8);
            k = 0;
            while (! v_fr.EndOfStream)
            {
                v_encryptedtext = v_fr.ReadLine();

                try
                {
                    v_decryptedtext = v_cryptor.Decrypt(v_encryptedtext);

                    System.Console.WriteLine("Texto criptografado {0}: [{1}]\tDescriptografado: [{2}]", k, v_encryptedtext, v_decryptedtext);
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Senha errada!");
                }

                k++;
            }
            v_fr.Close();
        }

        private static void CryptoFileTest(string p_mode, string p_input, string p_output)
        {
            Spartacus.Net.Cryptor v_cryptor;

            v_cryptor = new Spartacus.Net.Cryptor("senha");

            if (p_mode == "E")
                v_cryptor.EncryptFile(p_input, p_output);
            else
                v_cryptor.DecryptFile(p_input, p_output);
        }

        #endregion

        #region FILEARRAY

        private static void FileArrayTest()
        {
            System.Collections.ArrayList v_list;
            Spartacus.Utils.FileArray v_filearray;

            v_list = new System.Collections.ArrayList();
            v_list.Add("/home/william/tmp/arag");
            v_list.Add("/home/william/tmp/arag2");
            v_list.Add("/home/william/tmp/arag3");

            v_filearray = new Spartacus.Utils.FileArray(v_list, "*.txt|*.TXT");

            foreach (Spartacus.Utils.File v_file in v_filearray.v_files)
                System.Console.WriteLine(v_file.CompleteFileName() + "  " + v_file.CompleteFileName(true));
        }

        #endregion

        #region FORMS

        private static void FormsTest()
        {
            Spartacus.Forms.Window v_window;
            Spartacus.Forms.Menu v_menu;
            System.Windows.Forms.ToolStripMenuItem v_menugroup;
            Spartacus.Forms.Textbox v_textbox, v_textbox2;
            Spartacus.Forms.Lookup v_lookup;
            Spartacus.Forms.Grid v_grid;
            Spartacus.Forms.Buttons v_buttons;
            Spartacus.Database.Generic v_database;
            System.Data.DataTable v_table;

            try
            {
                v_database = new Spartacus.Database.Sqlite("sc_register.db");

                v_table = v_database.Query("select pais_st_codigo, pais_st_nome from paises", "PAISES");

                v_window = new Spartacus.Forms.Window("Formulario Teste", 500, 400);

                v_menu = new Spartacus.Forms.Menu(v_window);
                v_menugroup = v_menu.AddGroup("Cadastro");
                v_menu.AddItem(v_menugroup, "Clientes", new System.EventHandler(OnClickMenu));
                v_menu.AddItem(v_menugroup, "Usuários", new System.EventHandler(OnClickMenu));
                v_menu.AddItem(v_menugroup, "Licenças", new System.EventHandler(OnClickMenu));
                v_menugroup = v_menu.AddGroup("Ajuda");
                v_menu.AddItem(v_menugroup, "Sobre", new System.EventHandler(OnClickMenu));
                v_window.Add(v_menu);

                v_textbox = new Spartacus.Forms.Textbox(v_window, "Digite seu nome:", 40);
                v_window.Add(v_textbox);

                v_textbox2 = new Spartacus.Forms.Textbox(v_window, "Digite sua idade:", 40);
                v_window.Add(v_textbox2);

                v_lookup = new Spartacus.Forms.Lookup(v_window, "Olha só!!");
                v_lookup.Populate(v_table, "pais_st_codigo", "pais_st_nome", "70;180");
                v_window.Add(v_lookup);

                v_grid = new Spartacus.Forms.Grid(v_window, v_window.v_width, 100);
                v_grid.Populate(v_table);
                v_window.Add(v_grid);

                v_buttons = new Spartacus.Forms.Buttons(v_window);
                v_buttons.AddButton("Clique aqui", new System.EventHandler(OnClick));
                v_buttons.AddButton("Clique aqui 2", new System.EventHandler(OnClick2));
                v_window.Add(v_buttons);

                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.Run((System.Windows.Forms.Form)v_window.v_control);
            }
            catch(Spartacus.Database.Exception e)
            {
                System.Console.WriteLine(e.v_message);
            }
        }

        public static void OnClick(object sender, System.EventArgs e)
        {
            System.Console.WriteLine("Deu certo");
        }

        public static void OnClick2(object sender, System.EventArgs e)
        {
            System.Console.WriteLine("Deu certo 2");
        }

        public static void OnClickMenu(object sender, System.EventArgs e)
        {
            System.Console.WriteLine("Deu certo menu");
        }

        #endregion
    }
}

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

namespace Spartacus.Tools.Cryptor
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            (new MainClass()).Initialize(args);
        }

        public void Initialize(string[] args)
        {
            this.ShowHeader();

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-es":
                        switch (args.Length)
                        {
                            case 1:
                                this.EncryptString();
                                break;
                            case 2:
                                this.EncryptString(args[1]);
                                break;
                            default:
                                this.ShowHelp();
                                break;
                        }
                        break;
                    case "-ds":
                        switch (args.Length)
                        {
                            case 1:
                                this.DecryptString();
                                break;
                            case 2:
                                this.DecryptString(args[1]);
                                break;
                            default:
                                this.ShowHelp();
                                break;
                        }
                        break;
                    case "-ef":
                        switch (args.Length)
                        {
                            case 2:
                                this.EncryptFile(args[1]);
                                break;
                            case 3:
                                this.EncryptFile(args[1], args[2]);
                                break;
                            default:
                                this.ShowHelp();
                                break;
                        }
                        break;
                    case "-df":
                        switch (args.Length)
                        {
                            case 2:
                                this.DecryptFile(args[1]);
                                break;
                            case 3:
                                this.DecryptFile(args[1], args[2]);
                                break;
                            default:
                                this.ShowHelp();
                                break;
                        }
                        break;
                    default:
                        this.ShowHelp();
                        break;
                }
            }
            else
                this.ShowHelp();
        }

        private void EncryptString()
        {
            Spartacus.Utils.Cryptor v_cryptor;
            string v_input;

            try
            {
                Console.Write("Type the password to encrypt the string: ");
                v_cryptor = new Spartacus.Utils.Cryptor(Console.ReadLine());

                Console.Write("        Type the string to be encrypted: ");
                v_input = Console.ReadLine();

                Console.WriteLine("                The encrypted string is: {0}", v_cryptor.Encrypt(v_input));
            }
            catch (Spartacus.Utils.Exception e)
            {
                Console.WriteLine(e.v_message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void EncryptString(string p_input)
        {
            Spartacus.Utils.Cryptor v_cryptor;

            try
            {
                Console.Write("Type the password to encrypt the string: ");
                v_cryptor = new Spartacus.Utils.Cryptor(Console.ReadLine());

                Console.WriteLine("                The encrypted string is: {0}", v_cryptor.Encrypt(p_input));
            }
            catch (Spartacus.Utils.Exception e)
            {
                Console.WriteLine(e.v_message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void DecryptString()
        {
            Spartacus.Utils.Cryptor v_cryptor;
            string v_input;

            try
            {
                Console.Write("Type the password to decrypt the string: ");
                v_cryptor = new Spartacus.Utils.Cryptor(Console.ReadLine());

                Console.Write("        Type the string to be decrypted: ");
                v_input = Console.ReadLine();

                Console.WriteLine("                The decrypted string is: {0}", v_cryptor.Decrypt(v_input));
            }
            catch (Spartacus.Utils.Exception e)
            {
                Console.WriteLine(e.v_message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void DecryptString(string p_input)
        {
            Spartacus.Utils.Cryptor v_cryptor;

            try
            {
                Console.Write("Type the password to decrypt the string: ");
                v_cryptor = new Spartacus.Utils.Cryptor(Console.ReadLine());

                Console.WriteLine("                The decrypted string is: {0}", v_cryptor.Decrypt(p_input));
            }
            catch (Spartacus.Utils.Exception e)
            {
                Console.WriteLine(e.v_message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void EncryptFile(string p_input)
        {
            Spartacus.Utils.Cryptor v_cryptor;

            try
            {
                Console.Write("Type the password to encrypt the file: ");
                v_cryptor = new Spartacus.Utils.Cryptor(Console.ReadLine());

                v_cryptor.EncryptFile(p_input, p_input + ".crypt");

                Console.WriteLine();
                Console.WriteLine("File {0} successfully encrypted to the file {1}.", p_input, p_input + ".crypt");
            }
            catch (Spartacus.Utils.Exception e)
            {
                Console.WriteLine(e.v_message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void EncryptFile(string p_input, string p_output)
        {
            Spartacus.Utils.Cryptor v_cryptor;

            try
            {
                Console.Write("Type the password to encrypt the file: ");
                v_cryptor = new Spartacus.Utils.Cryptor(Console.ReadLine());

                v_cryptor.EncryptFile(p_input, p_output);

                Console.WriteLine();
                Console.WriteLine("File {0} successfully encrypted to the file {1}.", p_input, p_output);
            }
            catch (Spartacus.Utils.Exception e)
            {
                Console.WriteLine(e.v_message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void DecryptFile(string p_input)
        {
            Spartacus.Utils.Cryptor v_cryptor;
            string v_output;

            try
            {
                Console.Write("Type the password to decrypt the file: ");
                v_cryptor = new Spartacus.Utils.Cryptor(Console.ReadLine());

                if (p_input.EndsWith(".crypt"))
                    v_output = p_input.Replace(".crypt", "");
                else
                    v_output = p_input + ".decrypt";
                
                v_cryptor.DecryptFile(p_input, v_output);

                Console.WriteLine();
                Console.WriteLine("File {0} successfully decrypted to the file {1}.", p_input, v_output);
            }
            catch (Spartacus.Utils.Exception e)
            {
                Console.WriteLine(e.v_message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void DecryptFile(string p_input, string p_output)
        {
            Spartacus.Utils.Cryptor v_cryptor;

            try
            {
                Console.Write("Type the password to decrypt the file: ");
                v_cryptor = new Spartacus.Utils.Cryptor(Console.ReadLine());

                v_cryptor.DecryptFile(p_input, p_output);

                Console.WriteLine();
                Console.WriteLine("File {0} successfully decrypted to the file {1}.", p_input, p_output);
            }
            catch (Spartacus.Utils.Exception e)
            {
                Console.WriteLine(e.v_message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ShowHeader()
        {
            Console.WriteLine(this.Version());
            Console.WriteLine("Copyright 2015 William Ivanski - william.ivanski@gmail.com");
            Console.WriteLine();
        }

        private string Version()
        {
            System.Reflection.Assembly v_assembly;

            v_assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return "Spartacus.Tools.Cryptor " + v_assembly.GetName().Version.ToString();
        }

        private void ShowHelp()
        {
            Console.WriteLine("Command line options:");
            Console.WriteLine("-h                             : Show this help message");
            Console.WriteLine("-es                            : Encrypt string (input string is asked to the user)");
            Console.WriteLine("-es <input string>             : Encrypt string (input string is passed as argument)");
            Console.WriteLine("-ds                            : Decrypt string (input string is asked to the user)");
            Console.WriteLine("-ds <input string>             : Decrypt string (input string is passed as argument)");
            Console.WriteLine("-ef <input file>               : Encrypt file and save as <input file>.crypt");
            Console.WriteLine("-ef <input file> <output file> : Encrypt file and save as <output file>");
            Console.WriteLine("-df <input file>               : Decrypt file and save as <input file> without '.crypt' if it is terminated by '.crypt', or as <input file>.decrypt otherwise");
            Console.WriteLine("-df <input file> <output file> : Decrypt file and save as <output file>");
        }
    }
}

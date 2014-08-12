using System;

namespace Spartacus.Net
{
    public class Cryptor
    {
        private string v_password;

        private string v_initvector;

        private int v_keysize;

        private int v_minsaltlength;

        private int v_maxsaltlength;

        private System.Security.Cryptography.ICryptoTransform v_encryptor;

        private System.Security.Cryptography.ICryptoTransform v_decryptor;


        public Cryptor(string p_password)
        {
            this.v_password = p_password;
            this.v_keysize = 256;
            this.v_initvector = "0123456789ABCDEF";
            this.v_minsaltlength = 8;
            this.v_maxsaltlength = 8;
        }

        public Cryptor(string p_password, string p_initvector)
        {
            this.v_password = p_password;
            this.v_keysize = 256;
            if (p_initvector.Length == 16)
                this.v_initvector = p_initvector;
            else
                this.v_initvector = "0123456789ABCDEF";
            this.v_minsaltlength = 8;
            this.v_maxsaltlength = 8;
        }

        public Cryptor(string p_password, int p_minsaltlength, int p_maxsaltlength)
        {
            this.v_password = p_password;
            this.v_keysize = 256;
            this.v_initvector = "0123456789ABCDEF";
            if (p_minsaltlength >= 8 && p_minsaltlength < 255)
                this.v_minsaltlength = p_minsaltlength;
            else
                this.v_minsaltlength = 8;
            if (p_maxsaltlength > 8 && p_maxsaltlength <= 255)
                this.v_maxsaltlength = p_maxsaltlength;
            else
                this.v_maxsaltlength = 8;
        }

        public Cryptor(string p_password, string p_initvector, int p_minsaltlength, int p_maxsaltlength)
        {
            this.v_password = p_password;
            this.v_keysize = 256;
            if (p_initvector.Length == 16)
                this.v_initvector = p_initvector;
            else
                this.v_initvector = "0123456789ABCDEF";
            if (p_minsaltlength >= 8 && p_minsaltlength < 255)
                this.v_minsaltlength = p_minsaltlength;
            else
                this.v_minsaltlength = 8;
            if (p_maxsaltlength > 8 && p_maxsaltlength <= 255)
                this.v_maxsaltlength = p_maxsaltlength;
            else
                this.v_maxsaltlength = 8;
        }

        public Cryptor(string p_password, int p_keysize)
        {
            this.v_password = p_password;
            if (p_keysize == 128 || p_keysize == 192 || p_keysize == 256)
                this.v_keysize = p_keysize;
            else
                this.v_keysize = 256;
            this.v_initvector = "0123456789ABCDEF";
            this.v_minsaltlength = 8;
            this.v_maxsaltlength = 8;
        }

        public Cryptor(string p_password, int p_keysize, string p_initvector)
        {
            this.v_password = p_password;
            if (p_keysize == 128 || p_keysize == 192 || p_keysize == 256)
                this.v_keysize = p_keysize;
            else
                this.v_keysize = 256;
            if (p_initvector.Length == 16)
                this.v_initvector = p_initvector;
            else
                this.v_initvector = "0123456789ABCDEF";
            this.v_minsaltlength = 8;
            this.v_maxsaltlength = 8;
        }

        public Cryptor(string p_password, int p_keysize, int p_minsaltlength, int p_maxsaltlength)
        {
            this.v_password = p_password;
            if (p_keysize == 128 || p_keysize == 192 || p_keysize == 256)
                this.v_keysize = p_keysize;
            else
                this.v_keysize = 256;
            this.v_initvector = "0123456789ABCDEF";
            if (p_minsaltlength >= 8 && p_minsaltlength < 255)
                this.v_minsaltlength = p_minsaltlength;
            else
                this.v_minsaltlength = 8;
            if (p_maxsaltlength > 8 && p_maxsaltlength <= 255)
                this.v_maxsaltlength = p_maxsaltlength;
            else
                this.v_maxsaltlength = 8;
        }

        public Cryptor(string p_password, int p_keysize, string p_initvector, int p_minsaltlength, int p_maxsaltlength)
        {
            this.v_password = p_password;
            if (p_keysize == 128 || p_keysize == 192 || p_keysize == 256)
                this.v_keysize = p_keysize;
            else
                this.v_keysize = 256;
            if (p_initvector.Length == 16)
                this.v_initvector = p_initvector;
            else
                this.v_initvector = "0123456789ABCDEF";
            if (p_minsaltlength >= 8 && p_minsaltlength < 255)
                this.v_minsaltlength = p_minsaltlength;
            else
                this.v_minsaltlength = 8;
            if (p_maxsaltlength > 8 && p_maxsaltlength <= 255)
                this.v_maxsaltlength = p_maxsaltlength;
            else
                this.v_maxsaltlength = 8;
        }
        
        private void Initialize()
        {
            System.Security.Cryptography.RijndaelManaged v_rijndael;
            System.Security.Cryptography.Rfc2898DeriveBytes v_passwordbytes;
            byte[] v_initvectorbytes;
            //byte[] v_salt;
            byte[] v_keybytes;

            v_rijndael = new System.Security.Cryptography.RijndaelManaged();
            v_rijndael.Mode = System.Security.Cryptography.CipherMode.CBC;

            v_initvectorbytes = System.Text.Encoding.ASCII.GetBytes(this.v_initvector);

            v_passwordbytes = new System.Security.Cryptography.Rfc2898DeriveBytes(this.v_password, v_initvectorbytes, 1);
            v_keybytes = v_passwordbytes.GetBytes(this.v_keysize / 8);

            this.v_encryptor = v_rijndael.CreateEncryptor(v_keybytes, v_initvectorbytes);
            this.v_decryptor = v_rijndael.CreateDecryptor(v_keybytes, v_initvectorbytes);
        }

        public string Encrypt(string p_plaintext)
        {
            return this.Encrypt(System.Text.Encoding.UTF8.GetBytes(p_plaintext));
        }

        public string Encrypt(byte[] p_plaintextbytes)
        {
            return System.Convert.ToBase64String(this.EncryptToBytes(p_plaintextbytes));
        }

        public byte[] EncryptToBytes(string p_plaintext)
        {
            return this.EncryptToBytes(System.Text.Encoding.UTF8.GetBytes(p_plaintext));
        }

        public byte[] EncryptToBytes(byte[] p_plaintextbytes)
        {
            byte[] v_ciphertextbytes;
            byte[] v_plaintextbyteswithsalt;
            System.IO.MemoryStream v_memory;
            System.Security.Cryptography.CryptoStream v_crypto;

            this.Initialize();

            v_plaintextbyteswithsalt = this.AddSalt(p_plaintextbytes);

            v_memory = new System.IO.MemoryStream();

            lock (this)
            {
                v_crypto = new System.Security.Cryptography.CryptoStream(v_memory, this.v_encryptor, System.Security.Cryptography.CryptoStreamMode.Write);
                v_crypto.Write(v_plaintextbyteswithsalt, 0, v_plaintextbyteswithsalt.Length);
                v_crypto.FlushFinalBlock();

                v_ciphertextbytes = v_memory.ToArray();

                v_memory.Close();
                v_crypto.Close();

                return v_ciphertextbytes;
            }
        }

        public void EncryptFile(string p_inputfilename, string p_outputfilename)
        {
            System.IO.FileStream v_inputfile, v_outputfile;
            System.IO.BinaryReader v_reader;
            System.IO.StreamWriter v_writer;
            int v_chunksize, v_bytestoread;
            byte[] v_chunk;
            string v_cryptedchunk;
            string v_context;

            // tamanho em bytes da porcao do arquivo que corresponderah a uma linha no arquivo criptografado
            v_chunksize = 1048576;

            try
            {
                v_inputfile = new System.IO.FileStream(p_inputfilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                v_outputfile = new System.IO.FileStream(p_outputfilename, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                v_reader = new System.IO.BinaryReader(v_inputfile);
                v_writer = new System.IO.StreamWriter(v_outputfile);

                v_bytestoread = (int) v_inputfile.Length;

                while (v_bytestoread > 0)
                {
                    v_chunk = v_reader.ReadBytes(v_chunksize);

                    // criptografando
                    v_cryptedchunk = this.Encrypt(v_chunk);

                    // escrevendo linha criptografada
                    v_writer.WriteLine(v_cryptedchunk);

                    v_bytestoread -= v_chunksize;
                }

                v_reader.Close();
                v_writer.Flush();
                v_writer.Close();
            }
            catch (System.IO.IOException exc_io)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc_io);
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        public void EncryptFile(string p_inputfilename, string p_outputfilename, int p_chunksize)
        {
            System.IO.FileStream v_inputfile, v_outputfile;
            System.IO.BinaryReader v_reader;
            System.IO.StreamWriter v_writer;
            int v_chunksize, v_bytestoread;
            byte[] v_chunk;
            string v_cryptedchunk;
            string v_context;

            // tamanho em bytes da porcao do arquivo que corresponderah a uma linha no arquivo criptografado
            v_chunksize = p_chunksize;

            try
            {
                v_inputfile = new System.IO.FileStream(p_inputfilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                v_outputfile = new System.IO.FileStream(p_outputfilename, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                v_reader = new System.IO.BinaryReader(v_inputfile);
                v_writer = new System.IO.StreamWriter(v_outputfile);

                v_bytestoread = (int) v_inputfile.Length;

                while (v_bytestoread > 0)
                {
                    v_chunk = v_reader.ReadBytes(v_chunksize);

                    // criptografando
                    v_cryptedchunk = this.Encrypt(v_chunk);

                    // escrevendo linha criptografada
                    v_writer.WriteLine(v_cryptedchunk);

                    v_bytestoread -= v_chunksize;
                }

                v_reader.Close();
                v_writer.Flush();
                v_writer.Close();
            }
            catch (System.IO.IOException exc_io)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc_io);
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        public string Decrypt(string p_ciphertext)
        {
            return this.Decrypt(System.Convert.FromBase64String(p_ciphertext));
        }

        public string Decrypt(byte[] p_ciphertextbytes)
        {
            return System.Text.Encoding.UTF8.GetString(this.DecryptToBytes(p_ciphertextbytes));
        }

        public byte[] DecryptToBytes(string p_ciphertext)
        {
            return this.DecryptToBytes(System.Convert.FromBase64String(p_ciphertext));
        }

        public byte[] DecryptToBytes(byte[] p_ciphertextbytes)
        {
            byte[] v_plaintextbytes;
            byte[] v_decryptedbytes;
            int v_decryptedbytecount;
            int v_saltlength;
            System.IO.MemoryStream v_memory;
            System.Security.Cryptography.CryptoStream v_crypto;

            this.Initialize();

            v_memory = new System.IO.MemoryStream(p_ciphertextbytes);

            v_decryptedbytes = new byte[p_ciphertextbytes.Length];

            lock (this)
            {
                v_crypto = new System.Security.Cryptography.CryptoStream(v_memory, this.v_decryptor, System.Security.Cryptography.CryptoStreamMode.Read);
                v_decryptedbytecount = v_crypto.Read(v_decryptedbytes, 0, v_decryptedbytes.Length);

                v_memory.Close();
                v_crypto.Close();
            }

            v_saltlength = (v_decryptedbytes[0] & 0x03) |
                           (v_decryptedbytes[1] & 0x0c) |
                           (v_decryptedbytes[2] & 0x30) |
                           (v_decryptedbytes[3] & 0xc0);

            v_plaintextbytes = new byte[v_decryptedbytecount - v_saltlength];

            System.Array.Copy(v_decryptedbytes, v_saltlength, v_plaintextbytes, 0, v_decryptedbytecount - v_saltlength);

            return v_plaintextbytes;
        }

        public void DecryptFile(string p_inputfilename, string p_outputfilename)
        {
            System.IO.FileStream v_inputfile, v_outputfile;
            System.IO.StreamReader v_reader;
            System.IO.BinaryWriter v_writer;
            int v_bytestoread;
            string v_chunk;
            byte[] v_decryptedchunk;
            string v_context;

            try
            {
                v_inputfile = new System.IO.FileStream(p_inputfilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                v_outputfile = new System.IO.FileStream(p_outputfilename, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                v_reader = new System.IO.StreamReader(v_inputfile);
                v_writer = new System.IO.BinaryWriter(v_outputfile);

                v_bytestoread = (int) v_inputfile.Length;

                while (! v_reader.EndOfStream)
                {
                    v_chunk = v_reader.ReadLine();

                    // descriptografando
                    v_decryptedchunk = this.DecryptToBytes(v_chunk);

                    // escrevendo porcao descriptografada
                    v_writer.Write(v_decryptedchunk);
                }

                v_reader.Close();
                v_writer.Flush();
                v_writer.Close();
            }
            catch (System.IO.IOException exc_io)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc_io);
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        private byte[] AddSalt(byte[] p_plaintextbytes)
        {
            byte[] v_plaintextbyteswithsalt;
            byte[] v_salt;

            v_salt = this.GenerateSalt();

            v_plaintextbyteswithsalt = new byte[p_plaintextbytes.Length + v_salt.Length];

            System.Array.Copy(v_salt, v_plaintextbyteswithsalt, v_salt.Length);

            System.Array.Copy(p_plaintextbytes, 0, v_plaintextbyteswithsalt, v_salt.Length, p_plaintextbytes.Length);

            return v_plaintextbyteswithsalt;
        }

        private byte[] GenerateSalt()
        {
            System.Security.Cryptography.RNGCryptoServiceProvider v_randomnumbergenerator;
            byte[] v_salt;
            int v_saltlength;

            if (this.v_minsaltlength == this.v_maxsaltlength)
                v_saltlength = this.v_minsaltlength;
            else
                v_saltlength = this.GenerateRandomNumber(this.v_minsaltlength, this.v_maxsaltlength);

            v_salt = new byte[v_saltlength];

            v_randomnumbergenerator = new System.Security.Cryptography.RNGCryptoServiceProvider();
            v_randomnumbergenerator.GetNonZeroBytes(v_salt);

            v_salt[0] = (byte)((v_salt[0] & 0xfc) | (v_saltlength & 0x03));
            v_salt[1] = (byte)((v_salt[1] & 0xf3) | (v_saltlength & 0x0c));
            v_salt[2] = (byte)((v_salt[2] & 0xcf) | (v_saltlength & 0x30));
            v_salt[3] = (byte)((v_salt[3] & 0x3f) | (v_saltlength & 0xc0));

            return v_salt;
        }

        private int GenerateRandomNumber(int p_minvalue, int p_maxvalue)
        {
            System.Security.Cryptography.RNGCryptoServiceProvider v_randomnumbergenerator;
            System.Random v_random;
            byte[] v_randombytes;
            int v_seed;

            v_randombytes = new byte[4];

            v_randomnumbergenerator = new System.Security.Cryptography.RNGCryptoServiceProvider();
            v_randomnumbergenerator.GetBytes(v_randombytes);

            v_seed = ((v_randombytes[0] & 0x7f) << 24) |
                      (v_randombytes[1]         << 16) |
                      (v_randombytes[2]         << 8 ) |
                      (v_randombytes[3]);

            v_random = new System.Random(v_seed);

            return v_random.Next(p_minvalue, p_maxvalue);
        }
    }
}

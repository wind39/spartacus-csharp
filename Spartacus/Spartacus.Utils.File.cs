using System;

namespace Spartacus.Utils
{
    /// <summary>
    /// A classe <see cref="Spartacus.Utils.File"/> pode representar tanto um arquivo quanto um diretório.
    /// A única coisa que diferencia é o tipo do arquivo.
    /// </summary>
    public enum FileType
    {
        DIRECTORY,
        FILE
    }

    /// <summary>
    /// Separador de diretórios do nome do arquivo.
    /// Em sistemas Unix é SLASH (/), e em sistemas Windows é BACKSLASH (\).
    /// </summary>
    public enum PathSeparator
    {
        SLASH,
        BACKSLASH
    }

    /// <summary>
    /// Classe File.
    /// Representa um arquivo ou um diretório.
    /// Pode ser usado em listas de arquivos para processamento em massa, ou para construção de árvores de arquivos.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Identificador único do arquivo (se aplicável).
        /// </summary>
        public int v_id;

        /// <summary>
        /// Identificador do diretório pai do arquivo (se aplicável).
        /// </summary>
        public int v_parentid;

        /// <summary>
        /// Indica se é um arquivo ou um diretório.
        /// </summary>
        public Spartacus.Utils.FileType v_filetype;

        /// <summary>
        /// Separador de diretórios.
        /// </summary>
        public Spartacus.Utils.PathSeparator v_pathseparator;

        /// <summary>
        /// Caminho completo do diretório pai do arquivo ou diretório atual.
        /// </summary>
        public string v_path;

        /// <summary>
        /// Nome base do arquivo ou diretório.
        /// </summary>
        public string v_name;

        /// <summary>
        /// Extensão do arquivo.
        /// </summary>
        public string v_extension;

        /// <summary>
        /// Data da última modificação do arquivo ou diretório.
        /// </summary>
        public System.DateTime v_lastwritedate;

        /// <summary>
        /// Tamanho do arquivo.
        /// </summary>
        public long v_size;

        /// <summary>
        /// Codificação de caracteres do arquivo.
        /// </summary>
        public System.Text.Encoding v_encoding;

        /// <summary>
        /// Número da página de grid em que o arquivo se encontra.
        /// </summary>
        public int v_pagenumber;

        /// <summary>
        /// Informa se o arquivo ou diretório deve protegido contra escrita (fictício e não permissões reais do arquivo).
        /// </summary>
        public bool v_protected;

        /// <summary>
        /// Informa se o arquivo é oculto ou não.
        /// </summary>
        public bool v_hidden;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_size = -1;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, Spartacus.Utils.PathSeparator p_separator)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_size = -1;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, System.DateTime p_lastwritedate)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = -1;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, Spartacus.Utils.PathSeparator p_separator, System.DateTime p_lastwritedate)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = -1;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_size'>
        /// Tamanho do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, System.DateTime p_lastwritedate, long p_size)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = p_size;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_size'>
        /// Tamanho do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, Spartacus.Utils.PathSeparator p_separator, System.DateTime p_lastwritedate, long p_size)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = p_size;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, System.Text.Encoding p_encoding)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_size = -1;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, Spartacus.Utils.PathSeparator p_separator, System.Text.Encoding p_encoding)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_size = -1;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, System.DateTime p_lastwritedate, System.Text.Encoding p_encoding)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = -1;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, Spartacus.Utils.PathSeparator p_separator, System.DateTime p_lastwritedate, System.Text.Encoding p_encoding)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = -1;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_size'>
        /// Tamanho do arquivo.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, System.DateTime p_lastwritedate, long p_size, System.Text.Encoding p_encoding)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = p_size;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_size'>
        /// Tamanho do arquivo.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, Spartacus.Utils.PathSeparator p_separator, System.DateTime p_lastwritedate, long p_size, System.Text.Encoding p_encoding)
        {
            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(p_completename);
            this.v_extension = this.GetExtension(p_completename);
            this.v_path = this.GetPath(p_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = p_size;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_size = -1;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, Spartacus.Utils.PathSeparator p_separator)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_size = -1;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, System.DateTime p_lastwritedate)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = -1;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, Spartacus.Utils.PathSeparator p_separator, System.DateTime p_lastwritedate)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = -1;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_size'>
        /// Tamanho do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, System.DateTime p_lastwritedate, long p_size)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = p_size;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_size'>
        /// Tamanho do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, Spartacus.Utils.PathSeparator p_separator, System.DateTime p_lastwritedate, long p_size)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = p_size;
            this.v_encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, System.Text.Encoding p_encoding)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_size = -1;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, Spartacus.Utils.PathSeparator p_separator, System.Text.Encoding p_encoding)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_size = -1;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, System.DateTime p_lastwritedate, System.Text.Encoding p_encoding)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = -1;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, Spartacus.Utils.PathSeparator p_separator, System.DateTime p_lastwritedate, System.Text.Encoding p_encoding)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = -1;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_size'>
        /// Tamanho do arquivo.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, System.DateTime p_lastwritedate, long p_size, System.Text.Encoding p_encoding)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = Spartacus.Utils.PathSeparator.SLASH;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = p_size;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.File"/>.
        /// </summary>
        /// <param name='p_id'>
        /// Identificador único do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_parentid'>
        /// Identificador do diretório pai do arquivo ou diretório (se aplicável).
        /// </param>
        /// <param name='p_type'>
        /// Indica se é um arquivo ou um diretório.
        /// </param>
        /// <param name='p_completename'>
        /// Nome completo, absoluto ou relativo, do arquivo ou diretório atual.
        /// </param>
        /// <param name='p_encryptedname'>
        /// Se o nome do arquivo está criptografado ou não.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de diretórios do caminho completo do arquivo.
        /// </param>
        /// <param name='p_lastwritedate'>
        /// Data da última modificação do arquivo ou diretório.
        /// </param>
        /// <param name='p_size'>
        /// Tamanho do arquivo.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação do arquivo.
        /// </param>
        public File(int p_id, int p_parentid, Spartacus.Utils.FileType p_type, string p_completename, bool p_encryptedname, Spartacus.Utils.PathSeparator p_separator, System.DateTime p_lastwritedate, long p_size, System.Text.Encoding p_encoding)
        {
            Spartacus.Net.Cryptor v_cryptor;
            string v_completename;

            if (p_encryptedname)
            {
                try
                {
                    v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                    v_completename = v_cryptor.Decrypt(p_completename);
                }
                catch (System.Exception)
                {
                    v_completename = p_completename;
                }
            }
            else
                v_completename = p_completename;

            this.v_id = p_id;
            this.v_parentid = p_parentid;
            this.v_filetype = p_type;
            this.v_pathseparator = p_separator;
            this.v_name = this.GetBaseName(v_completename);
            this.v_extension = this.GetExtension(v_completename);
            this.v_path = this.GetPath(v_completename);
            this.v_lastwritedate = p_lastwritedate;
            this.v_size = p_size;
            this.v_encoding = p_encoding;
            this.v_protected = false;
            this.v_hidden = this.GetHidden();
        }

        /// <summary>
        /// Pega o nome base do arquivo ou diretório.
        /// </summary>
        /// <returns>
        /// Nome base do arquivo ou diretório.
        /// </returns>
        /// <param name='p_completename'>
        /// Nome completo do arquivo ou diretório.
        /// </param>
        private string GetBaseName(string p_completename)
        {
            string v_ret = null;
            string[] v_partes;
            char[] v_sep;

            v_sep = new char[1];

            switch (this.v_pathseparator)
            {
                case Spartacus.Utils.PathSeparator.SLASH:
                    v_sep[0] = '/';
                    if (p_completename.Contains("/"))
                    {
                        v_partes = p_completename.Split(v_sep);
                        v_ret = v_partes[v_partes.Length - 1];
                    }
                    else
                        v_ret = p_completename;
                    break;

                case Spartacus.Utils.PathSeparator.BACKSLASH:
                    v_sep[0] = '\\';
                    if (p_completename.Contains("\\"))
                    {
                        v_partes = p_completename.Split(v_sep);
                        v_ret = v_partes[v_partes.Length - 1];
                    }
                    else
                        v_ret = p_completename;
                    break;
            }

            return v_ret;
        }

        /// <summary>
        /// Pega a extensão do arquivo.
        /// </summary>
        /// <returns>
        /// Extensão do arquivo ou NULL se for um diretório.
        /// </returns>
        /// <param name='p_completename'>
        /// Nome completo do arquivo ou diretório.
        /// </param>
        private string GetExtension(string p_completename)
        {
            char[] v_sep = { '.' };
            string[] v_partes;

            if (this.v_filetype == Spartacus.Utils.FileType.FILE)
            {
                if (p_completename.Contains("."))
                {
                    v_partes = p_completename.Split(v_sep);
                    return v_partes [v_partes.Length - 1].ToUpper();
                }
                else
                    return " ";
            }
            else
                return " ";
        }

        /// <summary>
        /// Pega o caminho completo do diretório pai do arquivo ou diretório atual.
        /// </summary>
        /// <returns>
        /// Caminho completo do diretório pai do arquivo ou diretório atual.
        /// </returns>
        /// <param name='p_completename'>
        /// Nome completo do arquivo ou diretório.
        /// </param>
        private string GetPath(string p_completename)
        {
            if (p_completename == ".")
                return p_completename;
            else
                return p_completename.Substring(0, p_completename.Length - this.v_name.Length - 1);
        }

        /// <summary>
        /// Faz o mesmo que a função GetBaseName, porém não considera a extensão do arquivo.
        /// Se for um diretório, retorna o mesmo valor que a função GetBaseName.
        /// </summary>
        /// <returns>Nome base do arquivo ou diretório, sem considerar extensão.</returns>
        public string GetBaseNameNoExt()
        {
            string v_ret;
            char[] v_sep = { '.' };
            string[] v_partes;
            int k;

            if (this.v_filetype == Spartacus.Utils.FileType.FILE)
            {
                if (this.v_name.Contains("."))
                {
                    v_partes = this.v_name.Split(v_sep);

                    if (v_partes.Length == 2)
                        v_ret = v_partes[0];
                    else
                    {
                        v_ret = "";
                        for (k = 0; k < v_partes.Length-1; k++)
                        {
                            if (k < v_partes.Length-2)
                                v_ret += v_partes[k] + ".";
                            else
                                v_ret += v_partes[k];
                        }
                    }

                    return v_ret;
                }
                else
                    return this.v_name;
            }
            else
                return this.v_name;
        }


        /// <summary>
        /// Retorna o nome completo do arquivo ou diretório atual.
        /// </summary>
        /// <returns>
        /// Nome completo do arquivo ou diretório atual.
        /// </returns>
        public string CompleteFileName()
        {
            switch (this.v_pathseparator)
            {
                case Spartacus.Utils.PathSeparator.SLASH:
                    return this.v_path + "/" + this.v_name;

                case Spartacus.Utils.PathSeparator.BACKSLASH:
                    return this.v_path + "\\" + this.v_name;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Retorna o nome completo criptografado do arquivo ou diretório atual.
        /// Isso é necessário para armazenar strings sem acento no banco de dados.
        /// </summary>
        /// <returns>
        /// Nome completo do arquivo ou diretório atual.
        /// </returns>
        /// <param name='p_encryptname'>
        /// Se deve criptografar ou não o nome do arquivo.
        /// </param>
        public string CompleteFileName(bool p_encryptname)
        {
            Spartacus.Net.Cryptor v_cryptor;

            if (p_encryptname)
            {
                v_cryptor = new Spartacus.Net.Cryptor("spartacus");
                return v_cryptor.Encrypt(this.CompleteFileName());
            }
            else
                return this.CompleteFileName();
        }

        /// <summary>
        /// Converte o tamanho em bytes para uma string representando o tamanho do arquivo legível por humanos.
        /// </summary>
        /// <returns>
        /// Tamanho do arquivo legível por humanos.
        /// </returns>
        public string GetSize()
        {
            if (this.v_size == -1)
                return " ";
            else
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = this.v_size;
                int order = 0;

                while (len >= 1024 && (order + 1) < sizes.Length)
                {
                    order++;
                    len = len / 1024;
                }

                return String.Format("{0:0.#} {1}", len, sizes [order]);
            }
        }

        /// <summary>
        /// Verifica se o arquivo é oculto baseando-se no seu nome.
        /// </summary>
        /// <returns><c>true</c>, se o arquivo for oculto, <c>false</c> caso contrário.</returns>
        private bool GetHidden()
        {
            if (this.v_name.StartsWith(".") || this.v_name.StartsWith("cifs") || this.v_name.ToLower() == "thumbs.db")
                return true;
            else
                return false;
        }
    }
}

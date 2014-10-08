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
    public class CSVTable
    {
        /// <summary>
        /// Tabela que armazena os dados do arquivo CSV.
        /// </summary>
        public System.Data.DataTable v_table;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Utils.CSVTable"/>.
        /// </summary>
        public CSVTable()
        {
            this.v_table = new System.Data.DataTable();
        }

        /// <summary>
        /// Limpa os dados da tabela.
        /// </summary>
        public void Clear()
        {
            this.v_table.Clear();
            this.v_table.Columns.Clear();
        }

        /// <summary>
        /// Importa um arquivo CSV para dentro de uma <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <param name='p_filename'>
        /// Nome do arquivo CSV a ser importado.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de campos.
        /// </param>
        /// <param name='p_header'>
        /// Se contem ou nao uma linha de cabecalho.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação a ser usada para escrever o arquivo.
        /// </param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando não conseguir ler o arquivo de origem, ou quando o número de colunas for diferente do especificado na tabela <see cref="v_table"/>.</exception>
        public void Import(string p_filename, char p_separator, bool p_header, System.Text.Encoding p_encoding)
        {
            Spartacus.Utils.File v_file;
            string[] v_csvdata;
            string[] v_line;
            System.Data.DataRow v_row;
            int i, j, k;

            // colocando nome na tabela
            v_file = new Spartacus.Utils.File(1, 1, Spartacus.Utils.FileType.FILE, p_filename);
            this.v_table.TableName = v_file.v_name.Replace("." + v_file.v_extension, "");

            if (this.v_table.Columns.Count == 0)
            {
                throw new Spartacus.Utils.Exception("Objeto CSV não contém nenhuma coluna.");
            }

            try
            {
                v_csvdata = System.IO.File.ReadAllLines(p_filename, p_encoding);
            }
            catch (System.IO.IOException e)
            {
                throw new Spartacus.Utils.Exception(e);
            }

            if (p_header)
                k = 1;
            else
                k = 0;

            for (i = k; i < v_csvdata.Length; i++)
            {
                v_row = this.v_table.NewRow();
                v_line = v_csvdata[i].Split(p_separator);

                if (v_line.Length != this.v_table.Columns.Count)
                {
                    throw new Spartacus.Utils.Exception("Objeto CSV contém um número de colunas diferente do arquivo CSV.");
                }

                for (j = 0; j < this.v_table.Columns.Count; j++)
                    v_row[j] = v_line[j];

                this.v_table.Rows.Add(v_row);
            }

            return;
        }

        /// <summary>
        /// Exporta uma tabela para um arquivo CSV.
        /// </summary>
        /// <param name='p_filename'>
        /// Arquivo CSV de destino.
        /// </param>
        /// <param name='p_separator'>
        /// Separador de campos a ser usado.
        /// </param>
        /// <param name='p_header'>
        /// Se deve escrever ou nao uma linha de cabecalhos.
        /// </param>
        /// <param name='p_encoding'>
        /// Codificação a ser usada para escrever o arquivo.
        /// </param>
        /// <exception cref="Spartacus.Utils.Exception">Exceção acontece quando a tabela <see cref="v_table"/> não possuir nenhuma coluna, ou quando não conseguir escrever no arquivo de saída.</exception>
        public void Export(string p_filename, char p_separator, bool p_header, System.Text.Encoding p_encoding)
        {
            string v_text = "";
            int i;

            if (this.v_table.Columns.Count == 0)
            {
                throw new Spartacus.Utils.Exception("Objeto CSV não contém nenhuma coluna.");
            }

            if (p_header)
            {
                v_text += this.v_table.Columns [0].ColumnName;
                for (i = 1; i < this.v_table.Columns.Count; i++)
                    v_text += p_separator + this.v_table.Columns [i].ColumnName;
                v_text += "\r\n";
            }

            foreach (System.Data.DataRow r in this.v_table.Rows)
            {
                v_text += r[0].ToString();
                for (i = 1; i < this.v_table.Columns.Count; i++)
                    v_text += p_separator + r[i].ToString();
                v_text += "\r\n";
            }

            try
            {
                System.IO.File.WriteAllText(p_filename, v_text, p_encoding);
            }
            catch (System.Exception e)
            {
                throw new Spartacus.Utils.Exception(e);
            }

            return;
        }
    }
}

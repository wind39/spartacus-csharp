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

namespace Spartacus.Database
{
    /// <summary>
    /// Classe Command.
    /// Representa um comando SQL que pode possuir parâmetros entre #.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Código SQL a ser executado no banco de dados.
        /// </summary>
        public string v_text;

        /// <summary>
        /// Lista de Parâmetros.
        /// Cada parâmetro da lista deve ter um nome diferente.
        /// </summary>
        public System.Collections.ArrayList v_parameters;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Database.Command"/>.
        /// </summary>
        public Command()
        {
            this.v_text = null;
            this.v_parameters = new System.Collections.ArrayList();
        }

        /// <summary>
        /// Atualiza o código SQL.
        /// Substitui os nomes de parâmetro com tag de início e fim #, com o valor de cada parâmetro já formatado.
        /// Essa função deve ser chamada antes de executar o Comando no banco de dados.
        /// </summary>
        public void UpdateText()
        {
            int k;

            for (k = 0; k < this.v_parameters.Count; k++)
                this.v_text = this.v_text.Replace("#" + ((Spartacus.Database.Parameter)this.v_parameters[k]).v_name + "#", ((Spartacus.Database.Parameter)this.v_parameters[k]).Text());
        }

        /// <summary>
        /// Atualiza o código SQL, mas sem alterar o SQL original.
        /// Substitui os nomes de parâmetro com tag de início e fim #, com o valor de cada parâmetro já formatado.
        /// </summary>
        /// <returns>Código SQL tratado, pronto para execução no banco de dados.</returns>
        public string GetUpdatedText()
        {
            string v_localtext;
            int k;

            v_localtext = this.v_text;
            for (k = 0; k < this.v_parameters.Count; k++)
                v_localtext = v_localtext.Replace("#" + ((Spartacus.Database.Parameter)this.v_parameters[k]).v_name + "#", ((Spartacus.Database.Parameter)this.v_parameters[k]).Text());

            return v_localtext;
        }

        /// <summary>
        /// Apaga o texto e a lista de parâmetros da classe <see cref="Spartacus.Database.Command"/>.
        /// Dessa forma, a instância pode ser reaproveitada com um código SQL diferente.
        /// </summary>
        public void Clear()
        {
            this.v_text = null;
            this.v_parameters.Clear();
        }

        /// <summary>
        /// Adiciona um Parâmetro à lista de Parâmetros.
        /// </summary>
        /// <param name='p_name'>
        /// Nome do Parâmetro dentro do Comando SQL.
        /// </param>
        /// <param name='p_type'>
        /// Tipo de dados do Parâmetro.
        /// </param>
        public void AddParameter(string p_name, Spartacus.Database.Type p_type)
        {
            this.v_parameters.Add(new Spartacus.Database.Parameter(p_name, p_type));
        }

		/// <summary>
		/// Remove de uma string todos os caracteres com acentuação ou proibidos para a inserção SQL.
		/// </summary>
		/// <returns>
		/// String livre de caracteres com acentuação ou proibidos para a inserção SQL.
		/// </returns>
		/// <param name='p_string'>
		/// String a ser tratada.
		/// </param>
		public static string RemoveUnwantedChars(string p_string)
		{
			string v_newstring;
			int i, j, k;
			char[][] v_handler = new char[58][];
			char[] v_allowed = new char[85];
			bool v_achou;
			char[] v_newarray;

			v_handler[0] = new char[] { 'Á', 'A' };
			v_handler[1] = new char[] { 'À', 'A' };
			v_handler[2] = new char[] { 'Ã', 'A' };
			v_handler[3] = new char[] { 'Â', 'A' };
			v_handler[4] = new char[] { 'Ä', 'A' };
			v_handler[5] = new char[] { 'É', 'E' };
			v_handler[6] = new char[] { 'È', 'E' };
			v_handler[7] = new char[] { 'Ë', 'E' };
			v_handler[8] = new char[] { 'Ê', 'E' };
			v_handler[9] = new char[] { 'Í', 'I' };
			v_handler[10] = new char[] { 'Ì', 'I' };
			v_handler[11] = new char[] { 'Î', 'I' };
			v_handler[12] = new char[] { 'Ï', 'I' };
			v_handler[13] = new char[] { 'Ó', 'O' };
			v_handler[14] = new char[] { 'Õ', 'O' };
			v_handler[15] = new char[] { 'Ô', 'O' };
			v_handler[16] = new char[] { 'Ò', 'O' };
			v_handler[17] = new char[] { 'Ö', 'O' };
			v_handler[18] = new char[] { 'Ú', 'U' };
			v_handler[19] = new char[] { 'Ü', 'U' };
			v_handler[20] = new char[] { 'Ù', 'U' };
			v_handler[21] = new char[] { 'Û', 'U' };
			v_handler[22] = new char[] { 'Š', 'S' };
			v_handler[23] = new char[] { 'Ý', 'Y' };
			v_handler[24] = new char[] { 'Ÿ', 'Y' };
			v_handler[25] = new char[] { 'Ž', 'Z' };
			v_handler[26] = new char[] { 'Ç', 'C' };
			v_handler[27] = new char[] { 'Ñ', 'N' };
			v_handler[28] = new char[] { 'ñ', 'n' };
			v_handler[29] = new char[] { 'á', 'a' };
			v_handler[30] = new char[] { 'à', 'a' };
			v_handler[31] = new char[] { 'ã', 'a' };
			v_handler[32] = new char[] { 'â', 'a' };
			v_handler[33] = new char[] { 'ä', 'a' };
			v_handler[34] = new char[] { 'é', 'e' };
			v_handler[35] = new char[] { 'ê', 'e' };
			v_handler[36] = new char[] { 'è', 'e' };
			v_handler[37] = new char[] { 'ë', 'e' };
			v_handler[38] = new char[] { 'í', 'i' };
			v_handler[39] = new char[] { 'ì', 'i' };
			v_handler[40] = new char[] { 'î', 'i' };
			v_handler[41] = new char[] { 'ï', 'i' };
			v_handler[42] = new char[] { 'ó', 'o' };
			v_handler[43] = new char[] { 'õ', 'o' };
			v_handler[44] = new char[] { 'ô', 'o' };
			v_handler[45] = new char[] { 'ò', 'o' };
			v_handler[46] = new char[] { 'ö', 'o' };
			v_handler[47] = new char[] { 'ú', 'u' };
			v_handler[48] = new char[] { 'ü', 'u' };
			v_handler[49] = new char[] { 'ù', 'u' };
			v_handler[50] = new char[] { 'û', 'u' };
			v_handler[51] = new char[] { 'ç', 'c' };
			v_handler[52] = new char[] { 'ý', 'y' };
			v_handler[53] = new char[] { 'ÿ', 'y' };
			v_handler[54] = new char[] { 'ž', 'z' };
			v_handler[55] = new char[] { '\'', '.' };
			v_handler[56] = new char[] { '"', '.' };
			v_handler[57] = new char[] { '&', 'e' };

			v_newstring = p_string;
			for (k = 0; k < v_handler.Length; k++) 
			{
				v_newstring = v_newstring.Replace (v_handler [k] [0], v_handler [k] [1]);
			}
			v_allowed[0] = 'A';
			v_allowed[1] = 'B';
			v_allowed[2] = 'C';
			v_allowed[3] = 'D';
			v_allowed[4] = 'E';
			v_allowed[5] = 'F';
			v_allowed[6] = 'G';
			v_allowed[7] = 'H';
			v_allowed[8] = 'I';
			v_allowed[9] = 'J';
			v_allowed[10] = 'K';
			v_allowed[11] = 'L';
			v_allowed[12] = 'M';
			v_allowed[13] = 'N';
			v_allowed[14] = 'O';
			v_allowed[15] = 'P';
			v_allowed[16] = 'Q';
			v_allowed[17] = 'R';
			v_allowed[18] = 'S';
			v_allowed[19] = 'T';
			v_allowed[20] = 'U';
			v_allowed[21] = 'V';
			v_allowed[22] = 'W';
			v_allowed[23] = 'X';
			v_allowed[24] = 'Y';
			v_allowed[25] = 'Z';
			v_allowed[26] = 'a';
			v_allowed[27] = 'b';
			v_allowed[28] = 'c';
			v_allowed[29] = 'd';
			v_allowed[30] = 'e';
			v_allowed[31] = 'f';
			v_allowed[32] = 'g';
			v_allowed[33] = 'h';
			v_allowed[34] = 'i';
			v_allowed[35] = 'j';
			v_allowed[36] = 'k';
			v_allowed[37] = 'l';
			v_allowed[38] = 'm';
			v_allowed[39] = 'n';
			v_allowed[40] = 'o';
			v_allowed[41] = 'p';
			v_allowed[42] = 'q';
			v_allowed[43] = 'r';
			v_allowed[44] = 's';
			v_allowed[45] = 't';
			v_allowed[46] = 'u';
			v_allowed[47] = 'v';
			v_allowed[48] = 'w';
			v_allowed[49] = 'x';
			v_allowed[50] = 'y';
			v_allowed[51] = 'z';
			v_allowed[52] = '(';
			v_allowed[53] = ')';
			v_allowed[54] = '-';
			v_allowed[55] = '_';
			v_allowed[56] = '=';
			v_allowed[57] = '/';
			v_allowed[58] = '|';
			v_allowed[59] = '#';
			v_allowed[60] = '@';
			v_allowed[61] = ',';
			v_allowed[62] = '.';
			v_allowed[63] = '*';
			v_allowed[64] = '+';
			v_allowed[65] = ' ';
			v_allowed[66] = '0';
			v_allowed[67] = '1';
			v_allowed[68] = '2';
			v_allowed[69] = '3';
			v_allowed[70] = '4';
			v_allowed[71] = '5';
			v_allowed[72] = '6';
			v_allowed[73] = '7';
			v_allowed[74] = '8';
			v_allowed[75] = '9';
			v_allowed[76] = ':';
            v_allowed[77] = ';';
            v_allowed[78] = '\n';
            v_allowed[79] = '\r';
            v_allowed[80] = '\t';
            v_allowed[81] = '<';
            v_allowed[82] = '>';
            v_allowed[83] = '[';
            v_allowed[84] = ']';

			v_newarray = v_newstring.ToCharArray();
			for (i = 0; i < v_newarray.Length; i++)
			{
				v_achou = false;
				j = 0;
				while (j < v_allowed.Length && ! v_achou)
				{
					if (v_newarray[i] == v_allowed[j])
						v_achou = true;
					else
						j++;
				}
				if (! v_achou)
					v_newarray[i] = '_';
			}
			v_newstring = new string(v_newarray);

			return v_newstring;
		}

        /// <summary>
        /// Remove de uma string todos os caracteres com acentuação ou proibidos para a inserção SQL.
        /// Utilizada pela função Execute, que executa inserts, updates e procedures.
        /// Apenas o caractere ' (39) é permitido.
        /// </summary>
        /// <returns>
        /// String livre de caracteres com acentuação ou proibidos para a inserção SQL (com exceção do caractere ' (39)).
        /// </returns>
        /// <param name='p_string'>
        /// String a ser tratada.
        /// </param>
        public static string RemoveUnwantedCharsQuoted(string p_string)
        {
            string v_newstring;
            int i, j, k;
            char[][] v_handler = new char[57][];
            char[] v_allowed = new char[86];
            bool v_achou;
            char[] v_newarray;

            v_handler[0] = new char[] { 'Á', 'A' };
            v_handler[1] = new char[] { 'À', 'A' };
            v_handler[2] = new char[] { 'Ã', 'A' };
            v_handler[3] = new char[] { 'Â', 'A' };
            v_handler[4] = new char[] { 'Ä', 'A' };
            v_handler[5] = new char[] { 'É', 'E' };
            v_handler[6] = new char[] { 'È', 'E' };
            v_handler[7] = new char[] { 'Ë', 'E' };
            v_handler[8] = new char[] { 'Ê', 'E' };
            v_handler[9] = new char[] { 'Í', 'I' };
            v_handler[10] = new char[] { 'Ì', 'I' };
            v_handler[11] = new char[] { 'Î', 'I' };
            v_handler[12] = new char[] { 'Ï', 'I' };
            v_handler[13] = new char[] { 'Ó', 'O' };
            v_handler[14] = new char[] { 'Õ', 'O' };
            v_handler[15] = new char[] { 'Ô', 'O' };
            v_handler[16] = new char[] { 'Ò', 'O' };
            v_handler[17] = new char[] { 'Ö', 'O' };
            v_handler[18] = new char[] { 'Ú', 'U' };
            v_handler[19] = new char[] { 'Ü', 'U' };
            v_handler[20] = new char[] { 'Ù', 'U' };
            v_handler[21] = new char[] { 'Û', 'U' };
            v_handler[22] = new char[] { 'Š', 'S' };
            v_handler[23] = new char[] { 'Ý', 'Y' };
            v_handler[24] = new char[] { 'Ÿ', 'Y' };
            v_handler[25] = new char[] { 'Ž', 'Z' };
            v_handler[26] = new char[] { 'Ç', 'C' };
            v_handler[27] = new char[] { 'Ñ', 'N' };
            v_handler[28] = new char[] { 'ñ', 'n' };
            v_handler[29] = new char[] { 'á', 'a' };
            v_handler[30] = new char[] { 'à', 'a' };
            v_handler[31] = new char[] { 'ã', 'a' };
            v_handler[32] = new char[] { 'â', 'a' };
            v_handler[33] = new char[] { 'ä', 'a' };
            v_handler[34] = new char[] { 'é', 'e' };
            v_handler[35] = new char[] { 'ê', 'e' };
            v_handler[36] = new char[] { 'è', 'e' };
            v_handler[37] = new char[] { 'ë', 'e' };
            v_handler[38] = new char[] { 'í', 'i' };
            v_handler[39] = new char[] { 'ì', 'i' };
            v_handler[40] = new char[] { 'î', 'i' };
            v_handler[41] = new char[] { 'ï', 'i' };
            v_handler[42] = new char[] { 'ó', 'o' };
            v_handler[43] = new char[] { 'õ', 'o' };
            v_handler[44] = new char[] { 'ô', 'o' };
            v_handler[45] = new char[] { 'ò', 'o' };
            v_handler[46] = new char[] { 'ö', 'o' };
            v_handler[47] = new char[] { 'ú', 'u' };
            v_handler[48] = new char[] { 'ü', 'u' };
            v_handler[49] = new char[] { 'ù', 'u' };
            v_handler[50] = new char[] { 'û', 'u' };
            v_handler[51] = new char[] { 'ç', 'c' };
            v_handler[52] = new char[] { 'ý', 'y' };
            v_handler[53] = new char[] { 'ÿ', 'y' };
            v_handler[54] = new char[] { 'ž', 'z' };
            v_handler[55] = new char[] { '"', '.' };
            v_handler[56] = new char[] { '&', 'e' };

            v_newstring = p_string;
            for (k = 0; k < v_handler.Length; k++) 
            {
                v_newstring = v_newstring.Replace (v_handler [k] [0], v_handler [k] [1]);
            }
            v_allowed[0] = 'A';
            v_allowed[1] = 'B';
            v_allowed[2] = 'C';
            v_allowed[3] = 'D';
            v_allowed[4] = 'E';
            v_allowed[5] = 'F';
            v_allowed[6] = 'G';
            v_allowed[7] = 'H';
            v_allowed[8] = 'I';
            v_allowed[9] = 'J';
            v_allowed[10] = 'K';
            v_allowed[11] = 'L';
            v_allowed[12] = 'M';
            v_allowed[13] = 'N';
            v_allowed[14] = 'O';
            v_allowed[15] = 'P';
            v_allowed[16] = 'Q';
            v_allowed[17] = 'R';
            v_allowed[18] = 'S';
            v_allowed[19] = 'T';
            v_allowed[20] = 'U';
            v_allowed[21] = 'V';
            v_allowed[22] = 'W';
            v_allowed[23] = 'X';
            v_allowed[24] = 'Y';
            v_allowed[25] = 'Z';
            v_allowed[26] = 'a';
            v_allowed[27] = 'b';
            v_allowed[28] = 'c';
            v_allowed[29] = 'd';
            v_allowed[30] = 'e';
            v_allowed[31] = 'f';
            v_allowed[32] = 'g';
            v_allowed[33] = 'h';
            v_allowed[34] = 'i';
            v_allowed[35] = 'j';
            v_allowed[36] = 'k';
            v_allowed[37] = 'l';
            v_allowed[38] = 'm';
            v_allowed[39] = 'n';
            v_allowed[40] = 'o';
            v_allowed[41] = 'p';
            v_allowed[42] = 'q';
            v_allowed[43] = 'r';
            v_allowed[44] = 's';
            v_allowed[45] = 't';
            v_allowed[46] = 'u';
            v_allowed[47] = 'v';
            v_allowed[48] = 'w';
            v_allowed[49] = 'x';
            v_allowed[50] = 'y';
            v_allowed[51] = 'z';
            v_allowed[52] = '(';
            v_allowed[53] = ')';
            v_allowed[54] = '-';
            v_allowed[55] = '_';
            v_allowed[56] = '=';
            v_allowed[57] = '/';
            v_allowed[58] = '|';
            v_allowed[59] = '#';
            v_allowed[60] = '@';
            v_allowed[61] = ',';
            v_allowed[62] = '.';
            v_allowed[63] = '*';
            v_allowed[64] = '+';
            v_allowed[65] = ' ';
            v_allowed[66] = '0';
            v_allowed[67] = '1';
            v_allowed[68] = '2';
            v_allowed[69] = '3';
            v_allowed[70] = '4';
            v_allowed[71] = '5';
            v_allowed[72] = '6';
            v_allowed[73] = '7';
            v_allowed[74] = '8';
            v_allowed[75] = '9';
            v_allowed[76] = ':';
            v_allowed[77] = '\'';
            v_allowed[78] = ';';
            v_allowed[79] = '\n';
            v_allowed[80] = '\r';
            v_allowed[81] = '\t';
            v_allowed[82] = '<';
            v_allowed[83] = '>';
            v_allowed[84] = '[';
            v_allowed[85] = ']';

            v_newarray = v_newstring.ToCharArray();
            for (i = 0; i < v_newarray.Length; i++)
            {
                v_achou = false;
                j = 0;
                while (j < v_allowed.Length && ! v_achou)
                {
                    if (v_newarray[i] == v_allowed[j])
                        v_achou = true;
                    else
                        j++;
                }
                if (! v_achou)
                    v_newarray [i] = '.';
            }
            v_newstring = new string(v_newarray).Replace("'", "''");

            return v_newstring;
        }

        /// <summary>
        /// Remove de uma string todos os caracteres com acentuação ou proibidos para a inserção SQL.
        /// Utilizada pela função Execute, que executa inserts, updates e procedures.
        /// Apenas o caractere ' (39) é permitido.
        /// </summary>
        /// <returns>
        /// String livre de caracteres com acentuação ou proibidos para a inserção SQL (com exceção do caractere ' (39)).
        /// </returns>
        /// <param name='p_string'>
        /// String a ser tratada.
        /// </param>
        public static string RemoveUnwantedCharsExecute(string p_string)
        {
            string v_newstring;
            int i, j, k;
            char[][] v_handler = new char[57][];
            char[] v_allowed = new char[89];
            bool v_achou;
            char[] v_newarray;

            v_handler[0] = new char[] { 'Á', 'A' };
            v_handler[1] = new char[] { 'À', 'A' };
            v_handler[2] = new char[] { 'Ã', 'A' };
            v_handler[3] = new char[] { 'Â', 'A' };
            v_handler[4] = new char[] { 'Ä', 'A' };
            v_handler[5] = new char[] { 'É', 'E' };
            v_handler[6] = new char[] { 'È', 'E' };
            v_handler[7] = new char[] { 'Ë', 'E' };
            v_handler[8] = new char[] { 'Ê', 'E' };
            v_handler[9] = new char[] { 'Í', 'I' };
            v_handler[10] = new char[] { 'Ì', 'I' };
            v_handler[11] = new char[] { 'Î', 'I' };
            v_handler[12] = new char[] { 'Ï', 'I' };
            v_handler[13] = new char[] { 'Ó', 'O' };
            v_handler[14] = new char[] { 'Õ', 'O' };
            v_handler[15] = new char[] { 'Ô', 'O' };
            v_handler[16] = new char[] { 'Ò', 'O' };
            v_handler[17] = new char[] { 'Ö', 'O' };
            v_handler[18] = new char[] { 'Ú', 'U' };
            v_handler[19] = new char[] { 'Ü', 'U' };
            v_handler[20] = new char[] { 'Ù', 'U' };
            v_handler[21] = new char[] { 'Û', 'U' };
            v_handler[22] = new char[] { 'Š', 'S' };
            v_handler[23] = new char[] { 'Ý', 'Y' };
            v_handler[24] = new char[] { 'Ÿ', 'Y' };
            v_handler[25] = new char[] { 'Ž', 'Z' };
            v_handler[26] = new char[] { 'Ç', 'C' };
            v_handler[27] = new char[] { 'Ñ', 'N' };
            v_handler[28] = new char[] { 'ñ', 'n' };
            v_handler[29] = new char[] { 'á', 'a' };
            v_handler[30] = new char[] { 'à', 'a' };
            v_handler[31] = new char[] { 'ã', 'a' };
            v_handler[32] = new char[] { 'â', 'a' };
            v_handler[33] = new char[] { 'ä', 'a' };
            v_handler[34] = new char[] { 'é', 'e' };
            v_handler[35] = new char[] { 'ê', 'e' };
            v_handler[36] = new char[] { 'è', 'e' };
            v_handler[37] = new char[] { 'ë', 'e' };
            v_handler[38] = new char[] { 'í', 'i' };
            v_handler[39] = new char[] { 'ì', 'i' };
            v_handler[40] = new char[] { 'î', 'i' };
            v_handler[41] = new char[] { 'ï', 'i' };
            v_handler[42] = new char[] { 'ó', 'o' };
            v_handler[43] = new char[] { 'õ', 'o' };
            v_handler[44] = new char[] { 'ô', 'o' };
            v_handler[45] = new char[] { 'ò', 'o' };
            v_handler[46] = new char[] { 'ö', 'o' };
            v_handler[47] = new char[] { 'ú', 'u' };
            v_handler[48] = new char[] { 'ü', 'u' };
            v_handler[49] = new char[] { 'ù', 'u' };
            v_handler[50] = new char[] { 'û', 'u' };
            v_handler[51] = new char[] { 'ç', 'c' };
            v_handler[52] = new char[] { 'ý', 'y' };
            v_handler[53] = new char[] { 'ÿ', 'y' };
            v_handler[54] = new char[] { 'ž', 'z' };
            v_handler[55] = new char[] { '"', '.' };
            v_handler[56] = new char[] { '&', 'e' };

            v_newstring = p_string;
            for (k = 0; k < v_handler.Length; k++) 
            {
                v_newstring = v_newstring.Replace (v_handler [k] [0], v_handler [k] [1]);
            }
            v_allowed[0] = 'A';
            v_allowed[1] = 'B';
            v_allowed[2] = 'C';
            v_allowed[3] = 'D';
            v_allowed[4] = 'E';
            v_allowed[5] = 'F';
            v_allowed[6] = 'G';
            v_allowed[7] = 'H';
            v_allowed[8] = 'I';
            v_allowed[9] = 'J';
            v_allowed[10] = 'K';
            v_allowed[11] = 'L';
            v_allowed[12] = 'M';
            v_allowed[13] = 'N';
            v_allowed[14] = 'O';
            v_allowed[15] = 'P';
            v_allowed[16] = 'Q';
            v_allowed[17] = 'R';
            v_allowed[18] = 'S';
            v_allowed[19] = 'T';
            v_allowed[20] = 'U';
            v_allowed[21] = 'V';
            v_allowed[22] = 'W';
            v_allowed[23] = 'X';
            v_allowed[24] = 'Y';
            v_allowed[25] = 'Z';
            v_allowed[26] = 'a';
            v_allowed[27] = 'b';
            v_allowed[28] = 'c';
            v_allowed[29] = 'd';
            v_allowed[30] = 'e';
            v_allowed[31] = 'f';
            v_allowed[32] = 'g';
            v_allowed[33] = 'h';
            v_allowed[34] = 'i';
            v_allowed[35] = 'j';
            v_allowed[36] = 'k';
            v_allowed[37] = 'l';
            v_allowed[38] = 'm';
            v_allowed[39] = 'n';
            v_allowed[40] = 'o';
            v_allowed[41] = 'p';
            v_allowed[42] = 'q';
            v_allowed[43] = 'r';
            v_allowed[44] = 's';
            v_allowed[45] = 't';
            v_allowed[46] = 'u';
            v_allowed[47] = 'v';
            v_allowed[48] = 'w';
            v_allowed[49] = 'x';
            v_allowed[50] = 'y';
            v_allowed[51] = 'z';
            v_allowed[52] = '(';
            v_allowed[53] = ')';
            v_allowed[54] = '-';
            v_allowed[55] = '_';
            v_allowed[56] = '=';
            v_allowed[57] = '/';
            v_allowed[58] = '|';
            v_allowed[59] = '#';
            v_allowed[60] = '@';
            v_allowed[61] = ',';
            v_allowed[62] = '.';
            v_allowed[63] = '*';
            v_allowed[64] = '+';
            v_allowed[65] = ' ';
            v_allowed[66] = '0';
            v_allowed[67] = '1';
            v_allowed[68] = '2';
            v_allowed[69] = '3';
            v_allowed[70] = '4';
            v_allowed[71] = '5';
            v_allowed[72] = '6';
            v_allowed[73] = '7';
            v_allowed[74] = '8';
            v_allowed[75] = '9';
            v_allowed[76] = ':';
            v_allowed[77] = '\'';
            v_allowed[78] = ';';
            v_allowed[79] = '\n';
            v_allowed[80] = '\r';
            v_allowed[81] = '\t';
            v_allowed[82] = '<';
            v_allowed[83] = '>';
            v_allowed[84] = '[';
            v_allowed[85] = ']';
            v_allowed[86] = '%';
            v_allowed[87] = '"';
            v_allowed[88] = '\\';

            v_newarray = v_newstring.ToCharArray();
            for (i = 0; i < v_newarray.Length; i++)
            {
                v_achou = false;
                j = 0;
                while (j < v_allowed.Length && ! v_achou)
                {
                    if (v_newarray[i] == v_allowed[j])
                        v_achou = true;
                    else
                        j++;
                }
                if (! v_achou)
                    v_newarray[i] = '.';
            }
            v_newstring = new string(v_newarray);

            return v_newstring;
        }

        /// <summary>
        /// Atribui o valor do Parâmetro de nome <paramref name="p_name"/>.
        /// </summary>
        /// <param name='p_name'>
        /// Nome do Parâmetro que vai ter o valor atribuído.
        /// </param>
        /// <param name='p_value'>
        /// Valor a ser atribuído ao Parâmetro.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando o parâmetro não existir.</exception>
        public void SetValue(string p_name, string p_value)
        {
            int k;
            bool achou;

            k = 0;
            achou = false;
            while (k < this.v_parameters.Count && !achou)
            {
                if (((Spartacus.Database.Parameter)this.v_parameters[k]).v_name == p_name)
                    achou = true;
                else
                    k++;
            }

            if (achou)
            {
                if (p_value == null || p_value == "")
                {
                    ((Spartacus.Database.Parameter)this.v_parameters[k]).v_value = null;
                    ((Spartacus.Database.Parameter)this.v_parameters[k]).v_null = true;
                }
                else
                {
                    switch (((Spartacus.Database.Parameter)this.v_parameters [k]).v_type)
                    {
                        case Spartacus.Database.Type.QUOTEDSTRING:
                            ((Spartacus.Database.Parameter)this.v_parameters[k]).v_value = RemoveUnwantedCharsQuoted(p_value);
                            break;
                        case Spartacus.Database.Type.UNDEFINED:
                            ((Spartacus.Database.Parameter)this.v_parameters[k]).v_value = p_value;
                            break;
                        default:
                            ((Spartacus.Database.Parameter)this.v_parameters[k]).v_value = RemoveUnwantedChars(p_value);
                            break;
                    }
                    ((Spartacus.Database.Parameter)this.v_parameters[k]).v_null = false;
                }
            }
            else
            {
                throw new Spartacus.Database.Exception("Parâmetro de banco de dados {0} não existe.", p_name);
            }
        }

        /// <summary>
        /// Atribui o valor do Parâmetro de nome <paramref name="p_name"/>.
        /// </summary>
        /// <param name='p_name'>
        /// Nome do Parâmetro que vai ter o valor atribuído.
        /// </param>
        /// <param name='p_null'>
        /// Indica se o valor do Parâmetro deve ser NULL ou não.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando o parâmetro não existir.</exception>
        public void SetValue(string p_name, bool p_null)
        {
            int k;
            bool achou;

            k = 0;
            achou = false;
            while (k < this.v_parameters.Count && !achou)
            {
                if (((Spartacus.Database.Parameter)this.v_parameters [k]).v_name == p_name)
                    achou = true;
                else
                    k++;
            }

            if (achou)
            {
                ((Spartacus.Database.Parameter)this.v_parameters [k]).v_null = p_null;
                if (p_null)
                    ((Spartacus.Database.Parameter)this.v_parameters [k]).v_value = null;
            }
            else
            {
                throw new Spartacus.Database.Exception("Parâmetro de banco de dados {0} não existe.", p_name);
            }
        }

        /// <summary>
        /// Atribui um formato de data específico ao Parâmetro de nome <paramref name="p_name"/>.
        /// </summary>
        /// <param name='p_name'>
        /// Nome do Parâmetro.
        /// </param>
        /// <param name='p_dateformat'>
        /// Formato específico de data.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando o parâmetro não existir.</exception>
        public void SetDateFormat(string p_name, string p_dateformat)
        {
            int k;
            bool achou;

            k = 0;
            achou = false;
            while (k < this.v_parameters.Count && !achou)
            {
                if (((Spartacus.Database.Parameter)this.v_parameters [k]).v_name == p_name)
                    achou = true;
                else
                    k++;
            }

            if (achou)
                ((Spartacus.Database.Parameter)this.v_parameters [k]).v_dateformat = p_dateformat;
            else
            {
                throw new Spartacus.Database.Exception("Parâmetro de banco de dados {0} não existe.", p_name);
            }
        }

        /// <summary>
        /// Atribui uma localização, ou seja, uma representação de número real, ao Parâmetro de nome <paramref name="p_name"/>.
        /// </summary>
        /// <param name='p_name'>
        /// Nome do Parâmetro.
        /// </param>
        /// <param name='p_localse'>
        /// Localização, representação de número real, AMERICAN ou EUROPEAN.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando o parâmetro não existir.</exception>
        public void SetLocale(String p_name, Spartacus.Database.Locale p_locale)
        {
            int k;
            bool achou;

            k = 0;
            achou = false;
            while (k < this.v_parameters.Count && !achou)
            {
                if (((Spartacus.Database.Parameter)this.v_parameters [k]).v_name == p_name)
                    achou = true;
                else
                    k++;
            }

            if (achou)
                ((Spartacus.Database.Parameter)this.v_parameters [k]).v_locale = p_locale;
            else
            {
                throw new Spartacus.Database.Exception("Parâmetro de banco de dados {0} não existe.", p_name);
            }
        }

        /// <summary>
        /// Retorna o valor do Parâmetro de nome <paramref name="p_name"/>.
        /// </summary>
        /// <returns>
        /// Valor do Parâmetro.
        /// </returns>
        /// <param name='p_name'>
        /// Nome do Parâmetro.
        /// </param>
        /// <exception cref="Spartacus.Database.Exception">Exceção acontece quando o parâmetro não existir.</exception>
        public string GetValue(String p_name)
        {
            int k;
            bool achou;

            k = 0;
            achou = false;
            while (k < this.v_parameters.Count && !achou)
            {
                if (((Spartacus.Database.Parameter)this.v_parameters [k]).v_name == p_name)
                    achou = true;
                else
                    k++;
            }

            if (achou)
                return ((Spartacus.Database.Parameter)this.v_parameters [k]).v_value;
            else
            {
                throw new Spartacus.Database.Exception("Parâmetro de banco de dados {0} não existe.", p_name);
            }
        }
    }
}

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

namespace SpartacusMin.Database
{
	/// <summary>
	/// Tipos de Dados.
	/// </summary>
	public enum Type
	{
		INTEGER,
		REAL,
		BOOLEAN,
		CHAR,
		DATE,
		STRING,
		QUOTEDSTRING,
		UNDEFINED,
		SMALLINTEGER,
		BIGINTEGER,
		FLOAT,
		DOUBLE,
		DECIMAL,
		DATETIME,
		SECURESTRING,
		BYTE
	}

    /// <summary>
    /// Representações de números reais:
    /// AMERICAN: separador decimal: . separador de milhar: ,
    /// EUROPEAN: separador decimal: , separador de milhar: .
    /// </summary>
    public enum Locale
    {
        AMERICAN,
        EUROPEAN
    }

    /// <summary>
    /// Classe Parameter.
    /// Representa um parâmetro da classe <see cref="SpartacusMin.Database.Command"/>.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Nome do Parâmetro dentro do Comando SQL.
        /// Deve ser único dentro de uma mesma classe <see cref="SpartacusMin.Database.Command"/>.
        /// </summary>
        public string v_name;

        /// <summary>
        /// Tipo de Dados.
        /// </summary>
        public SpartacusMin.Database.Type v_type;

        /// <summary>
        /// Valor atual do Parâmetro.
        /// </summary>
        public string v_value;

        /// <summary>
        /// Máscara de Data, usado se caso o Parâmetro for do tipo DATE.
        /// </summary>
        public string v_datemask;

        /// <summary>
        /// Localização, representação de números reais.
        /// </summary>
        public SpartacusMin.Database.Locale v_locale;

        /// <summary>
        /// Indica se o Parâmetro possui valor NULL ou não.
        /// </summary>
        public bool v_null;

        /// <summary>
        /// Descrição do parâmetro, a ser mostrado para o usuário.
        /// </summary>
        public string v_description;

        /// <summary>
        /// String SQL para buscar as opções do parâmetro.
        /// </summary>
        public string v_lookup;


        /// <summary>
        /// Inicializa uma instância da classe <see cref="SpartacusMin.Database.Parameter"/> .
        /// </summary>
        /// <param name='p_name'>
        /// Nome do parâmetro dentro do Comando SQL.
        /// </param>
        /// <param name='p_type'>
        /// Tipo de dados do parâmetro.
        /// </param>
        public Parameter(String p_name, SpartacusMin.Database.Type p_type)
        {
            this.v_name = p_name.ToUpper();
            this.v_type = p_type;
            this.v_datemask = "to_date('#', 'dd/mm/yyyy')";

            this.v_value = "";
            this.v_null = true;

            this.v_description = "";
            this.v_lookup = "";
        }

        /// <summary>
        /// Inicializa uma instância da classe <see cref="SpartacusMin.Database.Parameter"/> .
        /// </summary>
        /// <param name='p_name'>
        /// Nome do parâmetro dentro do Comando SQL.
        /// </param>
        /// <param name='p_type'>
        /// Tipo de dados do parâmetro.
        /// </param>
        /// <param name='p_datemask'>
        /// Máscara de Data, usado se caso o parâmetro for do tipo DATE.
        /// </param>
        public Parameter(String p_name, SpartacusMin.Database.Type p_type, string p_datemask)
        {
            this.v_name = p_name.ToUpper();
            this.v_type = p_type;
            this.v_datemask = p_datemask;

            this.v_value = "";
            this.v_null = true;

            this.v_description = "";
            this.v_lookup = "";
        }

		/// <summary>
		/// Escreve o valor do Parâmetro em formato de string, para ser usado dentro do Comando SQL.
		/// Monta a string de acordo com os atributos do Parâmetro.
		/// </summary>
		public string Text()
		{
			if (this.v_null || this.v_value == null)
				return "null";
			else
			{
				if (this.v_value.Trim() == "")
				{
					if (this.v_type == SpartacusMin.Database.Type.BOOLEAN ||
					    this.v_type == SpartacusMin.Database.Type.CHAR ||
					    this.v_type == SpartacusMin.Database.Type.STRING ||
					    this.v_type == SpartacusMin.Database.Type.QUOTEDSTRING ||
					    this.v_type == SpartacusMin.Database.Type.SECURESTRING)
						return "''";
					else
						return "null";
				}
				else
				{
					if (this.v_type == SpartacusMin.Database.Type.INTEGER ||
					    this.v_type == SpartacusMin.Database.Type.SMALLINTEGER ||
					    this.v_type == SpartacusMin.Database.Type.BIGINTEGER)
						return this.v_value.Trim().Replace(".", "").Replace(",", "");
					else if (this.v_type == SpartacusMin.Database.Type.REAL ||
					         this.v_type == SpartacusMin.Database.Type.FLOAT ||
					         this.v_type == SpartacusMin.Database.Type.DOUBLE ||
					         this.v_type == SpartacusMin.Database.Type.DECIMAL)
					{
						if (this.v_locale == SpartacusMin.Database.Locale.AMERICAN)
							return this.v_value.Trim().Replace(",", "");
						else
							return this.v_value.Trim().Replace(".", "").Replace(",", ".");
					}
					else if (this.v_type == SpartacusMin.Database.Type.BOOLEAN ||
					         this.v_type == SpartacusMin.Database.Type.CHAR ||
					         this.v_type == SpartacusMin.Database.Type.STRING ||
					         this.v_type == SpartacusMin.Database.Type.QUOTEDSTRING ||
					         this.v_type == SpartacusMin.Database.Type.SECURESTRING)
						return "'" + this.v_value + "'";
					else if (this.v_type == SpartacusMin.Database.Type.DATE ||
					         this.v_type == SpartacusMin.Database.Type.DATETIME)
						return this.v_datemask.Trim().Replace("#", this.v_value.Trim());
					else if (this.v_type == SpartacusMin.Database.Type.BYTE ||
					         this.v_type == SpartacusMin.Database.Type.UNDEFINED)
						return this.v_value.Trim();
					else
						return "null";
				}
			}
		}
    }
}

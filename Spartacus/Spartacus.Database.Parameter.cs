using System;

namespace Spartacus.Database
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
        STRING
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
    /// Representa um parâmetro da classe <see cref="Spartacus.Database.Command"/>.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Nome do Parâmetro dentro do Comando SQL.
        /// Deve ser único dentro de uma mesma classe <see cref="Spartacus.Database.Command"/>.
        /// </summary>
        public string v_name;

        /// <summary>
        /// Tipo de Dados.
        /// </summary>
        public Spartacus.Database.Type v_type;

        /// <summary>
        /// Valor atual do Parâmetro.
        /// </summary>
        public string v_value;

        /// <summary>
        /// Formato de Data, usado se caso o Parâmetro for do tipo DATE.
        /// </summary>
        public string v_dateformat;

        /// <summary>
        /// Localização, representação de números reais.
        /// </summary>
        public Spartacus.Database.Locale v_locale;

        /// <summary>
        /// Indica se o Parâmetro possui valor NULL ou não.
        /// </summary>
        public bool v_null;

        /// <summary>
        /// Descrição do parâmetro, a ser mostrado para o usuário.
        /// </summary>
        public string v_description;

        /// <summary>
        /// Inicializa uma instância da classe <see cref="Spartacus.Database.Parameter"/> .
        /// </summary>
        /// <param name='p_name'>
        /// Nome do parâmetro dentro do Comando SQL.
        /// </param>
        /// <param name='p_type'>
        /// Tipo de dados do parâmetro.
        /// </param>
        public Parameter(String p_name, Spartacus.Database.Type p_type)
        {
            this.v_name = p_name;
            this.v_type = p_type;

            this.v_value = "";
            this.v_null = true;

            this.v_description = "";
        }

        /// <summary>
        /// Escreve o valor do Parâmetro em formato de string, para ser usado dentro do Comando SQL.
        /// Monta a string de acordo com os atributos do Parâmetro.
        /// </summary>
        public string Text()
        {
            if (!this.v_null)
            {
                if (string.IsNullOrEmpty(this.v_value))
                    return "null";
                else
                {
                    switch (this.v_type)
                    {
                        case Spartacus.Database.Type.INTEGER:
                            return this.v_value.Trim().Replace(".", "").Replace(",", "");
                        case Spartacus.Database.Type.REAL:
                            if (this.v_locale == Spartacus.Database.Locale.AMERICAN)
                                return this.v_value.Trim().Replace(",", "");
                            else
                                return this.v_value.Trim().Replace(".", "").Replace(",", ".");
                        case Spartacus.Database.Type.BOOLEAN:
                            return "'" + this.v_value.Trim() + "'";
                        case Spartacus.Database.Type.CHAR:
                            return "'" + this.v_value.Trim() + "'";
                        case Spartacus.Database.Type.DATE:
                            return "to_date('" + this.v_value.Trim() + "', '" + this.v_dateformat.Trim() + "')";
                        case Spartacus.Database.Type.STRING:
                            return "'" + this.v_value.Trim().Replace("'", ".") + "'";
                        default:
                            return "null";
                    }
                }
            }
            else
                return "null";
        }
    }
}


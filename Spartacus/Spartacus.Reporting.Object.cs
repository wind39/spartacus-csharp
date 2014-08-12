using System;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Tipo do Objeto.
    /// </summary>
    public enum ObjectType
    {
        IMAGE,
        TEXT,
        PAGENUMBER
    }

    /// <summary>
    /// Classe Object.
    /// Representa um objeto que pode ser renderizado dentro de um <see cref="Spartacus.Reporting.Block"/>.
    /// </summary>
    public class Object
    {
        /// <summary>
        /// Tipo do objeto.
        /// </summary>
        public Spartacus.Reporting.ObjectType v_type;

        /// <summary>
        /// Coluna associada ao Objeto.
        /// </summary>
        public string v_column;

        /// <summary>
        /// Valor do Objeto.
        /// </summary>
        public string v_value;

        /// <summary>
        /// Posição X onde o Objeto será renderizado.
        /// </summary>
        public double v_posx;

        /// <summary>
        /// Posição Y onde o Objeto será renderizado.
        /// </summary>
        public double v_posy;

        /// <summary>
        /// Alinhamento do Objeto dentro do Bloco.
        /// </summary>
        public Spartacus.Reporting.FieldAlignment v_align;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Object"/>.
        /// </summary>
        public Object()
        {
            this.v_type = Spartacus.Reporting.ObjectType.TEXT;
            this.v_column = "";
            this.v_value = "";
            this.v_posx = 0.0;
            this.v_posy = 0.0;
            this.v_align = Spartacus.Reporting.FieldAlignment.LEFT;
        }

        /// <summary>
        /// Configura a posição X do Objeto.
        /// </summary>
        /// <param name="p_text">Texto representando a posição.</param>
        public void SetPosX(string p_text)
        {
            double v_temp;

            if (System.Double.TryParse(p_text, out v_temp))
                this.v_posx = v_temp;
        }

        /// <summary>
        /// Configura a posição Y do Objeto.
        /// </summary>
        /// <param name="p_text">Texto representando a posição.</param>
        public void SetPosY(string p_text)
        {
            double v_temp;

            if (System.Double.TryParse(p_text, out v_temp))
                this.v_posy = v_temp;
        }
    }
}

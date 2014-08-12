using System;

namespace Spartacus.Reporting
{
    /// <summary>
    /// Classe Border.
    /// Armazena informações sobre quais bordas devem ser ativadas.
    /// </summary>
    public class Border
    {
        /// <summary>
        /// Borda superior.
        /// </summary>
        public bool v_top;

        /// <summary>
        /// Borda inferior.
        /// </summary>
        public bool v_bottom;

        /// <summary>
        /// Borda esquerda.
        /// </summary>
        public bool v_left;

        /// <summary>
        /// Borda direita.
        /// </summary>
        public bool v_right;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Border"/>.
        /// </summary>
        public Border()
        {
            this.v_top = false;
            this.v_bottom = false;
            this.v_left = false;
            this.v_right = false;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Reporting.Border"/>.
        /// </summary>
        /// <param name="p_text">Texto representando quais bordas devem ser ativadas.</param>
        public Border(string p_text)
        {
            char[] v_ch;

            v_ch = new char[1];
            v_ch[0] = ',';

            foreach (string s in p_text.Split(v_ch))
            {
                switch (s)
                {
                    case "TOP":
                        this.v_top = true;
                        break;
                    case "BOTTOM":
                        this.v_bottom = true;
                        break;
                    case "LEFT":
                        this.v_left = true;
                        break;
                    case "RIGHT":
                        this.v_right = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

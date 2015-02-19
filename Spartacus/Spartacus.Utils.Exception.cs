/*
The MIT License (MIT)

Copyright (c) 2014,2015 William Ivanski

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
    /// <summary>
    /// Classe Spartacus.Utils.Exception.
    /// Herda da classe <see cref="System.Exception"/>.
    /// </summary>
    public class Exception : System.Exception
    {
        /// <summary>
        /// Mensagem de Exceção.
        /// </summary>
        public string v_message;

        /// <summary>
        /// Inicializa uma nova instância da classe.
        /// </summary>
        public Exception()
            :base()
        {
            this.v_message = "[" + System.DateTime.Now.ToString() + "] " + this.ToString();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe.
        /// </summary>
        /// <param name='p_inner'>
        /// Exceção interna, encapsulada por esta exceção.
        /// </param>
        public Exception(System.Exception p_inner)
            : base(null, p_inner)
        {
            this.v_message = "[" + System.DateTime.Now.ToString() + "] " + this.ToString();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe.
        /// </summary>
        /// <param name='p_message'>
        /// Mensagem descritiva da exceção.
        /// </param>
        public Exception(string p_message)
            : base(p_message)
        {
            this.v_message = "[" + System.DateTime.Now.ToString() + "] " + this.ToString();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe.
        /// </summary>
        /// <param name='p_format'>
        /// Formato da mensagem descritiva da exceção.
        /// </param>
        /// <param name='p_args'>
        /// Argumentos da mensagem descritiva da exceção.
        /// </param>
        public Exception(string p_format, params object[] p_args)
            : base(string.Format(p_format, p_args))
        {
            this.v_message = "[" + System.DateTime.Now.ToString() + "] " + this.ToString();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe.
        /// </summary>
        /// <param name='p_message'>
        /// Mensagem descritiva da exceção.
        /// </param>
        /// <param name='p_inner'>
        /// Exceção interna, encapsulada por esta exceção.
        /// </param>
        public Exception(string p_message, System.Exception p_inner)
            : base(p_message, p_inner)
        {
            this.v_message = "[" + System.DateTime.Now.ToString() + "] " + this.ToString();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe.
        /// </summary>
        /// <param name='p_format'>
        /// Formato da mensagem descritiva da exceção.
        /// </param>
        /// <param name='p_inner'>
        /// Exceção interna, encapsulada por esta exceção.
        /// </param>
        /// <param name='p_args'>
        /// Argumentos da mensagem descritiva da exceção.
        /// </param>
        public Exception(string p_format, System.Exception p_inner, params object[] p_args)
            : base(string.Format(p_format, p_args), p_inner)
        {
            this.v_message = "[" + System.DateTime.Now.ToString() + "] " + this.ToString();
        }
    }
}

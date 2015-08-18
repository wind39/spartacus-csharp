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
    public class NotSupportedException : System.Exception
    {
        /// <summary>
        /// Mensagem de Exceção.
        /// </summary>
        public string v_message;

        /// <summary>
        /// Inicializa uma nova instância da classe.
        /// </summary>
        public NotSupportedException()
            :base()
        {
            this.v_message = "[" + System.DateTime.UtcNow.ToString() + "] " + this.ToString();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe.
        /// </summary>
        /// <param name='p_function'>
        /// Nome do método não implementado.
        /// </param>
        public NotSupportedException(string p_function)
            : base("O método " + p_function + " não é suportado na biblioteca Spartacus.")
        {
            this.v_message = "[" + System.DateTime.UtcNow.ToString() + "] " + this.ToString();
        }
    }
}

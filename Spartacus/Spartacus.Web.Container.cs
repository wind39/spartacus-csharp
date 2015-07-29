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

namespace Spartacus.Web
{
    /// <summary>
    /// Classe Container.
    /// Representa um formulário (janela), painel, ou outro componente que contenha outros componentes.
    /// </summary>
    public abstract class Container
    {
        /// <summary>
        /// Identificador do Container atual.
        /// </summary>
        public string v_id;

        /// <summary>
        /// Container pai do Container atual.
        /// </summary>
        public Spartacus.Web.Container v_parent;

        /// <summary>
        /// Lista de Containers filhos do Container atual.
        /// </summary>
        public System.Collections.ArrayList v_containers;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Container"/>.
        /// </summary>
        /// <param name="p_id">Identificador do Container atual.</param>
        public Container(string p_id)
        {
            this.v_id = p_id;

            this.v_parent = null;

            this.v_containers = new System.Collections.ArrayList();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Web.Container"/>.
        /// </summary>
        /// <param name="p_id">Identificador do Container atual.</param>
        /// <param name="p_parent">Container pai do Container atual.</param>
        public Container(string p_id, Spartacus.Web.Container p_parent)
        {
            this.v_id = p_id;

            this.v_parent = p_parent;

            this.v_containers = new System.Collections.ArrayList();
        }

        /// <summary>
        /// Adiciona um Container ao Container atual.
        /// </summary>
        /// <param name="p_container">Container a ser adicionado.</param>
        public void Add(Spartacus.Web.Container p_container)
        {
            this.v_containers.Add(p_container);
        }

        /// <summary>
        /// Limpa os dados do Container atual.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Atualiza os dados do Container atual.
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Informa o texto ou valor a ser mostrado no Container.
        /// Usado para mostrar ao usuário um formulário já preenchido.
        /// </summary>
        /// <param name="p_text">Texto a ser mostrado no Container.</param>
        public abstract void SetValue(string p_text);

        /// <summary>
        /// Retorna o texto ou valor atual do Container.
        /// </summary>
        /// <returns>Texto ou valor atual do Container.</returns>
        public abstract string GetValue();

        /// <summary>
        /// Renderiza o HTML do Container.
        /// </summary>
        public abstract string Render();
    }
}

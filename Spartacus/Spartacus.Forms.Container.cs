/*
The MIT License (MIT)

Copyright (c) 2014-2016 William Ivanski

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

namespace Spartacus.Forms
{
    /// <summary>
    /// Classe Container.
    /// Representa um formulário (janela), painel, ou outro componente que contenha outros componentes.
    /// </summary>
    public abstract class Container
    {
        /// <summary>
        /// Container pai do Container atual.
        /// </summary>
        public Spartacus.Forms.Container v_parent;

        /// <summary>
        /// Controle nativo do Container atual.
        /// </summary>
        public System.Windows.Forms.Control v_control;

        /// <summary>
        /// Largura do Container atual.
        /// </summary>
        public int v_width;

        /// <summary>
        /// Altura do Container atual.
        /// </summary>
        public int v_height;

        /// <summary>
        /// Posição X do Container dentro do Container pai.
        /// </summary>
        public int v_posx;

        /// <summary>
        /// Posição Y do Container dentro do Container pai.
        /// </summary>
        public int v_posy;

        /// <summary>
        /// Lista de Containers filhos do Container atual.
        /// </summary>
        public System.Collections.Generic.List<Spartacus.Forms.Container> v_containers;

        /// <summary>
        /// Deslocamento do eixo Y.
        /// Armazena a primeira posição Y livre para serem inseridos novos Containers.
        /// </summary>
        public int v_offsety;

        /// <summary>
        /// Soma das alturas dos Containers filhos que possuem altura fixa.
        /// </summary>
        public int v_frozenheight;

        /// <summary>
        /// Se o Container atual possui dimensões fixas dentro do Container pai.
        /// </summary>
        public bool v_isfrozen;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Container"/>.
        /// </summary>
        public Container()
        {
            this.v_parent = null;

            this.v_containers = new System.Collections.Generic.List<Container>();

            this.v_offsety = 0;

            this.v_frozenheight = 0;

            this.v_isfrozen = true;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Forms.Container"/>.
        /// </summary>
        /// <param name="p_parent">Container pai do Container atual.</param>
        public Container(Spartacus.Forms.Container p_parent)
        {
            this.v_parent = p_parent;

            this.v_containers = new System.Collections.Generic.List<Container>();

            this.v_offsety = 0;

            this.v_frozenheight = 0;

            this.v_isfrozen = true;
        }

        /// <summary>
        /// Configura a largura do Container.
        /// </summary>
        /// <param name="p_width">Largura do Container.</param>
        public void SetWidth(int p_width)
        {
            this.v_width = p_width;
            this.v_control.Width = p_width;
        }

        /// <summary>
        /// Configura a altura do Container.
        /// </summary>
        /// <param name="p_height">Altura do Container.</param>
        public void SetHeight(int p_height)
        {
            this.v_height = p_height;
            this.v_control.Height = p_height;
        }

        /// <summary>
        /// Configura a localização do Container atual dentro do Container pai.
        /// </summary>
        /// <param name="p_posx">Posição X.</param>
        /// <param name="p_posy">Posição Y.</param>
        public void SetLocation(int p_posx, int p_posy)
        {
            this.v_posx = p_posx;
            this.v_posy = p_posy;
            this.v_control.Location = new System.Drawing.Point(p_posx, p_posy);
        }

        /// <summary>
        /// Adiciona um Container ao Container atual.
        /// </summary>
        /// <param name="p_container">Container a ser adicionado.</param>
        public void Add(Spartacus.Forms.Container p_container)
        {
            this.v_containers.Add(p_container);

            p_container.v_control.Parent = this.v_control;

            this.v_offsety += p_container.v_height;

            if (p_container.v_isfrozen)
                this.v_frozenheight += p_container.v_height;
        }

        /// <summary>
        /// Redimensiona o Componente atual.
        /// Também reposiciona dentro do Container pai, se for necessário.
        /// </summary>
        /// <param name="p_newwidth">Nova largura.</param>
        /// <param name="p_newheight">Nova altura.</param>
        /// <param name="p_newposx">Nova posição X.</param>
        /// <param name="p_newposy">Nova posição Y.</param>
        public abstract void Resize(int p_newwidth, int p_newheight, int p_newposx, int p_newposy);

        /// <summary>
        /// Habilita o Container atual.
        /// </summary>
        public abstract void Enable();

        /// <summary>
        /// Desabilita o Container atual.
        /// </summary>
        public abstract void Disable();

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
    }
}

using System;
using System.Net;

namespace Spartacus.Net
{
    /// <summary>
    /// Classe Server.
    /// Herda da classe <see cref="Spartacus.Net.Endpoint"/>.
    /// </summary>
    public class Server : Spartacus.Net.Endpoint
    {
        /// <summary>
        /// Listener usado para recebe conexões de clientes.
        /// </summary>
        private System.Net.Sockets.TcpListener v_listener;


        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Net.Server"/>.
        /// </summary>
        /// <param name="p_ip">IP do Servidor.</param>
        /// <param name="p_port">Porta do Servidor.</param>
        public Server(string p_ip, int p_port)
            : base(p_ip, p_port)
        {
            this.v_ip = p_ip;
            this.v_port = p_port;

            this.v_listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse(p_ip), p_port);
            this.v_listener.Start();
        }

        /// <summary>
        /// Escuta por conexões de clientes e as aceita.
        /// </summary>
        public void Accept()
        {
            string v_context;

            try
            {
                this.v_socket = this.v_listener.AcceptTcpClient();
                this.v_stream = this.v_socket.GetStream();
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }

        /// <summary>
        /// Encerra o Servidor.
        /// </summary>
        public void StopServer()
        {
            string v_context;

            try
            {
                this.v_listener.Stop();
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }
    }
}

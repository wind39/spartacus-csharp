using System;
using System.Net;

namespace Spartacus.Net
{
    /// <summary>
    /// Classe Client.
    /// Herda da classe <see cref="Spartacus.Net.Endpoint"/>.
    /// </summary>
    public class Client : Spartacus.Net.Endpoint
    {
        /// <summary>
        /// IP do servidor.
        /// </summary>
        public string v_serverip;

        /// <summary>
        /// Porta do servidor.
        /// </summary>
        public int v_serverport;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Spartacus.Net.Client"/>.
        /// </summary>
        /// <param name="p_serverip">IP do servidor.</param>
        /// <param name="p_serverport">Porta do servidor.</param>
        /// <param name="p_clientip">IP do cliente.</param>
        /// <param name="p_clientport">Porta do cliente.</param>
        public Client(string p_serverip, int p_serverport, string p_clientip, int p_clientport)
            : base(p_clientip, p_clientport)
        {
            System.Net.IPEndPoint v_endpoint;

            this.v_serverip = p_serverip;
            this.v_serverport = p_serverport;

            v_endpoint = new IPEndPoint(System.Net.IPAddress.Parse(p_clientip), p_clientport);

            this.v_socket = new System.Net.Sockets.TcpClient(v_endpoint);
        }

        /// <summary>
        /// Conecta-se com o servidor.
        /// </summary>
        /// <exception cref="Spartacus.Net.Exception">Exceção pode ocorrer se não conseguir se conectar com o servidor.</exception>
        public void Connect()
        {
            string v_context;

            try
            {
                this.v_socket.Connect(this.v_serverip, this.v_serverport);
                this.v_stream = this.v_socket.GetStream();
            }
            catch (System.Exception exc)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, exc);
            }
        }
    }
}

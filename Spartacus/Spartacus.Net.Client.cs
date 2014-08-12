using System;
using System.Net;

namespace Spartacus.Net
{
    public class Client : Spartacus.Net.Endpoint
    {
        public string v_serverip;

        public int v_serverport;


        public Client(string p_serverip, int p_serverport, string p_clientip, int p_clientport)
            : base(p_clientip, p_clientport)
        {
            System.Net.IPEndPoint v_endpoint;

            this.v_serverip = p_serverip;
            this.v_serverport = p_serverport;

            v_endpoint = new IPEndPoint(System.Net.IPAddress.Parse(p_clientip), p_clientport);

            this.v_socket = new System.Net.Sockets.TcpClient(v_endpoint);
        }

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

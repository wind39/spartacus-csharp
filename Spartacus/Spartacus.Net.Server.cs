using System;
using System.Net;

namespace Spartacus.Net
{
    public class Server : Spartacus.Net.Endpoint
    {
        private System.Net.Sockets.TcpListener v_listener;


        public Server(string p_ip, int p_port)
            : base(p_ip, p_port)
        {
            this.v_ip = p_ip;
            this.v_port = p_port;

            this.v_listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Parse(p_ip), p_port);
            this.v_listener.Start();
        }

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

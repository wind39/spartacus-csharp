using System;

namespace Spartacus.Net
{
    public enum PacketType
    {
        QUERY,
        DATA,
        ACK,
        NACK,
        WARNING,
        ERROR
    }

    public class Packet
    {
        public Spartacus.Net.PacketType v_type;

        public int v_sequence;

        public int v_numpackets;

        public byte[] v_data;

        public byte[] v_buffer;

        private System.Text.ASCIIEncoding v_encoding;


        public Packet(Spartacus.Net.PacketType p_type)
        {
            this.v_encoding = new System.Text.ASCIIEncoding();

            this.v_type = p_type;
            this.v_sequence = 0;
            this.v_numpackets = 1;
            this.v_data = null;

            this.BuildBuffer();
        }

        public Packet(Spartacus.Net.PacketType p_type, int p_sequence, int p_numpackets, byte[] p_data)
        {
            this.v_encoding = new System.Text.ASCIIEncoding();

            this.v_type = p_type;
            this.v_sequence = p_sequence;
            this.v_numpackets = p_numpackets;
            this.v_data = p_data;

            this.BuildBuffer();
        }

        public Packet(Spartacus.Net.PacketType p_type, int p_sequence, int p_numpackets, string p_data)
        {
            this.v_encoding = new System.Text.ASCIIEncoding();

            this.v_type = p_type;
            this.v_sequence = p_sequence;
            this.v_numpackets = p_numpackets;
            this.v_data = this.v_encoding.GetBytes(p_data);

            this.BuildBuffer();
        }

        public Packet(byte[] p_buffer)
        {
            this.v_encoding = new System.Text.ASCIIEncoding();

            this.v_buffer = p_buffer;

            this.ParseBuffer();
        }

        private void BuildBuffer()
        {
            byte[] v_tmpbuffer;

            if (this.v_data != null && this.v_data.Length > 0)
                this.v_buffer = new byte[this.v_data.Length + 25];
            else
                this.v_buffer = new byte[25];

            // tipo do pacote
            switch (this.v_type)
            {
                case Spartacus.Net.PacketType.QUERY:
                    v_tmpbuffer = this.v_encoding.GetBytes("QUERY");
                    break;
                case Spartacus.Net.PacketType.DATA:
                    v_tmpbuffer = this.v_encoding.GetBytes("DATA ");
                    break;
                case Spartacus.Net.PacketType.ACK:
                    v_tmpbuffer = this.v_encoding.GetBytes("ACK  ");
                    break;
                case Spartacus.Net.PacketType.NACK:
                    v_tmpbuffer = this.v_encoding.GetBytes("NACK ");
                    break;
                case Spartacus.Net.PacketType.ERROR:
                    v_tmpbuffer = this.v_encoding.GetBytes("ERROR");
                    break;
                case Spartacus.Net.PacketType.WARNING:
                    v_tmpbuffer = this.v_encoding.GetBytes("WARNG");
                    break;
                default:
                    v_tmpbuffer = null;
                    break;
            }
            System.Array.Copy(v_tmpbuffer, 0, this.v_buffer, 0, v_tmpbuffer.Length);

            // sequencia do pacote
            v_tmpbuffer = this.v_encoding.GetBytes(this.v_sequence.ToString().PadLeft(10, '0'));
            System.Array.Copy(v_tmpbuffer, 0, this.v_buffer, 5, v_tmpbuffer.Length);

            // numero total de pacotes
            v_tmpbuffer = this.v_encoding.GetBytes(this.v_numpackets.ToString().PadLeft(10, '0'));
            System.Array.Copy(v_tmpbuffer, 0, this.v_buffer, 15, v_tmpbuffer.Length);

            // dados
            if (this.v_data != null && this.v_data.Length > 0)
                System.Array.Copy(this.v_data, 0, this.v_buffer, 25, this.v_data.Length);
        }

        private void ParseBuffer()
        {
            byte[] v_tmpbuffer;
            string v_tmp;
            string v_context;

            if (this.v_buffer.Length < 25)
            {
                v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new Spartacus.Net.Exception(v_context, "Pacote muito pequeno.");
            }

            v_tmpbuffer = new byte[25];
            System.Array.Copy(this.v_buffer, 0, v_tmpbuffer, 0, v_tmpbuffer.Length);
            v_tmp = this.v_encoding.GetString(v_tmpbuffer);

            // tipo do pacote
            switch (v_tmp.Substring(0, 5))
            {
                case "QUERY":
                    this.v_type = Spartacus.Net.PacketType.QUERY;
                    break;
                case "DATA ":
                    this.v_type = Spartacus.Net.PacketType.DATA;
                    break;
                case "ACK  ":
                    this.v_type = Spartacus.Net.PacketType.ACK;
                    break;
                case "NACK ":
                    this.v_type = Spartacus.Net.PacketType.NACK;
                    break;
                case "ERROR":
                    this.v_type = Spartacus.Net.PacketType.ERROR;
                    break;
                case "WARNG":
                    this.v_type = Spartacus.Net.PacketType.WARNING;
                    break;
                default:
                    v_context = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    throw new Spartacus.Net.Exception(v_context, "Tipo de pacote [{0}] não existe.", v_tmp.Substring(0, 5));
            }

            // sequencia
            this.v_sequence = System.Int32.Parse(v_tmp.Substring(5, 10));

            // numero total de pacotes
            this.v_numpackets = System.Int32.Parse(v_tmp.Substring(15, 10));

            // dados
            if (this.v_buffer.Length > 25)
            {
                this.v_data = new byte[this.v_buffer.Length - 25];
                System.Array.Copy(this.v_buffer, 25, this.v_data, 0, this.v_data.Length);
            }
            else
                this.v_data = null;
        }

        public string GetString()
        {
            return this.v_encoding.GetString(this.v_data);
        }
    }
}

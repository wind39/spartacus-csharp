using System;
using Spartacus;

namespace SpartacusDataClient
{
    class MainWindow : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TextBox txt_sql;
        private System.Windows.Forms.Button bt_query;
        private System.Windows.Forms.DataGridView dgv_grid;
        private System.Windows.Forms.StatusBar sb_status;

        private Spartacus.Net.Client v_client;

        //private Spartacus.Database.Generic v_database;
        private System.Data.DataTable v_table;

        public MainWindow(string p_serverip, int p_serverport, string p_clientip, int p_clientport)
        {
            System.Windows.Forms.Panel v_panel_sql;
            System.Windows.Forms.Panel v_panel_buttons;
            System.Windows.Forms.Panel v_panel_grid;

            this.Size = new System.Drawing.Size(1024, 612);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.MaximizeBox = false;
            this.Text = "Spartacus Data Client";
            this.Icon = new System.Drawing.Icon("spartacusdataclient.ico");

            v_panel_sql = new System.Windows.Forms.Panel();
            v_panel_sql.Parent = this;
            v_panel_sql.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            v_panel_sql.Size = new System.Drawing.Size(this.Width, 200);
            v_panel_sql.Location = new System.Drawing.Point(0, 0);

            v_panel_buttons = new System.Windows.Forms.Panel();
            v_panel_buttons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            v_panel_buttons.Parent = this;
            v_panel_buttons.Size = new System.Drawing.Size(this.Width, 50);
            v_panel_buttons.Location = new System.Drawing.Point(0, 200);

            v_panel_grid = new System.Windows.Forms.Panel();
            v_panel_grid.Parent = this;
            v_panel_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            v_panel_grid.Size = new System.Drawing.Size(this.Width, 300);
            v_panel_grid.Location = new System.Drawing.Point(0, 250);

            this.sb_status = new System.Windows.Forms.StatusBar();
            this.sb_status.Parent = this;
            this.sb_status.Text = "Pronto.";

            this.txt_sql = new System.Windows.Forms.TextBox();
            this.txt_sql.Parent = v_panel_sql;
            this.txt_sql.Font = new System.Drawing.Font("Courier New", 11.0F);
            this.txt_sql.Multiline = true;
            this.txt_sql.Dock = System.Windows.Forms.DockStyle.Fill;

            this.bt_query = new System.Windows.Forms.Button();
            this.bt_query.Parent = v_panel_buttons;
            this.bt_query.Text = "Buscar";
            this.bt_query.Size = new System.Drawing.Size(100, 30);
            this.bt_query.Location = new System.Drawing.Point(this.Width - this.bt_query.Width - 15, 10);
            this.bt_query.Click += new System.EventHandler(this.bt_query_Clicked);

            this.dgv_grid = new System.Windows.Forms.DataGridView();
            this.dgv_grid.Parent = v_panel_grid;
            this.dgv_grid.Dock = System.Windows.Forms.DockStyle.Fill;

            this.CenterToScreen();

            try
            {
                this.v_client = new Spartacus.Net.Client(p_serverip, p_serverport, p_clientip, p_clientport);
                this.v_client.Connect();
                System.Windows.Forms.MessageBox.Show(string.Format("Conectado ao servidor {0} na porta {1}.", p_serverip, p_serverport), "Conectou!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                System.Windows.Forms.MessageBox.Show(exc_net.v_message, "ERRO", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                this.Close();
                System.Environment.Exit(-1);
            }
            catch (System.Exception exc)
            {
                System.Windows.Forms.MessageBox.Show(exc.Message, "ERRO", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                this.Close();
                System.Environment.Exit(-1);
            }

            /*
            try
            {
                this.v_database = new Spartacus.Database.Odbc("tpmp563", "planning", "plaserv");
                this.v_database.Connect();
            }
            catch (Spartacus.Database.Exception exc)
            {
                System.Windows.Forms.MessageBox.Show(exc.v_message, "ERRO", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                this.Close();
                System.Environment.Exit(-1);
            }
            */
        }

        private void bt_query_Clicked(object sender, System.EventArgs e)
        {
            Spartacus.Net.Packet v_packetsend, v_packetrecv;
            bool v_ack;

            /*
            try
            {
                this.v_table = this.v_database.Query(this.txt_sql.Text, "RESULTS");
            }
            catch (System.Exception exc)
            {
                System.Windows.Forms.MessageBox.Show(exc.Message, "ERRO", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                this.sb_status.Text = "Erro ao executar a consulta.";
                return;
            }

            if (this.v_table == null)
            {
                this.sb_status.Text = "Erro ao executar a consulta.";
                return;
            }
            */

            try
            {
                // montando consulta
                v_packetsend = new Spartacus.Net.Packet(Spartacus.Net.PacketType.DATA, 0, 1, this.txt_sql.Text);

                v_ack = false;
                while (! v_ack)
                {
                    // enviando consulta
                    this.v_client.Send(v_packetsend);

                    // recebendo ACK
                    v_packetrecv = this.v_client.Recv();
                    if (v_packetrecv.v_type == Spartacus.Net.PacketType.ACK)
                        v_ack = true;
                }

                // recebendo dados de retorno
                this.v_table = this.v_client.RecvDataTable();
            }
            catch (Spartacus.Net.Exception exc_net)
            {
                System.Windows.Forms.MessageBox.Show(exc_net.v_message, "ERRO", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                this.sb_status.Text = "Erro ao executar a consulta.";
                return;
            }
            catch (System.Exception exc)
            {
                System.Windows.Forms.MessageBox.Show(exc.Message, "ERRO", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                this.sb_status.Text = "Erro ao executar a consulta.";
                return;
            }

            if (this.v_table == null)
            {
                this.sb_status.Text = "Erro ao executar a consulta.";
                return;
            }

            this.dgv_grid.DataSource = this.v_table;

            this.sb_status.Text = string.Format("Foram buscados {0} registros em {1} colunas.", this.v_table.Rows.Count, this.v_table.Columns.Count);
        }
    }

    class MainClass
    {
        // args[0] = server ip
        // args[1] = server port
        // args[2] = client ip
        // args[3] = client port
        public static void Main(string[] args)
        {
            MainWindow v_window;

            if (args.Length != 4)
            {
                System.Console.WriteLine("Uso: ./client <serverip> <serverport> <clientip> <clientport>");
                System.Environment.Exit(0);
            }

            v_window = new MainWindow(args [0], System.Int32.Parse(args [1]), args [2], System.Int32.Parse(args [3]));

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.Run(v_window);
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpartacusCryptor
{
    class MainWindow : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label lb_password;
        private System.Windows.Forms.TextBox txt_password;

        private System.Windows.Forms.Label lb_input;
        private System.Windows.Forms.TextBox txt_input;

        private System.Windows.Forms.Label lb_output;
        private System.Windows.Forms.TextBox txt_output;

        private System.Windows.Forms.StatusBar sb_status;

        private System.Windows.Forms.Button bt_encrypt;
        private System.Windows.Forms.Button bt_decrypt;

        public MainWindow()
        {
            this.Size = new System.Drawing.Size(600, 700);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.MaximizeBox = false;
            this.Text = "Spartacus Cryptor";
            this.Icon = new System.Drawing.Icon("spartacuscryptor.ico");

            this.lb_password = new System.Windows.Forms.Label();
            this.lb_password.Parent = this;
            this.lb_password.Text = "Senha";
            this.lb_password.Location = new System.Drawing.Point(10, 10);

            this.txt_password = new System.Windows.Forms.TextBox();
            this.txt_password.Parent = this;
            this.txt_password.Multiline = false;
            this.txt_password.Size = new System.Drawing.Size(575, 30);
            this.txt_password.Location = new System.Drawing.Point(10, 40);

            this.lb_input = new System.Windows.Forms.Label();
            this.lb_input.Parent = this;
            this.lb_input.Text = "Entrada";
            this.lb_input.Location = new System.Drawing.Point(10, 80);

            this.txt_input = new System.Windows.Forms.TextBox();
            this.txt_input.Parent = this;
            this.txt_input.Multiline = true;
            this.txt_input.Size = new System.Drawing.Size(575, 200);
            this.txt_input.Location = new System.Drawing.Point(10, 110);

            this.lb_output = new System.Windows.Forms.Label();
            this.lb_output.Parent = this;
            this.lb_output.Text = "Saída";
            this.lb_output.Location = new System.Drawing.Point(10, 330);

            this.txt_output = new System.Windows.Forms.TextBox();
            this.txt_output.Parent = this;
            this.txt_output.Multiline = true;
            this.txt_output.Size = new System.Drawing.Size(575, 200);
            this.txt_output.Location = new System.Drawing.Point(10, 360);

            this.bt_encrypt = new System.Windows.Forms.Button();
            this.bt_encrypt.Parent = this;
            this.bt_encrypt.Text = "Criptografar";
            this.bt_encrypt.Size = new System.Drawing.Size(100, 30);
            this.bt_encrypt.Location = new System.Drawing.Point(370, 580);
            this.bt_encrypt.Click += new System.EventHandler(this.bt_encrypt_Clicked);

            this.bt_decrypt = new System.Windows.Forms.Button();
            this.bt_decrypt.Parent = this;
            this.bt_decrypt.Text = "Descriptografar";
            this.bt_decrypt.Size = new System.Drawing.Size(100, 30);
            this.bt_decrypt.Location = new System.Drawing.Point(485, 580);
            this.bt_decrypt.Click += new System.EventHandler(this.bt_decrypt_Clicked);

            this.sb_status = new System.Windows.Forms.StatusBar();
            this.sb_status.Parent = this;
            this.sb_status.Text = "Pronto.";

            this.CenterToScreen();
        }

        private void bt_encrypt_Clicked(object sender, System.EventArgs e)
        {
            Spartacus.Net.Cryptor v_cryptor;

            try
            {
                v_cryptor = new Spartacus.Net.Cryptor(this.txt_password.Text);
                this.txt_output.Text = v_cryptor.Encrypt(this.txt_input.Text);
                this.sb_status.Text = "Criptografado com sucesso.";
            }
            catch (System.Exception)
            {
                this.sb_status.Text = "Erro ao criptografar.";
            }
        }

        private void bt_decrypt_Clicked(object sender, System.EventArgs e)
        {
            Spartacus.Net.Cryptor v_cryptor;

            try
            {
                v_cryptor = new Spartacus.Net.Cryptor(this.txt_password.Text);
                this.txt_output.Text = v_cryptor.Decrypt(this.txt_input.Text);
                this.sb_status.Text = "Descriptografado com sucesso.";
            }
            catch (System.Exception)
            {
                this.sb_status.Text = "Erro ao descriptografar.";
            }
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.Run(new MainWindow());
        }
    }
}

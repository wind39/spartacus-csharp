using System;

namespace Spartacus.Forms
{
    public class Form : System.Windows.Forms.Form
    {
        public bool v_showing;


        public Form()
            : base()
        {
            this.v_showing = false;
        }

        protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e)
        {
            if (e.CloseReason == System.Windows.Forms.CloseReason.WindowsShutDown ||
                e.CloseReason == System.Windows.Forms.CloseReason.ApplicationExitCall ||
                e.CloseReason == System.Windows.Forms.CloseReason.TaskManagerClosing)
            { 
                return; 
            }

            e.Cancel = true;

            this.v_showing = false;
            this.Hide();
        }
    }
}


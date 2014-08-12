using System;

namespace Spartacus.Utils
{
    public class WarningEventArgs : System.EventArgs
    {
        public string v_process;
        public string v_subprocess;
        public string v_message;
        public bool v_verbose;

        public WarningEventArgs()
        {
            this.v_verbose = false;
        }

        public WarningEventArgs(bool p_verbose)
        {
            this.v_verbose = p_verbose;
        }
    }

    public class WarningEventClass
    {
        public delegate void WarningEventHandler(Spartacus.Utils.WarningEventClass obj, Spartacus.Utils.WarningEventArgs e);
        public event WarningEventHandler WarningEvent;
        public Spartacus.Utils.WarningEventArgs WarningEventArgs = null;

        public WarningEventClass()
        {
            this.WarningEventArgs = new Spartacus.Utils.WarningEventArgs();
        }

        public WarningEventClass(bool p_verbose)
        {
            this.WarningEventArgs = new Spartacus.Utils.WarningEventArgs(p_verbose);
        }

        public void FireEvent(string p_process, string p_subprocess, string p_message)
        {
            if (this.WarningEvent != null)
            {
                this.WarningEventArgs.v_process = p_process;
                this.WarningEventArgs.v_subprocess = p_subprocess;
                this.WarningEventArgs.v_message = p_message;

                this.WarningEvent(this, this.WarningEventArgs);
            }
        }
    }
}

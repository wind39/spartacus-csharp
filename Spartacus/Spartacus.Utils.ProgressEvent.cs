using System;

namespace Spartacus.Utils
{
    public class ProgressEventArgs : System.EventArgs
    {
        public string v_process;
        public string v_subprocess;
        public double v_percentage;
        public string v_message;
        public bool v_verbose;

        public ProgressEventArgs()
        {
            this.v_verbose = false;
        }

        public ProgressEventArgs(bool p_verbose)
        {
            this.v_verbose = p_verbose;
        }
    }

    public class ProgressEventClass
    {
        public delegate void ProgressEventHandler(Spartacus.Utils.ProgressEventClass obj, Spartacus.Utils.ProgressEventArgs e);
        public event ProgressEventHandler ProgressEvent;
        public Spartacus.Utils.ProgressEventArgs ProgressEventArgs = null;

        public ProgressEventClass()
        {
            this.ProgressEventArgs = new Spartacus.Utils.ProgressEventArgs();
        }

        public ProgressEventClass(bool p_verbose)
        {
            this.ProgressEventArgs = new Spartacus.Utils.ProgressEventArgs(p_verbose);
        }

        public void FireEvent(string p_process, string p_subprocess, double p_percentage, string p_message)
        {
            if (this.ProgressEvent != null)
            {
                this.ProgressEventArgs.v_process = p_process;
                this.ProgressEventArgs.v_subprocess = p_subprocess;
                this.ProgressEventArgs.v_percentage = p_percentage;
                this.ProgressEventArgs.v_message = p_message;

                this.ProgressEvent(this, this.ProgressEventArgs);
            }
        }
    }
}

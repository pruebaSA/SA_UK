namespace System.ComponentModel
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class DoWorkEventArgs : CancelEventArgs
    {
        private object argument;
        private object result;

        public DoWorkEventArgs(object argument)
        {
            this.argument = argument;
        }

        [SRDescription("BackgroundWorker_DoWorkEventArgs_Argument")]
        public object Argument =>
            this.argument;

        [SRDescription("BackgroundWorker_DoWorkEventArgs_Result")]
        public object Result
        {
            get => 
                this.result;
            set
            {
                this.result = value;
            }
        }
    }
}


namespace System.ComponentModel
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class CancelEventArgs : EventArgs
    {
        private bool cancel;

        public CancelEventArgs() : this(false)
        {
        }

        public CancelEventArgs(bool cancel)
        {
            this.cancel = cancel;
        }

        public bool Cancel
        {
            get => 
                this.cancel;
            set
            {
                this.cancel = value;
            }
        }
    }
}


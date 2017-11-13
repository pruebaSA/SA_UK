namespace Microsoft.Win32
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public class SessionEndingEventArgs : EventArgs
    {
        private bool cancel;
        private readonly SessionEndReasons reason;

        public SessionEndingEventArgs(SessionEndReasons reason)
        {
            this.reason = reason;
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

        public SessionEndReasons Reason =>
            this.reason;
    }
}


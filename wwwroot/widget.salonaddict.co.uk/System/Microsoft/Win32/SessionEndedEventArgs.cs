namespace Microsoft.Win32
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public class SessionEndedEventArgs : EventArgs
    {
        private readonly SessionEndReasons reason;

        public SessionEndedEventArgs(SessionEndReasons reason)
        {
            this.reason = reason;
        }

        public SessionEndReasons Reason =>
            this.reason;
    }
}


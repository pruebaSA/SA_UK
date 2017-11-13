namespace Microsoft.Win32
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class TimerElapsedEventArgs : EventArgs
    {
        private readonly IntPtr timerId;

        public TimerElapsedEventArgs(IntPtr timerId)
        {
            this.timerId = timerId;
        }

        public IntPtr TimerId =>
            this.timerId;
    }
}


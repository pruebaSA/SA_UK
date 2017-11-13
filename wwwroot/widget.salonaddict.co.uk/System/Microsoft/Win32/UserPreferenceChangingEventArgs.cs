namespace Microsoft.Win32
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class UserPreferenceChangingEventArgs : EventArgs
    {
        private readonly UserPreferenceCategory category;

        public UserPreferenceChangingEventArgs(UserPreferenceCategory category)
        {
            this.category = category;
        }

        public UserPreferenceCategory Category =>
            this.category;
    }
}


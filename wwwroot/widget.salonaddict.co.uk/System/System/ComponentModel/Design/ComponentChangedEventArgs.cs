namespace System.ComponentModel.Design
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public sealed class ComponentChangedEventArgs : EventArgs
    {
        private object component;
        private MemberDescriptor member;
        private object newValue;
        private object oldValue;

        public ComponentChangedEventArgs(object component, MemberDescriptor member, object oldValue, object newValue)
        {
            this.component = component;
            this.member = member;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public object Component =>
            this.component;

        public MemberDescriptor Member =>
            this.member;

        public object NewValue =>
            this.newValue;

        public object OldValue =>
            this.oldValue;
    }
}


namespace System.ComponentModel.Design.Serialization
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class ResolveNameEventArgs : EventArgs
    {
        private string name;
        private object value;

        public ResolveNameEventArgs(string name)
        {
            this.name = name;
            this.value = null;
        }

        public string Name =>
            this.name;

        public object Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}


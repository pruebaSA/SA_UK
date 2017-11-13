namespace System.ComponentModel
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class RefreshEventArgs : EventArgs
    {
        private object componentChanged;
        private Type typeChanged;

        public RefreshEventArgs(object componentChanged)
        {
            this.componentChanged = componentChanged;
            this.typeChanged = componentChanged.GetType();
        }

        public RefreshEventArgs(Type typeChanged)
        {
            this.typeChanged = typeChanged;
        }

        public object ComponentChanged =>
            this.componentChanged;

        public Type TypeChanged =>
            this.typeChanged;
    }
}


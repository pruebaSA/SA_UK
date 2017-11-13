namespace System.ComponentModel.Design
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, SharedState=true), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class ActiveDesignerEventArgs : EventArgs
    {
        private readonly IDesignerHost newDesigner;
        private readonly IDesignerHost oldDesigner;

        public ActiveDesignerEventArgs(IDesignerHost oldDesigner, IDesignerHost newDesigner)
        {
            this.oldDesigner = oldDesigner;
            this.newDesigner = newDesigner;
        }

        public IDesignerHost NewDesigner =>
            this.newDesigner;

        public IDesignerHost OldDesigner =>
            this.oldDesigner;
    }
}


namespace System.Drawing.Design
{
    using System;
    using System.ComponentModel.Design;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class ToolboxComponentsCreatingEventArgs : EventArgs
    {
        private readonly IDesignerHost host;

        public ToolboxComponentsCreatingEventArgs(IDesignerHost host)
        {
            this.host = host;
        }

        public IDesignerHost DesignerHost =>
            this.host;
    }
}


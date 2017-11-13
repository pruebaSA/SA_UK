namespace System.Web.Services.Protocols
{
    using System;
    using System.Security.Permissions;
    using System.Security.Policy;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public class ServerType
    {
        private System.Type type;

        public ServerType(System.Type type)
        {
            this.type = type;
        }

        internal System.Security.Policy.Evidence Evidence
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
                return this.Type.Assembly.Evidence;
            }
        }

        internal System.Type Type =>
            this.type;
    }
}


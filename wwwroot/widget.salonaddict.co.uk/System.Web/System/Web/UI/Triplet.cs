namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class Triplet
    {
        public object First;
        public object Second;
        public object Third;

        public Triplet()
        {
        }

        public Triplet(object x, object y)
        {
            this.First = x;
            this.Second = y;
        }

        public Triplet(object x, object y, object z)
        {
            this.First = x;
            this.Second = y;
            this.Third = z;
        }
    }
}


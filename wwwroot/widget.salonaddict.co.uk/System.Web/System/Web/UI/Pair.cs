namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class Pair
    {
        public object First;
        public object Second;

        public Pair()
        {
        }

        public Pair(object x, object y)
        {
            this.First = x;
            this.Second = y;
        }
    }
}


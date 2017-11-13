namespace System.Runtime.Remoting.Activation
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Contexts;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class UrlAttribute : ContextAttribute
    {
        private static string propertyName = "UrlAttribute";
        private string url;

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public UrlAttribute(string callsiteURL) : base(propertyName)
        {
            if (callsiteURL == null)
            {
                throw new ArgumentNullException("callsiteURL");
            }
            this.url = callsiteURL;
        }

        public override bool Equals(object o) => 
            (((o is IContextProperty) && (o is UrlAttribute)) && ((UrlAttribute) o).UrlValue.Equals(this.url));

        public override int GetHashCode() => 
            this.url.GetHashCode();

        [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
        {
        }

        [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public override bool IsContextOK(Context ctx, IConstructionCallMessage msg) => 
            false;

        public string UrlValue =>
            this.url;
    }
}


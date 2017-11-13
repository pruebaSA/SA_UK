namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class IndexedString
    {
        private string _value;

        public IndexedString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException("s");
            }
            this._value = s;
        }

        public string Value =>
            this._value;
    }
}


namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebPartUserCapability
    {
        private string _name;

        public WebPartUserCapability(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("name");
            }
            this._name = name;
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            WebPartUserCapability capability = o as WebPartUserCapability;
            return ((capability != null) && (capability.Name == this.Name));
        }

        public override int GetHashCode() => 
            this._name.GetHashCode();

        public string Name =>
            this._name;
    }
}


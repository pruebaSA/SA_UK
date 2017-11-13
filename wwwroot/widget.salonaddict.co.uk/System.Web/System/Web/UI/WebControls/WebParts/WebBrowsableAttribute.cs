namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebBrowsableAttribute : Attribute
    {
        private bool _browsable;
        public static readonly WebBrowsableAttribute Default = No;
        public static readonly WebBrowsableAttribute No = new WebBrowsableAttribute(false);
        public static readonly WebBrowsableAttribute Yes = new WebBrowsableAttribute(true);

        public WebBrowsableAttribute() : this(true)
        {
        }

        public WebBrowsableAttribute(bool browsable)
        {
            this._browsable = browsable;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            WebBrowsableAttribute attribute = obj as WebBrowsableAttribute;
            return ((attribute != null) && (attribute.Browsable == this.Browsable));
        }

        public override int GetHashCode() => 
            this._browsable.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public bool Browsable =>
            this._browsable;
    }
}


namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebDisplayNameAttribute : Attribute
    {
        private string _displayName;
        public static readonly WebDisplayNameAttribute Default = new WebDisplayNameAttribute();

        public WebDisplayNameAttribute() : this(string.Empty)
        {
        }

        public WebDisplayNameAttribute(string displayName)
        {
            this._displayName = displayName;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            WebDisplayNameAttribute attribute = obj as WebDisplayNameAttribute;
            return ((attribute != null) && (attribute.DisplayName == this.DisplayName));
        }

        public override int GetHashCode() => 
            this.DisplayName.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public virtual string DisplayName =>
            this.DisplayNameValue;

        protected string DisplayNameValue
        {
            get => 
                this._displayName;
            set
            {
                this._displayName = value;
            }
        }
    }
}


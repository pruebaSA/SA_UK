namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebDescriptionAttribute : Attribute
    {
        private string _description;
        public static readonly WebDescriptionAttribute Default = new WebDescriptionAttribute();

        public WebDescriptionAttribute() : this(string.Empty)
        {
        }

        public WebDescriptionAttribute(string description)
        {
            this._description = description;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            WebDescriptionAttribute attribute = obj as WebDescriptionAttribute;
            return ((attribute != null) && (attribute.Description == this.Description));
        }

        public override int GetHashCode() => 
            this.Description.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public virtual string Description =>
            this.DescriptionValue;

        protected string DescriptionValue
        {
            get => 
                this._description;
            set
            {
                this._description = value;
            }
        }
    }
}


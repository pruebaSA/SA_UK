namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class UrlPropertyAttribute : Attribute
    {
        private string _filter;

        public UrlPropertyAttribute() : this("*.*")
        {
        }

        public UrlPropertyAttribute(string filter)
        {
            if (filter == null)
            {
                this._filter = "*.*";
            }
            else
            {
                this._filter = filter;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            UrlPropertyAttribute attribute = obj as UrlPropertyAttribute;
            return ((attribute != null) && this.Filter.Equals(attribute.Filter));
        }

        public override int GetHashCode() => 
            this.Filter.GetHashCode();

        public string Filter =>
            this._filter;
    }
}


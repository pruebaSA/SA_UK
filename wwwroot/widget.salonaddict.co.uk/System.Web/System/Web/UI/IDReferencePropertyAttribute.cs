namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class IDReferencePropertyAttribute : Attribute
    {
        private Type _referencedControlType;

        public IDReferencePropertyAttribute() : this(typeof(Control))
        {
        }

        public IDReferencePropertyAttribute(Type referencedControlType)
        {
            this._referencedControlType = referencedControlType;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            IDReferencePropertyAttribute attribute = obj as IDReferencePropertyAttribute;
            return ((attribute != null) && (this.ReferencedControlType == attribute.ReferencedControlType));
        }

        public override int GetHashCode() => 
            this.ReferencedControlType?.GetHashCode();

        public Type ReferencedControlType =>
            this._referencedControlType;
    }
}


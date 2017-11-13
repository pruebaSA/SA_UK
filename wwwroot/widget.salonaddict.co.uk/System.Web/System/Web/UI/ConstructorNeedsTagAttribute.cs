namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ConstructorNeedsTagAttribute : Attribute
    {
        private bool needsTag;

        public ConstructorNeedsTagAttribute()
        {
        }

        public ConstructorNeedsTagAttribute(bool needsTag)
        {
            this.needsTag = needsTag;
        }

        public bool NeedsTag =>
            this.needsTag;
    }
}


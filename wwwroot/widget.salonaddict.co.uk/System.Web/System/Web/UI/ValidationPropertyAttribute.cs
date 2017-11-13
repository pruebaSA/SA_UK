namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ValidationPropertyAttribute : Attribute
    {
        private readonly string name;

        public ValidationPropertyAttribute(string name)
        {
            this.name = name;
        }

        public string Name =>
            this.name;
    }
}


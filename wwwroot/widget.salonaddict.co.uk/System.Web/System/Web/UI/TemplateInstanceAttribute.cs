namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TemplateInstanceAttribute : Attribute
    {
        private TemplateInstance _instances;
        public static readonly TemplateInstanceAttribute Default = Multiple;
        public static readonly TemplateInstanceAttribute Multiple = new TemplateInstanceAttribute(TemplateInstance.Multiple);
        public static readonly TemplateInstanceAttribute Single = new TemplateInstanceAttribute(TemplateInstance.Single);

        public TemplateInstanceAttribute(TemplateInstance instances)
        {
            this._instances = instances;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            TemplateInstanceAttribute attribute = obj as TemplateInstanceAttribute;
            return ((attribute != null) && (attribute.Instances == this.Instances));
        }

        public override int GetHashCode() => 
            this._instances.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public TemplateInstance Instances =>
            this._instances;
    }
}


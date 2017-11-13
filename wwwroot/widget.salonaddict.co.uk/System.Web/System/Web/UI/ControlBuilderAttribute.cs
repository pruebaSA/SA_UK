namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ControlBuilderAttribute : Attribute
    {
        private Type builderType;
        public static readonly ControlBuilderAttribute Default = new ControlBuilderAttribute(null);

        public ControlBuilderAttribute(Type builderType)
        {
            this.builderType = builderType;
        }

        public override bool Equals(object obj) => 
            ((obj == this) || (((obj != null) && (obj is ControlBuilderAttribute)) && (((ControlBuilderAttribute) obj).BuilderType == this.builderType)));

        public override int GetHashCode() => 
            this.BuilderType?.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public Type BuilderType =>
            this.builderType;
    }
}


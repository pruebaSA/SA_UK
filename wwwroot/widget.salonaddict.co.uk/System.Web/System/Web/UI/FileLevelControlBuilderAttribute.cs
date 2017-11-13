namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class FileLevelControlBuilderAttribute : Attribute
    {
        private Type builderType;
        public static readonly FileLevelControlBuilderAttribute Default = new FileLevelControlBuilderAttribute(null);

        public FileLevelControlBuilderAttribute(Type builderType)
        {
            this.builderType = builderType;
        }

        public override bool Equals(object obj) => 
            ((obj == this) || (((obj != null) && (obj is FileLevelControlBuilderAttribute)) && (((FileLevelControlBuilderAttribute) obj).BuilderType == this.builderType)));

        public override int GetHashCode() => 
            this.builderType.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public Type BuilderType =>
            this.builderType;
    }
}


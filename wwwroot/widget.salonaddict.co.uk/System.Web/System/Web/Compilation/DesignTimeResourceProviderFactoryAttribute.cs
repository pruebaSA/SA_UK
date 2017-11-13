namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DesignTimeResourceProviderFactoryAttribute : Attribute
    {
        private string _factoryTypeName;

        public DesignTimeResourceProviderFactoryAttribute(string factoryTypeName)
        {
            this._factoryTypeName = factoryTypeName;
        }

        public DesignTimeResourceProviderFactoryAttribute(Type factoryType)
        {
            this._factoryTypeName = factoryType.AssemblyQualifiedName;
        }

        public override bool IsDefaultAttribute() => 
            (this._factoryTypeName == null);

        public string FactoryTypeName =>
            this._factoryTypeName;
    }
}


namespace System.Web.Script.Serialization
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SimpleTypeResolver : JavaScriptTypeResolver
    {
        public override Type ResolveType(string id) => 
            Type.GetType(id);

        public override string ResolveTypeId(Type type) => 
            type?.AssemblyQualifiedName;
    }
}


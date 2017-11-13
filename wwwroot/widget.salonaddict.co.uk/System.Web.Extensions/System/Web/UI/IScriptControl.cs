namespace System.Web.UI
{
    using System.Collections.Generic;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IScriptControl
    {
        IEnumerable<ScriptDescriptor> GetScriptDescriptors();
        IEnumerable<ScriptReference> GetScriptReferences();
    }
}


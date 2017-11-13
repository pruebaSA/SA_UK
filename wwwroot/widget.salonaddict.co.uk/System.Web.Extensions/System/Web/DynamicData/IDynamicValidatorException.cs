namespace System.Web.DynamicData
{
    using System.Collections.Generic;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IDynamicValidatorException
    {
        IDictionary<string, Exception> InnerExceptions { get; }
    }
}


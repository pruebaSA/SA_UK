namespace System.Web.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IWcfReferenceReceiveContextInformation
    {
        void ReceiveImportContextInformation(IDictionary<string, byte[]> serviceReferenceExtensionFileContents, IServiceProvider serviceProvider);
    }
}


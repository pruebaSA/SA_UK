namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [ToolboxItem(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class UnauthorizedWebPart : ProxyWebPart
    {
        public UnauthorizedWebPart(WebPart webPart) : base(webPart)
        {
        }

        public UnauthorizedWebPart(string originalID, string originalTypeName, string originalPath, string genericWebPartID) : base(originalID, originalTypeName, originalPath, genericWebPartID)
        {
        }
    }
}


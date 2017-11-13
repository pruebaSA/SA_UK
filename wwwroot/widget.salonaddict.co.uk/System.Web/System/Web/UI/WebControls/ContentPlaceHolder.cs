namespace System.Web.UI.WebControls
{
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ToolboxItemFilter("System.Web.UI"), Designer("System.Web.UI.Design.WebControls.ContentPlaceHolderDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxData("<{0}:ContentPlaceHolder runat=\"server\"></{0}:ContentPlaceHolder>"), ControlBuilder(typeof(ContentPlaceHolderBuilder)), ToolboxItemFilter("Microsoft.VisualStudio.Web.WebForms.MasterPageWebFormDesigner", ToolboxItemFilterType.Require), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ContentPlaceHolder : Control, INonBindingContainer, INamingContainer
    {
    }
}


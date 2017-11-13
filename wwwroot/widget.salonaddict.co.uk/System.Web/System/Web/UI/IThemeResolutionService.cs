namespace System.Web.UI
{
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IThemeResolutionService
    {
        ThemeProvider[] GetAllThemeProviders();
        ThemeProvider GetStylesheetThemeProvider();
        ThemeProvider GetThemeProvider();
    }
}


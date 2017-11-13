namespace System.Web.UI.WebControls
{
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ToolboxItem(false), Bindable(false), ControlBuilder(typeof(WizardStepControlBuilder)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WizardStep : WizardStepBase
    {
    }
}


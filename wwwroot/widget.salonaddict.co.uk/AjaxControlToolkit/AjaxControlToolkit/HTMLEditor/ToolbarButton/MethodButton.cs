namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.MethodButton", "HTMLEditor.Toolbar_buttons.MethodButton.js"), PersistChildren(false), ParseChildren(true)]
    public class MethodButton : DesignModeImageButton
    {
    }
}


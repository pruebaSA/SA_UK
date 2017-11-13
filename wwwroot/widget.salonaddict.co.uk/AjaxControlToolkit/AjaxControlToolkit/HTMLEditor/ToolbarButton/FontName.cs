namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.FontName", "HTMLEditor.Toolbar_buttons.FontName.js"), ToolboxItem(false), ParseChildren(true), PersistChildren(false)]
    public class FontName : DesignModeSelectButton
    {
    }
}


namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.ForeColorSelector", "HTMLEditor.Toolbar_buttons.ForeColorSelector.js"), ToolboxItem(false), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts))]
    public class ForeColorSelector : ColorSelector
    {
    }
}


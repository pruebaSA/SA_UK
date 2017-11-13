namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System.ComponentModel;
    using System.Web.UI;

    [ParseChildren(true), PersistChildren(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.BackColorSelector", "HTMLEditor.Toolbar_buttons.BackColorSelector.js"), ToolboxItem(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class BackColorSelector : ColorSelector
    {
    }
}


namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.EditorToggleButton", "HTMLEditor.Toolbar_buttons.EditorToggleButton.js"), ParseChildren(true), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts))]
    public abstract class EditorToggleButton : DesignModeImageButton
    {
        protected EditorToggleButton()
        {
        }
    }
}


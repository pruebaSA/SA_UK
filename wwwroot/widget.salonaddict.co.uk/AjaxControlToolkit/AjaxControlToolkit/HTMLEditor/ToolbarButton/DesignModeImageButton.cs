namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.DesignModeImageButton", "HTMLEditor.Toolbar_buttons.DesignModeImageButton.js"), PersistChildren(false), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts))]
    public abstract class DesignModeImageButton : ImageButton
    {
        protected DesignModeImageButton()
        {
            base.ActiveModes.Add(ActiveModeType.Design);
        }
    }
}


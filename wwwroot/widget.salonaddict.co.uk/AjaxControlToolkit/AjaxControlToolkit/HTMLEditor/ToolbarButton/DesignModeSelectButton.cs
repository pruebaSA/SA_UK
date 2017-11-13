namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.Web.UI;

    [PersistChildren(false), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.DesignModeSelectButton", "HTMLEditor.Toolbar_buttons.DesignModeSelectButton.js"), RequiredScript(typeof(CommonToolkitScripts))]
    public abstract class DesignModeSelectButton : SelectButton
    {
        protected DesignModeSelectButton()
        {
            base.ActiveModes.Add(ActiveModeType.Design);
        }
    }
}


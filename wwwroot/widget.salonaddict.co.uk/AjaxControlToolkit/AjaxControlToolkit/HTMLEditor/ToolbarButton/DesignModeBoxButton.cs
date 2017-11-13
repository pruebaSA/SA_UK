namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.DesignModeBoxButton", "HTMLEditor.Toolbar_buttons.DesignModeBoxButton.js"), ParseChildren(true), PersistChildren(false), ToolboxItem(false), RequiredScript(typeof(CommonToolkitScripts))]
    public class DesignModeBoxButton : BoxButton
    {
        public DesignModeBoxButton()
        {
            base.ActiveModes.Add(ActiveModeType.Design);
        }
    }
}


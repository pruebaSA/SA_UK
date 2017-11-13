namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.FontSize", "HTMLEditor.Toolbar_buttons.FontSize.js"), ToolboxItem(false), RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true), PersistChildren(false)]
    public class FontSize : DesignModeSelectButton
    {
        [DefaultValue("70px"), Category("Appearance")]
        public override string SelectWidth =>
            "70px";
    }
}


namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.DesignMode", "HTMLEditor.Toolbar_buttons.DesignMode.js"), ToolboxItem(false), ParseChildren(true)]
    public class DesignMode : ModeButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_design");
            base.ActiveMode = ActiveModeType.Design;
            base.OnPreRender(e);
        }
    }
}


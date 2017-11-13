namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.ForeColor", "HTMLEditor.Toolbar_buttons.ForeColor.js"), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ToolboxItem(false), ParseChildren(true)]
    public class ForeColor : ColorButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_color_fg");
            base.OnPreRender(e);
        }
    }
}


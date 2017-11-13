namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true), ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.FixedForeColor", "HTMLEditor.Toolbar_buttons.FixedForeColor.js")]
    public class FixedForeColor : FixedColorButton
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.MethodButton = new MethodButton();
            base.MethodButton.CssClass = "";
            base.DefaultColor = "#FF0000";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.MethodButton.InternalRegisterButtonImages("ed_forecolor");
            base.OnPreRender(e);
        }
    }
}


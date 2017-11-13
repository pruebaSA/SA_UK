namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.FixedBackColor", "HTMLEditor.Toolbar_buttons.FixedBackColor.js"), ToolboxItem(false)]
    public class FixedBackColor : FixedColorButton
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.MethodButton = new MethodButton();
            base.MethodButton.CssClass = "";
            base.DefaultColor = "#FFFF00";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.MethodButton.InternalRegisterButtonImages("ed_backcolor");
            base.OnPreRender(e);
        }
    }
}


namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor.Popups;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), ParseChildren(true), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.InsertLink", "HTMLEditor.Toolbar_buttons.InsertLink.js")]
    public class InsertLink : OkCancelPopupButton
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.RelatedPopup = new LinkProperties();
            base.AutoClose = false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_link");
            base.OnPreRender(e);
        }
    }
}


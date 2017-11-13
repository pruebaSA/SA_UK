namespace AjaxControlToolkit.HTMLEditor
{
    using AjaxControlToolkit;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.DesignPanel", "HTMLEditor.DesignPanel.js"), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(DesignPanelEventHandler)), RequiredScript(typeof(ExecCommandEmulation)), ParseChildren(true), RequiredScript(typeof(AjaxControlToolkit.HTMLEditor.HTMLEditor))]
    internal class DesignPanel : ModePanel
    {
        public DesignPanel() : base(HtmlTextWriterTag.Iframe)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.Attributes.Add("name", this.ClientID);
            base.Attributes.Add("marginheight", "0");
            base.Attributes.Add("marginwidth", "0");
            base.Attributes.Add("frameborder", "0");
            if (EditPanel.IE(this.Page))
            {
                base.Attributes.Add("src", "javascript:false;");
            }
            base.Style.Add(HtmlTextWriterStyle.BorderWidth, Unit.Pixel(0).ToString());
        }
    }
}


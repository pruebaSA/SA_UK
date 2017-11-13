namespace AjaxControlToolkit.HTMLEditor
{
    using AjaxControlToolkit;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [PersistChildren(false), ParseChildren(true), RequiredScript(typeof(AjaxControlToolkit.HTMLEditor.HTMLEditor)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.PreviewPanel", "HTMLEditor.PreviewPanel.js"), RequiredScript(typeof(CommonToolkitScripts))]
    internal class PreviewPanel : ModePanel
    {
        public PreviewPanel() : base(HtmlTextWriterTag.Iframe)
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


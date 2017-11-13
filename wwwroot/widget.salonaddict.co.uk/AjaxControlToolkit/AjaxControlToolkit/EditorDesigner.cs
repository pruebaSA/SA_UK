namespace AjaxControlToolkit
{
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.Design;

    public class EditorDesigner : DesignerWithMapPath
    {
        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            StringBuilder sb = new StringBuilder(0x400);
            StringWriter writer = new StringWriter(sb, CultureInfo.InvariantCulture);
            HtmlTextWriter writer2 = new HtmlTextWriter(writer);
            writer2.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
            writer2.AddAttribute(HtmlTextWriterAttribute.Href, this.Editor.Page.ClientScript.GetWebResourceUrl(typeof(AjaxControlToolkit.HTMLEditor.Editor), "HTMLEditor.Editor.css"));
            writer2.RenderBeginTag(HtmlTextWriterTag.Link);
            writer2.RenderEndTag();
            this.Editor.CreateChilds(this);
            this.Editor.RenderControl(writer2);
            return sb.ToString();
        }

        private AjaxControlToolkit.HTMLEditor.Editor Editor =>
            ((AjaxControlToolkit.HTMLEditor.Editor) base.Component);
    }
}


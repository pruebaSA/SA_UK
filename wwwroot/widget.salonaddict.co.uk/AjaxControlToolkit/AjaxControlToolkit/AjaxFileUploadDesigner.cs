namespace AjaxControlToolkit
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.Design;

    public class AjaxFileUploadDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            StringBuilder sb = new StringBuilder(0x400);
            StringWriter writer = new StringWriter(sb, CultureInfo.InvariantCulture);
            HtmlTextWriter writer2 = new HtmlTextWriter(writer);
            this.AjaxFileUpload.CreateChilds();
            this.AjaxFileUpload.RenderControl(writer2);
            return sb.ToString();
        }

        private AjaxControlToolkit.AjaxFileUpload AjaxFileUpload =>
            ((AjaxControlToolkit.AjaxFileUpload) base.Component);
    }
}


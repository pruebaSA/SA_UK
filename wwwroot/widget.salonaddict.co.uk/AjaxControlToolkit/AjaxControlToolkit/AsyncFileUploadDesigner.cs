namespace AjaxControlToolkit
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.Design;

    public class AsyncFileUploadDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            StringBuilder sb = new StringBuilder(0x400);
            StringWriter writer = new StringWriter(sb, CultureInfo.InvariantCulture);
            HtmlTextWriter writer2 = new HtmlTextWriter(writer);
            this.AsyncFileUpload.CreateChilds();
            this.AsyncFileUpload.RenderControl(writer2);
            return sb.ToString();
        }

        private AjaxControlToolkit.AsyncFileUpload AsyncFileUpload =>
            ((AjaxControlToolkit.AsyncFileUpload) base.Component);
    }
}


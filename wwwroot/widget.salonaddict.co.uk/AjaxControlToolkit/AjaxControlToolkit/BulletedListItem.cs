namespace AjaxControlToolkit
{
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxItem(false)]
    public class BulletedListItem : WebControl
    {
        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Li;
    }
}


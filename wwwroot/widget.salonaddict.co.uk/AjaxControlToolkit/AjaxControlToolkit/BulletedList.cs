namespace AjaxControlToolkit
{
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxItem(false)]
    public class BulletedList : WebControl
    {
        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Ul;
    }
}


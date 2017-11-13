namespace IFRAME.MasterPages
{
    using AjaxControlToolkit;
    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public class Root : MasterPage
    {
        protected HtmlForm form1;
        protected ContentPlaceHolder ph;
        protected ToolkitScriptManager sm;

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}


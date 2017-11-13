namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using System;
    using System.Web.UI.WebControls;

    public class DefaultPage : IFRMAdminPage
    {
        protected Overview cntlOverview;
        protected Panel pnl;

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}


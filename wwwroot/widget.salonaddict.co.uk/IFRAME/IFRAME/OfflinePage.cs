namespace IFRAME
{
    using IFRAME.Controllers;
    using System;
    using System.Web.UI.WebControls;

    public class OfflinePage : IFRMPage
    {
        protected Panel pnl;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack && IFRMContext.Current.Salon.BookOnWidget)
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
        }
    }
}


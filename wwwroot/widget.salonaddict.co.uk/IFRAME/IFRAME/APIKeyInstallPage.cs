namespace IFRAME
{
    using IFRAME.Controllers;
    using System;
    using System.Web.UI.WebControls;

    public class APIKeyInstallPage : IFRMPage
    {
        private const string _QUERYSTING_DOMAIN_INVALID = "d";
        protected MultiView mv;
        protected Panel pnl;
        protected System.Web.UI.WebControls.View v0;
        protected System.Web.UI.WebControls.View v1;
        protected System.Web.UI.WebControls.View v2;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IFRMContext.Current.APIKey == string.Empty)
            {
                this.mv.ActiveViewIndex = 0;
            }
            else if (IFRMContext.Current.Salon == null)
            {
                this.mv.ActiveViewIndex = 1;
            }
            else if (base.Request.QueryString["d"] != null)
            {
                this.mv.ActiveViewIndex = 2;
            }
            else
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
        }
    }
}


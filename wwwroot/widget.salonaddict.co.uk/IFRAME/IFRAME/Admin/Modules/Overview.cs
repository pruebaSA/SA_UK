namespace IFRAME.Admin.Modules
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Overview : IFRMUserControl
    {
        protected Literal ltrBillingCount;
        protected Panel pnlBillingBadge;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack && (IoC.Resolve<IBillingManager>().GetReportBillableSalonNotificationCount("GBP") > 0))
            {
                this.pnlBillingBadge.Visible = true;
                this.ltrBillingCount.Text = "<b>!</b>";
            }
        }
    }
}


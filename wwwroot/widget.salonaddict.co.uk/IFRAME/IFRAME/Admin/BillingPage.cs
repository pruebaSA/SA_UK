namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class BillingPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Literal ltrInvoicingCount;
        protected Literal ltrIssuedBillsCount;
        protected Literal ltrPaymentDueCount;
        protected Literal ltrPaymentOverdueCount;
        protected Panel pnl;

        private void BindInvoicingNotification()
        {
            this.ltrInvoicingCount.Text = IoC.Resolve<IBillingManager>().GetReportBillableSalonCount("GBP").ToString();
            this.ltrIssuedBillsCount.Text = IoC.Resolve<IBillingManager>().GetSalonInvoicesIssuedCount(null, "GBP").ToString();
            this.ltrPaymentDueCount.Text = IoC.Resolve<IBillingManager>().GetReportSalonInvoiceDueCount("R", "GBP").ToString();
            this.ltrPaymentOverdueCount.Text = IoC.Resolve<IBillingManager>().GetReportSalonInvoiceOverdueCount(null, "GBP").ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindInvoicingNotification();
            }
        }
    }
}


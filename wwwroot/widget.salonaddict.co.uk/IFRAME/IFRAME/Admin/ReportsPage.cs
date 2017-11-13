namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class ReportsPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Literal ltrNonDeliveryCount;
        protected Literal ltrWidgetOfflineCount;
        protected Panel pnl;

        private void BindNonDeliveryReportDetails()
        {
            this.ltrNonDeliveryCount.Text = IoC.Resolve<IMessageManager>().SearchQueuedMessages(null, null, DateTime.Now.AddMonths(-1), DateTime.Now, 100, true, 10).Count.ToString();
        }

        private void BindWidgetOfflineReportDetails()
        {
            int totalRecords = 0;
            IoC.Resolve<IReportManager>().GetWidgetOfflineReport("GBP", 0, 100, out totalRecords);
            this.ltrWidgetOfflineCount.Text = totalRecords.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindWidgetOfflineReportDetails();
                this.BindNonDeliveryReportDetails();
            }
        }
    }
}


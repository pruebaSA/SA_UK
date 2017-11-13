namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class InvoicesPaymentDuePage : IFRMAdminPage
    {
        protected Button btnTask;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
            this.gv.Columns[4].HeaderText = base.GetLocaleResourceString("gv.Columns[4].HeaderText");
        }

        private void BindInvoices()
        {
            List<SalonInvoiceDB> reportSalonInvoiceDue = IoC.Resolve<IBillingManager>().GetReportSalonInvoiceDue("R", "GBP");
            this.gv.DataSource = reportSalonInvoiceDue;
            this.gv.DataBind();
        }

        protected void btnTask_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("invoicessettlementtask.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindInvoices();
            }
        }
    }
}


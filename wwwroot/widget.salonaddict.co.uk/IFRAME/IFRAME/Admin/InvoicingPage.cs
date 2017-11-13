namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class InvoicingPage : IFRMAdminPage
    {
        protected Button btnGenerate;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
        }

        private void BindBillableSalons()
        {
            List<BillableSalonDB> reportBillableSalons = IoC.Resolve<IBillingManager>().GetReportBillableSalons("GBP");
            this.gv.DataSource = reportBillableSalons;
            this.gv.DataBind();
            this.btnGenerate.Visible = reportBillableSalons.Count > 0;
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("invoicescreatetask.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindBillableSalons();
            }
        }
    }
}


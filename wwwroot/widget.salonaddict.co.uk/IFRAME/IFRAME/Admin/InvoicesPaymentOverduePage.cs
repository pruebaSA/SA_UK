namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class InvoicesPaymentOverduePage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected DropDownList ddlInvoiceType;
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
            string postedInvoiceType = this.PostedInvoiceType;
            this.ddlInvoiceType.SelectedValue = postedInvoiceType;
            List<SalonInvoiceDB> reportSalonInvoiceOverdue = IoC.Resolve<IBillingManager>().GetReportSalonInvoiceOverdue((postedInvoiceType == string.Empty) ? null : postedInvoiceType, "GBP");
            this.gv.DataSource = reportSalonInvoiceOverdue;
            this.gv.DataBind();
        }

        protected void ddlInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = $"{"type"}={this.ddlInvoiceType.SelectedValue.ToLower()}";
            string uRL = IFRMHelper.GetURL("invoicespaymentoverdue.aspx", new string[] { str });
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

        public string PostedInvoiceType
        {
            get
            {
                string str2;
                string str = base.Request.QueryString["type"];
                str = str ?? string.Empty;
                str = str.ToLower();
                if (((str2 = str) == null) || ((str2 != "r") && (str2 != "u")))
                {
                    return string.Empty;
                }
                return str.ToUpper();
            }
        }
    }
}


namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class ReportInactivePaymentMethodsPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
        }

        private void BindReport()
        {
            List<InactivePaymentMethodDB> reportInactivePaymentMethods = IoC.Resolve<IBillingManager>().GetReportInactivePaymentMethods("GBP");
            this.gv.DataSource = reportInactivePaymentMethods;
            this.gv.DataBind();
        }

        protected void gv_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                InactivePaymentMethodDB dataItem = (InactivePaymentMethodDB) e.Row.DataItem;
                Literal literal = e.Row.FindControl("ltrCardNumber") as Literal;
                Literal literal2 = e.Row.FindControl("ltrExpiry") as Literal;
                string str = IoC.Resolve<ISecurityManager>().DecryptUserPassword(dataItem.MaskedCardNumber, IFRAME.Controllers.Settings.Security_Key_3DES);
                string s = IoC.Resolve<ISecurityManager>().DecryptUserPassword(dataItem.CardExpirationMonth, IFRAME.Controllers.Settings.Security_Key_3DES);
                string str3 = IoC.Resolve<ISecurityManager>().DecryptUserPassword(dataItem.CardExpirationYear, IFRAME.Controllers.Settings.Security_Key_3DES);
                DateTime time = new DateTime(int.Parse(str3), int.Parse(s), 1);
                literal.Text = str;
                literal2.Text = time.ToString("MMM yyyy");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindReport();
            }
        }
    }
}


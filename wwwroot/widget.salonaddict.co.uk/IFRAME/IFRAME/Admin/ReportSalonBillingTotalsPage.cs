namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class ReportSalonBillingTotalsPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Literal ltrHeader;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
        }

        private void BindReport(Guid salonID)
        {
            BillingTotalsDB billingTotalsReport = IoC.Resolve<IReportManager>().GetBillingTotalsReport(new Guid?(salonID), "GBP");
            List<object> list = new List<object> {
                billingTotalsReport
            };
            this.gv.DataSource = list;
            this.gv.DataBind();
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            base.Response.Redirect(IFRMHelper.GetURL("reportsalonbillingtotalshome.aspx", new string[0]), true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (this.PostedSalonId == Guid.Empty)
            {
                base.Response.Redirect(IFRMHelper.GetURL("reportbillingtotals.aspx", new string[0]), true);
            }
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            if (salonById == null)
            {
                base.Response.Redirect(IFRMHelper.GetURL("reportbillingtotals.aspx", new string[0]), true);
            }
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), salonById.Name);
            if (!this.Page.IsPostBack)
            {
                this.BindReport(salonById.SalonId);
            }
        }

        public Guid PostedSalonId
        {
            get
            {
                string str = base.Request.QueryString["sid"];
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        return new Guid(str);
                    }
                    catch
                    {
                    }
                }
                return Guid.Empty;
            }
        }
    }
}


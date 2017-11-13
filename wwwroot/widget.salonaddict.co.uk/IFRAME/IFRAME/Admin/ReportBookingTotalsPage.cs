namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class ReportBookingTotalsPage : IFRMAdminPage
    {
        protected Button btnView;
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
            BookingTotalsDB bookingTotalsReport = IoC.Resolve<IReportManager>().GetBookingTotalsReport(null, "GBP");
            List<object> list = new List<object> {
                bookingTotalsReport
            };
            this.gv.DataSource = list;
            this.gv.DataBind();
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            base.Response.Redirect(IFRMHelper.GetURL("reportsalonbookingtotalshome.aspx", new string[0]), true);
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


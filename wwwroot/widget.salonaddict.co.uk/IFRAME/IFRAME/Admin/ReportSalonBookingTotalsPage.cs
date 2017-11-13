namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class ReportSalonBookingTotalsPage : IFRMAdminPage
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
            BookingTotalsDB bookingTotalsReport = IoC.Resolve<IReportManager>().GetBookingTotalsReport(new Guid?(salonID), "GBP");
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
            if (this.PostedSalonId == Guid.Empty)
            {
                base.Response.Redirect(IFRMHelper.GetURL("reportbookingtotals.aspx", new string[0]), true);
            }
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            if (salonById == null)
            {
                base.Response.Redirect(IFRMHelper.GetURL("reportbookingtotals.aspx", new string[0]), true);
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


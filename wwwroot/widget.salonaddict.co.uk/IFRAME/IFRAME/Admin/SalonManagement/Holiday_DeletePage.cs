namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    public class Holiday_DeletePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected SalonMenu cntlMenu;
        protected Literal ltrDate;
        protected Literal ltrDescription;
        protected Panel pnl;

        private void BindHolidayDetails(ClosingDayDB holiday)
        {
            this.ltrDate.Text = holiday.Date.ToString("MMMM dd yyyy");
            this.ltrDescription.Text = HttpUtility.HtmlEncode(holiday.Description);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("holidays.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Guid id = this.PostedClosingDayId;
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            ClosingDayDB closingDay = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salonById.SalonId).Single<ClosingDayDB>(item => item.ClosingDayId == id);
            IoC.Resolve<ISalonManager>().DeleteClosingDay(closingDay);
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("holidays.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (this.PostedClosingDayId == Guid.Empty)
            {
                string url = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(url, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string str3 = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(str3, true);
                }
                Guid id = this.PostedClosingDayId;
                ClosingDayDB holiday = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salonById.SalonId).Single<ClosingDayDB>(item => item.ClosingDayId == id);
                if (holiday == null)
                {
                    string str4 = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(str4, true);
                }
                this.BindHolidayDetails(holiday);
            }
        }

        public Guid PostedClosingDayId
        {
            get
            {
                string str = base.Request.QueryString["hid"];
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


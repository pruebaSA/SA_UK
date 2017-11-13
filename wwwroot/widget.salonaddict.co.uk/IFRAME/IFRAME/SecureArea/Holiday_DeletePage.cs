namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    public class Holiday_DeletePage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
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
            string uRL = IFRMHelper.GetURL("holidays.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Guid id = this.PostedSalonHolidayID;
            SalonDB salon = IFRMContext.Current.Salon;
            ClosingDayDB closingDay = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId).Single<ClosingDayDB>(item => item.ClosingDayId == id);
            IoC.Resolve<ISalonManager>().DeleteClosingDay(closingDay);
            string uRL = IFRMHelper.GetURL("holidays.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (this.PostedSalonHolidayID == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("holidays.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                Guid id = this.PostedSalonHolidayID;
                SalonDB salon = IFRMContext.Current.Salon;
                ClosingDayDB holiday = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId).Single<ClosingDayDB>(item => item.ClosingDayId == id);
                if (holiday == null)
                {
                    string uRL = IFRMHelper.GetURL("holidays.aspx", new string[0]);
                    base.Response.Redirect(uRL, true);
                }
                this.BindHolidayDetails(holiday);
            }
        }

        public Guid PostedSalonHolidayID
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
    }
}


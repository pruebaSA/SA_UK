namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class SettingsPage : IFRMAdminPage
    {
        protected CheckBox cbTicketAlerts;
        protected SalonMenu cntlMenu;
        protected DropDownList ddlOpeningHours;
        protected DropDownList ddlStatus;
        protected Literal ltrAddress;
        protected Literal ltrCompany;
        protected Literal ltrPhone;
        protected Literal ltrSalon;
        protected Panel pnl;

        private void BindSalonDetails(SalonDB salon)
        {
            this.ltrSalon.Text = salon.Name;
            this.ltrPhone.Text = salon.PhoneNumber;
            this.ltrAddress.Text = IFRMHelper.GetAddressFriendlyString(salon);
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            this.ltrCompany.Text = (salon.Company == string.Empty) ? salon.Name : salon.Company;
            this.ddlOpeningHours.SelectedValue = openingHoursBySalonId.ShowOnWidget ? "PUBLIC" : "PRIVATE";
            this.ddlStatus.SelectedValue = salon.BookOnWidget ? "PUBLIC" : "PRIVATE";
            if ((from item in IoC.Resolve<ITicketManager>().GetTicketAlertsBySalonId(salon.SalonId)
                where item.Active
                select item).Count<TicketAlertDB>() > 0)
            {
                this.cbTicketAlerts.Checked = true;
            }
        }

        protected void ddlOpeningHours_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(this.PostedSalonId);
            openingHoursBySalonId.ShowOnWidget = this.ddlOpeningHours.SelectedValue == "PUBLIC";
            IoC.Resolve<ISalonManager>().UpdateOpeningHours(openingHoursBySalonId);
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("settings.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            salonById.BookOnWidget = this.ddlStatus.SelectedValue == "PUBLIC";
            IoC.Resolve<ISalonManager>().UpdateSalon(salonById);
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("settings.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string url = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindSalonDetails(salonById);
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


namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class SettingsPage : IFRMSecurePage
    {
        protected CheckBox cbTicketAlerts;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected DropDownList ddlOpeningHours;
        protected DropDownList ddlStatus;
        protected Literal ltrAddress;
        protected Literal ltrCompany;
        protected Literal ltrPhone;
        protected Literal ltrSalon;
        protected Panel pnl;

        private void BindSalonDetails()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            this.ltrSalon.Text = salon.Name;
            this.ltrPhone.Text = salon.PhoneNumber;
            this.ltrAddress.Text = IFRMHelper.GetAddressFriendlyString(salon);
            this.ltrCompany.Text = string.IsNullOrEmpty(salon.Company) ? salon.Name : salon.Company;
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
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
            SalonDB salon = IFRMContext.Current.Salon;
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            openingHoursBySalonId.ShowOnWidget = this.ddlOpeningHours.SelectedValue == "PUBLIC";
            openingHoursBySalonId = IoC.Resolve<ISalonManager>().UpdateOpeningHours(openingHoursBySalonId);
            string uRL = IFRMHelper.GetURL("settings.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            salon.BookOnWidget = this.ddlStatus.SelectedValue == "PUBLIC";
            IoC.Resolve<ISalonManager>().UpdateSalon(salon);
            string uRL = IFRMHelper.GetURL("settings.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindSalonDetails();
            }
        }
    }
}


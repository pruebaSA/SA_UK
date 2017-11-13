namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Ticket_Alert_CreatePage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Panel pnl;
        protected TextBox txtEmail;
        protected TextBox txtName;
        protected RequiredFieldValidator valEmail;
        protected ValidatorCalloutExtender valEmailEx;
        protected RegularExpressionValidator valEmailRegEx;
        protected ValidatorCalloutExtender valEmailRegExEx;
        protected RequiredFieldValidator valName;
        protected ValidatorCalloutExtender valNameEx;

        private void BindTicketAlertDetails(SalonDB salon)
        {
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("ticket-alerts.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                TicketAlertDB alert = new TicketAlertDB {
                    Active = true,
                    ByEmail = true,
                    DisplayText = this.txtName.Text.Trim(),
                    Email = this.txtEmail.Text.Trim(),
                    SalonId = salon.SalonId
                };
                alert = IoC.Resolve<ITicketManager>().InsertTicketAlert(alert);
            }
            string uRL = IFRMHelper.GetURL("ticket-alerts.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                this.BindTicketAlertDetails(salon);
            }
        }
    }
}


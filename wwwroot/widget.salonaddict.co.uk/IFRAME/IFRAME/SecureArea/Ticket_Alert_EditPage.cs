namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Ticket_Alert_EditPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected CheckBox cbActive;
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

        private void BindTicketAlertDetails(TicketAlertDB alert)
        {
            this.txtEmail.Text = alert.Email;
            this.txtName.Text = alert.DisplayText;
            this.cbActive.Checked = alert.Active;
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
                TicketAlertDB ticketAlertById = IoC.Resolve<ITicketManager>().GetTicketAlertById(this.PostedAlertId);
                ticketAlertById.Email = this.txtEmail.Text.Trim();
                ticketAlertById.DisplayText = this.txtName.Text.Trim();
                ticketAlertById.Active = this.cbActive.Checked;
                ticketAlertById = IoC.Resolve<ITicketManager>().UpdateTicketAlert(ticketAlertById);
            }
            string uRL = IFRMHelper.GetURL("ticket-alerts.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                TicketAlertDB ticketAlertById = IoC.Resolve<ITicketManager>().GetTicketAlertById(this.PostedAlertId);
                if (ticketAlertById == null)
                {
                    this.btnCancel_Click(this, new EventArgs());
                }
                else if (ticketAlertById.SalonId != salon.SalonId)
                {
                    this.btnCancel_Click(this, new EventArgs());
                }
                else
                {
                    this.BindTicketAlertDetails(ticketAlertById);
                }
            }
        }

        public Guid PostedAlertId
        {
            get
            {
                string str = base.Request.QueryString["aid"];
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


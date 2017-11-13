namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Ticket_Alert_DeletePage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Literal ltrEmail;
        protected Literal ltrName;
        protected Panel pnl;

        private void BindTicketAlertDetails(TicketAlertDB alert)
        {
            this.ltrEmail.Text = alert.Email;
            this.ltrName.Text = alert.DisplayText;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("ticket-alerts.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                TicketAlertDB ticketAlertById = IoC.Resolve<ITicketManager>().GetTicketAlertById(this.PostedAlertId);
                IoC.Resolve<ITicketManager>().DeleteTicketAlert(ticketAlertById);
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


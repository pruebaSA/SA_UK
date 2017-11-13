namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Ticket_Alert_DeletePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected SalonMenu cntlMenu;
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
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("ticket-alerts.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                TicketAlertDB ticketAlertById = IoC.Resolve<ITicketManager>().GetTicketAlertById(this.PostedAlertId);
                IoC.Resolve<ITicketManager>().DeleteTicketAlert(ticketAlertById);
            }
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("ticket-alerts.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                if (this.PostedSalonId == Guid.Empty)
                {
                    string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(uRL, true);
                }
                else
                {
                    SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                    if (salonById == null)
                    {
                        string url = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                        base.Response.Redirect(url, true);
                    }
                    else
                    {
                        TicketAlertDB ticketAlertById = IoC.Resolve<ITicketManager>().GetTicketAlertById(this.PostedAlertId);
                        if (ticketAlertById == null)
                        {
                            this.btnCancel_Click(this, new EventArgs());
                        }
                        else if (ticketAlertById.SalonId != salonById.SalonId)
                        {
                            this.btnCancel_Click(this, new EventArgs());
                        }
                        else
                        {
                            this.BindTicketAlertDetails(ticketAlertById);
                        }
                    }
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


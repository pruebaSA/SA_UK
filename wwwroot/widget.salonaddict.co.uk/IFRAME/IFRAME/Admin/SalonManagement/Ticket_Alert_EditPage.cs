namespace IFRAME.Admin.SalonManagement
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Ticket_Alert_EditPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected CheckBox cbActive;
        protected SalonMenu cntlMenu;
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
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("ticket-alerts.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                TicketAlertDB ticketAlertById = IoC.Resolve<ITicketManager>().GetTicketAlertById(this.PostedAlertId);
                ticketAlertById.Email = this.txtEmail.Text.Trim();
                ticketAlertById.DisplayText = this.txtName.Text.Trim();
                ticketAlertById.Active = this.cbActive.Checked;
                ticketAlertById = IoC.Resolve<ITicketManager>().UpdateTicketAlert(ticketAlertById);
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


namespace IFRAME.Admin.SalonManagement
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Ticket_Alert_CreatePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
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

        private void BindTicketAlertDetails(SalonDB salon)
        {
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
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                TicketAlertDB alert = new TicketAlertDB {
                    Active = true,
                    ByEmail = true,
                    DisplayText = this.txtName.Text.Trim(),
                    Email = this.txtEmail.Text.Trim(),
                    SalonId = salonById.SalonId
                };
                alert = IoC.Resolve<ITicketManager>().InsertTicketAlert(alert);
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
                        this.BindTicketAlertDetails(salonById);
                    }
                }
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


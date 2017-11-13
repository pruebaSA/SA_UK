namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class Ticket_AlertsPage : IFRMAdminPage
    {
        protected Button btnAdd;
        protected SalonMenu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void BindTicketAlerts(SalonDB salon)
        {
            List<TicketAlertDB> ticketAlertsBySalonId = IoC.Resolve<ITicketManager>().GetTicketAlertsBySalonId(salon.SalonId);
            this.gv.DataSource = ticketAlertsBySalonId;
            this.gv.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("ticket-alert-create.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"aid"}={guid}";
            string uRL = IFRMHelper.GetURL("ticket-alert-delete.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"aid"}={guid}";
            string uRL = IFRMHelper.GetURL("ticket-alert-edit.aspx", new string[] { str, str2 });
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
                        this.BindTicketAlerts(salonById);
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


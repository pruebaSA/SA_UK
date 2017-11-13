namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class Ticket_AlertsPage : IFRMSecurePage
    {
        protected Button btnAdd;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
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
            string uRL = IFRMHelper.GetURL("ticket-alert-create.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"aid"}={guid}";
            string uRL = IFRMHelper.GetURL("ticket-alert-delete.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"aid"}={guid}";
            string uRL = IFRMHelper.GetURL("ticket-alert-edit.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                this.BindTicketAlerts(salon);
            }
        }
    }
}


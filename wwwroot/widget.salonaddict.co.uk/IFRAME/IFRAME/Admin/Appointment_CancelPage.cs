namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Appointment_CancelPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected SalonMenu cntlMenu;
        protected Literal ltrClient;
        protected Literal ltrConfirmationNo;
        protected Literal ltrDate;
        protected Literal ltrEmployee;
        protected Literal ltrService;
        protected Literal ltrTime;
        protected Panel pnl;

        private void ApplyLocalization()
        {
        }

        private void BindTicketDetails(TicketSummaryDB ticket, SalonDB salon)
        {
            TicketRowDB wdb = IoC.Resolve<ITicketManager>().GetTicketRowsByTicketId(ticket.TicketId).First<TicketRowDB>();
            this.ltrConfirmationNo.Text = ticket.TicketNumber;
            this.ltrClient.Text = ticket.CustomerDisplayText;
            this.ltrDate.Text = wdb.StartDate.Value.ToString("ddd dd MMM yyyy");
            this.ltrTime.Text = new DateTime(wdb.StartTime.Value.Ticks).ToString("HH:mm");
            this.ltrService.Text = wdb.ServiceDisplayText;
            this.ltrEmployee.Text = string.IsNullOrEmpty(wdb.EmployeeDisplayText) ? base.GetLocaleResourceString("Employee.Any") : wdb.EmployeeDisplayText;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"tid"}={this.PostedTicketId}";
            string uRL = IFRMHelper.GetURL("appointment-details.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            TicketSummaryDB ticketById = IoC.Resolve<ITicketManager>().GetTicketById(this.PostedTicketId);
            IoC.Resolve<ITicketManager>().GetTicketRowsByTicketId(ticketById.TicketId).First<TicketRowDB>();
            ticketById.CancelledOnUtc = new DateTime?(DateTime.UtcNow);
            if (ticketById.TicketStatusTypeEnum <= TicketStatusTypeEnum.Open)
            {
                ticketById.TicketStatusType = 3;
            }
            if (!ticketById.OpenedOnUtc.HasValue)
            {
                ticketById.OpenedOnUtc = new DateTime?(DateTime.UtcNow);
            }
            ticketById = IoC.Resolve<ITicketManager>().UpdateTicket(ticketById);
            IoC.Resolve<ISchedulingManager>().DeleteTimeBlocksByTicket(ticketById);
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("appointments.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                if (this.PostedSalonId == Guid.Empty)
                {
                    string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(uRL, true);
                }
                if (this.PostedTicketId == Guid.Empty)
                {
                    string str2 = $"{"sid"}={this.PostedSalonId}";
                    string url = IFRMHelper.GetURL("appointments.aspx", new string[] { str2 });
                    base.Response.Redirect(url, true);
                }
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string str4 = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(str4, true);
                }
                TicketSummaryDB ticketById = IoC.Resolve<ITicketManager>().GetTicketById(this.PostedTicketId);
                if (ticketById == null)
                {
                    string str5 = $"{"sid"}={this.PostedSalonId}";
                    string str6 = IFRMHelper.GetURL("appointments.aspx", new string[] { str5 });
                    base.Response.Redirect(str6, true);
                }
                this.BindTicketDetails(ticketById, salonById);
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

        public Guid PostedTicketId
        {
            get
            {
                string str = base.Request.QueryString["tid"];
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


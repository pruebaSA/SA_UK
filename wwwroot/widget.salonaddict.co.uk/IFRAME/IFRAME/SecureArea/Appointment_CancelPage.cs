namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Appointment_CancelPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
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

        private void BindTicketDetails(TicketSummaryDB ticket)
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
            string str = $"{"tid"}={this.PostedTicketId}";
            string uRL = IFRMHelper.GetURL("appointment-details.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            TicketSummaryDB ticketById = IoC.Resolve<ITicketManager>().GetTicketById(this.PostedTicketId);
            IoC.Resolve<ITicketManager>().GetTicketRowsByTicketId(ticketById.TicketId).First<TicketRowDB>();
            ticketById.CancelledOnUtc = new DateTime?(DateTime.UtcNow);
            if (ticketById.TicketStatusTypeEnum <= TicketStatusTypeEnum.Open)
            {
                ticketById.TicketStatusType = 3;
            }
            ticketById = IoC.Resolve<ITicketManager>().UpdateTicket(ticketById);
            IoC.Resolve<ISchedulingManager>().DeleteTimeBlocksByTicket(ticketById);
            string uRL = IFRMHelper.GetURL("appointments.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                if (this.PostedTicketId == Guid.Empty)
                {
                    string uRL = IFRMHelper.GetURL("appointments.aspx", new string[0]);
                    base.Response.Redirect(uRL, true);
                }
                TicketSummaryDB ticketById = IoC.Resolve<ITicketManager>().GetTicketById(this.PostedTicketId);
                if ((ticketById == null) || (ticketById.SalonId != salon.SalonId))
                {
                    string url = IFRMHelper.GetURL("appointments.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindTicketDetails(ticketById);
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


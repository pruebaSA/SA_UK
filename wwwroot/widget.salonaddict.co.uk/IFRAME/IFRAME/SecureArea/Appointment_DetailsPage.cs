namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    public class Appointment_DetailsPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Literal ltrClient;
        protected Literal ltrConfirmationNo;
        protected Literal ltrDate;
        protected Literal ltrEmail;
        protected Literal ltrEmployee;
        protected Literal ltrFirstTimeAtSalon;
        protected Literal ltrPhone;
        protected Literal ltrService;
        protected Literal ltrSpecialRequest;
        protected Literal ltrStatus;
        protected Literal ltrTime;
        protected Panel pnl;
        protected TabPanel t1;
        protected TabPanel t2;
        protected TabContainer tc;

        private void ApplyLocalization()
        {
            this.tc.Tabs[0].HeaderText = base.GetLocaleResourceString("tc.Tabs[0].HeaderText");
            this.tc.Tabs[1].HeaderText = base.GetLocaleResourceString("tc.Tabs[1].HeaderText");
        }

        private void BindTicketDetails(TicketSummaryDB order, TicketRowDB appointment, SalonDB salon)
        {
            this.ltrConfirmationNo.Text = order.TicketNumber;
            this.ltrClient.Text = order.CustomerDisplayText;
            this.ltrPhone.Text = order.CustomerPhone;
            if (order.CustomerPhone == string.Empty)
            {
                this.ltrPhone.Text = order.CustomerMobile;
            }
            this.ltrEmail.Text = order.CustomerEmail;
            this.ltrDate.Text = appointment.StartDate.Value.ToString("ddd dd MMM yyyy");
            this.ltrTime.Text = new DateTime(appointment.StartTime.Value.Ticks).ToString("HH:mm");
            this.ltrService.Text = appointment.ServiceDisplayText;
            this.ltrEmployee.Text = string.IsNullOrEmpty(appointment.EmployeeDisplayText) ? base.GetLocaleResourceString("Employee.Any") : appointment.EmployeeDisplayText;
            this.ltrStatus.Text = (order.TicketStatusTypeEnum == TicketStatusTypeEnum.Open) ? "Open" : "Closed";
            this.ltrFirstTimeAtSalon.Text = this.ltrFirstTimeAtSalon.Text = base.GetLocaleResourceString("ltrYes.Text");
            if (order.RepeatCustomer.HasValue && order.RepeatCustomer.Value)
            {
                this.ltrFirstTimeAtSalon.Text = base.GetLocaleResourceString("ltrNo.Text");
            }
            this.ltrSpecialRequest.Text = HttpUtility.HtmlEncode(order.CustomerSpecialRequest.Replace("\n", "").Replace("\r", ""));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"tid"}={this.PostedTicketId}";
            string uRL = IFRMHelper.GetURL("appointment-cancel.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        private void MarkAppointmentAsRead(TicketSummaryDB ticket)
        {
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            if (!ticket.OpenedOnUtc.HasValue)
            {
                ticket.OpenedOnUtc = new DateTime?(DateTime.UtcNow);
            }
            if (ticket.TicketStatusTypeEnum == TicketStatusTypeEnum.None)
            {
                ticket.TicketStatusType = 1;
            }
            if (workingUser != null)
            {
                ticket.OpenUserDisplayText = workingUser.DisplayText;
                ticket.OpenUserId = new Guid?(workingUser.UserId);
            }
            ticket = IoC.Resolve<ITicketManager>().UpdateTicket(ticket);
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
                if (ticketById.SalonId != salon.SalonId)
                {
                    string url = IFRMHelper.GetURL("appointments.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                if ((ticketById.TicketStatusTypeEnum != TicketStatusTypeEnum.None) && (ticketById.TicketStatusTypeEnum != TicketStatusTypeEnum.Open))
                {
                    string str3 = IFRMHelper.GetURL("appointments.aspx", new string[0]);
                    base.Response.Redirect(str3, true);
                }
                TicketRowDB appointment = IoC.Resolve<ITicketManager>().GetTicketRowsByTicketId(ticketById.TicketId).First<TicketRowDB>();
                this.BindTicketDetails(ticketById, appointment, salon);
                this.MarkAppointmentAsRead(ticketById);
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


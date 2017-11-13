namespace IFRAME
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Web.UI.WebControls;

    public class ConfirmationPage : IFRMPage
    {
        protected Label lblDate;
        protected Label lblEmployee;
        protected Label lblService;
        protected Literal ltrAddress;
        protected Literal ltrHeader;
        protected Panel mini;
        protected Panel pnl;

        private void BindAppointmentDetails(TicketRowDB appointment)
        {
            this.lblEmployee.Text = string.Format(base.GetLocaleResourceString("lblEmployee.Text"), appointment.EmployeeDisplayText);
            this.lblService.Text = appointment.ServiceDisplayText;
            this.lblDate.Text = string.Format(base.GetLocaleResourceString("lblDate.Text"), appointment.StartDate.Value.ToString("ddd, dd MMM yyyy"), new DateTime(appointment.StartTime.Value.Ticks).ToString("HH:mm"));
        }

        private void BindSalonDetails()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), salon.Name);
            this.ltrAddress.Text = string.Format(base.GetLocaleResourceString("ltrAddress.Text"), IFRMHelper.GetAddressFriendlyString(salon));
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedOrderID == Guid.Empty)
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            SalonDB salon = IFRMContext.Current.Salon;
            TicketSummaryDB ticketById = IoC.Resolve<ITicketManager>().GetTicketById(this.PostedOrderID);
            if ((ticketById == null) && !ticketById.Deleted)
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            if (ticketById.SalonId != salon.SalonId)
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            TicketRowDB appointment = IoC.Resolve<ITicketManager>().GetTicketRowsByTicketId(ticketById.TicketId).FirstOrDefault<TicketRowDB>();
            if (appointment.StartDate.Value < DateTime.UtcNow)
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            if (!ticketById.Confirmed)
            {
                TaskInfo state = new TaskInfo {
                    APIKey = IFRMContext.Current.APIKey,
                    Appointment = appointment,
                    Order = ticketById,
                    Salon = salon,
                    IsLoopBack = base.Request.Url.IsLoopback
                };
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendNotifications), state);
            }
            this.BindSalonDetails();
            this.BindAppointmentDetails(appointment);
        }

        private void SendNotifications(object objectState)
        {
            TaskInfo info = (TaskInfo) objectState;
            TicketSummaryDB order = info.Order;
            TicketRowDB appointment = info.Appointment;
            SalonDB salon = info.Salon;
            bool firstTimeAtSalon = order.RepeatCustomer != true;
            string specialRequest = order.CustomerSpecialRequest.Replace("\r", string.Empty).Replace("\n", "");
            IFRMHelper.SendCusotmerOrderEmail(order, appointment, salon, specialRequest, info.IsLoopBack);
            IFRMHelper.SendSalonOrderEmail(order, appointment, salon, firstTimeAtSalon, specialRequest, info.IsLoopBack);
            WidgetApiKeyDB widgetApiKeyByVerificationToken = IoC.Resolve<IUserManager>().GetWidgetApiKeyByVerificationToken(info.APIKey);
            IFRMHelper.SendSalesOrderEmail(order, appointment, salon, specialRequest, widgetApiKeyByVerificationToken.HttpReferer, info.IsLoopBack);
            order.Confirmed = true;
            order = IoC.Resolve<ITicketManager>().UpdateTicket(order);
        }

        public Guid PostedOrderID
        {
            get
            {
                string str = base.Request.QueryString["oid"];
                if (string.IsNullOrEmpty(str))
                {
                    return Guid.Empty;
                }
                str = str.Trim();
                try
                {
                    return new Guid(str);
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }

        internal class TaskInfo
        {
            public string APIKey { get; set; }

            public TicketRowDB Appointment { get; set; }

            public bool IsLoopBack { get; set; }

            public TicketSummaryDB Order { get; set; }

            public SalonDB Salon { get; set; }
        }
    }
}


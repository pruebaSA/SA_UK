namespace IFRAME.Admin.SalonManagement
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    public class Appointment_DetailsPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected SalonMenu cntlMenu;
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

        private void BindTicketDetails(TicketSummaryDB ticket, SalonDB salon)
        {
            TicketRowDB wdb = IoC.Resolve<ITicketManager>().GetTicketRowsByTicketId(ticket.TicketId).First<TicketRowDB>();
            this.ltrConfirmationNo.Text = ticket.TicketNumber;
            this.ltrClient.Text = ticket.CustomerDisplayText;
            this.ltrPhone.Text = ticket.CustomerPhone;
            if (ticket.CustomerPhone == string.Empty)
            {
                this.ltrPhone.Text = ticket.CustomerMobile;
            }
            this.ltrEmail.Text = ticket.CustomerEmail;
            this.ltrDate.Text = wdb.StartDate.Value.ToString("ddd dd MMM yyyy");
            this.ltrTime.Text = new DateTime(wdb.StartTime.Value.Ticks).ToString("HH:mm");
            this.ltrService.Text = wdb.ServiceDisplayText;
            this.ltrEmployee.Text = string.IsNullOrEmpty(wdb.EmployeeDisplayText) ? base.GetLocaleResourceString("Employee.Any") : wdb.EmployeeDisplayText;
            this.ltrStatus.Text = (ticket.TicketStatusTypeEnum == TicketStatusTypeEnum.Open) ? "Open" : "Closed";
            this.ltrFirstTimeAtSalon.Text = this.ltrFirstTimeAtSalon.Text = base.GetLocaleResourceString("ltrYes.Text");
            if (ticket.RepeatCustomer.HasValue && ticket.RepeatCustomer.Value)
            {
                this.ltrFirstTimeAtSalon.Text = base.GetLocaleResourceString("ltrNo.Text");
            }
            this.ltrSpecialRequest.Text = HttpUtility.HtmlEncode(ticket.CustomerSpecialRequest.Replace("\n", "").Replace("\r", ""));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            TicketSummaryDB ticketById = IoC.Resolve<ITicketManager>().GetTicketById(this.PostedTicketId);
            string str = $"{"sid"}={ticketById.SalonId}";
            string str2 = $"{"tid"}={ticketById.TicketId}";
            string uRL = IFRMHelper.GetURL("appointment-cancel.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                if (this.PostedTicketId == Guid.Empty)
                {
                    string str = $"{"sid"}={this.PostedSalonId}";
                    string uRL = IFRMHelper.GetURL("appointments.aspx", new string[] { str });
                    base.Response.Redirect(uRL, true);
                }
                if (this.PostedSalonId == Guid.Empty)
                {
                    string url = IFRMHelper.GetURL("appointments.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                TicketSummaryDB ticketById = IoC.Resolve<ITicketManager>().GetTicketById(this.PostedTicketId);
                if (ticketById == null)
                {
                    string str4 = $"{"sid"}={this.PostedSalonId}";
                    string str5 = IFRMHelper.GetURL("appointments.aspx", new string[] { str4 });
                    base.Response.Redirect(str5, true);
                }
                if (ticketById.SalonId != this.PostedSalonId)
                {
                    string str6 = IFRMHelper.GetURL("appointments.aspx", new string[0]);
                    base.Response.Redirect(str6, true);
                }
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if ((ticketById.TicketStatusTypeEnum != TicketStatusTypeEnum.None) && (ticketById.TicketStatusTypeEnum != TicketStatusTypeEnum.Open))
                {
                    string str7 = $"{"sid"}={this.PostedSalonId}";
                    string str8 = IFRMHelper.GetURL("appointments.aspx", new string[] { str7 });
                    base.Response.Redirect(str8, true);
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


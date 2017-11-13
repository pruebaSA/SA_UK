namespace IFRAME
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI.WebControls;

    public class DetailsPage : IFRMPage
    {
        protected Button btnSubmit;
        protected PhoneDropDownList ddlPhoneType;
        protected Label lblDate;
        protected Label lblEmployee;
        protected Label lblService;
        protected Literal ltrAddress;
        protected Literal ltrHeader;
        protected Accordion mv;
        protected Panel pnl;
        protected YesNoOptions rblYesNo;
        protected TextBox txtEmail;
        protected TextBox txtEmailConfirm;
        protected TextBox txtFirstName;
        protected TextBox txtLastName;
        protected TextBox txtPhone;
        protected TextBox txtSpecialRequests;
        protected AccordionPane v0;
        protected AccordionPane v1;
        protected RequiredFieldValidator valEmail;
        protected CompareValidator valEmailConfirmComp;
        protected ValidatorCalloutExtender valEmailConfirmCompEx;
        protected ValidatorCalloutExtender valEmailEx;
        protected RegularExpressionValidator valEmailRegEx;
        protected ValidatorCalloutExtender valEmailRegExEx;
        protected RequiredFieldValidator valFirstName;
        protected ValidatorCalloutExtender valFirstNameEX;
        protected RegularExpressionValidator valFirstNameRegEx1;
        protected RegularExpressionValidator valFirstNameRegEx2;
        protected ValidatorCalloutExtender valFirstNameRegEx2Ex;
        protected ValidatorCalloutExtender valFirstNameRegExEx1;
        protected RequiredFieldValidator valLastName;
        protected ValidatorCalloutExtender valLastNameEx;
        protected RegularExpressionValidator valLastNameRegEx1;
        protected ValidatorCalloutExtender valLastNameRegEx1Ex;
        protected RegularExpressionValidator valLastNameRegEx2;
        protected ValidatorCalloutExtender valLastNameRegEx2Ex;
        protected RequiredFieldValidator valPhone;
        protected ValidatorCalloutExtender valPhoneEx;
        protected RegularExpressionValidator valPhoneRegEx;
        protected ValidatorCalloutExtender valPhoneRegExEx;
        protected RequiredFieldValidator valYesNo;
        protected ValidatorCalloutExtender valYesNoEx;

        private void BindAppointmentDetails(DateTime date, DateTime time, ServiceDB service, EmployeeDB employee)
        {
            this.lblService.Text = service.Name;
            if (employee != null)
            {
                this.lblEmployee.Text = string.Format(base.GetLocaleResourceString("lblEmployee.Text"), employee.DisplayText);
            }
            this.lblDate.Text = string.Format(base.GetLocaleResourceString("lblDate.Text"), date.ToString("ddd, dd MMM yyyy"), time.ToString("HH:mm"));
        }

        private void BindSalonDetails()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), salon.Name);
            this.ltrAddress.Text = string.Format(base.GetLocaleResourceString("ltrAddress.Text"), IFRMHelper.GetAddressFriendlyString(salon));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DateTime postedDate = this.PostedDate;
            DateTime postedTime = this.PostedTime;
            SalonDB salon = IFRMContext.Current.Salon;
            List<EmployeeDB> employeesBySalonId = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salon.SalonId);
            EmployeeDB employee = employeesBySalonId.SingleOrDefault<EmployeeDB>(item => item.EmployeeId == this.PostedEmployeeId);
            if ((this.PostedEmployeeId != Guid.Empty) && (employee == null))
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            ServiceDB service = (from item in IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId)
                where item.Active
                select item).SingleOrDefault<ServiceDB>(item => item.ServiceId == this.PostedServiceId);
            if ((service == null) || (service.SalonId != salon.SalonId))
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            if (this.Page.IsValid)
            {
                if (!this.IsAppointmentAvailble(postedDate, postedTime, salon, service, employeesBySalonId))
                {
                    base.Response.Redirect(IFRMHelper.GetURL("unavailable.aspx", new string[0]), true);
                }
                int num4 = salon.TotalTicketCount + 1;
                string str = $"{salon.Abbreviation}UK{num4.ToString().PadLeft(8, '0')}";
                string str2 = this.txtFirstName.Text.Trim();
                string str3 = this.txtLastName.Text.Trim();
                string str4 = (str2 + " " + str3).Trim();
                string str5 = this.txtEmail.Text.Trim();
                string str6 = string.Empty;
                string str7 = string.Empty;
                string text = this.txtSpecialRequests.Text;
                bool flag2 = false;
                decimal price = service.Price;
                decimal num2 = price;
                decimal num3 = num2;
                if (this.rblYesNo.SelectedItem == YesNoOption.No)
                {
                    flag2 = true;
                }
                if (this.ddlPhoneType.SelectedItem == PhoneNumberType.Mobile)
                {
                    str6 = this.txtPhone.Text.Trim();
                }
                else
                {
                    str7 = this.txtPhone.Text.Trim();
                }
                TicketSummaryDB ticket = new TicketSummaryDB {
                    BillingDisplayText = str4,
                    BillingEmail = str5,
                    BillingFirstName = str2,
                    BillingLastName = str3,
                    BillingMobile = str6,
                    BillingPhone = str7,
                    BookedOnWidget = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerDisplayText = str4,
                    CustomerEmail = str5,
                    CustomerFirstName = str2,
                    CustomerIPAddress = base.Request.UserHostAddress,
                    CustomerLastName = str3,
                    CustomerMobile = str6,
                    CustomerPhone = str7,
                    CustomerSpecialRequest = text,
                    CurrencyCode = "GBP",
                    PaidDateUtc = new DateTime?(DateTime.UtcNow),
                    RepeatCustomer = new bool?(flag2),
                    SalonDisplayText = salon.Name,
                    SalonId = salon.SalonId,
                    Subtotal = num2,
                    TicketNumber = str,
                    Total = num3,
                    UserId = null
                };
                ticket = IoC.Resolve<ITicketManager>().InsertTicket(ticket);
                TicketRowDB ticketRow = new TicketRowDB();
                if (employee != null)
                {
                    ticketRow.EmployeeDisplayText = employee.DisplayText;
                    ticketRow.EmployeeId = new Guid?(employee.EmployeeId);
                }
                ticketRow.MultiDay = false;
                ticketRow.Price = price;
                ticketRow.RowOrder = 1;
                ticketRow.RowTotal = price;
                ticketRow.ServiceDisplayText = service.Name;
                ticketRow.ServiceId = new Guid?(service.ServiceId);
                ticketRow.StartDate = new DateTime?(postedDate);
                ticketRow.StartTime = new TimeSpan?(postedTime.TimeOfDay);
                ticketRow.TicketId = ticket.TicketId;
                ticketRow.TicketRowType = 1;
                ticketRow = IoC.Resolve<ITicketManager>().InsertTicketRow(ticketRow);
                salon.TotalTicketCount++;
                salon = IoC.Resolve<ISalonManager>().UpdateSalon(salon);
                TimeBlockDB block = new TimeBlockDB {
                    BlockTypeId = 2,
                    CycleLength = 1,
                    CyclePeriodType = 10,
                    EndDateUtc = new DateTime?(postedDate),
                    EndTime = postedTime.TimeOfDay,
                    SalonId = salon.SalonId,
                    Slots = -1,
                    StartDateUtc = postedDate,
                    StartTime = postedTime.TimeOfDay,
                    TicketId = new Guid?(ticket.TicketId)
                };
                IoC.Resolve<ISchedulingManager>().InsertTimeBlock(block);
                string str9 = $"{"oid"}={ticket.TicketId}";
                string uRL = IFRMHelper.GetURL("~/confirmation.aspx", new string[] { str9 });
                base.Response.Redirect(uRL, true);
            }
            this.ddlPhoneType.DataBind();
            this.rblYesNo.DataBind();
            this.BindSalonDetails();
            this.BindAppointmentDetails(postedDate, postedTime, service, employee);
        }

        private bool IsAppointmentAvailble(DateTime date, DateTime time, SalonDB salon, ServiceDB service, List<EmployeeDB> employees)
        {
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            List<ClosingDayDB> closingDaysBySalonId = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId);
            if (IFRMHelper.IsSalonClosed(date, openingHoursBySalonId, closingDaysBySalonId))
            {
                return false;
            }
            List<ScheduleDB> list2 = (from item in IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salon.SalonId)
                where item.ScheduleTypeEnum == ScheduleTypeEnum.TimeSlot
                select item).ToList<ScheduleDB>();
            List<TimeSlotUI> timeslots = new List<TimeSlotUI>();
            list2.ForEach(delegate (ScheduleDB item) {
                TimeSlotUI tui = new TimeSlotUI {
                    WeekDay = item.WeekDayEnum.Value,
                    Time = item.Time.Value,
                    Slots = item.Slots.Value
                };
                timeslots.Add(tui);
            });
            string timeURL = Regex.Replace(IFRMHelper.ToUrlFriendlyTime(time), @"[^\d]", string.Empty);
            TimeSlotUI timeslot = timeslots.FirstOrDefault<TimeSlotUI>(delegate (TimeSlotUI item) {
                if (item.WeekDay == date.DayOfWeek)
                {
                    DateTime time = new DateTime(item.Time.Ticks);
                    return time.ToString("HHmm") == timeURL;
                }
                return false;
            });
            if (timeslot == null)
            {
                return false;
            }
            if (this.IsBlockedByDuration(timeslot, service, openingHoursBySalonId))
            {
                return false;
            }
            List<TimeBlockDB> timeBlocksBySalonId = IoC.Resolve<ISchedulingManager>().GetTimeBlocksBySalonId(salon.SalonId);
            if (this.IsBlockedByEmployee(employees, date, timeBlocksBySalonId))
            {
                return false;
            }
            if (this.IsBlockedByAllEmployees(employees, date, timeBlocksBySalonId))
            {
                return false;
            }
            if (this.IsBlockedByTime(date, timeslot, timeBlocksBySalonId))
            {
                return false;
            }
            if (this.IsBlockedBySlot(date, timeslot, timeBlocksBySalonId))
            {
                return false;
            }
            return true;
        }

        private bool IsBlockedByAllEmployees(List<EmployeeDB> employees, DateTime date, List<TimeBlockDB> blockedTimes)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<Employee_Service_MappingDB> list = (from item in IoC.Resolve<IEmployeeManager>().GetEmployeeServiceMappingBySalonId(salon.SalonId)
                where item.ServiceId == this.PostedServiceId
                select item).ToList<Employee_Service_MappingDB>();
            if (list.Count > 0)
            {
                DayOfWeek day = date.DayOfWeek;
                blockedTimes = (from item in blockedTimes
                    where item.EmployeeId.HasValue
                    select item).ToList<TimeBlockDB>();
                blockedTimes = (from item in blockedTimes
                    where (item.WeekDay.Value - 1) == day
                    select item).ToList<TimeBlockDB>();
                int num = 0;
                using (List<TimeBlockDB>.Enumerator enumerator = blockedTimes.GetEnumerator())
                {
                    Predicate<Employee_Service_MappingDB> match = null;
                    TimeBlockDB block;
                    while (enumerator.MoveNext())
                    {
                        block = enumerator.Current;
                        if (match == null)
                        {
                            match = item => item.EmployeeId == block.EmployeeId;
                        }
                        if (list.Exists(match))
                        {
                            num++;
                        }
                    }
                }
                if (num > (list.Count - 1))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBlockedByDuration(TimeSlotUI timeslot, ServiceDB service, OpeningHoursDB hours)
        {
            if (timeslot == null)
            {
                throw new ArgumentNullException("timeslot");
            }
            if (hours == null)
            {
                throw new Exception("Opening hours cannot be null.");
            }
            if (service == null)
            {
                throw new Exception("Service cannot be null.");
            }
            DateTime time10 = new DateTime(timeslot.Time.Ticks);
            DateTime time2 = IFRMHelper.FromUrlFriendlyTime(time10.ToString("HH:mm").Replace(':', '.')).AddMinutes((double) service.Duration);
            if (timeslot.WeekDay == DayOfWeek.Monday)
            {
                if (hours.MonClosed)
                {
                    return true;
                }
                DateTime time3 = IFRMHelper.FromUrlFriendlyTime(new DateTime(hours.MonEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time3)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Tuesday)
            {
                if (hours.TueClosed)
                {
                    return true;
                }
                DateTime time4 = IFRMHelper.FromUrlFriendlyTime(new DateTime(hours.TueEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time4)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Wednesday)
            {
                if (hours.WedClosed)
                {
                    return true;
                }
                DateTime time5 = IFRMHelper.FromUrlFriendlyTime(new DateTime(hours.WedEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time5)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Thursday)
            {
                if (hours.ThuClosed)
                {
                    return true;
                }
                DateTime time6 = IFRMHelper.FromUrlFriendlyTime(new DateTime(hours.ThuEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time6)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Friday)
            {
                if (hours.FriClosed)
                {
                    return true;
                }
                DateTime time7 = IFRMHelper.FromUrlFriendlyTime(new DateTime(hours.FriEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time7)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Saturday)
            {
                if (hours.SatClosed)
                {
                    return true;
                }
                DateTime time8 = IFRMHelper.FromUrlFriendlyTime(new DateTime(hours.SatEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time8)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Sunday)
            {
                if (hours.SunClosed)
                {
                    return true;
                }
                DateTime time9 = IFRMHelper.FromUrlFriendlyTime(new DateTime(hours.SunEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time9)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBlockedByEmployee(List<EmployeeDB> employees, DateTime date, List<TimeBlockDB> blockedTimes)
        {
            Func<EmployeeDB, bool> predicate = null;
            if (employees.Count > 0)
            {
                Func<TimeBlockDB, bool> func = null;
                if (predicate == null)
                {
                    predicate = item => item.EmployeeId == this.PostedEmployeeId;
                }
                EmployeeDB employee = employees.SingleOrDefault<EmployeeDB>(predicate);
                if (employee != null)
                {
                    DayOfWeek day = date.DayOfWeek;
                    if (func == null)
                    {
                        func = item => item.EmployeeId == employee.EmployeeId;
                    }
                    blockedTimes = blockedTimes.Where<TimeBlockDB>(func).ToList<TimeBlockDB>();
                    return blockedTimes.Exists(item => (item.WeekDay.Value - 1) == day);
                }
            }
            return false;
        }

        private bool IsBlockedBySlot(DateTime date, TimeSlotUI timeslot, IEnumerable<TimeBlockDB> blockedTimes)
        {
            if (timeslot == null)
            {
                throw new ArgumentNullException("timeslot");
            }
            if (blockedTimes == null)
            {
                throw new ArgumentNullException("blockedTimes");
            }
            blockedTimes = from item in blockedTimes
                where item.Slots.HasValue && (item.Date.Date == date.Date)
                select item;
            blockedTimes = from item in blockedTimes
                where !item.EmployeeId.HasValue && !item.ServiceId.HasValue
                select item;
            blockedTimes = from item in blockedTimes
                where item.StartTime == timeslot.Time
                select item;
            int? nullable = blockedTimes.Sum<TimeBlockDB>(item => item.Slots);
            int slots = timeslot.Slots;
            return ((slots + nullable) <= 0);
        }

        private bool IsBlockedByTime(DateTime date, TimeSlotUI timeslot, IEnumerable<TimeBlockDB> blockedTimes)
        {
            if (timeslot == null)
            {
                throw new ArgumentNullException("timeslot");
            }
            if (blockedTimes == null)
            {
                throw new ArgumentNullException("blockedTimes");
            }
            blockedTimes = from item in blockedTimes
                where !item.Slots.HasValue && (item.Date.Date == date.Date)
                select item;
            blockedTimes = from item in blockedTimes
                where !item.EmployeeId.HasValue && !item.ServiceId.HasValue
                select item;
            blockedTimes = from item in blockedTimes
                where item.StartTime == timeslot.Time
                select item;
            return (blockedTimes.Count<TimeBlockDB>() > 0);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Func<EmployeeDB, bool> predicate = null;
            Func<ServiceDB, bool> func2 = null;
            base.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1.0));
            base.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            base.Response.Cache.SetNoStore();
            this.Page.Form.Attributes.Add("autocomplete", "off");
            if (this.PostedServiceId == Guid.Empty)
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            if (this.PostedDate.Date.CompareTo(IFRMHelper.GetMinimumSearchDate().Date) < 0)
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            if (this.PostedTime == DateTime.MinValue)
            {
                base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
            }
            if (!this.Page.IsPostBack)
            {
                DateTime postedDate = this.PostedDate;
                DateTime postedTime = this.PostedTime;
                SalonDB salon = IFRMContext.Current.Salon;
                List<EmployeeDB> employeesBySalonId = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salon.SalonId);
                if (predicate == null)
                {
                    predicate = item => item.EmployeeId == this.PostedEmployeeId;
                }
                EmployeeDB employee = employeesBySalonId.SingleOrDefault<EmployeeDB>(predicate);
                if ((this.PostedEmployeeId != Guid.Empty) && (employee == null))
                {
                    base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
                }
                if (func2 == null)
                {
                    func2 = item => item.ServiceId == this.PostedServiceId;
                }
                ServiceDB service = (from item in IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId)
                    where item.Active
                    select item).SingleOrDefault<ServiceDB>(func2);
                if ((service == null) || (service.SalonId != salon.SalonId))
                {
                    base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
                }
                if (!this.IsAppointmentAvailble(postedDate, postedTime, salon, service, employeesBySalonId))
                {
                    base.Response.Redirect(IFRMHelper.GetURL("unavailable.aspx", new string[0]), true);
                }
                this.ddlPhoneType.DataBind();
                this.rblYesNo.DataBind();
                this.BindSalonDetails();
                this.BindAppointmentDetails(postedDate, postedTime, service, employee);
            }
        }

        public DateTime PostedDate
        {
            get
            {
                string str = base.Request.QueryString["date"];
                return IFRMHelper.FromUrlFriendlyDate(str);
            }
        }

        public Guid PostedEmployeeId
        {
            get
            {
                string str = base.Request.QueryString["eid"];
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

        public Guid PostedServiceId
        {
            get
            {
                string str = base.Request.QueryString["svid"];
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

        public DateTime PostedTime
        {
            get
            {
                string str = base.Request.QueryString["time"];
                return IFRMHelper.FromUrlFriendlyTime(str);
            }
        }
    }
}


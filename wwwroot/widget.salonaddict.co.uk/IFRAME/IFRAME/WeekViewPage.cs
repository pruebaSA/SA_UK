namespace IFRAME
{
    using IFRAME.Controllers;
    using IFRAME.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class WeekViewPage : IFRMPage
    {
        protected ViewTypeMenu cntlViewTypes;
        protected TimeSlotsGrid DayTimes1;
        protected TimeSlotsGrid DayTimes2;
        protected TimeSlotsGrid DayTimes3;
        protected TimeSlotsGrid DayTimes4;
        protected TimeSlotsGrid DayTimes5;
        protected TimeSlotsGrid DayTimes6;
        protected TimeSlotsGrid DayTimes7;
        protected DropDownList ddlEmployee;
        protected Literal ltrHeader;
        protected Panel pnl;
        protected Panel pnle;
        protected Panel pnlt;

        private void BindEmployeeList(List<EmployeeDB> employees, List<Employee_Service_MappingDB> mappings)
        {
            ListItemCollection datasource = new ListItemCollection {
                new ListItem(base.GetLocaleResourceString("ddlEmployee.DefaultText"), base.GetLocaleResourceString("ddlEmployee.DefaultValue"))
            };
            employees.ForEach(delegate (EmployeeDB item) {
                if (mappings.Exists(m => m.EmployeeId == item.EmployeeId))
                {
                    datasource.Add(new ListItem(item.DisplayText, item.EmployeeId.ToString()));
                }
            });
            this.ddlEmployee.DataSource = datasource;
            this.ddlEmployee.DataTextField = "Text";
            this.ddlEmployee.DataValueField = "Value";
            this.ddlEmployee.DataBind();
        }

        private void BindTimeSlotGrid1(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID, List<TimeSlotUI> timeslots, OpeningHoursDB hours, List<ClosingDayDB> holidays, List<TimeBlockDB> blockedTimes, bool isSearchedDay)
        {
            this.DayTimes1.Date = date;
            this.DayTimes1.OpeningHours = hours;
            this.DayTimes1.Service = service;
            this.DayTimes1.Mappings = mappings;
            if (IFRMHelper.IsSalonClosed(date, hours, holidays))
            {
                this.DayTimes1.IsSalonClosed = true;
            }
            else
            {
                this.DayTimes1.IsSearchedDay = isSearchedDay;
                this.DayTimes1.TimeSlots = timeslots;
                this.DayTimes1.EmployeeId = employeeID;
                this.DayTimes1.Employees = employees;
                this.DayTimes1.BlockedTimes = blockedTimes;
            }
            this.DayTimes1.DataBind();
        }

        private void BindTimeSlotGrid2(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID, List<TimeSlotUI> timeslots, OpeningHoursDB hours, List<ClosingDayDB> holidays, List<TimeBlockDB> blockedTimes, bool isSearchedDay)
        {
            this.DayTimes2.Date = date;
            this.DayTimes2.OpeningHours = hours;
            this.DayTimes2.Service = service;
            this.DayTimes2.Mappings = mappings;
            if (IFRMHelper.IsSalonClosed(date, hours, holidays))
            {
                this.DayTimes2.IsSalonClosed = true;
            }
            else
            {
                this.DayTimes2.IsSearchedDay = isSearchedDay;
                this.DayTimes2.TimeSlots = timeslots;
                this.DayTimes2.EmployeeId = employeeID;
                this.DayTimes2.Employees = employees;
                this.DayTimes2.BlockedTimes = blockedTimes;
            }
            this.DayTimes2.DataBind();
        }

        private void BindTimeSlotGrid3(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID, List<TimeSlotUI> timeslots, OpeningHoursDB hours, List<ClosingDayDB> holidays, List<TimeBlockDB> blockedTimes, bool isSearchedDay)
        {
            this.DayTimes3.Date = date;
            this.DayTimes3.OpeningHours = hours;
            this.DayTimes3.Service = service;
            this.DayTimes3.Mappings = mappings;
            if (IFRMHelper.IsSalonClosed(date, hours, holidays))
            {
                this.DayTimes3.IsSalonClosed = true;
            }
            else
            {
                this.DayTimes3.IsSearchedDay = isSearchedDay;
                this.DayTimes3.TimeSlots = timeslots;
                this.DayTimes3.EmployeeId = employeeID;
                this.DayTimes3.Employees = employees;
                this.DayTimes3.BlockedTimes = blockedTimes;
            }
            this.DayTimes3.DataBind();
        }

        private void BindTimeSlotGrid4(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID, List<TimeSlotUI> timeslots, OpeningHoursDB hours, List<ClosingDayDB> holidays, List<TimeBlockDB> blockedTimes, bool isSearchedDay)
        {
            this.DayTimes4.Date = date;
            this.DayTimes4.OpeningHours = hours;
            this.DayTimes4.Service = service;
            this.DayTimes4.Mappings = mappings;
            if (IFRMHelper.IsSalonClosed(date, hours, holidays))
            {
                this.DayTimes4.IsSalonClosed = true;
            }
            else
            {
                this.DayTimes4.IsSearchedDay = isSearchedDay;
                this.DayTimes4.TimeSlots = timeslots;
                this.DayTimes4.EmployeeId = employeeID;
                this.DayTimes4.Employees = employees;
                this.DayTimes4.BlockedTimes = blockedTimes;
            }
            this.DayTimes4.DataBind();
        }

        private void BindTimeSlotGrid5(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID, List<TimeSlotUI> timeslots, OpeningHoursDB hours, List<ClosingDayDB> holidays, List<TimeBlockDB> blockedTimes, bool isSearchedDay)
        {
            this.DayTimes5.Date = date;
            this.DayTimes5.OpeningHours = hours;
            this.DayTimes5.Service = service;
            this.DayTimes5.Mappings = mappings;
            if (IFRMHelper.IsSalonClosed(date, hours, holidays))
            {
                this.DayTimes5.IsSalonClosed = true;
            }
            else
            {
                this.DayTimes5.IsSearchedDay = isSearchedDay;
                this.DayTimes5.TimeSlots = timeslots;
                this.DayTimes5.EmployeeId = employeeID;
                this.DayTimes5.Employees = employees;
                this.DayTimes5.BlockedTimes = blockedTimes;
            }
            this.DayTimes5.DataBind();
        }

        private void BindTimeSlotGrid6(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID, List<TimeSlotUI> timeslots, OpeningHoursDB hours, List<ClosingDayDB> holidays, List<TimeBlockDB> blockedTimes, bool isSearchedDay)
        {
            this.DayTimes6.Date = date;
            this.DayTimes6.OpeningHours = hours;
            this.DayTimes6.Service = service;
            this.DayTimes6.Mappings = mappings;
            if (IFRMHelper.IsSalonClosed(date, hours, holidays))
            {
                this.DayTimes6.IsSalonClosed = true;
            }
            else
            {
                this.DayTimes6.IsSearchedDay = isSearchedDay;
                this.DayTimes6.TimeSlots = timeslots;
                this.DayTimes6.EmployeeId = employeeID;
                this.DayTimes6.Employees = employees;
                this.DayTimes6.BlockedTimes = blockedTimes;
            }
            this.DayTimes6.DataBind();
        }

        private void BindTimeSlotGrid7(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID, List<TimeSlotUI> timeslots, OpeningHoursDB hours, List<ClosingDayDB> holidays, List<TimeBlockDB> blockedTimes, bool isSearchedDay)
        {
            this.DayTimes7.Date = date;
            this.DayTimes7.OpeningHours = hours;
            this.DayTimes7.Service = service;
            this.DayTimes7.Mappings = mappings;
            if (IFRMHelper.IsSalonClosed(date, hours, holidays))
            {
                this.DayTimes7.IsSalonClosed = true;
            }
            else
            {
                this.DayTimes7.IsSearchedDay = isSearchedDay;
                this.DayTimes7.TimeSlots = timeslots;
                this.DayTimes7.EmployeeId = employeeID;
                this.DayTimes7.Employees = employees;
                this.DayTimes7.BlockedTimes = blockedTimes;
            }
            this.DayTimes7.DataBind();
        }

        private void BindTimeSlotGrids(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID)
        {
            DateTime time;
            DateTime time2;
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), service.Name, service.Price.ToString("C"));
            SalonDB salon = IFRMContext.Current.Salon;
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            List<ClosingDayDB> closingDaysBySalonId = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId);
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
            List<TimeBlockDB> timeBlocksBySalonId = IoC.Resolve<ISchedulingManager>().GetTimeBlocksBySalonId(salon.SalonId);
            IFRMHelper.GetWeekRangeBySearchDate(date, out time, out time2);
            DateTime time3 = time.Date;
            DateTime time4 = time3.AddDays(1.0);
            DateTime time5 = time4.AddDays(1.0);
            DateTime time6 = time5.AddDays(1.0);
            DateTime time7 = time6.AddDays(1.0);
            DateTime time8 = time7.AddDays(1.0);
            DateTime time9 = time8.AddDays(1.0);
            this.BindTimeSlotGrid1(time3, service, mappings, employees, employeeID, timeslots, openingHoursBySalonId, closingDaysBySalonId, timeBlocksBySalonId, time3 == date);
            this.BindTimeSlotGrid2(time4, service, mappings, employees, employeeID, timeslots, openingHoursBySalonId, closingDaysBySalonId, timeBlocksBySalonId, time4 == date);
            this.BindTimeSlotGrid3(time5, service, mappings, employees, employeeID, timeslots, openingHoursBySalonId, closingDaysBySalonId, timeBlocksBySalonId, time5 == date);
            this.BindTimeSlotGrid4(time6, service, mappings, employees, employeeID, timeslots, openingHoursBySalonId, closingDaysBySalonId, timeBlocksBySalonId, time6 == date);
            this.BindTimeSlotGrid5(time7, service, mappings, employees, employeeID, timeslots, openingHoursBySalonId, closingDaysBySalonId, timeBlocksBySalonId, time7 == date);
            this.BindTimeSlotGrid6(time8, service, mappings, employees, employeeID, timeslots, openingHoursBySalonId, closingDaysBySalonId, timeBlocksBySalonId, time8 == date);
            this.BindTimeSlotGrid7(time9, service, mappings, employees, employeeID, timeslots, openingHoursBySalonId, closingDaysBySalonId, timeBlocksBySalonId, time9 == date);
        }

        protected void DayTimes_TimeSelected(object sender, TimeSelectedEventArgs e)
        {
            string str = IFRMHelper.ToUrlFriendlyDate(e.Date);
            string str2 = IFRMHelper.ToUrlFriendlyTime(new DateTime(e.Time.Ticks));
            string selectedValue = this.ddlEmployee.SelectedValue;
            string str4 = $"{"svid"}={this.PostedServiceId}";
            string str5 = $"{"eid"}={selectedValue}";
            string str6 = $"{"date"}={str}";
            string str7 = $"{"time"}={str2}";
            if (string.IsNullOrEmpty(selectedValue))
            {
                string uRL = IFRMHelper.GetURL("details.aspx", new string[] { str4, str6, str7 });
                base.Response.Redirect(uRL, true);
            }
            else
            {
                string url = IFRMHelper.GetURL("details.aspx", new string[] { str4, str5, str6, str7 });
                base.Response.Redirect(url, true);
            }
        }

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = this.ddlEmployee.SelectedValue;
            string str2 = $"{"svid"}={this.PostedServiceId}";
            string str3 = $"{"eid"}={selectedValue}";
            string str4 = $"{"date"}={IFRMHelper.ToUrlFriendlyDate(this.PostedDate)}";
            if (string.IsNullOrEmpty(selectedValue))
            {
                string uRL = IFRMHelper.GetURL("weekview.aspx", new string[] { str2, str4 });
                base.Response.Redirect(uRL, true);
            }
            else
            {
                string url = IFRMHelper.GetURL("weekview.aspx", new string[] { str2, str3, str4 });
                base.Response.Redirect(url, true);
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            if (this.ddlEmployee.Items.Count == 1)
            {
                this.pnle.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Func<Employee_Service_MappingDB, bool> predicate = null;
            Func<EmployeeDB, bool> func2 = null;
            if (!this.Page.IsPostBack)
            {
                DateTime date = this.PostedDate.Date;
                Guid serviceID = this.PostedServiceId;
                Guid postedEmployeeId = this.PostedEmployeeId;
                if (serviceID == Guid.Empty)
                {
                    base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
                }
                if (date.CompareTo(IFRMHelper.GetMinimumSearchDate().Date) < 0)
                {
                    base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
                }
                SalonDB salon = IFRMContext.Current.Salon;
                ServiceDB service = (from item in IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId)
                    where item.Active
                    select item).SingleOrDefault<ServiceDB>(item => item.ServiceId == serviceID);
                if (service == null)
                {
                    base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
                }
                List<EmployeeDB> employeesBySalonId = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salon.SalonId);
                if (predicate == null)
                {
                    predicate = item => item.ServiceId == this.PostedServiceId;
                }
                List<Employee_Service_MappingDB> mappings = IoC.Resolve<IEmployeeManager>().GetEmployeeServiceMappingBySalonId(salon.SalonId).Where<Employee_Service_MappingDB>(predicate).ToList<Employee_Service_MappingDB>();
                this.BindEmployeeList(employeesBySalonId, mappings);
                if (func2 == null)
                {
                    func2 = item => item.EmployeeId == this.PostedEmployeeId;
                }
                EmployeeDB edb2 = employeesBySalonId.FirstOrDefault<EmployeeDB>(func2);
                if (edb2 == null)
                {
                    if (this.PostedEmployeeId != Guid.Empty)
                    {
                        base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
                    }
                }
                else if (this.ddlEmployee.Items.FindByValue(edb2.EmployeeId.ToString()) != null)
                {
                    this.ddlEmployee.SelectedValue = edb2.EmployeeId.ToString();
                }
                this.BindTimeSlotGrids(date, service, mappings, employeesBySalonId, postedEmployeeId);
            }
        }

        public DateTime PostedDate
        {
            get
            {
                string str = base.Request.QueryString["date"];
                return IFRMHelper.FromUrlFriendlyDate(str).Date;
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
    }
}


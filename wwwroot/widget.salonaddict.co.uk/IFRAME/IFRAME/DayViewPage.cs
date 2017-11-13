namespace IFRAME
{
    using IFRAME.Controllers;
    using IFRAME.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class DayViewPage : IFRMPage
    {
        protected ViewTypeMenu cntlViewTypes;
        protected TimeSlotsGrid DayTimes;
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

        private void BindTimeSlotGrid(DateTime date, ServiceDB service, List<Employee_Service_MappingDB> mappings, List<EmployeeDB> employees, Guid employeeID)
        {
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), service.Name, service.Price.ToString("C"));
            SalonDB salon = IFRMContext.Current.Salon;
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            List<ClosingDayDB> closingDaysBySalonId = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId);
            this.DayTimes.Date = date;
            this.DayTimes.OpeningHours = openingHoursBySalonId;
            this.DayTimes.Service = service;
            this.DayTimes.Mappings = mappings;
            if (IFRMHelper.IsSalonClosed(date, openingHoursBySalonId, closingDaysBySalonId))
            {
                this.DayTimes.IsSalonClosed = true;
            }
            else
            {
                List<ScheduleDB> list2 = (from item in IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salon.SalonId)
                    where item.ScheduleTypeEnum == ScheduleTypeEnum.TimeSlot
                    select item).ToList<ScheduleDB>();
                List<TimeSlotUI> timeslots = new List<TimeSlotUI>();
                list2.ForEach(delegate (ScheduleDB item) {
                    TimeSlotUI tui = new TimeSlotUI {
                        Slots = item.Slots.Value,
                        Time = item.Time.Value,
                        WeekDay = item.WeekDayEnum.Value
                    };
                    timeslots.Add(tui);
                });
                List<TimeBlockDB> timeBlocksBySalonId = IoC.Resolve<ISchedulingManager>().GetTimeBlocksBySalonId(salon.SalonId);
                this.DayTimes.IsSearchedDay = true;
                this.DayTimes.EmployeeId = employeeID;
                this.DayTimes.Employees = employees;
                this.DayTimes.TimeSlots = timeslots;
                this.DayTimes.BlockedTimes = timeBlocksBySalonId;
            }
            this.DayTimes.DataBind();
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
                string uRL = IFRMHelper.GetURL("dayview.aspx", new string[] { str2, str4 });
                base.Response.Redirect(uRL, true);
            }
            else
            {
                string url = IFRMHelper.GetURL("dayview.aspx", new string[] { str2, str3, str4 });
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
                this.BindTimeSlotGrid(date, service, mappings, employeesBySalonId, this.PostedEmployeeId);
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


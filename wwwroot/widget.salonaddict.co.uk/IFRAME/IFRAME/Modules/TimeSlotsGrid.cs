namespace IFRAME.Modules
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.UI.WebControls;

    public class TimeSlotsGrid : IFRMUserControl
    {
        private bool _IsDayBlocked;
        protected Label lblDate;
        protected ListView lv;
        protected MultiView mv;
        protected System.Web.UI.WebControls.View v0;
        protected System.Web.UI.WebControls.View v1;

        public event TimeSelectedHandler TimeSelected;

        public override void DataBind()
        {
            base.DataBind();
            DateTime date = this.Date;
            this.lblDate.Text = date.ToString("ddd dd MMM yyyy");
            if (this.IsSearchedDay)
            {
                this.lblDate.Font.Bold = true;
            }
            if (this.IsSalonClosed)
            {
                this.mv.ActiveViewIndex = 1;
            }
            else
            {
                List<TimeSlotUI> timeSlots = this.TimeSlots;
                if (timeSlots == null)
                {
                    throw new Exception("Timeslots cannot be null.");
                }
                bool flag = this.IsBlockedByEmployee();
                if (!flag)
                {
                    flag = this.IsBlockedByAllEmployees();
                }
                this._IsDayBlocked = flag;
                List<TimeSlotUI> list2 = (from item in timeSlots
                    where item.WeekDay == date.DayOfWeek
                    orderby item.Time
                    select item).ToList<TimeSlotUI>();
                this.lv.DataSource = list2;
                this.lv.DataBind();
            }
        }

        private bool IsBlockedByAllEmployees()
        {
            if (this.Mappings.Count > 0)
            {
                DayOfWeek day = this.Date.DayOfWeek;
                List<TimeBlockDB> list = (from item in (from item in this.BlockedTimes
                    where item.EmployeeId.HasValue
                    select item).ToList<TimeBlockDB>()
                    where (item.WeekDay.Value - 1) == day
                    select item).ToList<TimeBlockDB>();
                int num = 0;
                using (List<TimeBlockDB>.Enumerator enumerator = list.GetEnumerator())
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
                        if (this.Mappings.Exists(match))
                        {
                            num++;
                        }
                    }
                }
                if (num > (this.Mappings.Count - 1))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBlockedByDuration(TimeSlotUI timeslot)
        {
            OpeningHoursDB openingHours = this.OpeningHours;
            if (openingHours == null)
            {
                throw new Exception("Opening hours cannot be null.");
            }
            ServiceDB service = this.Service;
            if (service == null)
            {
                throw new Exception("Service cannot be null.");
            }
            DateTime time10 = new DateTime(timeslot.Time.Ticks);
            DateTime time2 = IFRMHelper.FromUrlFriendlyTime(time10.ToString("HH:mm").Replace(':', '.')).AddMinutes((double) service.Duration);
            if (timeslot.WeekDay == DayOfWeek.Monday)
            {
                if (openingHours.MonClosed)
                {
                    return true;
                }
                DateTime time3 = IFRMHelper.FromUrlFriendlyTime(new DateTime(openingHours.MonEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time3)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Tuesday)
            {
                if (openingHours.TueClosed)
                {
                    return true;
                }
                DateTime time4 = IFRMHelper.FromUrlFriendlyTime(new DateTime(openingHours.TueEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time4)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Wednesday)
            {
                if (openingHours.WedClosed)
                {
                    return true;
                }
                DateTime time5 = IFRMHelper.FromUrlFriendlyTime(new DateTime(openingHours.WedEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time5)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Thursday)
            {
                if (openingHours.ThuClosed)
                {
                    return true;
                }
                DateTime time6 = IFRMHelper.FromUrlFriendlyTime(new DateTime(openingHours.ThuEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time6)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Friday)
            {
                if (openingHours.FriClosed)
                {
                    return true;
                }
                DateTime time7 = IFRMHelper.FromUrlFriendlyTime(new DateTime(openingHours.FriEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time7)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Saturday)
            {
                if (openingHours.SatClosed)
                {
                    return true;
                }
                DateTime time8 = IFRMHelper.FromUrlFriendlyTime(new DateTime(openingHours.SatEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time8)
                {
                    return true;
                }
            }
            if (timeslot.WeekDay == DayOfWeek.Sunday)
            {
                if (openingHours.SunClosed)
                {
                    return true;
                }
                DateTime time9 = IFRMHelper.FromUrlFriendlyTime(new DateTime(openingHours.SunEnd1.Value.Ticks).ToString("HH.mm"));
                if (time2 > time9)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBlockedByEmployee()
        {
            Func<EmployeeDB, bool> predicate = null;
            if (this.Employees.Count > 0)
            {
                Func<TimeBlockDB, bool> func = null;
                if (predicate == null)
                {
                    predicate = item => item.EmployeeId == this.EmployeeId;
                }
                EmployeeDB employee = this.Employees.SingleOrDefault<EmployeeDB>(predicate);
                if (employee != null)
                {
                    DayOfWeek day = this.Date.DayOfWeek;
                    if (func == null)
                    {
                        func = item => item.EmployeeId == employee.EmployeeId;
                    }
                    return this.BlockedTimes.Where<TimeBlockDB>(func).ToList<TimeBlockDB>().Exists(item => (item.WeekDay.Value - 1) == day);
                }
            }
            return false;
        }

        private bool IsBlockedBySlot(TimeSlotUI timeslot)
        {
            IEnumerable<TimeBlockDB> enumerable = this.BlockedTimes.AsEnumerable<TimeBlockDB>();
            if (enumerable == null)
            {
                throw new Exception("Blocked times cannot be null.");
            }
            DateTime date = this.Date;
            int? nullable = (from item in enumerable
                where item.Slots.HasValue && (item.Date.Date == date.Date)
                where !item.EmployeeId.HasValue && !item.ServiceId.HasValue
                where item.StartTime == timeslot.Time
                select item).Sum<TimeBlockDB>(item => item.Slots);
            int slots = timeslot.Slots;
            return ((slots + nullable) <= 0);
        }

        private bool IsBlockedByTime(TimeSlotUI timeslot)
        {
            IEnumerable<TimeBlockDB> enumerable = this.BlockedTimes.AsEnumerable<TimeBlockDB>();
            if (enumerable == null)
            {
                throw new Exception("Blocked times cannot be null.");
            }
            DateTime date = this.Date;
            return ((from item in enumerable
                where !item.Slots.HasValue && (item.Date.Date == date.Date)
                where !item.EmployeeId.HasValue && !item.ServiceId.HasValue
                where item.StartTime == timeslot.Time
                select item).Count<TimeBlockDB>() > 0);
        }

        protected void lv_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                LinkButton button = (LinkButton) e.Item.FindControl("lb");
                if (button != null)
                {
                    button.CssClass = "lbutton-available-time-disabled";
                    button.Enabled = false;
                    TimeSlotUI dataItem = (TimeSlotUI) ((ListViewDataItem) e.Item).DataItem;
                    if (dataItem != null)
                    {
                        button.Text = new DateTime(dataItem.Time.Ticks).ToString("HH:mm");
                        if (((!this._IsDayBlocked && !this.IsBlockedByDuration(dataItem)) && !this.IsBlockedByTime(dataItem)) && !this.IsBlockedBySlot(dataItem))
                        {
                            button.CssClass = "lbutton-available-time";
                            button.Enabled = true;
                        }
                    }
                }
            }
        }

        protected void lv_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
        {
            string s = this.lv.DataKeys[e.NewSelectedIndex].Value.ToString();
            TimeSelectedEventArgs args = new TimeSelectedEventArgs {
                Date = this.Date,
                Time = TimeSpan.Parse(s)
            };
            if (this.TimeSelected != null)
            {
                this.TimeSelected(sender, args);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public List<TimeBlockDB> BlockedTimes { get; set; }

        public DateTime Date
        {
            get
            {
                object obj2 = this.ViewState["Date"];
                if (obj2 == null)
                {
                    throw new Exception($"Viewstate does not have an item with key '{"Date"}' defined.");
                }
                return Convert.ToDateTime(obj2);
            }
            set
            {
                this.ViewState["Date"] = value.ToString();
            }
        }

        public Guid EmployeeId { get; set; }

        public List<EmployeeDB> Employees { get; set; }

        public bool IsSalonClosed
        {
            get
            {
                object obj2 = this.ViewState["IsSalonClosed"];
                return Convert.ToBoolean(obj2);
            }
            set
            {
                this.ViewState["IsSalonClosed"] = value.ToString();
            }
        }

        public bool IsSearchedDay { get; set; }

        public List<Employee_Service_MappingDB> Mappings { get; set; }

        public OpeningHoursDB OpeningHours { get; set; }

        public ServiceDB Service { get; set; }

        public List<TimeSlotUI> TimeSlots { get; set; }

        public delegate void TimeSelectedHandler(object sender, TimeSelectedEventArgs e);
    }
}


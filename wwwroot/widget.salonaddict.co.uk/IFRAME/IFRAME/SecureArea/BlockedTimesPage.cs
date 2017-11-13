namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.Modules;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class BlockedTimesPage : IFRMSecurePage
    {
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected BlockedTimeLine cntrlDay1;
        protected BlockedTimeLine cntrlDay2;
        protected BlockedTimeLine cntrlDay3;
        protected BlockedTimeLine cntrlDay4;
        protected BlockedTimeLine cntrlDay5;
        protected BlockedTimeLine cntrlDay6;
        protected BlockedTimeLine cntrlDay7;
        protected Literal ltrHelp;
        protected Panel pnl;
        protected Panel pnlt;
        protected DateTextBox txtDate;

        private void BindBlockedTimes()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            List<ClosingDayDB> closingDaysBySalonId = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId);
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
            DateTime date1 = this.txtDate.Date;
            DateTime date = this.txtDate.Date.AddDays(1.0);
            DateTime time2 = this.txtDate.Date.AddDays(2.0);
            DateTime time3 = this.txtDate.Date.AddDays(3.0);
            DateTime time4 = this.txtDate.Date.AddDays(4.0);
            DateTime time5 = this.txtDate.Date.AddDays(5.0);
            DateTime date7 = this.txtDate.Date.AddDays(6.0);
            string str = $"{date1.ToString("MMM dd")} - {date7.ToString("MMM dd")}";
            this.ltrHelp.Text = string.Format(base.GetLocaleResourceString("ltrHelp.Text"), str);
            List<TimeBlockDB> blocked = (from item in (from item in IoC.Resolve<ISchedulingManager>().GetTimeBlocksBySalonId(salon.SalonId)
                where !item.TicketId.HasValue
                select item).ToList<TimeBlockDB>()
                where (date1 <= item.Date) && (item.Date <= date7)
                select item).ToList<TimeBlockDB>();
            this.cntrlDay1.DataBind(date1, timeslots, blocked, openingHoursBySalonId, closingDaysBySalonId);
            this.cntrlDay2.DataBind(date, timeslots, blocked, openingHoursBySalonId, closingDaysBySalonId);
            this.cntrlDay3.DataBind(time2, timeslots, blocked, openingHoursBySalonId, closingDaysBySalonId);
            this.cntrlDay4.DataBind(time3, timeslots, blocked, openingHoursBySalonId, closingDaysBySalonId);
            this.cntrlDay5.DataBind(time4, timeslots, blocked, openingHoursBySalonId, closingDaysBySalonId);
            this.cntrlDay6.DataBind(time5, timeslots, blocked, openingHoursBySalonId, closingDaysBySalonId);
            this.cntrlDay7.DataBind(date7, timeslots, blocked, openingHoursBySalonId, closingDaysBySalonId);
        }

        protected void BlockedTime_ItemEditing(object sender, BlockedTimeEventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            DateTime time = IFRMHelper.FromUrlFriendlyDate(e.Date);
            TimeSpan span = TimeSpan.Parse(e.Time);
            if (e.BlockedTimeId == Guid.Empty)
            {
                TimeBlockDB block = new TimeBlockDB {
                    BlockTypeId = 2,
                    CycleLength = 1,
                    CyclePeriodType = 10,
                    TotalCycles = 1,
                    EndDateUtc = new DateTime?(time.Date),
                    EndTime = span,
                    SalonId = salon.SalonId,
                    StartDateUtc = time.Date,
                    StartTime = span
                };
                IoC.Resolve<ISchedulingManager>().InsertTimeBlock(block);
            }
            else
            {
                TimeBlockDB timeBlockById = IoC.Resolve<ISchedulingManager>().GetTimeBlockById(e.BlockedTimeId);
                IoC.Resolve<ISchedulingManager>().DeleteTimeBlock(timeBlockById);
            }
            string str = $"{"date"}={IFRMHelper.ToUrlFriendlyDate(this.PostedDate)}";
            string uRL = IFRMHelper.GetURL("blockedtimes.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.txtDate.StartDate = DateTime.Today;
            if (!this.Page.IsPostBack)
            {
                DateTime postedDate = this.PostedDate;
                if ((postedDate != DateTime.MinValue) && (postedDate.CompareTo(DateTime.Today) >= 0))
                {
                    this.txtDate.Date = postedDate;
                }
                else
                {
                    this.txtDate.Date = DateTime.Today.AddDays(1.0);
                }
                this.BindBlockedTimes();
            }
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            DateTime date = this.txtDate.Date;
            if (date == DateTime.MinValue)
            {
                string uRL = IFRMHelper.GetURL("blockedtimes.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            else
            {
                string str2 = $"{"date"}={IFRMHelper.ToUrlFriendlyDate(date)}";
                string url = IFRMHelper.GetURL("blockedtimes.aspx", new string[] { str2 });
                base.Response.Redirect(url, true);
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
    }
}


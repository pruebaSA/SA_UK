namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Scheduling_Page : IFRMSecurePage
    {
        protected Button btnBack;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected SchedulingLine cntrlFriday;
        protected SchedulingLine cntrlMonday;
        protected SchedulingLine cntrlSaturday;
        protected SchedulingLine cntrlSunday;
        protected SchedulingLine cntrlThursday;
        protected SchedulingLine cntrlTuesday;
        protected SchedulingLine cntrlWednesday;
        protected Panel pnl;
        protected Panel pnlt;

        private void BindScheduledTimes()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            List<ScheduleDB> list = (from item in IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salon.SalonId)
                where item.ScheduleTypeEnum == ScheduleTypeEnum.TimeSlot
                select item).ToList<ScheduleDB>();
            List<TimeSlotUI> timeslots = new List<TimeSlotUI>();
            list.ForEach(delegate (ScheduleDB item) {
                TimeSlotUI tui = new TimeSlotUI {
                    Slots = item.Slots.Value,
                    Time = item.Time.Value,
                    WeekDay = item.WeekDayEnum.Value
                };
                timeslots.Add(tui);
            });
            this.cntrlMonday.DataBind(timeslots, openingHoursBySalonId);
            this.cntrlTuesday.DataBind(timeslots, openingHoursBySalonId);
            this.cntrlWednesday.DataBind(timeslots, openingHoursBySalonId);
            this.cntrlThursday.DataBind(timeslots, openingHoursBySalonId);
            this.cntrlFriday.DataBind(timeslots, openingHoursBySalonId);
            this.cntrlSaturday.DataBind(timeslots, openingHoursBySalonId);
            this.cntrlSunday.DataBind(timeslots, openingHoursBySalonId);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("scheduling-readonly.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindScheduledTimes();
            }
        }

        protected void SchedulingLine_ItemDeleting(object sender, SchedulingEventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            ScheduleDB schedule = IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salon.SalonId).SingleOrDefault<ScheduleDB>(delegate (ScheduleDB item) {
                DayOfWeek? weekDayEnum = item.WeekDayEnum;
                DayOfWeek weekDay = e.Time.WeekDay;
                if (!((((DayOfWeek) weekDayEnum.GetValueOrDefault()) == weekDay) && weekDayEnum.HasValue))
                {
                    return false;
                }
                return item.Time == e.Time.Time;
            });
            if (schedule != null)
            {
                IoC.Resolve<ISchedulingManager>().DeleteSchedule(schedule);
            }
            string uRL = IFRMHelper.GetURL("scheduling.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void SchedulingLine_ItemInserting(object sender, SchedulingEventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            if ((from item in IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salon.SalonId)
                where item.ScheduleTypeEnum == ScheduleTypeEnum.TimeSlot
                select item).ToList<ScheduleDB>().SingleOrDefault<ScheduleDB>(delegate (ScheduleDB item) {
                DayOfWeek? weekDayEnum = item.WeekDayEnum;
                DayOfWeek weekDay = e.Time.WeekDay;
                if (!((((DayOfWeek) weekDayEnum.GetValueOrDefault()) == weekDay) && weekDayEnum.HasValue))
                {
                    return false;
                }
                return (item.Time == e.Time.Time);
            }) == null)
            {
                ScheduleDB schedule = new ScheduleDB {
                    SalonId = salon.SalonId,
                    ScheduleType = 1,
                    Slots = new int?(e.Time.Slots),
                    Time = new TimeSpan?(e.Time.Time),
                    WeekDay = new int?(((int) e.Time.WeekDay) + 1)
                };
                schedule = IoC.Resolve<ISchedulingManager>().InsertSchedule(schedule);
            }
            string uRL = IFRMHelper.GetURL("scheduling.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void SchedulingLine_ItemUpdating(object sender, SchedulingEventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            ScheduleDB schedule = IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salon.SalonId).SingleOrDefault<ScheduleDB>(delegate (ScheduleDB item) {
                DayOfWeek? weekDayEnum = item.WeekDayEnum;
                DayOfWeek weekDay = e.Time.WeekDay;
                if (!((((DayOfWeek) weekDayEnum.GetValueOrDefault()) == weekDay) && weekDayEnum.HasValue))
                {
                    return false;
                }
                return item.Time == e.Time.Time;
            });
            if (schedule != null)
            {
                schedule.Time = new TimeSpan?(e.Time.Time);
                schedule.Slots = new int?(e.Time.Slots);
                schedule = IoC.Resolve<ISchedulingManager>().UpdateSchedule(schedule);
            }
            string uRL = IFRMHelper.GetURL("scheduling.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }
    }
}


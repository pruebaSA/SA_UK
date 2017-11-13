namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Scheduling_ReadOnlyPage : IFRMSecurePage
    {
        protected Button btnEdit;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected SchedulingLineReadOnly cntrlFriday;
        protected SchedulingLineReadOnly cntrlMonday;
        protected SchedulingLineReadOnly cntrlSaturday;
        protected SchedulingLineReadOnly cntrlSunday;
        protected SchedulingLineReadOnly cntrlThursday;
        protected SchedulingLineReadOnly cntrlTuesday;
        protected SchedulingLineReadOnly cntrlWednesday;
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

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("scheduling.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindScheduledTimes();
            }
        }
    }
}


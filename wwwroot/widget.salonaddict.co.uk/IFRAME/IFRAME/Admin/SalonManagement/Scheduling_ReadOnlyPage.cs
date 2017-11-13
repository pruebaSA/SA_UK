namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Scheduling_ReadOnlyPage : IFRMAdminPage
    {
        protected Button btnEdit;
        protected SalonMenu cntlMenu;
        protected SchedulingLineReadOnly cntrlFriday;
        protected SchedulingLineReadOnly cntrlMonday;
        protected SchedulingLineReadOnly cntrlSaturday;
        protected SchedulingLineReadOnly cntrlSunday;
        protected SchedulingLineReadOnly cntrlThursday;
        protected SchedulingLineReadOnly cntrlTuesday;
        protected SchedulingLineReadOnly cntrlWednesday;
        protected Panel pnl;
        protected Panel pnlt;

        private void BindScheduledTimes(SalonDB salon)
        {
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            List<ScheduleDB> list = (from item in IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salon.SalonId)
                where item.ScheduleTypeEnum == ScheduleTypeEnum.TimeSlot
                select item).ToList<ScheduleDB>();
            (from item in IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salon.SalonId)
                where ((DayOfWeek) item.WeekDayEnum) == DayOfWeek.Sunday
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
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("scheduling.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string url = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindScheduledTimes(salonById);
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
    }
}


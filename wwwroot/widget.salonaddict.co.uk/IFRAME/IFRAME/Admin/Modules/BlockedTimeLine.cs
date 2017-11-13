namespace IFRAME.Admin.Modules
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.UI.WebControls;

    public class BlockedTimeLine : IFRMUserControl
    {
        private List<TimeBlockDB> _blocked;
        protected Label lblDay;
        protected ListView lv;
        protected MultiView mv;
        protected System.Web.UI.WebControls.View v0;
        protected System.Web.UI.WebControls.View v1;

        public event ItemEditingHandler ItemEditing;

        private void BindScheduledTimes(DateTime date, List<TimeSlotUI> timetable, List<TimeBlockDB> blocked, OpeningHoursDB hours, List<ClosingDayDB> holidays)
        {
            DayOfWeek dayOfWeek = date.DayOfWeek;
            this.lblDay.Text = date.ToString("ddd MMM dd");
            this.Date = date;
            this._blocked = (from item in blocked
                where item.Date == date
                select item).ToList<TimeBlockDB>();
            if (IFRMHelper.IsSalonClosed(date, hours, holidays))
            {
                this.mv.ActiveViewIndex = 1;
            }
            else
            {
                List<TimeSlotUI> list = (from item in timetable
                    where item.WeekDay == dayOfWeek
                    orderby item.Time
                    select item).ToList<TimeSlotUI>();
                this.lv.DataSource = list;
                this.lv.DataBind();
            }
        }

        public override void DataBind()
        {
            base.DataBind();
        }

        public void DataBind(DateTime date, List<TimeSlotUI> timetable, List<TimeBlockDB> blocked, OpeningHoursDB hours, List<ClosingDayDB> holidays)
        {
            this.BindScheduledTimes(date, timetable, blocked, hours, holidays);
            this.DataBind();
        }

        protected void lv_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                TimeSlotUI time;
                ListViewDataItem item = (ListViewDataItem) e.Item;
                if (item != null)
                {
                    time = item.DataItem as TimeSlotUI;
                    if (time != null)
                    {
                        LinkButton button = e.Item.FindControl("lb") as LinkButton;
                        if (button != null)
                        {
                            HiddenField field = e.Item.FindControl("hf") as HiddenField;
                            if (field != null)
                            {
                                TimeBlockDB kdb = this._blocked.FirstOrDefault<TimeBlockDB>(item => item.StartTime == time.Time);
                                if (kdb != null)
                                {
                                    field.Value = kdb.BlockId.ToString();
                                    button.CssClass = "lbutton-blocked-time-disabled";
                                }
                                else
                                {
                                    field.Value = Guid.Empty.ToString();
                                    button.CssClass = "lbutton-blocked-time";
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void lv_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            ListViewDataItem item = this.lv.Items[e.NewEditIndex];
            LinkButton button = item.FindControl("lb") as LinkButton;
            HiddenField field = item.FindControl("hf") as HiddenField;
            Guid guid = new Guid(field.Value);
            string str = new DateTime(TimeSpan.Parse(button.CommandArgument.ToString()).Ticks).ToString("HH:mm");
            BlockedTimeEventArgs args = new BlockedTimeEventArgs {
                BlockedTimeId = guid,
                Date = IFRMHelper.ToUrlFriendlyDate(this.Date),
                Time = str
            };
            if (this.ItemEditing != null)
            {
                this.ItemEditing(sender, args);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private DateTime Date
        {
            get => 
                IFRMHelper.FromUrlFriendlyDate(this.ViewState["Date"].ToString());
            set
            {
                this.ViewState["Date"] = IFRMHelper.ToUrlFriendlyDate(value);
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

        public delegate void ItemEditingHandler(object sender, BlockedTimeEventArgs e);
    }
}


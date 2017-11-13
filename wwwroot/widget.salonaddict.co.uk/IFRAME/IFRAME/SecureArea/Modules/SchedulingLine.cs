namespace IFRAME.SecureArea.Modules
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.UI.WebControls;

    public class SchedulingLine : IFRMUserControl
    {
        private System.DayOfWeek _dayOfWeek;
        protected DropDownList ddlSlots;
        protected DropDownList ddlTimes;
        protected LinkButton lbAdd;
        protected Label lblDay;
        protected ListView lv;
        protected MultiView mv;
        protected Panel pnlAdd;
        protected BalloonPopupExtender pnlAddEx;
        protected System.Web.UI.WebControls.View v0;
        protected System.Web.UI.WebControls.View v1;

        public event ItemDeletingHandler ItemDeleting;

        public event ItemInsertingHandler ItemInserting;

        public event ItemUpdatingHandler ItemUpdating;

        private void BindNewTimesList(List<TimeSlotUI> value, string openingHour, string closingHour)
        {
            DateTime time = IFRMHelper.FromUrlFriendlyTime(openingHour.Replace(":", "."));
            DateTime time2 = IFRMHelper.FromUrlFriendlyTime(closingHour.Replace(":", "."));
            ListItemCollection items = new ListItemCollection {
                new ListItem(base.GetLocaleResourceString("None"), string.Empty)
            };
            do
            {
                TimeSpan time = TimeSpan.Parse(time.ToString("HH:mm"));
                if (!value.Exists(item => item.Time == time))
                {
                    string text = time.ToString("HH:mm");
                    items.Add(new ListItem(text, time.ToString("HH:mm")));
                }
                time = time.AddMinutes(30.0);
            }
            while (time < time2);
            this.ddlTimes.DataSource = items;
            this.ddlTimes.DataTextField = "Text";
            this.ddlTimes.DataValueField = "Value";
            this.ddlTimes.DataBind();
        }

        private void BindScheduledTimes(List<TimeSlotUI> value, OpeningHoursDB hours)
        {
            System.DayOfWeek day = this._dayOfWeek;
            this.lblDay.Text = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int) day];
            if (IFRMHelper.IsSalonClosed(day, hours))
            {
                this.mv.ActiveViewIndex = 1;
            }
            else
            {
                List<TimeSlotUI> list = (from item in value
                    where item.WeekDay == day
                    orderby item.Time
                    select item).ToList<TimeSlotUI>();
                string salonOpeningHour = IFRMHelper.GetSalonOpeningHour(day, hours);
                string salonClosingHour = IFRMHelper.GetSalonClosingHour(day, hours);
                this.BindNewTimesList(list, salonOpeningHour, salonClosingHour);
                this.lv.DataSource = list;
                this.lv.DataBind();
            }
        }

        public override void DataBind()
        {
            base.DataBind();
        }

        public void DataBind(List<TimeSlotUI> value, OpeningHoursDB hours)
        {
            this.BindScheduledTimes(value, hours);
            this.DataBind();
        }

        protected void ddlSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.DayOfWeek week = this._dayOfWeek;
            TimeSlotUI tui = new TimeSlotUI {
                WeekDay = week,
                Slots = int.Parse(this.ddlSlots.SelectedValue),
                Time = TimeSpan.Parse(this.ddlTimes.SelectedValue)
            };
            SchedulingEventArgs args = new SchedulingEventArgs {
                Time = tui
            };
            if (this.ItemInserting != null)
            {
                this.ItemInserting(sender, args);
            }
        }

        protected void lv_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem item = (ListViewDataItem) e.Item;
                if (item != null)
                {
                    TimeSlotUI dataItem = item.DataItem as TimeSlotUI;
                    if (dataItem != null)
                    {
                        DropDownList list = item.FindControl("ddlSlots") as DropDownList;
                        if (list != null)
                        {
                            list.SelectedValue = dataItem.Slots.ToString();
                        }
                    }
                }
            }
        }

        protected void lv_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            System.DayOfWeek week = this._dayOfWeek;
            ListViewDataItem item = this.lv.Items[e.ItemIndex];
            Button button = item.FindControl("btnUpdate") as Button;
            DropDownList list = item.FindControl("ddlSlots") as DropDownList;
            TimeSlotUI tui = new TimeSlotUI {
                WeekDay = week,
                Slots = int.Parse(list.SelectedValue),
                Time = TimeSpan.Parse(button.CommandArgument.ToString())
            };
            SchedulingEventArgs args = new SchedulingEventArgs {
                Time = tui
            };
            if (this.ItemDeleting != null)
            {
                this.ItemDeleting(sender, args);
            }
        }

        protected void lv_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            System.DayOfWeek week = this._dayOfWeek;
            ListViewDataItem item = this.lv.Items[e.ItemIndex];
            Button button = item.FindControl("btnUpdate") as Button;
            DropDownList list = item.FindControl("ddlSlots") as DropDownList;
            TimeSlotUI tui = new TimeSlotUI {
                WeekDay = week,
                Slots = int.Parse(list.SelectedValue),
                Time = TimeSpan.Parse(button.CommandArgument.ToString())
            };
            SchedulingEventArgs args = new SchedulingEventArgs {
                Time = tui
            };
            if (this.ItemUpdating != null)
            {
                this.ItemUpdating(sender, args);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public System.DayOfWeek DayOfWeek
        {
            get => 
                this._dayOfWeek;
            set
            {
                this._dayOfWeek = value;
            }
        }

        public delegate void ItemDeletingHandler(object sender, SchedulingEventArgs e);

        public delegate void ItemInsertingHandler(object sender, SchedulingEventArgs e);

        public delegate void ItemUpdatingHandler(object sender, SchedulingEventArgs e);
    }
}


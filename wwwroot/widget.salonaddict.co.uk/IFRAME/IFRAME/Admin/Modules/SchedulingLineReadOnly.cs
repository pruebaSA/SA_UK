namespace IFRAME.Admin.Modules
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class SchedulingLineReadOnly : IFRMUserControl
    {
        private System.DayOfWeek _dayOfWeek;
        protected Label lblDay;
        protected ListView lv;
        protected MultiView mv;
        protected System.Web.UI.WebControls.View v0;
        protected System.Web.UI.WebControls.View v1;

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

        protected void lv_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem item = (ListViewDataItem) e.Item;
                if (item != null)
                {
                    TimeSlotUI dataItem = item.DataItem as TimeSlotUI;
                }
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
    }
}


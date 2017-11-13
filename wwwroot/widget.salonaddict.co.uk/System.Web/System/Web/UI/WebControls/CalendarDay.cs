namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CalendarDay
    {
        private DateTime date;
        private string dayNumberText;
        private bool isOtherMonth;
        private bool isSelectable;
        private bool isSelected;
        private bool isToday;
        private bool isWeekend;

        public CalendarDay(DateTime date, bool isWeekend, bool isToday, bool isSelected, bool isOtherMonth, string dayNumberText)
        {
            this.date = date;
            this.isWeekend = isWeekend;
            this.isToday = isToday;
            this.isOtherMonth = isOtherMonth;
            this.isSelected = isSelected;
            this.dayNumberText = dayNumberText;
        }

        public DateTime Date =>
            this.date;

        public string DayNumberText =>
            this.dayNumberText;

        public bool IsOtherMonth =>
            this.isOtherMonth;

        public bool IsSelectable
        {
            get => 
                this.isSelectable;
            set
            {
                this.isSelectable = value;
            }
        }

        public bool IsSelected =>
            this.isSelected;

        public bool IsToday =>
            this.isToday;

        public bool IsWeekend =>
            this.isWeekend;
    }
}


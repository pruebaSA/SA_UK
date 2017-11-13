namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class MonthChangedEventArgs
    {
        private DateTime newDate;
        private DateTime previousDate;

        public MonthChangedEventArgs(DateTime newDate, DateTime previousDate)
        {
            this.newDate = newDate;
            this.previousDate = previousDate;
        }

        public DateTime NewDate =>
            this.newDate;

        public DateTime PreviousDate =>
            this.previousDate;
    }
}


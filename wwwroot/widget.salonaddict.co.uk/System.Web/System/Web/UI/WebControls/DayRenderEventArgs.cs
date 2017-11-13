namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DayRenderEventArgs
    {
        private TableCell cell;
        private CalendarDay day;
        private string selectUrl;

        public DayRenderEventArgs(TableCell cell, CalendarDay day)
        {
            this.day = day;
            this.cell = cell;
        }

        public DayRenderEventArgs(TableCell cell, CalendarDay day, string selectUrl)
        {
            this.day = day;
            this.cell = cell;
            this.selectUrl = selectUrl;
        }

        public TableCell Cell =>
            this.cell;

        public CalendarDay Day =>
            this.day;

        public string SelectUrl =>
            this.selectUrl;
    }
}


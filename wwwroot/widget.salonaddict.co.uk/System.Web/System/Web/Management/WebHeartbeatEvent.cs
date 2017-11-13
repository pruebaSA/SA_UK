namespace System.Web.Management
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebHeartbeatEvent : WebManagementEvent
    {
        private static WebProcessStatistics s_procStats = new WebProcessStatistics();

        internal WebHeartbeatEvent()
        {
        }

        protected internal WebHeartbeatEvent(string message, int eventCode) : base(message, null, eventCode)
        {
        }

        internal override void FormatToString(WebEventFormatter formatter, bool includeAppInfo)
        {
            base.FormatToString(formatter, includeAppInfo);
            formatter.AppendLine(string.Empty);
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_process_statistics"));
            formatter.IndentationLevel++;
            s_procStats.FormatToString(formatter);
            formatter.IndentationLevel--;
        }

        public WebProcessStatistics ProcessStatistics =>
            s_procStats;
    }
}


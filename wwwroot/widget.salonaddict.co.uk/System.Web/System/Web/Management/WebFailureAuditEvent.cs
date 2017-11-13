namespace System.Web.Management
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebFailureAuditEvent : WebAuditEvent
    {
        internal WebFailureAuditEvent()
        {
        }

        protected internal WebFailureAuditEvent(string message, object eventSource, int eventCode) : base(message, eventSource, eventCode)
        {
        }

        protected internal WebFailureAuditEvent(string message, object eventSource, int eventCode, int eventDetailCode) : base(message, eventSource, eventCode, eventDetailCode)
        {
        }

        protected internal override void IncrementPerfCounters()
        {
            base.IncrementPerfCounters();
            PerfCounters.IncrementCounter(AppPerfCounter.AUDIT_FAIL);
            PerfCounters.IncrementGlobalCounter(GlobalPerfCounter.GLOBAL_AUDIT_FAIL);
        }
    }
}


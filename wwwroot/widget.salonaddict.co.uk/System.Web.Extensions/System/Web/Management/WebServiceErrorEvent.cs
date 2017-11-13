namespace System.Web.Management
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebServiceErrorEvent : WebRequestErrorEvent
    {
        private const int _webServiceErrorEventCode = 0x186a1;

        protected internal WebServiceErrorEvent(string message, object eventSource, Exception exception) : base(message, eventSource, WebServiceErrorEventCode, exception)
        {
        }

        public static int WebServiceErrorEventCode =>
            0x186a1;
    }
}


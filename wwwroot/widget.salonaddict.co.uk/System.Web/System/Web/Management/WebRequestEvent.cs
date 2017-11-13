﻿namespace System.Web.Management
{
    using System;
    using System.Collections.Generic;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebRequestEvent : WebManagementEvent
    {
        private WebRequestInformation _requestInfo;

        internal WebRequestEvent()
        {
        }

        protected internal WebRequestEvent(string message, object eventSource, int eventCode) : base(message, eventSource, eventCode)
        {
        }

        protected internal WebRequestEvent(string message, object eventSource, int eventCode, int eventDetailCode) : base(message, eventSource, eventCode, eventDetailCode)
        {
        }

        internal override void FormatToString(WebEventFormatter formatter, bool includeAppInfo)
        {
            base.FormatToString(formatter, includeAppInfo);
            formatter.AppendLine(string.Empty);
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_request_information"));
            formatter.IndentationLevel++;
            this.RequestInformation.FormatToString(formatter);
            formatter.IndentationLevel--;
        }

        internal override void GenerateFieldsForMarshal(List<WebEventFieldData> fields)
        {
            base.GenerateFieldsForMarshal(fields);
            fields.Add(new WebEventFieldData("RequestUrl", this.RequestInformation.RequestUrl, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("RequestPath", this.RequestInformation.RequestPath, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("UserHostAddress", this.RequestInformation.UserHostAddress, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("UserName", this.RequestInformation.Principal.Identity.Name, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("UserAuthenticated", this.RequestInformation.Principal.Identity.IsAuthenticated.ToString(), WebEventFieldType.Bool));
            fields.Add(new WebEventFieldData("UserAuthenticationType", this.RequestInformation.Principal.Identity.AuthenticationType, WebEventFieldType.String));
            fields.Add(new WebEventFieldData("RequestThreadAccountName", this.RequestInformation.ThreadAccountName, WebEventFieldType.String));
        }

        protected internal override void IncrementPerfCounters()
        {
            base.IncrementPerfCounters();
            PerfCounters.IncrementCounter(AppPerfCounter.EVENTS_WEB_REQ);
        }

        private void InitRequestInformation()
        {
            if (this._requestInfo == null)
            {
                this._requestInfo = new WebRequestInformation();
            }
        }

        internal override void PreProcessEventInit()
        {
            base.PreProcessEventInit();
            this.InitRequestInformation();
        }

        public WebRequestInformation RequestInformation
        {
            get
            {
                this.InitRequestInformation();
                return this._requestInfo;
            }
        }
    }
}


﻿namespace System.Web.Management
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebApplicationInformation
    {
        private string _appDomain = Thread.GetDomain().FriendlyName;
        private string _appPath;
        private string _appUrl = HttpRuntime.AppDomainAppVirtualPath;
        private string _machineName;
        private string _trustLevel = HttpRuntime.TrustLevel;

        internal WebApplicationInformation()
        {
            try
            {
                this._appPath = HttpRuntime.AppDomainAppPath;
            }
            catch
            {
                this._appPath = null;
            }
            this._machineName = this.GetMachineNameWithAssert();
        }

        public void FormatToString(WebEventFormatter formatter)
        {
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_application_domain", this.ApplicationDomain));
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_trust_level", this.TrustLevel));
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_application_virtual_path", this.ApplicationVirtualPath));
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_application_path", this.ApplicationPath));
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_machine_name", this.MachineName));
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted=true)]
        private string GetMachineNameWithAssert() => 
            Environment.MachineName;

        public override string ToString()
        {
            WebEventFormatter formatter = new WebEventFormatter();
            this.FormatToString(formatter);
            return formatter.ToString();
        }

        public string ApplicationDomain =>
            this._appDomain;

        public string ApplicationPath =>
            this._appPath;

        public string ApplicationVirtualPath =>
            this._appUrl;

        public string MachineName =>
            this._machineName;

        public string TrustLevel =>
            this._trustLevel;
    }
}


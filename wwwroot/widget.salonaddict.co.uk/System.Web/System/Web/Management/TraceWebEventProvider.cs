﻿namespace System.Web.Management
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TraceWebEventProvider : WebEventProvider, IInternalWebEventProvider
    {
        internal TraceWebEventProvider()
        {
        }

        public override void Flush()
        {
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            ProviderUtil.CheckUnrecognizedAttributes(config, name);
        }

        public override void ProcessEvent(WebBaseEvent eventRaised)
        {
            if (eventRaised is WebBaseErrorEvent)
            {
                Trace.TraceError(eventRaised.ToString());
            }
            else
            {
                Trace.TraceInformation(eventRaised.ToString());
            }
        }

        public override void Shutdown()
        {
        }
    }
}


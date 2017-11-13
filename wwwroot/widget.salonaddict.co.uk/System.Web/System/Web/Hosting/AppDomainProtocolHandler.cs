﻿namespace System.Web.Hosting
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class AppDomainProtocolHandler : MarshalByRefObject, IRegisteredObject
    {
        protected AppDomainProtocolHandler()
        {
            HostingEnvironment.RegisterObject(this);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService() => 
            null;

        public abstract void StartListenerChannel(IListenerChannelCallback listenerChannelCallback);
        public virtual void Stop(bool immediate)
        {
            this.StopProtocol(true);
            HostingEnvironment.UnregisterObject(this);
        }

        public abstract void StopListenerChannel(int listenerChannelId, bool immediate);
        public abstract void StopProtocol(bool immediate);
    }
}


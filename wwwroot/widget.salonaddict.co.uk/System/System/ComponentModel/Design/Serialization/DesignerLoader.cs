﻿namespace System.ComponentModel.Design.Serialization
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public abstract class DesignerLoader
    {
        protected DesignerLoader()
        {
        }

        public abstract void BeginLoad(IDesignerLoaderHost host);
        public abstract void Dispose();
        public virtual void Flush()
        {
        }

        public virtual bool Loading =>
            false;
    }
}


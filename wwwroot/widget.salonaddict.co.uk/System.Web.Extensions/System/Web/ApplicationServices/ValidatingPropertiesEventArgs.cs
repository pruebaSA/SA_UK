namespace System.Web.ApplicationServices
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ValidatingPropertiesEventArgs : EventArgs
    {
        private Collection<string> _failedProperties;
        private IDictionary<string, object> _properties;

        internal ValidatingPropertiesEventArgs()
        {
        }

        internal ValidatingPropertiesEventArgs(IDictionary<string, object> properties)
        {
            this._properties = properties;
            this._failedProperties = new Collection<string>();
        }

        public Collection<string> FailedProperties =>
            this._failedProperties;

        public IDictionary<string, object> Properties =>
            this._properties;
    }
}


﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SqlDataSourceFilteringEventArgs : CancelEventArgs
    {
        private IOrderedDictionary _parameterValues;

        public SqlDataSourceFilteringEventArgs(IOrderedDictionary parameterValues)
        {
            this._parameterValues = parameterValues;
        }

        public IOrderedDictionary ParameterValues =>
            this._parameterValues;
    }
}


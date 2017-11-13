﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AccessDataSourceView : SqlDataSourceView
    {
        private AccessDataSource _owner;

        public AccessDataSourceView(AccessDataSource owner, string name, HttpContext context) : base(owner, name, context)
        {
            this._owner = owner;
        }

        protected internal override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            if (string.IsNullOrEmpty(this._owner.DataFile))
            {
                throw new InvalidOperationException(System.Web.SR.GetString("AccessDataSourceView_SelectRequiresDataFile", new object[] { this._owner.ID }));
            }
            return base.ExecuteSelect(arguments);
        }
    }
}


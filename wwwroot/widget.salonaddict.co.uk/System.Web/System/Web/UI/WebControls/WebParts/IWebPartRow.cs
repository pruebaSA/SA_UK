﻿namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IWebPartRow
    {
        void GetRowData(RowCallback callback);

        PropertyDescriptorCollection Schema { get; }
    }
}


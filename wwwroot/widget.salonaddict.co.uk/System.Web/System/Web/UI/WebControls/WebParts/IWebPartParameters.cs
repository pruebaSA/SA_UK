namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IWebPartParameters
    {
        void GetParametersData(ParametersCallback callback);
        void SetConsumerSchema(PropertyDescriptorCollection schema);

        PropertyDescriptorCollection Schema { get; }
    }
}


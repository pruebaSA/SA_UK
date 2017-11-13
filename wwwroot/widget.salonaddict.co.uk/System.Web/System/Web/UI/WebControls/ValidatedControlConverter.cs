namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ValidatedControlConverter : ControlIDConverter
    {
        protected override bool FilterControl(Control control)
        {
            ValidationPropertyAttribute attribute = (ValidationPropertyAttribute) TypeDescriptor.GetAttributes(control)[typeof(ValidationPropertyAttribute)];
            return ((attribute != null) && (attribute.Name != null));
        }
    }
}


namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class TemplatePropertyEntry : BuilderPropertyEntry
    {
        private bool _bindableTemplate;

        internal TemplatePropertyEntry()
        {
        }

        internal TemplatePropertyEntry(bool bindableTemplate)
        {
            this._bindableTemplate = bindableTemplate;
        }

        public bool BindableTemplate =>
            this._bindableTemplate;
    }
}


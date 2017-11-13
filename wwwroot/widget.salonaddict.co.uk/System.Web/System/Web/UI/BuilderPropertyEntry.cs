namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class BuilderPropertyEntry : PropertyEntry
    {
        private ControlBuilder _builder;

        internal BuilderPropertyEntry()
        {
        }

        public ControlBuilder Builder
        {
            get => 
                this._builder;
            set
            {
                this._builder = value;
            }
        }
    }
}


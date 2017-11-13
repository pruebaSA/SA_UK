namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class UpdatePanelTrigger
    {
        private UpdatePanel _owner;

        protected UpdatePanelTrigger()
        {
        }

        protected internal abstract bool HasTriggered();
        protected internal virtual void Initialize()
        {
        }

        internal void SetOwner(UpdatePanel owner)
        {
            this._owner = owner;
        }

        [Browsable(false)]
        public UpdatePanel Owner =>
            this._owner;
    }
}


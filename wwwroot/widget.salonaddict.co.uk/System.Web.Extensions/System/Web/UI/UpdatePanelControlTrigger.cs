namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class UpdatePanelControlTrigger : UpdatePanelTrigger
    {
        private string _controlID;

        protected UpdatePanelControlTrigger()
        {
        }

        protected Control FindTargetControl(bool searchNamingContainers)
        {
            if (string.IsNullOrEmpty(this.ControlID))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanelControlTrigger_NoControlID, new object[] { base.Owner.ID }));
            }
            Control control = ControlUtil.FindTargetControl(this.ControlID, base.Owner, searchNamingContainers);
            if (control == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanelControlTrigger_ControlNotFound, new object[] { this.ControlID, base.Owner.ID }));
            }
            return control;
        }

        [Category("Behavior"), DefaultValue(""), IDReferenceProperty, ResourceDescription("UpdatePanelControlTrigger_ControlID")]
        public string ControlID
        {
            get => 
                (this._controlID ?? string.Empty);
            set
            {
                this._controlID = value;
            }
        }
    }
}


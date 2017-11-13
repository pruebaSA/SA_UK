namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class PostBackTrigger : UpdatePanelControlTrigger
    {
        private IScriptManagerInternal _scriptManager;

        public PostBackTrigger()
        {
        }

        internal PostBackTrigger(IScriptManagerInternal scriptManager)
        {
            this._scriptManager = scriptManager;
        }

        protected internal override bool HasTriggered() => 
            false;

        protected internal override void Initialize()
        {
            base.Initialize();
            Control control = base.FindTargetControl(false);
            this.ScriptManager.RegisterPostBackControl(control);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.ControlID))
            {
                return "PostBack";
            }
            return ("PostBack: " + this.ControlID);
        }

        [TypeConverter("System.Web.UI.Design.PostBackTriggerControlIDConverter, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
        public string ControlID
        {
            get => 
                base.ControlID;
            set
            {
                base.ControlID = value;
            }
        }

        internal IScriptManagerInternal ScriptManager
        {
            get
            {
                if (this._scriptManager == null)
                {
                    Page page = base.Owner.Page;
                    if (page == null)
                    {
                        throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                    }
                    this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(page);
                    if (this._scriptManager == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ScriptManagerRequired, new object[] { base.Owner.ID }));
                    }
                }
                return this._scriptManager;
            }
        }
    }
}


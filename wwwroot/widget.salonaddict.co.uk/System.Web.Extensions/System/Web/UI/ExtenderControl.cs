namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [ToolboxItem("System.Web.UI.Design.ExtenderControlToolboxItem, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ParseChildren(true), PersistChildren(false), DefaultProperty("TargetControlID"), Designer("System.Web.UI.Design.ExtenderControlDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), NonVisualControl, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class ExtenderControl : Control, IExtenderControl
    {
        private System.Web.UI.IPage _page;
        private IScriptManagerInternal _scriptManager;
        private string _targetControlID;

        protected ExtenderControl()
        {
        }

        internal ExtenderControl(IScriptManagerInternal scriptManager, System.Web.UI.IPage page)
        {
            this._scriptManager = scriptManager;
            this._page = page;
        }

        private static UpdatePanel FindUpdatePanel(Control control)
        {
            for (Control control2 = control.Parent; control2 != null; control2 = control2.Parent)
            {
                UpdatePanel panel = control2 as UpdatePanel;
                if (panel != null)
                {
                    return panel;
                }
            }
            return null;
        }

        protected abstract IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl);
        protected abstract IEnumerable<ScriptReference> GetScriptReferences();
        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.RegisterWithScriptManager();
        }

        private void RegisterWithScriptManager()
        {
            if (string.IsNullOrEmpty(this.TargetControlID))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ExtenderControl_TargetControlIDEmpty, new object[] { this.ID }));
            }
            Control control = this.FindControl(this.TargetControlID);
            if (control == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ExtenderControl_TargetControlIDInvalid, new object[] { this.ID, this.TargetControlID }));
            }
            if (FindUpdatePanel(this) != FindUpdatePanel(control))
            {
                throw new InvalidOperationException(AtlasWeb.ExtenderControl_TargetControlDifferentUpdatePanel);
            }
            this.ScriptManager.RegisterExtenderControl<ExtenderControl>(this, control);
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            this.IPage.VerifyRenderingInServerForm(this);
            if (!base.DesignMode)
            {
                this.ScriptManager.RegisterScriptDescriptors(this);
            }
        }

        IEnumerable<ScriptDescriptor> IExtenderControl.GetScriptDescriptors(Control targetControl) => 
            this.GetScriptDescriptors(targetControl);

        IEnumerable<ScriptReference> IExtenderControl.GetScriptReferences() => 
            this.GetScriptReferences();

        private System.Web.UI.IPage IPage
        {
            get
            {
                if (this._page != null)
                {
                    return this._page;
                }
                Page page = this.Page;
                if (page == null)
                {
                    throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                }
                return new PageWrapper(page);
            }
        }

        private IScriptManagerInternal ScriptManager
        {
            get
            {
                if (this._scriptManager == null)
                {
                    Page page = this.Page;
                    if (page == null)
                    {
                        throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                    }
                    this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(page);
                    if (this._scriptManager == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ScriptManagerRequired, new object[] { this.ID }));
                    }
                }
                return this._scriptManager;
            }
        }

        [DefaultValue(""), Category("Behavior"), IDReferenceProperty, ResourceDescription("ExtenderControl_TargetControlID")]
        public string TargetControlID
        {
            get
            {
                if (this._targetControlID != null)
                {
                    return this._targetControlID;
                }
                return string.Empty;
            }
            set
            {
                this._targetControlID = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override bool Visible
        {
            get => 
                base.Visible;
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}


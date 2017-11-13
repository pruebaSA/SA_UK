namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;
    using System.Web.UI.WebControls;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class ScriptControl : WebControl, IScriptControl
    {
        private System.Web.UI.IPage _page;
        private IScriptManagerInternal _scriptManager;

        protected ScriptControl()
        {
        }

        internal ScriptControl(IScriptManagerInternal scriptManager, System.Web.UI.IPage page)
        {
            this._scriptManager = scriptManager;
            this._page = page;
        }

        protected abstract IEnumerable<ScriptDescriptor> GetScriptDescriptors();
        protected abstract IEnumerable<ScriptReference> GetScriptReferences();
        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.ScriptManager.RegisterScriptControl<ScriptControl>(this);
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

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors() => 
            this.GetScriptDescriptors();

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences() => 
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
    }
}


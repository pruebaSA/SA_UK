namespace AjaxControlToolkit.MaskedEditValidatorCompatibility
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class BaseValidator : System.Web.UI.WebControls.BaseValidator, IBaseValidatorAccessor, IWebControlAccessor
    {
        private System.Web.UI.ScriptManager _scriptManager;
        private bool _scriptManagerChecked;

        protected BaseValidator()
        {
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if ((this.ScriptManager == null) || !this.ScriptManager.SupportsPartialRendering)
            {
                base.AddAttributesToRender(writer);
            }
            else
            {
                ValidatorHelper.DoBaseValidatorAddAttributes(this, this, writer);
            }
        }

        void IBaseValidatorAccessor.EnsureID()
        {
            base.EnsureID();
        }

        string IBaseValidatorAccessor.GetControlRenderID(string name) => 
            base.GetControlRenderID(name);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if ((this.ScriptManager != null) && this.ScriptManager.SupportsPartialRendering)
            {
                ValidatorHelper.DoInitRegistration(this.Page);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if ((this.ScriptManager != null) && this.ScriptManager.SupportsPartialRendering)
            {
                ValidatorHelper.DoPreRenderRegistration(this, this);
            }
        }

        protected override void RegisterValidatorDeclaration()
        {
            if ((this.ScriptManager == null) || !this.ScriptManager.SupportsPartialRendering)
            {
                base.RegisterValidatorDeclaration();
            }
            else
            {
                ValidatorHelper.DoValidatorArrayDeclaration(this, typeof(AjaxControlToolkit.MaskedEditValidatorCompatibility.BaseValidator));
            }
        }

        bool IBaseValidatorAccessor.RenderUpLevel =>
            base.RenderUplevel;

        HtmlTextWriterTag IWebControlAccessor.TagKey =>
            this.TagKey;

        internal System.Web.UI.ScriptManager ScriptManager
        {
            get
            {
                if (!this._scriptManagerChecked)
                {
                    this._scriptManagerChecked = true;
                    Page page = this.Page;
                    if (page != null)
                    {
                        this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(page);
                    }
                }
                return this._scriptManager;
            }
        }
    }
}


namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.UI.HtmlControls;

    internal sealed class PageWrapper : IPage
    {
        private readonly Page _page;

        event EventHandler IPage.Error
        {
            add
            {
                this._page.Error += value;
            }
            remove
            {
                this._page.Error -= value;
            }
        }

        event EventHandler IPage.InitComplete
        {
            add
            {
                this._page.InitComplete += value;
            }
            remove
            {
                this._page.InitComplete -= value;
            }
        }

        event EventHandler IPage.LoadComplete
        {
            add
            {
                this._page.LoadComplete += value;
            }
            remove
            {
                this._page.LoadComplete -= value;
            }
        }

        event EventHandler IPage.PreRender
        {
            add
            {
                this._page.PreRender += value;
            }
            remove
            {
                this._page.PreRender -= value;
            }
        }

        event EventHandler IPage.PreRenderComplete
        {
            add
            {
                this._page.PreRenderComplete += value;
            }
            remove
            {
                this._page.PreRenderComplete -= value;
            }
        }

        public PageWrapper(Page page)
        {
            this._page = page;
        }

        void IPage.RegisterRequiresViewStateEncryption()
        {
            this._page.RegisterRequiresViewStateEncryption();
        }

        void IPage.SetFocus(string clientID)
        {
            this._page.SetFocus(clientID);
        }

        void IPage.SetFocus(Control control)
        {
            this._page.SetFocus(control);
        }

        void IPage.SetRenderMethodDelegate(RenderMethod renderMethod)
        {
            this._page.SetRenderMethodDelegate(renderMethod);
        }

        void IPage.Validate(string validationGroup)
        {
            this._page.Validate(validationGroup);
        }

        void IPage.VerifyRenderingInServerForm(Control control)
        {
            this._page.VerifyRenderingInServerForm(control);
        }

        string IPage.AppRelativeVirtualPath =>
            this._page.AppRelativeVirtualPath;

        IClientScriptManager IPage.ClientScript =>
            new ClientScriptManagerWrapper(this._page.ClientScript);

        bool IPage.EnableEventValidation =>
            this._page.EnableEventValidation;

        IHtmlForm IPage.Form
        {
            get
            {
                if (this._page.Form != null)
                {
                    return new HtmlFormWrapper(this._page.Form);
                }
                return null;
            }
        }

        HtmlHead IPage.Header =>
            this._page.Header;

        bool IPage.IsPostBack =>
            this._page.IsPostBack;

        bool IPage.IsValid =>
            this._page.IsValid;

        IDictionary IPage.Items =>
            this._page.Items;

        HttpRequestBase IPage.Request =>
            new HttpRequestWrapper(this._page.Request);

        HttpResponseInternalBase IPage.Response =>
            new HttpResponseInternalWrapper(this._page.Response);

        HttpServerUtilityBase IPage.Server =>
            new HttpServerUtilityWrapper(this._page.Server);

        string IPage.Title =>
            this._page.Title;
    }
}


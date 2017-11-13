namespace System.Web.UI
{
    using System;
    using System.Web.UI.HtmlControls;

    internal sealed class HtmlFormWrapper : IHtmlForm
    {
        private HtmlForm _form;

        public HtmlFormWrapper(HtmlForm form)
        {
            this._form = form;
        }

        void IHtmlForm.RenderControl(HtmlTextWriter writer)
        {
            this._form.RenderControl(writer);
        }

        void IHtmlForm.SetRenderMethodDelegate(RenderMethod renderMethod)
        {
            this._form.SetRenderMethodDelegate(renderMethod);
        }

        string IHtmlForm.ClientID =>
            this._form.ClientID;

        string IHtmlForm.Method =>
            this._form.Method;
    }
}


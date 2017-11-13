namespace System.Web.UI
{
    using System;

    internal interface IHtmlForm
    {
        void RenderControl(HtmlTextWriter writer);
        void SetRenderMethodDelegate(RenderMethod renderMethod);

        string ClientID { get; }

        string Method { get; }
    }
}


namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.UI.HtmlControls;

    internal interface IPage
    {
        event EventHandler Error;

        event EventHandler InitComplete;

        event EventHandler LoadComplete;

        event EventHandler PreRender;

        event EventHandler PreRenderComplete;

        void RegisterRequiresViewStateEncryption();
        void SetFocus(string clientID);
        void SetFocus(Control control);
        void SetRenderMethodDelegate(RenderMethod renderMethod);
        void Validate(string validationGroup);
        void VerifyRenderingInServerForm(Control control);

        string AppRelativeVirtualPath { get; }

        IClientScriptManager ClientScript { get; }

        bool EnableEventValidation { get; }

        IHtmlForm Form { get; }

        HtmlHead Header { get; }

        bool IsPostBack { get; }

        bool IsValid { get; }

        IDictionary Items { get; }

        HttpRequestBase Request { get; }

        HttpResponseInternalBase Response { get; }

        HttpServerUtilityBase Server { get; }

        string Title { get; }
    }
}


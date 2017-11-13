namespace System.Web.UI
{
    using System;

    internal interface IScriptManager
    {
        void RegisterArrayDeclaration(Control control, string arrayName, string arrayValue);
        void RegisterClientScriptBlock(Control control, Type type, string key, string script, bool addScriptTags);
        void RegisterClientScriptInclude(Control control, Type type, string key, string url);
        void RegisterClientScriptResource(Control control, Type type, string resourceName);
        void RegisterDispose(Control control, string disposeScript);
        void RegisterExpandoAttribute(Control control, string controlId, string attributeName, string attributeValue, bool encode);
        void RegisterHiddenField(Control control, string hiddenFieldName, string hiddenFieldValue);
        void RegisterOnSubmitStatement(Control control, Type type, string key, string script);
        void RegisterPostBackControl(Control control);
        void RegisterStartupScript(Control control, Type type, string key, string script, bool addScriptTags);
        void SetFocusInternal(string clientID);

        bool IsInAsyncPostBack { get; }

        bool SupportsPartialRendering { get; }
    }
}


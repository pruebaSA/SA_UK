namespace System.Web.UI
{
    using System;

    internal interface IClientScriptManager
    {
        string GetPostBackEventReference(PostBackOptions options);
        string GetWebResourceUrl(Type type, string resourceName);
        void RegisterClientScriptBlock(Type type, string key, string script);
        void RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTags);
        void RegisterClientScriptInclude(Type type, string key, string url);
        void RegisterStartupScript(Type type, string key, string script, bool addScriptTags);
    }
}


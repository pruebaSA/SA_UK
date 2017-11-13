namespace System.Web.UI
{
    using System;

    internal sealed class ClientScriptManagerWrapper : IClientScriptManager
    {
        private readonly ClientScriptManager _clientScriptManager;

        internal ClientScriptManagerWrapper(ClientScriptManager clientScriptManager)
        {
            this._clientScriptManager = clientScriptManager;
        }

        string IClientScriptManager.GetPostBackEventReference(PostBackOptions options) => 
            this._clientScriptManager.GetPostBackEventReference(options);

        string IClientScriptManager.GetWebResourceUrl(Type type, string resourceName) => 
            this._clientScriptManager.GetWebResourceUrl(type, resourceName);

        void IClientScriptManager.RegisterClientScriptBlock(Type type, string key, string script)
        {
            this._clientScriptManager.RegisterClientScriptBlock(type, key, script);
        }

        void IClientScriptManager.RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTags)
        {
            this._clientScriptManager.RegisterClientScriptBlock(type, key, script, addScriptTags);
        }

        void IClientScriptManager.RegisterClientScriptInclude(Type type, string key, string url)
        {
            this._clientScriptManager.RegisterClientScriptInclude(type, key, url);
        }

        void IClientScriptManager.RegisterStartupScript(Type type, string key, string script, bool addScriptTags)
        {
            this._clientScriptManager.RegisterStartupScript(type, key, script, addScriptTags);
        }
    }
}


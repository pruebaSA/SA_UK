namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RegisteredScript
    {
        private bool _addScriptTags;
        private System.Web.UI.Control _control;
        private string _key;
        private string _script;
        private RegisteredScriptType _scriptType;
        private System.Type _type;
        private string _url;

        internal RegisteredScript(System.Web.UI.Control control, System.Type type, string key, string url)
        {
            this._scriptType = RegisteredScriptType.ClientScriptInclude;
            this._control = control;
            this._type = type;
            this._key = key;
            this._url = url;
        }

        internal RegisteredScript(RegisteredScriptType scriptType, System.Web.UI.Control control, System.Type type, string key, string script, bool addScriptTags)
        {
            this._scriptType = scriptType;
            this._control = control;
            this._type = type;
            this._key = key;
            this._script = script;
            this._addScriptTags = addScriptTags;
        }

        public bool AddScriptTags =>
            this._addScriptTags;

        public System.Web.UI.Control Control =>
            this._control;

        public string Key =>
            this._key;

        public string Script =>
            this._script;

        public RegisteredScriptType ScriptType =>
            this._scriptType;

        public System.Type Type =>
            this._type;

        public string Url =>
            this._url;
    }
}


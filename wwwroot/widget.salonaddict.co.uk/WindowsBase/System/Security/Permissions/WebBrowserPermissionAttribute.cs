namespace System.Security.Permissions
{
    using System;
    using System.Security;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
    public sealed class WebBrowserPermissionAttribute : CodeAccessSecurityAttribute
    {
        private WebBrowserPermissionLevel _webBrowserPermissionLevel;

        public WebBrowserPermissionAttribute(SecurityAction action) : base(action)
        {
        }

        public override IPermission CreatePermission()
        {
            if (base.Unrestricted)
            {
                return new WebBrowserPermission(PermissionState.Unrestricted);
            }
            return new WebBrowserPermission(this._webBrowserPermissionLevel);
        }

        public WebBrowserPermissionLevel Level
        {
            get => 
                this._webBrowserPermissionLevel;
            set
            {
                WebBrowserPermission.VerifyWebBrowserPermissionLevel(value);
                this._webBrowserPermissionLevel = value;
            }
        }
    }
}


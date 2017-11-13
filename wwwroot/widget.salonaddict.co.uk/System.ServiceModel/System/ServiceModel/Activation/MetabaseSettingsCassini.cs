namespace System.ServiceModel.Activation
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;

    internal class MetabaseSettingsCassini : MetabaseSettings
    {
        internal MetabaseSettingsCassini(HostedHttpRequestAsyncResult result)
        {
            if (!ServiceHostingEnvironment.IsSimpleApplicationHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(true);
            }
            string str = string.Format(CultureInfo.InvariantCulture, ":{0}:{1}", new object[] { result.OriginalRequestUri.Port.ToString(NumberFormatInfo.InvariantInfo), "localhost" });
            base.Bindings.Add(result.OriginalRequestUri.Scheme, new string[] { str });
            base.Protocols.Add(result.OriginalRequestUri.Scheme);
        }

        internal override HttpAccessSslFlags GetAccessSslFlags(string virtualPath) => 
            HttpAccessSslFlags.None;

        internal override AuthenticationSchemes GetAuthenticationSchemes(string virtualPath) => 
            (AuthenticationSchemes.Anonymous | AuthenticationSchemes.Ntlm);

        internal override ExtendedProtectionPolicy GetExtendedProtectionPolicy(string virtualPath) => 
            null;

        internal override string GetRealm(string virtualPath) => 
            string.Empty;
    }
}


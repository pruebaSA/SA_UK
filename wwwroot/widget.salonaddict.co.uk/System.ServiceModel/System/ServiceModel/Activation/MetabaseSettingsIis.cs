namespace System.ServiceModel.Activation
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;
    using System.Web;

    internal abstract class MetabaseSettingsIis : MetabaseSettings
    {
        internal static string[] DefaultAuthProviders = new string[] { "negotiate", "ntlm" };
        internal const string NegotiateAuthProvider = "negotiate";
        internal const string NtlmAuthProvider = "ntlm";
        private IDictionary<string, HostedServiceTransportSettings> transportSettingsTable;

        protected MetabaseSettingsIis()
        {
            if (ServiceHostingEnvironment.IsSimpleApplicationHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(true);
            }
            this.transportSettingsTable = new Dictionary<string, HostedServiceTransportSettings>(StringComparer.OrdinalIgnoreCase);
        }

        protected abstract HostedServiceTransportSettings CreateTransportSettings(string relativeVirtualPath);
        internal override HttpAccessSslFlags GetAccessSslFlags(string virtualPath) => 
            this.GetTransportSettings(virtualPath).AccessSslFlags;

        internal override AuthenticationSchemes GetAuthenticationSchemes(string virtualPath)
        {
            HostedServiceTransportSettings transportSettings = this.GetTransportSettings(virtualPath);
            return this.RemapAuthenticationSchemes(transportSettings.AuthFlags, transportSettings.AuthProviders);
        }

        internal override ExtendedProtectionPolicy GetExtendedProtectionPolicy(string virtualPath) => 
            this.GetTransportSettings(virtualPath).IisExtendedProtectionPolicy;

        internal override string GetRealm(string virtualPath) => 
            this.GetTransportSettings(virtualPath).Realm;

        private HostedServiceTransportSettings GetTransportSettings(string virtualPath)
        {
            HostedServiceTransportSettings settings;
            string key = VirtualPathUtility.ToAppRelative(virtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath);
            if (!this.transportSettingsTable.TryGetValue(key, out settings))
            {
                lock (this.ThisLock)
                {
                    if (!this.transportSettingsTable.TryGetValue(key, out settings))
                    {
                        settings = this.CreateTransportSettings(key);
                        this.transportSettingsTable.Add(key, settings);
                    }
                }
            }
            return settings;
        }

        private AuthenticationSchemes RemapAuthenticationSchemes(AuthFlags flags, string[] providers)
        {
            AuthenticationSchemes none = AuthenticationSchemes.None;
            if ((flags & AuthFlags.AuthAnonymous) != AuthFlags.None)
            {
                none |= AuthenticationSchemes.Anonymous;
            }
            if ((flags & AuthFlags.AuthBasic) != AuthFlags.None)
            {
                none |= AuthenticationSchemes.Basic;
            }
            if ((flags & AuthFlags.AuthMD5) != AuthFlags.None)
            {
                none |= AuthenticationSchemes.Digest;
            }
            if ((flags & AuthFlags.AuthNTLM) != AuthFlags.None)
            {
                for (int i = 0; i < providers.Length; i++)
                {
                    if (string.Compare(providers[i], 0, "negotiate", 0, "negotiate".Length, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        none |= AuthenticationSchemes.Negotiate;
                    }
                    else
                    {
                        if (string.Compare(providers[i], "ntlm", StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("Hosting_NotSupportedAuthScheme", new object[] { providers[i] })));
                        }
                        none |= AuthenticationSchemes.Ntlm;
                    }
                }
            }
            if ((flags & AuthFlags.AuthPassport) != AuthFlags.None)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("Hosting_NotSupportedAuthScheme", new object[] { "Passport" })));
            }
            return none;
        }

        private object ThisLock =>
            this;
    }
}


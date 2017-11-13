namespace System.ServiceModel.Activation
{
    using System;
    using System.Security.Authentication.ExtendedProtection;

    internal class HostedServiceTransportSettings
    {
        internal HttpAccessSslFlags AccessSslFlags;
        internal System.ServiceModel.Activation.AuthFlags AuthFlags;
        internal string[] AuthProviders = MetabaseSettingsIis.DefaultAuthProviders;
        internal ExtendedProtectionPolicy IisExtendedProtectionPolicy;
        internal string Realm = string.Empty;
    }
}


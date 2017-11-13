﻿namespace System.Web.Profile
{
    using System;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProfileProviderCollection : SettingsProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (!(provider is ProfileProvider))
            {
                throw new ArgumentException(System.Web.SR.GetString("Provider_must_implement_type", new object[] { typeof(ProfileProvider).ToString() }), "provider");
            }
            base.Add(provider);
        }

        public ProfileProvider this[string name] =>
            ((ProfileProvider) base[name]);
    }
}


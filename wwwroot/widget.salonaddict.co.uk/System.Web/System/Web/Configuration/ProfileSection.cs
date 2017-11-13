namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProfileSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propAutomaticSaveEnabled = new ConfigurationProperty("automaticSaveEnabled", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultProvider = new ConfigurationProperty("defaultProvider", typeof(string), "AspNetSqlProfileProvider", null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnabled = new ConfigurationProperty("enabled", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propInherits = new ConfigurationProperty("inherits", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propProfile = new ConfigurationProperty("properties", typeof(RootProfilePropertySettingsCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        private static readonly ConfigurationProperty _propProviders = new ConfigurationProperty("providers", typeof(ProviderSettingsCollection), null, ConfigurationPropertyOptions.None);
        private long _recompilationHash;
        private bool _recompilationHashCached;

        static ProfileSection()
        {
            _properties.Add(_propEnabled);
            _properties.Add(_propDefaultProvider);
            _properties.Add(_propProviders);
            _properties.Add(_propProfile);
            _properties.Add(_propInherits);
            _properties.Add(_propAutomaticSaveEnabled);
        }

        private long CalculateHash()
        {
            HashCodeCombiner hashCombiner = new HashCodeCombiner();
            this.CalculateProfilePropertySettingsHash(this.PropertySettings, hashCombiner);
            if (this.PropertySettings != null)
            {
                foreach (ProfileGroupSettings settings in this.PropertySettings.GroupSettings)
                {
                    hashCombiner.AddObject(settings.Name);
                    this.CalculateProfilePropertySettingsHash(settings.PropertySettings, hashCombiner);
                }
            }
            return hashCombiner.CombinedHash;
        }

        private void CalculateProfilePropertySettingsHash(ProfilePropertySettingsCollection settings, HashCodeCombiner hashCombiner)
        {
            foreach (ProfilePropertySettings settings2 in settings)
            {
                hashCombiner.AddObject(settings2.Name);
                hashCombiner.AddObject(settings2.Type);
            }
        }

        [ConfigurationProperty("automaticSaveEnabled", DefaultValue=true)]
        public bool AutomaticSaveEnabled
        {
            get => 
                ((bool) base[_propAutomaticSaveEnabled]);
            set
            {
                base[_propAutomaticSaveEnabled] = value;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("defaultProvider", DefaultValue="AspNetSqlProfileProvider")]
        public string DefaultProvider
        {
            get => 
                ((string) base[_propDefaultProvider]);
            set
            {
                base[_propDefaultProvider] = value;
            }
        }

        [ConfigurationProperty("enabled", DefaultValue=true)]
        public bool Enabled
        {
            get => 
                ((bool) base[_propEnabled]);
            set
            {
                base[_propEnabled] = value;
            }
        }

        [ConfigurationProperty("inherits", DefaultValue="")]
        public string Inherits
        {
            get => 
                ((string) base[_propInherits]);
            set
            {
                base[_propInherits] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("properties")]
        public RootProfilePropertySettingsCollection PropertySettings =>
            ((RootProfilePropertySettingsCollection) base[_propProfile]);

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers =>
            ((ProviderSettingsCollection) base[_propProviders]);

        internal long RecompilationHash
        {
            get
            {
                if (!this._recompilationHashCached)
                {
                    this._recompilationHash = this.CalculateHash();
                    this._recompilationHashCached = true;
                }
                return this._recompilationHash;
            }
        }
    }
}


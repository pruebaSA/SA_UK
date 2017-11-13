namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class OutputCacheSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propEnableFragmentCache = new ConfigurationProperty("enableFragmentCache", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnableKernelCacheForVaryByStar = new ConfigurationProperty("enableKernelCacheForVaryByStar", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnableOutputCache = new ConfigurationProperty("enableOutputCache", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propOmitVaryStar = new ConfigurationProperty("omitVaryStar", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSendCacheControlHeader = new ConfigurationProperty("sendCacheControlHeader", typeof(bool), true, ConfigurationPropertyOptions.None);
        internal const bool DefaultOmitVaryStar = false;
        private bool enableKernelCacheForVaryByStar;
        private bool enableKernelCacheForVaryByStarCached;
        private bool enableOutputCache;
        private bool enableOutputCacheCached;
        private bool omitVaryStar;
        private bool omitVaryStarCached;
        private bool sendCacheControlHeaderCache;
        private bool sendCacheControlHeaderCached;

        static OutputCacheSection()
        {
            _properties.Add(_propEnableOutputCache);
            _properties.Add(_propEnableFragmentCache);
            _properties.Add(_propSendCacheControlHeader);
            _properties.Add(_propOmitVaryStar);
            _properties.Add(_propEnableKernelCacheForVaryByStar);
        }

        [ConfigurationProperty("enableFragmentCache", DefaultValue=true)]
        public bool EnableFragmentCache
        {
            get => 
                ((bool) base[_propEnableFragmentCache]);
            set
            {
                base[_propEnableFragmentCache] = value;
            }
        }

        [ConfigurationProperty("enableKernelCacheForVaryByStar", DefaultValue=false)]
        public bool EnableKernelCacheForVaryByStar
        {
            get
            {
                if (!this.enableKernelCacheForVaryByStarCached)
                {
                    this.enableKernelCacheForVaryByStar = (bool) base[_propEnableKernelCacheForVaryByStar];
                    this.enableKernelCacheForVaryByStarCached = true;
                }
                return this.enableKernelCacheForVaryByStar;
            }
            set
            {
                base[_propEnableKernelCacheForVaryByStar] = value;
                this.enableKernelCacheForVaryByStar = value;
            }
        }

        [ConfigurationProperty("enableOutputCache", DefaultValue=true)]
        public bool EnableOutputCache
        {
            get
            {
                if (!this.enableOutputCacheCached)
                {
                    this.enableOutputCache = (bool) base[_propEnableOutputCache];
                    this.enableOutputCacheCached = true;
                }
                return this.enableOutputCache;
            }
            set
            {
                base[_propEnableOutputCache] = value;
                this.enableOutputCache = value;
            }
        }

        [ConfigurationProperty("omitVaryStar", DefaultValue=false)]
        public bool OmitVaryStar
        {
            get
            {
                if (!this.omitVaryStarCached)
                {
                    this.omitVaryStar = (bool) base[_propOmitVaryStar];
                    this.omitVaryStarCached = true;
                }
                return this.omitVaryStar;
            }
            set
            {
                base[_propOmitVaryStar] = value;
                this.omitVaryStar = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("sendCacheControlHeader", DefaultValue=true)]
        public bool SendCacheControlHeader
        {
            get
            {
                if (!this.sendCacheControlHeaderCached)
                {
                    this.sendCacheControlHeaderCache = (bool) base[_propSendCacheControlHeader];
                    this.sendCacheControlHeaderCached = true;
                }
                return this.sendCacheControlHeaderCache;
            }
            set
            {
                base[_propSendCacheControlHeader] = value;
                this.sendCacheControlHeaderCache = value;
            }
        }
    }
}


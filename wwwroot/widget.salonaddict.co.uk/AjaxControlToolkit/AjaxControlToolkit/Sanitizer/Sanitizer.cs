namespace AjaxControlToolkit.Sanitizer
{
    using System;
    using System.Web.Configuration;

    internal class Sanitizer
    {
        private static bool _initialized;
        private static SanitizerProvider _provider;
        private static SanitizerProviderCollection _providers;

        public static SanitizerProvider GetProvider()
        {
            Initialize();
            return _provider;
        }

        private static void Initialize()
        {
            if (!_initialized)
            {
                ProviderSanitizerSection section = (ProviderSanitizerSection) WebConfigurationManager.GetSection("system.web/sanitizer");
                if (section != null)
                {
                    _providers = new SanitizerProviderCollection();
                    ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(SanitizerProvider));
                    _provider = _providers[section.DefaultProvider];
                }
                _initialized = true;
            }
        }
    }
}


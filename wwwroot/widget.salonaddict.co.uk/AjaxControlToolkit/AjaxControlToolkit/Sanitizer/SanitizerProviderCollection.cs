namespace AjaxControlToolkit.Sanitizer
{
    using System;
    using System.Configuration.Provider;
    using System.Reflection;

    public class SanitizerProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (!(provider is SanitizerProvider))
            {
                string paramName = typeof(SanitizerProvider).ToString();
                throw new ArgumentException("Provider must implement SanitizerProvider type", paramName);
            }
            base.Add(provider);
        }

        public SanitizerProvider this[string name] =>
            ((SanitizerProvider) base[name]);
    }
}


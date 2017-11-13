namespace AjaxControlToolkit.Sanitizer
{
    using System;
    using System.Collections.Generic;
    using System.Configuration.Provider;

    public abstract class SanitizerProvider : ProviderBase
    {
        protected SanitizerProvider()
        {
        }

        public abstract string GetSafeHtmlFragment(string htmlFragment, Dictionary<string, string[]> elementWhiteList, Dictionary<string, string[]> attributeWhiteList);

        public abstract string ApplicationName { get; set; }

        public abstract bool RequiresFullTrust { get; }
    }
}


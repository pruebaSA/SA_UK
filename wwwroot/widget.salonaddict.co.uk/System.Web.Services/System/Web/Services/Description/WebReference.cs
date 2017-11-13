namespace System.Web.Services.Description
{
    using System;
    using System.CodeDom;
    using System.Collections.Specialized;
    using System.Web.Services.Discovery;

    public sealed class WebReference
    {
        private string appSettingBaseUrl;
        private string appSettingUrlKey;
        private DiscoveryClientDocumentCollection documents;
        private string protocolName;
        private CodeNamespace proxyCode;
        private StringCollection validationWarnings;
        private ServiceDescriptionImportWarnings warnings;

        public WebReference(DiscoveryClientDocumentCollection documents, CodeNamespace proxyCode) : this(documents, proxyCode, null, null, null)
        {
        }

        public WebReference(DiscoveryClientDocumentCollection documents, CodeNamespace proxyCode, string appSettingUrlKey, string appSettingBaseUrl) : this(documents, proxyCode, null, appSettingUrlKey, appSettingBaseUrl)
        {
        }

        public WebReference(DiscoveryClientDocumentCollection documents, CodeNamespace proxyCode, string protocolName, string appSettingUrlKey, string appSettingBaseUrl)
        {
            if (documents == null)
            {
                throw new ArgumentNullException("documents");
            }
            if (proxyCode == null)
            {
                throw new ArgumentNullException("proxyCode");
            }
            if ((appSettingBaseUrl != null) && (appSettingUrlKey == null))
            {
                throw new ArgumentNullException("appSettingUrlKey");
            }
            this.protocolName = protocolName;
            this.appSettingUrlKey = appSettingUrlKey;
            this.appSettingBaseUrl = appSettingBaseUrl;
            this.documents = documents;
            this.proxyCode = proxyCode;
        }

        public string AppSettingBaseUrl =>
            this.appSettingBaseUrl;

        public string AppSettingUrlKey =>
            this.appSettingUrlKey;

        public DiscoveryClientDocumentCollection Documents =>
            this.documents;

        public string ProtocolName
        {
            get
            {
                if (this.protocolName != null)
                {
                    return this.protocolName;
                }
                return string.Empty;
            }
            set
            {
                this.protocolName = value;
            }
        }

        public CodeNamespace ProxyCode =>
            this.proxyCode;

        public StringCollection ValidationWarnings
        {
            get
            {
                if (this.validationWarnings == null)
                {
                    this.validationWarnings = new StringCollection();
                }
                return this.validationWarnings;
            }
        }

        public ServiceDescriptionImportWarnings Warnings
        {
            get => 
                this.warnings;
            set
            {
                this.warnings = value;
            }
        }
    }
}


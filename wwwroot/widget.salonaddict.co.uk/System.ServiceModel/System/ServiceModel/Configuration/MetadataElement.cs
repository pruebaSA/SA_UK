﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.ServiceModel.Description;

    public sealed class MetadataElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        public Collection<IPolicyImportExtension> LoadPolicyImportExtensions() => 
            ConfigLoader.LoadPolicyImporters(this.PolicyImporters, base.EvaluationContext);

        public Collection<IWsdlImportExtension> LoadWsdlImportExtensions() => 
            ConfigLoader.LoadWsdlImporters(this.WsdlImporters, base.EvaluationContext);

        internal void SetDefaults()
        {
            this.PolicyImporters.SetDefaults();
            this.WsdlImporters.SetDefaults();
        }

        [ConfigurationProperty("policyImporters")]
        public PolicyImporterElementCollection PolicyImporters =>
            ((PolicyImporterElementCollection) base["policyImporters"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("policyImporters", typeof(PolicyImporterElementCollection), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("wsdlImporters", typeof(WsdlImporterElementCollection), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("wsdlImporters")]
        public WsdlImporterElementCollection WsdlImporters =>
            ((WsdlImporterElementCollection) base["wsdlImporters"]);
    }
}


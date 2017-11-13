namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class BindingsSection : ConfigurationSection, IConfigurationContextProviderInternal
    {
        private static System.Configuration.Configuration configuration;
        private ConfigurationPropertyCollection properties;

        public static BindingsSection GetSection(System.Configuration.Configuration config)
        {
            if (config == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("config");
            }
            return (BindingsSection) config.GetSection(ConfigurationStrings.BindingsSectionGroupPath);
        }

        ContextInformation IConfigurationContextProviderInternal.GetEvaluationContext() => 
            base.EvaluationContext;

        ContextInformation IConfigurationContextProviderInternal.GetOriginalEvaluationContext() => 
            null;

        internal static bool TryAdd(string name, Binding binding, out string bindingSectionName)
        {
            if (Configuration == null)
            {
                DiagnosticUtility.FailFast("The TryAdd(string name, Binding binding, Configuration config, out string binding) variant of this function should always be called first. The Configuration object is not set.");
            }
            bool flag = false;
            string str = null;
            BindingsSection section = GetSection(Configuration);
            section.UpdateBindingSections();
            foreach (string str2 in section.BindingCollectionElements.Keys)
            {
                BindingCollectionElement element = section.BindingCollectionElements[str2];
                if (!(element is CustomBindingCollectionElement))
                {
                    MethodInfo method = element.GetType().GetMethod("TryAdd", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (method != null)
                    {
                        flag = (bool) method.Invoke(element, new object[] { name, binding, Configuration });
                        if (flag)
                        {
                            str = str2;
                            break;
                        }
                    }
                }
            }
            if (!flag)
            {
                flag = CustomBindingCollectionElement.GetBindingCollectionElement().TryAdd(name, binding, Configuration);
                if (flag)
                {
                    str = "customBinding";
                }
            }
            bindingSectionName = str;
            return flag;
        }

        internal static bool TryAdd(string name, Binding binding, System.Configuration.Configuration config, out string bindingSectionName)
        {
            bool flag = false;
            Configuration = config;
            try
            {
                flag = TryAdd(name, binding, out bindingSectionName);
            }
            finally
            {
                Configuration = null;
            }
            return flag;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private void UpdateBindingSections()
        {
            ExtensionElementCollection elements = ExtensionsSection.UnsafeLookupCollection("bindingExtensions", ConfigurationHelpers.GetEvaluationContext(this));
            if (elements.Count != this.properties.Count)
            {
                foreach (ExtensionElement element in elements)
                {
                    if ((element != null) && !this.properties.Contains(element.Name))
                    {
                        ConfigurationProperty property = new ConfigurationProperty(element.Name, Type.GetType(element.Type, true), null, ConfigurationPropertyOptions.None);
                        this.properties.Add(property);
                    }
                }
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static void ValidateBindingReference(string binding, string bindingConfiguration, ContextInformation evaluationContext, ConfigurationElement configurationElement)
        {
            if (evaluationContext == null)
            {
                DiagnosticUtility.FailFast("ValidateBindingReference() should only called with valid ContextInformation");
            }
            if (!string.IsNullOrEmpty(binding))
            {
                BindingCollectionElement element = null;
                if (evaluationContext != null)
                {
                    element = ConfigurationHelpers.UnsafeGetAssociatedBindingCollectionElement(evaluationContext, binding);
                }
                else
                {
                    element = ConfigurationHelpers.UnsafeGetBindingCollectionElement(binding);
                }
                if (element == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidSection", new object[] { ConfigurationHelpers.GetBindingsSectionPath(binding) }), configurationElement.ElementInformation.Source, configurationElement.ElementInformation.LineNumber));
                }
                if (!string.IsNullOrEmpty(bindingConfiguration) && !element.ContainsKey(bindingConfiguration))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidBindingName", new object[] { bindingConfiguration, ConfigurationHelpers.GetBindingsSectionPath(binding), "bindingConfiguration" }), configurationElement.ElementInformation.Source, configurationElement.ElementInformation.LineNumber));
                }
            }
        }

        [ConfigurationProperty("basicHttpBinding", Options=ConfigurationPropertyOptions.None)]
        public BasicHttpBindingCollectionElement BasicHttpBinding =>
            ((BasicHttpBindingCollectionElement) base["basicHttpBinding"]);

        private Dictionary<string, BindingCollectionElement> BindingCollectionElements
        {
            get
            {
                Dictionary<string, BindingCollectionElement> dictionary = new Dictionary<string, BindingCollectionElement>();
                foreach (ConfigurationProperty property in this.Properties)
                {
                    dictionary.Add(property.Name, this[property.Name]);
                }
                return dictionary;
            }
        }

        public List<BindingCollectionElement> BindingCollections
        {
            get
            {
                List<BindingCollectionElement> list = new List<BindingCollectionElement>();
                foreach (ConfigurationProperty property in this.Properties)
                {
                    list.Add(this[property.Name]);
                }
                return list;
            }
        }

        private static System.Configuration.Configuration Configuration
        {
            get => 
                configuration;
            set
            {
                configuration = value;
            }
        }

        [ConfigurationProperty("customBinding", Options=ConfigurationPropertyOptions.None)]
        public CustomBindingCollectionElement CustomBinding =>
            ((CustomBindingCollectionElement) base["customBinding"]);

        public BindingCollectionElement this[string binding] =>
            ((BindingCollectionElement) base[binding]);

        [ConfigurationProperty("msmqIntegrationBinding", Options=ConfigurationPropertyOptions.None)]
        public MsmqIntegrationBindingCollectionElement MsmqIntegrationBinding =>
            ((MsmqIntegrationBindingCollectionElement) base["msmqIntegrationBinding"]);

        [ConfigurationProperty("netMsmqBinding", Options=ConfigurationPropertyOptions.None)]
        public NetMsmqBindingCollectionElement NetMsmqBinding =>
            ((NetMsmqBindingCollectionElement) base["netMsmqBinding"]);

        [ConfigurationProperty("netNamedPipeBinding", Options=ConfigurationPropertyOptions.None)]
        public NetNamedPipeBindingCollectionElement NetNamedPipeBinding =>
            ((NetNamedPipeBindingCollectionElement) base["netNamedPipeBinding"]);

        [ConfigurationProperty("netPeerTcpBinding", Options=ConfigurationPropertyOptions.None)]
        public NetPeerTcpBindingCollectionElement NetPeerTcpBinding =>
            ((NetPeerTcpBindingCollectionElement) base["netPeerTcpBinding"]);

        [ConfigurationProperty("netTcpBinding", Options=ConfigurationPropertyOptions.None)]
        public NetTcpBindingCollectionElement NetTcpBinding =>
            ((NetTcpBindingCollectionElement) base["netTcpBinding"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = new ConfigurationPropertyCollection();
                }
                this.UpdateBindingSections();
                return this.properties;
            }
        }

        [ConfigurationProperty("ws2007FederationHttpBinding", Options=ConfigurationPropertyOptions.None)]
        public WS2007FederationHttpBindingCollectionElement WS2007FederationHttpBinding =>
            ((WS2007FederationHttpBindingCollectionElement) base["ws2007FederationHttpBinding"]);

        [ConfigurationProperty("ws2007HttpBinding", Options=ConfigurationPropertyOptions.None)]
        public WS2007HttpBindingCollectionElement WS2007HttpBinding =>
            ((WS2007HttpBindingCollectionElement) base["ws2007HttpBinding"]);

        [ConfigurationProperty("wsDualHttpBinding", Options=ConfigurationPropertyOptions.None)]
        public WSDualHttpBindingCollectionElement WSDualHttpBinding =>
            ((WSDualHttpBindingCollectionElement) base["wsDualHttpBinding"]);

        [ConfigurationProperty("wsFederationHttpBinding", Options=ConfigurationPropertyOptions.None)]
        public WSFederationHttpBindingCollectionElement WSFederationHttpBinding =>
            ((WSFederationHttpBindingCollectionElement) base["wsFederationHttpBinding"]);

        [ConfigurationProperty("wsHttpBinding", Options=ConfigurationPropertyOptions.None)]
        public WSHttpBindingCollectionElement WSHttpBinding =>
            ((WSHttpBindingCollectionElement) base["wsHttpBinding"]);
    }
}


﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public abstract class BindingCollectionElement : ConfigurationElement, IConfigurationContextProviderInternal
    {
        private string bindingName = string.Empty;

        protected BindingCollectionElement()
        {
        }

        public abstract bool ContainsKey(string name);
        [SecurityCritical, SecurityTreatAsSafe]
        private string GetBindingName()
        {
            string name = string.Empty;
            ExtensionElementCollection elements = null;
            Type type = base.GetType();
            elements = ExtensionsSection.UnsafeLookupCollection("bindingExtensions", ConfigurationHelpers.GetEvaluationContext(this));
            if (elements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigExtensionCollectionNotFound", new object[] { "bindingExtensions" }), base.ElementInformation.Source, base.ElementInformation.LineNumber));
            }
            for (int i = 0; i < elements.Count; i++)
            {
                ExtensionElement element = elements[i];
                if (element.Type.Equals(type.AssemblyQualifiedName, StringComparison.Ordinal))
                {
                    name = element.Name;
                    break;
                }
                Type o = Type.GetType(element.Type, false);
                if ((o != null) && type.Equals(o))
                {
                    name = element.Name;
                    break;
                }
            }
            if (string.IsNullOrEmpty(name))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigExtensionTypeNotRegisteredInCollection", new object[] { type.AssemblyQualifiedName, "bindingExtensions" }), base.ElementInformation.Source, base.ElementInformation.LineNumber));
            }
            return name;
        }

        protected internal abstract Binding GetDefault();
        ContextInformation IConfigurationContextProviderInternal.GetEvaluationContext() => 
            base.EvaluationContext;

        ContextInformation IConfigurationContextProviderInternal.GetOriginalEvaluationContext() => 
            null;

        protected internal abstract bool TryAdd(string name, Binding binding, System.Configuration.Configuration config);

        public string BindingName
        {
            get
            {
                if (string.IsNullOrEmpty(this.bindingName))
                {
                    this.bindingName = this.GetBindingName();
                }
                return this.bindingName;
            }
        }

        public abstract Type BindingType { get; }

        public abstract ReadOnlyCollection<IBindingConfigurationElement> ConfiguredBindings { get; }
    }
}


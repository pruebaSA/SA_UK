namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [ConfigurationPermission(SecurityAction.InheritanceDemand, Unrestricted=true)]
    public abstract class ServiceModelExtensionElement : ConfigurationElement, IConfigurationContextProviderInternal
    {
        private string configurationElementName = string.Empty;
        private ContextInformation containingEvaluationContext;
        [SecurityCritical]
        private EvaluationContextHelper contextHelper;
        private string extensionCollectionName = string.Empty;
        private bool modified;

        protected ServiceModelExtensionElement()
        {
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal bool CanAdd(string extensionCollectionName, ContextInformation evaluationContext)
        {
            bool flag = false;
            ExtensionElementCollection elements = ExtensionsSection.UnsafeLookupCollection(extensionCollectionName, evaluationContext);
            if ((elements == null) || (elements.Count == 0))
            {
                if (DiagnosticUtility.ShouldTraceWarning)
                {
                    TraceCode extensionCollectionDoesNotExist = TraceCode.ExtensionCollectionDoesNotExist;
                    if ((elements != null) && (elements.Count == 0))
                    {
                        extensionCollectionDoesNotExist = TraceCode.ExtensionCollectionIsEmpty;
                    }
                    TraceUtility.TraceEvent(TraceEventType.Warning, extensionCollectionDoesNotExist, this.CreateCanAddRecord(extensionCollectionName), this, null);
                }
                return flag;
            }
            foreach (ExtensionElement element in elements)
            {
                if (element.Type.Equals(base.GetType().AssemblyQualifiedName, StringComparison.Ordinal))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag && DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.ConfiguredExtensionTypeNotFound, this.CreateCanAddRecord(extensionCollectionName), this, null);
            }
            return flag;
        }

        public virtual void CopyFrom(ServiceModelExtensionElement from)
        {
            if (this.IsReadOnly())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigReadOnly")));
            }
            if (from == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("from");
            }
        }

        private DictionaryTraceRecord CreateCanAddRecord(string extensionCollectionName) => 
            new DictionaryTraceRecord(new Dictionary<string, string>(2) { 
                ["ElementType"] = DiagnosticTrace.XmlEncode(base.GetType().AssemblyQualifiedName),
                ["CollectionName"] = ConfigurationStrings.ExtensionsSectionPath + "/" + extensionCollectionName
            });

        internal void DeserializeInternal(XmlReader reader, bool serializeCollectionKey)
        {
            this.DeserializeElement(reader, serializeCollectionKey);
        }

        internal object FromProperty(ConfigurationProperty property) => 
            base[property];

        [SecurityCritical, SecurityTreatAsSafe]
        private string GetConfigurationElementName()
        {
            string name = string.Empty;
            ExtensionElementCollection elements = null;
            Type type = base.GetType();
            ContextInformation evaluationContext = ConfigurationHelpers.GetEvaluationContext(this);
            if (evaluationContext == null)
            {
                evaluationContext = this.ContainingEvaluationContext;
            }
            if (string.IsNullOrEmpty(this.extensionCollectionName))
            {
                if (DiagnosticUtility.ShouldTraceWarning)
                {
                    TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.ExtensionCollectionNameNotFound, this, (Exception) null);
                }
                elements = ExtensionsSection.UnsafeLookupAssociatedCollection(base.GetType(), evaluationContext, out this.extensionCollectionName);
            }
            else
            {
                elements = ExtensionsSection.UnsafeLookupCollection(this.extensionCollectionName, evaluationContext);
            }
            if (elements == null)
            {
                if (string.IsNullOrEmpty(this.extensionCollectionName))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigNoExtensionCollectionAssociatedWithType", new object[] { type.AssemblyQualifiedName }), base.ElementInformation.Source, base.ElementInformation.LineNumber));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigExtensionCollectionNotFound", new object[] { this.extensionCollectionName }), base.ElementInformation.Source, base.ElementInformation.LineNumber));
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigExtensionTypeNotRegisteredInCollection", new object[] { type.AssemblyQualifiedName, this.extensionCollectionName }), base.ElementInformation.Source, base.ElementInformation.LineNumber));
            }
            return name;
        }

        internal void InternalInitializeDefault()
        {
            this.InitializeDefault();
        }

        protected override bool IsModified() => 
            (this.modified | base.IsModified());

        internal bool IsModifiedInternal() => 
            this.IsModified();

        [SecurityCritical]
        protected override void Reset(ConfigurationElement parentElement)
        {
            this.contextHelper.OnReset(parentElement);
            base.Reset(parentElement);
        }

        internal void ResetModifiedInternal()
        {
            this.ResetModified();
        }

        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            base.SerializeElement(writer, serializeCollectionKey);
            return true;
        }

        internal bool SerializeInternal(XmlWriter writer, bool serializeCollectionKey) => 
            this.SerializeElement(writer, serializeCollectionKey);

        internal void SetReadOnlyInternal()
        {
            this.SetReadOnly();
        }

        ContextInformation IConfigurationContextProviderInternal.GetEvaluationContext() => 
            base.EvaluationContext;

        [SecurityCritical]
        ContextInformation IConfigurationContextProviderInternal.GetOriginalEvaluationContext() => 
            this.contextHelper.GetOriginalContext(this);

        public string ConfigurationElementName
        {
            get
            {
                if (string.IsNullOrEmpty(this.configurationElementName))
                {
                    this.configurationElementName = this.GetConfigurationElementName();
                }
                return this.configurationElementName;
            }
        }

        internal ContextInformation ContainingEvaluationContext
        {
            get => 
                this.containingEvaluationContext;
            set
            {
                this.containingEvaluationContext = value;
            }
        }

        internal ContextInformation EvalContext =>
            base.EvaluationContext;

        internal string ExtensionCollectionName
        {
            get => 
                this.extensionCollectionName;
            set
            {
                this.extensionCollectionName = value;
            }
        }

        internal ConfigurationPropertyCollection PropertiesInternal =>
            this.Properties;
    }
}


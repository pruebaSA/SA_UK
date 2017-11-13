namespace System.ServiceModel.Description
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.IdentityModel.Selectors;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Security;
    using System.Text;
    using System.Web.Configuration;

    internal class ConfigLoader
    {
        private Dictionary<string, Binding> bindingTable;
        private ContextInformation configurationContext;
        [SecurityCritical]
        private static System.Configuration.ConfigurationPermission configurationPermission;
        private IContractResolver contractResolver;
        private static readonly object[] emptyObjectArray = new object[0];
        private static readonly Type[] emptyTypeArray = new Type[0];
        [ThreadStatic]
        private static List<string> resolvedBindings;

        public ConfigLoader() : this((IContractResolver) null)
        {
        }

        public ConfigLoader(ContextInformation configurationContext) : this((IContractResolver) null)
        {
            this.configurationContext = configurationContext;
        }

        public ConfigLoader(IContractResolver contractResolver)
        {
            this.contractResolver = contractResolver;
            this.bindingTable = new Dictionary<string, Binding>();
        }

        private static void CheckAccess(IConfigurationContextProviderInternal element)
        {
            if (IsConfigAboveApplication(ConfigurationHelpers.GetOriginalEvaluationContext(element)))
            {
                ConfigurationPermission.Demand();
            }
        }

        private static bool IsConfigAboveApplication(ContextInformation contextInformation)
        {
            if (contextInformation == null)
            {
                return true;
            }
            if (contextInformation.IsMachineLevel)
            {
                return true;
            }
            if (contextInformation.HostingContext is ExeContext)
            {
                return false;
            }
            return IsWebConfigAboveApplication(contextInformation);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool IsWebConfigAboveApplication(ContextInformation contextInformation)
        {
            WebContext hostingContext = contextInformation.HostingContext as WebContext;
            return ((hostingContext != null) && (hostingContext.ApplicationLevel == WebApplicationLevel.AboveApplication));
        }

        private bool IsWildcardMatch(string endpointConfigurationName) => 
            string.Equals(endpointConfigurationName, "*", StringComparison.Ordinal);

        [SecurityCritical, SecurityTreatAsSafe]
        private static void LoadBehaviors<T>(ServiceModelExtensionCollectionElement<BehaviorExtensionElement> behaviorElement, KeyedByTypeCollection<T> behaviors, bool commonBehaviors)
        {
            bool? isPT = null;
            KeyedByTypeCollection<T> types = new KeyedByTypeCollection<T>();
            for (int i = 0; i < behaviorElement.Count; i++)
            {
                BehaviorExtensionElement behaviorExtension = behaviorElement[i];
                object obj2 = behaviorExtension.CreateBehavior();
                if (obj2 != null)
                {
                    Type c = obj2.GetType();
                    if (!typeof(T).IsAssignableFrom(c))
                    {
                        TraceBehaviorWarning(behaviorExtension, TraceCode.SkipBehavior, c, typeof(T));
                    }
                    else if (commonBehaviors && ShouldSkipCommonBehavior(c, ref isPT))
                    {
                        TraceBehaviorWarning(behaviorExtension, TraceCode.SkipBehavior, c, typeof(T));
                    }
                    else
                    {
                        types.Add((T) obj2);
                        if (behaviors.Contains(c))
                        {
                            TraceBehaviorWarning(behaviorExtension, TraceCode.RemoveBehavior, c, typeof(T));
                            behaviors.Remove(c);
                        }
                        behaviors.Add((T) obj2);
                    }
                }
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static void LoadChannelBehaviors(EndpointBehaviorElement behaviorElement, KeyedByTypeCollection<IEndpointBehavior> channelBehaviors)
        {
            if (behaviorElement != null)
            {
                LoadBehaviors<IEndpointBehavior>(behaviorElement, channelBehaviors, false);
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void LoadChannelBehaviors(ServiceEndpoint serviceEndpoint, string configurationName)
        {
            bool wildcard = this.IsWildcardMatch(configurationName);
            ChannelEndpointElement provider = this.LookupChannel(configurationName, serviceEndpoint.Contract.ConfigurationName, wildcard);
            if (provider == null)
            {
                if (wildcard)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxConfigContractNotFound", new object[] { serviceEndpoint.Contract.ConfigurationName })));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxConfigChannelConfigurationNotFound", new object[] { configurationName, serviceEndpoint.Contract.ConfigurationName })));
            }
            if ((serviceEndpoint.Binding == null) && !string.IsNullOrEmpty(provider.Binding))
            {
                serviceEndpoint.Binding = LookupBinding(provider.Binding, provider.BindingConfiguration, ConfigurationHelpers.GetEvaluationContext(provider));
            }
            if (((serviceEndpoint.Address == null) && (provider.Address != null)) && (provider.Address.OriginalString.Length > 0))
            {
                serviceEndpoint.Address = new EndpointAddress(provider.Address, LoadIdentity(provider.Identity), provider.Headers.Headers);
            }
            CommonBehaviorsSection commonBehaviors = LookupCommonBehaviors(ConfigurationHelpers.GetEvaluationContext(provider));
            if ((commonBehaviors != null) && (commonBehaviors.EndpointBehaviors != null))
            {
                LoadBehaviors<IEndpointBehavior>(commonBehaviors.EndpointBehaviors, serviceEndpoint.Behaviors, true);
            }
            EndpointBehaviorElement endpointBehaviors = LookupEndpointBehaviors(provider.BehaviorConfiguration, ConfigurationHelpers.GetEvaluationContext(provider));
            if (endpointBehaviors != null)
            {
                LoadBehaviors<IEndpointBehavior>(endpointBehaviors, serviceEndpoint.Behaviors, false);
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static void LoadChannelBehaviors(string behaviorName, ContextInformation context, KeyedByTypeCollection<IEndpointBehavior> channelBehaviors)
        {
            LoadChannelBehaviors(LookupEndpointBehaviors(behaviorName, context), channelBehaviors);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void LoadCommonClientBehaviors(ServiceEndpoint serviceEndpoint)
        {
            CommonBehaviorsSection commonBehaviors = LookupCommonBehaviors(this.configurationContext);
            if ((commonBehaviors != null) && (commonBehaviors.EndpointBehaviors != null))
            {
                LoadBehaviors<IEndpointBehavior>(commonBehaviors.EndpointBehaviors, serviceEndpoint.Behaviors, true);
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static EndpointAddress LoadEndpointAddress(EndpointAddressElementBase element) => 
            new EndpointAddress(element.Address, LoadIdentity(element.Identity), element.Headers.Headers);

        [SecurityTreatAsSafe, SecurityCritical]
        private void LoadHostConfig(ServiceElement serviceElement, ServiceHostBase host, Action<Uri> addBaseAddress)
        {
            HostElement element = serviceElement.Host;
            if (element != null)
            {
                if (!ServiceHostingEnvironment.IsHosted)
                {
                    foreach (BaseAddressElement element2 in element.BaseAddresses)
                    {
                        Uri uri;
                        string uriString = null;
                        string baseAddress = element2.BaseAddress;
                        int index = baseAddress.IndexOf(':');
                        if ((((index != -1) && (baseAddress.Length >= (index + 4))) && ((baseAddress[index + 1] == '/') && (baseAddress[index + 2] == '/'))) && (baseAddress[index + 3] == '*'))
                        {
                            string str3 = baseAddress.Substring(0, index + 3);
                            string str4 = baseAddress.Substring(index + 4);
                            StringBuilder builder = new StringBuilder(str3);
                            builder.Append(Dns.GetHostName());
                            builder.Append(str4);
                            uriString = builder.ToString();
                        }
                        if (uriString == null)
                        {
                            uriString = baseAddress;
                        }
                        if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("BaseAddressMustBeAbsolute")));
                        }
                        addBaseAddress(uri);
                    }
                }
                HostTimeoutsElement timeouts = element.Timeouts;
                if (timeouts != null)
                {
                    if (timeouts.OpenTimeout != TimeSpan.Zero)
                    {
                        host.OpenTimeout = timeouts.OpenTimeout;
                    }
                    if (timeouts.CloseTimeout != TimeSpan.Zero)
                    {
                        host.CloseTimeout = timeouts.CloseTimeout;
                    }
                }
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static EndpointIdentity LoadIdentity(IdentityElement element)
        {
            EndpointIdentity identity = null;
            PropertyInformationCollection properties = element.ElementInformation.Properties;
            if (properties["userPrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
            }
            if (properties["servicePrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
            }
            if (properties["dns"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
            }
            if (properties["rsa"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
            }
            if (properties["certificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                X509Certificate2Collection supportingCertificates = new X509Certificate2Collection();
                supportingCertificates.Import(Convert.FromBase64String(element.Certificate.EncodedValue));
                if (supportingCertificates.Count == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("UnableToLoadCertificateIdentity")));
                }
                X509Certificate2 primaryCertificate = supportingCertificates[0];
                supportingCertificates.RemoveAt(0);
                return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate, supportingCertificates);
            }
            if (properties["certificateReference"].ValueOrigin != PropertyValueOrigin.Default)
            {
                X509CertificateStore store = new X509CertificateStore(element.CertificateReference.StoreName, element.CertificateReference.StoreLocation);
                X509Certificate2Collection certificates = null;
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    certificates = store.Find(element.CertificateReference.X509FindType, element.CertificateReference.FindValue, false);
                    if (certificates.Count == 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("UnableToLoadCertificateIdentity")));
                    }
                    X509Certificate2 certificate = new X509Certificate2(certificates[0]);
                    if (element.CertificateReference.IsChainIncluded)
                    {
                        X509Chain certificateChain = new X509Chain {
                            ChainPolicy = { RevocationMode = X509RevocationMode.NoCheck }
                        };
                        certificateChain.Build(certificate);
                        return EndpointIdentity.CreateX509CertificateIdentity(certificateChain);
                    }
                    identity = EndpointIdentity.CreateX509CertificateIdentity(certificate);
                }
                finally
                {
                    System.ServiceModel.Security.SecurityUtils.ResetAllCertificates(certificates);
                    store.Close();
                }
            }
            return identity;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static Collection<IPolicyImportExtension> LoadPolicyImporters(PolicyImporterElementCollection policyImporterElements, ContextInformation context)
        {
            Collection<IPolicyImportExtension> collection = new Collection<IPolicyImportExtension>();
            foreach (PolicyImporterElement element in policyImporterElements)
            {
                Type c = Type.GetType(element.Type, true, true);
                if (!typeof(IPolicyImportExtension).IsAssignableFrom(c))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("InvalidPolicyExtensionTypeInConfig", new object[] { c.AssemblyQualifiedName })));
                }
                ConstructorInfo constructor = c.GetConstructor(emptyTypeArray);
                if (constructor == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("PolicyExtensionTypeRequiresDefaultConstructor", new object[] { c.AssemblyQualifiedName })));
                }
                collection.Add((IPolicyImportExtension) constructor.Invoke(emptyObjectArray));
            }
            return collection;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public void LoadServiceDescription(ServiceHostBase host, System.ServiceModel.Description.ServiceDescription description, ServiceElement serviceElement, Action<Uri> addBaseAddress)
        {
            CommonBehaviorsSection commonBehaviors = LookupCommonBehaviors((serviceElement == null) ? null : ConfigurationHelpers.GetEvaluationContext(serviceElement));
            if ((commonBehaviors != null) && (commonBehaviors.ServiceBehaviors != null))
            {
                LoadBehaviors<IServiceBehavior>(commonBehaviors.ServiceBehaviors, description.Behaviors, true);
            }
            if (serviceElement != null)
            {
                this.LoadHostConfig(serviceElement, host, addBaseAddress);
                ServiceBehaviorElement serviceBehaviors = LookupServiceBehaviors(serviceElement.BehaviorConfiguration, ConfigurationHelpers.GetEvaluationContext(serviceElement));
                if (serviceBehaviors != null)
                {
                    LoadBehaviors<IServiceBehavior>(serviceBehaviors, description.Behaviors, false);
                }
                ServiceHostBase.ServiceAndBehaviorsContractResolver contractResolver = this.contractResolver as ServiceHostBase.ServiceAndBehaviorsContractResolver;
                if (contractResolver != null)
                {
                    contractResolver.AddBehaviorContractsToResolver(description.Behaviors);
                }
                foreach (ServiceEndpointElement element2 in serviceElement.Endpoints)
                {
                    Binding binding;
                    ServiceEndpoint endpoint;
                    ContractDescription contract = this.LookupContract(element2.Contract, description.Name);
                    string key = element2.Binding + ":" + element2.BindingConfiguration;
                    if (!this.bindingTable.TryGetValue(key, out binding))
                    {
                        binding = LookupBinding(element2.Binding, element2.BindingConfiguration, ConfigurationHelpers.GetEvaluationContext(serviceElement));
                        this.bindingTable.Add(key, binding);
                    }
                    if (!string.IsNullOrEmpty(element2.BindingName))
                    {
                        binding.Name = element2.BindingName;
                    }
                    if (!string.IsNullOrEmpty(element2.BindingNamespace))
                    {
                        binding.Namespace = element2.BindingNamespace;
                    }
                    Uri address = element2.Address;
                    if (null == address)
                    {
                        endpoint = new ServiceEndpoint(contract) {
                            Binding = binding
                        };
                    }
                    else
                    {
                        Uri uri = ServiceHostBase.MakeAbsoluteUri(address, binding, host.InternalBaseAddresses);
                        endpoint = new ServiceEndpoint(contract, binding, new EndpointAddress(uri, LoadIdentity(element2.Identity), element2.Headers.Headers)) {
                            UnresolvedAddress = element2.Address
                        };
                    }
                    if (element2.ListenUri != null)
                    {
                        endpoint.ListenUri = ServiceHostBase.MakeAbsoluteUri(element2.ListenUri, binding, host.InternalBaseAddresses);
                        endpoint.UnresolvedListenUri = element2.ListenUri;
                    }
                    endpoint.ListenUriMode = element2.ListenUriMode;
                    if (!string.IsNullOrEmpty(element2.Name))
                    {
                        endpoint.Name = element2.Name;
                    }
                    KeyedByTypeCollection<IEndpointBehavior> behaviors = endpoint.Behaviors;
                    EndpointBehaviorElement endpointBehaviors = LookupEndpointBehaviors(element2.BehaviorConfiguration, ConfigurationHelpers.GetEvaluationContext(element2));
                    if (endpointBehaviors != null)
                    {
                        LoadBehaviors<IEndpointBehavior>(endpointBehaviors, behaviors, false);
                    }
                    description.Endpoints.Add(endpoint);
                }
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static Collection<IWsdlImportExtension> LoadWsdlImporters(WsdlImporterElementCollection wsdlImporterElements, ContextInformation context)
        {
            Collection<IWsdlImportExtension> collection = new Collection<IWsdlImportExtension>();
            foreach (WsdlImporterElement element in wsdlImporterElements)
            {
                Type c = Type.GetType(element.Type, true, true);
                if (!typeof(IWsdlImportExtension).IsAssignableFrom(c))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("InvalidWsdlExtensionTypeInConfig", new object[] { c.AssemblyQualifiedName })));
                }
                ConstructorInfo constructor = c.GetConstructor(emptyTypeArray);
                if (constructor == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("WsdlExtensionTypeRequiresDefaultConstructor", new object[] { c.AssemblyQualifiedName })));
                }
                collection.Add((IWsdlImportExtension) constructor.Invoke(emptyObjectArray));
            }
            return collection;
        }

        internal static Binding LookupBinding(string bindingSectionName, string configurationName) => 
            LookupBinding(bindingSectionName, configurationName, null);

        [SecurityCritical, SecurityTreatAsSafe]
        internal static Binding LookupBinding(string bindingSectionName, string configurationName, ContextInformation context)
        {
            if (string.IsNullOrEmpty(bindingSectionName))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigBindingTypeCannotBeNullOrEmpty")));
            }
            BindingCollectionElement element = null;
            if (context == null)
            {
                element = ConfigurationHelpers.UnsafeGetBindingCollectionElement(bindingSectionName);
            }
            else
            {
                element = ConfigurationHelpers.UnsafeGetAssociatedBindingCollectionElement(context, bindingSectionName);
            }
            Binding binding = element.GetDefault();
            if (!string.IsNullOrEmpty(configurationName))
            {
                bool flag = false;
                foreach (object obj2 in element.ConfiguredBindings)
                {
                    IBindingConfigurationElement element2 = obj2 as IBindingConfigurationElement;
                    if ((element2 != null) && element2.Name.Equals(configurationName, StringComparison.Ordinal))
                    {
                        if (resolvedBindings == null)
                        {
                            resolvedBindings = new List<string>();
                        }
                        string item = bindingSectionName + "/" + configurationName;
                        if (resolvedBindings.Contains(item))
                        {
                            ConfigurationElement element3 = (ConfigurationElement) obj2;
                            StringBuilder builder = new StringBuilder();
                            foreach (string str2 in resolvedBindings)
                            {
                                builder = builder.AppendFormat("{0}, ", str2);
                            }
                            builder = builder.Append(item);
                            resolvedBindings = null;
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigBindingReferenceCycleDetected", new object[] { builder.ToString() }), element3.ElementInformation.Source, element3.ElementInformation.LineNumber));
                        }
                        try
                        {
                            CheckAccess(obj2 as IConfigurationContextProviderInternal);
                            resolvedBindings.Add(item);
                            element2.ApplyConfiguration(binding);
                            resolvedBindings.Remove(item);
                        }
                        catch
                        {
                            if (resolvedBindings != null)
                            {
                                resolvedBindings = null;
                            }
                            throw;
                        }
                        if ((resolvedBindings != null) && (resolvedBindings.Count == 0))
                        {
                            resolvedBindings = null;
                        }
                        flag = true;
                    }
                }
                if (!flag)
                {
                    binding = null;
                }
            }
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>(3) {
                    ["FoundBinding"] = binding != null
                };
                bool flag2 = string.IsNullOrEmpty(configurationName);
                TraceCode getConfiguredBinding = TraceCode.GetConfiguredBinding;
                if (flag2)
                {
                    getConfiguredBinding = TraceCode.GetDefaultConfiguredBinding;
                }
                else
                {
                    dictionary["Name"] = string.IsNullOrEmpty(configurationName) ? System.ServiceModel.SR.GetString("Default") : configurationName;
                }
                dictionary["Binding"] = bindingSectionName;
                TraceUtility.TraceEvent(TraceEventType.Verbose, getConfiguredBinding, new DictionaryTraceRecord(dictionary), null, null);
            }
            return binding;
        }

        [SecurityCritical]
        private ChannelEndpointElement LookupChannel(string configurationName, string contractName, bool wildcard)
        {
            ClientSection section = (this.configurationContext == null) ? ClientSection.UnsafeGetSection() : ClientSection.UnsafeGetSection(this.configurationContext);
            ChannelEndpointElement element = null;
            foreach (ChannelEndpointElement element2 in section.Endpoints)
            {
                if ((element2.Contract == contractName) && ((element2.Name == configurationName) || wildcard))
                {
                    if (element != null)
                    {
                        if (wildcard)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxConfigLoaderMultipleEndpointMatchesWildcard1", new object[] { contractName })));
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxConfigLoaderMultipleEndpointMatchesSpecified2", new object[] { contractName, configurationName })));
                    }
                    element = element2;
                }
            }
            if (element != null)
            {
                CheckAccess(element);
            }
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>(8) {
                    ["FoundChannelElement"] = element != null,
                    ["Name"] = configurationName,
                    ["ContractName"] = contractName
                };
                if (element != null)
                {
                    if (!string.IsNullOrEmpty(element.Binding))
                    {
                        dictionary["Binding"] = element.Binding;
                    }
                    if (!string.IsNullOrEmpty(element.BindingConfiguration))
                    {
                        dictionary["BindingConfiguration"] = element.BindingConfiguration;
                    }
                    if (element.Address != null)
                    {
                        dictionary["RemoteEndpointUri"] = element.Address.ToString();
                    }
                    if (!string.IsNullOrEmpty(element.ElementInformation.Source))
                    {
                        dictionary["ConfigurationFileSource"] = element.ElementInformation.Source;
                        dictionary["ConfigurationFileLineNumber"] = element.ElementInformation.LineNumber;
                    }
                }
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.GetChannelEndpointElement, new DictionaryTraceRecord(dictionary), null, null);
            }
            return element;
        }

        internal static ComContractElement LookupComContract(Guid contractIID)
        {
            ComContractsSection section = (ComContractsSection) ConfigurationHelpers.GetSection(ConfigurationStrings.ComContractsSectionPath);
            foreach (ComContractElement element in section.ComContracts)
            {
                Guid guid;
                if (DiagnosticUtility.Utility.TryCreateGuid(element.Contract, out guid) && (guid == contractIID))
                {
                    return element;
                }
            }
            return null;
        }

        [SecurityCritical]
        private static CommonBehaviorsSection LookupCommonBehaviors(ContextInformation context)
        {
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.GetCommonBehaviors, (Message) null);
            }
            if (context != null)
            {
                return CommonBehaviorsSection.UnsafeGetAssociatedSection(context);
            }
            return CommonBehaviorsSection.UnsafeGetSection();
        }

        internal ContractDescription LookupContract(string contractName, string serviceName)
        {
            ContractDescription description = this.contractResolver.ResolveContract(contractName);
            if (description != null)
            {
                return description;
            }
            if (contractName == "IMetadataExchange")
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SfxReflectedContractKeyNotFoundIMetadataExchange", new object[] { serviceName })));
            }
            if (contractName == string.Empty)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SfxReflectedContractKeyNotFoundEmpty", new object[] { serviceName })));
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SfxReflectedContractKeyNotFound2", new object[] { contractName, serviceName })));
        }

        [SecurityCritical]
        private static EndpointBehaviorElement LookupEndpointBehaviors(string behaviorName, ContextInformation context)
        {
            EndpointBehaviorElement element = null;
            if (!string.IsNullOrEmpty(behaviorName))
            {
                if (DiagnosticUtility.ShouldTraceVerbose)
                {
                    TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.GetBehaviorElement, new StringTraceRecord("BehaviorName", behaviorName), null, null);
                }
                BehaviorsSection section = null;
                if (context == null)
                {
                    section = BehaviorsSection.UnsafeGetSection();
                }
                else
                {
                    section = BehaviorsSection.UnsafeGetAssociatedSection(context);
                }
                if (section.EndpointBehaviors.ContainsKey(behaviorName))
                {
                    element = section.EndpointBehaviors[behaviorName];
                }
            }
            if (element != null)
            {
                CheckAccess(element);
            }
            return element;
        }

        [SecurityCritical]
        public ServiceElement LookupService(string serviceConfigurationName)
        {
            ServicesSection section = ServicesSection.UnsafeGetSection();
            ServiceElement element = null;
            ServiceElementCollection services = section.Services;
            for (int i = 0; i < services.Count; i++)
            {
                ServiceElement element2 = services[i];
                if (element2.Name == serviceConfigurationName)
                {
                    element = element2;
                }
            }
            if (element != null)
            {
                CheckAccess(element);
            }
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.GetServiceElement, new ServiceConfigurationTraceRecord(element), null, null);
            }
            return element;
        }

        [SecurityCritical]
        private static ServiceBehaviorElement LookupServiceBehaviors(string behaviorName, ContextInformation context)
        {
            ServiceBehaviorElement element = null;
            if (!string.IsNullOrEmpty(behaviorName))
            {
                if (DiagnosticUtility.ShouldTraceVerbose)
                {
                    TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.GetBehaviorElement, new StringTraceRecord("BehaviorName", behaviorName), null, null);
                }
                BehaviorsSection section = null;
                if (context == null)
                {
                    section = BehaviorsSection.UnsafeGetSection();
                }
                else
                {
                    section = BehaviorsSection.UnsafeGetAssociatedSection(context);
                }
                if (section.ServiceBehaviors.ContainsKey(behaviorName))
                {
                    element = section.ServiceBehaviors[behaviorName];
                }
            }
            if (element != null)
            {
                CheckAccess(element);
            }
            return element;
        }

        [SecurityCritical]
        private static bool ShouldSkipCommonBehavior(Type behaviorType, ref bool? isPT)
        {
            bool flag = false;
            if (!isPT.HasValue)
            {
                if (!PartialTrustHelpers.IsTypeAptca(behaviorType))
                {
                    isPT = new bool?(!ThreadHasConfigurationPermission());
                    flag = isPT.Value;
                }
                return flag;
            }
            if (isPT.Value)
            {
                flag = !PartialTrustHelpers.IsTypeAptca(behaviorType);
            }
            return flag;
        }

        [SecurityCritical]
        private static bool ThreadHasConfigurationPermission()
        {
            try
            {
                ConfigurationPermission.Demand();
            }
            catch (SecurityException)
            {
                return false;
            }
            return true;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static void TraceBehaviorWarning(BehaviorExtensionElement behaviorExtension, TraceCode traceCode, Type type, Type behaviorType)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                Hashtable dictionary = new Hashtable(3) {
                    { 
                        "ConfigurationElementName",
                        behaviorExtension.ConfigurationElementName
                    },
                    { 
                        "ConfigurationType",
                        type.AssemblyQualifiedName
                    },
                    { 
                        "BehaviorType",
                        behaviorType.AssemblyQualifiedName
                    }
                };
                TraceUtility.TraceEvent(TraceEventType.Warning, traceCode, new DictionaryTraceRecord(dictionary), null, null);
            }
        }

        private static System.Configuration.ConfigurationPermission ConfigurationPermission
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (configurationPermission == null)
                {
                    configurationPermission = new System.Configuration.ConfigurationPermission(PermissionState.Unrestricted);
                }
                return configurationPermission;
            }
        }
    }
}


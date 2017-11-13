namespace System.ServiceModel.Description
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Dispatcher;
    using System.Xml;

    public class ServiceMetadataBehavior : IServiceBehavior
    {
        private static readonly Uri emptyUri = new Uri(string.Empty, UriKind.Relative);
        private Uri externalMetadataLocation;
        private Binding httpGetBinding;
        private bool httpGetEnabled;
        private Uri httpGetUrl;
        private Binding httpsGetBinding;
        private bool httpsGetEnabled;
        private Uri httpsGetUrl;
        private System.ServiceModel.Description.MetadataExporter metadataExporter;
        private ContractDescription mexContract;
        public const string MexContractName = "IMetadataExchange";
        internal const string MexContractNamespace = "http://schemas.microsoft.com/2006/04/mex";
        private object thisLock = new object();

        internal void AddImplementedContracts(ServiceHostBase.ServiceAndBehaviorsContractResolver resolver)
        {
            if (!resolver.BehaviorContracts.ContainsKey("IMetadataExchange"))
            {
                this.EnsureMexContractDescription();
                resolver.BehaviorContracts.Add("IMetadataExchange", this.mexContract);
            }
        }

        private void ApplyBehavior(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase host)
        {
            ServiceMetadataExtension mex = ServiceMetadataExtension.EnsureServiceMetadataExtension(description, host);
            this.SetExtensionProperties(description, host, mex);
            CustomizeMetadataEndpoints(description, host, mex);
            this.CreateHttpGetEndpoints(description, host, mex);
        }

        private static bool BehaviorMissingObjectNullOrServiceImplements(System.ServiceModel.Description.ServiceDescription description, object obj) => 
            ((obj == null) || (((description.Behaviors != null) && (description.Behaviors.Find<ServiceMetadataBehavior>() == null)) || ((description.ServiceType != null) && (description.ServiceType.GetInterface(typeof(IMetadataExchange).Name) != null))));

        private void CreateHttpGetEndpoints(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase host, ServiceMetadataExtension mex)
        {
            if (this.httpGetEnabled)
            {
                ((ServiceMetadataExtension.HttpGetImpl) EnsureGetDispatcher(host, mex, this.httpGetUrl, Uri.UriSchemeHttp).Endpoints[0].DispatchRuntime.SingletonInstanceContext.UserObject).GetWsdlEnabled = true;
            }
            if (this.httpsGetEnabled)
            {
                ((ServiceMetadataExtension.HttpGetImpl) EnsureGetDispatcher(host, mex, this.httpsGetUrl, Uri.UriSchemeHttps).Endpoints[0].DispatchRuntime.SingletonInstanceContext.UserObject).GetWsdlEnabled = true;
            }
        }

        private static ContractDescription CreateMexContract()
        {
            ContractDescription contract = ContractDescription.GetContract(typeof(IMetadataExchange));
            foreach (OperationDescription description2 in contract.Operations)
            {
                description2.Behaviors.Find<OperationBehaviorAttribute>().Impersonation = ImpersonationOption.Allowed;
            }
            contract.Behaviors.Add(new ServiceMetadataContractBehavior(true));
            return contract;
        }

        private static void CustomizeMetadataEndpoints(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase host, ServiceMetadataExtension mex)
        {
            for (int i = 0; i < host.ChannelDispatchers.Count; i++)
            {
                ChannelDispatcher channelDispatcher = host.ChannelDispatchers[i] as ChannelDispatcher;
                if ((channelDispatcher != null) && IsMetadataTransferDispatcher(description, channelDispatcher))
                {
                    if (channelDispatcher.Endpoints.Count != 1)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxServiceMetadataBehaviorInstancingError", new object[] { channelDispatcher.Listener.Uri, channelDispatcher.CreateContractListString() })));
                    }
                    DispatchRuntime dispatchRuntime = channelDispatcher.Endpoints[0].DispatchRuntime;
                    dispatchRuntime.InstanceContextProvider = InstanceContextProviderBase.GetProviderForMode(InstanceContextMode.Single, dispatchRuntime);
                    Uri listenUri = channelDispatcher.Listener.Uri;
                    ServiceMetadataExtension.WSMexImpl implementation = new ServiceMetadataExtension.WSMexImpl(mex, listenUri);
                    dispatchRuntime.SingletonInstanceContext = new InstanceContext(host, implementation, false);
                    implementation.IsListeningOnHttps = channelDispatcher.Listener.Uri.Scheme == Uri.UriSchemeHttps;
                }
            }
        }

        private static ChannelDispatcher EnsureGetDispatcher(ServiceHostBase host, ServiceMetadataExtension mex, Uri url, string scheme)
        {
            Uri via = host.GetVia(scheme, (url == null) ? new Uri(string.Empty, UriKind.Relative) : url);
            if (via != null)
            {
                return mex.EnsureGetDispatcher(via, false);
            }
            if (scheme == Uri.UriSchemeHttp)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxServiceMetadataBehaviorNoHttpBaseAddress")));
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxServiceMetadataBehaviorNoHttpsBaseAddress")));
        }

        private void EnsureMexContractDescription()
        {
            if (this.mexContract == null)
            {
                lock (this.thisLock)
                {
                    if (this.mexContract == null)
                    {
                        this.mexContract = CreateMexContract();
                    }
                }
            }
        }

        private static EndpointDispatcher GetListenerByID(SynchronizedCollection<ChannelDispatcherBase> channelDispatchers, string id)
        {
            for (int i = 0; i < channelDispatchers.Count; i++)
            {
                ChannelDispatcher dispatcher = channelDispatchers[i] as ChannelDispatcher;
                if (dispatcher != null)
                {
                    for (int j = 0; j < dispatcher.Endpoints.Count; j++)
                    {
                        EndpointDispatcher dispatcher2 = dispatcher.Endpoints[j];
                        if (dispatcher2.Id == id)
                        {
                            return dispatcher2;
                        }
                    }
                }
            }
            return null;
        }

        internal static bool IsHttpGetMetadataDispatcher(System.ServiceModel.Description.ServiceDescription description, ChannelDispatcher channelDispatcher)
        {
            if (description.Behaviors.Find<ServiceMetadataBehavior>() != null)
            {
                foreach (EndpointDispatcher dispatcher in channelDispatcher.Endpoints)
                {
                    if ((dispatcher.ContractName == "IHttpGetHelpPageAndMetadataContract") && (dispatcher.ContractNamespace == "http://schemas.microsoft.com/2006/04/http/metadata"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool IsMetadataDispatcher(System.ServiceModel.Description.ServiceDescription description, ChannelDispatcher channelDispatcher)
        {
            using (IEnumerator<EndpointDispatcher> enumerator = channelDispatcher.Endpoints.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    EndpointDispatcher current = enumerator.Current;
                    if (IsMetadataTransferDispatcher(description, channelDispatcher) || IsHttpGetMetadataDispatcher(description, channelDispatcher))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsMetadataEndpoint(ServiceEndpoint endpoint) => 
            ((endpoint.Contract.Name == "IMetadataExchange") && (endpoint.Contract.Namespace == "http://schemas.microsoft.com/2006/04/mex"));

        internal static bool IsMetadataEndpoint(System.ServiceModel.Description.ServiceDescription description, ServiceEndpoint endpoint)
        {
            if (BehaviorMissingObjectNullOrServiceImplements(description, endpoint))
            {
                return false;
            }
            return IsMetadataEndpoint(endpoint);
        }

        internal static bool IsMetadataImplementedType(Type type) => 
            (type == typeof(IMetadataExchange));

        internal static bool IsMetadataImplementedType(System.ServiceModel.Description.ServiceDescription description, Type type)
        {
            if (BehaviorMissingObjectNullOrServiceImplements(description, type))
            {
                return false;
            }
            return (type == typeof(IMetadataExchange));
        }

        private static bool IsMetadataTransferDispatcher(System.ServiceModel.Description.ServiceDescription description, ChannelDispatcher channelDispatcher)
        {
            if (!BehaviorMissingObjectNullOrServiceImplements(description, channelDispatcher))
            {
                foreach (EndpointDispatcher dispatcher in channelDispatcher.Endpoints)
                {
                    if ((dispatcher.ContractName == "IMetadataExchange") && (dispatcher.ContractNamespace == "http://schemas.microsoft.com/2006/04/mex"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void SetExtensionProperties(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase host, ServiceMetadataExtension mex)
        {
            mex.ExternalMetadataLocation = this.ExternalMetadataLocation;
            mex.Initializer = new MetadataExtensionInitializer(this, description, host);
            mex.HttpGetEnabled = this.httpGetEnabled;
            mex.HttpsGetEnabled = this.httpsGetEnabled;
            mex.HttpGetUrl = host.GetVia(Uri.UriSchemeHttp, (this.httpGetUrl == null) ? new Uri(string.Empty, UriKind.Relative) : this.httpGetUrl);
            mex.HttpsGetUrl = host.GetVia(Uri.UriSchemeHttps, (this.httpsGetUrl == null) ? new Uri(string.Empty, UriKind.Relative) : this.httpsGetUrl);
            mex.HttpGetBinding = this.httpGetBinding;
            mex.HttpsGetBinding = this.httpsGetBinding;
            UseRequestHeadersForMetadataAddressBehavior behavior = description.Behaviors.Find<UseRequestHeadersForMetadataAddressBehavior>();
            if (behavior != null)
            {
                mex.UpdateAddressDynamically = true;
                mex.UpdatePortsByScheme = new Dictionary<string, int>(behavior.DefaultPortsByScheme);
            }
            foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
                if (IsMetadataTransferDispatcher(description, dispatcher))
                {
                    mex.MexEnabled = true;
                    mex.MexUrl = dispatcher.Listener.Uri;
                    if (behavior != null)
                    {
                        foreach (EndpointDispatcher dispatcher2 in dispatcher.Endpoints)
                        {
                            if (!dispatcher2.AddressFilterSetExplicit)
                            {
                                dispatcher2.AddressFilter = new MatchAllMessageFilter();
                            }
                        }
                    }
                    break;
                }
            }
        }

        void IServiceBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            if (description == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("description");
            }
            if (serviceHostBase == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serviceHostBase");
            }
            this.ApplyBehavior(description, serviceHostBase);
        }

        void IServiceBehavior.Validate(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        public Uri ExternalMetadataLocation
        {
            get => 
                this.externalMetadataLocation;
            set
            {
                if (((value != null) && value.IsAbsoluteUri) && ((value.Scheme != Uri.UriSchemeHttp) && (value.Scheme != Uri.UriSchemeHttps)))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("ExternalMetadataLocation", System.ServiceModel.SR.GetString("SFxBadMetadataLocationUri", new object[] { value.OriginalString, value.Scheme }));
                }
                this.externalMetadataLocation = value;
            }
        }

        public Binding HttpGetBinding
        {
            get => 
                this.httpGetBinding;
            set
            {
                if (value != null)
                {
                    if (!value.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxBindingSchemeDoesNotMatch", new object[] { value.Scheme, value.GetType().ToString(), Uri.UriSchemeHttp }));
                    }
                    CustomBinding binding = new CustomBinding(value);
                    TextMessageEncodingBindingElement element = binding.Elements.Find<TextMessageEncodingBindingElement>();
                    if ((element != null) && !element.MessageVersion.IsMatch(MessageVersion.None))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxIncorrectMessageVersion", new object[] { element.MessageVersion.ToString(), MessageVersion.None.ToString() }));
                    }
                    HttpTransportBindingElement element2 = binding.Elements.Find<HttpTransportBindingElement>();
                    if (element2 != null)
                    {
                        element2.Method = "GET";
                    }
                    this.httpGetBinding = binding;
                }
            }
        }

        public bool HttpGetEnabled
        {
            get => 
                this.httpGetEnabled;
            set
            {
                this.httpGetEnabled = value;
            }
        }

        public Uri HttpGetUrl
        {
            get => 
                this.httpGetUrl;
            set
            {
                if (((value != null) && value.IsAbsoluteUri) && (value.Scheme != Uri.UriSchemeHttp))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxServiceMetadataBehaviorUrlMustBeHttpOrRelative", new object[] { "HttpGetUrl", Uri.UriSchemeHttp, value.ToString(), value.Scheme }));
                }
                this.httpGetUrl = value;
            }
        }

        public Binding HttpsGetBinding
        {
            get => 
                this.httpsGetBinding;
            set
            {
                if (value != null)
                {
                    if (!value.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxBindingSchemeDoesNotMatch", new object[] { value.Scheme, value.GetType().ToString(), Uri.UriSchemeHttps }));
                    }
                    CustomBinding binding = new CustomBinding(value);
                    TextMessageEncodingBindingElement element = binding.Elements.Find<TextMessageEncodingBindingElement>();
                    if ((element != null) && !element.MessageVersion.IsMatch(MessageVersion.None))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxIncorrectMessageVersion", new object[] { element.MessageVersion.ToString(), MessageVersion.None.ToString() }));
                    }
                    HttpsTransportBindingElement element2 = binding.Elements.Find<HttpsTransportBindingElement>();
                    if (element2 != null)
                    {
                        element2.Method = "GET";
                    }
                    this.httpsGetBinding = binding;
                }
            }
        }

        public bool HttpsGetEnabled
        {
            get => 
                this.httpsGetEnabled;
            set
            {
                this.httpsGetEnabled = value;
            }
        }

        public Uri HttpsGetUrl
        {
            get => 
                this.httpsGetUrl;
            set
            {
                if (((value != null) && value.IsAbsoluteUri) && (value.Scheme != Uri.UriSchemeHttps))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxServiceMetadataBehaviorUrlMustBeHttpOrRelative", new object[] { "HttpsGetUrl", Uri.UriSchemeHttps, value.ToString(), value.Scheme }));
                }
                this.httpsGetUrl = value;
            }
        }

        public System.ServiceModel.Description.MetadataExporter MetadataExporter
        {
            get
            {
                if (this.metadataExporter == null)
                {
                    this.metadataExporter = new WsdlExporter();
                }
                return this.metadataExporter;
            }
            set
            {
                this.metadataExporter = value;
            }
        }

        internal class MetadataExtensionInitializer
        {
            private ServiceMetadataBehavior behavior;
            private System.ServiceModel.Description.ServiceDescription description;
            private ServiceHostBase host;
            private Exception metadataGenerationException;

            internal MetadataExtensionInitializer(ServiceMetadataBehavior behavior, System.ServiceModel.Description.ServiceDescription description, ServiceHostBase host)
            {
                this.behavior = behavior;
                this.description = description;
                this.host = host;
            }

            internal MetadataSet GenerateMetadata()
            {
                if ((this.behavior.ExternalMetadataLocation == null) || (this.behavior.ExternalMetadataLocation.ToString() == string.Empty))
                {
                    if (this.metadataGenerationException != null)
                    {
                        throw this.metadataGenerationException;
                    }
                    try
                    {
                        MetadataExporter metadataExporter = this.behavior.MetadataExporter;
                        XmlQualifiedName wsdlServiceQName = new XmlQualifiedName(this.description.Name, this.description.Namespace);
                        Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
                        foreach (ServiceEndpoint endpoint in this.description.Endpoints)
                        {
                            ServiceMetadataContractBehavior behavior = endpoint.Contract.Behaviors.Find<ServiceMetadataContractBehavior>();
                            if ((behavior == null) || !behavior.MetadataGenerationDisabled)
                            {
                                EndpointAddress endpointAddress = null;
                                EndpointDispatcher listenerByID = ServiceMetadataBehavior.GetListenerByID(this.host.ChannelDispatchers, endpoint.Id);
                                if (listenerByID != null)
                                {
                                    endpointAddress = listenerByID.EndpointAddress;
                                }
                                ServiceEndpoint item = new ServiceEndpoint(endpoint.Contract) {
                                    Binding = endpoint.Binding,
                                    Name = endpoint.Name,
                                    Address = endpointAddress
                                };
                                foreach (IEndpointBehavior behavior2 in endpoint.Behaviors)
                                {
                                    item.Behaviors.Add(behavior2);
                                }
                                endpoints.Add(item);
                            }
                        }
                        WsdlExporter exporter2 = metadataExporter as WsdlExporter;
                        if (exporter2 != null)
                        {
                            exporter2.ExportEndpoints(endpoints, wsdlServiceQName);
                        }
                        else
                        {
                            foreach (ServiceEndpoint endpoint3 in endpoints)
                            {
                                metadataExporter.ExportEndpoint(endpoint3);
                            }
                        }
                        if ((metadataExporter.Errors.Count > 0) && DiagnosticUtility.ShouldTraceWarning)
                        {
                            TraceWsdlExportErrors(metadataExporter);
                        }
                        return metadataExporter.GetGeneratedMetadata();
                    }
                    catch (Exception exception)
                    {
                        this.metadataGenerationException = exception;
                        throw;
                    }
                }
                return null;
            }

            private static void TraceWsdlExportErrors(MetadataExporter exporter)
            {
                foreach (MetadataConversionError error in exporter.Errors)
                {
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        Hashtable dictionary = new Hashtable(2) {
                            { 
                                "IsWarning",
                                error.IsWarning
                            },
                            { 
                                "Message",
                                error.Message
                            }
                        };
                        TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.WsmexNonCriticalWsdlExportError, new DictionaryTraceRecord(dictionary), null, null);
                    }
                }
            }
        }
    }
}


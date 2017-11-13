namespace System.ServiceModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Transactions;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceBehaviorAttribute : Attribute, IServiceBehavior
    {
        private System.ServiceModel.AddressFilterMode addressFilterMode;
        private bool automaticSessionShutdown = true;
        private System.ServiceModel.ConcurrencyMode concurrencyMode;
        private string configurationName;
        internal static IsolationLevel DefaultIsolationLevel = IsolationLevel.Unspecified;
        private object hiddenSingleton;
        private bool ignoreExtensionDataObject;
        private bool includeExceptionDetailInFaults;
        private System.ServiceModel.InstanceContextMode instanceMode;
        private IInstanceProvider instanceProvider;
        private bool isolationLevelSet;
        private int maxItemsInObjectGraph = 0x10000;
        private bool releaseServiceInstanceOnTransactionComplete = true;
        private bool releaseServiceInstanceOnTransactionCompleteSet;
        private string serviceName;
        private string serviceNamespace;
        private bool transactionAutoCompleteOnSessionClose;
        private bool transactionAutoCompleteOnSessionCloseSet;
        private IsolationLevel transactionIsolationLevel = DefaultIsolationLevel;
        private TimeSpan transactionTimeout = TimeSpan.Zero;
        private bool transactionTimeoutSet;
        private string transactionTimeoutString;
        private bool useSynchronizationContext = true;
        private bool validateMustUnderstand = true;
        private object wellKnownSingleton;

        private void ApplyInstancing(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            Type serviceType = description.ServiceType;
            InstanceContext context = null;
            for (int i = 0; i < serviceHostBase.ChannelDispatchers.Count; i++)
            {
                ChannelDispatcher channelDispatcher = serviceHostBase.ChannelDispatchers[i] as ChannelDispatcher;
                if ((channelDispatcher != null) && !ServiceMetadataBehavior.IsMetadataDispatcher(description, channelDispatcher))
                {
                    foreach (EndpointDispatcher dispatcher2 in channelDispatcher.Endpoints)
                    {
                        DispatchRuntime dispatchRuntime = dispatcher2.DispatchRuntime;
                        if (dispatchRuntime.InstanceProvider == null)
                        {
                            if (this.instanceProvider == null)
                            {
                                if ((serviceType == null) && (this.wellKnownSingleton == null))
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("InstanceSettingsMustHaveTypeOrWellKnownObject0")));
                                }
                                if ((this.instanceMode != System.ServiceModel.InstanceContextMode.Single) && (this.wellKnownSingleton != null))
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxWellKnownNonSingleton0")));
                                }
                            }
                            else
                            {
                                dispatchRuntime.InstanceProvider = this.instanceProvider;
                            }
                        }
                        dispatchRuntime.Type = serviceType;
                        dispatchRuntime.InstanceContextProvider = InstanceContextProviderBase.GetProviderForMode(this.instanceMode, dispatchRuntime);
                        if ((this.instanceMode == System.ServiceModel.InstanceContextMode.Single) && (dispatchRuntime.SingletonInstanceContext == null))
                        {
                            if (context == null)
                            {
                                if (this.wellKnownSingleton != null)
                                {
                                    context = new InstanceContext(serviceHostBase, this.wellKnownSingleton, true, false);
                                }
                                else if (this.hiddenSingleton != null)
                                {
                                    context = new InstanceContext(serviceHostBase, this.hiddenSingleton, false, false);
                                }
                                else
                                {
                                    context = new InstanceContext(serviceHostBase, false);
                                }
                                context.AutoClose = false;
                            }
                            dispatchRuntime.SingletonInstanceContext = context;
                        }
                    }
                }
            }
        }

        internal object GetHiddenSingleton() => 
            this.hiddenSingleton;

        public object GetWellKnownSingleton() => 
            this.wellKnownSingleton;

        internal void SetHiddenSingleton(object value)
        {
            if (value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
            }
            this.hiddenSingleton = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SetIsolationLevel(ChannelDispatcher channelDispatcher)
        {
            if (channelDispatcher == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelDispatcher");
            }
            channelDispatcher.TransactionIsolationLevel = this.transactionIsolationLevel;
        }

        public void SetWellKnownSingleton(object value)
        {
            if (value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
            }
            this.wellKnownSingleton = value;
        }

        void IServiceBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            for (int i = 0; i < serviceHostBase.ChannelDispatchers.Count; i++)
            {
                ChannelDispatcher channelDispatcher = serviceHostBase.ChannelDispatchers[i] as ChannelDispatcher;
                if (channelDispatcher != null)
                {
                    channelDispatcher.IncludeExceptionDetailInFaults = this.includeExceptionDetailInFaults;
                    if (!ServiceMetadataBehavior.IsMetadataDispatcher(description, channelDispatcher))
                    {
                        channelDispatcher.TransactionTimeout = this.transactionTimeout;
                        if (this.isolationLevelSet)
                        {
                            this.SetIsolationLevel(channelDispatcher);
                        }
                        foreach (EndpointDispatcher dispatcher2 in channelDispatcher.Endpoints)
                        {
                            DispatchRuntime dispatchRuntime = dispatcher2.DispatchRuntime;
                            dispatchRuntime.ConcurrencyMode = this.concurrencyMode;
                            dispatchRuntime.ValidateMustUnderstand = this.validateMustUnderstand;
                            dispatchRuntime.AutomaticInputSessionShutdown = this.automaticSessionShutdown;
                            dispatchRuntime.TransactionAutoCompleteOnSessionClose = this.transactionAutoCompleteOnSessionClose;
                            dispatchRuntime.ReleaseServiceInstanceOnTransactionComplete = this.releaseServiceInstanceOnTransactionComplete;
                            if (!this.useSynchronizationContext)
                            {
                                dispatchRuntime.SynchronizationContext = null;
                            }
                            if (!dispatcher2.AddressFilterSetExplicit)
                            {
                                EndpointAddress originalAddress = dispatcher2.OriginalAddress;
                                if ((originalAddress == null) || (this.AddressFilterMode == System.ServiceModel.AddressFilterMode.Any))
                                {
                                    dispatcher2.AddressFilter = new MatchAllMessageFilter();
                                }
                                else if (this.AddressFilterMode == System.ServiceModel.AddressFilterMode.Prefix)
                                {
                                    dispatcher2.AddressFilter = new PrefixEndpointAddressMessageFilter(originalAddress);
                                }
                                else if (this.AddressFilterMode == System.ServiceModel.AddressFilterMode.Exact)
                                {
                                    dispatcher2.AddressFilter = new EndpointAddressMessageFilter(originalAddress);
                                }
                            }
                        }
                    }
                }
            }
            DataContractSerializerServiceBehavior.ApplySerializationSettings(description, this.ignoreExtensionDataObject, this.maxItemsInObjectGraph);
            this.ApplyInstancing(description, serviceHostBase);
        }

        void IServiceBehavior.Validate(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        public System.ServiceModel.AddressFilterMode AddressFilterMode
        {
            get => 
                this.addressFilterMode;
            set
            {
                if (!AddressFilterModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.addressFilterMode = value;
            }
        }

        public bool AutomaticSessionShutdown
        {
            get => 
                this.automaticSessionShutdown;
            set
            {
                this.automaticSessionShutdown = value;
            }
        }

        public System.ServiceModel.ConcurrencyMode ConcurrencyMode
        {
            get => 
                this.concurrencyMode;
            set
            {
                if (!ConcurrencyModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.concurrencyMode = value;
            }
        }

        public string ConfigurationName
        {
            get => 
                this.configurationName;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value == string.Empty)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", System.ServiceModel.SR.GetString("SFxConfigurationNameCannotBeEmpty")));
                }
                this.configurationName = value;
            }
        }

        public bool IgnoreExtensionDataObject
        {
            get => 
                this.ignoreExtensionDataObject;
            set
            {
                this.ignoreExtensionDataObject = value;
            }
        }

        public bool IncludeExceptionDetailInFaults
        {
            get => 
                this.includeExceptionDetailInFaults;
            set
            {
                this.includeExceptionDetailInFaults = value;
            }
        }

        public System.ServiceModel.InstanceContextMode InstanceContextMode
        {
            get => 
                this.instanceMode;
            set
            {
                if (!InstanceContextModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.instanceMode = value;
            }
        }

        internal IInstanceProvider InstanceProvider
        {
            set
            {
                this.instanceProvider = value;
            }
        }

        internal bool IsolationLevelSet =>
            this.isolationLevelSet;

        public int MaxItemsInObjectGraph
        {
            get => 
                this.maxItemsInObjectGraph;
            set
            {
                this.maxItemsInObjectGraph = value;
            }
        }

        public string Name
        {
            get => 
                this.serviceName;
            set
            {
                this.serviceName = value;
            }
        }

        public string Namespace
        {
            get => 
                this.serviceNamespace;
            set
            {
                this.serviceNamespace = value;
            }
        }

        public bool ReleaseServiceInstanceOnTransactionComplete
        {
            get => 
                this.releaseServiceInstanceOnTransactionComplete;
            set
            {
                this.releaseServiceInstanceOnTransactionComplete = value;
                this.releaseServiceInstanceOnTransactionCompleteSet = true;
            }
        }

        internal bool ReleaseServiceInstanceOnTransactionCompleteSet =>
            this.releaseServiceInstanceOnTransactionCompleteSet;

        public bool TransactionAutoCompleteOnSessionClose
        {
            get => 
                this.transactionAutoCompleteOnSessionClose;
            set
            {
                this.transactionAutoCompleteOnSessionClose = value;
                this.transactionAutoCompleteOnSessionCloseSet = true;
            }
        }

        internal bool TransactionAutoCompleteOnSessionCloseSet =>
            this.transactionAutoCompleteOnSessionCloseSet;

        public IsolationLevel TransactionIsolationLevel
        {
            get => 
                this.transactionIsolationLevel;
            set
            {
                switch (value)
                {
                    case IsolationLevel.Serializable:
                    case IsolationLevel.RepeatableRead:
                    case IsolationLevel.ReadCommitted:
                    case IsolationLevel.ReadUncommitted:
                    case IsolationLevel.Snapshot:
                    case IsolationLevel.Chaos:
                    case IsolationLevel.Unspecified:
                        this.transactionIsolationLevel = value;
                        this.isolationLevelSet = true;
                        return;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
            }
        }

        public string TransactionTimeout
        {
            get => 
                this.transactionTimeoutString;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                try
                {
                    TimeSpan span = TimeSpan.Parse(value);
                    if (span < TimeSpan.Zero)
                    {
                        string message = System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0");
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, message));
                    }
                    this.transactionTimeout = span;
                    this.transactionTimeoutString = value;
                    this.transactionTimeoutSet = true;
                }
                catch (FormatException exception)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("SFxTimeoutInvalidStringFormat"), "value", exception));
                }
                catch (OverflowException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
            }
        }

        internal bool TransactionTimeoutSet =>
            this.transactionTimeoutSet;

        internal TimeSpan TransactionTimeoutTimespan =>
            this.transactionTimeout;

        public bool UseSynchronizationContext
        {
            get => 
                this.useSynchronizationContext;
            set
            {
                this.useSynchronizationContext = value;
            }
        }

        public bool ValidateMustUnderstand
        {
            get => 
                this.validateMustUnderstand;
            set
            {
                this.validateMustUnderstand = value;
            }
        }
    }
}


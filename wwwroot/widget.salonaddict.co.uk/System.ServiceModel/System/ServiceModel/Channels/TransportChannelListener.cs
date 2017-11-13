namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;

    internal abstract class TransportChannelListener : ChannelListenerBase, ITransportFactorySettings, IDefaultCommunicationTimeouts
    {
        private ServiceModelActivity activity;
        private ServiceSecurityAuditBehavior auditBehavior;
        private System.Uri baseUri;
        private System.ServiceModel.Channels.BufferManager bufferManager;
        private static string exactGeneratedAddressPrefix;
        private string hostedVirtualPath;
        private HostNameComparisonMode hostNameComparisonMode;
        private bool inheritBaseAddressSettings;
        private bool manualAddressing;
        private long maxBufferPoolSize;
        private long maxReceivedMessageSize;
        private System.ServiceModel.Channels.MessageEncoderFactory messageEncoderFactory;
        private MessageReceivedCallback messageReceivedCallback;
        private System.ServiceModel.Channels.MessageVersion messageVersion;
        private static object staticLock = new object();
        private static string strongWildcardGeneratedAddressPrefix;
        private TransportManagerContainer transportManagerContainer;
        private System.Uri uri;
        private static string weakWildcardGeneratedAddressPrefix;

        protected TransportChannelListener(TransportBindingElement bindingElement, BindingContext context) : this(bindingElement, context, TransportDefaults.GetDefaultMessageEncoderFactory())
        {
        }

        protected TransportChannelListener(TransportBindingElement bindingElement, BindingContext context, System.ServiceModel.Channels.MessageEncoderFactory defaultMessageEncoderFactory) : this(bindingElement, context, defaultMessageEncoderFactory, HostNameComparisonMode.Exact)
        {
        }

        protected TransportChannelListener(TransportBindingElement bindingElement, BindingContext context, HostNameComparisonMode hostNameComparisonMode) : this(bindingElement, context, TransportDefaults.GetDefaultMessageEncoderFactory(), hostNameComparisonMode)
        {
        }

        protected TransportChannelListener(TransportBindingElement bindingElement, BindingContext context, System.ServiceModel.Channels.MessageEncoderFactory defaultMessageEncoderFactory, HostNameComparisonMode hostNameComparisonMode) : base(context.Binding)
        {
            HostNameComparisonModeHelper.Validate(hostNameComparisonMode);
            this.hostNameComparisonMode = hostNameComparisonMode;
            this.manualAddressing = bindingElement.ManualAddressing;
            this.maxBufferPoolSize = bindingElement.MaxBufferPoolSize;
            this.maxReceivedMessageSize = bindingElement.MaxReceivedMessageSize;
            Collection<MessageEncodingBindingElement> collection = context.BindingParameters.FindAll<MessageEncodingBindingElement>();
            if (collection.Count > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("MultipleMebesInParameters")));
            }
            if (collection.Count == 1)
            {
                this.messageEncoderFactory = collection[0].CreateMessageEncoderFactory();
                context.BindingParameters.Remove<MessageEncodingBindingElement>();
            }
            else
            {
                this.messageEncoderFactory = defaultMessageEncoderFactory;
            }
            if (this.messageEncoderFactory != null)
            {
                this.messageVersion = this.messageEncoderFactory.MessageVersion;
            }
            else
            {
                this.messageVersion = System.ServiceModel.Channels.MessageVersion.None;
            }
            ServiceSecurityAuditBehavior behavior = context.BindingParameters.Find<ServiceSecurityAuditBehavior>();
            if (behavior != null)
            {
                this.auditBehavior = behavior.Clone();
            }
            else
            {
                this.auditBehavior = new ServiceSecurityAuditBehavior();
            }
            if ((context.ListenUriMode == ListenUriMode.Unique) && (context.ListenUriBaseAddress == null))
            {
                UriBuilder builder = new UriBuilder(this.Scheme, DnsCache.MachineName) {
                    Path = this.GeneratedAddressPrefix
                };
                context.ListenUriBaseAddress = builder.Uri;
            }
            UriSchemeKeyedCollection.ValidateBaseAddress(context.ListenUriBaseAddress, "baseAddress");
            if ((context.ListenUriBaseAddress.Scheme != this.Scheme) && (string.Compare(context.ListenUriBaseAddress.Scheme, this.Scheme, StringComparison.OrdinalIgnoreCase) != 0))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("context.ListenUriBaseAddress", System.ServiceModel.SR.GetString("InvalidUriScheme", new object[] { context.ListenUriBaseAddress.Scheme, this.Scheme }));
            }
            if (context.ListenUriMode == ListenUriMode.Explicit)
            {
                this.SetUri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress);
            }
            else
            {
                string listenUriRelativeAddress = context.ListenUriRelativeAddress;
                if ((listenUriRelativeAddress.Length > 0) && !listenUriRelativeAddress.EndsWith("/", StringComparison.Ordinal))
                {
                    listenUriRelativeAddress = listenUriRelativeAddress + "/";
                }
                this.SetUri(context.ListenUriBaseAddress, listenUriRelativeAddress + Guid.NewGuid().ToString());
            }
            this.transportManagerContainer = new TransportManagerContainer(this);
        }

        private static System.Uri AddSegment(System.Uri baseUri, System.Uri fullUri)
        {
            System.Uri uri = null;
            if (baseUri.AbsolutePath.Length >= fullUri.AbsolutePath.Length)
            {
                return uri;
            }
            UriBuilder uriBuilder = new UriBuilder(baseUri);
            TcpChannelListener.FixIpv6Hostname(uriBuilder, baseUri);
            if (!uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
            {
                uriBuilder.Path = uriBuilder.Path + "/";
                baseUri = uriBuilder.Uri;
            }
            string originalString = baseUri.MakeRelativeUri(fullUri).OriginalString;
            int index = originalString.IndexOf('/');
            string str2 = (index == -1) ? originalString : originalString.Substring(0, index);
            uriBuilder.Path = uriBuilder.Path + str2;
            return uriBuilder.Uri;
        }

        internal virtual void ApplyHostedContext(VirtualPathExtension virtualPathExtension, bool isMetadataListener)
        {
            this.hostedVirtualPath = virtualPathExtension.VirtualPath;
        }

        internal virtual ITransportManagerRegistration CreateTransportManagerRegistration() => 
            this.CreateTransportManagerRegistration(this.BaseUri);

        internal abstract ITransportManagerRegistration CreateTransportManagerRegistration(System.Uri listenUri);
        private static string GetGeneratedAddressPrefix(ref string generatedAddressPrefix)
        {
            if (generatedAddressPrefix == null)
            {
                lock (staticLock)
                {
                    if (generatedAddressPrefix == null)
                    {
                        generatedAddressPrefix = "Temporary_Listen_Addresses/" + Guid.NewGuid().ToString();
                    }
                }
            }
            return generatedAddressPrefix;
        }

        internal virtual int GetMaxBufferSize()
        {
            if (this.MaxReceivedMessageSize > 0x7fffffffL)
            {
                return 0x7fffffff;
            }
            return (int) this.MaxReceivedMessageSize;
        }

        public override T GetProperty<T>() where T: class
        {
            if (typeof(T) == typeof(System.ServiceModel.Channels.MessageVersion))
            {
                return (T) this.MessageVersion;
            }
            if (typeof(T) != typeof(FaultConverter))
            {
                return base.GetProperty<T>();
            }
            return this.MessageEncoderFactory?.Encoder.GetProperty<T>();
        }

        internal TransportManagerContainer GetTransportManagers() => 
            TransportManagerContainer.TransferTransportManagers(this.transportManagerContainer);

        internal bool IsScopeIdCompatible(HostNameComparisonMode hostNameComparisonMode, System.Uri uri)
        {
            if (this.hostNameComparisonMode != hostNameComparisonMode)
            {
                return false;
            }
            if ((hostNameComparisonMode == HostNameComparisonMode.Exact) && (uri.HostNameType == UriHostNameType.IPv6))
            {
                if (this.Uri.HostNameType != UriHostNameType.IPv6)
                {
                    return false;
                }
                IPAddress address = IPAddress.Parse(this.Uri.DnsSafeHost);
                IPAddress address2 = IPAddress.Parse(uri.DnsSafeHost);
                if (address.ScopeId != address2.ScopeId)
                {
                    return false;
                }
            }
            return true;
        }

        protected override void OnAbort()
        {
            this.transportManagerContainer.Close();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
            this.transportManagerContainer.BeginClose(timeout, callback, state);

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => 
            this.transportManagerContainer.BeginOpen(new SelectTransportManagersCallback(this.SelectTransportManagers), callback, state);

        protected override void OnClose(TimeSpan timeout)
        {
            this.transportManagerContainer.Close();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            if (this.bufferManager != null)
            {
                this.bufferManager.Clear();
            }
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.transportManagerContainer.EndClose(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.transportManagerContainer.EndOpen(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.transportManagerContainer.Open(new SelectTransportManagersCallback(this.SelectTransportManagers));
        }

        protected override void OnOpened()
        {
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.OpenedListener, new UriTraceRecord(this.Uri), this, null);
            }
            base.OnOpened();
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            if (this.HostedVirtualPath != null)
            {
                HostedTransportConfigurationBase configuration = HostedTransportConfigurationManager.GetConfiguration(this.Scheme) as HostedTransportConfigurationBase;
                if (configuration != null)
                {
                    BaseUriWithWildcard wildcard = configuration.FindBaseAddress(this.Uri);
                    if (wildcard == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_TransportBindingNotFound", new object[] { this.Uri.ToString() })));
                    }
                    this.hostNameComparisonMode = wildcard.HostNameComparisonMode;
                }
            }
            this.bufferManager = System.ServiceModel.Channels.BufferManager.CreateBufferManager(this.MaxBufferPoolSize, this.GetMaxBufferSize());
        }

        internal void RaiseMessageReceived()
        {
            if (this.messageReceivedCallback != null)
            {
                this.messageReceivedCallback();
            }
        }

        internal virtual IList<TransportManager> SelectTransportManagers()
        {
            IList<TransportManager> list = null;
            ITransportManagerRegistration registration;
            if (!this.TryGetTransportManagerRegistration(out registration))
            {
                if (DiagnosticUtility.ShouldTraceVerbose)
                {
                    TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.NoExistingTransportManager, new UriTraceRecord(this.Uri), this, null);
                }
                if (this.HostedVirtualPath == null)
                {
                    registration = this.CreateTransportManagerRegistration();
                    this.TransportManagerTable.RegisterUri(registration.ListenUri, this.hostNameComparisonMode, registration);
                }
            }
            if (registration != null)
            {
                list = registration.Select(this);
                if (list == null)
                {
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.IncompatibleExistingTransportManager, new UriTraceRecord(this.Uri), this, null);
                    }
                    if (this.HostedVirtualPath == null)
                    {
                        System.Uri listenUri = AddSegment(registration.ListenUri, this.Uri);
                        if (listenUri != null)
                        {
                            registration = this.CreateTransportManagerRegistration(listenUri);
                            this.TransportManagerTable.RegisterUri(listenUri, this.hostNameComparisonMode, registration);
                            list = registration.Select(this);
                        }
                    }
                }
            }
            if (list == null)
            {
                this.ThrowTransportManagersNotFound();
            }
            return list;
        }

        internal void SetMessageReceivedCallback(MessageReceivedCallback messageReceivedCallback)
        {
            this.messageReceivedCallback = messageReceivedCallback;
        }

        protected void SetUri(System.Uri baseAddress, string relativeAddress)
        {
            System.Uri uri = baseAddress;
            if (relativeAddress != string.Empty)
            {
                if (!baseAddress.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
                {
                    UriBuilder uriBuilder = new UriBuilder(baseAddress);
                    TcpChannelListener.FixIpv6Hostname(uriBuilder, baseAddress);
                    uriBuilder.Path = uriBuilder.Path + "/";
                    baseAddress = uriBuilder.Uri;
                }
                uri = new System.Uri(baseAddress, relativeAddress);
                if (!baseAddress.IsBaseOf(uri))
                {
                    baseAddress = uri;
                }
            }
            this.baseUri = baseAddress;
            this.ValidateUri(uri);
            this.uri = uri;
        }

        private void ThrowTransportManagersNotFound()
        {
            if (this.HostedVirtualPath != null)
            {
                if ((string.Compare(this.Uri.Scheme, System.Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(this.Uri.Scheme, System.Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_NoHttpTransportManagerForUri", new object[] { this.Uri })));
                }
                if ((string.Compare(this.Uri.Scheme, System.Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(this.Uri.Scheme, System.Uri.UriSchemeNetPipe, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_NoTcpPipeTransportManagerForUri", new object[] { this.Uri })));
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("NoCompatibleTransportManagerForUri", new object[] { this.Uri })));
        }

        private bool TryGetTransportManagerRegistration(out ITransportManagerRegistration registration)
        {
            if (!this.InheritBaseAddressSettings)
            {
                return this.TryGetTransportManagerRegistration(this.hostNameComparisonMode, out registration);
            }
            if (this.TryGetTransportManagerRegistration(HostNameComparisonMode.StrongWildcard, out registration))
            {
                return true;
            }
            if (this.TryGetTransportManagerRegistration(HostNameComparisonMode.Exact, out registration))
            {
                return true;
            }
            if (this.TryGetTransportManagerRegistration(HostNameComparisonMode.WeakWildcard, out registration))
            {
                return true;
            }
            registration = null;
            return false;
        }

        protected virtual bool TryGetTransportManagerRegistration(HostNameComparisonMode hostNameComparisonMode, out ITransportManagerRegistration registration) => 
            this.TransportManagerTable.TryLookupUri(this.Uri, hostNameComparisonMode, out registration);

        protected virtual void ValidateUri(System.Uri uri)
        {
        }

        internal ServiceModelActivity Activity
        {
            get => 
                this.activity;
            set
            {
                this.activity = value;
            }
        }

        internal ServiceSecurityAuditBehavior AuditBehavior =>
            this.auditBehavior;

        internal System.Uri BaseUri =>
            this.baseUri;

        public System.ServiceModel.Channels.BufferManager BufferManager =>
            this.bufferManager;

        private string GeneratedAddressPrefix
        {
            get
            {
                switch (this.hostNameComparisonMode)
                {
                    case HostNameComparisonMode.StrongWildcard:
                        return GetGeneratedAddressPrefix(ref strongWildcardGeneratedAddressPrefix);

                    case HostNameComparisonMode.Exact:
                        return GetGeneratedAddressPrefix(ref exactGeneratedAddressPrefix);

                    case HostNameComparisonMode.WeakWildcard:
                        return GetGeneratedAddressPrefix(ref weakWildcardGeneratedAddressPrefix);
                }
                return null;
            }
        }

        internal string HostedVirtualPath =>
            this.hostedVirtualPath;

        internal HostNameComparisonMode HostNameComparisonModeInternal =>
            this.hostNameComparisonMode;

        internal bool InheritBaseAddressSettings
        {
            get => 
                this.inheritBaseAddressSettings;
            set
            {
                this.inheritBaseAddressSettings = value;
            }
        }

        public bool ManualAddressing =>
            this.manualAddressing;

        public long MaxBufferPoolSize =>
            this.maxBufferPoolSize;

        public virtual long MaxReceivedMessageSize =>
            this.maxReceivedMessageSize;

        public System.ServiceModel.Channels.MessageEncoderFactory MessageEncoderFactory =>
            this.messageEncoderFactory;

        public System.ServiceModel.Channels.MessageVersion MessageVersion =>
            this.messageVersion;

        public abstract string Scheme { get; }

        System.ServiceModel.Channels.BufferManager ITransportFactorySettings.BufferManager =>
            this.BufferManager;

        bool ITransportFactorySettings.ManualAddressing =>
            this.ManualAddressing;

        long ITransportFactorySettings.MaxReceivedMessageSize =>
            this.MaxReceivedMessageSize;

        System.ServiceModel.Channels.MessageEncoderFactory ITransportFactorySettings.MessageEncoderFactory =>
            this.MessageEncoderFactory;

        internal abstract UriPrefixTable<ITransportManagerRegistration> TransportManagerTable { get; }

        public override System.Uri Uri =>
            this.uri;
    }
}


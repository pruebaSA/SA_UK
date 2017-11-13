namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.Threading;
    using System.Web.Security;

    public sealed class DispatchRuntime
    {
        private bool automaticInputSessionShutdown;
        private System.ServiceModel.Dispatcher.ChannelDispatcher channelDispatcher;
        private System.ServiceModel.ConcurrencyMode concurrencyMode;
        private System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher;
        private ReadOnlyCollection<IAuthorizationPolicy> externalAuthorizationPolicies;
        private bool ignoreTransactionMessageProperty;
        private bool impersonateCallerForAllOperations;
        private SynchronizedCollection<IInputSessionShutdown> inputSessionShutdownHandlers;
        private SynchronizedCollection<IInstanceContextInitializer> instanceContextInitializers;
        private IInstanceContextProvider instanceContextProvider;
        private IInstanceProvider instanceProvider;
        private bool isAuthorizationManagerSet;
        private bool isExternalPoliciesSet;
        private AuditLevel messageAuthenticationAuditLevel;
        private SynchronizedCollection<IDispatchMessageInspector> messageInspectors;
        private OperationCollection operations;
        private IDispatchOperationSelector operationSelector;
        private System.ServiceModel.Description.PrincipalPermissionMode principalPermissionMode;
        private System.ServiceModel.Dispatcher.ClientRuntime proxyRuntime;
        private bool releaseServiceInstanceOnTransactionComplete;
        private object roleProvider;
        private ImmutableDispatchRuntime runtime;
        private AuditLogLocation securityAuditLogLocation;
        private AuditLevel serviceAuthorizationAuditLevel;
        private System.ServiceModel.ServiceAuthorizationManager serviceAuthorizationManager;
        private SharedRuntimeState shared;
        private InstanceContext singleton;
        private bool suppressAuditFailure;
        private System.Threading.SynchronizationContext synchronizationContext;
        private bool transactionAutoCompleteOnSessionClose;
        private System.Type type;
        private DispatchOperation unhandled;

        internal DispatchRuntime(System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher) : this(new SharedRuntimeState(true))
        {
            if (endpointDispatcher == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointDispatcher");
            }
            this.endpointDispatcher = endpointDispatcher;
        }

        private DispatchRuntime(SharedRuntimeState shared)
        {
            this.shared = shared;
            this.operations = new OperationCollection(this);
            this.inputSessionShutdownHandlers = this.NewBehaviorCollection<IInputSessionShutdown>();
            this.messageInspectors = this.NewBehaviorCollection<IDispatchMessageInspector>();
            this.instanceContextInitializers = this.NewBehaviorCollection<IInstanceContextInitializer>();
            this.synchronizationContext = ThreadBehavior.GetCurrentSynchronizationContext();
            this.automaticInputSessionShutdown = true;
            this.principalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.UseWindowsGroups;
            this.securityAuditLogLocation = AuditLogLocation.Default;
            this.suppressAuditFailure = true;
            this.serviceAuthorizationAuditLevel = AuditLevel.None;
            this.messageAuthenticationAuditLevel = AuditLevel.None;
            this.unhandled = new DispatchOperation(this, "*", "*", "*");
            this.unhandled.InternalFormatter = MessageOperationFormatter.Instance;
            this.unhandled.InternalInvoker = new UnhandledActionInvoker(this);
        }

        internal DispatchRuntime(System.ServiceModel.Dispatcher.ClientRuntime proxyRuntime, SharedRuntimeState shared) : this(shared)
        {
            if (proxyRuntime == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("proxyRuntime");
            }
            this.proxyRuntime = proxyRuntime;
            this.instanceProvider = new CallbackInstanceProvider();
            this.channelDispatcher = new System.ServiceModel.Dispatcher.ChannelDispatcher(shared);
            this.instanceContextProvider = InstanceContextProviderBase.GetProviderForMode(InstanceContextMode.PerSession, this);
        }

        internal DispatchOperationRuntime GetOperation(ref Message message) => 
            this.GetRuntime().GetOperation(ref message);

        internal ImmutableDispatchRuntime GetRuntime()
        {
            ImmutableDispatchRuntime runtime = this.runtime;
            if (runtime != null)
            {
                return runtime;
            }
            return this.GetRuntimeCore();
        }

        private ImmutableDispatchRuntime GetRuntimeCore()
        {
            lock (this.ThisLock)
            {
                if (this.runtime == null)
                {
                    this.runtime = new ImmutableDispatchRuntime(this);
                }
                return this.runtime;
            }
        }

        internal void InvalidateRuntime()
        {
            lock (this.ThisLock)
            {
                this.shared.ThrowIfImmutable();
                this.runtime = null;
            }
        }

        internal void LockDownProperties()
        {
            this.shared.LockDownProperties();
        }

        internal SynchronizedCollection<T> NewBehaviorCollection<T>() => 
            new DispatchBehaviorCollection<T>(this);

        public bool AutomaticInputSessionShutdown
        {
            get => 
                this.automaticInputSessionShutdown;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.automaticInputSessionShutdown = value;
                }
            }
        }

        public System.ServiceModel.Dispatcher.ClientRuntime CallbackClientRuntime
        {
            get
            {
                if (this.proxyRuntime == null)
                {
                    lock (this.ThisLock)
                    {
                        if (this.proxyRuntime == null)
                        {
                            this.proxyRuntime = new System.ServiceModel.Dispatcher.ClientRuntime(this, this.shared);
                        }
                    }
                }
                return this.proxyRuntime;
            }
        }

        public System.ServiceModel.Dispatcher.ChannelDispatcher ChannelDispatcher =>
            (this.channelDispatcher ?? this.endpointDispatcher.ChannelDispatcher);

        internal System.ServiceModel.Dispatcher.ClientRuntime ClientRuntime =>
            this.proxyRuntime;

        public System.ServiceModel.ConcurrencyMode ConcurrencyMode
        {
            get => 
                this.concurrencyMode;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.concurrencyMode = value;
                }
            }
        }

        internal bool EnableFaults
        {
            get
            {
                if (!this.IsOnServer)
                {
                    return this.shared.EnableFaults;
                }
                System.ServiceModel.Dispatcher.ChannelDispatcher channelDispatcher = this.ChannelDispatcher;
                return ((channelDispatcher != null) && channelDispatcher.EnableFaults);
            }
        }

        public System.ServiceModel.Dispatcher.EndpointDispatcher EndpointDispatcher =>
            this.endpointDispatcher;

        public ReadOnlyCollection<IAuthorizationPolicy> ExternalAuthorizationPolicies
        {
            get => 
                this.externalAuthorizationPolicies;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.externalAuthorizationPolicies = value;
                    this.isExternalPoliciesSet = true;
                }
            }
        }

        internal bool HasMatchAllOperation
        {
            get
            {
                lock (this.ThisLock)
                {
                    return !(this.unhandled.Invoker is UnhandledActionInvoker);
                }
            }
        }

        public bool IgnoreTransactionMessageProperty
        {
            get => 
                this.ignoreTransactionMessageProperty;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.ignoreTransactionMessageProperty = value;
                }
            }
        }

        public bool ImpersonateCallerForAllOperations
        {
            get => 
                this.impersonateCallerForAllOperations;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.impersonateCallerForAllOperations = value;
                }
            }
        }

        public SynchronizedCollection<IInputSessionShutdown> InputSessionShutdownHandlers =>
            this.inputSessionShutdownHandlers;

        public SynchronizedCollection<IInstanceContextInitializer> InstanceContextInitializers =>
            this.instanceContextInitializers;

        public IInstanceContextProvider InstanceContextProvider
        {
            get => 
                this.instanceContextProvider;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.instanceContextProvider = value;
                }
            }
        }

        public IInstanceProvider InstanceProvider
        {
            get => 
                this.instanceProvider;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.instanceProvider = value;
                }
            }
        }

        internal bool IsOnServer =>
            this.shared.IsOnServer;

        internal bool IsRoleProviderSet =>
            (this.roleProvider != null);

        internal bool ManualAddressing
        {
            get
            {
                if (!this.IsOnServer)
                {
                    return this.shared.ManualAddressing;
                }
                System.ServiceModel.Dispatcher.ChannelDispatcher channelDispatcher = this.ChannelDispatcher;
                return ((channelDispatcher != null) && channelDispatcher.ManualAddressing);
            }
        }

        internal int MaxCallContextInitializers
        {
            get
            {
                lock (this.ThisLock)
                {
                    int num = 0;
                    for (int i = 0; i < this.operations.Count; i++)
                    {
                        num = Math.Max(num, this.operations[i].CallContextInitializers.Count);
                    }
                    return num;
                }
            }
        }

        internal int MaxParameterInspectors
        {
            get
            {
                lock (this.ThisLock)
                {
                    int num = 0;
                    for (int i = 0; i < this.operations.Count; i++)
                    {
                        num = Math.Max(num, this.operations[i].ParameterInspectors.Count);
                    }
                    return num;
                }
            }
        }

        public AuditLevel MessageAuthenticationAuditLevel
        {
            get => 
                this.messageAuthenticationAuditLevel;
            set
            {
                if (!AuditLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.messageAuthenticationAuditLevel = value;
                }
            }
        }

        public SynchronizedCollection<IDispatchMessageInspector> MessageInspectors =>
            this.messageInspectors;

        public SynchronizedKeyedCollection<string, DispatchOperation> Operations =>
            this.operations;

        public IDispatchOperationSelector OperationSelector
        {
            get => 
                this.operationSelector;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.operationSelector = value;
                }
            }
        }

        public System.ServiceModel.Description.PrincipalPermissionMode PrincipalPermissionMode
        {
            get => 
                this.principalPermissionMode;
            set
            {
                if (!PrincipalPermissionModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.principalPermissionMode = value;
                }
            }
        }

        public bool ReleaseServiceInstanceOnTransactionComplete
        {
            get => 
                this.releaseServiceInstanceOnTransactionComplete;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.releaseServiceInstanceOnTransactionComplete = value;
                }
            }
        }

        internal bool RequiresAuthorization
        {
            get
            {
                if (!this.isAuthorizationManagerSet && !this.isExternalPoliciesSet)
                {
                    return (AuditLevel.Success == (this.serviceAuthorizationAuditLevel & AuditLevel.Success));
                }
                return true;
            }
        }

        public System.Web.Security.RoleProvider RoleProvider
        {
            get => 
                ((System.Web.Security.RoleProvider) this.roleProvider);
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.roleProvider = value;
                }
            }
        }

        public AuditLogLocation SecurityAuditLogLocation
        {
            get => 
                this.securityAuditLogLocation;
            set
            {
                if (!AuditLogLocationHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.securityAuditLogLocation = value;
                }
            }
        }

        public AuditLevel ServiceAuthorizationAuditLevel
        {
            get => 
                this.serviceAuthorizationAuditLevel;
            set
            {
                if (!AuditLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.serviceAuthorizationAuditLevel = value;
                }
            }
        }

        public System.ServiceModel.ServiceAuthorizationManager ServiceAuthorizationManager
        {
            get => 
                this.serviceAuthorizationManager;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.serviceAuthorizationManager = value;
                    this.isAuthorizationManagerSet = true;
                }
            }
        }

        public InstanceContext SingletonInstanceContext
        {
            get => 
                this.singleton;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.singleton = value;
                }
            }
        }

        public bool SuppressAuditFailure
        {
            get => 
                this.suppressAuditFailure;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.suppressAuditFailure = value;
                }
            }
        }

        public System.Threading.SynchronizationContext SynchronizationContext
        {
            get => 
                this.synchronizationContext;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.synchronizationContext = value;
                }
            }
        }

        internal object ThisLock =>
            this.shared;

        public bool TransactionAutoCompleteOnSessionClose
        {
            get => 
                this.transactionAutoCompleteOnSessionClose;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.transactionAutoCompleteOnSessionClose = value;
                }
            }
        }

        public System.Type Type
        {
            get => 
                this.type;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.type = value;
                }
            }
        }

        public DispatchOperation UnhandledDispatchOperation
        {
            get => 
                this.unhandled;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.unhandled = value;
                }
            }
        }

        public bool ValidateMustUnderstand
        {
            get => 
                this.shared.ValidateMustUnderstand;
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    this.shared.ValidateMustUnderstand = value;
                }
            }
        }

        private class CallbackInstanceProvider : IInstanceProvider
        {
            object IInstanceProvider.GetInstance(InstanceContext instanceContext)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxCannotActivateCallbackInstace")));
            }

            object IInstanceProvider.GetInstance(InstanceContext instanceContext, Message message)
            {
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxCannotActivateCallbackInstace")), message);
            }

            void IInstanceProvider.ReleaseInstance(InstanceContext instanceContext, object instance)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxCannotActivateCallbackInstace")));
            }
        }

        private class DispatchBehaviorCollection<T> : SynchronizedCollection<T>
        {
            private DispatchRuntime outer;

            internal DispatchBehaviorCollection(DispatchRuntime outer) : base(outer.ThisLock)
            {
                this.outer = outer;
            }

            protected override void ClearItems()
            {
                this.outer.InvalidateRuntime();
                base.ClearItems();
            }

            protected override void InsertItem(int index, T item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }
                this.outer.InvalidateRuntime();
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(int index)
            {
                this.outer.InvalidateRuntime();
                base.RemoveItem(index);
            }

            protected override void SetItem(int index, T item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }
                this.outer.InvalidateRuntime();
                base.SetItem(index, item);
            }
        }

        private class OperationCollection : SynchronizedKeyedCollection<string, DispatchOperation>
        {
            private DispatchRuntime outer;

            internal OperationCollection(DispatchRuntime outer) : base(outer.ThisLock)
            {
                this.outer = outer;
            }

            protected override void ClearItems()
            {
                this.outer.InvalidateRuntime();
                base.ClearItems();
            }

            protected override string GetKeyForItem(DispatchOperation item) => 
                item.Name;

            protected override void InsertItem(int index, DispatchOperation item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }
                if (item.Parent != this.outer)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxMismatchedOperationParent"));
                }
                this.outer.InvalidateRuntime();
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(int index)
            {
                this.outer.InvalidateRuntime();
                base.RemoveItem(index);
            }

            protected override void SetItem(int index, DispatchOperation item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }
                if (item.Parent != this.outer)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("SFxMismatchedOperationParent"));
                }
                this.outer.InvalidateRuntime();
                base.SetItem(index, item);
            }
        }

        internal class UnhandledActionInvoker : IOperationInvoker
        {
            private DispatchRuntime dispatchRuntime;

            public UnhandledActionInvoker(DispatchRuntime dispatchRuntime)
            {
                this.dispatchRuntime = dispatchRuntime;
            }

            public object[] AllocateInputs() => 
                new object[1];

            public object Invoke(object instance, object[] inputs, out object[] outputs)
            {
                FaultException exception;
                ServiceChannel serviceChannel;
                outputs = EmptyArray<object>.Allocate(0);
                Message message = inputs[0] as Message;
                if (message != null)
                {
                    string action = message.Headers.Action;
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.UnhandledAction, new StringTraceRecord("Action", action), this, null, message);
                    }
                    FaultCode code = FaultCode.CreateSenderFaultCode("ActionNotSupported", message.Version.Addressing.Namespace);
                    FaultReason reason = new FaultReason(System.ServiceModel.SR.GetString("SFxNoEndpointMatchingContract", new object[] { action }));
                    exception = new FaultException(reason, code);
                    System.ServiceModel.Dispatcher.ErrorBehavior.ThrowAndCatch(exception);
                    serviceChannel = OperationContext.Current.InternalServiceChannel;
                    OperationContext.Current.OperationCompleted += delegate (object sender, EventArgs e) {
                        ChannelDispatcher channelDispatcher = this.dispatchRuntime.ChannelDispatcher;
                        if (!channelDispatcher.HandleError(exception) && serviceChannel.HasSession)
                        {
                            try
                            {
                                serviceChannel.Close(ChannelHandler.CloseAfterFaultTimeout);
                            }
                            catch (Exception exception1)
                            {
                                if (DiagnosticUtility.IsFatal(exception1))
                                {
                                    throw;
                                }
                                channelDispatcher.HandleError(exception1);
                            }
                        }
                    };
                    if (this.dispatchRuntime.shared.EnableFaults)
                    {
                        MessageFault fault = MessageFault.CreateFault(code, reason, action);
                        return Message.CreateMessage(message.Version, fault, message.Version.Addressing.DefaultFaultAction);
                    }
                    OperationContext.Current.RequestContext.Close();
                    OperationContext.Current.RequestContext = null;
                }
                return null;
            }

            public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
            }

            public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
            }

            public bool IsSynchronous =>
                true;
        }
    }
}


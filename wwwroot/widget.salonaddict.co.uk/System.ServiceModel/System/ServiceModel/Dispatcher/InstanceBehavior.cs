namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    internal class InstanceBehavior
    {
        private const BindingFlags DefaultBindingFlags = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        private ServiceHostBase host;
        private ImmutableDispatchRuntime immutableRuntime;
        private IInstanceContextInitializer[] initializers;
        private IInstanceContextProvider instanceContextProvider;
        private bool isSynchronized;
        private IInstanceProvider provider;
        private bool releaseServiceInstanceOnTransactionComplete = true;
        private InstanceContext singleton;
        private bool transactionAutoCompleteOnSessionClose;
        private bool useSession;

        internal InstanceBehavior(DispatchRuntime dispatch, ImmutableDispatchRuntime immutableRuntime)
        {
            this.useSession = dispatch.ChannelDispatcher.Session;
            this.immutableRuntime = immutableRuntime;
            this.host = dispatch.ChannelDispatcher?.Host;
            this.initializers = EmptyArray<IInstanceContextInitializer>.ToArray(dispatch.InstanceContextInitializers);
            this.provider = dispatch.InstanceProvider;
            this.singleton = dispatch.SingletonInstanceContext;
            this.transactionAutoCompleteOnSessionClose = dispatch.TransactionAutoCompleteOnSessionClose;
            this.releaseServiceInstanceOnTransactionComplete = dispatch.ReleaseServiceInstanceOnTransactionComplete;
            this.isSynchronized = dispatch.ConcurrencyMode != ConcurrencyMode.Multiple;
            this.instanceContextProvider = dispatch.InstanceContextProvider;
            if (this.provider == null)
            {
                ConstructorInfo constructor = null;
                if (dispatch.Type != null)
                {
                    constructor = GetConstructor(dispatch.Type);
                }
                if (this.singleton == null)
                {
                    if ((dispatch.Type != null) && (dispatch.Type.IsAbstract || dispatch.Type.IsInterface))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxServiceTypeNotCreatable")));
                    }
                    if (constructor == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxNoDefaultConstructor")));
                    }
                }
                if ((constructor != null) && ((this.singleton == null) || !this.singleton.IsWellKnown))
                {
                    CreateInstanceDelegate creator = new InvokerUtil().GenerateCreateInstanceDelegate(dispatch.Type, constructor);
                    this.provider = new InstanceProvider(creator);
                }
            }
            if (this.singleton != null)
            {
                this.singleton.Behavior = this;
            }
        }

        internal void AfterReply(ref MessageRpc rpc, ErrorBehavior error)
        {
            InstanceContext instanceContext = rpc.InstanceContext;
            if (instanceContext != null)
            {
                try
                {
                    if (rpc.Operation.ReleaseInstanceAfterCall)
                    {
                        if (instanceContext.State == CommunicationState.Opened)
                        {
                            instanceContext.ReleaseServiceInstance();
                        }
                    }
                    else if (((this.releaseServiceInstanceOnTransactionComplete && this.isSynchronized) && (rpc.transaction != null)) && (rpc.transaction.IsCompleted || (rpc.Error != null)))
                    {
                        if (instanceContext.State == CommunicationState.Opened)
                        {
                            instanceContext.ReleaseServiceInstance();
                        }
                        if (DiagnosticUtility.ShouldTraceInformation)
                        {
                            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.TxReleaseServiceInstanceOnCompletion, System.ServiceModel.SR.GetString("TraceCodeTxReleaseServiceInstanceOnCompletion", new object[] { "*" }));
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    error.HandleError(exception);
                }
                try
                {
                    instanceContext.UnbindRpc(ref rpc);
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    error.HandleError(exception2);
                }
            }
        }

        internal bool CanUnload(InstanceContext instanceContext)
        {
            if (InstanceContextProviderBase.IsProviderSingleton(this.instanceContextProvider))
            {
                return false;
            }
            if ((!InstanceContextProviderBase.IsProviderPerCall(this.instanceContextProvider) && !InstanceContextProviderBase.IsProviderSessionful(this.instanceContextProvider)) && !this.instanceContextProvider.IsIdle(instanceContext))
            {
                this.instanceContextProvider.NotifyIdle(InstanceContext.NotifyIdleCallback, instanceContext);
                return false;
            }
            return true;
        }

        internal void EnsureInstanceContext(ref MessageRpc rpc)
        {
            if (rpc.InstanceContext == null)
            {
                rpc.InstanceContext = new InstanceContext(rpc.Host, false);
                rpc.InstanceContext.ServiceThrottle = rpc.channelHandler.InstanceContextServiceThrottle;
                rpc.MessageRpcOwnsInstanceContextThrottle = false;
            }
            rpc.OperationContext.SetInstanceContext(rpc.InstanceContext);
            rpc.InstanceContext.Behavior = this;
            if (rpc.InstanceContext.State == CommunicationState.Created)
            {
                lock (rpc.InstanceContext.ThisLock)
                {
                    if (rpc.InstanceContext.State == CommunicationState.Created)
                    {
                        rpc.InstanceContext.Open(rpc.Channel.CloseTimeout);
                    }
                }
            }
            rpc.InstanceContext.BindRpc(ref rpc);
        }

        private static ConstructorInfo GetConstructor(Type type) => 
            type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

        internal object GetInstance(InstanceContext instanceContext) => 
            this.provider?.GetInstance(instanceContext);

        internal object GetInstance(InstanceContext instanceContext, Message request) => 
            this.provider?.GetInstance(instanceContext, request);

        internal void Initialize(InstanceContext instanceContext)
        {
            OperationContext current = OperationContext.Current;
            Message incomingMessage = current?.IncomingMessage;
            if ((current != null) && (current.InternalServiceChannel != null))
            {
                IContextChannel proxy = (IContextChannel) current.InternalServiceChannel.Proxy;
                this.instanceContextProvider.InitializeInstanceContext(instanceContext, incomingMessage, proxy);
            }
            for (int i = 0; i < this.initializers.Length; i++)
            {
                this.initializers[i].Initialize(instanceContext, incomingMessage);
            }
        }

        internal void ReleaseBefore(ref MessageRpc rpc)
        {
            if (rpc.Operation.ReleaseInstanceBeforeCall)
            {
                rpc.InstanceContext.ReleaseServiceInstance();
            }
        }

        internal void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (this.provider != null)
            {
                try
                {
                    this.provider.ReleaseInstance(instanceContext, instance);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    this.immutableRuntime.ErrorBehavior.HandleError(exception);
                }
            }
        }

        internal IInstanceContextProvider InstanceContextProvider =>
            this.instanceContextProvider;

        internal bool ReleaseServiceInstanceOnTransactionComplete =>
            this.releaseServiceInstanceOnTransactionComplete;

        internal bool TransactionAutoCompleteOnSessionClose =>
            this.transactionAutoCompleteOnSessionClose;

        private class InstanceProvider : IInstanceProvider
        {
            private CreateInstanceDelegate creator;

            internal InstanceProvider(CreateInstanceDelegate creator)
            {
                if (creator == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("creator");
                }
                this.creator = creator;
            }

            public object GetInstance(InstanceContext instanceContext) => 
                this.creator();

            public object GetInstance(InstanceContext instanceContext, Message message) => 
                this.creator();

            public void ReleaseInstance(InstanceContext instanceContext, object instance)
            {
                IDisposable disposable = instance as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}


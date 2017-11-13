namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    public sealed class DispatchOperation
    {
        private string action;
        private bool autoDisposeParameters;
        private SynchronizedCollection<ICallContextInitializer> callContextInitializers;
        private bool deserializeRequest;
        private SynchronizedCollection<FaultContractInfo> faultContractInfos;
        private IDispatchFaultFormatter faultFormatter;
        private IDispatchMessageFormatter formatter;
        private bool hasNoDisposableParameters;
        private ImpersonationOption impersonation;
        private IOperationInvoker invoker;
        private bool isFaultFormatterSetExplicit;
        private bool isOneWay;
        private bool isTerminating;
        private string name;
        private SynchronizedCollection<IParameterInspector> parameterInspectors;
        private DispatchRuntime parent;
        private bool releaseInstanceAfterCall;
        private bool releaseInstanceBeforeCall;
        private string replyAction;
        private bool serializeReply;
        private bool transactionAutoComplete;
        private bool transactionRequired;

        public DispatchOperation(DispatchRuntime parent, string name, string action)
        {
            this.deserializeRequest = true;
            this.serializeReply = true;
            this.autoDisposeParameters = true;
            if (parent == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parent");
            }
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
            }
            this.parent = parent;
            this.name = name;
            this.action = action;
            this.impersonation = ImpersonationOption.NotAllowed;
            this.callContextInitializers = parent.NewBehaviorCollection<ICallContextInitializer>();
            this.faultContractInfos = parent.NewBehaviorCollection<FaultContractInfo>();
            this.parameterInspectors = parent.NewBehaviorCollection<IParameterInspector>();
            this.isOneWay = true;
        }

        public DispatchOperation(DispatchRuntime parent, string name, string action, string replyAction) : this(parent, name, action)
        {
            this.replyAction = replyAction;
            this.isOneWay = false;
        }

        public string Action =>
            this.action;

        public bool AutoDisposeParameters
        {
            get => 
                this.autoDisposeParameters;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.autoDisposeParameters = value;
                }
            }
        }

        public SynchronizedCollection<ICallContextInitializer> CallContextInitializers =>
            this.callContextInitializers;

        public bool DeserializeRequest
        {
            get => 
                this.deserializeRequest;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.deserializeRequest = value;
                }
            }
        }

        public SynchronizedCollection<FaultContractInfo> FaultContractInfos =>
            this.faultContractInfos;

        internal IDispatchFaultFormatter FaultFormatter
        {
            get
            {
                if (this.faultFormatter == null)
                {
                    this.faultFormatter = new DataContractSerializerFaultFormatter(this.faultContractInfos);
                }
                return this.faultFormatter;
            }
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.faultFormatter = value;
                    this.isFaultFormatterSetExplicit = true;
                }
            }
        }

        public IDispatchMessageFormatter Formatter
        {
            get => 
                this.formatter;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.formatter = value;
                }
            }
        }

        internal bool HasNoDisposableParameters
        {
            get => 
                this.hasNoDisposableParameters;
            set
            {
                this.hasNoDisposableParameters = value;
            }
        }

        public ImpersonationOption Impersonation
        {
            get => 
                this.impersonation;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.impersonation = value;
                }
            }
        }

        internal IDispatchMessageFormatter InternalFormatter
        {
            get => 
                this.formatter;
            set
            {
                this.formatter = value;
            }
        }

        internal IOperationInvoker InternalInvoker
        {
            get => 
                this.invoker;
            set
            {
                this.invoker = value;
            }
        }

        public IOperationInvoker Invoker
        {
            get => 
                this.invoker;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.invoker = value;
                }
            }
        }

        internal bool IsFaultFormatterSetExplicit =>
            this.isFaultFormatterSetExplicit;

        public bool IsOneWay =>
            this.isOneWay;

        public bool IsTerminating
        {
            get => 
                this.isTerminating;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.isTerminating = value;
                }
            }
        }

        public string Name =>
            this.name;

        public SynchronizedCollection<IParameterInspector> ParameterInspectors =>
            this.parameterInspectors;

        public DispatchRuntime Parent =>
            this.parent;

        public bool ReleaseInstanceAfterCall
        {
            get => 
                this.releaseInstanceAfterCall;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.releaseInstanceAfterCall = value;
                }
            }
        }

        public bool ReleaseInstanceBeforeCall
        {
            get => 
                this.releaseInstanceBeforeCall;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.releaseInstanceBeforeCall = value;
                }
            }
        }

        public string ReplyAction =>
            this.replyAction;

        public bool SerializeReply
        {
            get => 
                this.serializeReply;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.serializeReply = value;
                }
            }
        }

        public bool TransactionAutoComplete
        {
            get => 
                this.transactionAutoComplete;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.transactionAutoComplete = value;
                }
            }
        }

        public bool TransactionRequired
        {
            get => 
                this.transactionRequired;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.transactionRequired = value;
                }
            }
        }
    }
}


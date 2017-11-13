namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceModel;

    public sealed class ClientOperation
    {
        private string action;
        private MethodInfo beginMethod;
        private bool deserializeReply;
        private MethodInfo endMethod;
        private SynchronizedCollection<FaultContractInfo> faultContractInfos;
        private IClientFaultFormatter faultFormatter;
        private IClientMessageFormatter formatter;
        private bool isFaultFormatterSetExplicit;
        private bool isInitiating;
        private bool isOneWay;
        private bool isTerminating;
        private string name;
        private SynchronizedCollection<IParameterInspector> parameterInspectors;
        private ClientRuntime parent;
        private string replyAction;
        private bool serializeRequest;
        private MethodInfo syncMethod;

        public ClientOperation(ClientRuntime parent, string name, string action) : this(parent, name, action, null)
        {
        }

        public ClientOperation(ClientRuntime parent, string name, string action, string replyAction)
        {
            this.isInitiating = true;
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
            this.replyAction = replyAction;
            this.faultContractInfos = parent.NewBehaviorCollection<FaultContractInfo>();
            this.parameterInspectors = parent.NewBehaviorCollection<IParameterInspector>();
        }

        public string Action =>
            this.action;

        public MethodInfo BeginMethod
        {
            get => 
                this.beginMethod;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.beginMethod = value;
                }
            }
        }

        public bool DeserializeReply
        {
            get => 
                this.deserializeReply;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.deserializeReply = value;
                }
            }
        }

        public MethodInfo EndMethod
        {
            get => 
                this.endMethod;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.endMethod = value;
                }
            }
        }

        public SynchronizedCollection<FaultContractInfo> FaultContractInfos =>
            this.faultContractInfos;

        internal IClientFaultFormatter FaultFormatter
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

        public IClientMessageFormatter Formatter
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

        internal IClientMessageFormatter InternalFormatter
        {
            get => 
                this.formatter;
            set
            {
                this.formatter = value;
            }
        }

        internal bool IsFaultFormatterSetExplicit =>
            this.isFaultFormatterSetExplicit;

        public bool IsInitiating
        {
            get => 
                this.isInitiating;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.isInitiating = value;
                }
            }
        }

        public bool IsOneWay
        {
            get => 
                this.isOneWay;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.isOneWay = value;
                }
            }
        }

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

        public ClientRuntime Parent =>
            this.parent;

        public string ReplyAction =>
            this.replyAction;

        public bool SerializeRequest
        {
            get => 
                this.serializeRequest;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.serializeRequest = value;
                }
            }
        }

        public MethodInfo SyncMethod
        {
            get => 
                this.syncMethod;
            set
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.InvalidateRuntime();
                    this.syncMethod = value;
                }
            }
        }
    }
}


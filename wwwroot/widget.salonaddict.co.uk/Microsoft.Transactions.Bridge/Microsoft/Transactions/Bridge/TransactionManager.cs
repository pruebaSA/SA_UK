namespace Microsoft.Transactions.Bridge
{
    using Microsoft.Transactions;
    using System;
    using System.Configuration;
    using System.ServiceModel.Internal;

    internal abstract class TransactionManager
    {
        private object bridgeConfig;
        private Guid id;
        private Microsoft.Transactions.Bridge.IProtocolProvider protocolProvider;
        private Microsoft.Transactions.Bridge.IProtocolProviderCoordinatorService protocolProviderCoordinatorService;
        private Microsoft.Transactions.Bridge.IProtocolProviderPropagationService protocolProviderPropagationService;
        private Microsoft.Transactions.Bridge.TransactionManagerCoordinatorService transactionManagerCoordinatorService;
        private Microsoft.Transactions.Bridge.TransactionManagerPropagationService transactionManagerPropagationService;
        private Microsoft.Transactions.Bridge.TransactionManagerSettings transactionManagerSettings;

        protected TransactionManager()
        {
            PropagationProtocolsTracing.TraceVerbose("TransactionManager::TransactionManager");
            this.id = Guid.NewGuid();
            PropagationProtocolsTracing.TraceVerbose(this.id.ToString("B", null));
        }

        public abstract void Initialize();
        public void Initialize(string fullyQualifiedTypeName, object bridgeConfig)
        {
            PropagationProtocolsTracing.TraceVerbose("TransactionManager::Initialize");
            PropagationProtocolsTracing.TraceVerbose(fullyQualifiedTypeName);
            if (!TransactionBridge.IsAssemblyMicrosoftSigned(fullyQualifiedTypeName))
            {
                PropagationProtocolsTracing.TraceVerbose("Protocol type has wrong signature: " + fullyQualifiedTypeName);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(Microsoft.Transactions.SR.GetString("ProtocolTypeWrongSignature")));
            }
            this.bridgeConfig = bridgeConfig;
            Type type = Type.GetType(fullyQualifiedTypeName, true);
            PropagationProtocolsTracing.TraceVerbose(type.ToString());
            this.protocolProvider = (Microsoft.Transactions.Bridge.IProtocolProvider) Activator.CreateInstance(type);
            this.Initialize();
            this.protocolProviderCoordinatorService = this.protocolProvider.CoordinatorService;
            this.protocolProviderPropagationService = this.protocolProvider.PropagationService;
        }

        public abstract void Recover();
        public abstract void Start();
        public abstract void Stop();
        public override string ToString() => 
            (base.GetType().ToString() + " " + this.id.ToString("B", null));

        protected static void UnhandledExceptionHandler(Exception exception)
        {
            DiagnosticUtility.InvokeFinalHandler(exception);
        }

        protected object BridgeConfiguration =>
            this.bridgeConfig;

        public Microsoft.Transactions.Bridge.TransactionManagerCoordinatorService CoordinatorService =>
            this.transactionManagerCoordinatorService;

        public Microsoft.Transactions.Bridge.IProtocolProvider IProtocolProvider =>
            this.protocolProvider;

        public Microsoft.Transactions.Bridge.IProtocolProviderCoordinatorService IProtocolProviderCoordinatorService =>
            this.protocolProviderCoordinatorService;

        public Microsoft.Transactions.Bridge.IProtocolProviderPropagationService IProtocolProviderPropagationService =>
            this.protocolProviderPropagationService;

        public abstract int MaxLogEntrySize { get; }

        public Microsoft.Transactions.Bridge.TransactionManagerPropagationService PropagationService =>
            this.transactionManagerPropagationService;

        public Microsoft.Transactions.Bridge.TransactionManagerSettings Settings =>
            this.TransactionManagerSettings;

        protected Microsoft.Transactions.Bridge.TransactionManagerCoordinatorService TransactionManagerCoordinatorService
        {
            get => 
                this.transactionManagerCoordinatorService;
            set
            {
                this.transactionManagerCoordinatorService = value;
            }
        }

        protected Microsoft.Transactions.Bridge.TransactionManagerPropagationService TransactionManagerPropagationService
        {
            get => 
                this.transactionManagerPropagationService;
            set
            {
                this.transactionManagerPropagationService = value;
            }
        }

        protected Microsoft.Transactions.Bridge.TransactionManagerSettings TransactionManagerSettings
        {
            get => 
                this.transactionManagerSettings;
            set
            {
                this.transactionManagerSettings = value;
            }
        }
    }
}


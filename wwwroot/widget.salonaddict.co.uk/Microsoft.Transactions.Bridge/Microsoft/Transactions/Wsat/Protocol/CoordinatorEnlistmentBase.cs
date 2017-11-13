namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.ServiceModel;

    internal abstract class CoordinatorEnlistmentBase : TransactionEnlistment
    {
        protected TwoPhaseCommitCoordinatorProxy coordinatorProxy;
        protected EndpointAddress participantService;

        protected CoordinatorEnlistmentBase(ProtocolState state) : base(state)
        {
        }

        protected CoordinatorEnlistmentBase(ProtocolState state, Guid enlistmentId) : base(state, enlistmentId)
        {
        }

        public void CreateParticipantService()
        {
            EnlistmentHeader refParam = new EnlistmentHeader(base.enlistmentId);
            this.participantService = base.state.TwoPhaseCommitParticipantListener.CreateEndpointReference(refParam);
            if (this.coordinatorProxy != null)
            {
                this.coordinatorProxy.From = this.participantService;
            }
        }

        public override void OnStateMachineComplete()
        {
            base.OnStateMachineComplete();
            if (this.coordinatorProxy != null)
            {
                this.coordinatorProxy.Release();
            }
        }

        public void SetCoordinatorProxy(TwoPhaseCommitCoordinatorProxy proxy)
        {
            if (this.participantService == null)
            {
                Microsoft.Transactions.Bridge.DiagnosticUtility.FailFast("participantService needed for coordinatorProxy");
            }
            proxy.AddRef();
            this.coordinatorProxy = proxy;
            this.coordinatorProxy.From = this.participantService;
        }

        public TwoPhaseCommitCoordinatorProxy CoordinatorProxy =>
            this.coordinatorProxy;

        public EndpointAddress ParticipantService =>
            this.participantService;
    }
}


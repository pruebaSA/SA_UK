namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class MsgRegisterVolatileResponseEvent : MsgRegisterResponseEvent
    {
        private VolatileCoordinatorEnlistment volatileCoordinator;

        public MsgRegisterVolatileResponseEvent(VolatileCoordinatorEnlistment volatileCoordinator, RegisterResponse response, TwoPhaseCommitCoordinatorProxy proxy) : base(volatileCoordinator, response, proxy)
        {
            this.volatileCoordinator = volatileCoordinator;
        }

        public override void Execute(StateMachine stateMachine)
        {
            try
            {
                if (DebugTrace.Info)
                {
                    base.state.DebugTraceSink.OnEvent(this);
                }
                stateMachine.State.OnEvent(this);
            }
            finally
            {
                base.Proxy.Release();
            }
        }

        public VolatileCoordinatorEnlistment VolatileCoordinator =>
            this.volatileCoordinator;
    }
}


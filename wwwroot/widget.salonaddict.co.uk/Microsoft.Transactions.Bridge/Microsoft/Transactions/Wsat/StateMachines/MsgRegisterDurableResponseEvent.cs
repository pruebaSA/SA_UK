namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class MsgRegisterDurableResponseEvent : MsgRegisterResponseEvent
    {
        private CoordinatorEnlistment coordinator;

        public MsgRegisterDurableResponseEvent(CoordinatorEnlistment coordinator, RegisterResponse response, TwoPhaseCommitCoordinatorProxy proxy) : base(coordinator, response, proxy)
        {
            this.coordinator = coordinator;
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

        public CoordinatorEnlistment Coordinator =>
            this.coordinator;
    }
}


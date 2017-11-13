namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class TerminalState : State
    {
        protected TerminalState(ProtocolState state) : base(state)
        {
        }

        public override void Enter(StateMachine stateMachine)
        {
            base.Enter(stateMachine);
            stateMachine.Cleanup();
        }
    }
}


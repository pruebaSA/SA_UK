namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TransactionContextInitializingCoordinator : InactiveState
    {
        public TransactionContextInitializingCoordinator(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(TransactionContextCreatedEvent e)
        {
            e.ContextManager.TransactionContext = e.TransactionContext;
            e.StateMachine.ChangeState(base.state.States.TransactionContextActive);
        }

        public override void OnEvent(TransactionContextEnlistTransactionEvent e)
        {
            e.ContextManager.Requests.Enqueue(e);
        }

        public override void OnEvent(TransactionContextTransactionDoneEvent e)
        {
            e.StateMachine.ChangeState(base.state.States.TransactionContextFinished);
        }
    }
}


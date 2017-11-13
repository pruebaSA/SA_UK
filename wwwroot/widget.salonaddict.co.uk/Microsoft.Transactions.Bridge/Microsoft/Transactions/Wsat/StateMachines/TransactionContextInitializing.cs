namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TransactionContextInitializing : InactiveState
    {
        public TransactionContextInitializing(ProtocolState state) : base(state)
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
            e.StateMachine.ChangeState(base.state.States.TransactionContextInitializingCoordinator);
            CoordinatorEnlistment coordinator = new CoordinatorEnlistment(base.state, e.ContextManager, e.Body.CurrentContext, e.Body.IssuedToken);
            CreateCoordinationContext body = e.Body;
            MsgEnlistTransactionEvent event2 = new MsgEnlistTransactionEvent(coordinator, ref body, e.Result);
            coordinator.StateMachine.Enqueue(event2);
        }
    }
}


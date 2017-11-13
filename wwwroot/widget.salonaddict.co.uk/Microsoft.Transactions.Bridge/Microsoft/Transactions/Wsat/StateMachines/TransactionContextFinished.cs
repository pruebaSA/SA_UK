﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TransactionContextFinished : TerminalState
    {
        public TransactionContextFinished(ProtocolState state) : base(state)
        {
        }

        public override void Enter(StateMachine stateMachine)
        {
            base.Enter(stateMachine);
            TransactionContextStateMachine machine = (TransactionContextStateMachine) stateMachine;
            TransactionContextManager contextManager = machine.ContextManager;
            Fault fault = contextManager.Fault;
            foreach (TransactionContextEnlistTransactionEvent event2 in contextManager.Requests)
            {
                base.state.ActivationCoordinator.SendFault(event2.Result, fault);
            }
            contextManager.Requests.Clear();
        }

        public override void OnEvent(TransactionContextEnlistTransactionEvent e)
        {
            base.state.ActivationCoordinator.SendFault(e.Result, e.ContextManager.Fault);
        }
    }
}


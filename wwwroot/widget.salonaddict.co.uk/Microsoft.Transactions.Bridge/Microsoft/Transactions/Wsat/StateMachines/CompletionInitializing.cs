﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CompletionInitializing : InactiveState
    {
        public CompletionInitializing(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(MsgCreateTransactionEvent e)
        {
            CompletionEnlistment completion = e.Completion;
            EnlistmentOptions options = completion.CreateEnlistmentOptions(e.Body.Expires, e.Body.ExpiresPresent, e.Body.IsolationLevel, 0, null);
            base.state.TransactionManagerSend.CreateTransaction(completion, options, e);
            e.StateMachine.ChangeState(base.state.States.CompletionCreating);
        }
    }
}


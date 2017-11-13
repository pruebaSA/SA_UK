﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TmRollbackResponseEvent : SynchronizationEvent
    {
        private Microsoft.Transactions.Bridge.Status status;

        public TmRollbackResponseEvent(TransactionEnlistment enlistment, Microsoft.Transactions.Bridge.Status status) : base(enlistment)
        {
            this.status = status;
        }

        public override void Execute(StateMachine stateMachine)
        {
            if (DebugTrace.Info)
            {
                base.state.DebugTraceSink.OnEvent(this);
            }
            stateMachine.State.OnEvent(this);
        }

        public Microsoft.Transactions.Bridge.Status Status =>
            this.status;
    }
}


namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class CompletionStatusEvent : CompletionEvent
    {
        protected Microsoft.Transactions.Bridge.Status status;

        protected CompletionStatusEvent(CompletionEnlistment completion, Microsoft.Transactions.Bridge.Status status) : base(completion)
        {
            this.status = status;
        }

        public Microsoft.Transactions.Bridge.Status Status =>
            this.status;
    }
}


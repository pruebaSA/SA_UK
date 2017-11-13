namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class CompletionEvent : SynchronizationEvent
    {
        protected CompletionEnlistment completion;

        protected CompletionEvent(CompletionEnlistment completion) : base(completion)
        {
            this.completion = completion;
        }

        public CompletionEnlistment Completion =>
            this.completion;
    }
}


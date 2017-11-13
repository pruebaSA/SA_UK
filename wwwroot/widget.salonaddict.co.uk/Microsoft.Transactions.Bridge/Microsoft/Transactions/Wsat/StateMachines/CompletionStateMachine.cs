namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CompletionStateMachine : StateMachine
    {
        public CompletionStateMachine(CompletionEnlistment completion) : base(completion)
        {
        }

        public override State AbortedState =>
            base.state.States.CompletionAborted;
    }
}


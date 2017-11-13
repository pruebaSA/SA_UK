namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CompletionInitializationFailed : TerminalState
    {
        public CompletionInitializationFailed(ProtocolState state) : base(state)
        {
        }
    }
}


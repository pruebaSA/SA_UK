namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class DurableInitializationFailed : TerminalState
    {
        public DurableInitializationFailed(ProtocolState state) : base(state)
        {
        }
    }
}


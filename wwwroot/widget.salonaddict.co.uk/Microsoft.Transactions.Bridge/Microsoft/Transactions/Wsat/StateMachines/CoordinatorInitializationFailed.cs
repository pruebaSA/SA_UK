namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CoordinatorInitializationFailed : TerminalState
    {
        public CoordinatorInitializationFailed(ProtocolState state) : base(state)
        {
        }
    }
}


namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class SubordinateFinished : TerminalState
    {
        public SubordinateFinished(ProtocolState state) : base(state)
        {
        }
    }
}


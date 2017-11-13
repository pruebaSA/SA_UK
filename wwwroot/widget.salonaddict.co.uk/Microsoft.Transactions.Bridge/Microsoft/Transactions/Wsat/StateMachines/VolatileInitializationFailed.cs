namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class VolatileInitializationFailed : TerminalState
    {
        public VolatileInitializationFailed(ProtocolState state) : base(state)
        {
        }
    }
}


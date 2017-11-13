namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class DecidedState : State
    {
        protected DecidedState(ProtocolState state) : base(state)
        {
        }
    }
}


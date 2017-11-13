namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class InactiveState : State
    {
        protected InactiveState(ProtocolState state) : base(state)
        {
        }
    }
}


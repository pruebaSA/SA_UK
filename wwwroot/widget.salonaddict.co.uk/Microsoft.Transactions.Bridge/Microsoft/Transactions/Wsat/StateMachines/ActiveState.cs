namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class ActiveState : State
    {
        protected ActiveState(ProtocolState state) : base(state)
        {
        }
    }
}


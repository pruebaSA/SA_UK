namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class VolatileCoordinatorStatusEvent : VolatileCoordinatorEvent
    {
        protected Microsoft.Transactions.Bridge.Status status;

        protected VolatileCoordinatorStatusEvent(VolatileCoordinatorEnlistment coordinator, Microsoft.Transactions.Bridge.Status status) : base(coordinator)
        {
            this.status = status;
        }

        public Microsoft.Transactions.Bridge.Status Status =>
            this.status;
    }
}


namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class VolatileCoordinatorEvent : SynchronizationEvent
    {
        protected VolatileCoordinatorEnlistment coordinator;

        protected VolatileCoordinatorEvent(VolatileCoordinatorEnlistment coordinator) : base(coordinator)
        {
            this.coordinator = coordinator;
        }

        public VolatileCoordinatorEnlistment VolatileCoordinator =>
            this.coordinator;
    }
}


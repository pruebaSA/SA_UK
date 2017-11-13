namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class CoordinatorEvent : SynchronizationEvent
    {
        protected CoordinatorEnlistment coordinator;

        protected CoordinatorEvent(CoordinatorEnlistment coordinator) : base(coordinator)
        {
            this.coordinator = coordinator;
        }

        public CoordinatorEnlistment Coordinator =>
            this.coordinator;
    }
}


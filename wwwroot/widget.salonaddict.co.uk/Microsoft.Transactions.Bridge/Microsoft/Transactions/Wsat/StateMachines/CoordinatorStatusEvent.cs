namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class CoordinatorStatusEvent : CoordinatorEvent
    {
        protected Microsoft.Transactions.Bridge.Status status;

        protected CoordinatorStatusEvent(CoordinatorEnlistment coordinator, Microsoft.Transactions.Bridge.Status status) : base(coordinator)
        {
            this.status = status;
        }

        public Microsoft.Transactions.Bridge.Status Status =>
            this.status;
    }
}


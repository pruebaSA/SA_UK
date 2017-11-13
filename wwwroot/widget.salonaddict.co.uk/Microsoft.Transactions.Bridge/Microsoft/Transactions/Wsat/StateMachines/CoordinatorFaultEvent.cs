namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal abstract class CoordinatorFaultEvent : CoordinatorEvent
    {
        protected MessageFault fault;

        protected CoordinatorFaultEvent(CoordinatorEnlistment coordinator, MessageFault fault) : base(coordinator)
        {
            this.fault = fault;
        }

        public MessageFault Fault =>
            this.fault;
    }
}


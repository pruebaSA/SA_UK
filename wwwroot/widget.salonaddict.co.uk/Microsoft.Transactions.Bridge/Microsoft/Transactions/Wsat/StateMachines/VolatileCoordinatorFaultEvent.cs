﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal abstract class VolatileCoordinatorFaultEvent : VolatileCoordinatorEvent
    {
        protected MessageFault fault;

        protected VolatileCoordinatorFaultEvent(VolatileCoordinatorEnlistment coordinator, MessageFault fault) : base(coordinator)
        {
            this.fault = fault;
        }

        public MessageFault Fault =>
            this.fault;
    }
}


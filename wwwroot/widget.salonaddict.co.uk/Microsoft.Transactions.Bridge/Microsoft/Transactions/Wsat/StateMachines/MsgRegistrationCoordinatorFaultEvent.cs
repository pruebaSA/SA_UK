namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal class MsgRegistrationCoordinatorFaultEvent : CoordinatorFaultEvent
    {
        private ControlProtocol protocol;

        public MsgRegistrationCoordinatorFaultEvent(CoordinatorEnlistment coordinator, ControlProtocol protocol, MessageFault fault) : base(coordinator, fault)
        {
            this.protocol = protocol;
        }

        public override void Execute(StateMachine stateMachine)
        {
            if (DebugTrace.Info)
            {
                base.state.DebugTraceSink.OnEvent(this);
            }
            stateMachine.State.OnEvent(this);
        }
    }
}


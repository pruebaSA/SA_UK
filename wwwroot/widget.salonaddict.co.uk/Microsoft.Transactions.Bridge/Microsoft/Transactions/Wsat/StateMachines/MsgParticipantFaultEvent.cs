namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal class MsgParticipantFaultEvent : ParticipantFaultEvent
    {
        public MsgParticipantFaultEvent(ParticipantEnlistment participant, MessageFault fault) : base(participant, fault)
        {
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


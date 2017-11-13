namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TmRegisterResponseEvent : ParticipantStatusEvent
    {
        private MsgRegisterEvent source;

        public TmRegisterResponseEvent(ParticipantEnlistment participant, Status status, MsgRegisterEvent source) : base(participant, status)
        {
            this.source = source;
        }

        public override void Execute(StateMachine stateMachine)
        {
            if (DebugTrace.Info)
            {
                base.state.DebugTraceSink.OnEvent(this);
            }
            stateMachine.State.OnEvent(this);
        }

        public MsgRegisterEvent SourceEvent =>
            this.source;
    }
}


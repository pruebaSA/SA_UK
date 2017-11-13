﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class MsgRegisterEvent : ParticipantEvent
    {
        private Register register;
        private RequestAsyncResult result;

        public MsgRegisterEvent(ParticipantEnlistment participant, ref Register register, RequestAsyncResult result) : base(participant)
        {
            this.register = register;
            this.result = result;
        }

        public override void Execute(StateMachine stateMachine)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
        }

        public ControlProtocol Protocol =>
            this.register.Protocol;

        public RequestAsyncResult Result =>
            this.result;
    }
}


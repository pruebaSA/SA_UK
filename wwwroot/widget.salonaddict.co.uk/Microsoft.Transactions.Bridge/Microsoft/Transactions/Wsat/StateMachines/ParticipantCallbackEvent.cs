namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class ParticipantCallbackEvent : ParticipantEvent
    {
        protected ProtocolProviderCallback callback;
        protected object callbackState;

        protected ParticipantCallbackEvent(ParticipantEnlistment participant, ProtocolProviderCallback callback, object state) : base(participant)
        {
            this.callback = callback;
            this.callbackState = state;
        }

        public ProtocolProviderCallback Callback =>
            this.callback;

        public object CallbackState =>
            this.callbackState;
    }
}


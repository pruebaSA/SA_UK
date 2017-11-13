namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class CoordinatorCallbackEvent : CoordinatorEvent
    {
        protected ProtocolProviderCallback callback;
        protected object callbackState;

        protected CoordinatorCallbackEvent(CoordinatorEnlistment coordinator, ProtocolProviderCallback callback, object state) : base(coordinator)
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


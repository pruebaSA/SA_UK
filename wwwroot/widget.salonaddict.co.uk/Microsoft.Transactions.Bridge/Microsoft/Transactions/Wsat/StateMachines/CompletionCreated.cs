﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CompletionCreated : ActiveState
    {
        public CompletionCreated(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(MsgRegisterCompletionEvent e)
        {
            CompletionEnlistment completion = e.Completion;
            completion.SetCompletionProxy(e.Proxy);
            base.state.RegistrationCoordinator.SendRegisterResponse(completion, e.Result, ControlProtocol.Completion, completion.CoordinatorService);
            e.StateMachine.ChangeState(base.state.States.CompletionActive);
        }

        public override void OnEvent(TmAsyncRollbackEvent e)
        {
            CompletionEnlistment completion = (CompletionEnlistment) e.Enlistment;
            completion.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Aborted(completion);
            e.StateMachine.ChangeState(base.state.States.CompletionAborted);
        }
    }
}


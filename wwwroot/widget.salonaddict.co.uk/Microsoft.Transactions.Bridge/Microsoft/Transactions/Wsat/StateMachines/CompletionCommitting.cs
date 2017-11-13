namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CompletionCommitting : DecidedState
    {
        public CompletionCommitting(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(MsgCompletionCommitEvent e)
        {
        }

        public override void OnEvent(TmAsyncRollbackEvent e)
        {
            CompletionEnlistment completion = (CompletionEnlistment) e.Enlistment;
            base.state.CompletionCoordinator.SendAborted(completion);
            completion.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Aborted(completion);
            e.StateMachine.ChangeState(base.state.States.CompletionAborted);
        }

        public override void OnEvent(TmCompletionCommitResponseEvent e)
        {
            State completionCommitted;
            CompletionEnlistment completion = e.Completion;
            switch (e.Status)
            {
                case Status.Committed:
                    base.state.CompletionCoordinator.SendCommitted(completion);
                    completionCommitted = base.state.States.CompletionCommitted;
                    break;

                case Status.Aborted:
                    base.state.CompletionCoordinator.SendAborted(completion);
                    completionCommitted = base.state.States.CompletionAborted;
                    break;

                default:
                    DiagnosticUtility.FailFast("Invalid status code");
                    completionCommitted = null;
                    break;
            }
            e.StateMachine.ChangeState(completionCommitted);
        }
    }
}


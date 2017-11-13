namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class DurableRegistering : InactiveState
    {
        public DurableRegistering(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(TmRegisterResponseEvent e)
        {
            base.ProcessTmRegisterResponse(e);
            if (e.Status == Status.Success)
            {
                e.StateMachine.ChangeState(base.state.States.DurableActive);
            }
            else
            {
                e.StateMachine.ChangeState(base.state.States.DurableInitializationFailed);
            }
        }
    }
}


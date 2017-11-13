namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class VolatileRegistering : InactiveState
    {
        public VolatileRegistering(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(TmRegisterResponseEvent e)
        {
            base.ProcessTmRegisterResponse(e);
            if (e.Status == Status.Success)
            {
                e.StateMachine.ChangeState(base.state.States.VolatilePhaseZeroActive);
            }
            else
            {
                e.StateMachine.ChangeState(base.state.States.VolatileInitializationFailed);
            }
        }
    }
}


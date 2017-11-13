﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class SubordinateRegistering : InactiveState
    {
        public SubordinateRegistering(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(TmSubordinateRegisterResponseEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            Status status = e.Status;
            if (status == Status.Success)
            {
                participant.OnSubordinateRegistered();
                participant.StateMachine.ChangeState(base.state.States.SubordinateActive);
            }
            else
            {
                if ((status == Status.TransactionNotFound) && !base.state.TransactionManager.Settings.NetworkInboundAccess)
                {
                    participant.ContextManager.Fault = base.state.Faults.SubordinateRegistrationNetAccessDisabled;
                }
                else
                {
                    participant.ContextManager.Fault = base.state.Faults.SubordinateTMRegistrationFailed(status);
                }
                participant.StateMachine.ChangeState(base.state.States.SubordinateFinished);
            }
        }
    }
}


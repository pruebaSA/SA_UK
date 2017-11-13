﻿namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.ServiceModel;

    internal class Faults10 : Faults
    {
        private Fault alreadyRegistered;
        private Fault contextRefused;
        private Fault disabled;
        private static Faults10 instance = new Faults10();
        private Fault tooManyEnlistments;

        private Faults10() : base(ProtocolVersion.Version10)
        {
            string reasonText = Microsoft.Transactions.SR.GetString("ContextRefusedReason");
            FaultCode code = new FaultCode(base.coordinationStrings.ContextRefused, base.coordinationStrings.Namespace);
            this.contextRefused = new Fault(base.coordinationStrings.FaultAction, code, reasonText);
            string str2 = Microsoft.Transactions.SR.GetString("AlreadyRegisteredReason");
            FaultCode code2 = new FaultCode(base.coordinationStrings.AlreadyRegistered, base.coordinationStrings.Namespace);
            this.alreadyRegistered = new Fault(base.coordinationStrings.FaultAction, code2, str2);
            string str3 = Microsoft.Transactions.SR.GetString("TooManyEnlistmentsReason");
            FaultCode code3 = new FaultCode("TooManyEnlistments", "http://schemas.microsoft.com/ws/2006/02/transactions");
            this.tooManyEnlistments = new Fault(base.atomicTransactionStrings.FaultAction, code3, str3);
            string str4 = Microsoft.Transactions.SR.GetString("DisabledReason");
            FaultCode code4 = new FaultCode("Disabled", "http://schemas.microsoft.com/ws/2006/02/transactions");
            this.disabled = new Fault(base.atomicTransactionStrings.FaultAction, code4, str4);
        }

        public override Fault ParticipantTMRegistrationFailed(Status status)
        {
            if (status == Status.TooManySubordinateEnlistments)
            {
                return this.tooManyEnlistments;
            }
            return base.invalidState;
        }

        public override Fault SubordinateTMRegistrationFailed(Status status)
        {
            if (status == Status.TooManySubordinateEnlistments)
            {
                return this.tooManyEnlistments;
            }
            return base.invalidState;
        }

        public override Fault TMEnlistFailed(Status status)
        {
            if (status == Status.TooManySubordinateEnlistments)
            {
                return this.tooManyEnlistments;
            }
            return base.invalidState;
        }

        public override Fault CannotCreateContext =>
            base.invalidState;

        public override Fault CompletionAlreadyRegistered =>
            this.alreadyRegistered;

        public override Fault CreateContextDispatchFailed =>
            base.invalidParameters;

        public static Faults Instance =>
            instance;

        public override Fault ParticipantRegistrationLoopback =>
            this.contextRefused;

        public override Fault ParticipantRegistrationNetAccessDisabled =>
            this.disabled;

        public override Fault RegistrationDispatchFailed =>
            base.invalidParameters;

        public override Fault RegistrationProxyFailed =>
            this.contextRefused;

        public override Fault SubordinateRegistrationNetAccessDisabled =>
            this.disabled;

        public override Fault UnknownCompletionEnlistment =>
            base.invalidState;

        public override Fault UnknownTransaction =>
            base.invalidState;
    }
}


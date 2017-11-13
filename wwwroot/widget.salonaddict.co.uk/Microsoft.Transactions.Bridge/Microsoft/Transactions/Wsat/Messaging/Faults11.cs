namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.ServiceModel;

    internal class Faults11 : Faults
    {
        private Fault cannotCreateContext;
        private Fault cannotRegisterParticipant;
        private static Faults11 instance = new Faults11();
        private Fault unknownTransaction;

        private Faults11() : base(ProtocolVersion.Version11)
        {
            string reasonText = Microsoft.Transactions.SR.GetString("CannotCreateContextReason");
            FaultCode code = new FaultCode(base.coordinationStrings.CannotCreateContext, base.coordinationStrings.Namespace);
            this.cannotCreateContext = new Fault(base.coordinationStrings.FaultAction, code, reasonText);
            string str2 = Microsoft.Transactions.SR.GetString("CannotRegisterParticipant");
            FaultCode code2 = new FaultCode(base.coordinationStrings.CannotRegisterParticipant, base.coordinationStrings.Namespace);
            this.cannotRegisterParticipant = new Fault(base.coordinationStrings.FaultAction, code2, str2);
            string str3 = Microsoft.Transactions.SR.GetString("UnknownTransactionReason");
            FaultCode code3 = new FaultCode(base.atomicTransactionStrings.UnknownTransaction, base.atomicTransactionStrings.Namespace);
            this.unknownTransaction = new Fault(base.atomicTransactionStrings.FaultAction, code3, str3);
        }

        public override Fault ParticipantTMRegistrationFailed(Status status) => 
            this.cannotRegisterParticipant;

        public override Fault SubordinateTMRegistrationFailed(Status status) => 
            this.cannotCreateContext;

        public override Fault TMEnlistFailed(Status status) => 
            this.cannotCreateContext;

        public override Fault CannotCreateContext =>
            this.cannotCreateContext;

        public override Fault CompletionAlreadyRegistered =>
            this.cannotRegisterParticipant;

        public override Fault CreateContextDispatchFailed =>
            this.cannotCreateContext;

        public static Faults Instance =>
            instance;

        public override Fault ParticipantRegistrationLoopback =>
            this.cannotRegisterParticipant;

        public override Fault ParticipantRegistrationNetAccessDisabled =>
            this.cannotRegisterParticipant;

        public override Fault RegistrationDispatchFailed =>
            this.cannotRegisterParticipant;

        public override Fault RegistrationProxyFailed =>
            this.cannotCreateContext;

        public override Fault SubordinateRegistrationNetAccessDisabled =>
            this.cannotCreateContext;

        public override Fault UnknownCompletionEnlistment =>
            this.cannotRegisterParticipant;

        public override Fault UnknownTransaction =>
            this.unknownTransaction;
    }
}


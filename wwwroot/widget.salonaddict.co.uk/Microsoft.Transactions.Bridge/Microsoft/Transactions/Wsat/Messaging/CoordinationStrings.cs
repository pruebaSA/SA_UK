namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class CoordinationStrings
    {
        protected CoordinationStrings()
        {
        }

        public static CoordinationStrings Version(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(CoordinationStrings), "V");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return CoordinationStrings10.Instance;

                case ProtocolVersion.Version11:
                    return CoordinationStrings11.Instance;
            }
            return null;
        }

        public string ActivationCoordinatorPortType =>
            "ActivationCoordinatorPortType";

        public string AlreadyRegistered =>
            "AlreadyRegistered";

        public string CannotCreateContext =>
            "CannotCreateContext";

        public string CannotRegisterParticipant =>
            "CannotRegisterParticipant";

        public string ContextRefused =>
            "ContextRefused";

        public string CoordinationContext =>
            "CoordinationContext";

        public string CoordinationType =>
            "CoordinationType";

        public string CoordinatorProtocolService =>
            "CoordinatorProtocolService";

        public string CreateCoordinationContext =>
            "CreateCoordinationContext";

        public abstract string CreateCoordinationContextAction { get; }

        public string CreateCoordinationContextResponse =>
            "CreateCoordinationContextResponse";

        public abstract string CreateCoordinationContextResponseAction { get; }

        public string CurrentContext =>
            "CurrentContext";

        public string Expires =>
            "Expires";

        public abstract string FaultAction { get; }

        public string Identifier =>
            "Identifier";

        public string InvalidParameters =>
            "InvalidParameters";

        public string InvalidProtocol =>
            "InvalidProtocol";

        public string InvalidState =>
            "InvalidState";

        public abstract string Namespace { get; }

        public string NoActivity =>
            "NoActivity";

        public string ParticipantProtocolService =>
            "ParticipantProtocolService";

        public string Prefix =>
            "wscoor";

        public string Protocol =>
            "ProtocolIdentifier";

        public string Register =>
            "Register";

        public abstract string RegisterAction { get; }

        public string RegisterResponse =>
            "RegisterResponse";

        public abstract string RegisterResponseAction { get; }

        public string RegistrationCoordinatorPortType =>
            "RegistrationCoordinatorPortType";

        public string RegistrationService =>
            "RegistrationService";
    }
}


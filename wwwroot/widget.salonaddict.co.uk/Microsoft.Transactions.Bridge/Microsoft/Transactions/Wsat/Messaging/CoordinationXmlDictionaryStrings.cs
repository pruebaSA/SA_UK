namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal abstract class CoordinationXmlDictionaryStrings
    {
        protected CoordinationXmlDictionaryStrings()
        {
        }

        public static CoordinationXmlDictionaryStrings Version(ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(CoordinationXmlDictionaryStrings), "V");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return CoordinationXmlDictionaryStrings10.Instance;

                case ProtocolVersion.Version11:
                    return CoordinationXmlDictionaryStrings11.Instance;
            }
            return null;
        }

        public XmlDictionaryString ActivationCoordinatorPortType =>
            XD.CoordinationExternalDictionary.ActivationCoordinatorPortType;

        public XmlDictionaryString AlreadyRegistered =>
            XD.CoordinationExternalDictionary.AlreadyRegistered;

        public XmlDictionaryString CannotCreateContext =>
            DXD.CoordinationExternal11Dictionary.CannotCreateContext;

        public XmlDictionaryString CannotRegisterParticipant =>
            DXD.CoordinationExternal11Dictionary.CannotRegisterParticipant;

        public XmlDictionaryString ContextRefused =>
            XD.CoordinationExternalDictionary.ContextRefused;

        public XmlDictionaryString CoordinationContext =>
            XD.CoordinationExternalDictionary.CoordinationContext;

        public XmlDictionaryString CoordinationType =>
            XD.CoordinationExternalDictionary.CoordinationType;

        public XmlDictionaryString CoordinatorProtocolService =>
            XD.CoordinationExternalDictionary.CoordinatorProtocolService;

        public XmlDictionaryString CreateCoordinationContext =>
            XD.CoordinationExternalDictionary.CreateCoordinationContext;

        public abstract XmlDictionaryString CreateCoordinationContextAction { get; }

        public XmlDictionaryString CreateCoordinationContextResponse =>
            XD.CoordinationExternalDictionary.CreateCoordinationContextResponse;

        public abstract XmlDictionaryString CreateCoordinationContextResponseAction { get; }

        public XmlDictionaryString CurrentContext =>
            XD.CoordinationExternalDictionary.CurrentContext;

        public XmlDictionaryString Expires =>
            XD.CoordinationExternalDictionary.Expires;

        public abstract XmlDictionaryString FaultAction { get; }

        public XmlDictionaryString Identifier =>
            XD.CoordinationExternalDictionary.Identifier;

        public XmlDictionaryString InvalidParameters =>
            XD.CoordinationExternalDictionary.InvalidParameters;

        public XmlDictionaryString InvalidProtocol =>
            XD.CoordinationExternalDictionary.InvalidProtocol;

        public XmlDictionaryString InvalidState =>
            XD.CoordinationExternalDictionary.InvalidState;

        public abstract XmlDictionaryString Namespace { get; }

        public XmlDictionaryString NoActivity =>
            XD.CoordinationExternalDictionary.NoActivity;

        public XmlDictionaryString ParticipantProtocolService =>
            XD.CoordinationExternalDictionary.ParticipantProtocolService;

        public XmlDictionaryString Prefix =>
            XD.CoordinationExternalDictionary.Prefix;

        public XmlDictionaryString Protocol =>
            XD.CoordinationExternalDictionary.Protocol;

        public XmlDictionaryString Register =>
            XD.CoordinationExternalDictionary.Register;

        public abstract XmlDictionaryString RegisterAction { get; }

        public XmlDictionaryString RegisterResponse =>
            XD.CoordinationExternalDictionary.RegisterResponse;

        public abstract XmlDictionaryString RegisterResponseAction { get; }

        public XmlDictionaryString RegistrationCoordinatorPortType =>
            XD.CoordinationExternalDictionary.RegistrationCoordinatorPortType;

        public XmlDictionaryString RegistrationService =>
            XD.CoordinationExternalDictionary.RegistrationService;
    }
}


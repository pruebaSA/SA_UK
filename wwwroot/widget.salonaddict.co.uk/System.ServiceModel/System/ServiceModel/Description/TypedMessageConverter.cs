namespace System.ServiceModel.Description
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    public abstract class TypedMessageConverter
    {
        protected TypedMessageConverter()
        {
        }

        public static TypedMessageConverter Create(Type messageContract, string action) => 
            Create(messageContract, action, null, TypeLoader.DefaultDataContractFormatAttribute);

        public static TypedMessageConverter Create(Type messageContract, string action, DataContractFormatAttribute formatterAttribute) => 
            Create(messageContract, action, null, formatterAttribute);

        public static TypedMessageConverter Create(Type messageContract, string action, XmlSerializerFormatAttribute formatterAttribute) => 
            Create(messageContract, action, null, formatterAttribute);

        public static TypedMessageConverter Create(Type messageContract, string action, string defaultNamespace) => 
            Create(messageContract, action, defaultNamespace, TypeLoader.DefaultDataContractFormatAttribute);

        public static TypedMessageConverter Create(Type messageContract, string action, string defaultNamespace, DataContractFormatAttribute formatterAttribute)
        {
            if (messageContract == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("messageContract"));
            }
            if (!messageContract.IsDefined(typeof(MessageContractAttribute), false))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("SFxMessageContractAttributeRequired", new object[] { messageContract }), "messageContract"));
            }
            if (defaultNamespace == null)
            {
                defaultNamespace = "http://tempuri.org/";
            }
            return new XmlMessageConverter(GetOperationFormatter(messageContract, formatterAttribute, defaultNamespace, action));
        }

        public static TypedMessageConverter Create(Type messageContract, string action, string defaultNamespace, XmlSerializerFormatAttribute formatterAttribute)
        {
            if (messageContract == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("messageContract"));
            }
            if (defaultNamespace == null)
            {
                defaultNamespace = "http://tempuri.org/";
            }
            return new XmlMessageConverter(GetOperationFormatter(messageContract, formatterAttribute, defaultNamespace, action));
        }

        public abstract object FromMessage(Message message);
        private static OperationFormatter GetOperationFormatter(Type t, Attribute formatAttribute, string defaultNS, string action)
        {
            bool flag = formatAttribute is XmlSerializerFormatAttribute;
            MessageDescription item = new TypeLoader().CreateTypedMessageDescription(t, null, null, defaultNS, action, MessageDirection.Output);
            ContractDescription declaringContract = new ContractDescription("dummy_contract", defaultNS);
            OperationDescription operation = new OperationDescription(NamingHelper.XmlName(t.Name), declaringContract, false);
            operation.Messages.Add(item);
            if (flag)
            {
                return XmlSerializerOperationBehavior.CreateOperationFormatter(operation, (XmlSerializerFormatAttribute) formatAttribute);
            }
            return new DataContractSerializerOperationFormatter(operation, (DataContractFormatAttribute) formatAttribute, null);
        }

        public abstract Message ToMessage(object typedMessage);
        public abstract Message ToMessage(object typedMessage, MessageVersion version);
    }
}


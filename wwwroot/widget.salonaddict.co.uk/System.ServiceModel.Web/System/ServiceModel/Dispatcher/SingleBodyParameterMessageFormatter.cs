namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Xml;

    internal abstract class SingleBodyParameterMessageFormatter : IDispatchMessageFormatter, IClientMessageFormatter
    {
        private string contractName;
        private string contractNs;
        private bool isRequestFormatter;
        private string operationName;
        private string serializerType;

        protected SingleBodyParameterMessageFormatter(OperationDescription operation, bool isRequestFormatter, string serializerType)
        {
            if (operation == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operation");
            }
            this.contractName = operation.DeclaringContract.Name;
            this.contractNs = operation.DeclaringContract.Namespace;
            this.operationName = operation.Name;
            this.isRequestFormatter = isRequestFormatter;
            this.serializerType = serializerType;
        }

        protected virtual void AttachMessageProperties(Message message, bool isRequest)
        {
        }

        private BodyWriter CreateBodyWriter(object body)
        {
            XmlObjectSerializer outputSerializer;
            if (body != null)
            {
                outputSerializer = this.GetOutputSerializer(body.GetType());
                if (outputSerializer == null)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.CannotSerializeType, new object[] { body.GetType(), this.operationName, this.contractName, this.contractNs, this.serializerType })));
                }
            }
            else
            {
                outputSerializer = null;
            }
            return new SingleParameterBodyWriter(body, outputSerializer);
        }

        internal static IClientMessageFormatter CreateClientFormatter(OperationDescription operation, Type type, bool isRequestFormatter, bool useJson, UnwrappedTypesXmlSerializerManager xmlSerializerManager)
        {
            if (type == null)
            {
                return new NullMessageFormatter();
            }
            return CreateFormatter(operation, type, isRequestFormatter, useJson, xmlSerializerManager);
        }

        internal static IDispatchMessageFormatter CreateDispatchFormatter(OperationDescription operation, Type type, bool isRequestFormatter, bool useJson, UnwrappedTypesXmlSerializerManager xmlSerializerManager)
        {
            if (type == null)
            {
                return new NullMessageFormatter();
            }
            return CreateFormatter(operation, type, isRequestFormatter, useJson, xmlSerializerManager);
        }

        private static SingleBodyParameterMessageFormatter CreateFormatter(OperationDescription operation, Type type, bool isRequestFormatter, bool useJson, UnwrappedTypesXmlSerializerManager xmlSerializerManager)
        {
            IOperationBehavior behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
            if (behavior == null)
            {
                behavior = operation.Behaviors.Find<XmlSerializerOperationBehavior>();
            }
            return CreateFormatter(behavior, operation, type, isRequestFormatter, useJson, xmlSerializerManager);
        }

        private static SingleBodyParameterMessageFormatter CreateFormatter(IOperationBehavior behavior, OperationDescription operation, Type type, bool isRequestFormatter, bool useJson, UnwrappedTypesXmlSerializerManager xmlSerializerManager)
        {
            DataContractSerializerOperationBehavior dcsob = behavior as DataContractSerializerOperationBehavior;
            if (useJson && (dcsob == null))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.JsonFormatRequiresDataContract, new object[] { operation.Name, operation.DeclaringContract.Name, operation.DeclaringContract.Namespace })));
            }
            if (dcsob != null)
            {
                return new SingleBodyParameterDataContractMessageFormatter(operation, type, isRequestFormatter, useJson, dcsob);
            }
            XmlSerializerOperationBehavior xsob = behavior as XmlSerializerOperationBehavior;
            if (xsob == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.OnlyDataContractAndXmlSerializerTypesInUnWrappedMode, new object[] { operation.Name })));
            }
            return new SingleBodyParameterXmlSerializerMessageFormatter(operation, type, isRequestFormatter, xsob, xmlSerializerManager);
        }

        public static IClientMessageFormatter CreateXmlAndJsonClientFormatter(OperationDescription operation, Type type, bool isRequestFormatter, UnwrappedTypesXmlSerializerManager xmlSerializerManager)
        {
            IClientMessageFormatter defaultFormatter = CreateClientFormatter(operation, type, isRequestFormatter, false, xmlSerializerManager);
            if (!WebHttpBehavior.SupportsJsonFormat(operation))
            {
                return defaultFormatter;
            }
            IClientMessageFormatter formatter2 = CreateClientFormatter(operation, type, isRequestFormatter, true, xmlSerializerManager);
            return new DemultiplexingClientMessageFormatter(new Dictionary<WebContentFormat, IClientMessageFormatter> { 
                { 
                    WebContentFormat.Xml,
                    defaultFormatter
                },
                { 
                    WebContentFormat.Json,
                    formatter2
                }
            }, defaultFormatter);
        }

        public static IDispatchMessageFormatter CreateXmlAndJsonDispatchFormatter(OperationDescription operation, Type type, bool isRequestFormatter, UnwrappedTypesXmlSerializerManager xmlSerializerManager)
        {
            IDispatchMessageFormatter defaultFormatter = CreateDispatchFormatter(operation, type, isRequestFormatter, false, xmlSerializerManager);
            if (!WebHttpBehavior.SupportsJsonFormat(operation))
            {
                return defaultFormatter;
            }
            IDispatchMessageFormatter formatter2 = CreateDispatchFormatter(operation, type, isRequestFormatter, true, xmlSerializerManager);
            return new DemultiplexingDispatchMessageFormatter(new Dictionary<WebContentFormat, IDispatchMessageFormatter> { 
                { 
                    WebContentFormat.Xml,
                    defaultFormatter
                },
                { 
                    WebContentFormat.Json,
                    formatter2
                }
            }, defaultFormatter);
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            if (this.isRequestFormatter)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.FormatterCannotBeUsedForReplyMessages, new object[0])));
            }
            return this.ReadObject(message);
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (!this.isRequestFormatter)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.FormatterCannotBeUsedForRequestMessages, new object[0])));
            }
            parameters[0] = this.ReadObject(message);
        }

        protected abstract XmlObjectSerializer[] GetInputSerializers();
        protected abstract XmlObjectSerializer GetOutputSerializer(Type type);
        private object ReadObject(Message message)
        {
            if (HttpStreamFormatter.IsEmptyMessage(message))
            {
                return null;
            }
            XmlObjectSerializer[] inputSerializers = this.GetInputSerializers();
            XmlDictionaryReader readerAtBodyContents = message.GetReaderAtBodyContents();
            if (inputSerializers != null)
            {
                for (int i = 0; i < inputSerializers.Length; i++)
                {
                    if (inputSerializers[i].IsStartObject(readerAtBodyContents))
                    {
                        return inputSerializers[i].ReadObject(readerAtBodyContents, false);
                    }
                }
            }
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(SR2.GetString(SR2.CannotDeserializeBody, new object[] { readerAtBodyContents.LocalName, readerAtBodyContents.NamespaceURI, this.operationName, this.contractName, this.contractNs, this.serializerType })));
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            if (this.isRequestFormatter)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.FormatterCannotBeUsedForReplyMessages, new object[0])));
            }
            Message message = Message.CreateMessage(messageVersion, (string) null, this.CreateBodyWriter(result));
            if (result == null)
            {
                SuppressReplyEntityBody(message);
            }
            this.AttachMessageProperties(message, false);
            return message;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            if (!this.isRequestFormatter)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.FormatterCannotBeUsedForRequestMessages, new object[0])));
            }
            Message message = Message.CreateMessage(messageVersion, (string) null, this.CreateBodyWriter(parameters[0]));
            if (parameters[0] == null)
            {
                SuppressRequestEntityBody(message);
            }
            this.AttachMessageProperties(message, true);
            return message;
        }

        internal static void SuppressReplyEntityBody(Message message)
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = true;
            }
            else
            {
                object obj2;
                message.Properties.TryGetValue(HttpResponseMessageProperty.Name, out obj2);
                HttpResponseMessageProperty property = obj2 as HttpResponseMessageProperty;
                if (property == null)
                {
                    property = new HttpResponseMessageProperty();
                    message.Properties[HttpResponseMessageProperty.Name] = property;
                }
                property.SuppressEntityBody = true;
            }
        }

        internal static void SuppressRequestEntityBody(Message message)
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingRequest.SuppressEntityBody = true;
            }
            else
            {
                object obj2;
                message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj2);
                HttpRequestMessageProperty property = obj2 as HttpRequestMessageProperty;
                if (property == null)
                {
                    property = new HttpRequestMessageProperty();
                    message.Properties[HttpRequestMessageProperty.Name] = property;
                }
                property.SuppressEntityBody = true;
            }
        }

        protected virtual void ValidateMessageFormatProperty(Message message)
        {
        }

        protected void ValidateOutputType(Type type, Type parameterType, IList<Type> knownTypes)
        {
            bool flag = false;
            if (type == parameterType)
            {
                flag = true;
            }
            else if (knownTypes != null)
            {
                for (int i = 0; i < knownTypes.Count; i++)
                {
                    if (type == knownTypes[i])
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(SR2.GetString(SR2.TypeIsNotParameterTypeAndIsNotPresentInKnownTypes, new object[] { type, this.OperationName, this.ContractName, parameterType })));
            }
        }

        protected string ContractName =>
            this.contractName;

        protected string ContractNs =>
            this.contractNs;

        protected string OperationName =>
            this.operationName;

        private class NullMessageFormatter : IClientMessageFormatter, IDispatchMessageFormatter
        {
            public object DeserializeReply(Message message, object[] parameters) => 
                null;

            public void DeserializeRequest(Message message, object[] parameters)
            {
            }

            public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
            {
                Message message = Message.CreateMessage(messageVersion, (string) null);
                SingleBodyParameterMessageFormatter.SuppressReplyEntityBody(message);
                return message;
            }

            public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
            {
                Message message = Message.CreateMessage(messageVersion, (string) null);
                SingleBodyParameterMessageFormatter.SuppressRequestEntityBody(message);
                return message;
            }
        }

        private class SingleParameterBodyWriter : BodyWriter
        {
            private object body;
            private XmlObjectSerializer serializer;

            public SingleParameterBodyWriter(object body, XmlObjectSerializer serializer) : base(false)
            {
                if ((body != null) && (serializer == null))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
                }
                this.body = body;
                this.serializer = serializer;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                if (this.body != null)
                {
                    this.serializer.WriteObject(writer, this.body);
                }
            }
        }
    }
}


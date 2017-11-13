namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    internal class SingleBodyParameterDataContractMessageFormatter : SingleBodyParameterMessageFormatter
    {
        private XmlObjectSerializer cachedOutputSerializer;
        private Type cachedOutputSerializerType;
        private bool ignoreExtensionData;
        private XmlObjectSerializer[] inputSerializers;
        private IList<Type> knownTypes;
        private int maxItemsInObjectGraph;
        private Type parameterType;
        private IDataContractSurrogate surrogate;
        private object thisLock;
        private static readonly Type TypeOfNullable = typeof(Nullable<>);
        private bool useJsonFormat;

        public SingleBodyParameterDataContractMessageFormatter(OperationDescription operation, Type parameterType, bool isRequestFormatter, bool useJsonFormat, DataContractSerializerOperationBehavior dcsob) : base(operation, isRequestFormatter, "DataContractSerializer")
        {
            if (operation == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operation");
            }
            if (parameterType == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterType");
            }
            if (dcsob == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dcsob");
            }
            this.parameterType = parameterType;
            List<Type> list = new List<Type>();
            if (operation.KnownTypes != null)
            {
                foreach (Type type in operation.KnownTypes)
                {
                    list.Add(type);
                }
            }
            Type item = UnwrapNullableType(this.parameterType);
            if (item != this.parameterType)
            {
                list.Add(item);
            }
            this.surrogate = dcsob.DataContractSurrogate;
            this.ignoreExtensionData = dcsob.IgnoreExtensionDataObject;
            this.maxItemsInObjectGraph = dcsob.MaxItemsInObjectGraph;
            this.knownTypes = list.AsReadOnly();
            ValidateType(parameterType, this.surrogate, this.knownTypes);
            this.useJsonFormat = useJsonFormat;
            this.CreateInputSerializers(parameterType);
            this.thisLock = new object();
        }

        protected override void AttachMessageProperties(Message message, bool isRequest)
        {
            if (this.useJsonFormat)
            {
                message.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.JsonProperty);
            }
        }

        private void CreateInputSerializers(Type type)
        {
            List<XmlObjectSerializer> list = new List<XmlObjectSerializer> {
                this.CreateSerializer(type)
            };
            foreach (Type type2 in this.knownTypes)
            {
                list.Add(this.CreateSerializer(type2));
            }
            this.inputSerializers = list.ToArray();
        }

        private XmlObjectSerializer CreateSerializer(Type type)
        {
            if (this.useJsonFormat)
            {
                return new DataContractJsonSerializer(type, this.knownTypes, this.maxItemsInObjectGraph, this.ignoreExtensionData, this.surrogate, false);
            }
            return new DataContractSerializer(type, this.knownTypes, this.maxItemsInObjectGraph, this.ignoreExtensionData, false, this.surrogate);
        }

        protected override XmlObjectSerializer[] GetInputSerializers() => 
            this.inputSerializers;

        protected override XmlObjectSerializer GetOutputSerializer(Type type)
        {
            lock (this.thisLock)
            {
                if (this.cachedOutputSerializerType != type)
                {
                    base.ValidateOutputType(type, this.parameterType, this.knownTypes);
                    this.cachedOutputSerializer = this.CreateSerializer(type);
                    this.cachedOutputSerializerType = type;
                }
                return this.cachedOutputSerializer;
            }
        }

        internal static Type UnwrapNullableType(Type type)
        {
            while (type.IsGenericType && (type.GetGenericTypeDefinition() == TypeOfNullable))
            {
                type = type.GetGenericArguments()[0];
            }
            return type;
        }

        protected override void ValidateMessageFormatProperty(Message message)
        {
            if (this.useJsonFormat)
            {
                object obj2;
                message.Properties.TryGetValue("WebBodyFormatMessageProperty", out obj2);
                WebBodyFormatMessageProperty property = obj2 as WebBodyFormatMessageProperty;
                if (property == null)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR2.GetString(SR2.MessageFormatPropertyNotFound, new object[] { base.OperationName, base.ContractName, base.ContractNs })));
                }
                if (property.Format != WebContentFormat.Json)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR2.GetString(SR2.InvalidHttpMessageFormat, new object[] { base.OperationName, base.ContractName, base.ContractNs, property.Format, WebContentFormat.Json })));
                }
            }
            else
            {
                base.ValidateMessageFormatProperty(message);
            }
        }

        private static void ValidateType(Type parameterType, IDataContractSurrogate surrogate, IEnumerable<Type> knownTypes)
        {
            XsdDataContractExporter exporter = new XsdDataContractExporter();
            if ((surrogate != null) || (knownTypes != null))
            {
                ExportOptions options = new ExportOptions {
                    DataContractSurrogate = surrogate
                };
                if (knownTypes != null)
                {
                    foreach (Type type in knownTypes)
                    {
                        options.KnownTypes.Add(type);
                    }
                }
                exporter.Options = options;
            }
            exporter.GetSchemaTypeName(parameterType);
        }
    }
}


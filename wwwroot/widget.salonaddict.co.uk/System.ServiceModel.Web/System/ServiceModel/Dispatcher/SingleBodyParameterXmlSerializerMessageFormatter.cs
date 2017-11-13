namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    internal class SingleBodyParameterXmlSerializerMessageFormatter : SingleBodyParameterMessageFormatter
    {
        private XmlObjectSerializer cachedOutputSerializer;
        private Type cachedOutputSerializerType;
        private List<Type> knownTypes;
        private Type parameterType;
        private UnwrappedTypesXmlSerializerManager serializerManager;
        private XmlObjectSerializer[] serializers;
        private object thisLock;
        private UnwrappedTypesXmlSerializerManager.TypeSerializerPair[] typeSerializerPairs;

        public SingleBodyParameterXmlSerializerMessageFormatter(OperationDescription operation, Type parameterType, bool isRequestFormatter, XmlSerializerOperationBehavior xsob, UnwrappedTypesXmlSerializerManager serializerManager) : base(operation, isRequestFormatter, "XmlSerializer")
        {
            if (operation == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operation");
            }
            if (parameterType == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterType");
            }
            if (xsob == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("xsob");
            }
            if (serializerManager == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializerManager");
            }
            this.serializerManager = serializerManager;
            this.parameterType = parameterType;
            List<Type> types = new List<Type> {
                parameterType
            };
            this.knownTypes = new List<Type>();
            if (operation.KnownTypes != null)
            {
                foreach (Type type in operation.KnownTypes)
                {
                    this.knownTypes.Add(type);
                    types.Add(type);
                }
            }
            Type item = SingleBodyParameterDataContractMessageFormatter.UnwrapNullableType(this.parameterType);
            if (item != this.parameterType)
            {
                this.knownTypes.Add(item);
                types.Add(item);
            }
            this.serializerManager.RegisterType(this, types);
            this.thisLock = new object();
        }

        private void EnsureSerializers()
        {
            if (this.typeSerializerPairs == null)
            {
                this.typeSerializerPairs = this.serializerManager.GetOperationSerializers(this);
                this.serializers = new XmlObjectSerializer[this.typeSerializerPairs.Length];
                for (int i = 0; i < this.typeSerializerPairs.Length; i++)
                {
                    this.serializers[i] = this.typeSerializerPairs[i].Serializer;
                }
            }
        }

        protected override XmlObjectSerializer[] GetInputSerializers()
        {
            lock (this.thisLock)
            {
                this.EnsureSerializers();
                return this.serializers;
            }
        }

        protected override XmlObjectSerializer GetOutputSerializer(Type type)
        {
            lock (this.thisLock)
            {
                if (this.cachedOutputSerializerType != type)
                {
                    base.ValidateOutputType(type, this.parameterType, this.knownTypes);
                    this.EnsureSerializers();
                    bool flag = false;
                    if (this.typeSerializerPairs != null)
                    {
                        for (int i = 0; i < this.typeSerializerPairs.Length; i++)
                        {
                            if (type == this.typeSerializerPairs[i].Type)
                            {
                                this.cachedOutputSerializer = this.typeSerializerPairs[i].Serializer;
                                this.cachedOutputSerializerType = type;
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        return null;
                    }
                }
                return this.cachedOutputSerializer;
            }
        }
    }
}


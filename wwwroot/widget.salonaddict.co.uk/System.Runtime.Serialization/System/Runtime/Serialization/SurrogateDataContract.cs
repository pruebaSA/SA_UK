namespace System.Runtime.Serialization
{
    using System;
    using System.Security;

    internal sealed class SurrogateDataContract : DataContract
    {
        [SecurityCritical]
        private SurrogateDataContractCriticalHelper helper;

        [SecurityCritical, SecurityTreatAsSafe]
        internal SurrogateDataContract(Type type, ISerializationSurrogate serializationSurrogate) : base(new SurrogateDataContractCriticalHelper(type, serializationSurrogate))
        {
            this.helper = base.Helper as SurrogateDataContractCriticalHelper;
        }

        public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
        {
            xmlReader.Read();
            Type underlyingType = base.UnderlyingType;
            object obj2 = underlyingType.IsArray ? Array.CreateInstance(underlyingType.GetElementType(), 0) : FormatterServices.GetUninitializedObject(underlyingType);
            context.AddNewObject(obj2);
            string objectId = context.GetObjectId();
            SerializationInfo info = context.ReadSerializationInfo(xmlReader, underlyingType);
            object newObj = this.SerializationSurrogate.SetObjectData(obj2, info, context.GetStreamingContext(), null);
            if (newObj == null)
            {
                newObj = obj2;
            }
            if (newObj is IDeserializationCallback)
            {
                ((IDeserializationCallback) newObj).OnDeserialization(null);
            }
            if (newObj is IObjectReference)
            {
                newObj = ((IObjectReference) newObj).GetRealObject(context.GetStreamingContext());
            }
            context.ReplaceDeserializedObject(objectId, obj2, newObj);
            xmlReader.ReadEndElement();
            return newObj;
        }

        public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
        {
            SerializationInfo info = new SerializationInfo(base.UnderlyingType, XmlObjectSerializer.FormatterConverter);
            this.SerializationSurrogate.GetObjectData(obj, info, context.GetStreamingContext());
            context.WriteSerializationInfo(xmlWriter, base.UnderlyingType, info);
        }

        internal ISerializationSurrogate SerializationSurrogate =>
            this.helper.SerializationSurrogate;

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private class SurrogateDataContractCriticalHelper : DataContract.DataContractCriticalHelper
        {
            private ISerializationSurrogate serializationSurrogate;

            internal SurrogateDataContractCriticalHelper(Type type, ISerializationSurrogate serializationSurrogate) : base(type)
            {
                string str;
                string str2;
                this.serializationSurrogate = serializationSurrogate;
                DataContract.GetDefaultStableName(DataContract.GetClrTypeFullName(type), out str, out str2);
                base.SetDataContractName(DataContract.CreateQualifiedName(str, str2));
            }

            internal ISerializationSurrogate SerializationSurrogate =>
                this.serializationSurrogate;
        }
    }
}


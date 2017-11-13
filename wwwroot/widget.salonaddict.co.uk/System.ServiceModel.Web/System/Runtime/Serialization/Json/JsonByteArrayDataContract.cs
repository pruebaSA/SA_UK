namespace System.Runtime.Serialization.Json
{
    using System;
    using System.Runtime.Serialization;

    internal class JsonByteArrayDataContract : JsonDataContract
    {
        public JsonByteArrayDataContract(ByteArrayDataContract traditionalByteArrayDataContract) : base(traditionalByteArrayDataContract)
        {
        }

        public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
        {
            if (context != null)
            {
                return JsonDataContract.HandleReadValue(jsonReader.ReadElementContentAsBase64(), context);
            }
            if (!JsonDataContract.TryReadNullAtTopLevel(jsonReader))
            {
                return jsonReader.ReadElementContentAsBase64();
            }
            return null;
        }
    }
}


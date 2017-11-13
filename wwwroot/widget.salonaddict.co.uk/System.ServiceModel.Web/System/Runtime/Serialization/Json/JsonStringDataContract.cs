namespace System.Runtime.Serialization.Json
{
    using System;
    using System.Runtime.Serialization;

    internal class JsonStringDataContract : JsonDataContract
    {
        public JsonStringDataContract(StringDataContract traditionalStringDataContract) : base(traditionalStringDataContract)
        {
        }

        public override object ReadJsonValueCore(XmlReaderDelegator jsonReader, XmlObjectSerializerReadContextComplexJson context)
        {
            if (context != null)
            {
                return JsonDataContract.HandleReadValue(jsonReader.ReadElementContentAsString(), context);
            }
            if (!JsonDataContract.TryReadNullAtTopLevel(jsonReader))
            {
                return jsonReader.ReadElementContentAsString();
            }
            return null;
        }
    }
}


namespace System.Runtime.Serialization
{
    using System;

    internal class UnsignedLongDataContract : PrimitiveDataContract
    {
        internal UnsignedLongDataContract() : base(typeof(ulong), DictionaryGlobals.UnsignedLongLocalName, DictionaryGlobals.SchemaNamespace)
        {
        }

        public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
        {
            if (context != null)
            {
                return base.HandleReadValue(reader.ReadElementContentAsUnsignedLong(), context);
            }
            return reader.ReadElementContentAsUnsignedLong();
        }

        public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
        {
            writer.WriteUnsignedLong((ulong) obj);
        }

        internal override string ReadMethodName =>
            "ReadElementContentAsUnsignedLong";

        internal override string WriteMethodName =>
            "WriteUnsignedLong";
    }
}


namespace System.Runtime.Serialization
{
    using System;

    internal class DateTimeDataContract : PrimitiveDataContract
    {
        internal DateTimeDataContract() : base(typeof(DateTime), DictionaryGlobals.DateTimeLocalName, DictionaryGlobals.SchemaNamespace)
        {
        }

        public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
        {
            if (context != null)
            {
                return base.HandleReadValue(reader.ReadElementContentAsDateTime(), context);
            }
            return reader.ReadElementContentAsDateTime();
        }

        public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
        {
            writer.WriteDateTime((DateTime) obj);
        }

        internal override string ReadMethodName =>
            "ReadElementContentAsDateTime";

        internal override string WriteMethodName =>
            "WriteDateTime";
    }
}


namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Schema;

    internal class SchemaObjectInfo
    {
        internal XmlSchemaElement element;
        internal List<XmlSchemaType> knownTypes;
        internal XmlSchema schema;
        internal XmlSchemaType type;

        internal SchemaObjectInfo(XmlSchemaType type, XmlSchemaElement element, XmlSchema schema, List<XmlSchemaType> knownTypes)
        {
            this.type = type;
            this.element = element;
            this.schema = schema;
            this.knownTypes = knownTypes;
        }
    }
}


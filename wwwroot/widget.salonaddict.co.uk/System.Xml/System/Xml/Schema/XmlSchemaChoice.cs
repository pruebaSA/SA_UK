namespace System.Xml.Schema
{
    using System;
    using System.Xml.Serialization;

    public class XmlSchemaChoice : XmlSchemaGroupBase
    {
        private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();

        internal override void SetItems(XmlSchemaObjectCollection newItems)
        {
            this.items = newItems;
        }

        internal override bool IsEmpty =>
            base.IsEmpty;

        [XmlElement("sequence", typeof(XmlSchemaSequence)), XmlElement("any", typeof(XmlSchemaAny)), XmlElement("element", typeof(XmlSchemaElement)), XmlElement("choice", typeof(XmlSchemaChoice)), XmlElement("group", typeof(XmlSchemaGroupRef))]
        public override XmlSchemaObjectCollection Items =>
            this.items;
    }
}


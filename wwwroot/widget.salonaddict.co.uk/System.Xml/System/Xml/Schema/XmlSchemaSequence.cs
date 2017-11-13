namespace System.Xml.Schema
{
    using System;
    using System.Xml.Serialization;

    public class XmlSchemaSequence : XmlSchemaGroupBase
    {
        private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();

        internal override void SetItems(XmlSchemaObjectCollection newItems)
        {
            this.items = newItems;
        }

        internal override bool IsEmpty
        {
            get
            {
                if (!base.IsEmpty)
                {
                    return (this.items.Count == 0);
                }
                return true;
            }
        }

        [XmlElement("choice", typeof(XmlSchemaChoice)), XmlElement("any", typeof(XmlSchemaAny)), XmlElement("group", typeof(XmlSchemaGroupRef)), XmlElement("element", typeof(XmlSchemaElement)), XmlElement("sequence", typeof(XmlSchemaSequence))]
        public override XmlSchemaObjectCollection Items =>
            this.items;
    }
}


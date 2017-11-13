namespace System.Xml
{
    using System;
    using System.Xml.Schema;

    internal sealed class XmlNameEx : XmlName
    {
        private object decl;
        private byte flags;
        private const byte IsDefaultBit = 4;
        private const byte IsNilBit = 8;
        private XmlSchemaSimpleType memberType;
        private XmlSchemaType schemaType;
        private const byte ValidityMask = 3;

        internal XmlNameEx(string prefix, string localName, string ns, int hashCode, XmlDocument ownerDoc, XmlName next, IXmlSchemaInfo schemaInfo) : base(prefix, localName, ns, hashCode, ownerDoc, next)
        {
            this.SetValidity(schemaInfo.Validity);
            this.SetIsDefault(schemaInfo.IsDefault);
            this.SetIsNil(schemaInfo.IsNil);
            this.memberType = schemaInfo.MemberType;
            this.schemaType = schemaInfo.SchemaType;
            this.decl = (schemaInfo.SchemaElement != null) ? ((object) schemaInfo.SchemaElement) : ((object) schemaInfo.SchemaAttribute);
        }

        public override bool Equals(IXmlSchemaInfo schemaInfo) => 
            ((((schemaInfo != null) && (schemaInfo.Validity == (this.flags & 3))) && ((schemaInfo.IsDefault == ((this.flags & 4) != 0)) && (schemaInfo.IsNil == ((this.flags & 8) != 0)))) && (((schemaInfo.MemberType == this.memberType) && (schemaInfo.SchemaType == this.schemaType)) && ((schemaInfo.SchemaElement == (this.decl as XmlSchemaElement)) && (schemaInfo.SchemaAttribute == (this.decl as XmlSchemaAttribute)))));

        public void SetIsDefault(bool value)
        {
            if (value)
            {
                this.flags = (byte) (this.flags | 4);
            }
            else
            {
                this.flags = (byte) (this.flags & -5);
            }
        }

        public void SetIsNil(bool value)
        {
            if (value)
            {
                this.flags = (byte) (this.flags | 8);
            }
            else
            {
                this.flags = (byte) (this.flags & -9);
            }
        }

        public void SetValidity(XmlSchemaValidity value)
        {
            this.flags = (byte) ((this.flags & -4) | ((byte) value));
        }

        public override bool IsDefault =>
            ((this.flags & 4) != 0);

        public override bool IsNil =>
            ((this.flags & 8) != 0);

        public override XmlSchemaSimpleType MemberType =>
            this.memberType;

        public override XmlSchemaAttribute SchemaAttribute =>
            (this.decl as XmlSchemaAttribute);

        public override XmlSchemaElement SchemaElement =>
            (this.decl as XmlSchemaElement);

        public override XmlSchemaType SchemaType =>
            this.schemaType;

        public override XmlSchemaValidity Validity
        {
            get
            {
                if (!base.ownerDoc.CanReportValidity)
                {
                    return XmlSchemaValidity.NotKnown;
                }
                return (((XmlSchemaValidity) this.flags) & (XmlSchemaValidity.Invalid | XmlSchemaValidity.Valid));
            }
        }
    }
}


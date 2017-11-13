namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class ByteFacetDescriptionElement : FacetDescriptionElement
    {
        public ByteFacetDescriptionElement(TypeElement type, string name) : base(type, name)
        {
        }

        protected override void HandleDefaultAttribute(XmlReader reader)
        {
            byte field = 0;
            if (base.HandleByteAttribute(reader, ref field))
            {
                base.DefaultValue = field;
            }
        }

        public override EdmType FacetType =>
            MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte);
    }
}


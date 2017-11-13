namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class IntegerFacetDescriptionElement : FacetDescriptionElement
    {
        public IntegerFacetDescriptionElement(TypeElement type, string name) : base(type, name)
        {
        }

        protected override void HandleDefaultAttribute(XmlReader reader)
        {
            int field = -1;
            if (base.HandleIntAttribute(reader, ref field))
            {
                base.DefaultValue = field;
            }
        }

        public override EdmType FacetType =>
            MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Int32);
    }
}


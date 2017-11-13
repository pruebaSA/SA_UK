namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class BooleanFacetDescriptionElement : FacetDescriptionElement
    {
        public BooleanFacetDescriptionElement(TypeElement type, string name) : base(type, name)
        {
        }

        protected override void HandleDefaultAttribute(XmlReader reader)
        {
            bool field = false;
            if (base.HandleBoolAttribute(reader, ref field))
            {
                base.DefaultValue = field;
            }
        }

        public override EdmType FacetType =>
            MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean);
    }
}


namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;

    internal sealed class ComplexTypeEmitter : StructuredTypeEmitter
    {
        public ComplexTypeEmitter(ClientApiGenerator generator, ComplexType complexType) : base(generator, complexType)
        {
        }

        protected override void EmitTypeAttributes(CodeTypeDeclaration typeDecl)
        {
            base.Generator.AttributeEmitter.EmitTypeAttributes(this, typeDecl);
            base.EmitTypeAttributes(typeDecl);
        }

        protected override CodeTypeReference GetBaseType() => 
            base.GetBaseType();

        protected override ReadOnlyMetadataCollection<EdmProperty> GetProperties() => 
            this.Item.Properties;

        internal ComplexType Item =>
            (base.Item as ComplexType);
    }
}


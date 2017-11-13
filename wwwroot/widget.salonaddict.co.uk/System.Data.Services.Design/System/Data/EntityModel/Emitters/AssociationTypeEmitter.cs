namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;

    internal sealed class AssociationTypeEmitter : SchemaTypeEmitter
    {
        public AssociationTypeEmitter(ClientApiGenerator generator, AssociationType associationType) : base(generator, associationType)
        {
        }

        public override CodeTypeDeclarationCollection EmitApiClass() => 
            new CodeTypeDeclarationCollection();
    }
}


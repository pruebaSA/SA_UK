namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;

    internal sealed class EntityTypeEmitter : StructuredTypeEmitter
    {
        public EntityTypeEmitter(ClientApiGenerator generator, EntityType entity) : base(generator, entity)
        {
        }

        public override CodeTypeDeclarationCollection EmitApiClass()
        {
            CodeTypeDeclarationCollection declarations = base.EmitApiClass();
            if ((this.Item.KeyMembers.Count > 0) && (declarations.Count == 1))
            {
                CodeTypeDeclaration declaration = declarations[0];
                declaration.Comments.Add(new CodeCommentStatement("<KeyProperties>", true));
                foreach (EdmMember member in this.Item.KeyMembers)
                {
                    string name = member.Name;
                    declaration.Comments.Add(new CodeCommentStatement(name, true));
                }
                declaration.Comments.Add(new CodeCommentStatement("</KeyProperties>", true));
            }
            return declarations;
        }

        protected override void EmitProperties(CodeTypeDeclaration typeDecl)
        {
            base.EmitProperties(typeDecl);
            foreach (EdmMember member in this.Item.Members)
            {
                NavigationProperty navigationProperty = member as NavigationProperty;
                if ((navigationProperty != null) && (navigationProperty.DeclaringType == this.Item))
                {
                    new NavigationPropertyEmitter(base.Generator, navigationProperty, base.UsingStandardBaseClass).Emit(typeDecl);
                }
            }
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

        public EntityType Item =>
            (base.Item as EntityType);
    }
}


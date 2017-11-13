namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Design;

    internal abstract class PropertyEmitterBase : MetadataItemEmitter
    {
        private bool _declaringTypeUsesStandardBaseType;

        protected PropertyEmitterBase(ClientApiGenerator generator, MetadataItem item, bool declaringTypeUsesStandardBaseType) : base(generator, item)
        {
            this._declaringTypeUsesStandardBaseType = declaringTypeUsesStandardBaseType;
        }

        protected bool AncestorClassDefinesName(string name)
        {
            if (this._declaringTypeUsesStandardBaseType && Utils.DoesTypeReserveMemberName(this.Item.DeclaringType, name, base.Generator.LanguageAppropriateStringComparer))
            {
                return true;
            }
            StructuralType baseType = this.Item.DeclaringType.BaseType as StructuralType;
            return ((baseType != null) && baseType.Members.Contains(name));
        }

        public void Emit(CodeTypeDeclaration typeDecl)
        {
            this.Validate();
            this.EmitProperty(typeDecl);
        }

        protected abstract void EmitProperty(CodeTypeDeclaration typeDecl);
        protected override void Validate()
        {
            this.VerifyGetterAndSetterAccessibilityCompatability();
            base.Generator.VerifyLanguageCaseSensitiveCompatibilityForProperty(this.Item);
        }

        private void VerifyGetterAndSetterAccessibilityCompatability()
        {
            if ((MetadataItemEmitter.GetGetterAccessibility(this.Item) == MemberAttributes.Assembly) && (MetadataItemEmitter.GetSetterAccessibility(this.Item) == MemberAttributes.Family))
            {
                base.Generator.AddError(Strings.GeneratedPropertyAccessibilityConflict(this.Item.Name, "Internal", "Protected"), ModelBuilderErrorCode.GeneratedPropertyAccessibilityConflict, EdmSchemaErrorSeverity.Error);
            }
            else if ((MetadataItemEmitter.GetGetterAccessibility(this.Item) == MemberAttributes.Family) && (MetadataItemEmitter.GetSetterAccessibility(this.Item) == MemberAttributes.Assembly))
            {
                base.Generator.AddError(Strings.GeneratedPropertyAccessibilityConflict(this.Item.Name, "Protected", "Internal"), ModelBuilderErrorCode.GeneratedPropertyAccessibilityConflict, EdmSchemaErrorSeverity.Error);
            }
        }

        public EdmMember Item =>
            (base.Item as EdmMember);
    }
}


namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Design;
    using System.Reflection;

    internal abstract class SchemaTypeEmitter : MetadataItemEmitter
    {
        protected SchemaTypeEmitter(ClientApiGenerator generator, MetadataItem item) : base(generator, item)
        {
        }

        protected void AddInterfaces(string itemName, CodeTypeDeclaration typeDecl, List<Type> additionalInterfaces)
        {
            if (additionalInterfaces != null)
            {
                try
                {
                    foreach (Type type in additionalInterfaces)
                    {
                        typeDecl.BaseTypes.Add(new CodeTypeReference(type, CodeTypeReferenceOptions.GlobalReference));
                    }
                }
                catch (ArgumentNullException exception)
                {
                    base.Generator.AddError(Strings.InvalidInterfaceSuppliedForType(itemName), ModelBuilderErrorCode.InvalidInterfaceSuppliedForType, EdmSchemaErrorSeverity.Error, exception);
                }
            }
        }

        protected void AddMembers(string itemName, CodeTypeDeclaration typeDecl, List<CodeTypeMember> additionalMembers)
        {
            if ((additionalMembers != null) && (additionalMembers.Count > 0))
            {
                try
                {
                    typeDecl.Members.AddRange(additionalMembers.ToArray());
                }
                catch (ArgumentNullException exception)
                {
                    base.Generator.AddError(Strings.InvalidMemberSuppliedForType(itemName), ModelBuilderErrorCode.InvalidMemberSuppliedForType, EdmSchemaErrorSeverity.Error, exception);
                }
            }
        }

        public abstract CodeTypeDeclarationCollection EmitApiClass();
        protected virtual void EmitTypeAttributes(CodeTypeDeclaration typeDecl)
        {
            base.Generator.AttributeEmitter.EmitTypeAttributes(this, typeDecl);
        }

        protected void EmitTypeAttributes(string itemName, CodeTypeDeclaration typeDecl, List<CodeAttributeDeclaration> additionalAttributes)
        {
            if ((additionalAttributes != null) && (additionalAttributes.Count > 0))
            {
                try
                {
                    typeDecl.CustomAttributes.AddRange(additionalAttributes.ToArray());
                }
                catch (ArgumentNullException exception)
                {
                    base.Generator.AddError(Strings.InvalidAttributeSuppliedForType(itemName), ModelBuilderErrorCode.InvalidAttributeSuppliedForType, EdmSchemaErrorSeverity.Error, exception);
                }
            }
            this.EmitTypeAttributes(typeDecl);
        }

        internal void SetTypeVisibility(CodeTypeDeclaration typeDecl)
        {
            typeDecl.TypeAttributes &= ~TypeAttributes.NestedFamORAssem;
            typeDecl.TypeAttributes |= MetadataItemEmitter.GetTypeAccessibilityValue(this.Item);
        }

        protected override void Validate()
        {
            base.Generator.VerifyLanguageCaseSensitiveCompatibilityForType(this.Item);
        }

        internal GlobalItem Item =>
            (base.Item as GlobalItem);
    }
}


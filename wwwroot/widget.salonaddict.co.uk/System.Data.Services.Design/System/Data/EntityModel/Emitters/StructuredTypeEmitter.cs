namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Design;
    using System.Data.Services.Design.Common;
    using System.Reflection;

    internal abstract class StructuredTypeEmitter : SchemaTypeEmitter
    {
        private bool _usingStandardBaseClass;

        protected StructuredTypeEmitter(ClientApiGenerator generator, StructuralType structuralType) : base(generator, structuralType)
        {
            this._usingStandardBaseClass = true;
        }

        private void AssignBaseType(CodeTypeDeclaration typeDecl, CodeTypeReference baseType, CodeTypeReference eventReturnedBaseType)
        {
            if ((eventReturnedBaseType != null) && !eventReturnedBaseType.Equals(baseType))
            {
                this._usingStandardBaseClass = false;
                typeDecl.BaseTypes.Add(eventReturnedBaseType);
            }
            else if (baseType != null)
            {
                typeDecl.BaseTypes.Add(baseType);
            }
        }

        public override CodeTypeDeclarationCollection EmitApiClass()
        {
            this.Validate();
            CodeTypeReference baseType = this.GetBaseType();
            TypeGeneratedEventArgs eventArgs = new TypeGeneratedEventArgs(this.Item, baseType);
            base.Generator.RaiseTypeGeneratedEvent(eventArgs);
            CodeTypeDeclaration typeDecl = new CodeTypeDeclaration(this.Item.Name) {
                IsPartial = true,
                TypeAttributes = TypeAttributes.AnsiClass
            };
            if (this.Item.Abstract)
            {
                typeDecl.TypeAttributes |= TypeAttributes.Abstract;
            }
            base.SetTypeVisibility(typeDecl);
            base.EmitTypeAttributes(this.Item.Name, typeDecl, eventArgs.AdditionalAttributes);
            this.AssignBaseType(typeDecl, baseType, eventArgs.BaseType);
            base.AddInterfaces(this.Item.Name, typeDecl, eventArgs.AdditionalInterfaces);
            CommentEmitter.EmitSummaryComments(this.Item, typeDecl.Comments);
            if ((typeDecl.TypeAttributes & TypeAttributes.Abstract) == TypeAttributes.AnsiClass)
            {
                this.EmitFactoryMethod(typeDecl);
            }
            this.EmitProperties(typeDecl);
            base.AddMembers(this.Item.Name, typeDecl, eventArgs.AdditionalMembers);
            CodeTypeDeclarationCollection declarations = new CodeTypeDeclarationCollection();
            declarations.Add(typeDecl);
            return declarations;
        }

        protected virtual void EmitFactoryMethod(CodeTypeDeclaration typeDecl)
        {
            ReadOnlyMetadataCollection<EdmProperty> properties = this.GetProperties();
            List<EdmProperty> list = new List<EdmProperty>(properties.Count);
            foreach (EdmProperty property in properties)
            {
                if (this.IncludeFieldInFactoryMethod(property))
                {
                    list.Add(property);
                }
            }
            if (list.Count >= 1)
            {
                CodeMemberMethod ctm = new CodeMemberMethod();
                CodeTypeReference type = base.TypeReference.FromString(this.Item.Name);
                UniqueIdentifierService service = new UniqueIdentifierService(base.Generator.IsLanguageCaseSensitive);
                string name = service.AdjustIdentifier(Utils.CamelCase(this.Item.Name));
                ctm.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                ctm.Name = "Create" + this.Item.Name;
                if (NavigationPropertyEmitter.IsNameAlreadyAMemberName(this.Item, ctm.Name, base.Generator.LanguageAppropriateStringComparer))
                {
                    base.Generator.AddError(Strings.GeneratedFactoryMethodNameConflict(ctm.Name, this.Item.Name), ModelBuilderErrorCode.GeneratedFactoryMethodNameConflict, EdmSchemaErrorSeverity.Error);
                }
                ctm.ReturnType = type;
                AttributeEmitter.AddGeneratedCodeAttribute(ctm);
                CommentEmitter.EmitSummaryComments(Strings.FactoryMethodSummaryComment(this.Item.Name), ctm.Comments);
                CodeVariableDeclarationStatement statement = new CodeVariableDeclarationStatement(type, name, new CodeObjectCreateExpression(type, new CodeExpression[0]));
                ctm.Statements.Add(statement);
                CodeVariableReferenceExpression targetObject = new CodeVariableReferenceExpression(name);
                foreach (EdmProperty property2 in list)
                {
                    CodeExpression expression4;
                    PropertyEmitter emitter = new PropertyEmitter(base.Generator, property2, this.UsingStandardBaseClass);
                    CodeTypeReference propertyType = emitter.PropertyType;
                    string str2 = Utils.SetSpecialCaseForFxCopOnPropertyName(service.AdjustIdentifier(Utils.FixParameterName(emitter.PropertyName, "argument")));
                    CodeParameterDeclarationExpression expression2 = new CodeParameterDeclarationExpression(propertyType, str2);
                    CodeArgumentReferenceExpression expression = new CodeArgumentReferenceExpression(expression2.Name);
                    ctm.Parameters.Add(expression2);
                    CommentEmitter.EmitParamComments(expression2, Strings.FactoryParamCommentGeneral(emitter.PropertyName), ctm.Comments);
                    if (System.Data.Metadata.Edm.TypeSemantics.IsComplexType(emitter.Item.TypeUsage))
                    {
                        List<CodeExpression> list2 = new List<CodeExpression> {
                            expression,
                            new CodePrimitiveExpression(emitter.PropertyName)
                        };
                        ctm.Statements.Add(new CodeConditionStatement(Emitter.EmitExpressionEqualsNull(expression), new CodeStatement[] { new CodeThrowExceptionStatement(new CodeObjectCreateExpression(base.TypeReference.ForType(typeof(ArgumentNullException)), new CodeExpression[] { new CodePrimitiveExpression(str2) })) }));
                        expression4 = expression;
                    }
                    else
                    {
                        expression4 = expression;
                    }
                    ctm.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(targetObject, emitter.PropertyName), expression4));
                }
                ctm.Statements.Add(new CodeMethodReturnStatement(targetObject));
                typeDecl.Members.Add(ctm);
            }
        }

        protected virtual void EmitProperties(CodeTypeDeclaration typeDecl)
        {
            foreach (EdmMember member in this.Item.Members)
            {
                EdmProperty property = member as EdmProperty;
                if ((property != null) && (property.DeclaringType == this.Item))
                {
                    new PropertyEmitter(base.Generator, property, this._usingStandardBaseClass).Emit(typeDecl);
                }
            }
        }

        protected override void EmitTypeAttributes(CodeTypeDeclaration typeDecl)
        {
            base.Generator.AttributeEmitter.EmitTypeAttributes(this, typeDecl);
            base.EmitTypeAttributes(typeDecl);
        }

        protected virtual CodeTypeReference GetBaseType()
        {
            if (this.Item.BaseType == null)
            {
                return null;
            }
            return base.Generator.GetLeastPossibleQualifiedTypeReference(this.Item.BaseType);
        }

        protected abstract ReadOnlyMetadataCollection<EdmProperty> GetProperties();
        private bool IncludeFieldInFactoryMethod(EdmProperty property)
        {
            if (property.Nullable)
            {
                return false;
            }
            if (PropertyEmitter.HasDefault(property))
            {
                return false;
            }
            if ((MetadataItemEmitter.GetGetterAccessibility(property) != MemberAttributes.Public) && (MetadataItemEmitter.GetSetterAccessibility(property) != MemberAttributes.Public))
            {
                return false;
            }
            return true;
        }

        internal StructuralType Item =>
            (base.Item as StructuralType);

        protected bool UsingStandardBaseClass =>
            this._usingStandardBaseClass;
    }
}


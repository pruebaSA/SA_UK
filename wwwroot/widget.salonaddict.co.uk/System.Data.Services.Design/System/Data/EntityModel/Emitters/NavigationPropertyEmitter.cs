namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Client;
    using System.Data.Services.Design;

    internal sealed class NavigationPropertyEmitter : PropertyEmitterBase
    {
        private const string ValuePropertyName = "Value";

        public NavigationPropertyEmitter(ClientApiGenerator generator, NavigationProperty navigationProperty, bool declaringTypeUsesStandardBaseType) : base(generator, navigationProperty, declaringTypeUsesStandardBaseType)
        {
        }

        private void EmitField(CodeTypeDeclaration typeDecl, CodeTypeReference fieldType, bool hasDefault)
        {
            CodeMemberField ctm = new CodeMemberField(fieldType, Utils.FieldNameFromPropName(this.Item.Name)) {
                Attributes = MemberAttributes.Private
            };
            AttributeEmitter.AddGeneratedCodeAttribute(ctm);
            if (hasDefault)
            {
                if (base.Generator.UseDataServiceCollection)
                {
                    ctm.InitExpression = new CodeObjectCreateExpression(fieldType, new CodeExpression[] { new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(TrackingMode)), "None") });
                }
                else
                {
                    ctm.InitExpression = new CodeObjectCreateExpression(fieldType, new CodeExpression[0]);
                }
            }
            typeDecl.Members.Add(ctm);
        }

        private CodeExpression EmitGetMethod(RelationshipEndMember target) => 
            new CodeFieldReferenceExpression(Emitter.ThisRef, Utils.FieldNameFromPropName(this.Item.Name));

        private void EmitNavigationProperty(CodeTypeDeclaration typeDecl)
        {
            CodeMemberProperty property = this.EmitNavigationProperty(this.Item.ToEndMember);
            typeDecl.Members.Add(property);
            this.EmitField(typeDecl, this.GetReturnType(this.Item.ToEndMember), this.Item.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many);
        }

        private CodeMemberProperty EmitNavigationProperty(RelationshipEndMember target)
        {
            CodeExpression expression2;
            CodeTypeReference returnType = this.GetReturnType(target);
            PropertyGeneratedEventArgs eventArgs = new PropertyGeneratedEventArgs(this.Item, null, returnType);
            base.Generator.RaisePropertyGeneratedEvent(eventArgs);
            CodeMemberProperty propertyDecl = new CodeMemberProperty();
            base.AttributeEmitter.AddIgnoreAttributes(propertyDecl);
            base.AttributeEmitter.AddBrowsableAttribute(propertyDecl);
            AttributeEmitter.AddGeneratedCodeAttribute(propertyDecl);
            CommentEmitter.EmitSummaryComments(this.Item, propertyDecl.Comments);
            propertyDecl.Name = this.Item.Name;
            if ((eventArgs.ReturnType != null) && !eventArgs.ReturnType.Equals(returnType))
            {
                propertyDecl.Type = eventArgs.ReturnType;
            }
            else
            {
                propertyDecl.Type = returnType;
            }
            propertyDecl.Attributes = MemberAttributes.Final;
            CodeExpression left = this.EmitGetMethod(target);
            if (target.RelationshipMultiplicity != RelationshipMultiplicity.Many)
            {
                propertyDecl.Attributes |= MetadataItemEmitter.AccessibilityFromGettersAndSetters(this.Item);
                List<CodeStatement> additionalSetStatements = eventArgs.AdditionalSetStatements;
                if ((additionalSetStatements != null) && (additionalSetStatements.Count > 0))
                {
                    try
                    {
                        propertyDecl.SetStatements.AddRange(additionalSetStatements.ToArray());
                    }
                    catch (ArgumentNullException exception)
                    {
                        base.Generator.AddError(System.Data.Services.Design.Strings.InvalidSetStatementSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidSetStatementSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception);
                    }
                }
                CodeExpression expression = new CodePropertySetValueReferenceExpression();
                if (returnType != eventArgs.ReturnType)
                {
                    expression = new CodeCastExpression(returnType, expression);
                }
                CodeExpression expression4 = left;
                expression2 = expression4;
                propertyDecl.SetStatements.Add(new CodeAssignStatement(expression4, expression));
                MemberAttributes propertyAccessibility = propertyDecl.Attributes & MemberAttributes.AccessMask;
                PropertyEmitter.AddGetterSetterFixUp(base.Generator.FixUps, this.GetFullyQualifiedPropertyName(propertyDecl.Name), MetadataItemEmitter.GetGetterAccessibility(this.Item), propertyAccessibility, true);
                PropertyEmitter.AddGetterSetterFixUp(base.Generator.FixUps, this.GetFullyQualifiedPropertyName(propertyDecl.Name), MetadataItemEmitter.GetSetterAccessibility(this.Item), propertyAccessibility, false);
                List<CodeStatement> additionalAfterSetStatements = eventArgs.AdditionalAfterSetStatements;
                if ((additionalAfterSetStatements != null) && (additionalAfterSetStatements.Count > 0))
                {
                    try
                    {
                        propertyDecl.SetStatements.AddRange(additionalAfterSetStatements.ToArray());
                    }
                    catch (ArgumentNullException exception2)
                    {
                        base.Generator.AddError(System.Data.Services.Design.Strings.InvalidSetStatementSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidSetStatementSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception2);
                    }
                }
            }
            else
            {
                propertyDecl.Attributes |= MetadataItemEmitter.GetGetterAccessibility(this.Item);
                expression2 = left;
                CodeExpression expression5 = new CodePropertySetValueReferenceExpression();
                CodeStatementCollection setStatements = null;
                if (base.Generator.UseDataServiceCollection)
                {
                    setStatements = propertyDecl.SetStatements;
                }
                else
                {
                    CodeConditionStatement statement = new CodeConditionStatement(Emitter.EmitExpressionDoesNotEqualNull(expression5), new CodeStatement[0]);
                    propertyDecl.SetStatements.Add(statement);
                    setStatements = statement.TrueStatements;
                }
                setStatements.Add(new CodeAssignStatement(left, expression5));
                if (eventArgs.AdditionalAfterSetStatements != null)
                {
                    try
                    {
                        foreach (CodeStatement statement2 in eventArgs.AdditionalAfterSetStatements)
                        {
                            setStatements.Add(statement2);
                        }
                    }
                    catch (ArgumentNullException exception3)
                    {
                        base.Generator.AddError(System.Data.Services.Design.Strings.InvalidSetStatementSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidSetStatementSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception3);
                    }
                }
            }
            List<CodeStatement> additionalGetStatements = eventArgs.AdditionalGetStatements;
            if ((additionalGetStatements != null) && (additionalGetStatements.Count > 0))
            {
                try
                {
                    propertyDecl.GetStatements.AddRange(additionalGetStatements.ToArray());
                }
                catch (ArgumentNullException exception4)
                {
                    base.Generator.AddError(System.Data.Services.Design.Strings.InvalidGetStatementSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidGetStatementSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception4);
                }
            }
            propertyDecl.GetStatements.Add(new CodeMethodReturnStatement(expression2));
            return propertyDecl;
        }

        protected override void EmitProperty(CodeTypeDeclaration typeDecl)
        {
            this.EmitNavigationProperty(typeDecl);
        }

        private static EntityTypeBase GetEntityType(RelationshipEndMember endMember) => 
            ((RefType) endMember.TypeUsage.EdmType).ElementType;

        private string GetFullyQualifiedPropertyName(string propertyName) => 
            (this.Item.DeclaringType.FullName + "." + propertyName);

        private CodeTypeReference GetReturnType(RelationshipEndMember target)
        {
            CodeTypeReference leastPossibleQualifiedTypeReference = base.Generator.GetLeastPossibleQualifiedTypeReference(GetEntityType(target));
            if (target.RelationshipMultiplicity == RelationshipMultiplicity.Many)
            {
                leastPossibleQualifiedTypeReference = base.TypeReference.FrameworkGenericClass(base.Generator.GetRelationshipMultiplicityManyCollectionTypeName(), leastPossibleQualifiedTypeReference);
            }
            return leastPossibleQualifiedTypeReference;
        }

        internal static bool IsNameAlreadyAMemberName(StructuralType type, string generatedPropertyName, StringComparison comparison)
        {
            foreach (EdmMember member in type.Members)
            {
                if ((member.DeclaringType == type) && member.Name.Equals(generatedPropertyName, comparison))
                {
                    return true;
                }
            }
            return false;
        }

        private NavigationProperty Item =>
            (base.Item as NavigationProperty);
    }
}


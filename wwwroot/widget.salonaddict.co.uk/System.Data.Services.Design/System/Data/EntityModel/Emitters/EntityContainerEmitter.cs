namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Design;
    using System.Data.Services.Design.Common;
    using System.Linq;

    internal sealed class EntityContainerEmitter : SchemaTypeEmitter
    {
        private const string _onContextCreatedString = "OnContextCreated";

        public EntityContainerEmitter(ClientApiGenerator generator, EntityContainer entityContainer) : base(generator, entityContainer)
        {
        }

        private void AddRootNamespaceField(CodeTypeDeclaration typeDecl)
        {
            EntityContainer item = this.Item;
            string clientTypeNamespace = base.Generator.GetClientTypeNamespace(base.Generator.GetContainerNamespace(item));
            string type = (string.IsNullOrEmpty(clientTypeNamespace) || item.Name.Equals(clientTypeNamespace, StringComparison.OrdinalIgnoreCase)) ? item.Name : (clientTypeNamespace + "." + item.Name);
            CodeExpression targetObject = new CodePropertyReferenceExpression(new CodeTypeOfExpression(type), "Namespace");
            CodeMemberField ctm = new CodeMemberField(base.TypeReference.ForType(typeof(string)), "ROOTNAMESPACE") {
                Attributes = MemberAttributes.Private | MemberAttributes.Static,
                InitExpression = new CodeMethodInvokeExpression(targetObject, "Remove", new CodeExpression[] { new CodeMethodInvokeExpression(targetObject, "LastIndexOf", new CodeExpression[] { new CodePrimitiveExpression(clientTypeNamespace) }) })
            };
            AttributeEmitter.AddGeneratedCodeAttribute(ctm);
            typeDecl.Members.Add(ctm);
        }

        private bool AreTypeAndSetAccessCompatible(MemberAttributes typeAccess, MemberAttributes setAccess) => 
            ((typeAccess != MemberAttributes.Assembly) || ((setAccess != MemberAttributes.Public) && (setAccess != MemberAttributes.Family)));

        private void CreateConstructors(CodeTypeDeclaration typeDecl, bool setupTypeMapper, bool hasInheritance)
        {
            CodeConstructor ctm = new CodeConstructor {
                Attributes = MemberAttributes.Public
            };
            CodeParameterDeclarationExpression expression = new CodeParameterDeclarationExpression(base.TypeReference.FromString("System.Uri", true), "serviceRoot");
            ctm.Parameters.Add(expression);
            ctm.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(expression.Name));
            AttributeEmitter.AddGeneratedCodeAttribute(ctm);
            CommentEmitter.EmitSummaryComments(System.Data.Services.Design.Strings.CtorSummaryComment(this.Item.Name), ctm.Comments);
            if (setupTypeMapper || hasInheritance)
            {
                ctm.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(Emitter.ThisRef, "ResolveName"), new CodeDelegateCreateExpression(base.TypeReference.ForType(typeof(Func<,>), new CodeTypeReference[] { base.TypeReference.ForType(typeof(Type)), base.TypeReference.ForType(typeof(string)) }), Emitter.ThisRef, "ResolveNameFromType")));
            }
            if (setupTypeMapper)
            {
                ctm.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(Emitter.ThisRef, "ResolveType"), new CodeDelegateCreateExpression(base.TypeReference.ForType(typeof(Func<,>), new CodeTypeReference[] { base.TypeReference.ForType(typeof(string)), base.TypeReference.ForType(typeof(Type)) }), Emitter.ThisRef, "ResolveTypeFromName")));
            }
            ctm.Statements.Add(this.OnContextCreatedCodeMethodInvokeExpression());
            typeDecl.Members.Add(ctm);
        }

        private void CreateContextPartialMethods(CodeTypeDeclaration typeDecl)
        {
            CodeMemberMethod method = new CodeMemberMethod {
                Name = "OnContextCreated",
                ReturnType = new CodeTypeReference(typeof(void)),
                Attributes = MemberAttributes.Public | MemberAttributes.Abstract
            };
            typeDecl.Members.Add(method);
            base.Generator.FixUps.Add(new FixUp(this.Item.Name + ".OnContextCreated", FixUpType.MarkAbstractMethodAsPartial));
        }

        private CodeMemberMethod CreateEntitySetAddObjectMethod(EntitySet set)
        {
            CodeMemberMethod ctm = new CodeMemberMethod {
                Attributes = MemberAttributes.Final | MetadataItemEmitter.GetEntityTypeAccessibility(set.ElementType),
                Name = "AddTo" + set.Name
            };
            CodeParameterDeclarationExpression expression = new CodeParameterDeclarationExpression {
                Type = base.Generator.GetLeastPossibleQualifiedTypeReference(set.ElementType),
                Name = Utils.CamelCase(set.ElementType.Name)
            };
            expression.Name = Utils.SetSpecialCaseForFxCopOnPropertyName(expression.Name);
            ctm.Parameters.Add(expression);
            ctm.ReturnType = new CodeTypeReference(typeof(void));
            AttributeEmitter.AddGeneratedCodeAttribute(ctm);
            ctm.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "AddObject", new CodeExpression[] { new CodePrimitiveExpression(set.Name), new CodeFieldReferenceExpression(null, expression.Name) }));
            CommentEmitter.EmitSummaryComments(set, ctm.Comments);
            return ctm;
        }

        private CodeMemberField CreateEntitySetField(EntitySet set)
        {
            CodeMemberField ctm = new CodeMemberField {
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                Name = Utils.FieldNameFromPropName(set.Name)
            };
            AttributeEmitter.AddGeneratedCodeAttribute(ctm);
            CodeTypeReference leastPossibleQualifiedTypeReference = base.Generator.GetLeastPossibleQualifiedTypeReference(set.ElementType);
            ctm.Type = base.TypeReference.AdoFrameworkGenericClass("DataServiceQuery", leastPossibleQualifiedTypeReference);
            return ctm;
        }

        private CodeMemberProperty CreateEntitySetProperty(EntitySet set)
        {
            CodeMemberProperty propertyDecl = new CodeMemberProperty {
                Attributes = MemberAttributes.Final | MetadataItemEmitter.GetEntitySetPropertyAccessibility(set),
                Name = set.Name,
                HasGet = true,
                HasSet = false
            };
            if (TypeReference.ObjectContextBaseClassType.GetProperty(set.Name) != null)
            {
                propertyDecl.Attributes |= MemberAttributes.New;
            }
            base.AttributeEmitter.AddBrowsableAttribute(propertyDecl);
            AttributeEmitter.AddGeneratedCodeAttribute(propertyDecl);
            CodeTypeReference leastPossibleQualifiedTypeReference = base.Generator.GetLeastPossibleQualifiedTypeReference(set.ElementType);
            propertyDecl.Type = base.TypeReference.AdoFrameworkGenericClass("DataServiceQuery", leastPossibleQualifiedTypeReference);
            string backingFieldName = Utils.FieldNameFromPropName(set.Name);
            PropertyGeneratedEventArgs eventArgs = new PropertyGeneratedEventArgs(set, backingFieldName, propertyDecl.Type);
            base.Generator.RaisePropertyGeneratedEvent(eventArgs);
            if ((eventArgs.ReturnType == null) || !eventArgs.ReturnType.Equals(propertyDecl.Type))
            {
                throw EDesignUtil.InvalidOperation(System.Data.Services.Design.Strings.CannotChangePropertyReturnType(set.Name, this.Item.Name));
            }
            List<CodeAttributeDeclaration> additionalAttributes = eventArgs.AdditionalAttributes;
            if ((additionalAttributes != null) && (additionalAttributes.Count > 0))
            {
                try
                {
                    propertyDecl.CustomAttributes.AddRange(additionalAttributes.ToArray());
                }
                catch (ArgumentNullException exception)
                {
                    base.Generator.AddError(System.Data.Services.Design.Strings.InvalidAttributeSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidAttributeSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception);
                }
            }
            List<CodeStatement> additionalGetStatements = eventArgs.AdditionalGetStatements;
            if ((additionalGetStatements != null) && (additionalGetStatements.Count > 0))
            {
                try
                {
                    propertyDecl.GetStatements.AddRange(additionalGetStatements.ToArray());
                }
                catch (ArgumentNullException exception2)
                {
                    base.Generator.AddError(System.Data.Services.Design.Strings.InvalidGetStatementSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidGetStatementSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception2);
                }
            }
            propertyDecl.GetStatements.Add(new CodeConditionStatement(Emitter.EmitExpressionEqualsNull(new CodeFieldReferenceExpression(Emitter.ThisRef, backingFieldName)), new CodeStatement[] { new CodeAssignStatement(new CodeFieldReferenceExpression(Emitter.ThisRef, backingFieldName), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeBaseReferenceExpression(), "CreateQuery", new CodeTypeReference[] { leastPossibleQualifiedTypeReference }), new CodeExpression[] { new CodePrimitiveExpression(set.Name) })) }));
            propertyDecl.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(Emitter.ThisRef, backingFieldName)));
            CommentEmitter.EmitSummaryComments(set, propertyDecl.Comments);
            return propertyDecl;
        }

        private void CreateTypeMappingMethods(CodeTypeDeclaration typeDecl, bool needTypeMapper, bool hasInheritance)
        {
            if (base.Generator.Language == LanguageOption.GenerateVBCode)
            {
                this.AddRootNamespaceField(typeDecl);
            }
            CodeExpression expression = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(base.TypeReference.ForType(typeof(StringComparison))), Enum.GetName(typeof(StringComparison), base.Generator.LanguageAppropriateStringComparer));
            if (needTypeMapper)
            {
                CodeMemberMethod method = new CodeMemberMethod {
                    Name = "ResolveTypeFromName",
                    Attributes = MemberAttributes.Family | MemberAttributes.Final
                };
                method.Parameters.Add(new CodeParameterDeclarationExpression(base.TypeReference.ForType(typeof(string)), "typeName"));
                method.ReturnType = base.TypeReference.ForType(typeof(Type));
                AttributeEmitter.AddGeneratedCodeAttribute(method);
                CommentEmitter.EmitSummaryComments(System.Data.Services.Design.Strings.TypeMapperDescription, method.Comments);
                foreach (KeyValuePair<string, string> pair in from p in base.Generator.NamespaceMap
                    orderby p.Key.Length descending, p.Key
                    select p)
                {
                    method.Statements.Add(new CodeConditionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("typeName"), "StartsWith", new CodeExpression[] { new CodePrimitiveExpression(pair.Key), expression }), new CodeStatement[] { new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeMethodInvokeExpression(Emitter.ThisRef, "GetType", new CodeExpression[0]), "Assembly"), "GetType", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(base.TypeReference.ForType(typeof(string))), "Concat", new CodeExpression[] { this.LanguageSpecificNamespace(pair.Value), new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("typeName"), "Substring", new CodeExpression[] { new CodePrimitiveExpression(pair.Key.Length) }) }), new CodePrimitiveExpression(false) })) }));
                }
                method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
                typeDecl.Members.Add(method);
            }
            CodeMemberMethod ctm = new CodeMemberMethod {
                Name = "ResolveNameFromType",
                Attributes = MemberAttributes.Family | MemberAttributes.Final
            };
            ctm.Parameters.Add(new CodeParameterDeclarationExpression(base.TypeReference.ForType(typeof(Type)), "clientType"));
            ctm.ReturnType = base.TypeReference.ForType(typeof(string));
            AttributeEmitter.AddGeneratedCodeAttribute(ctm);
            CommentEmitter.EmitSummaryComments(System.Data.Services.Design.Strings.TypeMapperDescription, ctm.Comments);
            foreach (KeyValuePair<string, string> pair2 in from p in base.Generator.NamespaceMap
                orderby p.Value.Length descending, p.Key
                select p)
            {
                ctm.Statements.Add(new CodeConditionStatement(new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("clientType"), "Namespace"), "Equals", new CodeExpression[] { this.LanguageSpecificNamespace(pair2.Value), expression }), new CodeStatement[] { new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(base.TypeReference.ForType(typeof(string))), "Concat", new CodeExpression[] { new CodePrimitiveExpression(pair2.Key + "."), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("clientType"), "Name") })) }));
            }
            if (hasInheritance)
            {
                CodeExpression targetObject = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("clientType"), "FullName");
                if (base.Generator.Language == LanguageOption.GenerateVBCode)
                {
                    ctm.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(targetObject, "Substring", new CodeExpression[] { new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("ROOTNAMESPACE"), "Length") })));
                }
                else
                {
                    ctm.Statements.Add(new CodeMethodReturnStatement(targetObject));
                }
            }
            else
            {
                ctm.Statements.Add(new CodeMethodReturnStatement(Emitter.NullExpression));
            }
            typeDecl.Members.Add(ctm);
        }

        public override CodeTypeDeclarationCollection EmitApiClass()
        {
            this.Validate();
            CodeTypeDeclaration typeDecl = new CodeTypeDeclaration(this.Item.Name) {
                IsPartial = true
            };
            CodeTypeReference objectContext = base.TypeReference.ObjectContext;
            TypeGeneratedEventArgs eventArgs = new TypeGeneratedEventArgs(this.Item, objectContext);
            base.Generator.RaiseTypeGeneratedEvent(eventArgs);
            if ((eventArgs.BaseType != null) && !eventArgs.BaseType.Equals(objectContext))
            {
                typeDecl.BaseTypes.Add(eventArgs.BaseType);
            }
            else
            {
                typeDecl.BaseTypes.Add(base.TypeReference.ObjectContext);
            }
            base.AddInterfaces(this.Item.Name, typeDecl, eventArgs.AdditionalInterfaces);
            CommentEmitter.EmitSummaryComments(this.Item, typeDecl.Comments);
            base.EmitTypeAttributes(this.Item.Name, typeDecl, eventArgs.AdditionalAttributes);
            bool setupTypeMapper = 0 < base.Generator.NamespaceMap.Count;
            IEnumerable<StructuralType> source = base.Generator.EdmItemCollection.GetItems<StructuralType>().Where<StructuralType>(delegate (StructuralType a) {
                if (a.BaseType == null)
                {
                    return false;
                }
                if (a.BuiltInTypeKind != BuiltInTypeKind.ComplexType)
                {
                    return a.BuiltInTypeKind == BuiltInTypeKind.EntityType;
                }
                return true;
            });
            bool hasInheritance = null != source.FirstOrDefault<StructuralType>();
            this.CreateConstructors(typeDecl, setupTypeMapper, hasInheritance);
            this.CreateContextPartialMethods(typeDecl);
            if (setupTypeMapper || hasInheritance)
            {
                this.CreateTypeMappingMethods(typeDecl, setupTypeMapper, hasInheritance);
            }
            foreach (EntitySetBase base2 in this.Item.BaseEntitySets)
            {
                if (System.Data.Metadata.Edm.Helper.IsEntitySet(base2))
                {
                    EntitySet set = (EntitySet) base2;
                    CodeMemberProperty property = this.CreateEntitySetProperty(set);
                    typeDecl.Members.Add(property);
                    CodeMemberField field = this.CreateEntitySetField(set);
                    typeDecl.Members.Add(field);
                }
            }
            foreach (EntitySetBase base3 in this.Item.BaseEntitySets)
            {
                if (System.Data.Metadata.Edm.Helper.IsEntitySet(base3))
                {
                    EntitySet set2 = (EntitySet) base3;
                    CodeMemberMethod method = this.CreateEntitySetAddObjectMethod(set2);
                    typeDecl.Members.Add(method);
                }
            }
            base.AddMembers(this.Item.Name, typeDecl, eventArgs.AdditionalMembers);
            CodeTypeDeclarationCollection declarations = new CodeTypeDeclarationCollection();
            declarations.Add(typeDecl);
            return declarations;
        }

        private CodeExpression LanguageSpecificNamespace(string ns)
        {
            if (base.Generator.Language == LanguageOption.GenerateVBCode)
            {
                return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(base.TypeReference.ForType(typeof(string))), "Concat", new CodeExpression[] { new CodeVariableReferenceExpression("ROOTNAMESPACE"), new CodePrimitiveExpression(ns) });
            }
            return new CodePrimitiveExpression(ns);
        }

        private CodeMethodInvokeExpression OnContextCreatedCodeMethodInvokeExpression() => 
            new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "OnContextCreated", new CodeExpression[0]);

        protected override void Validate()
        {
            base.Validate();
            base.Generator.VerifyLanguageCaseSensitiveCompatibilityForEntitySet(this.Item);
            this.VerifyEntityTypeAndSetAccessibilityCompatability();
        }

        private void VerifyEntityTypeAndSetAccessibilityCompatability()
        {
            foreach (EntitySetBase base2 in this.Item.BaseEntitySets)
            {
                if (System.Data.Metadata.Edm.Helper.IsEntitySet(base2))
                {
                    EntitySet item = (EntitySet) base2;
                    if (!this.AreTypeAndSetAccessCompatible(MetadataItemEmitter.GetEntityTypeAccessibility(item.ElementType), MetadataItemEmitter.GetEntitySetPropertyAccessibility(item)))
                    {
                        base.Generator.AddError(System.Data.Services.Design.Strings.EntityTypeAndSetAccessibilityConflict(item.ElementType.Name, MetadataItemEmitter.GetAccessibilityCsdlStringFromMemberAttribute(MetadataItemEmitter.GetEntityTypeAccessibility(item.ElementType)), item.Name, MetadataItemEmitter.GetAccessibilityCsdlStringFromMemberAttribute(MetadataItemEmitter.GetEntitySetPropertyAccessibility(item))), ModelBuilderErrorCode.EntityTypeAndSetAccessibilityConflict, EdmSchemaErrorSeverity.Error);
                    }
                }
            }
        }

        private EntityContainer Item =>
            (base.Item as EntityContainer);
    }
}


namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Design;
    using System.Data.Services.Design.Common;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal sealed class PropertyEmitter : PropertyEmitterBase
    {
        private CodeFieldReferenceExpression _complexPropertyInitializedFieldRef;
        private CodeFieldReferenceExpression _fieldRef;
        private const string DetachFromParentMethodName = "DetachFromParent";
        private const string NestedStoreObjectCollection = "InlineObjectCollection";

        public PropertyEmitter(ClientApiGenerator generator, EdmProperty property, bool declaringTypeUsesStandardBaseType) : base(generator, property, declaringTypeUsesStandardBaseType)
        {
        }

        internal static void AddGetterSetterFixUp(FixUpCollection fixups, string propertyFqName, MemberAttributes accessibility, MemberAttributes propertyAccessibility, bool isGetter)
        {
            if ((accessibility == MemberAttributes.Private) && (propertyAccessibility != MemberAttributes.Private))
            {
                if (isGetter)
                {
                    fixups.Add(new FixUp(propertyFqName, FixUpType.MarkPropertyGetAsPrivate));
                }
                else
                {
                    fixups.Add(new FixUp(propertyFqName, FixUpType.MarkPropertySetAsPrivate));
                }
            }
            if ((accessibility == MemberAttributes.Assembly) && (propertyAccessibility != MemberAttributes.Assembly))
            {
                if (isGetter)
                {
                    fixups.Add(new FixUp(propertyFqName, FixUpType.MarkPropertyGetAsInternal));
                }
                else
                {
                    fixups.Add(new FixUp(propertyFqName, FixUpType.MarkPropertySetAsInternal));
                }
            }
            if ((accessibility == MemberAttributes.Public) && (propertyAccessibility != MemberAttributes.Public))
            {
                if (isGetter)
                {
                    fixups.Add(new FixUp(propertyFqName, FixUpType.MarkPropertyGetAsPublic));
                }
                else
                {
                    fixups.Add(new FixUp(propertyFqName, FixUpType.MarkPropertySetAsPublic));
                }
            }
            if ((accessibility == MemberAttributes.Family) && (propertyAccessibility != MemberAttributes.Family))
            {
                if (isGetter)
                {
                    fixups.Add(new FixUp(propertyFqName, FixUpType.MarkPropertyGetAsProtected));
                }
                else
                {
                    fixups.Add(new FixUp(propertyFqName, FixUpType.MarkPropertySetAsProtected));
                }
            }
        }

        private void DisallowReturnTypeChange(CodeTypeReference baseType, CodeTypeReference newType)
        {
            if (System.Data.Metadata.Edm.Helper.IsCollectionType(this.Item.TypeUsage.EdmType) && (GetCollectionKind(this.Item.TypeUsage) != CollectionKind.None))
            {
                if (newType == null)
                {
                    throw EDesignUtil.InvalidOperation(Strings.CannotChangePropertyReturnTypeToNull(this.Item.Name, this.Item.DeclaringType.Name));
                }
            }
            else if (((baseType != null) || (newType != null)) && (((baseType != null) && !baseType.Equals(newType)) || ((newType != null) && !newType.Equals(baseType))))
            {
                throw EDesignUtil.InvalidOperation(Strings.CannotChangePropertyReturnType(this.Item.Name, this.Item.DeclaringType.Name));
            }
        }

        private void EmitCustomAttributes(CodeMemberProperty memberProperty, List<CodeAttributeDeclaration> additionalAttributes)
        {
            AttributeEmitter.AddGeneratedCodeAttribute(memberProperty);
            base.Generator.AttributeEmitter.EmitPropertyAttributes(this, memberProperty, additionalAttributes);
        }

        private void EmitField(CodeTypeDeclaration typeDecl, CodeTypeReference fieldType)
        {
            CodeMemberField ctm = new CodeMemberField(fieldType, this.FieldName) {
                Attributes = MemberAttributes.Private
            };
            if (HasDefault(this.Item))
            {
                ctm.InitExpression = this.GetDefaultValueExpression(this.Item);
            }
            AttributeEmitter.AddGeneratedCodeAttribute(ctm);
            typeDecl.Members.Add(ctm);
            if (System.Data.Metadata.Edm.TypeSemantics.IsComplexType(this.Item.TypeUsage))
            {
                CodeMemberField field2 = new CodeMemberField(base.TypeReference.ForType(typeof(bool)), this.ComplexPropertyInitializedFieldName) {
                    Attributes = MemberAttributes.Private
                };
                AttributeEmitter.AddGeneratedCodeAttribute(field2);
                typeDecl.Members.Add(field2);
            }
        }

        protected override void EmitProperty(CodeTypeDeclaration typeDecl)
        {
            CodeTypeReference propertyType = this.PropertyType;
            PropertyGeneratedEventArgs eventArgs = new PropertyGeneratedEventArgs(this.Item, this.FieldName, propertyType);
            base.Generator.RaisePropertyGeneratedEvent(eventArgs);
            this.DisallowReturnTypeChange(propertyType, eventArgs.ReturnType);
            CodeMemberProperty memberProperty = this.EmitPropertyDeclaration(eventArgs.ReturnType);
            if (memberProperty != null)
            {
                this.EmitCustomAttributes(memberProperty, eventArgs.AdditionalAttributes);
                this.EmitPropertyGetter(memberProperty, eventArgs.AdditionalGetStatements);
                this.EmitPropertySetter(memberProperty, eventArgs.AdditionalSetStatements, eventArgs.AdditionalAfterSetStatements);
                typeDecl.Members.Add(memberProperty);
                this.EmitField(typeDecl, eventArgs.ReturnType);
                this.EmitPropertyOnChangePartialMethods(typeDecl, eventArgs.ReturnType);
            }
        }

        public CodeMemberProperty EmitPropertyDeclaration(CodeTypeReference propertyReturnType)
        {
            MemberAttributes scope = MetadataItemEmitter.AccessibilityFromGettersAndSetters(this.Item);
            CodeMemberProperty property = this.EmitPropertyDeclaration(scope, propertyReturnType, this.IsVirtualProperty, this.HidesBaseClassProperty);
            property.HasSet = true;
            property.HasGet = true;
            return property;
        }

        private CodeMemberProperty EmitPropertyDeclaration(MemberAttributes scope, CodeTypeReference propertyType, bool isVirtual, bool hidesBaseProperty)
        {
            CodeMemberProperty property = new CodeMemberProperty {
                Name = this.PropertyName
            };
            CommentEmitter.EmitSummaryComments(this.Item, property.Comments);
            property.Attributes = scope;
            if (!isVirtual)
            {
                property.Attributes |= MemberAttributes.Final;
            }
            if (hidesBaseProperty || base.AncestorClassDefinesName(property.Name))
            {
                property.Attributes |= MemberAttributes.New;
            }
            property.Type = propertyType;
            return property;
        }

        private void EmitPropertyGetter(CodeMemberProperty memberProperty, List<CodeStatement> additionalGetStatements)
        {
            CodeStatementCollection getStatements = memberProperty.GetStatements;
            if ((additionalGetStatements != null) && (additionalGetStatements.Count > 0))
            {
                try
                {
                    CodeStatementCollection statements2 = new CodeStatementCollection();
                    statements2.AddRange(additionalGetStatements.ToArray());
                    if ((getStatements != null) && (getStatements.Count > 0))
                    {
                        statements2.AddRange(getStatements);
                    }
                    getStatements.Clear();
                    getStatements.AddRange(statements2);
                }
                catch (ArgumentNullException exception)
                {
                    base.Generator.AddError(Strings.InvalidGetStatementSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidGetStatementSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception);
                }
            }
            MemberAttributes propertyAccessibility = memberProperty.Attributes & MemberAttributes.AccessMask;
            AddGetterSetterFixUp(base.Generator.FixUps, this.PropertyFQName, MetadataItemEmitter.GetGetterAccessibility(this.Item), propertyAccessibility, true);
            this.EmitPropertyGetterBody(getStatements);
        }

        private void EmitPropertyGetterBody(CodeStatementCollection statements)
        {
            if (System.Data.Metadata.Edm.Helper.IsComplexType(this.Item.TypeUsage.EdmType))
            {
                if (GetCollectionKind(this.Item.TypeUsage) == CollectionKind.None)
                {
                    statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(Emitter.EmitExpressionEqualsNull(this.FieldRef), CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(this.ComplexPropertyInitializedFieldRef, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(true))), new CodeStatement[] { new CodeAssignStatement(this.FieldRef, new CodeObjectCreateExpression(this.PropertyType, new CodeExpression[0])), new CodeAssignStatement(this.ComplexPropertyInitializedFieldRef, new CodePrimitiveExpression(true)) }));
                }
                statements.Add(new CodeMethodReturnStatement(this.FieldRef));
            }
            else
            {
                PrimitiveType edmType = this.Item.TypeUsage.EdmType as PrimitiveType;
                if ((edmType != null) && (edmType.ClrEquivalentType == typeof(byte[])))
                {
                    statements.Add(new CodeConditionStatement(Emitter.EmitExpressionDoesNotEqualNull(this.FieldRef), new CodeStatement[] { new CodeMethodReturnStatement(new CodeCastExpression(typeof(byte[]), new CodeMethodInvokeExpression(this.FieldRef, "Clone", new CodeExpression[0]))) }, new CodeStatement[] { new CodeMethodReturnStatement(Emitter.NullExpression) }));
                }
                else
                {
                    statements.Add(new CodeMethodReturnStatement(this.FieldRef));
                }
            }
        }

        private void EmitPropertyOnChangePartialMethods(CodeTypeDeclaration typeDecl, CodeTypeReference returnType)
        {
            CodeMemberMethod method = new CodeMemberMethod {
                Name = this.OnChangingPartialMethodName(this.PropertyName),
                ReturnType = new CodeTypeReference(typeof(void)),
                Attributes = MemberAttributes.Public | MemberAttributes.Abstract
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(returnType, "value"));
            typeDecl.Members.Add(method);
            CodeMemberMethod method2 = new CodeMemberMethod {
                Name = this.OnChangedPartialMethodName(this.PropertyName),
                ReturnType = new CodeTypeReference(typeof(void)),
                Attributes = MemberAttributes.Public | MemberAttributes.Abstract
            };
            typeDecl.Members.Add(method2);
            base.Generator.FixUps.Add(new FixUp(this.PropertyClassName + "." + this.OnChangingPartialMethodName(this.PropertyName), FixUpType.MarkAbstractMethodAsPartial));
            base.Generator.FixUps.Add(new FixUp(this.PropertyClassName + "." + this.OnChangedPartialMethodName(this.PropertyName), FixUpType.MarkAbstractMethodAsPartial));
        }

        private void EmitPropertySetter(CodeMemberProperty memberProperty, List<CodeStatement> additionalSetStatements, List<CodeStatement> additionalAfterSetStatements)
        {
            CodeStatementCollection setStatements = memberProperty.SetStatements;
            MemberAttributes propertyAccessibility = memberProperty.Attributes & MemberAttributes.AccessMask;
            AddGetterSetterFixUp(base.Generator.FixUps, this.PropertyFQName, MetadataItemEmitter.GetSetterAccessibility(this.Item), propertyAccessibility, false);
            this.EmitPropertySetterBody(setStatements, additionalSetStatements, additionalAfterSetStatements);
        }

        private void EmitPropertySetterBody(CodeStatementCollection statements, List<CodeStatement> additionalSetStatements, List<CodeStatement> additionalAfterSetStatements)
        {
            statements.Add(new CodeMethodInvokeExpression(Emitter.ThisRef, this.OnChangingPartialMethodName(this.PropertyName), new CodeExpression[] { new CodePropertySetValueReferenceExpression() }));
            if ((additionalSetStatements != null) && (additionalSetStatements.Count > 0))
            {
                try
                {
                    statements.AddRange(additionalSetStatements.ToArray());
                }
                catch (ArgumentNullException exception)
                {
                    base.Generator.AddError(Strings.InvalidSetStatementSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidSetStatementSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception);
                }
            }
            statements.Add(new CodeAssignStatement(this.FieldRef, new CodePropertySetValueReferenceExpression()));
            if (System.Data.Metadata.Edm.Helper.IsComplexType(this.Item.TypeUsage.EdmType) && (GetCollectionKind(this.Item.TypeUsage) == CollectionKind.None))
            {
                statements.Add(new CodeAssignStatement(this.ComplexPropertyInitializedFieldRef, new CodePrimitiveExpression(true)));
            }
            statements.Add(new CodeMethodInvokeExpression(Emitter.ThisRef, this.OnChangedPartialMethodName(this.PropertyName), new CodeExpression[0]));
            if ((additionalAfterSetStatements != null) && (additionalAfterSetStatements.Count > 0))
            {
                try
                {
                    statements.AddRange(additionalAfterSetStatements.ToArray());
                }
                catch (ArgumentNullException exception2)
                {
                    base.Generator.AddError(Strings.InvalidSetStatementSuppliedForProperty(this.Item.Name), ModelBuilderErrorCode.InvalidSetStatementSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception2);
                }
            }
        }

        private CodeExpression GetCodeExpressionFromBinary(object value)
        {
            byte[] buffer = (byte[]) value;
            CodeExpression[] initializers = new CodeExpression[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                initializers[i] = new CodePrimitiveExpression(buffer[i]);
            }
            return new CodeArrayCreateExpression(base.TypeReference.ByteArray, initializers);
        }

        private CodeExpression GetCodeExpressionFromDateTimeDefaultValue(object value, EdmProperty property)
        {
            DateTime time = (DateTime) value;
            DateTime time2 = DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
            return new CodeObjectCreateExpression(base.TypeReference.DateTime, new CodeExpression[] { new CodePrimitiveExpression(time2.Ticks), this.GetEnumValue<DateTimeKind>(DateTimeKind.Unspecified) });
        }

        private CodeExpression GetCodeExpressionFromGuid(object value)
        {
            Guid guid = (Guid) value;
            return new CodeObjectCreateExpression(base.TypeReference.Guid, new CodeExpression[] { new CodePrimitiveExpression(guid.ToString("D", CultureInfo.InvariantCulture)) });
        }

        private static CollectionKind GetCollectionKind(TypeUsage usage)
        {
            Facet facet;
            if (usage.Facets.TryGetValue("CollectionKind", false, out facet))
            {
                return (CollectionKind) facet.Value;
            }
            return CollectionKind.None;
        }

        private CodeExpression GetDefaultValueExpression(EdmProperty property)
        {
            PrimitiveTypeKind kind;
            object defaultValue = property.DefaultValue;
            if ((defaultValue != null) && Utils.TryGetPrimitiveTypeKind(property.TypeUsage.EdmType, out kind))
            {
                switch (kind)
                {
                    case PrimitiveTypeKind.Binary:
                        return this.GetCodeExpressionFromBinary(defaultValue);

                    case PrimitiveTypeKind.Boolean:
                    case PrimitiveTypeKind.Byte:
                    case PrimitiveTypeKind.Decimal:
                    case PrimitiveTypeKind.Double:
                    case PrimitiveTypeKind.Single:
                    case PrimitiveTypeKind.Int16:
                    case PrimitiveTypeKind.Int32:
                    case PrimitiveTypeKind.Int64:
                    case PrimitiveTypeKind.String:
                        if (!property.Nullable && defaultValue.Equals(TypeSystem.GetDefaultValue(defaultValue.GetType())))
                        {
                            break;
                        }
                        return new CodePrimitiveExpression(defaultValue);

                    case PrimitiveTypeKind.DateTime:
                        if (!property.Nullable && defaultValue.Equals(TypeSystem.GetDefaultValue(defaultValue.GetType())))
                        {
                            break;
                        }
                        return this.GetCodeExpressionFromDateTimeDefaultValue(defaultValue, property);

                    case PrimitiveTypeKind.Guid:
                        if (!property.Nullable && defaultValue.Equals(TypeSystem.GetDefaultValue(defaultValue.GetType())))
                        {
                            break;
                        }
                        return this.GetCodeExpressionFromGuid(defaultValue);
                }
            }
            return null;
        }

        private CodeExpression GetEnumValue<T>(T value)
        {
            Type type = typeof(T);
            return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(base.TypeReference.ForType(type)), Enum.GetName(type, value));
        }

        private CodeTypeReference GetType(EdmProperty property, bool getElementType)
        {
            PropertyTypeReferences references = new PropertyTypeReferences();
            EdmType edmType = property.TypeUsage.EdmType;
            if (System.Data.Metadata.Edm.Helper.IsPrimitiveType(edmType))
            {
                references = new PropertyTypeReferences(base.TypeReference, (PrimitiveType) edmType);
            }
            else if (System.Data.Metadata.Edm.Helper.IsComplexType(edmType))
            {
                references = new PropertyTypeReferences(base.TypeReference, (ComplexType) edmType, base.Generator);
            }
            else if (System.Data.Metadata.Edm.Helper.IsCollectionType(edmType))
            {
                TypeUsage typeUsage = ((CollectionType) edmType).TypeUsage;
                if (System.Data.Metadata.Edm.Helper.IsPrimitiveType(typeUsage.EdmType))
                {
                    references = new PropertyTypeReferences(base.TypeReference, (PrimitiveType) typeUsage.EdmType, GetCollectionKind(property.TypeUsage));
                }
                else
                {
                    references = new PropertyTypeReferences(base.TypeReference, (ComplexType) typeUsage.EdmType, GetCollectionKind(property.TypeUsage), base.Generator);
                }
            }
            System.Data.Metadata.Edm.Helper.IsCollectionType(edmType);
            if (property.Nullable)
            {
                return references.Nullable;
            }
            return references.NonNullable;
        }

        internal static bool HasDefault(EdmProperty property) => 
            (property.DefaultValue != null);

        private string OnChangedPartialMethodName(string propertyName) => 
            ("On" + propertyName + "Changed");

        private string OnChangingPartialMethodName(string propertyName) => 
            ("On" + propertyName + "Changing");

        private string ComplexPropertyInitializedFieldName =>
            Utils.ComplexPropertyInitializedNameFromPropName(this.PropertyName);

        private CodeFieldReferenceExpression ComplexPropertyInitializedFieldRef
        {
            get
            {
                if (this._complexPropertyInitializedFieldRef == null)
                {
                    this._complexPropertyInitializedFieldRef = new CodeFieldReferenceExpression(Emitter.ThisRef, this.ComplexPropertyInitializedFieldName);
                }
                return this._complexPropertyInitializedFieldRef;
            }
        }

        public string EntityPropertyName =>
            this.Item.Name;

        private string FieldName =>
            Utils.FieldNameFromPropName(this.PropertyName);

        private CodeFieldReferenceExpression FieldRef
        {
            get
            {
                if (this._fieldRef == null)
                {
                    this._fieldRef = new CodeFieldReferenceExpression(Emitter.ThisRef, this.FieldName);
                }
                return this._fieldRef;
            }
        }

        private bool HidesBaseClassProperty
        {
            get
            {
                StructuralType baseType = this.Item.DeclaringType.BaseType as StructuralType;
                return ((baseType != null) && baseType.Members.Contains(this.PropertyName));
            }
        }

        public bool IsVirtualProperty =>
            false;

        public EdmProperty Item =>
            (base.Item as EdmProperty);

        private string PropertyClassName =>
            this.Item.DeclaringType.Name;

        public string PropertyFQName =>
            (this.Item.DeclaringType.FullName + "." + this.Item.Name);

        public string PropertyName =>
            this.EntityPropertyName;

        public CodeTypeReference PropertyType =>
            this.GetType(this.Item, false);

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertyTypeReferences
        {
            private CodeTypeReference _nonNullable;
            private CodeTypeReference _nullable;
            public PropertyTypeReferences(TypeReference typeReference, PrimitiveType primitiveType) : this(typeReference, primitiveType, CollectionKind.None)
            {
            }

            public PropertyTypeReferences(TypeReference typeReference, PrimitiveType primitiveType, CollectionKind collectionKind)
            {
                Type clrEquivalentType = primitiveType.ClrEquivalentType;
                if (collectionKind == CollectionKind.None)
                {
                    this._nonNullable = typeReference.ForType(clrEquivalentType);
                    if (clrEquivalentType.IsValueType)
                    {
                        this._nullable = typeReference.NullableForType(clrEquivalentType);
                    }
                    else
                    {
                        this._nullable = typeReference.ForType(clrEquivalentType);
                    }
                }
                else
                {
                    CodeTypeReference baseType = typeReference.ForType(clrEquivalentType);
                    CodeTypeReference reference2 = GetCollectionTypeReference(typeReference, baseType, collectionKind);
                    this._nonNullable = reference2;
                    this._nullable = reference2;
                }
            }

            public PropertyTypeReferences(TypeReference typeReference, ComplexType complexType, CollectionKind collectionKind, ClientApiGenerator generator)
            {
                CodeTypeReference leastPossibleQualifiedTypeReference = generator.GetLeastPossibleQualifiedTypeReference(complexType);
                leastPossibleQualifiedTypeReference = GetCollectionTypeReference(typeReference, leastPossibleQualifiedTypeReference, collectionKind);
                this._nonNullable = leastPossibleQualifiedTypeReference;
                this._nullable = leastPossibleQualifiedTypeReference;
            }

            private static CodeTypeReference GetCollectionTypeReference(TypeReference typeReference, CodeTypeReference baseType, CollectionKind collectionKind)
            {
                if (collectionKind == CollectionKind.Bag)
                {
                    baseType = GetCollectionTypeReferenceForBagSemantics(typeReference, baseType);
                    return baseType;
                }
                if (collectionKind == CollectionKind.List)
                {
                    baseType = GetCollectionTypeReferenceForListSemantics(typeReference, baseType);
                }
                return baseType;
            }

            public PropertyTypeReferences(TypeReference typeReference, ComplexType complexType, ClientApiGenerator generator) : this(typeReference, complexType, CollectionKind.None, generator)
            {
            }

            private static CodeTypeReference GetCollectionTypeReferenceForBagSemantics(TypeReference typeReference, CodeTypeReference baseType) => 
                typeReference.ForType(typeof(ICollection<>), new CodeTypeReference[] { baseType });

            private static CodeTypeReference GetCollectionTypeReferenceForListSemantics(TypeReference typeReference, CodeTypeReference baseType) => 
                typeReference.ForType(typeof(IList<>), new CodeTypeReference[] { baseType });

            public CodeTypeReference NonNullable =>
                this._nonNullable;
            public CodeTypeReference Nullable =>
                this._nullable;
        }
    }
}


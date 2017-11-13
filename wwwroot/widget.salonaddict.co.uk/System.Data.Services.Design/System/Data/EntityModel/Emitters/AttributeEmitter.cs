namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Common;
    using System.Data.Services.Design;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal sealed class AttributeEmitter
    {
        private static readonly CodeAttributeDeclaration _generatedCodeAttribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)), new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression("System.Data.Services.Design")), new CodeAttributeArgument(new CodePrimitiveExpression("1.0.0")) });
        private const string _generatorName = "System.Data.Services.Design";
        private const string _generatorVersion = "1.0.0";
        private System.Data.EntityModel.Emitters.TypeReference _typeReference;

        internal AttributeEmitter(System.Data.EntityModel.Emitters.TypeReference typeReference)
        {
            this._typeReference = typeReference;
        }

        public static void AddAttributeArguments(CodeAttributeDeclaration attribute, object[] arguments)
        {
            foreach (object obj2 in arguments)
            {
                CodeExpression expression = obj2 as CodeExpression;
                if (expression == null)
                {
                    expression = new CodePrimitiveExpression(obj2);
                }
                attribute.Arguments.Add(new CodeAttributeArgument(expression));
            }
        }

        public void AddBrowsableAttribute(CodeMemberProperty propertyDecl)
        {
        }

        private void AddEpmAttributeToTypeDeclaration(EntityPropertyMappingAttribute epmAttr, CodeTypeDeclaration typeDecl)
        {
            if (epmAttr.TargetSyndicationItem != SyndicationItemProperty.CustomProperty)
            {
                CodeFieldReferenceExpression expression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SyndicationItemProperty)), epmAttr.TargetSyndicationItem.ToString());
                CodeFieldReferenceExpression expression2 = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SyndicationTextContentKind)), epmAttr.TargetTextContentKind.ToString());
                CodeAttributeDeclaration attribute = new CodeAttributeDeclaration(this.TypeReference.FromString("System.Data.Services.Common.EntityPropertyMappingAttribute", true));
                AddAttributeArguments(attribute, new object[] { epmAttr.SourcePath, expression, expression2, epmAttr.KeepInContent });
                typeDecl.CustomAttributes.Add(attribute);
            }
            else
            {
                CodeAttributeDeclaration declaration2 = new CodeAttributeDeclaration(this.TypeReference.FromString("System.Data.Services.Common.EntityPropertyMappingAttribute", true));
                AddAttributeArguments(declaration2, new object[] { epmAttr.SourcePath, epmAttr.TargetPath, epmAttr.TargetNamespacePrefix, epmAttr.TargetNamespaceUri, epmAttr.KeepInContent });
                typeDecl.CustomAttributes.Add(declaration2);
            }
        }

        public static void AddGeneratedCodeAttribute(CodeTypeMember ctm)
        {
            ctm.CustomAttributes.Add(_generatedCodeAttribute);
        }

        public void AddIgnoreAttributes(CodeMemberProperty propertyDecl)
        {
        }

        private void EmitEpmAttributeForEntityProperty(EpmPropertyInformation propertyInformation, EdmInfo entityProperty, CodeTypeDeclaration typeDecl)
        {
            if (propertyInformation.IsAtom)
            {
                if (entityProperty.IsComplex)
                {
                    throw new InvalidOperationException(System.Data.Services.Design.Strings.ObjectContext_SyndicationMappingForComplexPropertiesNotAllowed);
                }
                EntityPropertyMappingAttribute epmAttr = new EntityPropertyMappingAttribute(propertyInformation.SourcePath, propertyInformation.SyndicationItem, propertyInformation.ContentKind, propertyInformation.KeepInContent);
                this.AddEpmAttributeToTypeDeclaration(epmAttr, typeDecl);
            }
            else if (entityProperty.IsComplex)
            {
                foreach (EntityPropertyMappingAttribute attribute2 in GetEpmAttrsFromComplexProperty(entityProperty.Member, propertyInformation.SourcePath, propertyInformation.TargetPath, propertyInformation.NsPrefix, propertyInformation.NsUri, propertyInformation.KeepInContent))
                {
                    this.AddEpmAttributeToTypeDeclaration(attribute2, typeDecl);
                }
            }
            else
            {
                EntityPropertyMappingAttribute attribute3 = new EntityPropertyMappingAttribute(propertyInformation.SourcePath, propertyInformation.TargetPath, propertyInformation.NsPrefix, propertyInformation.NsUri, propertyInformation.KeepInContent);
                this.AddEpmAttributeToTypeDeclaration(attribute3, typeDecl);
            }
        }

        private void EmitEpmAttributesForEntityType(EdmItemCollection itemCollection, EntityType entityType, CodeTypeDeclaration typeDecl)
        {
            foreach (EpmPropertyInformation information in GetEpmPropertyInformation(from mp in entityType.MetadataProperties
                where mp.PropertyKind == PropertyKind.Extended
                select mp, entityType.Name, null))
            {
                EdmMember entityPropertyFromEpmPath = GetEntityPropertyFromEpmPath(entityType, information.SourcePath);
                if (entityPropertyFromEpmPath == null)
                {
                    if (!this.IsOpenPropertyOnPath(entityType, information.SourcePath))
                    {
                        throw new InvalidOperationException(System.Data.Services.Design.Strings.ObjectContext_UnknownPropertyNameInEpmAttributesType(information.SourcePath, entityType.Name));
                    }
                    EdmInfo entityProperty = new EdmInfo {
                        IsComplex = false,
                        Member = null
                    };
                    this.EmitEpmAttributeForEntityProperty(information, entityProperty, typeDecl);
                }
                else
                {
                    EdmInfo info2 = new EdmInfo {
                        IsComplex = entityPropertyFromEpmPath.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType,
                        Member = entityPropertyFromEpmPath
                    };
                    this.EmitEpmAttributeForEntityProperty(information, info2, typeDecl);
                }
            }
            using (IEnumerator<EdmMember> enumerator2 = (from m in entityType.Members
                where m.DeclaringType == entityType
                select m).GetEnumerator())
            {
                Func<EdmProperty, bool> predicate = null;
                EdmMember member;
                while (enumerator2.MoveNext())
                {
                    member = enumerator2.Current;
                    if (predicate == null)
                    {
                        predicate = p => (p.DeclaringType == entityType) && (p.Name == member.Name);
                    }
                    EdmMember member2 = entityType.Properties.SingleOrDefault<EdmProperty>(predicate);
                    foreach (EpmPropertyInformation information2 in GetEpmPropertyInformation(from mdp in member.MetadataProperties
                        where mdp.PropertyKind == PropertyKind.Extended
                        select mdp, entityType.Name, member.Name))
                    {
                        EdmMember member3 = member2;
                        if ((member2.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType) && information2.PathGiven)
                        {
                            string sourcePath = information2.SourcePath;
                            information2.SourcePath = member2.Name + "/" + information2.SourcePath;
                            member3 = GetEntityPropertyFromEpmPath(entityType, information2.SourcePath);
                            if (member3 == null)
                            {
                                if (!this.IsOpenPropertyOnPath(member2.TypeUsage.EdmType as StructuralType, sourcePath))
                                {
                                    throw new InvalidOperationException(System.Data.Services.Design.Strings.ObjectContext_UnknownPropertyNameInEpmAttributesMember(sourcePath, member.Name, entityType.Name));
                                }
                                EdmInfo info3 = new EdmInfo {
                                    IsComplex = false,
                                    Member = null
                                };
                                this.EmitEpmAttributeForEntityProperty(information2, info3, typeDecl);
                                continue;
                            }
                        }
                        EdmInfo info4 = new EdmInfo {
                            IsComplex = member3.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType,
                            Member = member3
                        };
                        this.EmitEpmAttributeForEntityProperty(information2, info4, typeDecl);
                    }
                }
            }
        }

        public void EmitPropertyAttributes(PropertyEmitter emitter, CodeMemberProperty propertyDecl, List<CodeAttributeDeclaration> additionalAttributes)
        {
            if ((additionalAttributes != null) && (additionalAttributes.Count > 0))
            {
                try
                {
                    propertyDecl.CustomAttributes.AddRange(additionalAttributes.ToArray());
                }
                catch (ArgumentNullException exception)
                {
                    emitter.Generator.AddError(System.Data.Services.Design.Strings.InvalidAttributeSuppliedForProperty(emitter.Item.Name), ModelBuilderErrorCode.InvalidAttributeSuppliedForProperty, EdmSchemaErrorSeverity.Error, exception);
                }
            }
        }

        public CodeAttributeDeclaration EmitSimpleAttribute(string attributeType, params object[] arguments)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration(this.TypeReference.FromString(attributeType, true));
            AddAttributeArguments(attribute, arguments);
            return attribute;
        }

        private void EmitStreamAttributesForEntityType(EntityType entityType, CodeTypeDeclaration typeDecl)
        {
            IEnumerable<MetadataProperty> enumerable = from mp in entityType.MetadataProperties
                where (mp.PropertyKind == PropertyKind.Extended) && (mp.Name == "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata:HasStream")
                select mp;
            MetadataProperty property = null;
            foreach (MetadataProperty property2 in enumerable)
            {
                if (property != null)
                {
                    throw new InvalidOperationException(System.Data.Services.Design.Strings.ObjectContext_MultipleValuesForSameExtendedAttributeType("HasStream", entityType.Name));
                }
                property = property2;
            }
            if ((property != null) && string.Equals(Convert.ToString(property.Value, CultureInfo.InvariantCulture), "true", StringComparison.Ordinal))
            {
                CodeAttributeDeclaration declaration = new CodeAttributeDeclaration(this.TypeReference.FromString("System.Data.Services.Common.HasStreamAttribute", true));
                typeDecl.CustomAttributes.Add(declaration);
            }
        }

        public void EmitTypeAttributes(ComplexTypeEmitter emitter, CodeTypeDeclaration typeDecl)
        {
        }

        public void EmitTypeAttributes(EntityTypeEmitter emitter, CodeTypeDeclaration typeDecl)
        {
            if (emitter.Generator.Version != DataServiceCodeVersion.V1)
            {
                this.EmitEpmAttributesForEntityType(emitter.Generator.EdmItemCollection, emitter.Item, typeDecl);
                this.EmitStreamAttributesForEntityType(emitter.Item, typeDecl);
            }
            object[] arguments = (from km in emitter.Item.KeyMembers select km.Name).ToArray<object>();
            typeDecl.CustomAttributes.Add(this.EmitSimpleAttribute("System.Data.Services.Common.DataServiceKeyAttribute", arguments));
        }

        public void EmitTypeAttributes(SchemaTypeEmitter emitter, CodeTypeDeclaration typeDecl)
        {
        }

        public void EmitTypeAttributes(StructuredTypeEmitter emitter, CodeTypeDeclaration typeDecl)
        {
        }

        private static MetadataProperty FindSingletonExtendedProperty(IEnumerable<MetadataProperty> metadataExtendedProperties, string propertyName, string typeName, string memberName)
        {
            string extendedPropertyName = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata:" + propertyName;
            IEnumerable<MetadataProperty> enumerable = from mdp in metadataExtendedProperties
                where mdp.Name == extendedPropertyName
                select mdp;
            bool flag = false;
            MetadataProperty property = null;
            foreach (MetadataProperty property2 in enumerable)
            {
                if (flag)
                {
                    throw new InvalidOperationException((memberName == null) ? System.Data.Services.Design.Strings.ObjectContext_MultipleValuesForSameExtendedAttributeType(propertyName, typeName) : System.Data.Services.Design.Strings.ObjectContext_MultipleValuesForSameExtendedAttributeMember(propertyName, memberName, typeName));
                }
                property = property2;
                flag = true;
            }
            return property;
        }

        private static EdmMember GetEntityPropertyFromEpmPath(StructuralType baseEntityType, string sourcePath)
        {
            string[] propertyPath = sourcePath.Split(new char[] { '/' });
            if (!baseEntityType.Members.Any<EdmMember>(p => (p.Name == propertyPath[0])))
            {
                if (baseEntityType.BaseType == null)
                {
                    return null;
                }
                return GetEntityPropertyFromEpmPath(baseEntityType.BaseType as StructuralType, sourcePath);
            }
            EdmMember member = null;
            Func<EdmMember, bool> predicate = null;
            foreach (string pathSegment in propertyPath)
            {
                if (baseEntityType == null)
                {
                    return null;
                }
                if (predicate == null)
                {
                    predicate = p => p.Name == pathSegment;
                }
                member = baseEntityType.Members.SingleOrDefault<EdmMember>(predicate);
                if (member == null)
                {
                    return null;
                }
                baseEntityType = member.TypeUsage.EdmType as StructuralType;
            }
            return member;
        }

        private static IEnumerable<EntityPropertyMappingAttribute> GetEpmAttrsFromComplexProperty(EdmMember complexProperty, string epmSourcePath, string epmTargetPath, string epmNsPrefix, string epmNsUri, bool epmKeepContent)
        {
            ComplexType edmType = complexProperty.TypeUsage.EdmType as ComplexType;
            if (edmType == null)
            {
                throw new ArgumentException(System.Data.Services.Design.Strings.ExpectingComplexTypeForMember(complexProperty.Name, complexProperty.DeclaringType.Name));
            }
            foreach (EdmMember iteratorVariable1 in edmType.Properties)
            {
                string iteratorVariable2 = epmSourcePath + "/" + iteratorVariable1.Name;
                string iteratorVariable3 = epmTargetPath + "/" + iteratorVariable1.Name;
                if (iteratorVariable1.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType)
                {
                    foreach (EntityPropertyMappingAttribute iteratorVariable4 in GetEpmAttrsFromComplexProperty(iteratorVariable1, iteratorVariable2, iteratorVariable3, epmNsPrefix, epmNsUri, epmKeepContent))
                    {
                        yield return iteratorVariable4;
                    }
                    continue;
                }
                yield return new EntityPropertyMappingAttribute(iteratorVariable2, iteratorVariable3, epmNsPrefix, epmNsUri, epmKeepContent);
            }
        }

        private static IEnumerable<EpmPropertyInformation> GetEpmPropertyInformation(IEnumerable<MetadataProperty> extendedProperties, string typeName, string memberName)
        {
            EpmAttributeNameBuilder iteratorVariable0 = new EpmAttributeNameBuilder();
            while (true)
            {
                string iteratorVariable6;
                bool iteratorVariable1 = true;
                MetadataProperty iteratorVariable2 = FindSingletonExtendedProperty(extendedProperties, iteratorVariable0.EpmTargetPath, typeName, memberName);
                if (iteratorVariable2 == null)
                {
                    break;
                }
                bool result = true;
                MetadataProperty iteratorVariable4 = FindSingletonExtendedProperty(extendedProperties, iteratorVariable0.EpmKeepInContent, typeName, memberName);
                if ((iteratorVariable4 != null) && !bool.TryParse(Convert.ToString(iteratorVariable4.Value, CultureInfo.InvariantCulture), out result))
                {
                    throw new InvalidOperationException((memberName == null) ? System.Data.Services.Design.Strings.ObjectContext_InvalidValueForEpmPropertyType(iteratorVariable0.EpmKeepInContent, typeName) : System.Data.Services.Design.Strings.ObjectContext_InvalidValueForEpmPropertyMember(iteratorVariable0.EpmKeepInContent, memberName, typeName));
                }
                MetadataProperty iteratorVariable5 = FindSingletonExtendedProperty(extendedProperties, iteratorVariable0.EpmSourcePath, typeName, memberName);
                if (iteratorVariable5 == null)
                {
                    if (memberName == null)
                    {
                        throw new InvalidOperationException(System.Data.Services.Design.Strings.ObjectContext_MissingExtendedAttributeType(iteratorVariable0.EpmSourcePath, typeName));
                    }
                    iteratorVariable1 = false;
                    iteratorVariable6 = memberName;
                }
                else
                {
                    iteratorVariable6 = Convert.ToString(iteratorVariable5.Value, CultureInfo.InvariantCulture);
                }
                string targetPath = Convert.ToString(iteratorVariable2.Value, CultureInfo.InvariantCulture);
                SyndicationItemProperty iteratorVariable8 = MapEpmTargetPathToSyndicationProperty(targetPath);
                MetadataProperty iteratorVariable9 = FindSingletonExtendedProperty(extendedProperties, iteratorVariable0.EpmContentKind, typeName, memberName);
                MetadataProperty iteratorVariable10 = FindSingletonExtendedProperty(extendedProperties, iteratorVariable0.EpmNsPrefix, typeName, memberName);
                MetadataProperty iteratorVariable11 = FindSingletonExtendedProperty(extendedProperties, iteratorVariable0.EpmNsUri, typeName, memberName);
                if ((iteratorVariable9 != null) && ((iteratorVariable10 != null) || (iteratorVariable11 != null)))
                {
                    string str = (iteratorVariable10 != null) ? iteratorVariable0.EpmNsPrefix : iteratorVariable0.EpmNsUri;
                    throw new InvalidOperationException((memberName == null) ? System.Data.Services.Design.Strings.ObjectContext_InvalidAttributeForNonSyndicationItemsType(str, typeName) : System.Data.Services.Design.Strings.ObjectContext_InvalidAttributeForNonSyndicationItemsMember(str, memberName, typeName));
                }
                if (((iteratorVariable10 != null) || (iteratorVariable11 != null)) || (iteratorVariable8 == SyndicationItemProperty.CustomProperty))
                {
                    string iteratorVariable12 = (iteratorVariable10 != null) ? Convert.ToString(iteratorVariable10.Value, CultureInfo.InvariantCulture) : null;
                    string iteratorVariable13 = (iteratorVariable11 != null) ? Convert.ToString(iteratorVariable11.Value, CultureInfo.InvariantCulture) : null;
                    EpmPropertyInformation iteratorVariable14 = new EpmPropertyInformation {
                        IsAtom = false,
                        KeepInContent = result,
                        SourcePath = iteratorVariable6,
                        PathGiven = iteratorVariable1,
                        TargetPath = targetPath,
                        NsPrefix = iteratorVariable12,
                        NsUri = iteratorVariable13
                    };
                    yield return iteratorVariable14;
                }
                else
                {
                    SyndicationTextContentKind plaintext;
                    if (iteratorVariable9 != null)
                    {
                        plaintext = MapEpmContentKindToSyndicationTextContentKind(Convert.ToString(iteratorVariable9.Value, CultureInfo.InvariantCulture), typeName, memberName);
                    }
                    else
                    {
                        plaintext = SyndicationTextContentKind.Plaintext;
                    }
                    EpmPropertyInformation iteratorVariable16 = new EpmPropertyInformation {
                        IsAtom = true,
                        KeepInContent = result,
                        SourcePath = iteratorVariable6,
                        PathGiven = iteratorVariable1,
                        SyndicationItem = iteratorVariable8,
                        ContentKind = plaintext
                    };
                    yield return iteratorVariable16;
                }
                iteratorVariable0.MoveNext();
            }
        }

        private bool IsOpenPropertyOnPath(StructuralType baseEntityType, string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                return false;
            }
            string[] propertyPath = sourcePath.Split(new char[] { '/' });
            EdmMember member = baseEntityType.Members.SingleOrDefault<EdmMember>(p => p.Name == propertyPath[0]);
            if (member == null)
            {
                if (baseEntityType.BaseType != null)
                {
                    return this.IsOpenPropertyOnPath(baseEntityType.BaseType as StructuralType, sourcePath);
                }
                return IsOpenType(baseEntityType);
            }
            StructuralType edmType = member.TypeUsage.EdmType as StructuralType;
            return ((edmType != null) && this.IsOpenPropertyOnPath(edmType, string.Join("/", propertyPath, 1, propertyPath.Length - 1)));
        }

        private static bool IsOpenType(StructuralType entityType)
        {
            bool flag;
            MetadataProperty property = entityType.MetadataProperties.FirstOrDefault<MetadataProperty>(x => x.Name == "http://schemas.microsoft.com/ado/2008/01/edm:OpenType");
            if (property == null)
            {
                return false;
            }
            if (!bool.TryParse(Convert.ToString(property.Value, CultureInfo.InvariantCulture), out flag))
            {
                throw new InvalidOperationException(System.Data.Services.Design.Strings.ObjectContext_OpenTypePropertyValueIsNotCorrect("OpenType", entityType.Name));
            }
            return flag;
        }

        private static SyndicationTextContentKind MapEpmContentKindToSyndicationTextContentKind(string strContentKind, string typeName, string memberName)
        {
            switch (strContentKind)
            {
                case "text":
                    return SyndicationTextContentKind.Plaintext;

                case "html":
                    return SyndicationTextContentKind.Html;

                case "xhtml":
                    return SyndicationTextContentKind.Xhtml;
            }
            throw new InvalidOperationException((memberName == null) ? System.Data.Services.Design.Strings.ObjectContext_InvalidValueForTargetTextContentKindPropertyType(strContentKind, typeName) : System.Data.Services.Design.Strings.ObjectContext_InvalidValueForTargetTextContentKindPropertyMember(strContentKind, memberName, typeName));
        }

        private static SyndicationItemProperty MapEpmTargetPathToSyndicationProperty(string targetPath)
        {
            switch (targetPath)
            {
                case "SyndicationAuthorEmail":
                    return SyndicationItemProperty.AuthorEmail;

                case "SyndicationAuthorName":
                    return SyndicationItemProperty.AuthorName;

                case "SyndicationAuthorUri":
                    return SyndicationItemProperty.AuthorUri;

                case "SyndicationContributorEmail":
                    return SyndicationItemProperty.ContributorEmail;

                case "SyndicationContributorName":
                    return SyndicationItemProperty.ContributorName;

                case "SyndicationContributorUri":
                    return SyndicationItemProperty.ContributorUri;

                case "SyndicationUpdated":
                    return SyndicationItemProperty.Updated;

                case "SyndicationPublished":
                    return SyndicationItemProperty.Published;

                case "SyndicationRights":
                    return SyndicationItemProperty.Rights;

                case "SyndicationSummary":
                    return SyndicationItemProperty.Summary;

                case "SyndicationTitle":
                    return SyndicationItemProperty.Title;
            }
            return SyndicationItemProperty.CustomProperty;
        }

        internal System.Data.EntityModel.Emitters.TypeReference TypeReference =>
            this._typeReference;



        private sealed class EdmInfo
        {
            public bool IsComplex { get; set; }

            public EdmMember Member { get; set; }
        }

        private sealed class EpmPropertyInformation
        {
            internal SyndicationTextContentKind ContentKind { get; set; }

            internal bool IsAtom { get; set; }

            internal bool KeepInContent { get; set; }

            internal string NsPrefix { get; set; }

            internal string NsUri { get; set; }

            internal bool PathGiven { get; set; }

            internal string SourcePath { get; set; }

            internal SyndicationItemProperty SyndicationItem { get; set; }

            internal string TargetPath { get; set; }
        }
    }
}


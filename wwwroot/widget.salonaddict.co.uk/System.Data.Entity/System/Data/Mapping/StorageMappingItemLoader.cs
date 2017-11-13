namespace System.Data.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;

    internal class StorageMappingItemLoader
    {
        private Dictionary<string, string> m_alias;
        private bool m_hasQueryViews;
        private List<EdmSchemaError> m_parsingErrors;
        private Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>> m_scalarMemberMappings;
        private string m_sourceLocation;
        private StorageMappingItemCollection m_storageMappingItemCollection;
        private static XmlSchemaSet s_mappingXmlSchema;

        internal StorageMappingItemLoader(StorageMappingItemCollection storageMappingItemCollection, string fileName, Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>> scalarMemberMappings)
        {
            this.m_storageMappingItemCollection = storageMappingItemCollection;
            this.m_alias = new Dictionary<string, string>(StringComparer.Ordinal);
            if (fileName != null)
            {
                this.m_sourceLocation = fileName;
            }
            else
            {
                this.m_sourceLocation = null;
            }
            this.m_parsingErrors = new List<EdmSchemaError>();
            this.m_scalarMemberMappings = scalarMemberMappings;
        }

        internal static void AddToSchemaErrors(string message, StorageMappingErrorCode errorCode, string location, IXmlLineInfo lineInfo, IList<EdmSchemaError> parsingErrors)
        {
            EdmSchemaError item = new EdmSchemaError(message, (int) errorCode, EdmSchemaErrorSeverity.Error, location, lineInfo.LineNumber, lineInfo.LinePosition);
            parsingErrors.Add(item);
        }

        internal static void AddToSchemaErrorsWithMemberInfo(Func<object, string> messageFormat, string errorMember, StorageMappingErrorCode errorCode, string location, IXmlLineInfo lineInfo, IList<EdmSchemaError> parsingErrors)
        {
            EdmSchemaError item = new EdmSchemaError(messageFormat(errorMember), (int) errorCode, EdmSchemaErrorSeverity.Error, location, lineInfo.LineNumber, lineInfo.LinePosition);
            parsingErrors.Add(item);
        }

        internal static void AddToSchemaErrorWithMemberAndStructure(System.Func<object, object, string> messageFormat, string errorMember, string errorStructure, StorageMappingErrorCode errorCode, string location, IXmlLineInfo lineInfo, IList<EdmSchemaError> parsingErrors)
        {
            EdmSchemaError item = new EdmSchemaError(messageFormat(errorMember, errorStructure), (int) errorCode, EdmSchemaErrorSeverity.Error, location, lineInfo.LineNumber, lineInfo.LinePosition);
            parsingErrors.Add(item);
        }

        internal static void AddToSchemaErrorWithMessage(string errorMessage, StorageMappingErrorCode errorCode, string location, IXmlLineInfo lineInfo, IList<EdmSchemaError> parsingErrors)
        {
            EdmSchemaError item = new EdmSchemaError(errorMessage, (int) errorCode, EdmSchemaErrorSeverity.Error, location, lineInfo.LineNumber, lineInfo.LinePosition);
            parsingErrors.Add(item);
        }

        private static System.Data.Common.Utils.Set<EntitySetBase> FindMissingFunctionMappings(StorageEntityContainerMapping entityContainerMapping, System.Data.Common.Utils.Set<EntitySetBase> setsWithFunctionMapping)
        {
            System.Data.Common.Utils.Set<EntitySetBase> set = new System.Data.Common.Utils.Set<EntitySetBase>();
            foreach (EntitySetBase base2 in setsWithFunctionMapping)
            {
                if (BuiltInTypeKind.AssociationSet == base2.BuiltInTypeKind)
                {
                    AssociationSet set2 = (AssociationSet) base2;
                    foreach (AssociationSetEnd end in set2.AssociationSetEnds)
                    {
                        switch (end.CorrespondingAssociationEndMember.RelationshipMultiplicity)
                        {
                            case RelationshipMultiplicity.One:
                            case RelationshipMultiplicity.ZeroOrOne:
                            {
                                EntitySet entitySet = MetadataHelper.GetOppositeEnd(end).EntitySet;
                                if (!setsWithFunctionMapping.Contains(entitySet))
                                {
                                    set.Add(MetadataHelper.GetOppositeEnd(end).EntitySet);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            foreach (EntitySetBase base3 in entityContainerMapping.EdmEntityContainer.BaseEntitySets)
            {
                if (BuiltInTypeKind.AssociationSet == base3.BuiltInTypeKind)
                {
                    AssociationSet element = (AssociationSet) base3;
                    if (!setsWithFunctionMapping.Contains(element))
                    {
                        foreach (AssociationSetEnd end2 in element.AssociationSetEnds)
                        {
                            switch (end2.CorrespondingAssociationEndMember.RelationshipMultiplicity)
                            {
                                case RelationshipMultiplicity.One:
                                case RelationshipMultiplicity.ZeroOrOne:
                                {
                                    AssociationSetEnd oppositeEnd = MetadataHelper.GetOppositeEnd(end2);
                                    if (setsWithFunctionMapping.Contains(oppositeEnd.EntitySet))
                                    {
                                        set.Add(element);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return set;
        }

        private string GetAliasResolvedAttributeValue(XPathNavigator nav, string attributeName) => 
            this.GetAliasResolvedValue(GetAttributeValue(nav, attributeName));

        private string GetAliasResolvedValue(string aliasedString)
        {
            if ((aliasedString != null) && (aliasedString.Length != 0))
            {
                int length = aliasedString.LastIndexOf('.');
                if (length != -1)
                {
                    string str2;
                    string key = aliasedString.Substring(0, length);
                    this.m_alias.TryGetValue(key, out str2);
                    if (str2 != null)
                    {
                        aliasedString = str2 + aliasedString.Substring(length);
                    }
                }
            }
            return aliasedString;
        }

        private static string GetAttributeValue(XPathNavigator nav, string attributeName) => 
            Helper.GetAttributeValue(nav, attributeName);

        private static EnumMember GetEnumAttributeValue(XPathNavigator nav, string attributeName, EnumType enumType, string sourceLocation, IList<EdmSchemaError> parsingErrors)
        {
            EnumMember member;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            string attributeValue = GetAttributeValue(nav, attributeName);
            if (string.IsNullOrEmpty(attributeValue))
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Enum_EmptyValue_1), enumType.FullName, StorageMappingErrorCode.InvalidEnumValue, sourceLocation, lineInfo, parsingErrors);
            }
            if (!enumType.EnumMembers.TryGetValue(attributeValue, false, out member))
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Enum_InvalidValue_1), attributeValue, StorageMappingErrorCode.InvalidEnumValue, sourceLocation, lineInfo, parsingErrors);
            }
            return member;
        }

        private string GetFacetsForDisplay(TypeUsage typeUsage)
        {
            ReadOnlyMetadataCollection<Facet> facets = typeUsage.Facets;
            if ((facets == null) || (facets.Count == 0))
            {
                return string.Empty;
            }
            int count = facets.Count;
            StringBuilder builder = new StringBuilder("[");
            for (int i = 0; i < (count - 1); i++)
            {
                builder.AppendFormat("{0}={1},", facets[i].Name, facets[i].Value ?? string.Empty);
            }
            builder.AppendFormat("{0}={1}]", facets[count - 1].Name, facets[count - 1].Value ?? string.Empty);
            return builder.ToString();
        }

        private static XmlSchemaSet GetOrCreateSchemaSet()
        {
            if (s_mappingXmlSchema == null)
            {
                using (XmlReader reader = XmlReader.Create(typeof(StorageMappingItemLoader).Assembly.GetManifestResourceStream("System.Data.Resources.CSMSL.xsd")))
                {
                    XmlSchema schema = XmlSchema.Read(reader, null);
                    XmlSchemaSet set = new XmlSchemaSet();
                    set.Add(schema);
                    Interlocked.CompareExchange<XmlSchemaSet>(ref s_mappingXmlSchema, set, null);
                }
            }
            return s_mappingXmlSchema;
        }

        internal XmlReader GetSchemaValidatingReader(XmlReader innerReader)
        {
            XmlReaderSettings xmlReaderSettings = this.GetXmlReaderSettings();
            return XmlReader.Create(innerReader, xmlReaderSettings);
        }

        private XmlReaderSettings GetXmlReaderSettings()
        {
            XmlReaderSettings settings = Schema.CreateEdmStandardXmlReaderSettings();
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(this.XsdValidationCallBack);
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = GetOrCreateSchemaSet();
            return settings;
        }

        private static void IncrementCount<T>(Dictionary<T, int> counts, T key)
        {
            int num;
            if (counts.TryGetValue(key, out num))
            {
                num++;
            }
            else
            {
                num = 1;
            }
            counts[key] = num;
        }

        internal static bool IsTypeSupportedForCondition(PrimitiveTypeKind primitiveTypeKind)
        {
            switch (primitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary:
                case PrimitiveTypeKind.DateTime:
                case PrimitiveTypeKind.Decimal:
                case PrimitiveTypeKind.Double:
                case PrimitiveTypeKind.Guid:
                case PrimitiveTypeKind.Single:
                case PrimitiveTypeKind.Time:
                case PrimitiveTypeKind.DateTimeOffset:
                    return false;

                case PrimitiveTypeKind.Boolean:
                case PrimitiveTypeKind.Byte:
                case PrimitiveTypeKind.SByte:
                case PrimitiveTypeKind.Int16:
                case PrimitiveTypeKind.Int32:
                case PrimitiveTypeKind.Int64:
                case PrimitiveTypeKind.String:
                    return true;
            }
            return false;
        }

        private StorageMappingFragment LoadAssociationMappingFragment(XPathNavigator nav, StorageAssociationSetMapping setMapping, StorageAssociationTypeMapping typeMapping, string tableName, System.Data.Metadata.Edm.EntityContainer storageEntityContainerType)
        {
            string str2;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            StorageMappingFragment fragment = null;
            System.Data.Metadata.Edm.EntityType tableType = null;
            if (setMapping.QueryView == null)
            {
                EntitySet set;
                storageEntityContainerType.TryGetEntitySetByName(tableName, false, out set);
                if (set == null)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Table_1), tableName, StorageMappingErrorCode.InvalidTable, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    return null;
                }
                tableType = set.ElementType;
                fragment = new StorageMappingFragment(set, typeMapping) {
                    StartLineNumber = setMapping.StartLineNumber,
                    StartLinePosition = setMapping.StartLinePosition
                };
            }
        Label_0071:
            if ((str2 = nav.LocalName) != null)
            {
                switch (str2)
                {
                    case "EndProperty":
                    {
                        if (setMapping.QueryView != null)
                        {
                            AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_QueryView_PropertyMaps_1), setMapping.Set.Name, StorageMappingErrorCode.PropertyMapsWithQueryView, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                            return null;
                        }
                        string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
                        EdmMember item = null;
                        typeMapping.AssociationType.Members.TryGetValue(aliasResolvedAttributeValue, false, out item);
                        AssociationEndMember end = item as AssociationEndMember;
                        if (end == null)
                        {
                            AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_End_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                        }
                        else
                        {
                            fragment.AddProperty(this.LoadEndPropertyMapping(nav.Clone(), end, tableType));
                        }
                        goto Label_0232;
                    }
                    case "Condition":
                    {
                        if (setMapping.QueryView != null)
                        {
                            AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_QueryView_PropertyMaps_1), setMapping.Set.Name, StorageMappingErrorCode.PropertyMapsWithQueryView, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                            return null;
                        }
                        StorageConditionPropertyMapping conditionPropertyMap = this.LoadConditionPropertyMapping(nav.Clone(), null, tableType);
                        if ((conditionPropertyMap == null) || fragment.AddConditionProperty(conditionPropertyMap))
                        {
                            goto Label_0232;
                        }
                        EdmProperty property = (conditionPropertyMap.EdmProperty != null) ? conditionPropertyMap.EdmProperty : conditionPropertyMap.ColumnProperty;
                        AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Duplicate_Condition_Member_1), property.Name, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                        return null;
                    }
                    case "ModificationFunctionMapping":
                        this.LoadAssociationTypeFunctionMapping(nav.Clone(), setMapping, typeMapping);
                        goto Label_0232;
                }
            }
            AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_General_0, StorageMappingErrorCode.InvalidContent, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
        Label_0232:
            if (nav.MoveToNext(XPathNodeType.Element))
            {
                goto Label_0071;
            }
            if (setMapping.QueryView == null)
            {
                fragment.EndLineNumber = lineInfo.LineNumber;
                fragment.EndLinePosition = lineInfo.LinePosition;
            }
            return fragment;
        }

        private void LoadAssociationSetMapping(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping, System.Data.Metadata.Edm.EntityContainer storageEntityContainerType)
        {
            RelationshipSet set;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
            string str2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
            string str3 = this.GetAliasResolvedAttributeValue(nav.Clone(), "StoreEntitySet");
            entityContainerMapping.EdmEntityContainer.TryGetRelationshipSetByName(aliasResolvedAttributeValue, false, out set);
            AssociationSet associationSet = set as AssociationSet;
            if (associationSet == null)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Association_Set_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidAssociationSet, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            else if (entityContainerMapping.ContainsAssociationSetMapping(associationSet))
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Duplicate_CdmAssociationSet_StorageMap_1), aliasResolvedAttributeValue, StorageMappingErrorCode.DuplicateSetMapping, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            else
            {
                StorageAssociationSetMapping setMapping = new StorageAssociationSetMapping(associationSet, entityContainerMapping) {
                    StartLineNumber = lineInfo.LineNumber,
                    StartLinePosition = lineInfo.LinePosition
                };
                if (!nav.MoveToChild(XPathNodeType.Element))
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Emtpty_SetMap_1), associationSet.Name, StorageMappingErrorCode.EmptySetMapping, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                else
                {
                    entityContainerMapping.AddAssociationSetMapping(setMapping);
                    if (nav.LocalName == "QueryView")
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_TableName_QueryView_1), aliasResolvedAttributeValue, StorageMappingErrorCode.TableNameAttributeWithQueryView, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                            return;
                        }
                        if (!this.LoadQueryView(nav.Clone(), setMapping))
                        {
                            return;
                        }
                        if (!nav.MoveToNext(XPathNodeType.Element))
                        {
                            return;
                        }
                    }
                    if ((nav.LocalName == "EndProperty") || (nav.LocalName == "ModificationFunctionMapping"))
                    {
                        if (string.IsNullOrEmpty(str2))
                        {
                            AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_Association_Type_Empty_0, StorageMappingErrorCode.InvalidAssociationType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                        }
                        else
                        {
                            this.LoadAssociationTypeMapping(nav.Clone(), setMapping, str2, str3, storageEntityContainerType);
                        }
                    }
                    else if (nav.LocalName == "Condition")
                    {
                        AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_AssociationSet_Condition_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidContent, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    }
                }
            }
        }

        private void LoadAssociationTypeFunctionMapping(XPathNavigator nav, StorageAssociationSetMapping associationSetMapping, StorageAssociationTypeMapping associationTypeMapping)
        {
            FunctionMappingLoader loader = new FunctionMappingLoader(this, associationSetMapping.Set);
            StorageFunctionMapping deleteFunctionMapping = null;
            StorageFunctionMapping insertFunctionMapping = null;
            if (nav.MoveToChild(XPathNodeType.Element))
            {
                do
                {
                    string localName = nav.LocalName;
                    if (localName != null)
                    {
                        if (localName == "DeleteFunction")
                        {
                            deleteFunctionMapping = loader.LoadAssociationSetFunctionMapping(nav.Clone(), false);
                        }
                        else if (localName == "InsertFunction")
                        {
                            insertFunctionMapping = loader.LoadAssociationSetFunctionMapping(nav.Clone(), true);
                        }
                    }
                }
                while (nav.MoveToNext(XPathNodeType.Element));
            }
            if ((deleteFunctionMapping != null) && (insertFunctionMapping != null))
            {
                associationSetMapping.FunctionMapping = new StorageAssociationSetFunctionMapping((AssociationSet) associationSetMapping.Set, deleteFunctionMapping, insertFunctionMapping);
            }
        }

        private void LoadAssociationTypeMapping(XPathNavigator nav, StorageAssociationSetMapping associationSetMapping, string associationTypeName, string tableName, System.Data.Metadata.Edm.EntityContainer storageEntityContainerType)
        {
            AssociationType type;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            this.EdmItemCollection.TryGetItem<AssociationType>(associationTypeName, out type);
            if (type == null)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Association_Type_1), associationTypeName, StorageMappingErrorCode.InvalidAssociationType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            else if (!associationSetMapping.Set.ElementType.Equals(type))
            {
                AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_Invalid_Association_Type_For_Association_Set_3(associationTypeName, associationSetMapping.Set.ElementType.FullName, associationSetMapping.Set.Name), StorageMappingErrorCode.DuplicateTypeMapping, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            else
            {
                StorageAssociationTypeMapping typeMapping = new StorageAssociationTypeMapping(type, associationSetMapping);
                associationSetMapping.AddTypeMapping(typeMapping);
                if (string.IsNullOrEmpty(tableName) && (associationSetMapping.QueryView == null))
                {
                    AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_Table_Expected_0, StorageMappingErrorCode.InvalidTable, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                else
                {
                    StorageMappingFragment fragment = this.LoadAssociationMappingFragment(nav.Clone(), associationSetMapping, typeMapping, tableName, storageEntityContainerType);
                    if (fragment != null)
                    {
                        typeMapping.AddFragment(fragment);
                    }
                }
            }
        }

        private StorageComplexPropertyMapping LoadComplexPropertyMapping(XPathNavigator nav, EdmType containerType, System.Data.Metadata.Edm.EntityType tableType)
        {
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            CollectionType type = containerType as CollectionType;
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
            EdmProperty cdmMember = null;
            EdmType item = null;
            string str2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
            StructuralType type3 = containerType as StructuralType;
            if (string.IsNullOrEmpty(str2))
            {
                if (type == null)
                {
                    EdmMember member;
                    type3.Members.TryGetValue(aliasResolvedAttributeValue, false, out member);
                    cdmMember = member as EdmProperty;
                    if (cdmMember == null)
                    {
                        AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Cdm_Member_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    }
                    item = cdmMember.TypeUsage.EdmType;
                }
                else
                {
                    item = type.TypeUsage.EdmType;
                }
            }
            else
            {
                if (containerType != null)
                {
                    EdmMember member2;
                    type3.Members.TryGetValue(aliasResolvedAttributeValue, false, out member2);
                    cdmMember = member2 as EdmProperty;
                }
                if (cdmMember == null)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Cdm_Member_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                this.EdmItemCollection.TryGetItem<EdmType>(str2, out item);
                item = item as ComplexType;
                if (item == null)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Complex_Type_1), str2, StorageMappingErrorCode.InvalidComplexType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
            }
            StorageComplexPropertyMapping mapping = new StorageComplexPropertyMapping(cdmMember);
            XPathNavigator navigator = nav.Clone();
            bool flag = false;
            if (navigator.MoveToChild(XPathNodeType.Element) && (navigator.LocalName == "ComplexTypeMapping"))
            {
                flag = true;
            }
            if ((cdmMember == null) || (item == null))
            {
                return null;
            }
            if (flag)
            {
                nav.MoveToChild(XPathNodeType.Element);
                do
                {
                    mapping.AddTypeMapping(this.LoadComplexTypeMapping(nav.Clone(), null, tableType));
                }
                while (nav.MoveToNext(XPathNodeType.Element));
                return mapping;
            }
            mapping.AddTypeMapping(this.LoadComplexTypeMapping(nav.Clone(), item, tableType));
            return mapping;
        }

        private StorageComplexTypeMapping LoadComplexTypeMapping(XPathNavigator nav, EdmType type, System.Data.Metadata.Edm.EntityType tableType)
        {
            bool isPartial = false;
            string attributeValue = GetAttributeValue(nav.Clone(), "IsPartial");
            if (!string.IsNullOrEmpty(attributeValue))
            {
                isPartial = Convert.ToBoolean(attributeValue, CultureInfo.InvariantCulture);
            }
            StorageComplexTypeMapping mapping = new StorageComplexTypeMapping(isPartial);
            if (type != null)
            {
                mapping.AddType(type as ComplexType);
            }
            else
            {
                string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
                int index = aliasResolvedAttributeValue.IndexOf(';');
                string aliasedString = null;
                do
                {
                    ComplexType type2;
                    if (index != -1)
                    {
                        aliasedString = aliasResolvedAttributeValue.Substring(0, index);
                        aliasResolvedAttributeValue = aliasResolvedAttributeValue.Substring(index + 1, aliasResolvedAttributeValue.Length - (index + 1));
                    }
                    else
                    {
                        aliasedString = aliasResolvedAttributeValue;
                        aliasResolvedAttributeValue = string.Empty;
                    }
                    int num2 = aliasedString.IndexOf("IsTypeOf(", StringComparison.Ordinal);
                    if (num2 == 0)
                    {
                        aliasedString = aliasedString.Substring("IsTypeOf(".Length, aliasedString.Length - ("IsTypeOf(".Length + 1));
                        aliasedString = this.GetAliasResolvedValue(aliasedString);
                    }
                    else
                    {
                        aliasedString = this.GetAliasResolvedValue(aliasedString);
                    }
                    this.EdmItemCollection.TryGetItem<ComplexType>(aliasedString, out type2);
                    if (type2 == null)
                    {
                        AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Complex_Type_1), aliasedString, StorageMappingErrorCode.InvalidComplexType, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                        index = aliasResolvedAttributeValue.IndexOf(';');
                    }
                    else
                    {
                        if (num2 == 0)
                        {
                            mapping.AddIsOfType(type2);
                        }
                        else
                        {
                            mapping.AddType(type2);
                        }
                        index = aliasResolvedAttributeValue.IndexOf(';');
                    }
                }
                while (aliasResolvedAttributeValue.Length != 0);
            }
            if (nav.MoveToChild(XPathNodeType.Element))
            {
                do
                {
                    EdmType ownerType = mapping.GetOwnerType(GetAttributeValue(nav.Clone(), "Name"));
                    switch (nav.LocalName)
                    {
                        case "ScalarProperty":
                        {
                            StorageScalarPropertyMapping prop = this.LoadScalarPropertyMapping(nav.Clone(), ownerType, tableType);
                            if (prop != null)
                            {
                                mapping.AddProperty(prop);
                            }
                            break;
                        }
                        case "ComplexProperty":
                        {
                            StorageComplexPropertyMapping mapping3 = this.LoadComplexPropertyMapping(nav.Clone(), ownerType, tableType);
                            if (mapping3 != null)
                            {
                                mapping.AddProperty(mapping3);
                            }
                            break;
                        }
                        case "Condition":
                        {
                            StorageConditionPropertyMapping conditionPropertyMap = this.LoadConditionPropertyMapping(nav.Clone(), ownerType, tableType);
                            if (conditionPropertyMap != null)
                            {
                                mapping.AddConditionProperty(conditionPropertyMap);
                            }
                            break;
                        }
                        default:
                            throw System.Data.Entity.Error.NotSupported();
                    }
                }
                while (nav.MoveToNext(XPathNodeType.Element));
            }
            return mapping;
        }

        private StorageConditionPropertyMapping LoadConditionPropertyMapping(XPathNavigator nav, EdmType containerType, System.Data.Metadata.Edm.EntityType tableType)
        {
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
            string identity = this.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            if ((aliasResolvedAttributeValue != null) && (identity != null))
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_Both_Members_0, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            if ((aliasResolvedAttributeValue == null) && (identity == null))
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_Either_Members_0, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            EdmProperty cdmMember = null;
            if ((aliasResolvedAttributeValue != null) && (containerType != null))
            {
                EdmMember member;
                ((StructuralType) containerType).Members.TryGetValue(aliasResolvedAttributeValue, false, out member);
                cdmMember = member as EdmProperty;
            }
            EdmProperty item = null;
            if (identity != null)
            {
                tableType.Properties.TryGetValue(identity, false, out item);
            }
            EdmProperty property3 = (item != null) ? item : cdmMember;
            if (property3 == null)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_InvalidMember_1), (identity != null) ? identity : aliasResolvedAttributeValue, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            bool? isNull = null;
            object obj2 = null;
            string attributeValue = GetAttributeValue(nav.Clone(), "IsNull");
            EdmType edmType = property3.TypeUsage.EdmType;
            if (Helper.IsPrimitiveType(edmType))
            {
                TypeUsage typeUsage;
                if (property3.DeclaringType.GetDataSpace() == DataSpace.SSpace)
                {
                    typeUsage = this.StoreItemCollection.StoreProviderManifest.GetEdmType(property3.TypeUsage);
                    if (typeUsage == null)
                    {
                        AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_ProviderReturnsNullType(property3.Name), StorageMappingErrorCode.MappingStoreProviderReturnsNullEdmType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                        return null;
                    }
                }
                else
                {
                    typeUsage = property3.TypeUsage;
                }
                PrimitiveType type2 = (PrimitiveType) typeUsage.EdmType;
                Type clrEquivalentType = type2.ClrEquivalentType;
                PrimitiveTypeKind primitiveTypeKind = type2.PrimitiveTypeKind;
                if ((attributeValue == null) && !IsTypeSupportedForCondition(primitiveTypeKind))
                {
                    AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_InvalidPrimitiveTypeKind_2), property3.Name, type2.FullName, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    return null;
                }
                if (!TryGetTypedAttributeValue(nav.Clone(), "Value", clrEquivalentType, this.m_sourceLocation, this.m_parsingErrors, out obj2))
                {
                    return null;
                }
            }
            else if (Helper.IsEnumType(edmType))
            {
                obj2 = GetEnumAttributeValue(nav.Clone(), "Value", (EnumType) edmType, this.m_sourceLocation, this.m_parsingErrors);
            }
            else
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_NonScalar_0, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            if ((attributeValue != null) && (obj2 != null))
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_Both_Values_0, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            if ((attributeValue == null) && (obj2 == null))
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_Either_Values_0, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            if (attributeValue != null)
            {
                isNull = new bool?(Convert.ToBoolean(attributeValue, CultureInfo.InvariantCulture));
            }
            if ((item != null) && (item.IsStoreGeneratedComputed || item.IsStoreGeneratedIdentity))
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_Computed), item.Name, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            return new StorageConditionPropertyMapping(cdmMember, item, obj2, isNull);
        }

        private StorageEndPropertyMapping LoadEndPropertyMapping(XPathNavigator nav, AssociationEndMember end, System.Data.Metadata.Edm.EntityType tableType)
        {
            StorageEndPropertyMapping mapping = new StorageEndPropertyMapping(null) {
                EndMember = end
            };
            nav.MoveToChild(XPathNodeType.Element);
            do
            {
                string str;
                if (((str = nav.LocalName) != null) && (str == "ScalarProperty"))
                {
                    RefType edmType = end.TypeUsage.EdmType as RefType;
                    EntityTypeBase elementType = edmType.ElementType;
                    StorageScalarPropertyMapping prop = this.LoadScalarPropertyMapping(nav.Clone(), elementType, tableType);
                    if (prop != null)
                    {
                        mapping.AddProperty(prop);
                    }
                }
            }
            while (nav.MoveToNext(XPathNodeType.Element));
            return mapping;
        }

        private void LoadEntityContainerMapping(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping, System.Data.Metadata.Edm.EntityContainer storageEntityContainerType)
        {
            string str;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            bool flag = false;
            if (!nav.MoveToChild(XPathNodeType.Element))
            {
                goto Label_00A2;
            }
        Label_0015:
            if ((str = nav.LocalName) != null)
            {
                switch (str)
                {
                    case "EntitySetMapping":
                        this.LoadEntitySetMapping(nav.Clone(), entityContainerMapping, storageEntityContainerType);
                        flag = true;
                        goto Label_0096;

                    case "AssociationSetMapping":
                        this.LoadAssociationSetMapping(nav.Clone(), entityContainerMapping, storageEntityContainerType);
                        goto Label_0096;

                    case "FunctionImportMapping":
                        this.LoadFunctionImportMapping(nav.Clone(), entityContainerMapping, storageEntityContainerType);
                        goto Label_0096;
                }
            }
            AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_Set_Mapping_0, StorageMappingErrorCode.SetMappingExpected, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
        Label_0096:
            if (nav.MoveToNext(XPathNodeType.Element))
            {
                goto Label_0015;
            }
        Label_00A2:
            if ((entityContainerMapping.EdmEntityContainer.BaseEntitySets.Count != 0) && !flag)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.ViewGen_Missing_Sets_Mapping_0), entityContainerMapping.EdmEntityContainer.Name, StorageMappingErrorCode.EmptyContainerMapping, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            else
            {
                this.ValidateFunctionMappingClosure(nav.Clone(), entityContainerMapping);
                this.ValidateFunctionAssociationFunctionMappingUnique(nav.Clone(), entityContainerMapping);
                this.ValidateAssociationFunctionMappingConsistent(nav.Clone(), entityContainerMapping);
                this.ValidateQueryViewsClosure(nav.Clone(), entityContainerMapping);
                entityContainerMapping.SourceLocation = this.m_sourceLocation;
            }
        }

        private void LoadEntitySetMapping(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping, System.Data.Metadata.Edm.EntityContainer storageEntityContainerType)
        {
            EntitySet set;
            string str4;
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
            string attributeValue = GetAttributeValue(nav.Clone(), "TypeName");
            string tableName = this.GetAliasResolvedAttributeValue(nav.Clone(), "StoreEntitySet");
            StorageEntitySetMapping entitySetMapping = (StorageEntitySetMapping) entityContainerMapping.GetEntitySetMapping(aliasResolvedAttributeValue);
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            if (entitySetMapping == null)
            {
                if (!entityContainerMapping.EdmEntityContainer.TryGetEntitySetByName(aliasResolvedAttributeValue, false, out set))
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Entity_Set_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidEntitySet, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    return;
                }
                entitySetMapping = new StorageEntitySetMapping(set, entityContainerMapping);
            }
            else
            {
                set = (EntitySet) entitySetMapping.Set;
            }
            entitySetMapping.StartLineNumber = lineInfo.LineNumber;
            entitySetMapping.StartLinePosition = lineInfo.LinePosition;
            entityContainerMapping.AddEntitySetMapping(entitySetMapping);
            if (!string.IsNullOrEmpty(attributeValue))
            {
                this.LoadEntityTypeMapping(nav.Clone(), entitySetMapping, tableName, storageEntityContainerType);
                goto Label_01A3;
            }
            if (!nav.MoveToChild(XPathNodeType.Element))
            {
                goto Label_01A3;
            }
        Label_00D9:
            if ((str4 = nav.LocalName) != null)
            {
                switch (str4)
                {
                    case "EntityTypeMapping":
                        tableName = this.GetAliasResolvedAttributeValue(nav.Clone(), "StoreEntitySet");
                        this.LoadEntityTypeMapping(nav.Clone(), entitySetMapping, tableName, storageEntityContainerType);
                        goto Label_0185;

                    case "QueryView":
                        if (!string.IsNullOrEmpty(tableName))
                        {
                            AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_TableName_QueryView_1), aliasResolvedAttributeValue, StorageMappingErrorCode.TableNameAttributeWithQueryView, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                            return;
                        }
                        if (this.LoadQueryView(nav.Clone(), entitySetMapping))
                        {
                            goto Label_0185;
                        }
                        return;
                }
            }
            AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_TypeMapping_QueryView, StorageMappingErrorCode.InvalidContent, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
        Label_0185:
            if (nav.MoveToNext(XPathNodeType.Element))
            {
                goto Label_00D9;
            }
        Label_01A3:
            this.ValidateAllEntityTypesHaveFunctionMapping(nav.Clone(), entitySetMapping);
            if (entitySetMapping.HasNoContent)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Emtpty_SetMap_1), set.Name, StorageMappingErrorCode.EmptySetMapping, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
        }

        private void LoadEntityTypeFunctionMapping(XPathNavigator nav, StorageEntitySetMapping entitySetMapping, StorageEntityTypeMapping entityTypeMapping)
        {
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            if ((entityTypeMapping.IsOfTypes.Count != 0) || (entityTypeMapping.Types.Count != 1))
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_Multiple_Types_0, StorageMappingErrorCode.InvalidFunctionMappingForMultipleTypes, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            else
            {
                System.Data.Metadata.Edm.EntityType type = (System.Data.Metadata.Edm.EntityType) entityTypeMapping.Types[0];
                if (type.Abstract)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_AbstractEntity_FunctionMapping_1), type.FullName, StorageMappingErrorCode.MappingForAbstractEntityType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                else
                {
                    foreach (StorageEntityTypeFunctionMapping mapping in entitySetMapping.FunctionMappings)
                    {
                        if (mapping.EntityType.Equals(type))
                        {
                            AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_RedundantEntityTypeMapping_1), type.Name, StorageMappingErrorCode.RedundantEntityTypeMappingInFunctionMapping, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                            return;
                        }
                    }
                    FunctionMappingLoader loader = new FunctionMappingLoader(this, entitySetMapping.Set);
                    StorageFunctionMapping deleteFunctionMapping = null;
                    StorageFunctionMapping insertFunctionMapping = null;
                    StorageFunctionMapping updateFunctionMapping = null;
                    if (nav.MoveToChild(XPathNodeType.Element))
                    {
                        do
                        {
                            string localName = nav.LocalName;
                            if (localName != null)
                            {
                                if (localName == "DeleteFunction")
                                {
                                    deleteFunctionMapping = loader.LoadEntityTypeFunctionMapping(nav.Clone(), false, true, type);
                                }
                                else if (localName == "InsertFunction")
                                {
                                    insertFunctionMapping = loader.LoadEntityTypeFunctionMapping(nav.Clone(), true, false, type);
                                }
                                else if (localName == "UpdateFunction")
                                {
                                    updateFunctionMapping = loader.LoadEntityTypeFunctionMapping(nav.Clone(), true, true, type);
                                }
                            }
                        }
                        while (nav.MoveToNext(XPathNodeType.Element));
                    }
                    if (((deleteFunctionMapping != null) && (insertFunctionMapping != null)) && (updateFunctionMapping != null))
                    {
                        Dictionary<AssociationSet, EdmMember> dictionary = new Dictionary<AssociationSet, EdmMember>();
                        foreach (StorageFunctionParameterBinding binding in Helper.Concat<StorageFunctionParameterBinding>(new IEnumerable<StorageFunctionParameterBinding>[] { deleteFunctionMapping.ParameterBindings, insertFunctionMapping.ParameterBindings, updateFunctionMapping.ParameterBindings }))
                        {
                            if (binding.MemberPath.AssociationSetEnd != null)
                            {
                                EdmMember member2;
                                AssociationSet parentAssociationSet = binding.MemberPath.AssociationSetEnd.ParentAssociationSet;
                                EdmMember correspondingAssociationEndMember = binding.MemberPath.AssociationSetEnd.CorrespondingAssociationEndMember;
                                if (dictionary.TryGetValue(parentAssociationSet, out member2) && (member2 != correspondingAssociationEndMember))
                                {
                                    AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_MultipleEndsOfAssociationMapped_3(correspondingAssociationEndMember.Name, member2.Name, parentAssociationSet.Name), StorageMappingErrorCode.InvalidFunctionMappingMultipleEndsOfAssociationMapped, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                                    return;
                                }
                                dictionary[parentAssociationSet] = correspondingAssociationEndMember;
                            }
                        }
                        StorageEntityTypeFunctionMapping functionMapping = new StorageEntityTypeFunctionMapping(type, deleteFunctionMapping, insertFunctionMapping, updateFunctionMapping);
                        entitySetMapping.AddFunctionMapping(functionMapping);
                    }
                }
            }
        }

        private void LoadEntityTypeMapping(XPathNavigator nav, StorageEntitySetMapping entitySetMapping, string tableName, System.Data.Metadata.Edm.EntityContainer storageEntityContainerType)
        {
            System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType> set;
            System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType> set2;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            StorageEntityTypeMapping entityTypeMapping = new StorageEntityTypeMapping(entitySetMapping);
            System.Data.Metadata.Edm.EntityType rootEntityType = (System.Data.Metadata.Edm.EntityType) entitySetMapping.Set.ElementType;
            if (this.TryParseEntityTypeAttribute(nav.Clone(), rootEntityType, e => System.Data.Entity.Strings.Mapping_InvalidContent_Entity_Type_For_Entity_Set_3(e.FullName, rootEntityType.FullName, entitySetMapping.Set.Name), out set2, out set))
            {
                foreach (System.Data.Metadata.Edm.EntityType type in set)
                {
                    entityTypeMapping.AddType(type);
                }
                foreach (System.Data.Metadata.Edm.EntityType type2 in set2)
                {
                    entityTypeMapping.AddIsOfType(type2);
                }
                if (string.IsNullOrEmpty(tableName))
                {
                    if (!nav.MoveToChild(XPathNodeType.Element))
                    {
                        return;
                    }
                    do
                    {
                        if (nav.LocalName == "ModificationFunctionMapping")
                        {
                            this.LoadEntityTypeFunctionMapping(nav.Clone(), entitySetMapping, entityTypeMapping);
                        }
                        else if (nav.LocalName != "MappingFragment")
                        {
                            AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_Table_Expected_0, StorageMappingErrorCode.TableMappingFragmentExpected, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                        }
                        else
                        {
                            tableName = this.GetAliasResolvedAttributeValue(nav.Clone(), "StoreEntitySet");
                            StorageMappingFragment fragment = this.LoadMappingFragment(nav.Clone(), entityTypeMapping, tableName, storageEntityContainerType);
                            if (fragment != null)
                            {
                                entityTypeMapping.AddFragment(fragment);
                            }
                        }
                    }
                    while (nav.MoveToNext(XPathNodeType.Element));
                }
                else
                {
                    if (nav.LocalName == "ModificationFunctionMapping")
                    {
                        AddToSchemaErrors(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_In_Table_Context_0, StorageMappingErrorCode.InvalidTableNameAttributeWithFunctionMapping, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    }
                    StorageMappingFragment fragment2 = this.LoadMappingFragment(nav.Clone(), entityTypeMapping, tableName, storageEntityContainerType);
                    if (fragment2 != null)
                    {
                        entityTypeMapping.AddFragment(fragment2);
                    }
                }
                entitySetMapping.AddTypeMapping(entityTypeMapping);
            }
        }

        private void LoadFunctionImportEntityTypeMappingCondition(XPathNavigator nav, List<FunctionImportEntityTypeMappingCondition> conditions)
        {
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
            string str2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "Value");
            string str3 = this.GetAliasResolvedAttributeValue(nav.Clone(), "IsNull");
            if ((str3 != null) && (str2 != null))
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_Both_Values_0, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            else if ((str3 == null) && (str2 == null))
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_ConditionMapping_Either_Values_0, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            else if (str3 != null)
            {
                bool isNull = Convert.ToBoolean(str3, CultureInfo.InvariantCulture);
                conditions.Add(new FunctionImportEntityTypeMappingConditionIsNull(aliasResolvedAttributeValue, isNull));
            }
            else
            {
                XPathNavigator columnValue = nav.Clone();
                columnValue.MoveToAttribute("Value", string.Empty);
                conditions.Add(new FunctionImportEntityTypeMappingConditionValue(aliasResolvedAttributeValue, columnValue));
            }
        }

        private void LoadFunctionImportMapping(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping, System.Data.Metadata.Edm.EntityContainer storageEntityContainerType)
        {
            EdmFunction function;
            EdmFunction function2;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav.Clone();
            if (this.TryGetFunctionImportStoreFunction(nav, out function) && this.TryGetFunctionImportModelFunction(nav, entityContainerMapping, out function2))
            {
                KeyToListMap<System.Data.Metadata.Edm.EntityType, IXmlLineInfo> map;
                KeyToListMap<System.Data.Metadata.Edm.EntityType, IXmlLineInfo> map2;
                System.Data.Metadata.Edm.EntityType type;
                this.ValidateFunctionImportMappingParameters(nav, function, function2);
                List<FunctionImportEntityTypeMapping> entityTypeMappings = new List<FunctionImportEntityTypeMapping>();
                if ((nav.MoveToChild(XPathNodeType.Element) && (nav.LocalName == "ResultMapping")) && nav.MoveToChild(XPathNodeType.Element))
                {
                    do
                    {
                        FunctionImportEntityTypeMapping mapping;
                        if ((nav.LocalName == "EntityTypeMapping") && this.TryLoadFunctionImportEntityTypeMapping(nav.Clone(), function, function2, out mapping))
                        {
                            entityTypeMappings.Add(mapping);
                        }
                    }
                    while (nav.MoveToNext(XPathNodeType.Element));
                }
                FunctionImportMapping targetFunction = new FunctionImportMapping(function, function2, entityTypeMappings, this.EdmItemCollection);
                entityContainerMapping.AddFunctionImportMapping(function2, targetFunction);
                targetFunction.GetUnreachableTypes(this.EdmItemCollection, out map, out map2);
                foreach (KeyValuePair<System.Data.Metadata.Edm.EntityType, List<IXmlLineInfo>> pair in map.KeyValuePairs)
                {
                    string str = StringUtil.ToCommaSeparatedString(from li in pair.Value select li.LineNumber);
                    AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_UnreachableType(pair.Key.FullName, str), StorageMappingErrorCode.MappingFunctionImportAmbiguousTypeConditions, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                foreach (KeyValuePair<System.Data.Metadata.Edm.EntityType, List<IXmlLineInfo>> pair2 in map2.KeyValuePairs)
                {
                    string str2 = StringUtil.ToCommaSeparatedString(from li in pair2.Value select li.LineNumber);
                    AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_UnreachableIsTypeOf("IsTypeOf(" + pair2.Key.FullName + ")", str2), StorageMappingErrorCode.MappingFunctionImportAmbiguousTypeConditions, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                if ((MetadataHelper.TryGetFunctionImportReturnEntityType(function2, out type) && type.Abstract) && (targetFunction.NormalizedEntityTypeMappings.Count == 0))
                {
                    AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_ImplicitMappingForAbstractReturnType_FunctionMapping_1), type.FullName, function2.FullName, StorageMappingErrorCode.MappingForAbstractEntityType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
            }
        }

        private StorageEntityContainerMapping LoadMappingChildNodes(XPathNavigator nav)
        {
            bool flag;
            StorageEntityContainerMapping mapping;
            System.Data.Metadata.Edm.EntityContainer edmEntityContainer;
            System.Data.Metadata.Edm.EntityContainer storageEntityContainer;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            if (nav.MoveToChild("Alias", "urn:schemas-microsoft-com:windows:storage:mapping:CS"))
            {
                do
                {
                    this.m_alias.Add(GetAttributeValue(nav.Clone(), "Key"), GetAttributeValue(nav.Clone(), "Value"));
                }
                while (nav.MoveToNext("Alias", "urn:schemas-microsoft-com:windows:storage:mapping:CS"));
                flag = nav.MoveToNext(XPathNodeType.Element);
            }
            else
            {
                flag = nav.MoveToChild(XPathNodeType.Element);
            }
            if (!flag)
            {
                return null;
            }
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "CdmEntityContainer");
            string str2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "StorageEntityContainer");
            if (this.m_storageMappingItemCollection.TryGetItem<StorageEntityContainerMapping>(aliasResolvedAttributeValue, out mapping))
            {
                edmEntityContainer = mapping.EdmEntityContainer;
                storageEntityContainer = mapping.StorageEntityContainer;
                if (str2 != storageEntityContainer.Name)
                {
                    AddToSchemaErrors(System.Data.Entity.Strings.StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping(str2, storageEntityContainer.Name, edmEntityContainer.Name), StorageMappingErrorCode.StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    return null;
                }
            }
            else
            {
                if (this.m_storageMappingItemCollection.ContainsStorageEntityContainer(str2))
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_AlreadyMapped_StorageEntityContainer_1), str2, StorageMappingErrorCode.AlreadyMappedStorageEntityContainer, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    return null;
                }
                this.EdmItemCollection.TryGetEntityContainer(aliasResolvedAttributeValue, out edmEntityContainer);
                if (edmEntityContainer == null)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_EntityContainer_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidEntityContainer, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                this.StoreItemCollection.TryGetEntityContainer(str2, out storageEntityContainer);
                if (storageEntityContainer == null)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_StorageEntityContainer_1), str2, StorageMappingErrorCode.InvalidEntityContainer, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                if ((edmEntityContainer == null) || (storageEntityContainer == null))
                {
                    return null;
                }
                mapping = new StorageEntityContainerMapping(edmEntityContainer, storageEntityContainer, this.m_storageMappingItemCollection) {
                    StartLineNumber = lineInfo.LineNumber,
                    StartLinePosition = lineInfo.LinePosition
                };
            }
            this.LoadEntityContainerMapping(nav.Clone(), mapping, storageEntityContainer);
            return mapping;
        }

        private StorageMappingFragment LoadMappingFragment(XPathNavigator nav, StorageEntityTypeMapping typeMapping, string tableName, System.Data.Metadata.Edm.EntityContainer storageEntityContainerType)
        {
            EntitySet set;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            if (typeMapping.SetMapping.QueryView != null)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_QueryView_PropertyMaps_1), typeMapping.SetMapping.Set.Name, StorageMappingErrorCode.PropertyMapsWithQueryView, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            storageEntityContainerType.TryGetEntitySetByName(tableName, false, out set);
            if (set == null)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Table_1), tableName, StorageMappingErrorCode.InvalidTable, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return null;
            }
            System.Data.Metadata.Edm.EntityType elementType = set.ElementType;
            StorageMappingFragment fragment = new StorageMappingFragment(set, typeMapping) {
                StartLineNumber = lineInfo.LineNumber,
                StartLinePosition = lineInfo.LinePosition
            };
            if (nav.MoveToChild(XPathNodeType.Element))
            {
                do
                {
                    EdmType containerType = null;
                    string attributeValue = GetAttributeValue(nav.Clone(), "Name");
                    if (attributeValue != null)
                    {
                        containerType = typeMapping.GetContainerType(attributeValue);
                    }
                    switch (nav.LocalName)
                    {
                        case "ScalarProperty":
                        {
                            StorageScalarPropertyMapping prop = this.LoadScalarPropertyMapping(nav.Clone(), containerType, elementType);
                            if (prop != null)
                            {
                                fragment.AddProperty(prop);
                            }
                            break;
                        }
                        case "ComplexProperty":
                        {
                            StorageComplexPropertyMapping mapping2 = this.LoadComplexPropertyMapping(nav.Clone(), containerType, elementType);
                            if (mapping2 != null)
                            {
                                fragment.AddProperty(mapping2);
                            }
                            break;
                        }
                        case "Condition":
                        {
                            StorageConditionPropertyMapping conditionPropertyMap = this.LoadConditionPropertyMapping(nav.Clone(), containerType, elementType);
                            if ((conditionPropertyMap == null) || fragment.AddConditionProperty(conditionPropertyMap))
                            {
                                break;
                            }
                            EdmProperty property = (conditionPropertyMap.EdmProperty != null) ? conditionPropertyMap.EdmProperty : conditionPropertyMap.ColumnProperty;
                            AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Duplicate_Condition_Member_1), property.Name, StorageMappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                            return null;
                        }
                        default:
                            AddToSchemaErrors(System.Data.Entity.Strings.Mapping_InvalidContent_General_0, StorageMappingErrorCode.InvalidContent, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                            break;
                    }
                }
                while (nav.MoveToNext(XPathNodeType.Element));
            }
            fragment.EndLineNumber = lineInfo.LineNumber;
            fragment.EndLinePosition = lineInfo.LinePosition;
            nav.MoveToChild(XPathNodeType.Element);
            return fragment;
        }

        internal StorageEntityContainerMapping LoadMappingItems(XmlReader innerReader)
        {
            XmlReader schemaValidatingReader = this.GetSchemaValidatingReader(innerReader);
            try
            {
                XPathDocument document = new XPathDocument(schemaValidatingReader);
                if ((this.m_parsingErrors.Count != 0) && !MetadataHelper.CheckIfAllErrorsAreWarnings(this.m_parsingErrors))
                {
                    return null;
                }
                XPathNavigator nav = document.CreateNavigator();
                return this.LoadMappingItems(nav);
            }
            catch (XmlException exception)
            {
                EdmSchemaError item = new EdmSchemaError(System.Data.Entity.Strings.Mapping_InvalidMappingSchema_Parsing_1(exception.Message), 0x7e8, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, exception.LineNumber, exception.LinePosition);
                this.m_parsingErrors.Add(item);
            }
            return null;
        }

        internal StorageEntityContainerMapping LoadMappingItems(XPathNavigator nav)
        {
            if (!nav.MoveToChild("Mapping", "urn:schemas-microsoft-com:windows:storage:mapping:CS") || (nav.NodeType != XPathNodeType.Element))
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_Invalid_CSRootElementMissing_0, StorageMappingErrorCode.RootMappingElementMissing, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                return null;
            }
            StorageEntityContainerMapping mapping = this.LoadMappingChildNodes(nav.Clone());
            if ((this.m_parsingErrors.Count != 0) && !MetadataHelper.CheckIfAllErrorsAreWarnings(this.m_parsingErrors))
            {
                mapping = null;
            }
            return mapping;
        }

        private bool LoadQueryView(XPathNavigator nav, StorageSetMapping setMapping)
        {
            System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType> set;
            System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType> set2;
            System.Data.Metadata.Edm.EntityType type;
            string str = nav.Value;
            bool second = false;
            string attributeValue = GetAttributeValue(nav.Clone(), "TypeName");
            if (attributeValue != null)
            {
                attributeValue = attributeValue.Trim();
            }
            if (setMapping.QueryView == null)
            {
                if (attributeValue != null)
                {
                    AddToSchemaErrorsWithMemberInfo(val => System.Data.Entity.Strings.Mapping_TypeName_For_First_QueryView, setMapping.Set.Name, StorageMappingErrorCode.TypeNameForFirstQueryView, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                    return false;
                }
                if (string.IsNullOrEmpty(str))
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Empty_QueryView_1), setMapping.Set.Name, StorageMappingErrorCode.EmptyQueryView, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                    return false;
                }
                setMapping.QueryView = str;
                this.m_hasQueryViews = true;
                return true;
            }
            if ((attributeValue == null) || (attributeValue.Trim().Length == 0))
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_QueryView_TypeName_Not_Defined), setMapping.Set.Name, StorageMappingErrorCode.NoTypeNameForTypeSpecificQueryView, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                return false;
            }
            System.Data.Metadata.Edm.EntityType rootEntityType = (System.Data.Metadata.Edm.EntityType) setMapping.Set.ElementType;
            if (this.TryParseEntityTypeAttribute(nav.Clone(), rootEntityType, e => System.Data.Entity.Strings.Mapping_InvalidContent_Entity_Type_For_Entity_Set_3(e.FullName, rootEntityType.FullName, setMapping.Set.Name), out set2, out set))
            {
                if (set2.Count == 1)
                {
                    type = set2.First<System.Data.Metadata.Edm.EntityType>();
                    second = true;
                    goto Label_01F3;
                }
                if (set.Count == 1)
                {
                    type = set.First<System.Data.Metadata.Edm.EntityType>();
                    second = false;
                    goto Label_01F3;
                }
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_QueryViewMultipleTypeInTypeName), setMapping.Set.ToString(), StorageMappingErrorCode.TypeNameContainsMultipleTypesForQueryView, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
            }
            return false;
        Label_01F3:
            if (second && setMapping.Set.ElementType.EdmEquals(type))
            {
                AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_QueryView_For_Base_Type), type.ToString(), setMapping.Set.ToString(), StorageMappingErrorCode.IsTypeOfQueryViewForBaseType, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                return false;
            }
            if (string.IsNullOrEmpty(str))
            {
                if (second)
                {
                    AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Empty_QueryView_OfType_2), type.Name, setMapping.Set.Name, StorageMappingErrorCode.EmptyQueryView, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                    return false;
                }
                AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Empty_QueryView_OfTypeOnly_2), setMapping.Set.Name, type.Name, StorageMappingErrorCode.EmptyQueryView, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                return false;
            }
            Pair<EntitySetBase, Pair<EntityTypeBase, bool>> key = new Pair<EntitySetBase, Pair<EntityTypeBase, bool>>(setMapping.Set, new Pair<EntityTypeBase, bool>(type, second));
            if (setMapping.ContainsTypeSpecificQueryView(key))
            {
                EdmSchemaError item = null;
                if (second)
                {
                    item = new EdmSchemaError(System.Data.Entity.Strings.Mapping_QueryView_Duplicate_OfType(setMapping.Set, type), 0x822, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, ((IXmlLineInfo) nav).LineNumber, ((IXmlLineInfo) nav).LinePosition);
                }
                else
                {
                    item = new EdmSchemaError(System.Data.Entity.Strings.Mapping_QueryView_Duplicate_OfTypeOnly(setMapping.Set, type), 0x822, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, ((IXmlLineInfo) nav).LineNumber, ((IXmlLineInfo) nav).LinePosition);
                }
                this.m_parsingErrors.Add(item);
                return false;
            }
            setMapping.AddTypeSpecificQueryView(key, str);
            return true;
        }

        private StorageScalarPropertyMapping LoadScalarPropertyMapping(XPathNavigator nav, EdmType containerType, System.Data.Metadata.Edm.EntityType tableType)
        {
            EdmProperty property2;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
            EdmProperty item = null;
            if (!string.IsNullOrEmpty(aliasResolvedAttributeValue) && ((containerType == null) || !Helper.IsCollectionType(containerType)))
            {
                if (containerType != null)
                {
                    if (Helper.IsRefType(containerType))
                    {
                        RefType type = (RefType) containerType;
                        ((System.Data.Metadata.Edm.EntityType) type.ElementType).Properties.TryGetValue(aliasResolvedAttributeValue, false, out item);
                    }
                    else
                    {
                        EdmMember member;
                        (containerType as StructuralType).Members.TryGetValue(aliasResolvedAttributeValue, false, out member);
                        item = member as EdmProperty;
                    }
                }
                if (item == null)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Cdm_Member_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
            }
            string identity = this.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
            tableType.Properties.TryGetValue(identity, false, out property2);
            if (property2 == null)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Column_1), identity, StorageMappingErrorCode.InvalidStorageMember, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            if ((item == null) || (property2 == null))
            {
                return null;
            }
            if (!Helper.IsPrimitiveType(item.TypeUsage.EdmType))
            {
                EdmSchemaError error = new EdmSchemaError(System.Data.Entity.Strings.Mapping_Invalid_CSide_ScalarProperty_1(item.Name), 0x825, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition);
                this.m_parsingErrors.Add(error);
                return null;
            }
            this.ValidateAndUpdateScalarMemberMapping(item, property2, lineInfo);
            return new StorageScalarPropertyMapping(item, property2);
        }

        private bool TryGetFunctionImportModelFunction(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping, out EdmFunction functionImport)
        {
            FunctionImportMapping mapping;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "FunctionImportName");
            System.Data.Metadata.Edm.EntityContainer edmEntityContainer = entityContainerMapping.EdmEntityContainer;
            functionImport = null;
            foreach (EdmFunction function in edmEntityContainer.FunctionImports)
            {
                if (function.Name == aliasResolvedAttributeValue)
                {
                    functionImport = function;
                    break;
                }
            }
            if (functionImport == null)
            {
                AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_FunctionImportDoesNotExist(aliasResolvedAttributeValue, entityContainerMapping.EdmEntityContainer.Name), StorageMappingErrorCode.MappingFunctionImportFunctionImportDoesNotExist, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return false;
            }
            if (entityContainerMapping.TryGetFunctionImportMapping(functionImport, out mapping))
            {
                AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_FunctionImportMappedMultipleTimes(aliasResolvedAttributeValue), StorageMappingErrorCode.MappingFunctionImportFunctionImportMappedMultipleTimes, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return false;
            }
            return true;
        }

        private bool TryGetFunctionImportStoreFunction(XPathNavigator nav, out EdmFunction targetFunction)
        {
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            targetFunction = null;
            string aliasResolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "FunctionName");
            ReadOnlyCollection<EdmFunction> functions = this.StoreItemCollection.GetFunctions(aliasResolvedAttributeValue);
            if (functions.Count == 0)
            {
                AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_StoreFunctionDoesNotExist(aliasResolvedAttributeValue), StorageMappingErrorCode.MappingFunctionImportStoreFunctionDoesNotExist, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return false;
            }
            if (1 < functions.Count)
            {
                AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_StoreFunctionAmbiguous(aliasResolvedAttributeValue), StorageMappingErrorCode.MappingFunctionImportStoreFunctionAmbiguous, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return false;
            }
            targetFunction = functions.Single<EdmFunction>();
            if (targetFunction.IsComposableAttribute)
            {
                AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_TargetFunctionMustBeComposable(targetFunction.FullName), StorageMappingErrorCode.MappingFunctionImportTargetFunctionMustBeComposable, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                return false;
            }
            return true;
        }

        private static bool TryGetTypedAttributeValue(XPathNavigator nav, string attributeName, Type clrType, string sourceLocation, IList<EdmSchemaError> parsingErrors, out object value)
        {
            value = null;
            try
            {
                value = Helper.GetTypedAttributeValue(nav, attributeName, clrType);
            }
            catch (FormatException)
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_ConditionValueTypeMismatch_0, StorageMappingErrorCode.ConditionError, sourceLocation, (IXmlLineInfo) nav, parsingErrors);
                return false;
            }
            return true;
        }

        private bool TryLoadFunctionImportEntityTypeMapping(XPathNavigator nav, EdmFunction targetFunction, EdmFunction functionImport, out FunctionImportEntityTypeMapping typeMapping)
        {
            System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType> set;
            System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType> set2;
            typeMapping = null;
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav.Clone();
            if (functionImport.EntitySet == null)
            {
                AddToSchemaErrors(System.Data.Entity.Strings.Mapping_FunctionImport_EntityTypeMappingForFunctionNotReturningEntitySet("EntityTypeMapping", functionImport.FullName), StorageMappingErrorCode.MappingFunctionImportEntityTypeMappingForFunctionNotReturningEntitySet, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
            }
            System.Data.Metadata.Edm.EntityType rootEntityType;
            this.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
            if (!MetadataHelper.TryGetFunctionImportReturnEntityType(functionImport, out rootEntityType) || !this.TryParseEntityTypeAttribute(nav.Clone(), rootEntityType, e => System.Data.Entity.Strings.Mapping_FunctionImport_InvalidContentEntityTypeForEntitySet(e.FullName, rootEntityType.FullName, functionImport.EntitySet.Name, functionImport.FullName), out set, out set2))
            {
                return false;
            }
            List<FunctionImportEntityTypeMappingCondition> conditions = new List<FunctionImportEntityTypeMappingCondition>();
            if (nav.MoveToChild(XPathNodeType.Element))
            {
                do
                {
                    if (nav.LocalName == "Condition")
                    {
                        this.LoadFunctionImportEntityTypeMappingCondition(nav, conditions);
                    }
                }
                while (nav.MoveToNext(XPathNodeType.Element));
            }
            HashSet<string> set3 = new HashSet<string>();
            foreach (FunctionImportEntityTypeMappingCondition condition in conditions)
            {
                if (!set3.Add(condition.ColumnName))
                {
                    AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_MultipleConditionsOnSingleColumn(condition.ColumnName), StorageMappingErrorCode.MappingFunctionMultipleTypeConditionsForOneColumn, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    return false;
                }
            }
            typeMapping = new FunctionImportEntityTypeMapping(set, set2, conditions, lineInfo);
            return true;
        }

        private bool TryParseEntityTypeAttribute(XPathNavigator nav, System.Data.Metadata.Edm.EntityType rootEntityType, Func<System.Data.Metadata.Edm.EntityType, string> typeNotAssignableMessage, out System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType> isOfTypeEntityTypes, out System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType> entityTypes)
        {
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            string attributeValue = GetAttributeValue(nav.Clone(), "TypeName");
            isOfTypeEntityTypes = new System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType>();
            entityTypes = new System.Data.Common.Utils.Set<System.Data.Metadata.Edm.EntityType>();
            foreach (string str2 in from s in attributeValue.Split(new char[] { ';' }) select s.Trim())
            {
                string aliasResolvedValue;
                System.Data.Metadata.Edm.EntityType type;
                bool flag = str2.StartsWith("IsTypeOf(", StringComparison.Ordinal);
                if (flag)
                {
                    if (!str2.EndsWith(")", StringComparison.Ordinal))
                    {
                        AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_InvalidContent_IsTypeOfNotTerminated, StorageMappingErrorCode.InvalidEntityType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                        return false;
                    }
                    aliasResolvedValue = str2.Substring("IsTypeOf(".Length);
                    aliasResolvedValue = aliasResolvedValue.Substring(0, aliasResolvedValue.Length - ")".Length).Trim();
                }
                else
                {
                    aliasResolvedValue = str2;
                }
                aliasResolvedValue = this.GetAliasResolvedValue(aliasResolvedValue);
                if (!this.EdmItemCollection.TryGetItem<System.Data.Metadata.Edm.EntityType>(aliasResolvedValue, out type))
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_Entity_Type_1), aliasResolvedValue, StorageMappingErrorCode.InvalidEntityType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    return false;
                }
                if (!Helper.IsAssignableFrom(rootEntityType, type))
                {
                    AddToSchemaErrorWithMessage(typeNotAssignableMessage(type), StorageMappingErrorCode.InvalidEntityType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    return false;
                }
                if (type.Abstract)
                {
                    if (flag)
                    {
                        if (!MetadataHelper.GetTypeAndSubtypesOf(type, this.EdmItemCollection, false).GetEnumerator().MoveNext())
                        {
                            AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_AbstractEntity_IsOfType_1), type.FullName, StorageMappingErrorCode.MappingForAbstractEntityType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                            return false;
                        }
                    }
                    else
                    {
                        AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_InvalidContent_AbstractEntity_Type_1), type.FullName, StorageMappingErrorCode.MappingForAbstractEntityType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                        return false;
                    }
                }
                if (flag)
                {
                    isOfTypeEntityTypes.Add(type);
                }
                else
                {
                    entityTypes.Add(type);
                }
            }
            return true;
        }

        private void ValidateAllEntityTypesHaveFunctionMapping(XPathNavigator nav, StorageEntitySetMapping setMapping)
        {
            System.Data.Common.Utils.Set<EdmType> other = new System.Data.Common.Utils.Set<EdmType>();
            foreach (StorageEntityTypeFunctionMapping mapping in setMapping.FunctionMappings)
            {
                other.Add(mapping.EntityType);
            }
            if (0 < other.Count)
            {
                System.Data.Common.Utils.Set<EdmType> list = new System.Data.Common.Utils.Set<EdmType>(MetadataHelper.GetTypeAndSubtypesOf(setMapping.Set.ElementType, this.EdmItemCollection, false));
                list.Subtract(other);
                System.Data.Common.Utils.Set<EdmType> set3 = new System.Data.Common.Utils.Set<EdmType>();
                foreach (System.Data.Metadata.Edm.EntityType type in list)
                {
                    if (type.Abstract)
                    {
                        set3.Add(type);
                    }
                }
                list.Subtract(set3);
                if (0 < list.Count)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_MissingEntityType_1), StringUtil.ToCommaSeparatedString(list), StorageMappingErrorCode.MissingFunctionMappingForEntityType, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                }
            }
        }

        private void ValidateAndUpdateScalarMemberMapping(EdmProperty member, EdmProperty columnMember, IXmlLineInfo lineInfo)
        {
            KeyValuePair<TypeUsage, TypeUsage> pair;
            if (!this.m_scalarMemberMappings.TryGetValue(member, out pair))
            {
                int count = this.m_parsingErrors.Count;
                TypeUsage key = Helper.ValidateAndConvertTypeUsage(member, columnMember, lineInfo, this.m_sourceLocation, this.m_parsingErrors, this.StoreItemCollection);
                if (key != null)
                {
                    this.m_scalarMemberMappings.Add(member, new KeyValuePair<TypeUsage, TypeUsage>(key, columnMember.TypeUsage));
                }
                else if (count == this.m_parsingErrors.Count)
                {
                    EdmSchemaError item = new EdmSchemaError(System.Data.Entity.Strings.Mapping_Invalid_Member_Mapping_6(member.TypeUsage.EdmType + this.GetFacetsForDisplay(member.TypeUsage), member.Name, member.DeclaringType.FullName, columnMember.TypeUsage.EdmType + this.GetFacetsForDisplay(columnMember.TypeUsage), columnMember.Name, columnMember.DeclaringType.FullName), 0x7e3, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition);
                    this.m_parsingErrors.Add(item);
                }
            }
            else
            {
                TypeUsage usage2 = pair.Value;
                TypeUsage modelTypeUsage = columnMember.TypeUsage.GetModelTypeUsage();
                if (!object.ReferenceEquals(columnMember.TypeUsage.EdmType, usage2.EdmType))
                {
                    EdmSchemaError error2 = new EdmSchemaError(System.Data.Entity.Strings.Mapping_StoreTypeMismatch_ScalarPropertyMapping_2(member.Name, usage2.EdmType.Name), 0x7f7, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition);
                    this.m_parsingErrors.Add(error2);
                }
                else if (!TypeSemantics.IsSubTypeOf(member.TypeUsage, modelTypeUsage))
                {
                    EdmSchemaError error3 = new EdmSchemaError(System.Data.Entity.Strings.Mapping_Invalid_Member_Mapping_6(member.TypeUsage.EdmType + this.GetFacetsForDisplay(member.TypeUsage), member.Name, member.DeclaringType.FullName, columnMember.TypeUsage.EdmType + this.GetFacetsForDisplay(columnMember.TypeUsage), columnMember.Name, columnMember.DeclaringType.FullName), 0x7e3, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition);
                    this.m_parsingErrors.Add(error3);
                }
            }
        }

        private void ValidateAssociationFunctionMappingConsistent(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping)
        {
            foreach (StorageEntitySetMapping mapping in entityContainerMapping.EntitySetMaps)
            {
                if (mapping.FunctionMappings.Count > 0)
                {
                    System.Data.Common.Utils.Set<AssociationSetEnd> expectedEnds = new System.Data.Common.Utils.Set<AssociationSetEnd>(mapping.ImplicitlyMappedAssociationSetEnds).MakeReadOnly();
                    foreach (StorageEntityTypeFunctionMapping mapping2 in mapping.FunctionMappings)
                    {
                        this.ValidateAssociationFunctionMappingConsistent(nav, mapping, mapping2, mapping2.DeleteFunctionMapping, expectedEnds, "DeleteFunction");
                        this.ValidateAssociationFunctionMappingConsistent(nav, mapping, mapping2, mapping2.InsertFunctionMapping, expectedEnds, "InsertFunction");
                        this.ValidateAssociationFunctionMappingConsistent(nav, mapping, mapping2, mapping2.UpdateFunctionMapping, expectedEnds, "UpdateFunction");
                    }
                }
            }
        }

        private void ValidateAssociationFunctionMappingConsistent(XPathNavigator nav, StorageEntitySetMapping entitySetMapping, StorageEntityTypeFunctionMapping entityTypeMapping, StorageFunctionMapping functionMapping, System.Data.Common.Utils.Set<AssociationSetEnd> expectedEnds, string elementName)
        {
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            System.Data.Common.Utils.Set<AssociationSetEnd> set = new System.Data.Common.Utils.Set<AssociationSetEnd>(functionMapping.GetReferencedAssociationSetEnds((EntitySet) entitySetMapping.Set));
            set.MakeReadOnly();
            foreach (AssociationSetEnd end in expectedEnds)
            {
                if (MetadataHelper.IsAssociationValidForEntityType(end, entityTypeMapping.EntityType) && !set.Contains(end))
                {
                    AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationSetNotMappedForOperation_4(entitySetMapping.Set.Name, end.ParentAssociationSet.Name, elementName, entityTypeMapping.EntityType.FullName), StorageMappingErrorCode.InvalidFunctionMappingAssociationSetNotMappedForOperation, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
            }
            foreach (AssociationSetEnd end2 in set)
            {
                if (!MetadataHelper.IsAssociationValidForEntityType(end2, entityTypeMapping.EntityType))
                {
                    AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationEndMappingInvalidForEntityType_3(entityTypeMapping.EntityType.FullName, end2.ParentAssociationSet.Name, MetadataHelper.GetEntityTypeForEnd(MetadataHelper.GetOppositeEnd(end2).CorrespondingAssociationEndMember).FullName), StorageMappingErrorCode.InvalidFunctionMappingAssociationEndMappingInvalidForEntityType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
            }
        }

        private static void ValidateClosureAmongSets(StorageEntityContainerMapping entityContainerMapping, System.Data.Common.Utils.Set<EntitySetBase> sets, System.Data.Common.Utils.Set<EntitySetBase> additionalSetsInClosure)
        {
            bool flag;
            do
            {
                flag = false;
                List<EntitySetBase> elements = new List<EntitySetBase>();
                foreach (EntitySetBase base2 in additionalSetsInClosure)
                {
                    if (BuiltInTypeKind.AssociationSet == base2.BuiltInTypeKind)
                    {
                        AssociationSet set = (AssociationSet) base2;
                        foreach (AssociationSetEnd end in set.AssociationSetEnds)
                        {
                            if (!additionalSetsInClosure.Contains(end.EntitySet))
                            {
                                elements.Add(end.EntitySet);
                            }
                        }
                    }
                }
                foreach (EntitySetBase base3 in entityContainerMapping.EdmEntityContainer.BaseEntitySets)
                {
                    if (BuiltInTypeKind.AssociationSet == base3.BuiltInTypeKind)
                    {
                        AssociationSet element = (AssociationSet) base3;
                        if (!additionalSetsInClosure.Contains(element))
                        {
                            foreach (AssociationSetEnd end2 in element.AssociationSetEnds)
                            {
                                if (additionalSetsInClosure.Contains(end2.EntitySet))
                                {
                                    elements.Add(element);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (0 < elements.Count)
                {
                    flag = true;
                    additionalSetsInClosure.AddRange(elements);
                }
            }
            while (flag);
            additionalSetsInClosure.Subtract(sets);
        }

        private void ValidateFunctionAssociationFunctionMappingUnique(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping)
        {
            Dictionary<EntitySetBase, int> counts = new Dictionary<EntitySetBase, int>();
            foreach (StorageEntitySetMapping mapping in entityContainerMapping.EntitySetMaps)
            {
                if (mapping.FunctionMappings.Count > 0)
                {
                    System.Data.Common.Utils.Set<EntitySetBase> set = new System.Data.Common.Utils.Set<EntitySetBase>();
                    foreach (AssociationSetEnd end in mapping.ImplicitlyMappedAssociationSetEnds)
                    {
                        set.Add(end.ParentAssociationSet);
                    }
                    foreach (EntitySetBase base2 in set)
                    {
                        IncrementCount<EntitySetBase>(counts, base2);
                    }
                }
            }
            foreach (StorageAssociationSetMapping mapping2 in entityContainerMapping.RelationshipSetMaps)
            {
                if (mapping2.FunctionMapping != null)
                {
                    IncrementCount<EntitySetBase>(counts, mapping2.Set);
                }
            }
            List<string> list = new List<string>();
            foreach (KeyValuePair<EntitySetBase, int> pair in counts)
            {
                if (pair.Value > 1)
                {
                    list.Add(pair.Key.Name);
                }
            }
            if (0 < list.Count)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationSetAmbiguous_1), StringUtil.ToCommaSeparatedString(list), StorageMappingErrorCode.AmbiguousFunctionMappingForAssociationSet, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
            }
        }

        private void ValidateFunctionImportMappingParameters(XPathNavigator nav, EdmFunction targetFunction, EdmFunction functionImport)
        {
            IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
            foreach (FunctionParameter parameter in targetFunction.Parameters)
            {
                FunctionParameter parameter2;
                if (!functionImport.Parameters.TryGetValue(parameter.Name, false, out parameter2))
                {
                    AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_TargetParameterHasNoCorrespondingImportParameter(parameter.Name), StorageMappingErrorCode.MappingFunctionImportTargetParameterHasNoCorrespondingImportParameter, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
                else
                {
                    if (parameter.Mode != parameter2.Mode)
                    {
                        AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_IncompatibleParameterMode(parameter.Name, parameter.Mode, parameter2.Mode), StorageMappingErrorCode.MappingFunctionImportIncompatibleParameterMode, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    }
                    PrimitiveType edmType = (PrimitiveType) parameter2.TypeUsage.EdmType;
                    PrimitiveType type2 = (PrimitiveType) this.StoreItemCollection.StoreProviderManifest.GetEdmType(parameter.TypeUsage).EdmType;
                    if (type2 == null)
                    {
                        AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_ProviderReturnsNullType(parameter.Name), StorageMappingErrorCode.MappingStoreProviderReturnsNullEdmType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                        return;
                    }
                    if (type2.PrimitiveTypeKind != edmType.PrimitiveTypeKind)
                    {
                        AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_IncompatibleParameterType(parameter.Name, type2.Name, edmType.Name), StorageMappingErrorCode.MappingFunctionImportIncompatibleParameterType, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                    }
                }
            }
            foreach (FunctionParameter parameter3 in functionImport.Parameters)
            {
                FunctionParameter parameter4;
                if (!targetFunction.Parameters.TryGetValue(parameter3.Name, false, out parameter4))
                {
                    AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_ImportParameterHasNoCorrespondingTargetParameter(parameter3.Name), StorageMappingErrorCode.MappingFunctionImportImportParameterHasNoCorrespondingTargetParameter, this.m_sourceLocation, lineInfo, this.m_parsingErrors);
                }
            }
        }

        private void ValidateFunctionMappingClosure(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping)
        {
            System.Data.Common.Utils.Set<EntitySetBase> setsWithFunctionMapping = new System.Data.Common.Utils.Set<EntitySetBase>();
            foreach (StorageEntitySetMapping mapping in entityContainerMapping.EntitySetMaps)
            {
                if (mapping.FunctionMappings.Count > 0)
                {
                    setsWithFunctionMapping.Add(mapping.Set);
                    foreach (AssociationSetEnd end in mapping.ImplicitlyMappedAssociationSetEnds)
                    {
                        setsWithFunctionMapping.Add(end.ParentAssociationSet);
                    }
                }
            }
            foreach (StorageAssociationSetMapping mapping2 in entityContainerMapping.RelationshipSetMaps)
            {
                if (mapping2.FunctionMapping != null)
                {
                    setsWithFunctionMapping.Add(mapping2.Set);
                }
            }
            System.Data.Common.Utils.Set<EntitySetBase> list = FindMissingFunctionMappings(entityContainerMapping, setsWithFunctionMapping);
            if (0 < list.Count)
            {
                AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_MissingSetClosure_1), StringUtil.ToCommaSeparatedString(list), StorageMappingErrorCode.MissingSetClosureInFunctionMapping, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
            }
        }

        private void ValidateQueryViewsClosure(XPathNavigator nav, StorageEntityContainerMapping entityContainerMapping)
        {
            if (this.m_hasQueryViews)
            {
                System.Data.Common.Utils.Set<EntitySetBase> elements = new System.Data.Common.Utils.Set<EntitySetBase>();
                System.Data.Common.Utils.Set<EntitySetBase> additionalSetsInClosure = new System.Data.Common.Utils.Set<EntitySetBase>();
                foreach (StorageSetMapping mapping in entityContainerMapping.AllSetMaps)
                {
                    if (mapping.QueryView != null)
                    {
                        elements.Add(mapping.Set);
                    }
                }
                additionalSetsInClosure.AddRange(elements);
                ValidateClosureAmongSets(entityContainerMapping, elements, additionalSetsInClosure);
                if (0 < additionalSetsInClosure.Count)
                {
                    AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Query_Views_MissingSetClosure_1), StringUtil.ToCommaSeparatedString(additionalSetsInClosure), StorageMappingErrorCode.MissingSetClosureInQueryViews, this.m_sourceLocation, (IXmlLineInfo) nav, this.m_parsingErrors);
                }
            }
        }

        internal void XsdValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity != XmlSeverityType.Warning)
            {
                string schemaLocation = null;
                if (!string.IsNullOrEmpty(args.Exception.SourceUri))
                {
                    schemaLocation = Helper.GetFileNameFromUri(new Uri(args.Exception.SourceUri));
                }
                EdmSchemaErrorSeverity warning = EdmSchemaErrorSeverity.Error;
                if (args.Severity == XmlSeverityType.Warning)
                {
                    warning = EdmSchemaErrorSeverity.Warning;
                }
                EdmSchemaError item = new EdmSchemaError(System.Data.Entity.Strings.Mapping_InvalidMappingSchema_validation_1(args.Exception.Message), 0x7e9, warning, schemaLocation, args.Exception.LineNumber, args.Exception.LinePosition);
                this.m_parsingErrors.Add(item);
            }
        }

        private System.Data.Metadata.Edm.EdmItemCollection EdmItemCollection =>
            this.m_storageMappingItemCollection.EdmItemCollection;

        internal bool HasQueryViews =>
            this.m_hasQueryViews;

        internal IList<EdmSchemaError> ParsingErrors =>
            this.m_parsingErrors;

        private System.Data.Metadata.Edm.StoreItemCollection StoreItemCollection =>
            this.m_storageMappingItemCollection.StoreItemCollection;

        private class FunctionMappingLoader
        {
            private bool m_allowCurrentVersion;
            private bool m_allowOriginalVersion;
            private readonly AssociationSet m_associationSet;
            private AssociationSet m_associationSetNavigation;
            private readonly EdmItemCollection m_edmItemCollection;
            private readonly EntitySet m_entitySet;
            private EdmFunction m_function;
            private readonly Stack<EdmMember> m_members;
            private readonly System.Data.Metadata.Edm.EntityContainer m_modelContainer;
            private readonly StorageMappingItemLoader m_parentLoader;
            private readonly System.Data.Common.Utils.Set<FunctionParameter> m_seenParameters;
            private readonly StoreItemCollection m_storeItemCollection;

            internal FunctionMappingLoader(StorageMappingItemLoader parentLoader, EntitySetBase extent)
            {
                this.m_parentLoader = EntityUtil.CheckArgumentNull<StorageMappingItemLoader>(parentLoader, "parentLoader");
                this.m_modelContainer = EntityUtil.CheckArgumentNull<EntitySetBase>(extent, "extent").EntityContainer;
                this.m_edmItemCollection = parentLoader.EdmItemCollection;
                this.m_storeItemCollection = parentLoader.StoreItemCollection;
                this.m_entitySet = extent as EntitySet;
                if (this.m_entitySet == null)
                {
                    this.m_associationSet = (AssociationSet) extent;
                }
                this.m_seenParameters = new System.Data.Common.Utils.Set<FunctionParameter>();
                this.m_members = new Stack<EdmMember>();
            }

            private EdmFunction LoadAndValidateFunctionMetadata(XPathNavigator nav, out FunctionParameter rowsAffectedParameter)
            {
                IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
                string aliasResolvedAttributeValue = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "FunctionName");
                rowsAffectedParameter = null;
                ReadOnlyCollection<EdmFunction> functions = this.m_storeItemCollection.GetFunctions(aliasResolvedAttributeValue);
                if (functions.Count == 0)
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_UnknownFunction_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidFunctionMappingUnknownFunction, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                if (1 < functions.Count)
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AmbiguousFunction_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidFunctionMappingAmbiguousFunction, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                EdmFunction function = functions[0];
                if (MetadataHelper.IsComposable(function))
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_NotValidFunction_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidFunctionMappingNotValidFunction, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                string attributeValue = StorageMappingItemLoader.GetAttributeValue(nav, "RowsAffectedParameter");
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    if (!function.Parameters.TryGetValue(attributeValue, false, out rowsAffectedParameter))
                    {
                        StorageMappingItemLoader.AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_RowsAffectedParameterDoesNotExist_2(attributeValue, function.FullName), StorageMappingErrorCode.MappingFunctionImportRowsAffectedParameterDoesNotExist, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                        return null;
                    }
                    if (ParameterMode.Out != rowsAffectedParameter.Mode)
                    {
                        StorageMappingItemLoader.AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_RowsAffectedParameterHasWrongMode_3(attributeValue, rowsAffectedParameter.Mode, ParameterMode.Out), StorageMappingErrorCode.MappingFunctionImportRowsAffectedParameterHasWrongMode, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                        return null;
                    }
                    PrimitiveType edmType = (PrimitiveType) rowsAffectedParameter.TypeUsage.EdmType;
                    if (edmType.PrimitiveTypeKind != PrimitiveTypeKind.Int32)
                    {
                        StorageMappingItemLoader.AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_FunctionImport_RowsAffectedParameterHasWrongType_3(attributeValue, edmType.PrimitiveTypeKind, PrimitiveTypeKind.Int32), StorageMappingErrorCode.MappingFunctionImportRowsAffectedParameterHasWrongType, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                        return null;
                    }
                    this.m_seenParameters.Add(rowsAffectedParameter);
                }
                foreach (FunctionParameter parameter in function.Parameters)
                {
                    if ((parameter.Mode != ParameterMode.In) && (attributeValue != parameter.Name))
                    {
                        StorageMappingItemLoader.AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_NotValidFunctionParameter_3(aliasResolvedAttributeValue, parameter.Name, "RowsAffectedParameter"), StorageMappingErrorCode.InvalidFunctionMappingNotValidFunctionParameter, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                        return null;
                    }
                }
                return function;
            }

            private AssociationSetEnd LoadAssociationEnd(XPathNavigator nav)
            {
                IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
                string aliasResolvedAttributeValue = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "AssociationSet");
                string identity = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "From");
                string str3 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "To");
                RelationshipSet relationshipSet = null;
                if (((aliasResolvedAttributeValue == null) || !this.m_modelContainer.TryGetRelationshipSetByName(aliasResolvedAttributeValue, false, out relationshipSet)) || (BuiltInTypeKind.AssociationSet != relationshipSet.BuiltInTypeKind))
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationSetDoesNotExist_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidAssociationSet, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                AssociationSet set2 = (AssociationSet) relationshipSet;
                AssociationSetEnd item = null;
                if ((identity == null) || !set2.AssociationSetEnds.TryGetValue(identity, false, out item))
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationSetRoleDoesNotExist_1), identity, StorageMappingErrorCode.InvalidAssociationSetRoleInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                AssociationSetEnd end2 = null;
                if ((str3 == null) || !set2.AssociationSetEnds.TryGetValue(str3, false, out end2))
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationSetRoleDoesNotExist_1), str3, StorageMappingErrorCode.InvalidAssociationSetRoleInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                if (!item.EntitySet.Equals(this.m_entitySet))
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationSetFromRoleIsNotEntitySet_1), identity, StorageMappingErrorCode.InvalidAssociationSetRoleInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                if ((end2.CorrespondingAssociationEndMember.RelationshipMultiplicity != RelationshipMultiplicity.One) && (end2.CorrespondingAssociationEndMember.RelationshipMultiplicity != RelationshipMultiplicity.ZeroOrOne))
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationSetCardinality_1), str3, StorageMappingErrorCode.InvalidAssociationSetCardinalityInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                return end2;
            }

            internal StorageFunctionMapping LoadAssociationSetFunctionMapping(XPathNavigator nav, bool isInsert)
            {
                FunctionParameter parameter;
                this.m_function = this.LoadAndValidateFunctionMetadata(nav.Clone(), out parameter);
                if (this.m_function == null)
                {
                    return null;
                }
                if (isInsert)
                {
                    this.m_allowCurrentVersion = true;
                    this.m_allowOriginalVersion = false;
                }
                else
                {
                    this.m_allowCurrentVersion = false;
                    this.m_allowOriginalVersion = true;
                }
                return new StorageFunctionMapping(this.m_function, this.LoadParameterBindings(nav.Clone(), this.m_associationSet.ElementType), parameter, null);
            }

            private EdmMember LoadComplexTypeProperty(XPathNavigator nav, StructuralType type, out ComplexType complexType)
            {
                IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
                string aliasResolvedAttributeValue = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
                string identity = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
                EdmMember item = null;
                if ((aliasResolvedAttributeValue == null) || !type.Members.TryGetValue(aliasResolvedAttributeValue, false, out item))
                {
                    StorageMappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_PropertyNotFound_2), aliasResolvedAttributeValue, type.Name, StorageMappingErrorCode.InvalidEdmMember, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    complexType = null;
                    return null;
                }
                complexType = null;
                if ((identity == null) || !this.m_edmItemCollection.TryGetItem<ComplexType>(identity, out complexType))
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_ComplexTypeNotFound_1), identity, StorageMappingErrorCode.InvalidComplexType, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                if (!item.TypeUsage.EdmType.Equals(complexType) && !Helper.IsSubtypeOf(item.TypeUsage.EdmType, complexType))
                {
                    StorageMappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_WrongComplexType_2), identity, item.Name, StorageMappingErrorCode.InvalidComplexType, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                return item;
            }

            private AssociationSetEnd LoadEndProperty(XPathNavigator nav)
            {
                string aliasResolvedAttributeValue = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
                AssociationSetEnd item = null;
                if ((aliasResolvedAttributeValue != null) && this.m_associationSet.AssociationSetEnds.TryGetValue(aliasResolvedAttributeValue, false, out item))
                {
                    return item;
                }
                StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AssociationSetRoleDoesNotExist_1), aliasResolvedAttributeValue, StorageMappingErrorCode.InvalidAssociationSetRoleInFunctionMapping, this.m_parentLoader.m_sourceLocation, (IXmlLineInfo) nav, this.m_parentLoader.m_parsingErrors);
                return null;
            }

            internal StorageFunctionMapping LoadEntityTypeFunctionMapping(XPathNavigator nav, bool allowCurrentVersion, bool allowOriginalVersion, System.Data.Metadata.Edm.EntityType entityType)
            {
                FunctionParameter parameter;
                this.m_function = this.LoadAndValidateFunctionMetadata(nav.Clone(), out parameter);
                if (this.m_function == null)
                {
                    return null;
                }
                this.m_allowCurrentVersion = allowCurrentVersion;
                this.m_allowOriginalVersion = allowOriginalVersion;
                IEnumerable<StorageFunctionParameterBinding> parameterBindings = this.LoadParameterBindings(nav.Clone(), entityType);
                return new StorageFunctionMapping(this.m_function, parameterBindings, parameter, this.LoadResultBindings(nav.Clone(), entityType));
            }

            private IEnumerable<StorageFunctionParameterBinding> LoadParameterBindings(XPathNavigator nav, StructuralType type)
            {
                List<StorageFunctionParameterBinding> list = new List<StorageFunctionParameterBinding>(this.LoadParameterBindings(nav.Clone(), type, false));
                System.Data.Common.Utils.Set<FunctionParameter> set = new System.Data.Common.Utils.Set<FunctionParameter>(this.m_function.Parameters);
                set.Subtract(this.m_seenParameters);
                if (set.Count != 0)
                {
                    StorageMappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_MissingParameter_2), this.m_function.FullName, StringUtil.ToCommaSeparatedString(set), StorageMappingErrorCode.InvalidParameterInFunctionMapping, this.m_parentLoader.m_sourceLocation, (IXmlLineInfo) nav, this.m_parentLoader.m_parsingErrors);
                    return new List<StorageFunctionParameterBinding>();
                }
                return list;
            }

            private IEnumerable<StorageFunctionParameterBinding> LoadParameterBindings(XPathNavigator nav, StructuralType type, bool restrictToKeyMembers)
            {
                if (nav.MoveToChild(XPathNodeType.Element))
                {
                    do
                    {
                        string localName = nav.LocalName;
                        if (localName != null)
                        {
                            switch (localName)
                            {
                                case "ScalarProperty":
                                {
                                    StorageFunctionParameterBinding iteratorVariable0 = this.LoadScalarPropertyParameterBinding(nav.Clone(), type, restrictToKeyMembers);
                                    if (iteratorVariable0 == null)
                                    {
                                        goto Label_03A9;
                                    }
                                    yield return iteratorVariable0;
                                    break;
                                }
                                case "ComplexProperty":
                                {
                                    ComplexType iteratorVariable1;
                                    EdmMember item = this.LoadComplexTypeProperty(nav.Clone(), type, out iteratorVariable1);
                                    if (item != null)
                                    {
                                        this.m_members.Push(item);
                                        foreach (StorageFunctionParameterBinding iteratorVariable3 in this.LoadParameterBindings(nav.Clone(), iteratorVariable1, restrictToKeyMembers))
                                        {
                                            yield return iteratorVariable3;
                                        }
                                        this.m_members.Pop();
                                    }
                                    break;
                                }
                                case "AssociationEnd":
                                {
                                    AssociationSetEnd iteratorVariable4 = this.LoadAssociationEnd(nav.Clone());
                                    if (iteratorVariable4 != null)
                                    {
                                        this.m_members.Push(iteratorVariable4.CorrespondingAssociationEndMember);
                                        this.m_associationSetNavigation = iteratorVariable4.ParentAssociationSet;
                                        foreach (StorageFunctionParameterBinding iteratorVariable5 in this.LoadParameterBindings(nav.Clone(), iteratorVariable4.EntitySet.ElementType, true))
                                        {
                                            yield return iteratorVariable5;
                                        }
                                        this.m_associationSetNavigation = null;
                                        this.m_members.Pop();
                                    }
                                    break;
                                }
                                case "EndProperty":
                                {
                                    AssociationSetEnd iteratorVariable6 = this.LoadEndProperty(nav.Clone());
                                    if (iteratorVariable6 != null)
                                    {
                                        this.m_members.Push(iteratorVariable6.CorrespondingAssociationEndMember);
                                        foreach (StorageFunctionParameterBinding iteratorVariable7 in this.LoadParameterBindings(nav.Clone(), iteratorVariable6.EntitySet.ElementType, true))
                                        {
                                            yield return iteratorVariable7;
                                        }
                                        this.m_members.Pop();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    while (nav.MoveToNext(XPathNodeType.Element));
                }
            Label_03A9:
                yield break;
            }

            private IEnumerable<StorageFunctionResultBinding> LoadResultBindings(XPathNavigator nav, System.Data.Metadata.Edm.EntityType entityType)
            {
                List<StorageFunctionResultBinding> list = new List<StorageFunctionResultBinding>();
                IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
                if (nav.MoveToChild(XPathNodeType.Element))
                {
                    do
                    {
                        if (nav.LocalName == "ResultBinding")
                        {
                            string aliasResolvedAttributeValue = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
                            string columnName = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
                            EdmProperty item = null;
                            if ((aliasResolvedAttributeValue == null) || !entityType.Properties.TryGetValue(aliasResolvedAttributeValue, false, out item))
                            {
                                StorageMappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_PropertyNotFound_2), aliasResolvedAttributeValue, entityType.Name, StorageMappingErrorCode.InvalidEdmMember, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                                return new List<StorageFunctionResultBinding>();
                            }
                            StorageFunctionResultBinding binding = new StorageFunctionResultBinding(columnName, item);
                            list.Add(binding);
                        }
                    }
                    while (nav.MoveToNext(XPathNodeType.Element));
                }
                KeyToListMap<EdmProperty, string> map = new KeyToListMap<EdmProperty, string>(EqualityComparer<EdmProperty>.Default);
                foreach (StorageFunctionResultBinding binding2 in list)
                {
                    map.Add(binding2.Property, binding2.ColumnName);
                }
                foreach (EdmProperty property2 in map.Keys)
                {
                    ReadOnlyCollection<string> onlys = map.ListForKey(property2);
                    if (1 < onlys.Count)
                    {
                        StorageMappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_AmbiguousResultBinding_2), property2.Name, StringUtil.ToCommaSeparatedString(onlys), StorageMappingErrorCode.AmbiguousResultBindingInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                        return new List<StorageFunctionResultBinding>();
                    }
                }
                return list;
            }

            private StorageFunctionParameterBinding LoadScalarPropertyParameterBinding(XPathNavigator nav, StructuralType type, bool restrictToKeyMembers)
            {
                IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
                string aliasResolvedAttributeValue = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "ParameterName");
                string identity = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
                string str3 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Version");
                bool isCurrent = false;
                if (str3 == null)
                {
                    if (this.m_allowOriginalVersion)
                    {
                        if (this.m_allowCurrentVersion)
                        {
                            StorageMappingItemLoader.AddToSchemaErrors(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_MissingVersion_0, StorageMappingErrorCode.MissingVersionInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                            return null;
                        }
                        isCurrent = false;
                    }
                    else
                    {
                        isCurrent = true;
                    }
                }
                else
                {
                    isCurrent = str3 == "Current";
                }
                if (isCurrent && !this.m_allowCurrentVersion)
                {
                    StorageMappingItemLoader.AddToSchemaErrors(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_VersionMustBeOriginal_0, StorageMappingErrorCode.InvalidVersionInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                if (!isCurrent && !this.m_allowOriginalVersion)
                {
                    StorageMappingItemLoader.AddToSchemaErrors(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_VersionMustBeCurrent_0, StorageMappingErrorCode.InvalidVersionInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                FunctionParameter item = null;
                if ((aliasResolvedAttributeValue == null) || !this.m_function.Parameters.TryGetValue(aliasResolvedAttributeValue, false, out item))
                {
                    StorageMappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_ParameterNotFound_2), aliasResolvedAttributeValue, this.m_function.Name, StorageMappingErrorCode.InvalidParameterInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                EdmMember member = null;
                if (restrictToKeyMembers)
                {
                    if ((identity == null) || !((System.Data.Metadata.Edm.EntityType) type).KeyMembers.TryGetValue(identity, false, out member))
                    {
                        StorageMappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_PropertyNotKey_2), identity, type.Name, StorageMappingErrorCode.InvalidEdmMember, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                        return null;
                    }
                }
                else if ((identity == null) || !type.Members.TryGetValue(identity, false, out member))
                {
                    StorageMappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new System.Func<object, object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_PropertyNotFound_2), identity, type.Name, StorageMappingErrorCode.InvalidEdmMember, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                if (this.m_seenParameters.Contains(item))
                {
                    StorageMappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_ParameterBoundTwice_1), aliasResolvedAttributeValue, StorageMappingErrorCode.ParameterBoundTwiceInFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                    return null;
                }
                int count = this.m_parentLoader.m_parsingErrors.Count;
                if ((Helper.ValidateAndConvertTypeUsage(member, lineInfo, this.m_parentLoader.m_sourceLocation, member.TypeUsage, item.TypeUsage, this.m_parentLoader.m_parsingErrors, this.m_storeItemCollection) == null) && (count == this.m_parentLoader.m_parsingErrors.Count))
                {
                    StorageMappingItemLoader.AddToSchemaErrorWithMessage(System.Data.Entity.Strings.Mapping_Invalid_Function_Mapping_PropertyParameterTypeMismatch_6(member.TypeUsage.EdmType, member.Name, member.DeclaringType.FullName, item.TypeUsage.EdmType, item.Name, this.m_function.FullName), StorageMappingErrorCode.InvalidFunctionMappingPropertyParameterTypeMismatch, this.m_parentLoader.m_sourceLocation, lineInfo, this.m_parentLoader.m_parsingErrors);
                }
                this.m_members.Push(member);
                StorageFunctionParameterBinding binding = new StorageFunctionParameterBinding(item, new StorageFunctionMemberPath(this.m_members, this.m_associationSetNavigation), isCurrent);
                this.m_members.Pop();
                this.m_seenParameters.Add(item);
                return binding;
            }

        }
    }
}


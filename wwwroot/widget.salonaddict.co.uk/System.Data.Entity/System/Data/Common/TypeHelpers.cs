namespace System.Data.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal static class TypeHelpers
    {
        internal static readonly ReadOnlyMetadataCollection<EdmMember> EmptyArrayEdmMember = new ReadOnlyMetadataCollection<EdmMember>(new MetadataCollection<EdmMember>().SetReadOnly());
        internal static readonly FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember> EmptyArrayEdmProperty = new FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember>(EmptyArrayEdmMember, null);

        [Conditional("DEBUG")]
        internal static void AssertEdmType(DbCommandTree commandTree)
        {
            DbQueryCommandTree tree = commandTree as DbQueryCommandTree;
        }

        [Conditional("DEBUG")]
        internal static void AssertEdmType(TypeUsage typeUsage)
        {
            EdmType edmType = typeUsage.EdmType;
            if (!TypeSemantics.IsCollectionType(typeUsage))
            {
                if ((TypeSemantics.IsStructuralType(typeUsage) && !Helper.IsComplexType(typeUsage.EdmType)) && !Helper.IsEntityType(typeUsage.EdmType))
                {
                    using (IEnumerator enumerator = GetDeclaredStructuralMembers(typeUsage).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            EdmMember current = (EdmMember) enumerator.Current;
                        }
                        return;
                    }
                }
                if (TypeSemantics.IsPrimitiveType(typeUsage))
                {
                    PrimitiveType type2 = edmType as PrimitiveType;
                    if ((type2 != null) && (type2.DataSpace != DataSpace.CSpace))
                    {
                        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "PrimitiveType must be CSpace '{0}'", new object[] { typeUsage.ToString() }));
                    }
                }
            }
        }

        internal static DbType ConvertClrTypeToDbType(Type clrType)
        {
            switch (Type.GetTypeCode(clrType))
            {
                case TypeCode.Empty:
                    throw EntityUtil.InvalidDataType(TypeCode.Empty);

                case TypeCode.Object:
                    if (clrType == typeof(byte[]))
                    {
                        return DbType.Binary;
                    }
                    if (clrType == typeof(char[]))
                    {
                        return DbType.String;
                    }
                    if (clrType == typeof(Guid))
                    {
                        return DbType.Guid;
                    }
                    if (clrType == typeof(TimeSpan))
                    {
                        return DbType.Time;
                    }
                    if (clrType == typeof(DateTimeOffset))
                    {
                        return DbType.DateTimeOffset;
                    }
                    return DbType.Object;

                case TypeCode.DBNull:
                    return DbType.Object;

                case TypeCode.Boolean:
                    return DbType.Boolean;

                case TypeCode.Char:
                    return DbType.String;

                case TypeCode.SByte:
                    return DbType.SByte;

                case TypeCode.Byte:
                    return DbType.Byte;

                case TypeCode.Int16:
                    return DbType.Int16;

                case TypeCode.UInt16:
                    return DbType.UInt16;

                case TypeCode.Int32:
                    return DbType.Int32;

                case TypeCode.UInt32:
                    return DbType.UInt32;

                case TypeCode.Int64:
                    return DbType.Int64;

                case TypeCode.UInt64:
                    return DbType.UInt64;

                case TypeCode.Single:
                    return DbType.Single;

                case TypeCode.Double:
                    return DbType.Double;

                case TypeCode.Decimal:
                    return DbType.Decimal;

                case TypeCode.DateTime:
                    return DbType.DateTime;

                case TypeCode.String:
                    return DbType.String;
            }
            throw EntityUtil.UnknownDataTypeCode(clrType, Type.GetTypeCode(clrType));
        }

        internal static CollectionType CreateCollectionType(TypeUsage elementType) => 
            new CollectionType(elementType);

        internal static TypeUsage CreateCollectionTypeUsage(TypeUsage elementType) => 
            CreateCollectionTypeUsage(elementType, false);

        internal static TypeUsage CreateCollectionTypeUsage(TypeUsage elementType, bool readOnly) => 
            TypeUsage.Create(new CollectionType(elementType));

        internal static RowType CreateKeyRowType(EntityTypeBase entityType, MetadataWorkspace metadataWorkspace)
        {
            IEnumerable<EdmMember> keyMembers = entityType.KeyMembers;
            if (keyMembers == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EntityTypeNullKeyMembersInvalid, "entityType");
            }
            List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>>();
            foreach (EdmProperty property in keyMembers)
            {
                columns.Add(new KeyValuePair<string, TypeUsage>(property.Name, Helper.GetModelTypeUsage(property)));
            }
            if (columns.Count < 1)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EntityTypeEmptyKeyMembersInvalid, "entityType");
            }
            return CreateRowType(columns);
        }

        internal static RefType CreateReferenceType(EntityTypeBase entityType) => 
            new RefType((EntityType) entityType);

        internal static TypeUsage CreateReferenceTypeUsage(EntityType entityType) => 
            TypeUsage.Create(CreateReferenceType(entityType));

        internal static RowType CreateRowType(IEnumerable<KeyValuePair<string, TypeUsage>> columns)
        {
            List<EdmProperty> properties = new List<EdmProperty>();
            foreach (KeyValuePair<string, TypeUsage> pair in columns)
            {
                properties.Add(new EdmProperty(pair.Key, pair.Value));
            }
            return new RowType(properties);
        }

        internal static TypeUsage CreateRowTypeUsage(IEnumerable<KeyValuePair<string, TypeUsage>> columns, bool readOnly) => 
            TypeUsage.Create(CreateRowType(columns));

        internal static IBaseList<EdmMember> GetAllStructuralMembers(EdmType edmType)
        {
            switch (edmType.BuiltInTypeKind)
            {
                case BuiltInTypeKind.AssociationType:
                    return (IBaseList<EdmMember>) ((AssociationType) edmType).AssociationEndMembers;

                case BuiltInTypeKind.ComplexType:
                    return (IBaseList<EdmMember>) ((ComplexType) edmType).Properties;

                case BuiltInTypeKind.EntityType:
                    return (IBaseList<EdmMember>) ((EntityType) edmType).Properties;

                case BuiltInTypeKind.RowType:
                    return (IBaseList<EdmMember>) ((RowType) edmType).Properties;
            }
            return EmptyArrayEdmProperty;
        }

        internal static IBaseList<EdmMember> GetAllStructuralMembers(TypeUsage type) => 
            GetAllStructuralMembers(type.EdmType);

        internal static TypeUsage GetCommonTypeUsage(IEnumerable<TypeUsage> types)
        {
            TypeUsage commonType = null;
            foreach (TypeUsage usage2 in types)
            {
                if (usage2 == null)
                {
                    return null;
                }
                if (commonType == null)
                {
                    commonType = usage2;
                }
                else
                {
                    commonType = TypeSemantics.GetCommonType(commonType, usage2);
                    if (commonType == null)
                    {
                        return commonType;
                    }
                }
            }
            return commonType;
        }

        internal static TypeUsage GetCommonTypeUsage(TypeUsage typeUsage1, TypeUsage typeUsage2) => 
            TypeSemantics.GetCommonType(typeUsage1, typeUsage2);

        internal static IEnumerable GetDeclaredStructuralMembers(EdmType edmType)
        {
            switch (edmType.BuiltInTypeKind)
            {
                case BuiltInTypeKind.AssociationType:
                    return ((AssociationType) edmType).GetDeclaredOnlyMembers<AssociationEndMember>();

                case BuiltInTypeKind.ComplexType:
                    return ((ComplexType) edmType).GetDeclaredOnlyMembers<EdmProperty>();

                case BuiltInTypeKind.EntityType:
                    return ((EntityType) edmType).GetDeclaredOnlyMembers<EdmProperty>();

                case BuiltInTypeKind.RowType:
                    return ((RowType) edmType).GetDeclaredOnlyMembers<EdmProperty>();
            }
            return EmptyArrayEdmProperty;
        }

        internal static IEnumerable GetDeclaredStructuralMembers(TypeUsage type) => 
            GetDeclaredStructuralMembers(type.EdmType);

        internal static TEdmType GetEdmType<TEdmType>(TypeUsage typeUsage) where TEdmType: EdmType => 
            ((TEdmType) typeUsage.EdmType);

        internal static TypeUsage GetElementTypeUsage(TypeUsage type)
        {
            if (TypeSemantics.IsCollectionType(type))
            {
                return ((CollectionType) type.EdmType).TypeUsage;
            }
            if (TypeSemantics.IsReferenceType(type))
            {
                return TypeUsage.Create(((RefType) type.EdmType).ElementType);
            }
            return null;
        }

        internal static string GetFullName(EdmType type) => 
            GetFullName(type.NamespaceName, type.Name);

        internal static string GetFullName(EntitySetBase entitySet) => 
            GetFullName(entitySet.EntityContainer.Name, entitySet.Name);

        internal static string GetFullName(TypeUsage type) => 
            type.ToString();

        internal static string GetFullName(string qualifier, string name)
        {
            if (string.IsNullOrEmpty(qualifier))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { name });
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { qualifier, name });
        }

        internal static TypeUsage GetLiteralTypeUsage(PrimitiveTypeKind primitiveTypeKind) => 
            GetLiteralTypeUsage(primitiveTypeKind, true);

        internal static TypeUsage GetLiteralTypeUsage(PrimitiveTypeKind primitiveTypeKind, bool isUnicode)
        {
            PrimitiveType primitiveType = EdmProviderManifest.Instance.GetPrimitiveType(primitiveTypeKind);
            if (primitiveTypeKind == PrimitiveTypeKind.String)
            {
                FacetValues values = new FacetValues {
                    Unicode = new bool?(isUnicode),
                    MaxLength = TypeUsage.DefaultMaxLengthFacetValue,
                    FixedLength = 0,
                    Nullable = 0
                };
                return TypeUsage.Create(primitiveType, values);
            }
            FacetValues values2 = new FacetValues {
                Nullable = 0
            };
            return TypeUsage.Create(primitiveType, values2);
        }

        internal static ReadOnlyMetadataCollection<EdmProperty> GetProperties(EdmType edmType)
        {
            BuiltInTypeKind builtInTypeKind = edmType.BuiltInTypeKind;
            if (builtInTypeKind != BuiltInTypeKind.ComplexType)
            {
                if (builtInTypeKind == BuiltInTypeKind.EntityType)
                {
                    return ((EntityType) edmType).Properties;
                }
                if (builtInTypeKind == BuiltInTypeKind.RowType)
                {
                    return ((RowType) edmType).Properties;
                }
                return EmptyArrayEdmProperty;
            }
            return ((ComplexType) edmType).Properties;
        }

        internal static ReadOnlyMetadataCollection<EdmProperty> GetProperties(TypeUsage typeUsage) => 
            GetProperties(typeUsage.EdmType);

        internal static TypeUsage GetReadOnlyType(TypeUsage type)
        {
            if (!type.IsReadOnly)
            {
                type.SetReadOnly();
            }
            return type;
        }

        internal static string GetStrongName(EdmType type) => 
            GetFullName(type.NamespaceName, type.Name);

        internal static string GetStrongName(TypeUsage type) => 
            GetStrongName(type.EdmType);

        internal static bool IsCanonicalFunction(EdmFunction function) => 
            ((function.DataSpace == DataSpace.CSpace) && !function.IsCachedStoreFunction);

        internal static bool IsFacetValueConstant(TypeUsage type, string facetName) => 
            Helper.GetFacet(((PrimitiveType) type.EdmType).FacetDescriptions, facetName).IsConstant;

        internal static bool IsIntegerConstant(TypeUsage valueType, object value, long expectedValue)
        {
            if (TypeSemantics.IsIntegerNumericType(valueType))
            {
                if (value == null)
                {
                    return false;
                }
                PrimitiveType edmType = (PrimitiveType) valueType.EdmType;
                switch (edmType.PrimitiveTypeKind)
                {
                    case PrimitiveTypeKind.SByte:
                        return (expectedValue == ((sbyte) value));

                    case PrimitiveTypeKind.Int16:
                        return (expectedValue == ((short) value));

                    case PrimitiveTypeKind.Int32:
                        return (expectedValue == ((int) value));

                    case PrimitiveTypeKind.Int64:
                        return (expectedValue == ((long) value));

                    case PrimitiveTypeKind.Byte:
                        return (expectedValue == ((byte) value));
                }
            }
            return false;
        }

        internal static bool IsSetComparableOpType(TypeUsage typeUsage)
        {
            if ((!Helper.IsEntityType(typeUsage.EdmType) && !Helper.IsPrimitiveType(typeUsage.EdmType)) && !Helper.IsRefType(typeUsage.EdmType))
            {
                if (!TypeSemantics.IsRowType(typeUsage))
                {
                    return false;
                }
                RowType edmType = (RowType) typeUsage.EdmType;
                foreach (EdmProperty property in edmType.Properties)
                {
                    if (!IsSetComparableOpType(property.TypeUsage))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool IsValidDistinctOpType(TypeUsage typeUsage) => 
            IsSetComparableOpType(typeUsage);

        internal static bool IsValidGroupKeyType(TypeUsage typeUsage) => 
            IsSetComparableOpType(typeUsage);

        internal static bool IsValidInOpType(TypeUsage typeUsage)
        {
            if (!TypeSemantics.IsReferenceType(typeUsage) && !TypeSemantics.IsPrimitiveType(typeUsage))
            {
                return TypeSemantics.IsEntityType(typeUsage);
            }
            return true;
        }

        internal static bool IsValidIsNullOpType(TypeUsage typeUsage)
        {
            if ((!TypeSemantics.IsReferenceType(typeUsage) && !TypeSemantics.IsEntityType(typeUsage)) && !TypeSemantics.IsPrimitiveType(typeUsage))
            {
                return TypeSemantics.IsNullType(typeUsage);
            }
            return true;
        }

        internal static bool IsValidSortOpKeyType(TypeUsage typeUsage)
        {
            if (!TypeSemantics.IsRowType(typeUsage))
            {
                return TypeSemantics.IsOrderComparable(typeUsage);
            }
            RowType edmType = (RowType) typeUsage.EdmType;
            foreach (EdmProperty property in edmType.Properties)
            {
                if (!IsValidSortOpKeyType(property.TypeUsage))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool TryGetBooleanFacetValue(TypeUsage type, string facetName, out bool boolValue)
        {
            Facet facet;
            boolValue = false;
            if (type.Facets.TryGetValue(facetName, false, out facet) && (facet.Value != null))
            {
                boolValue = (bool) facet.Value;
                return true;
            }
            return false;
        }

        internal static bool TryGetByteFacetValue(TypeUsage type, string facetName, out byte byteValue)
        {
            Facet facet;
            byteValue = 0;
            if ((type.Facets.TryGetValue(facetName, false, out facet) && (facet.Value != null)) && !Helper.IsUnboundedFacetValue(facet))
            {
                byteValue = (byte) facet.Value;
                return true;
            }
            return false;
        }

        internal static bool TryGetClosestPromotableType(TypeUsage fromType, out TypeUsage promotableType)
        {
            promotableType = null;
            if (Helper.IsPrimitiveType(fromType.EdmType))
            {
                PrimitiveType edmType = (PrimitiveType) fromType.EdmType;
                IList<PrimitiveType> promotionTypes = EdmProviderManifest.Instance.GetPromotionTypes(edmType);
                int index = promotionTypes.IndexOf(edmType);
                if ((-1 != index) && ((index + 1) < promotionTypes.Count))
                {
                    promotableType = TypeUsage.Create(promotionTypes[index + 1]);
                }
            }
            return (null != promotableType);
        }

        internal static bool TryGetCollectionElementType(TypeUsage type, out TypeUsage elementType)
        {
            CollectionType type2;
            if (TryGetEdmType<CollectionType>(type, out type2))
            {
                elementType = type2.TypeUsage;
                return (elementType != null);
            }
            elementType = null;
            return false;
        }

        internal static bool TryGetEdmType<TEdmType>(TypeUsage typeUsage, out TEdmType type) where TEdmType: EdmType
        {
            type = typeUsage.EdmType as TEdmType;
            return (((TEdmType) type) != null);
        }

        internal static bool TryGetIntFacetValue(TypeUsage type, string facetName, out int intValue)
        {
            Facet facet;
            intValue = 0;
            if ((type.Facets.TryGetValue(facetName, false, out facet) && (facet.Value != null)) && !Helper.IsUnboundedFacetValue(facet))
            {
                intValue = (int) facet.Value;
                return true;
            }
            return false;
        }

        internal static bool TryGetIsFixedLength(TypeUsage type, out bool isFixedLength)
        {
            if (!TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String) && !TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Binary))
            {
                isFixedLength = false;
                return false;
            }
            return TryGetBooleanFacetValue(type, "FixedLength", out isFixedLength);
        }

        internal static bool TryGetIsUnicode(TypeUsage type, out bool isUnicode)
        {
            if (!TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String))
            {
                isUnicode = false;
                return false;
            }
            return TryGetBooleanFacetValue(type, "Unicode", out isUnicode);
        }

        internal static bool TryGetMaxLength(TypeUsage type, out int maxLength)
        {
            if (!TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String) && !TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Binary))
            {
                maxLength = 0;
                return false;
            }
            return TryGetIntFacetValue(type, "MaxLength", out maxLength);
        }

        internal static bool TryGetPrecision(TypeUsage type, out byte precision)
        {
            if (!TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Decimal))
            {
                precision = 0;
                return false;
            }
            return TryGetByteFacetValue(type, "Precision", out precision);
        }

        internal static bool TryGetPrimitiveTypeKind(TypeUsage type, out PrimitiveTypeKind typeKind)
        {
            if (((type != null) && (type.EdmType != null)) && (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType))
            {
                typeKind = ((PrimitiveType) type.EdmType).PrimitiveTypeKind;
                return true;
            }
            typeKind = PrimitiveTypeKind.Binary;
            return false;
        }

        internal static bool TryGetRefEntityType(TypeUsage type, out EntityType referencedEntityType)
        {
            RefType type2;
            if (TryGetEdmType<RefType>(type, out type2) && Helper.IsEntityType(type2.ElementType))
            {
                referencedEntityType = (EntityType) type2.ElementType;
                return true;
            }
            referencedEntityType = null;
            return false;
        }

        internal static bool TryGetScale(TypeUsage type, out byte scale)
        {
            if (!TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Decimal))
            {
                scale = 0;
                return false;
            }
            return TryGetByteFacetValue(type, "Scale", out scale);
        }
    }
}


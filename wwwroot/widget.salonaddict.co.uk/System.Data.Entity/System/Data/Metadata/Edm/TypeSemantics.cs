namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal static class TypeSemantics
    {
        private static System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>[,] _commonTypeClosure;

        [Conditional("DEBUG")]
        internal static void AssertTypeInvariant(string message, Func<bool> assertPredicate)
        {
        }

        private static bool CompareTypes(TypeUsage fromType, TypeUsage toType, bool equivalenceOnly)
        {
            if (!object.ReferenceEquals(fromType, toType))
            {
                if (fromType.EdmType.BuiltInTypeKind != toType.EdmType.BuiltInTypeKind)
                {
                    return false;
                }
                if (fromType.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
                {
                    return CompareTypes(((CollectionType) fromType.EdmType).TypeUsage, ((CollectionType) toType.EdmType).TypeUsage, equivalenceOnly);
                }
                if (fromType.EdmType.BuiltInTypeKind == BuiltInTypeKind.RefType)
                {
                    return ((RefType) fromType.EdmType).ElementType.EdmEquals(((RefType) toType.EdmType).ElementType);
                }
                if (fromType.EdmType.BuiltInTypeKind != BuiltInTypeKind.RowType)
                {
                    return fromType.EdmType.EdmEquals(toType.EdmType);
                }
                RowType edmType = (RowType) fromType.EdmType;
                RowType type2 = (RowType) toType.EdmType;
                if (edmType.Properties.Count != type2.Properties.Count)
                {
                    return false;
                }
                for (int i = 0; i < edmType.Properties.Count; i++)
                {
                    EdmProperty property = edmType.Properties[i];
                    EdmProperty property2 = type2.Properties[i];
                    if (!equivalenceOnly && (property.Name != property2.Name))
                    {
                        return false;
                    }
                    if (!CompareTypes(property.TypeUsage, property2.TypeUsage, equivalenceOnly))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void ComputeCommonTypeClosure()
        {
            if (_commonTypeClosure == null)
            {
                System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>[,] onlysArray = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>[15, 15];
                for (int i = 0; i < 15; i++)
                {
                    onlysArray[i, i] = Helper.EmptyPrimitiveTypeReadOnlyCollection;
                }
                System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> storeTypes = EdmProviderManifest.Instance.GetStoreTypes();
                for (int j = 0; j < 15; j++)
                {
                    for (int k = 0; k < j; k++)
                    {
                        onlysArray[j, k] = Intersect(EdmProviderManifest.Instance.GetPromotionTypes(storeTypes[j]), EdmProviderManifest.Instance.GetPromotionTypes(storeTypes[k]));
                        onlysArray[k, j] = onlysArray[j, k];
                    }
                }
                Interlocked.CompareExchange<System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>[,]>(ref _commonTypeClosure, onlysArray, null);
            }
        }

        internal static TypeUsage ForgetConstraints(TypeUsage type)
        {
            if (Helper.IsPrimitiveType(type.EdmType))
            {
                return EdmProviderManifest.Instance.ForgetScalarConstraints(type);
            }
            return type;
        }

        internal static TypeUsage GetCommonType(TypeUsage type1, TypeUsage type2)
        {
            TypeUsage commonType = null;
            if (TryGetCommonType(type1, type2, out commonType))
            {
                return commonType;
            }
            return null;
        }

        private static System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetPrimitiveCommonSuperTypes(PrimitiveType primitiveType1, PrimitiveType primitiveType2)
        {
            ComputeCommonTypeClosure();
            return _commonTypeClosure[(int) primitiveType1.PrimitiveTypeKind, (int) primitiveType2.PrimitiveTypeKind];
        }

        private static bool HasCommonType(TypeUsage type1, TypeUsage type2) => 
            (null != TypeHelpers.GetCommonTypeUsage(type1, type2));

        private static System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> Intersect(IList<PrimitiveType> types1, IList<PrimitiveType> types2)
        {
            List<PrimitiveType> list = new List<PrimitiveType>();
            for (int i = 0; i < types1.Count; i++)
            {
                if (types2.Contains(types1[i]))
                {
                    list.Add(types1[i]);
                }
            }
            if (list.Count == 0)
            {
                return Helper.EmptyPrimitiveTypeReadOnlyCollection;
            }
            return new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(list);
        }

        internal static bool IsAggregateFunction(EdmFunction function) => 
            function.AggregateAttribute;

        internal static bool IsBooleanType(TypeUsage type) => 
            IsPrimitiveType(type, PrimitiveTypeKind.Boolean);

        internal static bool IsCastAllowed(TypeUsage fromType, TypeUsage toType)
        {
            if (!IsNullType(fromType) && !Helper.IsPrimitiveType(fromType.EdmType))
            {
                return false;
            }
            return Helper.IsPrimitiveType(toType.EdmType);
        }

        internal static bool IsCollectionType(TypeUsage type) => 
            Helper.IsCollectionType(type.EdmType);

        internal static bool IsComplexType(TypeUsage type) => 
            (BuiltInTypeKind.ComplexType == type.EdmType.BuiltInTypeKind);

        internal static bool IsEntityType(TypeUsage type) => 
            Helper.IsEntityType(type.EdmType);

        internal static bool IsEnumerationType(TypeUsage type) => 
            (BuiltInTypeKind.EnumType == type.EdmType.BuiltInTypeKind);

        private static bool IsEqualComparable(EdmType edmType)
        {
            if ((!Helper.IsPrimitiveType(edmType) && !Helper.IsRefType(edmType)) && !Helper.IsEntityType(edmType))
            {
                if (!Helper.IsRowType(edmType))
                {
                    return false;
                }
                RowType type = (RowType) edmType;
                foreach (EdmProperty property in type.Properties)
                {
                    if (!IsEqualComparable(property.TypeUsage))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool IsEqualComparable(TypeUsage type) => 
            IsEqualComparable(type.EdmType);

        internal static bool IsEqualComparableTo(TypeUsage type1, TypeUsage type2) => 
            ((IsEqualComparable(type1) && IsEqualComparable(type2)) && HasCommonType(type1, type2));

        internal static bool IsEquivalent(TypeUsage fromType, TypeUsage toType) => 
            CompareTypes(fromType, toType, true);

        internal static bool IsEquivalentOrPromotableTo(TypeUsage fromType, TypeUsage toType)
        {
            if (!IsEquivalent(fromType, toType))
            {
                return IsPromotableTo(fromType, toType);
            }
            return true;
        }

        internal static bool IsFixedPointNumericType(TypeUsage type)
        {
            PrimitiveTypeKind kind;
            return (TypeHelpers.TryGetPrimitiveTypeKind(type, out kind) && (kind == PrimitiveTypeKind.Decimal));
        }

        internal static bool IsFloatPointNumericType(TypeUsage type)
        {
            PrimitiveTypeKind kind;
            if (!TypeHelpers.TryGetPrimitiveTypeKind(type, out kind))
            {
                return false;
            }
            if (kind != PrimitiveTypeKind.Double)
            {
                return (kind == PrimitiveTypeKind.Single);
            }
            return true;
        }

        internal static bool IsIntegerNumericType(TypeUsage type)
        {
            PrimitiveTypeKind kind;
            if (TypeHelpers.TryGetPrimitiveTypeKind(type, out kind))
            {
                switch (kind)
                {
                    case PrimitiveTypeKind.SByte:
                    case PrimitiveTypeKind.Int16:
                    case PrimitiveTypeKind.Int32:
                    case PrimitiveTypeKind.Int64:
                    case PrimitiveTypeKind.Byte:
                        return true;
                }
            }
            return false;
        }

        internal static bool IsNominalType(TypeUsage type)
        {
            if (!IsEntityType(type))
            {
                return IsComplexType(type);
            }
            return true;
        }

        internal static bool IsNullable(EdmMember edmMember) => 
            IsNullable(edmMember.TypeUsage);

        internal static bool IsNullable(TypeUsage type)
        {
            Facet facet;
            if (type.Facets.TryGetValue("Nullable", false, out facet))
            {
                return (bool) facet.Value;
            }
            return true;
        }

        internal static bool IsNullOrNullType(EdmType edmType)
        {
            if (edmType != null)
            {
                return edmType.Equals(MetadataItem.NullType.EdmType);
            }
            return true;
        }

        internal static bool IsNullOrNullType(TypeUsage type)
        {
            if (type != null)
            {
                return IsNullType(type);
            }
            return true;
        }

        internal static bool IsNullType(TypeUsage type) => 
            type.EdmEquals(MetadataItem.NullType);

        internal static bool IsNumericType(TypeUsage type)
        {
            if (!IsIntegerNumericType(type) && !IsFixedPointNumericType(type))
            {
                return IsFloatPointNumericType(type);
            }
            return true;
        }

        private static bool IsOrderComparable(EdmType edmType) => 
            Helper.IsPrimitiveType(edmType);

        internal static bool IsOrderComparable(TypeUsage type) => 
            IsOrderComparable(type.EdmType);

        internal static bool IsOrderComparableTo(TypeUsage type1, TypeUsage type2) => 
            ((IsOrderComparable(type1) && IsOrderComparable(type2)) && HasCommonType(type1, type2));

        internal static bool IsPartOfKey(EdmMember edmMember)
        {
            if (Helper.IsRelationshipEndMember(edmMember))
            {
                return ((RelationshipType) edmMember.DeclaringType).KeyMembers.Contains(edmMember);
            }
            if (!Helper.IsEdmProperty(edmMember))
            {
                return false;
            }
            return (Helper.IsEntityTypeBase(edmMember.DeclaringType) && ((EntityTypeBase) edmMember.DeclaringType).KeyMembers.Contains(edmMember));
        }

        internal static bool IsPolymorphicType(TypeUsage type)
        {
            if (!IsEntityType(type))
            {
                return IsComplexType(type);
            }
            return true;
        }

        internal static bool IsPrimitiveType(TypeUsage type) => 
            Helper.IsPrimitiveType(type.EdmType);

        internal static bool IsPrimitiveType(TypeUsage type, PrimitiveTypeKind primitiveTypeKind)
        {
            PrimitiveTypeKind kind;
            return (TypeHelpers.TryGetPrimitiveTypeKind(type, out kind) && (kind == primitiveTypeKind));
        }

        private static bool IsPrimitiveTypePromotableTo(TypeUsage fromType, TypeUsage toType)
        {
            if (!IsSubTypeOf((PrimitiveType) fromType.EdmType, (PrimitiveType) toType.EdmType))
            {
                return false;
            }
            return true;
        }

        private static bool IsPrimitiveTypeSubTypeOf(TypeUsage fromType, TypeUsage toType)
        {
            if (!IsSubTypeOf((PrimitiveType) fromType.EdmType, (PrimitiveType) toType.EdmType))
            {
                return false;
            }
            return true;
        }

        private static bool IsPromotableTo(RowType fromRowType, RowType toRowType)
        {
            if (fromRowType.Properties.Count != toRowType.Properties.Count)
            {
                return false;
            }
            for (int i = 0; i < fromRowType.Properties.Count; i++)
            {
                if (!IsPromotableTo(fromRowType.Properties[i].TypeUsage, toRowType.Properties[i].TypeUsage))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsPromotableTo(TypeUsage fromType, TypeUsage toType)
        {
            if (toType.EdmEquals(fromType))
            {
                return true;
            }
            if (IsNullType(fromType) && !Helper.IsCollectionType(toType.EdmType))
            {
                return true;
            }
            if (Helper.IsPrimitiveType(fromType.EdmType) && Helper.IsPrimitiveType(toType.EdmType))
            {
                return IsPrimitiveTypePromotableTo(fromType, toType);
            }
            if (Helper.IsCollectionType(fromType.EdmType) && Helper.IsCollectionType(toType.EdmType))
            {
                return IsPromotableTo(TypeHelpers.GetElementTypeUsage(fromType), TypeHelpers.GetElementTypeUsage(toType));
            }
            if (Helper.IsEntityTypeBase(fromType.EdmType) && Helper.IsEntityTypeBase(toType.EdmType))
            {
                return fromType.EdmType.IsSubtypeOf(toType.EdmType);
            }
            if (Helper.IsRefType(fromType.EdmType) && Helper.IsRefType(toType.EdmType))
            {
                return IsPromotableTo(TypeHelpers.GetElementTypeUsage(fromType), TypeHelpers.GetElementTypeUsage(toType));
            }
            if (Helper.IsRowType(fromType.EdmType) && Helper.IsRowType(toType.EdmType))
            {
                return IsPromotableTo((RowType) fromType.EdmType, (RowType) toType.EdmType);
            }
            return ((Helper.IsComplexType(fromType.EdmType) && Helper.IsComplexType(toType.EdmType)) && fromType.EdmType.EdmEquals(toType.EdmType));
        }

        internal static bool IsReferenceType(TypeUsage type) => 
            Helper.IsRefType(type.EdmType);

        internal static bool IsRelationshipType(TypeUsage type) => 
            (BuiltInTypeKind.AssociationType == type.EdmType.BuiltInTypeKind);

        internal static bool IsRowType(TypeUsage type) => 
            Helper.IsRowType(type.EdmType);

        internal static bool IsStructurallyEqualTo(TypeUsage type1, TypeUsage type2) => 
            CompareTypes(type1, type2, false);

        internal static bool IsStructuralType(TypeUsage type) => 
            Helper.IsStructuralType(type.EdmType);

        internal static bool IsSubTypeOf(EdmType subEdmType, EdmType superEdmType) => 
            subEdmType.IsSubtypeOf(superEdmType);

        private static bool IsSubTypeOf(PrimitiveType subPrimitiveType, PrimitiveType superPrimitiveType)
        {
            if (object.ReferenceEquals(subPrimitiveType, superPrimitiveType))
            {
                return true;
            }
            System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> promotionTypes = EdmProviderManifest.Instance.GetPromotionTypes(subPrimitiveType);
            return (-1 != promotionTypes.IndexOf(superPrimitiveType));
        }

        internal static bool IsSubTypeOf(TypeUsage subType, TypeUsage superType)
        {
            if (subType.EdmEquals(superType))
            {
                return true;
            }
            if (Helper.IsPrimitiveType(subType.EdmType) && Helper.IsPrimitiveType(superType.EdmType))
            {
                return IsPrimitiveTypeSubTypeOf(subType, superType);
            }
            return subType.IsSubtypeOf(superType);
        }

        internal static bool IsTypeValidForRelationship(TypeUsage type, RelationshipType relationshipType)
        {
            if (Helper.IsEntityType(type.EdmType))
            {
                foreach (EdmMember member in relationshipType.Members)
                {
                    if (IsValidPolymorphicCast(type.EdmType, TypeHelpers.GetElementTypeUsage(member.TypeUsage).EdmType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool IsUnsignedNumericType(TypeUsage type)
        {
            PrimitiveTypeKind kind;
            if (!TypeHelpers.TryGetPrimitiveTypeKind(type, out kind))
            {
                return false;
            }
            PrimitiveTypeKind kind2 = kind;
            return (kind2 == PrimitiveTypeKind.Byte);
        }

        internal static bool IsValidPolymorphicCast(EdmType fromEdmType, EdmType toEdmType) => 
            IsValidPolymorphicCast(TypeUsage.Create(fromEdmType), TypeUsage.Create(toEdmType));

        internal static bool IsValidPolymorphicCast(TypeUsage fromType, TypeUsage toType)
        {
            if (!IsPolymorphicType(fromType) || !IsPolymorphicType(toType))
            {
                return false;
            }
            if (!IsEquivalent(fromType, toType) && !IsSubTypeOf(fromType, toType))
            {
                return IsSubTypeOf(toType, fromType);
            }
            return true;
        }

        private static bool TryGetCommonBaseType(EdmType type1, EdmType type2, out EdmType commonBaseType)
        {
            Dictionary<EdmType, byte> dictionary = new Dictionary<EdmType, byte>();
            for (EdmType type = type2; type != null; type = type.BaseType)
            {
                dictionary.Add(type, 0);
            }
            for (EdmType type3 = type1; type3 != null; type3 = type3.BaseType)
            {
                if (dictionary.ContainsKey(type3))
                {
                    commonBaseType = type3;
                    return true;
                }
            }
            commonBaseType = null;
            return false;
        }

        private static bool TryGetCommonPrimitiveType(TypeUsage type1, TypeUsage type2, out TypeUsage commonType)
        {
            commonType = null;
            if (IsPromotableTo(type1, type2))
            {
                commonType = ForgetConstraints(type2);
                return true;
            }
            if (IsPromotableTo(type2, type1))
            {
                commonType = ForgetConstraints(type1);
                return true;
            }
            System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> primitiveCommonSuperTypes = GetPrimitiveCommonSuperTypes((PrimitiveType) type1.EdmType, (PrimitiveType) type2.EdmType);
            if (primitiveCommonSuperTypes.Count == 0)
            {
                return false;
            }
            commonType = TypeUsage.CreateDefaultTypeUsage(primitiveCommonSuperTypes[0]);
            return (null != commonType);
        }

        private static bool TryGetCommonType(CollectionType collectionType1, CollectionType collectionType2, out EdmType commonType)
        {
            TypeUsage usage = null;
            if (!TryGetCommonType(collectionType1.TypeUsage, collectionType2.TypeUsage, out usage))
            {
                commonType = null;
                return false;
            }
            commonType = new CollectionType(usage);
            return true;
        }

        private static bool TryGetCommonType(EdmType edmType1, EdmType edmType2, out EdmType commonEdmType)
        {
            if (edmType2 == edmType1)
            {
                commonEdmType = edmType1;
                return true;
            }
            if (Helper.IsPrimitiveType(edmType1) && Helper.IsPrimitiveType(edmType2))
            {
                return TryGetCommonType((PrimitiveType) edmType1, (PrimitiveType) edmType2, out commonEdmType);
            }
            if (Helper.IsCollectionType(edmType1) && Helper.IsCollectionType(edmType2))
            {
                return TryGetCommonType((CollectionType) edmType1, (CollectionType) edmType2, out commonEdmType);
            }
            if (Helper.IsEntityTypeBase(edmType1) && Helper.IsEntityTypeBase(edmType2))
            {
                return TryGetCommonBaseType(edmType1, edmType2, out commonEdmType);
            }
            if (Helper.IsRefType(edmType1) && Helper.IsRefType(edmType2))
            {
                return TryGetCommonType((RefType) edmType1, (RefType) edmType2, out commonEdmType);
            }
            if (Helper.IsRowType(edmType1) && Helper.IsRowType(edmType2))
            {
                return TryGetCommonType((RowType) edmType1, (RowType) edmType2, out commonEdmType);
            }
            commonEdmType = null;
            return false;
        }

        private static bool TryGetCommonType(PrimitiveType primitiveType1, PrimitiveType primitiveType2, out EdmType commonType)
        {
            commonType = null;
            if (IsSubTypeOf(primitiveType1, primitiveType2))
            {
                commonType = primitiveType2;
                return true;
            }
            if (IsSubTypeOf(primitiveType2, primitiveType1))
            {
                commonType = primitiveType1;
                return true;
            }
            System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> primitiveCommonSuperTypes = GetPrimitiveCommonSuperTypes(primitiveType1, primitiveType2);
            if (primitiveCommonSuperTypes.Count > 0)
            {
                commonType = primitiveCommonSuperTypes[0];
                return true;
            }
            return false;
        }

        private static bool TryGetCommonType(RefType refType1, RefType reftype2, out EdmType commonType)
        {
            if (!TryGetCommonType(refType1.ElementType, reftype2.ElementType, out commonType))
            {
                return false;
            }
            commonType = new RefType((EntityType) commonType);
            return true;
        }

        private static bool TryGetCommonType(RowType rowType1, RowType rowType2, out EdmType commonRowType)
        {
            if ((rowType1.Properties.Count != rowType2.Properties.Count) || (rowType1.InitializerMetadata != rowType2.InitializerMetadata))
            {
                commonRowType = null;
                return false;
            }
            List<EdmProperty> properties = new List<EdmProperty>();
            for (int i = 0; i < rowType1.Properties.Count; i++)
            {
                TypeUsage usage;
                if (!TryGetCommonType(rowType1.Properties[i].TypeUsage, rowType2.Properties[i].TypeUsage, out usage))
                {
                    commonRowType = null;
                    return false;
                }
                properties.Add(new EdmProperty(rowType1.Properties[i].Name, usage));
            }
            commonRowType = new RowType(properties, rowType1.InitializerMetadata);
            return true;
        }

        internal static bool TryGetCommonType(TypeUsage type1, TypeUsage type2, out TypeUsage commonType)
        {
            EdmType type;
            commonType = null;
            if (type1.EdmEquals(type2) || IsNullType(type1))
            {
                commonType = ForgetConstraints(type2);
                return true;
            }
            if (IsNullType(type2))
            {
                commonType = ForgetConstraints(type1);
                return true;
            }
            if (Helper.IsPrimitiveType(type1.EdmType) && Helper.IsPrimitiveType(type2.EdmType))
            {
                return TryGetCommonPrimitiveType(type1, type2, out commonType);
            }
            if (TryGetCommonType(type1.EdmType, type2.EdmType, out type))
            {
                commonType = ForgetConstraints(TypeUsage.Create(type));
                return true;
            }
            commonType = null;
            return false;
        }
    }
}


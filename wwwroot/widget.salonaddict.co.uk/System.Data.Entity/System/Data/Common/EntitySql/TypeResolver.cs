namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;
    using System.Text;

    internal sealed class TypeResolver
    {
        private Dictionary<string, string> _aliasedNamespaces;
        private TypeUsage _cachedBooleanType;
        private TypeUsage _cachedInt64Type;
        private TypeUsage _cachedStringType;
        private HashSet<string> _namespaces;
        private System.Data.Metadata.Edm.Perspective _perspective;

        internal TypeResolver(System.Data.Metadata.Edm.Perspective perspective, StringComparer stringComparer)
        {
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.Perspective>(perspective, "perspective");
            this._perspective = perspective;
            this._aliasedNamespaces = new Dictionary<string, string>(stringComparer);
            this._namespaces = new HashSet<string>(stringComparer);
        }

        private static string ConcatStringsWithSeparator(string[] names, char separator, int startIndex, int endIndex)
        {
            if (startIndex == endIndex)
            {
                return names[startIndex];
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(names[startIndex]);
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                builder.Append(separator).Append(names[i]);
            }
            return builder.ToString();
        }

        internal static string GetFullName(string[] names) => 
            ConcatStringsWithSeparator(names, '.', 0, names.Length);

        internal TypeUsage GetLiteralTypeUsage(Literal literal)
        {
            PrimitiveType primitiveType = null;
            if (!ClrProviderManifest.Instance.TryGetPrimitiveType(literal.Type, out primitiveType))
            {
                throw EntityUtil.EntitySqlError(literal.ErrCtx, Strings.LiteralTypeNotFoundInMetadata(literal.OriginalValue));
            }
            return TypeHelpers.GetLiteralTypeUsage(primitiveType.PrimitiveTypeKind, literal.IsUnicodeString);
        }

        private static int GetPromotionRank(TypeUsage candidateType, TypeUsage formalType)
        {
            int num = 1;
            if (TypeSemantics.IsCollectionType(candidateType) && TypeSemantics.IsCollectionType(formalType))
            {
                return GetPromotionRank(TypeHelpers.GetElementTypeUsage(candidateType), TypeHelpers.GetElementTypeUsage(formalType));
            }
            PrimitiveType edmType = candidateType.EdmType as PrimitiveType;
            PrimitiveType primitiveType = formalType.EdmType as PrimitiveType;
            if ((edmType == null) || (primitiveType == null))
            {
                return num;
            }
            IList<PrimitiveType> promotionTypes = EdmProviderManifest.Instance.GetPromotionTypes(edmType);
            if (edmType == primitiveType)
            {
                return (promotionTypes.Count + 1);
            }
            return (promotionTypes.Count - Helper.IsPromotableTo(promotionTypes, primitiveType));
        }

        internal static bool IsBooleanType(TypeUsage type) => 
            TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Boolean);

        internal static bool IsKeyValidForCollation(TypeUsage type) => 
            TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String);

        internal static bool IsStringType(TypeUsage type) => 
            TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String);

        internal static bool IsSubOrSuperType(TypeUsage type1, TypeUsage type2)
        {
            if (!TypeSemantics.IsEquivalent(type1, type2) && !type1.IsSubtypeOf(type2))
            {
                return type2.IsSubtypeOf(type1);
            }
            return true;
        }

        internal static bool IsValidTypeForMethodCall(TypeUsage type) => 
            (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.EntityType);

        internal static uint RankFunctionParameters(IList<TypeUsage> candidateParams, IList<FunctionParameter> signatureParams, bool isGroupAggregateFunction)
        {
            if (candidateParams.Count != signatureParams.Count)
            {
                return 0;
            }
            if ((candidateParams.Count == 0) && (signatureParams.Count == 0))
            {
                return 0x40000000;
            }
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < candidateParams.Count; i++)
            {
                TypeUsage fromType = candidateParams[i];
                TypeUsage typeUsage = signatureParams[i].TypeUsage;
                if (isGroupAggregateFunction)
                {
                    if (!TypeSemantics.IsCollectionType(typeUsage))
                    {
                        throw EntityUtil.EntitySqlError(Strings.InvalidArgumentTypeForAggregateFunction);
                    }
                    typeUsage = TypeHelpers.GetElementTypeUsage(typeUsage);
                }
                if ((signatureParams[i].Mode != ParameterMode.In) && (signatureParams[i].Mode != ParameterMode.InOut))
                {
                    return 0;
                }
                if (TypeSemantics.IsEquivalent(fromType, typeUsage) || (TypeSemantics.IsNullType(fromType) && !TypeSemantics.IsCollectionType(typeUsage)))
                {
                    num++;
                }
                else if (TypeSemantics.IsPromotableTo(fromType, typeUsage))
                {
                    num2 += GetPromotionRank(fromType, typeUsage);
                }
                else
                {
                    return 0;
                }
            }
            if ((num >= 0x8000) || (num2 >= 0x8000))
            {
                throw EntityUtil.EntitySqlError(Strings.TooManyFunctionArguments);
            }
            return (uint) ((num << 15) | num2);
        }

        internal TypeUsage ResolveBaseType(string[] names, out int suffixIndex, out int matchCount)
        {
            TypeUsage typeUsage = null;
            suffixIndex = 0;
            matchCount = 0;
            if (!this.TryGetBaseTypeInAliasedNamespaces(names, out typeUsage, out suffixIndex))
            {
                TypeUsage usage2;
                int num = 0;
                int num2 = 0;
                if (this.TryGetIdentifierBaseType(names, out usage2, out num2))
                {
                    num++;
                }
                if (this.TryGetBaseTypeInNamespaces(names, out typeUsage, out suffixIndex, out matchCount))
                {
                    matchCount += num;
                }
                if (typeUsage == null)
                {
                    typeUsage = usage2;
                    suffixIndex = num2;
                    matchCount = num;
                }
            }
            return typeUsage;
        }

        internal static EdmFunction ResolveFunctionOverloads(IList<EdmFunction> functionsMetadata, IList<TypeUsage> argTypes, bool isGroupAggregateFunction, out bool isAmbiguous)
        {
            EdmFunction function = null;
            uint num = 0;
            isAmbiguous = false;
            for (int i = 0; i < functionsMetadata.Count; i++)
            {
                uint num3 = RankFunctionParameters(argTypes, functionsMetadata[i].Parameters, isGroupAggregateFunction);
                if (num3 != 0)
                {
                    if (num3 == num)
                    {
                        isAmbiguous = true;
                    }
                    if (num3 > num)
                    {
                        isAmbiguous = false;
                        num = num3;
                        function = functionsMetadata[i];
                    }
                }
            }
            return function;
        }

        internal IList<EdmFunction> ResolveNameAsFunction(string[] names, bool ignoreCase, out int matchCount, out List<string> foundNamespaces)
        {
            IList<EdmFunction> functionMetadata = null;
            foundNamespaces = new List<string>();
            string str = null;
            string name = names[names.Length - 1];
            matchCount = 0;
            if (names.Length > 1)
            {
                if (this._aliasedNamespaces.TryGetValue(names[0], out str) && this.TryGetFunctionFromMetadata(name, str, ignoreCase, out functionMetadata))
                {
                    matchCount = 1;
                    foundNamespaces.Add(str);
                    return functionMetadata;
                }
                str = ConcatStringsWithSeparator(names, '.', 0, names.Length - 1);
                if (this.TryGetFunctionFromMetadata(name, str, ignoreCase, out functionMetadata))
                {
                    matchCount = 1;
                    foundNamespaces.Add(str);
                }
                return functionMetadata;
            }
            foreach (string str3 in this._namespaces)
            {
                IList<EdmFunction> list2;
                if (this.TryGetFunctionFromMetadata(name, str3, ignoreCase, out list2))
                {
                    matchCount++;
                    functionMetadata = list2;
                    foundNamespaces.Add(str3);
                }
            }
            return functionMetadata;
        }

        internal TypeUsage ResolveNameAsType(string[] names, int countNames, out int matchCount)
        {
            TypeUsage typeUsage = null;
            matchCount = 0;
            if ((names.Length > 1) && (countNames > (names.Length - 1)))
            {
                string str = null;
                if (this._aliasedNamespaces.TryGetValue(names[0], out str) && this.TryGetTypeFromMetadata(str + "." + ConcatStringsWithSeparator(names, '.', 1, countNames - 1), out typeUsage))
                {
                    matchCount = 1;
                    return typeUsage;
                }
            }
            string typeFullName = ConcatStringsWithSeparator(names, '.', 0, countNames);
            if (this.TryGetTypeFromMetadata(typeFullName, out typeUsage))
            {
                matchCount = 1;
                return typeUsage;
            }
            foreach (string str3 in this._namespaces)
            {
                TypeUsage usage2;
                if (this.TryGetTypeFromMetadata(str3 + "." + typeFullName, out usage2))
                {
                    matchCount++;
                    typeUsage = usage2;
                }
            }
            return typeUsage;
        }

        internal static string[] TrimNamesPrefix(string[] names, int startIndex)
        {
            string[] strArray = new string[names.Length - 1];
            for (int i = startIndex; i < names.Length; i++)
            {
                strArray[i - 1] = names[i];
            }
            return strArray;
        }

        internal bool TryAddAliasedNamespace(string alias, string namespaceName)
        {
            if (this._aliasedNamespaces.ContainsKey(alias))
            {
                return false;
            }
            this._aliasedNamespaces.Add(alias, namespaceName);
            return true;
        }

        internal bool TryAddNamespace(string namespaceName)
        {
            if (this._namespaces.Contains(namespaceName))
            {
                return false;
            }
            this._namespaces.Add(namespaceName);
            return true;
        }

        internal bool TryGetBaseTypeFromMetadata(string namespaceName, string[] names, out TypeUsage TypeUsage, out int suffixIndex)
        {
            TypeUsage = null;
            suffixIndex = 0;
            if (this.TryGetTypeFromMetadata(namespaceName + "." + names[0], out TypeUsage))
            {
                suffixIndex = 1;
            }
            return (null != TypeUsage);
        }

        internal bool TryGetBaseTypeInAliasedNamespaces(string[] names, out TypeUsage TypeUsage, out int suffixIndex)
        {
            TypeUsage = null;
            suffixIndex = 0;
            if (names.Length > 1)
            {
                string str = null;
                if (this._aliasedNamespaces.TryGetValue(names[0], out str) && this.TryGetBaseTypeFromMetadata(str, TrimNamesPrefix(names, 1), out TypeUsage, out suffixIndex))
                {
                    suffixIndex++;
                    return true;
                }
            }
            return false;
        }

        internal bool TryGetBaseTypeInNamespaces(string[] names, out TypeUsage TypeUsage, out int suffixIndex, out int matchCount)
        {
            TypeUsage = null;
            suffixIndex = 0;
            matchCount = 0;
            TypeUsage usage = null;
            foreach (string str in this._namespaces)
            {
                if (this.TryGetTypeFromMetadata(str + "." + names[0], out TypeUsage))
                {
                    usage = TypeUsage;
                    suffixIndex = 1;
                    matchCount++;
                }
            }
            TypeUsage = usage;
            return (matchCount > 0);
        }

        internal bool TryGetFunctionFromMetadata(string name, string namespaceName, bool ignoreCase, out IList<EdmFunction> functionMetadata)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> onlys = this._perspective.GetFunctions(name, namespaceName, ignoreCase);
            functionMetadata = onlys;
            return (functionMetadata.Count > 0);
        }

        internal bool TryGetIdentifierBaseType(string[] names, out TypeUsage TypeUsage, out int suffixIndex)
        {
            TypeUsage = null;
            suffixIndex = 0;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < names.Length; i++)
            {
                builder.Append(names[i]);
                if (this.TryGetTypeFromMetadata(builder.ToString(), out TypeUsage))
                {
                    suffixIndex = i;
                    return true;
                }
                builder.Append('.');
            }
            return false;
        }

        internal bool TryGetTypeFromMetadata(string typeFullName, out TypeUsage typeUsage) => 
            this._perspective.TryGetTypeByName(typeFullName, true, out typeUsage);

        internal TypeUsage BooleanType
        {
            get
            {
                if (this._cachedBooleanType == null)
                {
                    this._cachedBooleanType = this._perspective.MetadataWorkspace.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Boolean);
                }
                return this._cachedBooleanType;
            }
        }

        internal TypeUsage Int64Type
        {
            get
            {
                if (this._cachedInt64Type == null)
                {
                    this._cachedInt64Type = this._perspective.MetadataWorkspace.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Int64);
                }
                return this._cachedInt64Type;
            }
        }

        internal System.Data.Metadata.Edm.Perspective Perspective =>
            this._perspective;

        internal TypeUsage StringType
        {
            get
            {
                if (this._cachedStringType == null)
                {
                    this._cachedStringType = this._perspective.MetadataWorkspace.GetCanonicalModelTypeUsage(PrimitiveTypeKind.String);
                }
                return this._cachedStringType;
            }
        }
    }
}


namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Text;

    internal static class CqlErrorHelper
    {
        private static string GetReadableTypeKind(EdmType type)
        {
            string localizedCollection = string.Empty;
            BuiltInTypeKind builtInTypeKind = type.BuiltInTypeKind;
            if (builtInTypeKind <= BuiltInTypeKind.EntityType)
            {
                switch (builtInTypeKind)
                {
                    case BuiltInTypeKind.CollectionType:
                        localizedCollection = Strings.LocalizedCollection;
                        goto Label_007D;

                    case BuiltInTypeKind.ComplexType:
                        localizedCollection = Strings.LocalizedComplex;
                        goto Label_007D;

                    case BuiltInTypeKind.EntityType:
                        localizedCollection = Strings.LocalizedEntity;
                        goto Label_007D;
                }
            }
            else
            {
                if (builtInTypeKind == BuiltInTypeKind.PrimitiveType)
                {
                    localizedCollection = Strings.LocalizedPrimitive;
                }
                else if (builtInTypeKind != BuiltInTypeKind.RefType)
                {
                    if (builtInTypeKind != BuiltInTypeKind.RowType)
                    {
                        goto Label_006C;
                    }
                    localizedCollection = Strings.LocalizedRow;
                }
                else
                {
                    localizedCollection = Strings.LocalizedReference;
                }
                goto Label_007D;
            }
        Label_006C:
            localizedCollection = type.BuiltInTypeKind.ToString();
        Label_007D:
            return (localizedCollection + " " + Strings.LocalizedType);
        }

        private static string GetReadableTypeKind(TypeUsage type) => 
            GetReadableTypeKind(type.EdmType);

        private static string GetReadableTypeName(EdmType type)
        {
            if (((type.BuiltInTypeKind != BuiltInTypeKind.RowType) && (type.BuiltInTypeKind != BuiltInTypeKind.CollectionType)) && (type.BuiltInTypeKind != BuiltInTypeKind.RefType))
            {
                return type.FullName;
            }
            return type.Name;
        }

        private static string GetReadableTypeName(TypeUsage type) => 
            GetReadableTypeName(type.EdmType);

        internal static void ReportAliasAlreadyUsedError(string aliasName, ErrorContext errCtx, string contextMessage)
        {
            throw EntityUtil.EntitySqlError(errCtx, string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { Strings.AliasNameAlreadyUsed(aliasName), contextMessage }));
        }

        internal static void ReportAmbiguousFunctionError(MethodExpr functionExpr, List<string> foundNamespaces)
        {
            if (foundNamespaces[0].Equals("Edm", StringComparison.OrdinalIgnoreCase) || foundNamespaces[1].Equals("Edm", StringComparison.OrdinalIgnoreCase))
            {
                string str = foundNamespaces[0].Equals("Edm", StringComparison.OrdinalIgnoreCase) ? foundNamespaces[1] : foundNamespaces[0];
                throw EntityUtil.EntitySqlError(functionExpr.ErrCtx, Strings.AmbiguousCanonicalFunction(functionExpr.MethodPrefixExpr.FullName, functionExpr.MethodName, str));
            }
            throw EntityUtil.EntitySqlError(functionExpr.ErrCtx, Strings.AmbiguousFunction(functionExpr.MethodPrefixExpr.FullName, functionExpr.MethodName, foundNamespaces[0], foundNamespaces[1]));
        }

        internal static void ReportFunctionOverloadError(MethodExpr functionExpr, EdmFunction functionType, List<TypeUsage> argTypes)
        {
            Func<object, object, object, string> func;
            string str = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(functionExpr.MethodPrefixExpr.FullName).Append("(");
            for (int i = 0; i < argTypes.Count; i++)
            {
                builder.Append(str);
                builder.Append(argTypes[i].EdmType.NamespaceName);
                builder.Append(".");
                builder.Append(argTypes[i].EdmType.Name);
                str = ", ";
            }
            builder.Append(")");
            if (TypeSemantics.IsAggregateFunction(functionType))
            {
                func = TypeHelpers.IsCanonicalFunction(functionType) ? new Func<object, object, object, string>(Strings.NoCanonicalAggrFunctionOverloadMatch) : new Func<object, object, object, string>(Strings.NoAggrFunctionOverloadMatch);
            }
            else
            {
                func = TypeHelpers.IsCanonicalFunction(functionType) ? new Func<object, object, object, string>(Strings.NoCanonicalFunctionOverloadMatch) : new Func<object, object, object, string>(Strings.NoFunctionOverloadMatch);
            }
            throw EntityUtil.EntitySqlError(functionExpr.ErrCtx.QueryText, func(functionType.NamespaceName, functionType.Name, builder.ToString()), functionExpr.ErrCtx.InputPosition, Strings.CtxFunction(functionType.Name), false);
        }

        internal static EntitySqlException ReportIdentifierElementError(ErrorContext errCtx, string name, TypeUsage definingType)
        {
            if (TypeSemantics.IsCollectionType(definingType))
            {
                return EntityUtil.EntitySqlError(errCtx, Strings.NotAMemberOfCollection(name, definingType.EdmType.FullName));
            }
            return EntityUtil.EntitySqlError(errCtx, Strings.NotAMemberOfType(name, definingType.EdmType.FullName));
        }

        internal static void ReportIdentifierError(Expr expr, SemanticResolver sr)
        {
            string key = null;
            string fullName = null;
            ScopeEntry entry;
            int num;
            Identifier identifier = expr as Identifier;
            if (identifier != null)
            {
                key = fullName = identifier.Name;
            }
            DotExpr expr2 = expr as DotExpr;
            if (expr2 != null)
            {
                key = expr2.Names[0];
                fullName = expr2.FullName;
            }
            if ((expr2 == null) && (identifier == null))
            {
                EntityUtil.Argument("expr must be an identifier");
            }
            sr.SetScopeView(SemanticResolver.ScopeViewKind.CurrentContext);
            if ((sr.CurrentScopeRegionFlags.IsInGroupScope && sr.TryScopeLookup(key, true, out entry, out num)) && (entry.VarKind == SourceVarKind.GroupInput))
            {
                throw EntityUtil.EntitySqlError(expr.ErrCtx, Strings.InvalidGroupIdentifierReference(fullName));
            }
            throw EntityUtil.EntitySqlError(expr.ErrCtx, Strings.CouldNotResolveIdentifier(fullName));
        }

        internal static void ReportIncompatibleCommonType(ErrorContext errCtx, TypeUsage leftType, TypeUsage rightType)
        {
            ReportIncompatibleCommonType(errCtx, leftType, rightType, leftType, rightType);
            throw EntityUtil.EntitySqlError(errCtx, Strings.ArgumentTypesAreIncompatible(leftType.Identity, rightType.Identity));
        }

        private static void ReportIncompatibleCommonType(ErrorContext errCtx, TypeUsage rootLeftType, TypeUsage rootRightType, TypeUsage leftType, TypeUsage rightType)
        {
            TypeUsage commonType = null;
            bool flag = rootLeftType == leftType;
            string message = string.Empty;
            if (leftType.EdmType.BuiltInTypeKind != rightType.EdmType.BuiltInTypeKind)
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.TypeKindMismatch(GetReadableTypeKind(leftType), GetReadableTypeName(leftType), GetReadableTypeKind(rightType), GetReadableTypeName(rightType)));
            }
            switch (leftType.EdmType.BuiltInTypeKind)
            {
                case BuiltInTypeKind.RefType:
                case BuiltInTypeKind.CollectionType:
                    ReportIncompatibleCommonType(errCtx, rootLeftType, rootRightType, TypeHelpers.GetElementTypeUsage(leftType), TypeHelpers.GetElementTypeUsage(rightType));
                    return;

                case BuiltInTypeKind.RowType:
                {
                    RowType edmType = (RowType) leftType.EdmType;
                    RowType type = (RowType) rightType.EdmType;
                    if (edmType.Members.Count != type.Members.Count)
                    {
                        if (flag)
                        {
                            message = Strings.InvalidRootRowType(GetReadableTypeName(edmType), GetReadableTypeName(type));
                        }
                        else
                        {
                            message = Strings.InvalidRowType(GetReadableTypeName(edmType), GetReadableTypeName(rootLeftType), GetReadableTypeName(type), GetReadableTypeName(rootRightType));
                        }
                        throw EntityUtil.EntitySqlError(errCtx, message);
                    }
                    for (int i = 0; i < edmType.Members.Count; i++)
                    {
                        ReportIncompatibleCommonType(errCtx, rootLeftType, rootRightType, edmType.Members[i].TypeUsage, type.Members[i].TypeUsage);
                    }
                    return;
                }
                case BuiltInTypeKind.ComplexType:
                {
                    ComplexType type3 = (ComplexType) leftType.EdmType;
                    ComplexType type4 = (ComplexType) rightType.EdmType;
                    if (type3.Members.Count == type4.Members.Count)
                    {
                        for (int j = 0; j < type3.Members.Count; j++)
                        {
                            ReportIncompatibleCommonType(errCtx, rootLeftType, rootRightType, type3.Members[j].TypeUsage, type4.Members[j].TypeUsage);
                        }
                        return;
                    }
                    if (!flag)
                    {
                        message = Strings.InvalidComplexType(GetReadableTypeName(type3), GetReadableTypeName(rootLeftType), GetReadableTypeName(type4), GetReadableTypeName(rootRightType));
                        break;
                    }
                    message = Strings.InvalidRootComplexType(GetReadableTypeName(type3), GetReadableTypeName(type4));
                    break;
                }
                case BuiltInTypeKind.EntityType:
                    if (!TypeSemantics.TryGetCommonType(leftType, rightType, out commonType))
                    {
                        if (flag)
                        {
                            message = Strings.InvalidEntityRootTypeArgument(GetReadableTypeName(leftType), GetReadableTypeName(rightType));
                        }
                        else
                        {
                            message = Strings.InvalidEntityTypeArgument(GetReadableTypeName(leftType), GetReadableTypeName(rootLeftType), GetReadableTypeName(rightType), GetReadableTypeName(rootRightType));
                        }
                        throw EntityUtil.EntitySqlError(errCtx, message);
                    }
                    return;

                default:
                    if (!TypeSemantics.TryGetCommonType(leftType, rightType, out commonType))
                    {
                        if (flag)
                        {
                            message = Strings.InvalidPlaceholderRootTypeArgument(GetReadableTypeKind(leftType), GetReadableTypeName(leftType), GetReadableTypeKind(rightType), GetReadableTypeName(rightType));
                        }
                        else
                        {
                            message = Strings.InvalidPlaceholderTypeArgument(GetReadableTypeKind(leftType), GetReadableTypeName(leftType), GetReadableTypeName(rootLeftType), GetReadableTypeKind(rightType), GetReadableTypeName(rightType), GetReadableTypeName(rootRightType));
                        }
                        throw EntityUtil.EntitySqlError(errCtx, message);
                    }
                    return;
            }
            throw EntityUtil.EntitySqlError(errCtx, message);
        }

        internal static void ReportTypeResolutionError(string[] names, Expr astExpr, SemanticResolver sr)
        {
            string namespaceName = (names.Length > 1) ? string.Join(".", names, 0, names.Length - 1) : string.Empty;
            string name = names[names.Length - 1];
            ErrorContext errCtx = astExpr.ErrCtx;
            if (namespaceName.Equals("Edm", StringComparison.OrdinalIgnoreCase))
            {
                EdmType type;
                if (!sr.TypeResolver.Perspective.MetadataWorkspace.TryGetType(name, namespaceName, true, DataSpace.CSpace, out type))
                {
                    throw EntityUtil.EntitySqlError(errCtx, Strings.CanonicalTypeNameNotFound(name, "Edm".ToUpperInvariant()));
                }
                throw EntityUtil.EntitySqlError(errCtx, Strings.CanonicalTypeCannotBeMapped(TypeResolver.GetFullName(names)));
            }
            if ((names != null) && (names.Length == 1))
            {
                Identifier identifier = astExpr as Identifier;
                MethodExpr expr = astExpr as MethodExpr;
                if ((((expr == null) || !expr.MethodIdentifier.IsEscaped) && ((identifier == null) || !identifier.IsEscaped)) && (names[0].Equals("ROW", StringComparison.OrdinalIgnoreCase) || names[0].Equals("MULTISET", StringComparison.OrdinalIgnoreCase)))
                {
                    throw EntityUtil.EntitySqlError(errCtx, Strings.GenericSyntaxError);
                }
            }
            throw EntityUtil.EntitySqlError(errCtx, Strings.TypeNameNotFound(TypeResolver.GetFullName(names)));
        }
    }
}


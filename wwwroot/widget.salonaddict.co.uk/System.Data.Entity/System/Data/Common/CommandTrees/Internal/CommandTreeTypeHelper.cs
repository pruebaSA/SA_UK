namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Globalization;

    internal class CommandTreeTypeHelper
    {
        private TypeUsage _boolType;
        private DbCommandTree _owner;

        internal CommandTreeTypeHelper(DbCommandTree owner)
        {
            this._owner = owner;
        }

        internal void CheckEntitySet(EntitySetBase entitySet)
        {
            this.CheckEntitySet(entitySet, "entitySet");
        }

        internal void CheckEntitySet(EntitySetBase entitySet, string varName)
        {
            EntityUtil.CheckArgumentNull<EntitySetBase>(entitySet, varName);
            CheckReadOnly(entitySet, varName);
            if (entitySet.Name == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EntitySetNameNull, varName);
            }
            if (entitySet.EntityContainer == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EntitySetEntityContainerNull, varName);
            }
            if (!this.CheckWorkspaceAndDataSpace(entitySet.EntityContainer))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EntitySetIncorrectSpace, varName);
            }
            if (entitySet.ElementType == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EntitySetElementTypeNull, varName);
            }
            if (!this.CheckWorkspaceAndDataSpace(entitySet.ElementType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EntitySetIncorrectSpace, varName);
            }
        }

        internal void CheckFunction(EdmFunction function)
        {
            EntityUtil.CheckArgumentNull<EdmFunction>(function, "function");
            CheckReadOnly(function, "function");
            if (function.Name == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionNameNull, "function");
            }
            if (!this.CheckWorkspaceAndDataSpace(function))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionIncorrectSpace, "function");
            }
            if (function.IsComposableAttribute && (function.ReturnParameter == null))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionReturnParameterNull, "function");
            }
            if (function.ReturnParameter != null)
            {
                if (TypeSemantics.IsNullOrNullType(function.ReturnParameter.TypeUsage))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionParameterTypeNull, "function.ReturnParameter");
                }
                if (!this.CheckWorkspaceAndDataSpace(function.ReturnParameter.TypeUsage))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionParameterIncorrectSpace, "function.ReturnParameter");
                }
            }
            IList<FunctionParameter> parameters = function.Parameters;
            if (parameters == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionParametersNull, "function");
            }
            for (int i = 0; i < parameters.Count; i++)
            {
                this.CheckParameter(parameters[i], CommandTreeUtils.FormatIndex("function.Parameters", i));
            }
        }

        internal void CheckMember(EdmMember memberMeta, string varName)
        {
            EntityUtil.CheckArgumentNull<EdmMember>(memberMeta, varName);
            CheckReadOnly(memberMeta.DeclaringType, varName);
            if (memberMeta.Name == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EdmMemberNameNull, varName);
            }
            if (TypeSemantics.IsNullOrNullType(memberMeta.TypeUsage))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EdmMemberTypeNull, varName);
            }
            if (TypeSemantics.IsNullOrNullType(memberMeta.DeclaringType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EdmMemberDeclaringTypeNull, varName);
            }
            if (!this.CheckWorkspaceAndDataSpace(memberMeta.TypeUsage) || !this.CheckWorkspaceAndDataSpace(memberMeta.DeclaringType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_EdmMemberIncorrectSpace, varName);
            }
        }

        internal void CheckParameter(FunctionParameter paramMeta, string varName)
        {
            EntityUtil.CheckArgumentNull<FunctionParameter>(paramMeta, varName);
            CheckReadOnly(paramMeta.DeclaringFunction, varName);
            if (paramMeta.Name == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionParameterNameNull, varName);
            }
            if (TypeSemantics.IsNullOrNullType(paramMeta.TypeUsage))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionParameterTypeNull, varName);
            }
            if (!this.CheckWorkspaceAndDataSpace(paramMeta.TypeUsage))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_FunctionParameterIncorrectSpace, varName);
            }
        }

        internal void CheckPolymorphicType(TypeUsage type)
        {
            this.CheckType(type);
            if (!TypeSemantics.IsPolymorphicType(type))
            {
                throw EntityUtil.Argument(Strings.Cqt_General_PolymorphicTypeRequired(TypeHelpers.GetFullName(type)), "type");
            }
        }

        internal static void CheckReadOnly(EntitySetBase item, string varName)
        {
            EntityUtil.CheckArgumentNull<EntitySetBase>(item, varName);
            if (!item.IsReadOnly)
            {
                throw EntityUtil.Argument(Strings.Cqt_General_MetadataNotReadOnly, varName);
            }
        }

        internal static void CheckReadOnly(GlobalItem item, string varName)
        {
            EntityUtil.CheckArgumentNull<GlobalItem>(item, varName);
            if (!item.IsReadOnly)
            {
                throw EntityUtil.Argument(Strings.Cqt_General_MetadataNotReadOnly, varName);
            }
        }

        internal static void CheckReadOnly(TypeUsage item, string varName)
        {
            EntityUtil.CheckArgumentNull<TypeUsage>(item, varName);
            if (!item.IsReadOnly)
            {
                throw EntityUtil.Argument(Strings.Cqt_General_MetadataNotReadOnly, varName);
            }
        }

        internal static void CheckType(EdmType type)
        {
            EntityUtil.CheckArgumentNull<EdmType>(type, "type");
            CheckReadOnly(type, "type");
        }

        internal void CheckType(TypeUsage type)
        {
            this.CheckType(type, "type");
        }

        internal void CheckType(TypeUsage type, string varName)
        {
            EntityUtil.CheckArgumentNull<TypeUsage>(type, varName);
            CheckReadOnly(type, varName);
            if (TypeSemantics.IsNullType(type))
            {
                throw EntityUtil.Argument(Strings.Cqt_General_NullTypeInvalid, varName);
            }
            if (type.EdmType == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_General_TypeUsageNullEdmTypeInvalid, "type");
            }
            if (!this.CheckWorkspaceAndDataSpace(type))
            {
                throw EntityUtil.Argument(Strings.Cqt_Metadata_TypeUsageIncorrectSpace, "type");
            }
        }

        private bool CheckWorkspaceAndDataSpace(GlobalItem item)
        {
            if ((BuiltInTypeKind.PrimitiveType == item.BuiltInTypeKind) || ((BuiltInTypeKind.EdmFunction == item.BuiltInTypeKind) && (DataSpace.CSpace == item.DataSpace)))
            {
                return true;
            }
            GlobalItem item2 = null;
            if (((item.DataSpace == DataSpace.SSpace) || (item.DataSpace == DataSpace.CSpace)) && (this._owner.MetadataWorkspace.TryGetItem<GlobalItem>(item.Identity, item.DataSpace, out item2) && object.ReferenceEquals(item, item2)))
            {
                return true;
            }
            if (Helper.IsRowType(item))
            {
                foreach (EdmProperty property in ((RowType) item).Properties)
                {
                    if (!this.CheckWorkspaceAndDataSpace(property.TypeUsage))
                    {
                        return false;
                    }
                }
                return true;
            }
            if (Helper.IsCollectionType(item))
            {
                return this.CheckWorkspaceAndDataSpace(((CollectionType) item).TypeUsage);
            }
            return (Helper.IsRefType(item) && this.CheckWorkspaceAndDataSpace(((RefType) item).ElementType));
        }

        private bool CheckWorkspaceAndDataSpace(TypeUsage type) => 
            this.CheckWorkspaceAndDataSpace(type.EdmType);

        internal TypeUsage CreateBooleanResultType()
        {
            if (this._boolType == null)
            {
                this._boolType = this._owner.MetadataWorkspace.GetCanonicalModelTypeUsage(PrimitiveTypeKind.Boolean);
            }
            return this._boolType;
        }

        internal static TypeUsage CreateCollectionOfRowResultType(List<KeyValuePair<string, TypeUsage>> columns) => 
            TypeUsage.Create(TypeHelpers.CreateCollectionType(TypeUsage.Create(TypeHelpers.CreateRowType(columns))));

        internal static TypeUsage CreateCollectionResultType(EdmType type) => 
            TypeUsage.Create(TypeHelpers.CreateCollectionType(TypeUsage.Create(type)));

        internal static TypeUsage CreateCollectionResultType(TypeUsage type) => 
            TypeUsage.Create(TypeHelpers.CreateCollectionType(type));

        internal static TypeUsage CreateReferenceResultType(EntityTypeBase referencedEntityType) => 
            TypeUsage.Create(TypeHelpers.CreateReferenceType(referencedEntityType));

        internal static TypeUsage CreateResultType(EdmType resultType) => 
            TypeUsage.Create(resultType);

        internal static bool IsConstantNegativeInteger(DbExpression expression) => 
            (((expression.ExpressionKind == DbExpressionKind.Constant) && TypeSemantics.IsIntegerNumericType(expression.ResultType)) && (Convert.ToInt64(((DbConstantExpression) expression).Value, CultureInfo.InvariantCulture) < 0L));

        internal static TypeUsage SetResultAsNullable(TypeUsage typeUsage)
        {
            TypeUsage usage = typeUsage;
            if (!TypeSemantics.IsNullable(typeUsage))
            {
                FacetValues facetValues = new FacetValues {
                    Nullable = 1
                };
                usage = usage.ShallowCopy(facetValues);
            }
            return usage;
        }
    }
}


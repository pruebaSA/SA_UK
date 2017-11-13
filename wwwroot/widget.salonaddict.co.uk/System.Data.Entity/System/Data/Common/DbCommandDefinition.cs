namespace System.Data.Common
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;

    public class DbCommandDefinition
    {
        private readonly ICloneable _prototype;

        protected DbCommandDefinition()
        {
        }

        protected DbCommandDefinition(DbCommand prototype)
        {
            EntityUtil.CheckArgumentNull<DbCommand>(prototype, "prototype");
            this._prototype = prototype as ICloneable;
            if (this._prototype == null)
            {
                throw EntityUtil.CannotCloneStoreProvider();
            }
        }

        public virtual DbCommand CreateCommand() => 
            ((DbCommand) this._prototype.Clone());

        internal static DbCommandDefinition CreateCommandDefinition(DbCommand prototype)
        {
            EntityUtil.CheckArgumentNull<DbCommand>(prototype, "prototype");
            ICloneable cloneable = prototype as ICloneable;
            if (cloneable == null)
            {
                throw EntityUtil.CannotCloneStoreProvider();
            }
            return new DbCommandDefinition((DbCommand) cloneable.Clone());
        }

        private static void PopulateBinaryParameter(DbParameter parameter, TypeUsage type, DbType dbType, bool isOutParam)
        {
            parameter.DbType = dbType;
            SetParameterSize(parameter, type, isOutParam);
        }

        private static void PopulateDateTimeParameter(DbParameter parameter, TypeUsage type, DbType dbType)
        {
            byte num;
            parameter.DbType = dbType;
            IDbDataParameter parameter2 = parameter;
            if (TypeHelpers.TryGetPrecision(type, out num))
            {
                parameter2.Precision = num;
            }
        }

        private static void PopulateDecimalParameter(DbParameter parameter, TypeUsage type, DbType dbType)
        {
            byte num;
            byte num2;
            parameter.DbType = dbType;
            IDbDataParameter parameter2 = parameter;
            if (TypeHelpers.TryGetPrecision(type, out num))
            {
                parameter2.Precision = num;
            }
            if (TypeHelpers.TryGetScale(type, out num2))
            {
                parameter2.Scale = num2;
            }
        }

        internal static void PopulateParameterFromTypeUsage(DbParameter parameter, TypeUsage type, bool isOutParam)
        {
            EntityUtil.CheckArgumentNull<DbParameter>(parameter, "parameter");
            EntityUtil.CheckArgumentNull<TypeUsage>(type, "type");
            parameter.IsNullable = TypeSemantics.IsNullable(type);
            if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Binary))
            {
                PopulateBinaryParameter(parameter, type, DbType.Binary, isOutParam);
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Boolean))
            {
                parameter.DbType = DbType.Boolean;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Byte))
            {
                parameter.DbType = DbType.Byte;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.DateTime))
            {
                PopulateDateTimeParameter(parameter, type, DbType.DateTime);
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Time))
            {
                PopulateDateTimeParameter(parameter, type, DbType.Time);
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.DateTimeOffset))
            {
                PopulateDateTimeParameter(parameter, type, DbType.DateTimeOffset);
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Decimal))
            {
                PopulateDecimalParameter(parameter, type, DbType.Decimal);
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Double))
            {
                parameter.DbType = DbType.Double;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Guid))
            {
                parameter.DbType = DbType.Guid;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Single))
            {
                parameter.DbType = DbType.Single;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.SByte))
            {
                parameter.DbType = DbType.SByte;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Int16))
            {
                parameter.DbType = DbType.Int16;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Int32))
            {
                parameter.DbType = DbType.Int32;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.Int64))
            {
                parameter.DbType = DbType.Int64;
            }
            else if (TypeSemantics.IsPrimitiveType(type, PrimitiveTypeKind.String))
            {
                PopulateStringParameter(parameter, type, isOutParam);
            }
        }

        private static void PopulateStringParameter(DbParameter parameter, TypeUsage type, bool isOutParam)
        {
            bool isUnicode = true;
            bool isFixedLength = false;
            if (!TypeHelpers.TryGetIsFixedLength(type, out isFixedLength))
            {
                isFixedLength = false;
            }
            if (!TypeHelpers.TryGetIsUnicode(type, out isUnicode))
            {
                isUnicode = true;
            }
            if (isFixedLength)
            {
                parameter.DbType = isUnicode ? DbType.StringFixedLength : DbType.AnsiStringFixedLength;
            }
            else
            {
                parameter.DbType = isUnicode ? DbType.String : DbType.AnsiString;
            }
            SetParameterSize(parameter, type, isOutParam);
        }

        private static void SetParameterSize(DbParameter parameter, TypeUsage type, bool isOutParam)
        {
            Facet facet;
            if (type.Facets.TryGetValue("MaxLength", true, out facet) && (facet.Value != null))
            {
                if (!Helper.IsUnboundedFacetValue(facet))
                {
                    parameter.Size = (int) facet.Value;
                }
                else if (isOutParam)
                {
                    parameter.Size = 0x7fffffff;
                }
            }
        }
    }
}


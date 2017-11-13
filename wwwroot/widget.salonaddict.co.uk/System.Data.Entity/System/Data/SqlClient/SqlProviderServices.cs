namespace System.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.SqlClient.SqlGen;
    using System.Runtime.InteropServices;

    internal sealed class SqlProviderServices : DbProviderServices
    {
        internal static readonly SqlProviderServices Instance = new SqlProviderServices();

        internal override DbCommand CreateCommand(DbCommandTree commandTree)
        {
            EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
            StoreItemCollection itemCollection = (StoreItemCollection) commandTree.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
            return this.CreateCommand(itemCollection.StoreProviderManifest, commandTree);
        }

        private DbCommand CreateCommand(DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            List<SqlParameter> list;
            CommandType type;
            EntityUtil.CheckArgumentNull<DbProviderManifest>(providerManifest, "providerManifest");
            EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
            SqlProviderManifest manifest = providerManifest as SqlProviderManifest;
            if (manifest == null)
            {
                throw EntityUtil.Argument(Strings.Mapping_Provider_WrongManifestType(typeof(SqlProviderManifest)));
            }
            SqlVersion sqlVersion = manifest.SqlVersion;
            SqlCommand command = new SqlCommand();
            EntityBid.Trace("<sc.SqlProviderServices.CreateCommandDefinition|ADV> sqlVersion=%d commandTree=%d#\n", (int) sqlVersion, commandTree.ObjectId);
            command.CommandText = SqlGenerator.GenerateSql(commandTree, sqlVersion, out list, out type);
            command.CommandType = type;
            EntityBid.Trace("<sc.SqlProviderServices.CreateCommandDefinition|ADV> Generated SQL=%s\n", command.CommandText);
            EdmFunction edmFunction = null;
            if (commandTree.CommandTreeKind == DbCommandTreeKind.Function)
            {
                edmFunction = ((DbFunctionCommandTree) commandTree).EdmFunction;
            }
            foreach (KeyValuePair<string, TypeUsage> pair in commandTree.Parameters)
            {
                SqlParameter parameter;
                FunctionParameter parameter2;
                if ((edmFunction != null) && edmFunction.Parameters.TryGetValue(pair.Key, false, out parameter2))
                {
                    parameter = CreateSqlParameter(parameter2.Name, parameter2.TypeUsage, parameter2.Mode, DBNull.Value, false, sqlVersion);
                }
                else
                {
                    parameter = CreateSqlParameter(pair.Key, pair.Value, ParameterMode.In, DBNull.Value, false, sqlVersion);
                }
                command.Parameters.Add(parameter);
            }
            if ((list != null) && (0 < list.Count))
            {
                if (((commandTree.CommandTreeKind != DbCommandTreeKind.Delete) && (commandTree.CommandTreeKind != DbCommandTreeKind.Insert)) && (commandTree.CommandTreeKind != DbCommandTreeKind.Update))
                {
                    throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.SqlGenParametersNotPermitted);
                }
                foreach (SqlParameter parameter3 in list)
                {
                    command.Parameters.Add(parameter3);
                }
            }
            return command;
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            DbCommand prototype = this.CreateCommand(providerManifest, commandTree);
            return this.CreateCommandDefinition(prototype);
        }

        internal static SqlParameter CreateSqlParameter(string name, TypeUsage type, ParameterMode mode, object value, bool ignoreMaxLengthFacet, SqlVersion version)
        {
            int? nullable;
            byte? nullable2;
            byte? nullable3;
            SqlParameter parameter = new SqlParameter(name, value);
            ParameterDirection direction = MetadataHelper.ParameterModeToParameterDirection(mode);
            if (parameter.Direction != direction)
            {
                parameter.Direction = direction;
            }
            bool isOutParam = mode != ParameterMode.In;
            SqlDbType type2 = GetSqlDbType(type, isOutParam, version, out nullable, out nullable2, out nullable3);
            if (parameter.SqlDbType != type2)
            {
                parameter.SqlDbType = type2;
            }
            if ((!ignoreMaxLengthFacet && nullable.HasValue) && (isOutParam || (parameter.Size != nullable.Value)))
            {
                parameter.Size = nullable.Value;
            }
            if (nullable2.HasValue && (isOutParam || (parameter.Precision != nullable2.Value)))
            {
                parameter.Precision = nullable2.Value;
            }
            if (nullable3.HasValue && (isOutParam || (parameter.Scale != nullable3.Value)))
            {
                parameter.Scale = nullable3.Value;
            }
            bool flag2 = TypeSemantics.IsNullable(type);
            if (isOutParam || (flag2 != parameter.IsNullable))
            {
                parameter.IsNullable = flag2;
            }
            return parameter;
        }

        private static SqlDbType GetBinaryDbType(TypeUsage type)
        {
            bool flag;
            if (!TypeHelpers.TryGetIsFixedLength(type, out flag))
            {
                flag = false;
            }
            if (!flag)
            {
                return SqlDbType.VarBinary;
            }
            return SqlDbType.Binary;
        }

        protected override DbProviderManifest GetDbProviderManifest(string versionHint)
        {
            if (string.IsNullOrEmpty(versionHint))
            {
                throw EntityUtil.Argument(Strings.UnableToDetermineStoreVersion);
            }
            return new SqlProviderManifest(versionHint);
        }

        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            string versionHint;
            EntityUtil.CheckArgumentNull<DbConnection>(connection, "connection");
            SqlConnection connection2 = connection as SqlConnection;
            if (connection2 == null)
            {
                throw EntityUtil.Argument(Strings.Mapping_Provider_WrongConnectionType(typeof(SqlConnection)));
            }
            if (string.IsNullOrEmpty(connection2.ConnectionString))
            {
                throw EntityUtil.Argument(Strings.UnableToDetermineStoreVersion);
            }
            bool flag = false;
            try
            {
                if (connection2.State != ConnectionState.Open)
                {
                    connection2.Open();
                    flag = true;
                }
                versionHint = SqlVersionUtils.GetVersionHint(SqlVersionUtils.GetSqlVersion(connection2));
            }
            finally
            {
                if (flag)
                {
                    connection2.Close();
                }
            }
            return versionHint;
        }

        private static byte? GetKatmaiDateTimePrecision(TypeUsage type, bool isOutParam)
        {
            byte? defaultIfUndefined = isOutParam ? ((byte?) 7) : null;
            return GetParameterPrecision(type, defaultIfUndefined);
        }

        private static byte? GetParameterPrecision(TypeUsage type, byte? defaultIfUndefined)
        {
            byte num;
            if (TypeHelpers.TryGetPrecision(type, out num))
            {
                return new byte?(num);
            }
            return defaultIfUndefined;
        }

        private static int? GetParameterSize(TypeUsage type, bool isOutParam)
        {
            int num;
            if (TypeHelpers.TryGetMaxLength(type, out num))
            {
                return new int?(num);
            }
            if (isOutParam)
            {
                return 0x7fffffff;
            }
            return null;
        }

        private static byte? GetScale(TypeUsage type)
        {
            byte num;
            if (TypeHelpers.TryGetScale(type, out num))
            {
                return new byte?(num);
            }
            return null;
        }

        private static SqlDbType GetSqlDbType(TypeUsage type, bool isOutParam, SqlVersion version, out int? size, out byte? precision, out byte? scale)
        {
            PrimitiveTypeKind primitiveTypeKind = MetadataHelper.GetPrimitiveTypeKind(type);
            size = 0;
            precision = 0;
            scale = 0;
            switch (primitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary:
                    size = GetParameterSize(type, isOutParam);
                    return GetBinaryDbType(type);

                case PrimitiveTypeKind.Boolean:
                    return SqlDbType.Bit;

                case PrimitiveTypeKind.Byte:
                    return SqlDbType.TinyInt;

                case PrimitiveTypeKind.DateTime:
                    if (SqlVersionUtils.IsPreKatmai(version))
                    {
                        return SqlDbType.DateTime;
                    }
                    precision = GetKatmaiDateTimePrecision(type, isOutParam);
                    return SqlDbType.DateTime2;

                case PrimitiveTypeKind.Decimal:
                    precision = GetParameterPrecision(type, null);
                    scale = GetScale(type);
                    return SqlDbType.Decimal;

                case PrimitiveTypeKind.Double:
                    return SqlDbType.Float;

                case PrimitiveTypeKind.Guid:
                    return SqlDbType.UniqueIdentifier;

                case PrimitiveTypeKind.Single:
                    return SqlDbType.Real;

                case PrimitiveTypeKind.SByte:
                    return SqlDbType.SmallInt;

                case PrimitiveTypeKind.Int16:
                    return SqlDbType.SmallInt;

                case PrimitiveTypeKind.Int32:
                    return SqlDbType.Int;

                case PrimitiveTypeKind.Int64:
                    return SqlDbType.BigInt;

                case PrimitiveTypeKind.String:
                    size = GetParameterSize(type, isOutParam);
                    return GetStringDbType(type);

                case PrimitiveTypeKind.Time:
                    if (!SqlVersionUtils.IsPreKatmai(version))
                    {
                        precision = GetKatmaiDateTimePrecision(type, isOutParam);
                    }
                    return SqlDbType.Time;

                case PrimitiveTypeKind.DateTimeOffset:
                    if (!SqlVersionUtils.IsPreKatmai(version))
                    {
                        precision = GetKatmaiDateTimePrecision(type, isOutParam);
                    }
                    return SqlDbType.DateTimeOffset;
            }
            return SqlDbType.Variant;
        }

        private static SqlDbType GetStringDbType(TypeUsage type)
        {
            bool flag;
            bool flag2;
            if (type.EdmType.Name.ToLowerInvariant() == "xml")
            {
                return SqlDbType.Xml;
            }
            if (!TypeHelpers.TryGetIsFixedLength(type, out flag2))
            {
                flag2 = false;
            }
            if (!TypeHelpers.TryGetIsUnicode(type, out flag))
            {
                flag = true;
            }
            if (flag2)
            {
                return (flag ? SqlDbType.NChar : SqlDbType.Char);
            }
            return (flag ? SqlDbType.NVarChar : SqlDbType.VarChar);
        }
    }
}


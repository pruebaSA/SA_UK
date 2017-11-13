namespace System.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal class SqlProviderManifest : DbXmlEnabledProviderManifest
    {
        private System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> _functions;
        private System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> _primitiveTypes;
        private System.Data.SqlClient.SqlVersion _version;
        private const int binaryMaxSize = 0x1f40;
        private const int nvarcharMaxSize = 0xfa0;
        internal const string TokenSql10 = "2008";
        internal const string TokenSql8 = "2000";
        internal const string TokenSql9 = "2005";
        private const int varcharMaxSize = 0x1f40;

        public SqlProviderManifest(string manifestToken) : base(GetProviderManifest())
        {
            this._version = System.Data.SqlClient.SqlVersion.Sql9;
            this._version = SqlVersionUtils.GetSqlVersion(manifestToken);
        }

        protected override XmlReader GetDbInformation(string informationType)
        {
            if (informationType == DbProviderManifest.StoreSchemaDefinition)
            {
                return this.GetStoreSchemaDescription();
            }
            if (informationType == DbProviderManifest.StoreSchemaMapping)
            {
                return this.GetStoreSchemaMapping();
            }
            if (informationType != DbProviderManifest.ConceptualSchemaDefinition)
            {
                throw EntityUtil.ProviderIncompatible(Strings.ProviderReturnedNullForGetDbInformation(informationType));
            }
            return null;
        }

        public override TypeUsage GetEdmType(TypeUsage storeType)
        {
            PrimitiveTypeKind binary;
            EntityUtil.CheckArgumentNull<TypeUsage>(storeType, "storeType");
            string key = storeType.EdmType.Name.ToLowerInvariant();
            if (!base.StoreTypeNameToEdmPrimitiveType.ContainsKey(key))
            {
                throw EntityUtil.Argument(Strings.ProviderDoesNotSupportType(key));
            }
            PrimitiveType edmType = base.StoreTypeNameToEdmPrimitiveType[key];
            int maxLength = 0;
            bool isUnicode = true;
            bool isFixedLength = false;
            bool flag3 = true;
            switch (key)
            {
                case "tinyint":
                case "smallint":
                case "bigint":
                case "bit":
                case "uniqueidentifier":
                case "int":
                    return TypeUsage.CreateDefaultTypeUsage(edmType);

                case "varchar":
                    binary = PrimitiveTypeKind.String;
                    flag3 = !TypeHelpers.TryGetMaxLength(storeType, out maxLength);
                    isUnicode = false;
                    isFixedLength = false;
                    break;

                case "char":
                    binary = PrimitiveTypeKind.String;
                    flag3 = !TypeHelpers.TryGetMaxLength(storeType, out maxLength);
                    isUnicode = false;
                    isFixedLength = true;
                    break;

                case "nvarchar":
                    binary = PrimitiveTypeKind.String;
                    flag3 = !TypeHelpers.TryGetMaxLength(storeType, out maxLength);
                    isUnicode = true;
                    isFixedLength = false;
                    break;

                case "nchar":
                    binary = PrimitiveTypeKind.String;
                    flag3 = !TypeHelpers.TryGetMaxLength(storeType, out maxLength);
                    isUnicode = true;
                    isFixedLength = true;
                    break;

                case "varchar(max)":
                case "text":
                    binary = PrimitiveTypeKind.String;
                    flag3 = true;
                    isUnicode = false;
                    isFixedLength = false;
                    break;

                case "nvarchar(max)":
                case "ntext":
                case "xml":
                    binary = PrimitiveTypeKind.String;
                    flag3 = true;
                    isUnicode = true;
                    isFixedLength = false;
                    break;

                case "binary":
                    binary = PrimitiveTypeKind.Binary;
                    flag3 = !TypeHelpers.TryGetMaxLength(storeType, out maxLength);
                    isFixedLength = true;
                    break;

                case "varbinary":
                    binary = PrimitiveTypeKind.Binary;
                    flag3 = !TypeHelpers.TryGetMaxLength(storeType, out maxLength);
                    isFixedLength = false;
                    break;

                case "varbinary(max)":
                case "image":
                    binary = PrimitiveTypeKind.Binary;
                    flag3 = true;
                    isFixedLength = false;
                    break;

                case "timestamp":
                case "rowversion":
                    return TypeUsage.CreateBinaryTypeUsage(edmType, true, 8);

                case "float":
                case "real":
                    return TypeUsage.CreateDefaultTypeUsage(edmType);

                case "decimal":
                case "numeric":
                    byte num2;
                    byte num3;
                    if (!TypeHelpers.TryGetPrecision(storeType, out num2) || !TypeHelpers.TryGetScale(storeType, out num3))
                    {
                        return TypeUsage.CreateDecimalTypeUsage(edmType);
                    }
                    return TypeUsage.CreateDecimalTypeUsage(edmType, num2, num3);

                case "money":
                    return TypeUsage.CreateDecimalTypeUsage(edmType, 0x13, 4);

                case "smallmoney":
                    return TypeUsage.CreateDecimalTypeUsage(edmType, 10, 4);

                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return TypeUsage.CreateDateTimeTypeUsage(edmType, null);

                case "date":
                    return TypeUsage.CreateDefaultTypeUsage(edmType);

                case "time":
                    return TypeUsage.CreateTimeTypeUsage(edmType, null);

                case "datetimeoffset":
                    return TypeUsage.CreateDateTimeOffsetTypeUsage(edmType, null);

                default:
                    throw EntityUtil.NotSupported(Strings.ProviderDoesNotSupportType(key));
            }
            PrimitiveTypeKind kind2 = binary;
            if (kind2 != PrimitiveTypeKind.Binary)
            {
                if (kind2 != PrimitiveTypeKind.String)
                {
                    throw EntityUtil.NotSupported(Strings.ProviderDoesNotSupportType(key));
                }
                if (!flag3)
                {
                    return TypeUsage.CreateStringTypeUsage(edmType, isUnicode, isFixedLength, maxLength);
                }
                return TypeUsage.CreateStringTypeUsage(edmType, isUnicode, isFixedLength);
            }
            if (!flag3)
            {
                return TypeUsage.CreateBinaryTypeUsage(edmType, isFixedLength, maxLength);
            }
            return TypeUsage.CreateBinaryTypeUsage(edmType, isFixedLength);
        }

        private static XmlReader GetProviderManifest() => 
            DbProviderServices.GetXmlResource("System.Data.Resources.SqlClient.SqlProviderServices.ProviderManifest.xml");

        public override System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> GetStoreFunctions()
        {
            if (this._functions == null)
            {
                if (this._version == System.Data.SqlClient.SqlVersion.Sql10)
                {
                    this._functions = base.GetStoreFunctions();
                }
                else
                {
                    List<EdmFunction> list = new List<EdmFunction>(base.GetStoreFunctions());
                    list.RemoveAll(delegate (EdmFunction edmFunction) {
                        ReadOnlyMetadataCollection<FunctionParameter> parameters = edmFunction.Parameters;
                        switch (edmFunction.Name.ToUpperInvariant())
                        {
                            case "COUNT":
                            case "COUNT_BIG":
                            case "MAX":
                            case "MIN":
                            {
                                string name = ((CollectionType) parameters[0].TypeUsage.EdmType).TypeUsage.EdmType.Name;
                                return name.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase) || name.Equals("Time", StringComparison.OrdinalIgnoreCase);
                            }
                            case "DAY":
                            case "MONTH":
                            case "YEAR":
                            case "DATALENGTH":
                            case "CHECKSUM":
                            {
                                string str2 = parameters[0].TypeUsage.EdmType.Name;
                                return str2.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase) || str2.Equals("Time", StringComparison.OrdinalIgnoreCase);
                            }
                            case "DATEADD":
                            case "DATEDIFF":
                            {
                                string str3 = parameters[1].TypeUsage.EdmType.Name;
                                string str4 = parameters[2].TypeUsage.EdmType.Name;
                                return ((str3.Equals("Time", StringComparison.OrdinalIgnoreCase) || str4.Equals("Time", StringComparison.OrdinalIgnoreCase)) || str3.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase)) || str4.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase);
                            }
                            case "DATENAME":
                            case "DATEPART":
                            {
                                string str5 = parameters[1].TypeUsage.EdmType.Name;
                                return str5.Equals("DateTimeOffset", StringComparison.OrdinalIgnoreCase) || str5.Equals("Time", StringComparison.OrdinalIgnoreCase);
                            }
                            case "SYSUTCDATETIME":
                            case "SYSDATETIME":
                            case "SYSDATETIMEOFFSET":
                                return true;
                        }
                        return false;
                    });
                    if (this._version == System.Data.SqlClient.SqlVersion.Sql8)
                    {
                        list.RemoveAll(delegate (EdmFunction edmFunction) {
                            ReadOnlyMetadataCollection<FunctionParameter> parameters = edmFunction.Parameters;
                            if ((parameters != null) && (parameters.Count != 0))
                            {
                                switch (edmFunction.Name.ToUpperInvariant())
                                {
                                    case "COUNT":
                                    case "COUNT_BIG":
                                        return ((CollectionType) parameters[0].TypeUsage.EdmType).TypeUsage.EdmType.Name.Equals("Guid", StringComparison.OrdinalIgnoreCase);

                                    case "CHARINDEX":
                                        foreach (FunctionParameter parameter in parameters)
                                        {
                                            if (parameter.TypeUsage.EdmType.Name.Equals("Int64", StringComparison.OrdinalIgnoreCase))
                                            {
                                                return true;
                                            }
                                        }
                                        break;
                                }
                            }
                            return false;
                        });
                    }
                    this._functions = list.AsReadOnly();
                }
            }
            return this._functions;
        }

        private XmlReader GetStoreSchemaDescription()
        {
            if (this._version == System.Data.SqlClient.SqlVersion.Sql8)
            {
                return DbProviderServices.GetXmlResource("System.Data.Resources.SqlClient.SqlProviderServices.StoreSchemaDefinition_Sql8.ssdl");
            }
            return DbProviderServices.GetXmlResource("System.Data.Resources.SqlClient.SqlProviderServices.StoreSchemaDefinition.ssdl");
        }

        private XmlReader GetStoreSchemaMapping() => 
            DbProviderServices.GetXmlResource("System.Data.Resources.SqlClient.SqlProviderServices.StoreSchemaMapping.msl");

        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            EntityUtil.CheckArgumentNull<TypeUsage>(edmType, "edmType");
            PrimitiveType type = edmType.EdmType as PrimitiveType;
            if (type == null)
            {
                throw EntityUtil.Argument(Strings.ProviderDoesNotSupportType(edmType.Identity));
            }
            ReadOnlyMetadataCollection<Facet> facets = edmType.Facets;
            switch (type.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary:
                {
                    bool flag = (facets["FixedLength"].Value != null) && ((bool) facets["FixedLength"].Value);
                    Facet facet = facets["MaxLength"];
                    bool flag2 = (Helper.IsUnboundedFacetValue(facet) || (facet.Value == null)) || (((int) facet.Value) > 0x1f40);
                    int maxLength = !flag2 ? ((int) facet.Value) : -2147483648;
                    if (!flag)
                    {
                        if (flag2)
                        {
                            if (this._version != System.Data.SqlClient.SqlVersion.Sql8)
                            {
                                return TypeUsage.CreateBinaryTypeUsage(base.StoreTypeNameToStorePrimitiveType["varbinary(max)"], false);
                            }
                            return TypeUsage.CreateBinaryTypeUsage(base.StoreTypeNameToStorePrimitiveType["varbinary"], false, 0x1f40);
                        }
                        return TypeUsage.CreateBinaryTypeUsage(base.StoreTypeNameToStorePrimitiveType["varbinary"], false, maxLength);
                    }
                    return TypeUsage.CreateBinaryTypeUsage(base.StoreTypeNameToStorePrimitiveType["binary"], true, flag2 ? 0x1f40 : maxLength);
                }
                case PrimitiveTypeKind.Boolean:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["bit"]);

                case PrimitiveTypeKind.Byte:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["tinyint"]);

                case PrimitiveTypeKind.DateTime:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["datetime"]);

                case PrimitiveTypeKind.Decimal:
                    byte num;
                    byte num2;
                    if (!TypeHelpers.TryGetPrecision(edmType, out num))
                    {
                        num = 0x12;
                    }
                    if (!TypeHelpers.TryGetScale(edmType, out num2))
                    {
                        num2 = 0;
                    }
                    return TypeUsage.CreateDecimalTypeUsage(base.StoreTypeNameToStorePrimitiveType["decimal"], num, num2);

                case PrimitiveTypeKind.Double:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["float"]);

                case PrimitiveTypeKind.Guid:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["uniqueidentifier"]);

                case PrimitiveTypeKind.Single:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["real"]);

                case PrimitiveTypeKind.Int16:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["smallint"]);

                case PrimitiveTypeKind.Int32:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["int"]);

                case PrimitiveTypeKind.Int64:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["bigint"]);

                case PrimitiveTypeKind.String:
                {
                    bool flag3 = (facets["Unicode"].Value == null) || ((bool) facets["Unicode"].Value);
                    bool flag4 = (facets["FixedLength"].Value != null) && ((bool) facets["FixedLength"].Value);
                    Facet facet2 = facets["MaxLength"];
                    bool flag5 = (Helper.IsUnboundedFacetValue(facet2) || (facet2.Value == null)) || (((int) facet2.Value) > (flag3 ? 0xfa0 : 0x1f40));
                    int num4 = !flag5 ? ((int) facet2.Value) : -2147483648;
                    if (!flag3)
                    {
                        if (flag4)
                        {
                            return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["char"], false, true, flag5 ? 0x1f40 : num4);
                        }
                        if (flag5)
                        {
                            if (this._version != System.Data.SqlClient.SqlVersion.Sql8)
                            {
                                return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["varchar(max)"], false, false);
                            }
                            return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["varchar"], false, false, 0x1f40);
                        }
                        return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["varchar"], false, false, num4);
                    }
                    if (!flag4)
                    {
                        if (flag5)
                        {
                            if (this._version != System.Data.SqlClient.SqlVersion.Sql8)
                            {
                                return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["nvarchar(max)"], true, false);
                            }
                            return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["nvarchar"], true, false, 0xfa0);
                        }
                        return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["nvarchar"], true, false, num4);
                    }
                    return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["nchar"], true, true, flag5 ? 0xfa0 : num4);
                }
                case PrimitiveTypeKind.Time:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["time"]);

                case PrimitiveTypeKind.DateTimeOffset:
                    return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["datetimeoffset"]);
            }
            throw EntityUtil.NotSupported(Strings.NoStoreTypeForEdmType(edmType.Identity, type.PrimitiveTypeKind));
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetStoreTypes()
        {
            if (this._primitiveTypes == null)
            {
                if (this._version == System.Data.SqlClient.SqlVersion.Sql10)
                {
                    this._primitiveTypes = base.GetStoreTypes();
                }
                else
                {
                    List<PrimitiveType> list = new List<PrimitiveType>(base.GetStoreTypes());
                    list.RemoveAll(delegate (PrimitiveType primitiveType) {
                        string str = primitiveType.Name.ToLowerInvariant();
                        if ((!str.Equals("time", StringComparison.Ordinal) && !str.Equals("date", StringComparison.Ordinal)) && !str.Equals("datetime2", StringComparison.Ordinal))
                        {
                            return str.Equals("datetimeoffset", StringComparison.Ordinal);
                        }
                        return true;
                    });
                    if (this._version == System.Data.SqlClient.SqlVersion.Sql8)
                    {
                        list.RemoveAll(delegate (PrimitiveType primitiveType) {
                            string str = primitiveType.Name.ToLowerInvariant();
                            if (!str.Equals("xml", StringComparison.Ordinal))
                            {
                                return str.EndsWith("(max)", StringComparison.Ordinal);
                            }
                            return true;
                        });
                    }
                    this._primitiveTypes = list.AsReadOnly();
                }
            }
            return this._primitiveTypes;
        }

        internal System.Data.SqlClient.SqlVersion SqlVersion =>
            this._version;
    }
}


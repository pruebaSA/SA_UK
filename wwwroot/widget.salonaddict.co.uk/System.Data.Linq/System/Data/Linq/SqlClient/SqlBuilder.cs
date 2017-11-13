namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal static class SqlBuilder
    {
        internal static void BuildFieldDeclarations(MetaTable table, StringBuilder sb)
        {
            int num = 0;
            Dictionary<object, string> memberNameToMappedName = new Dictionary<object, string>();
            foreach (MetaType type in table.RowType.InheritanceTypes)
            {
                num += BuildFieldDeclarations(type, memberNameToMappedName, sb);
            }
            if (num == 0)
            {
                throw System.Data.Linq.SqlClient.Error.CreateDatabaseFailedBecauseOfClassWithNoMembers(table.RowType.Type);
            }
        }

        private static int BuildFieldDeclarations(MetaType type, Dictionary<object, string> memberNameToMappedName, StringBuilder sb)
        {
            int num = 0;
            foreach (MetaDataMember member in type.DataMembers)
            {
                string str;
                if ((!member.IsDeclaredBy(type) || member.IsAssociation) || !member.IsPersistent)
                {
                    continue;
                }
                object key = InheritanceRules.DistinguishedMemberName(member.Member);
                if (memberNameToMappedName.TryGetValue(key, out str))
                {
                    if (str != member.MappedName)
                    {
                        goto Label_0075;
                    }
                    continue;
                }
                memberNameToMappedName.Add(key, member.MappedName);
            Label_0075:
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.AppendLine();
                sb.Append(string.Format(CultureInfo.InvariantCulture, "  {0} ", new object[] { SqlIdentifier.QuoteCompoundIdentifier(member.MappedName) }));
                if (!string.IsNullOrEmpty(member.Expression))
                {
                    sb.Append("AS " + member.Expression);
                }
                else
                {
                    sb.Append(GetDbType(member));
                }
                num++;
            }
            return num;
        }

        private static string BuildKey(IEnumerable<MetaDataMember> members)
        {
            StringBuilder builder = new StringBuilder();
            foreach (MetaDataMember member in members)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(SqlIdentifier.QuoteCompoundIdentifier(member.MappedName));
            }
            return builder.ToString();
        }

        private static void BuildPrimaryKey(MetaTable table, StringBuilder sb)
        {
            foreach (MetaDataMember member in table.RowType.IdentityMembers)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(SqlIdentifier.QuoteCompoundIdentifier(member.MappedName));
            }
        }

        internal static string GetCreateDatabaseCommand(string catalog, string dataFilename, string logFilename)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("CREATE DATABASE {0}", SqlIdentifier.QuoteIdentifier(catalog));
            if (dataFilename != null)
            {
                builder.AppendFormat(" ON PRIMARY (NAME='{0}', FILENAME='{1}')", Path.GetFileName(dataFilename), dataFilename);
                builder.AppendFormat(" LOG ON (NAME='{0}', FILENAME='{1}')", Path.GetFileName(logFilename), logFilename);
            }
            return builder.ToString();
        }

        internal static IEnumerable<string> GetCreateForeignKeyCommands(MetaTable table)
        {
            foreach (MetaType iteratorVariable0 in table.RowType.InheritanceTypes)
            {
                foreach (string iteratorVariable1 in GetCreateForeignKeyCommands(iteratorVariable0))
                {
                    yield return iteratorVariable1;
                }
            }
        }

        private static IEnumerable<string> GetCreateForeignKeyCommands(MetaType type)
        {
            string tableName = type.Table.TableName;
            foreach (MetaDataMember iteratorVariable1 in type.DataMembers)
            {
                if (iteratorVariable1.IsDeclaredBy(type) && iteratorVariable1.IsAssociation)
                {
                    MetaAssociation association = iteratorVariable1.Association;
                    if (association.IsForeignKey)
                    {
                        StringBuilder iteratorVariable3 = new StringBuilder();
                        string s = BuildKey(association.ThisKey);
                        string iteratorVariable5 = BuildKey(association.OtherKey);
                        string iteratorVariable6 = association.OtherType.Table.TableName;
                        string mappedName = iteratorVariable1.MappedName;
                        if (mappedName == iteratorVariable1.Name)
                        {
                            mappedName = string.Format(CultureInfo.InvariantCulture, "FK_{0}_{1}", new object[] { tableName, iteratorVariable1.Name });
                        }
                        string format = "ALTER TABLE {0}" + Environment.NewLine + "  ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3}({4})";
                        MetaDataMember otherMember = iteratorVariable1.Association.OtherMember;
                        if (otherMember != null)
                        {
                            string deleteRule = otherMember.Association.DeleteRule;
                            if (deleteRule != null)
                            {
                                format = format + Environment.NewLine + "  ON DELETE " + deleteRule;
                            }
                        }
                        iteratorVariable3.AppendFormat(format, new object[] { SqlIdentifier.QuoteCompoundIdentifier(tableName), SqlIdentifier.QuoteIdentifier(mappedName), SqlIdentifier.QuoteCompoundIdentifier(s), SqlIdentifier.QuoteCompoundIdentifier(iteratorVariable6), SqlIdentifier.QuoteCompoundIdentifier(iteratorVariable5) });
                        yield return iteratorVariable3.ToString();
                    }
                }
            }
        }

        internal static string GetCreateSchemaForTableCommand(MetaTable table)
        {
            StringBuilder builder = new StringBuilder();
            List<string> list = new List<string>(SqlIdentifier.GetCompoundIdentifierParts(table.TableName));
            if (list.Count < 2)
            {
                return null;
            }
            string strA = list[list.Count - 2];
            if ((string.Compare(strA, "DBO", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(strA, "[DBO]", StringComparison.OrdinalIgnoreCase) != 0))
            {
                builder.AppendFormat("CREATE SCHEMA {0}", SqlIdentifier.QuoteIdentifier(strA));
            }
            return builder.ToString();
        }

        internal static string GetCreateTableCommand(MetaTable table)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            BuildFieldDeclarations(table, sb);
            builder.AppendFormat("CREATE TABLE {0}", SqlIdentifier.QuoteCompoundIdentifier(table.TableName));
            builder.Append("(");
            builder.Append(sb.ToString());
            sb = new StringBuilder();
            BuildPrimaryKey(table, sb);
            if (sb.Length > 0)
            {
                string s = string.Format(CultureInfo.InvariantCulture, "PK_{0}", new object[] { table.TableName });
                builder.Append(", ");
                builder.AppendLine();
                builder.AppendFormat("  CONSTRAINT {0} PRIMARY KEY ({1})", SqlIdentifier.QuoteIdentifier(s), sb.ToString());
            }
            builder.AppendLine();
            builder.Append("  )");
            return builder.ToString();
        }

        private static string GetDbType(MetaDataMember mm)
        {
            string dbType = mm.DbType;
            if (dbType != null)
            {
                return dbType;
            }
            StringBuilder builder = new StringBuilder();
            Type type = mm.Type;
            bool canBeNull = mm.CanBeNull;
            if (type.IsValueType && IsNullable(type))
            {
                type = type.GetGenericArguments()[0];
            }
            if (mm.IsVersion)
            {
                builder.Append("Timestamp");
            }
            else if (mm.IsPrimaryKey && mm.IsDbGenerated)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Object:
                        if (type != typeof(Guid))
                        {
                            throw System.Data.Linq.SqlClient.Error.CouldNotDetermineDbGeneratedSqlType(type);
                        }
                        builder.Append("UniqueIdentifier");
                        break;

                    case TypeCode.SByte:
                    case TypeCode.Int16:
                        builder.Append("SmallInt");
                        break;

                    case TypeCode.Byte:
                        builder.Append("TinyInt");
                        break;

                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                        builder.Append("Int");
                        break;

                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                        builder.Append("BigInt");
                        break;

                    case TypeCode.UInt64:
                    case TypeCode.Decimal:
                        builder.Append("Decimal(20)");
                        break;
                }
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Object:
                        if (type != typeof(Guid))
                        {
                            if (type == typeof(byte[]))
                            {
                                builder.Append("VarBinary(8000)");
                            }
                            else if (type == typeof(char[]))
                            {
                                builder.Append("NVarChar(4000)");
                            }
                            else if (type == typeof(DateTimeOffset))
                            {
                                builder.Append("DateTimeOffset");
                            }
                            else
                            {
                                if (type != typeof(TimeSpan))
                                {
                                    throw System.Data.Linq.SqlClient.Error.CouldNotDetermineSqlType(type);
                                }
                                builder.Append("Time");
                            }
                            break;
                        }
                        builder.Append("UniqueIdentifier");
                        break;

                    case TypeCode.Boolean:
                        builder.Append("Bit");
                        break;

                    case TypeCode.Char:
                        builder.Append("NChar(1)");
                        break;

                    case TypeCode.SByte:
                    case TypeCode.Int16:
                        builder.Append("SmallInt");
                        break;

                    case TypeCode.Byte:
                        builder.Append("TinyInt");
                        break;

                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                        builder.Append("Int");
                        break;

                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                        builder.Append("BigInt");
                        break;

                    case TypeCode.UInt64:
                        builder.Append("Decimal(20)");
                        break;

                    case TypeCode.Single:
                        builder.Append("Real");
                        break;

                    case TypeCode.Double:
                        builder.Append("Float");
                        break;

                    case TypeCode.Decimal:
                        builder.Append("Decimal(29, 4)");
                        break;

                    case TypeCode.DateTime:
                        builder.Append("DateTime");
                        break;

                    case TypeCode.String:
                        builder.Append("NVarChar(4000)");
                        break;
                }
            }
            if (!canBeNull)
            {
                builder.Append(" NOT NULL");
            }
            if (mm.IsPrimaryKey && mm.IsDbGenerated)
            {
                if (type == typeof(Guid))
                {
                    builder.Append(" DEFAULT NEWID()");
                }
                else
                {
                    builder.Append(" IDENTITY");
                }
            }
            return builder.ToString();
        }

        internal static string GetDropDatabaseCommand(string catalog)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("DROP DATABASE {0}", SqlIdentifier.QuoteIdentifier(catalog));
            return builder.ToString();
        }

        internal static bool IsNullable(Type type) => 
            (type.IsGenericType && typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()));


    }
}


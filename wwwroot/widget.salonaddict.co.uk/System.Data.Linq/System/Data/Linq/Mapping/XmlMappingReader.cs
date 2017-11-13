namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;

    internal class XmlMappingReader
    {
        private static void AssertEmptyElement(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
            {
                string name = reader.Name;
                reader.Read();
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    throw Error.ExpectedEmptyElement(name, reader.NodeType, reader.Name);
                }
            }
            reader.Skip();
        }

        internal static bool IsInNamespace(XmlReader reader) => 
            (reader.LookupNamespace(reader.Prefix) == "http://schemas.microsoft.com/linqtosql/mapping/2007");

        private static string OptionalAttribute(XmlReader reader, string attribute) => 
            reader.GetAttribute(attribute);

        private static bool OptionalBoolAttribute(XmlReader reader, string attribute, bool @default)
        {
            string str = OptionalAttribute(reader, attribute);
            if (str == null)
            {
                return @default;
            }
            return bool.Parse(str);
        }

        private static bool? OptionalNullableBoolAttribute(XmlReader reader, string attribute)
        {
            string str = OptionalAttribute(reader, attribute);
            if (str == null)
            {
                return null;
            }
            return new bool?(bool.Parse(str));
        }

        private static AssociationMapping ReadAssociationMapping(XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "Association"))
            {
                throw Error.UnexpectedElement("Association", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            ValidateAttributes(reader, new string[] { "Name", "IsForeignKey", "IsUnique", "Member", "OtherKey", "Storage", "ThisKey", "DeleteRule", "DeleteOnNull" });
            AssociationMapping mapping = new AssociationMapping {
                DbName = OptionalAttribute(reader, "Name"),
                IsForeignKey = OptionalBoolAttribute(reader, "IsForeignKey", false),
                IsUnique = OptionalBoolAttribute(reader, "IsUnique", false),
                MemberName = RequiredAttribute(reader, "Member"),
                OtherKey = OptionalAttribute(reader, "OtherKey"),
                StorageMemberName = OptionalAttribute(reader, "Storage"),
                ThisKey = OptionalAttribute(reader, "ThisKey"),
                DeleteRule = OptionalAttribute(reader, "DeleteRule"),
                DeleteOnNull = OptionalBoolAttribute(reader, "DeleteOnNull", false)
            };
            AssertEmptyElement(reader);
            return mapping;
        }

        private static ColumnMapping ReadColumnMapping(XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "Column"))
            {
                throw Error.UnexpectedElement("Column", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            ValidateAttributes(reader, new string[] { "Name", "DbType", "IsDbGenerated", "IsDiscriminator", "IsPrimaryKey", "IsVersion", "Member", "Storage", "Expression", "CanBeNull", "UpdateCheck", "AutoSync" });
            ColumnMapping mapping = new ColumnMapping {
                DbName = OptionalAttribute(reader, "Name"),
                DbType = OptionalAttribute(reader, "DbType"),
                IsDbGenerated = OptionalBoolAttribute(reader, "IsDbGenerated", false),
                IsDiscriminator = OptionalBoolAttribute(reader, "IsDiscriminator", false),
                IsPrimaryKey = OptionalBoolAttribute(reader, "IsPrimaryKey", false),
                IsVersion = OptionalBoolAttribute(reader, "IsVersion", false),
                MemberName = RequiredAttribute(reader, "Member"),
                StorageMemberName = OptionalAttribute(reader, "Storage"),
                Expression = OptionalAttribute(reader, "Expression"),
                CanBeNull = OptionalNullableBoolAttribute(reader, "CanBeNull")
            };
            string str = OptionalAttribute(reader, "UpdateCheck");
            mapping.UpdateCheck = (str == null) ? UpdateCheck.Always : ((UpdateCheck) Enum.Parse(typeof(UpdateCheck), str));
            string str2 = OptionalAttribute(reader, "AutoSync");
            mapping.AutoSync = (str2 == null) ? AutoSync.Default : ((AutoSync) Enum.Parse(typeof(AutoSync), str2));
            AssertEmptyElement(reader);
            return mapping;
        }

        internal static DatabaseMapping ReadDatabaseMapping(XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "Database"))
            {
                return null;
            }
            ValidateAttributes(reader, new string[] { "Name", "Provider" });
            DatabaseMapping mapping = new DatabaseMapping {
                DatabaseName = RequiredAttribute(reader, "Name"),
                Provider = OptionalAttribute(reader, "Provider")
            };
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return mapping;
            }
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                {
                    reader.Skip();
                    continue;
                }
                switch (reader.LocalName)
                {
                    case "Table":
                        mapping.Tables.Add(ReadTableMapping(reader));
                        break;

                    case "Function":
                        mapping.Functions.Add(ReadFunctionMapping(reader));
                        break;

                    default:
                        throw Error.UnrecognizedElement(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
                }
                reader.MoveToContent();
            }
            if (reader.LocalName != "Database")
            {
                throw Error.UnexpectedElement("Database", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            reader.ReadEndElement();
            return mapping;
        }

        private static TypeMapping ReadElementTypeMapping(TypeMapping baseType, XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "ElementType"))
            {
                throw Error.UnexpectedElement("Type", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            return ReadTypeMappingImpl(baseType, reader);
        }

        internal static FunctionMapping ReadFunctionMapping(XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "Function"))
            {
                throw Error.UnexpectedElement("Function", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            ValidateAttributes(reader, new string[] { "Name", "Method", "IsComposable" });
            FunctionMapping mapping = new FunctionMapping {
                MethodName = RequiredAttribute(reader, "Method"),
                Name = OptionalAttribute(reader, "Name"),
                IsComposable = OptionalBoolAttribute(reader, "IsComposable", false)
            };
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return mapping;
            }
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                {
                    reader.Skip();
                    continue;
                }
                switch (reader.LocalName)
                {
                    case "Parameter":
                        mapping.Parameters.Add(ReadParameterMapping(reader));
                        break;

                    case "ElementType":
                        mapping.Types.Add(ReadElementTypeMapping(null, reader));
                        break;

                    case "Return":
                        mapping.FunReturn = ReadReturnMapping(reader);
                        break;

                    default:
                        throw Error.UnrecognizedElement(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
                }
                reader.MoveToContent();
            }
            reader.ReadEndElement();
            return mapping;
        }

        private static ParameterMapping ReadParameterMapping(XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "Parameter"))
            {
                throw Error.UnexpectedElement("Parameter", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            ValidateAttributes(reader, new string[] { "Name", "DbType", "Parameter", "Direction" });
            ParameterMapping mapping = new ParameterMapping {
                Name = RequiredAttribute(reader, "Name"),
                ParameterName = RequiredAttribute(reader, "Parameter"),
                DbType = OptionalAttribute(reader, "DbType"),
                XmlDirection = OptionalAttribute(reader, "Direction")
            };
            AssertEmptyElement(reader);
            return mapping;
        }

        private static ReturnMapping ReadReturnMapping(XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "Return"))
            {
                throw Error.UnexpectedElement("Return", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            ValidateAttributes(reader, new string[] { "DbType" });
            ReturnMapping mapping = new ReturnMapping {
                DbType = OptionalAttribute(reader, "DbType")
            };
            AssertEmptyElement(reader);
            return mapping;
        }

        private static TableMapping ReadTableMapping(XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "Table"))
            {
                throw Error.UnexpectedElement("Table", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            ValidateAttributes(reader, new string[] { "Name", "Member" });
            TableMapping mapping = new TableMapping {
                TableName = OptionalAttribute(reader, "Name"),
                Member = OptionalAttribute(reader, "Member")
            };
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                reader.MoveToContent();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                    {
                        reader.Skip();
                    }
                    else
                    {
                        string str;
                        if ((((str = reader.LocalName) == null) || (str != "Type")) || (mapping.RowType != null))
                        {
                            throw Error.UnrecognizedElement(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
                        }
                        mapping.RowType = ReadTypeMapping(null, reader);
                        reader.MoveToContent();
                    }
                }
                if (reader.LocalName != "Table")
                {
                    throw Error.UnexpectedElement("Table", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
                }
                reader.ReadEndElement();
                return mapping;
            }
            reader.Skip();
            return mapping;
        }

        private static TypeMapping ReadTypeMapping(TypeMapping baseType, XmlReader reader)
        {
            if (!IsInNamespace(reader) || (reader.LocalName != "Type"))
            {
                throw Error.UnexpectedElement("Type", string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
            }
            return ReadTypeMappingImpl(baseType, reader);
        }

        private static TypeMapping ReadTypeMappingImpl(TypeMapping baseType, XmlReader reader)
        {
            ValidateAttributes(reader, new string[] { "Name", "InheritanceCode", "IsInheritanceDefault" });
            TypeMapping mapping = new TypeMapping {
                BaseType = baseType,
                Name = RequiredAttribute(reader, "Name"),
                InheritanceCode = OptionalAttribute(reader, "InheritanceCode"),
                IsInheritanceDefault = OptionalBoolAttribute(reader, "IsInheritanceDefault", false)
            };
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return mapping;
            }
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if ((reader.NodeType == XmlNodeType.Whitespace) || !IsInNamespace(reader))
                {
                    reader.Skip();
                    continue;
                }
                switch (reader.LocalName)
                {
                    case "Type":
                        mapping.DerivedTypes.Add(ReadTypeMapping(mapping, reader));
                        break;

                    case "Association":
                        mapping.Members.Add(ReadAssociationMapping(reader));
                        break;

                    case "Column":
                        mapping.Members.Add(ReadColumnMapping(reader));
                        break;

                    default:
                        throw Error.UnrecognizedElement(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : "/", reader.LocalName }));
                }
                reader.MoveToContent();
            }
            reader.ReadEndElement();
            return mapping;
        }

        private static string RequiredAttribute(XmlReader reader, string attribute)
        {
            string str = OptionalAttribute(reader, attribute);
            if (str == null)
            {
                throw Error.CouldNotFindRequiredAttribute(attribute, reader.ReadOuterXml());
            }
            return str;
        }

        internal static void ValidateAttributes(XmlReader reader, string[] validAttributes)
        {
            if (reader.HasAttributes)
            {
                List<string> list = new List<string>(validAttributes);
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    if ((IsInNamespace(reader) && (reader.LocalName != "xmlns")) && !list.Contains(reader.LocalName))
                    {
                        throw Error.UnrecognizedAttribute(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { reader.Prefix, string.IsNullOrEmpty(reader.Prefix) ? "" : ":", reader.LocalName }));
                    }
                }
                reader.MoveToElement();
            }
        }
    }
}


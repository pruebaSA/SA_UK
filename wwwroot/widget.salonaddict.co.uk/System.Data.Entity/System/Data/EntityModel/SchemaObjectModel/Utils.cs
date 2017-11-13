namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.CodeDom.Compiler;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Schema;

    internal static class Utils
    {
        private const string NameExp = @"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}][\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]{0,}";
        private const string OtherCharacterExp = @"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]";
        private const string StartCharacterExp = @"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}]";
        private static Regex UndottedNameValidator = new Regex(@"^[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}][\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]{0,}$", RegexOptions.Singleline | RegexOptions.Compiled);

        public static int CompareNames(string lhsName, string rhsName) => 
            string.Compare(lhsName, rhsName, StringComparison.Ordinal);

        internal static void ExtractNamespaceAndName(SchemaDataModelOption dataModel, string qualifiedTypeName, out string namespaceName, out string name)
        {
            GetBeforeAndAfterLastPeriod(qualifiedTypeName, out namespaceName, out name);
        }

        internal static string ExtractTypeName(SchemaDataModelOption dataModel, string qualifiedTypeName) => 
            GetEverythingAfterLastPeriod(qualifiedTypeName);

        private static void GetBeforeAndAfterLastPeriod(string qualifiedTypeName, out string before, out string after)
        {
            int length = qualifiedTypeName.LastIndexOf('.');
            if (length < 0)
            {
                before = null;
                after = qualifiedTypeName;
            }
            else
            {
                before = qualifiedTypeName.Substring(0, length);
                after = qualifiedTypeName.Substring(length + 1);
            }
        }

        public static bool GetBool(Schema schema, XmlReader reader, out bool value)
        {
            if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
            {
                value = true;
                return false;
            }
            try
            {
                value = reader.ReadContentAsBoolean();
                return true;
            }
            catch (XmlException)
            {
            }
            schema.AddError(ErrorCode.InvalidBoolean, EdmSchemaErrorSeverity.Error, reader, Strings.InvalidBoolean(reader.Value, reader.Name));
            value = true;
            return false;
        }

        public static bool GetByte(Schema schema, XmlReader reader, out byte value)
        {
            if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
            {
                value = 0;
                return false;
            }
            string s = reader.Value;
            value = 0;
            if (byte.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }
            schema.AddError(ErrorCode.ByteValueExpected, EdmSchemaErrorSeverity.Error, reader, Strings.ValueNotUnderstood(reader.Value, reader.Name));
            return false;
        }

        public static bool GetDottedName(Schema schema, XmlReader reader, out string name)
        {
            if (!GetString(schema, reader, out name))
            {
                return false;
            }
            return ValidateDottedName(schema, reader, name);
        }

        private static string GetEverythingAfterLastPeriod(string qualifiedTypeName)
        {
            int num = qualifiedTypeName.LastIndexOf('.');
            if (num < 0)
            {
                return qualifiedTypeName;
            }
            return qualifiedTypeName.Substring(num + 1);
        }

        internal static string GetEverythingBeforeLastPeriod(string qualifiedTypeName)
        {
            int length = qualifiedTypeName.LastIndexOf('.');
            if (length < 0)
            {
                return null;
            }
            return qualifiedTypeName.Substring(0, length);
        }

        public static bool GetInt(Schema schema, XmlReader reader, out int value)
        {
            if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
            {
                value = 0;
                return false;
            }
            string s = reader.Value;
            value = -2147483648;
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }
            schema.AddError(ErrorCode.IntegerExpected, EdmSchemaErrorSeverity.Error, reader, Strings.ValueNotUnderstood(reader.Value, reader.Name));
            return false;
        }

        public static bool GetString(Schema schema, XmlReader reader, out string value)
        {
            if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
            {
                value = null;
                return false;
            }
            value = reader.Value;
            if (string.IsNullOrEmpty(value))
            {
                schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, Strings.InvalidName(value, reader.Name));
                return false;
            }
            return true;
        }

        public static bool GetUndottedName(Schema schema, XmlReader reader, out string name)
        {
            if (reader.SchemaInfo.Validity == XmlSchemaValidity.Invalid)
            {
                name = null;
                return false;
            }
            name = reader.Value;
            if (string.IsNullOrEmpty(name))
            {
                schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, Strings.EmptyName(reader.Name));
                return false;
            }
            if ((schema.DataModel == SchemaDataModelOption.EntityDataModel) && (name.IndexOf('.') >= 0))
            {
                schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, Strings.InvalidQualifiedName(name, reader.Name));
                return false;
            }
            if ((schema.DataModel == SchemaDataModelOption.EntityDataModel) && !ValidUndottedName(name))
            {
                schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, Strings.InvalidName(name, reader.Name));
                return false;
            }
            return true;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static bool IsValidLanguageIndependentIdentifier(string name) => 
            CodeGenerator.IsValidLanguageIndependentIdentifier(name);

        internal static bool ValidateDottedName(Schema schema, XmlReader reader, string name)
        {
            if (schema.DataModel == SchemaDataModelOption.EntityDataModel)
            {
                foreach (string str in name.Split(new char[] { '.' }))
                {
                    if (!ValidUndottedName(str))
                    {
                        schema.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, reader, Strings.InvalidName(name, reader.Name));
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool ValidUndottedName(string name) => 
            ((!string.IsNullOrEmpty(name) && UndottedNameValidator.IsMatch(name)) && IsValidLanguageIndependentIdentifier(name));
    }
}


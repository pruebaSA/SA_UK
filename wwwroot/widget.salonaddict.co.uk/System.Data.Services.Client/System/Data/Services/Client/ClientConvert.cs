namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Parsing;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Linq;

    internal static class ClientConvert
    {
        private static readonly Type[] knownTypes = CreateKnownPrimitives();
        private static readonly Dictionary<string, Type> namedTypesMap = CreateKnownNamesMap();
        private static bool needSystemDataLinqBinary = true;
        private const string SystemDataLinq = "System.Data.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        internal static object ChangeType(string propertyValue, Type propertyType)
        {
            object obj2;
            try
            {
                switch (IndexOfStorage(propertyType))
                {
                    case 0:
                        return XmlConvert.ToBoolean(propertyValue);

                    case 1:
                        return XmlConvert.ToByte(propertyValue);

                    case 2:
                        return Convert.FromBase64String(propertyValue);

                    case 3:
                        return XmlConvert.ToChar(propertyValue);

                    case 4:
                        return propertyValue.ToCharArray();

                    case 5:
                        return XmlConvert.ToDateTime(propertyValue, XmlDateTimeSerializationMode.RoundtripKind);

                    case 6:
                        return XmlConvert.ToDateTimeOffset(propertyValue);

                    case 7:
                        return XmlConvert.ToDecimal(propertyValue);

                    case 8:
                        return XmlConvert.ToDouble(propertyValue);

                    case 9:
                        return new Guid(propertyValue);

                    case 10:
                        return XmlConvert.ToInt16(propertyValue);

                    case 11:
                        return XmlConvert.ToInt32(propertyValue);

                    case 12:
                        return XmlConvert.ToInt64(propertyValue);

                    case 13:
                        return XmlConvert.ToSingle(propertyValue);

                    case 14:
                        return propertyValue;

                    case 15:
                        return XmlConvert.ToSByte(propertyValue);

                    case 0x10:
                        return XmlConvert.ToTimeSpan(propertyValue);

                    case 0x11:
                        return Type.GetType(propertyValue, true);

                    case 0x12:
                        return XmlConvert.ToUInt16(propertyValue);

                    case 0x13:
                        return XmlConvert.ToUInt32(propertyValue);

                    case 20:
                        return XmlConvert.ToUInt64(propertyValue);

                    case 0x15:
                        return Util.CreateUri(propertyValue, UriKind.RelativeOrAbsolute);

                    case 0x16:
                        return ((0 < propertyValue.Length) ? XDocument.Parse(propertyValue) : new XDocument());

                    case 0x17:
                        return XElement.Parse(propertyValue);

                    case 0x18:
                        return Activator.CreateInstance(knownTypes[0x18], new object[] { Convert.FromBase64String(propertyValue) });
                }
                obj2 = propertyValue;
            }
            catch (FormatException exception)
            {
                propertyValue = (propertyValue.Length == 0) ? "String.Empty" : "String";
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_Current(propertyType.ToString(), propertyValue), exception);
            }
            catch (OverflowException exception2)
            {
                propertyValue = (propertyValue.Length == 0) ? "String.Empty" : "String";
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_Current(propertyType.ToString(), propertyValue), exception2);
            }
            return obj2;
        }

        private static Dictionary<string, Type> CreateKnownNamesMap() => 
            new Dictionary<string, Type>(EqualityComparer<string>.Default) { 
                { 
                    "Edm.String",
                    typeof(string)
                },
                { 
                    "Edm.Boolean",
                    typeof(bool)
                },
                { 
                    "Edm.Byte",
                    typeof(byte)
                },
                { 
                    "Edm.DateTime",
                    typeof(DateTime)
                },
                { 
                    "Edm.Decimal",
                    typeof(decimal)
                },
                { 
                    "Edm.Double",
                    typeof(double)
                },
                { 
                    "Edm.Guid",
                    typeof(Guid)
                },
                { 
                    "Edm.Int16",
                    typeof(short)
                },
                { 
                    "Edm.Int32",
                    typeof(int)
                },
                { 
                    "Edm.Int64",
                    typeof(long)
                },
                { 
                    "Edm.SByte",
                    typeof(sbyte)
                },
                { 
                    "Edm.Single",
                    typeof(float)
                },
                { 
                    "Edm.Binary",
                    typeof(byte[])
                }
            };

        private static Type[] CreateKnownPrimitives() => 
            new Type[] { 
                typeof(bool), typeof(byte), typeof(byte[]), typeof(char), typeof(char[]), typeof(DateTime), typeof(DateTimeOffset), typeof(decimal), typeof(double), typeof(Guid), typeof(short), typeof(int), typeof(long), typeof(float), typeof(string), typeof(sbyte),
                typeof(TimeSpan), typeof(Type), typeof(ushort), typeof(uint), typeof(ulong), typeof(Uri), typeof(XDocument), typeof(XElement), null
            };

        internal static string GetEdmType(Type propertyType)
        {
            switch (IndexOfStorage(propertyType))
            {
                case 0:
                    return "Edm.Boolean";

                case 1:
                    return "Edm.Byte";

                case 2:
                case 0x18:
                    return "Edm.Binary";

                case 3:
                case 4:
                case 14:
                case 0x11:
                case 0x15:
                case 0x16:
                case 0x17:
                    return null;

                case 5:
                    return "Edm.DateTime";

                case 6:
                case 0x10:
                case 0x12:
                case 0x13:
                case 20:
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantCastToUnsupportedPrimitive(propertyType.Name));

                case 7:
                    return "Edm.Decimal";

                case 8:
                    return "Edm.Double";

                case 9:
                    return "Edm.Guid";

                case 10:
                    return "Edm.Int16";

                case 11:
                    return "Edm.Int32";

                case 12:
                    return "Edm.Int64";

                case 13:
                    return "Edm.Single";

                case 15:
                    return "Edm.SByte";
            }
            return null;
        }

        private static int IndexOfStorage(Type type)
        {
            int num = Util.IndexOfReference<Type>(knownTypes, type);
            if (((num < 0) && needSystemDataLinqBinary) && (type.Name == "Binary"))
            {
                return LoadSystemDataLinqBinary(type);
            }
            return num;
        }

        internal static bool IsBinaryValue(object value) => 
            (0x18 == IndexOfStorage(value.GetType()));

        internal static bool IsKnownNullableType(Type type) => 
            IsKnownType(Nullable.GetUnderlyingType(type) ?? type);

        internal static bool IsKnownType(Type type) => 
            (0 <= IndexOfStorage(type));

        internal static bool IsSupportedPrimitiveTypeForUri(Type type) => 
            Util.ContainsReference<Type>(namedTypesMap.Values.ToArray<Type>(), type);

        private static int LoadSystemDataLinqBinary(Type type)
        {
            if ((type.Namespace == "System.Data.Linq") && AssemblyName.ReferenceMatchesDefinition(type.Assembly.GetName(), new AssemblyName("System.Data.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")))
            {
                knownTypes[0x18] = type;
                needSystemDataLinqBinary = false;
                return 0x18;
            }
            return -1;
        }

        internal static bool ToNamedType(string typeName, out Type type)
        {
            type = typeof(string);
            if (!string.IsNullOrEmpty(typeName))
            {
                return namedTypesMap.TryGetValue(typeName, out type);
            }
            return true;
        }

        internal static string ToString(object propertyValue, bool atomDateConstruct)
        {
            switch (IndexOfStorage(propertyValue.GetType()))
            {
                case 0:
                    return XmlConvert.ToString((bool) propertyValue);

                case 1:
                    return XmlConvert.ToString((byte) propertyValue);

                case 2:
                    return Convert.ToBase64String((byte[]) propertyValue);

                case 3:
                    return XmlConvert.ToString((char) propertyValue);

                case 4:
                    return new string((char[]) propertyValue);

                case 5:
                {
                    DateTime time = (DateTime) propertyValue;
                    return XmlConvert.ToString(((time.Kind == DateTimeKind.Unspecified) && atomDateConstruct) ? new DateTime(time.Ticks, DateTimeKind.Utc) : time, XmlDateTimeSerializationMode.RoundtripKind);
                }
                case 6:
                    return XmlConvert.ToString((DateTimeOffset) propertyValue);

                case 7:
                    return XmlConvert.ToString((decimal) propertyValue);

                case 8:
                    return XmlConvert.ToString((double) propertyValue);

                case 9:
                {
                    Guid guid = (Guid) propertyValue;
                    return guid.ToString();
                }
                case 10:
                    return XmlConvert.ToString((short) propertyValue);

                case 11:
                    return XmlConvert.ToString((int) propertyValue);

                case 12:
                    return XmlConvert.ToString((long) propertyValue);

                case 13:
                    return XmlConvert.ToString((float) propertyValue);

                case 14:
                    return (string) propertyValue;

                case 15:
                    return XmlConvert.ToString((sbyte) propertyValue);

                case 0x10:
                    return XmlConvert.ToString((TimeSpan) propertyValue);

                case 0x11:
                    return ((Type) propertyValue).AssemblyQualifiedName;

                case 0x12:
                    return XmlConvert.ToString((ushort) propertyValue);

                case 0x13:
                    return XmlConvert.ToString((uint) propertyValue);

                case 20:
                    return XmlConvert.ToString((ulong) propertyValue);

                case 0x15:
                    return Util.UriToString((Uri) propertyValue);

                case 0x16:
                    return ((XDocument) propertyValue).ToString();

                case 0x17:
                    return ((XElement) propertyValue).ToString();

                case 0x18:
                    return propertyValue.ToString();
            }
            return propertyValue.ToString();
        }

        internal static string ToTypeName(Type type)
        {
            foreach (KeyValuePair<string, Type> pair in namedTypesMap)
            {
                if (pair.Value == type)
                {
                    return pair.Key;
                }
            }
            return type.FullName;
        }

        internal static bool TryKeyBinaryToString(object binaryValue, out string result)
        {
            byte[] buffer = (byte[]) binaryValue.GetType().InvokeMember("ToArray", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, binaryValue, null, CultureInfo.InvariantCulture);
            return WebConvert.TryKeyPrimitiveToString(buffer, out result);
        }

        internal static bool TryKeyPrimitiveToString(object value, out string result)
        {
            if (IsBinaryValue(value))
            {
                return TryKeyBinaryToString(value, out result);
            }
            return WebConvert.TryKeyPrimitiveToString(value, out result);
        }

        internal enum StorageType
        {
            Boolean,
            Byte,
            ByteArray,
            Char,
            CharArray,
            DateTime,
            DateTimeOffset,
            Decimal,
            Double,
            Guid,
            Int16,
            Int32,
            Int64,
            Single,
            String,
            SByte,
            TimeSpan,
            Type,
            UInt16,
            UInt32,
            UInt64,
            Uri,
            XDocument,
            XElement,
            Binary
        }
    }
}


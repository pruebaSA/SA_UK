﻿namespace System.Runtime.Serialization
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Configuration;
    using System.Security;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

    internal abstract class DataContract
    {
        [SecurityCritical]
        private static DataContractSerializerSection configSection;
        [SecurityCritical]
        private DataContractCriticalHelper helper;
        [SecurityCritical]
        private XmlDictionaryString name;
        [SecurityCritical]
        private XmlDictionaryString ns;

        [SecurityCritical, SecurityTreatAsSafe]
        internal DataContract(DataContractCriticalHelper helper)
        {
            this.helper = helper;
            this.name = helper.Name;
            this.ns = helper.Namespace;
        }

        internal virtual DataContract BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts) => 
            this;

        internal static void CheckAndAdd(Type type, Dictionary<Type, Type> typesChecked, ref Dictionary<XmlQualifiedName, DataContract> nameToDataContractTable)
        {
            type = UnwrapNullableType(type);
            DataContract dataContract = GetDataContract(type);
            if (nameToDataContractTable == null)
            {
                nameToDataContractTable = new Dictionary<XmlQualifiedName, DataContract>();
            }
            else
            {
                DataContract contract2;
                if (nameToDataContractTable.TryGetValue(dataContract.StableName, out contract2))
                {
                    if (contract2.UnderlyingType != type)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("DupContractInKnownTypes", new object[] { type, contract2.UnderlyingType, dataContract.StableName.Namespace, dataContract.StableName.Name })));
                    }
                    return;
                }
            }
            nameToDataContractTable.Add(dataContract.StableName, dataContract);
            ImportKnownTypeAttributes(type, typesChecked, ref nameToDataContractTable);
        }

        private static void CheckExplicitDataContractNamespaceUri(string dataContractNs, Type type)
        {
            Uri uri;
            if (dataContractNs.Length > 0)
            {
                string str = dataContractNs.Trim();
                if ((str.Length == 0) || (str.IndexOf("##", StringComparison.Ordinal) != -1))
                {
                    ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("DataContractNamespaceIsNotValid", new object[] { dataContractNs }), type);
                }
                dataContractNs = str;
            }
            if (Uri.TryCreate(dataContractNs, UriKind.RelativeOrAbsolute, out uri))
            {
                if (uri.ToString() == "http://schemas.microsoft.com/2003/10/Serialization/")
                {
                    ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("DataContractNamespaceReserved", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/" }), type);
                }
            }
            else
            {
                ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("DataContractNamespaceIsNotValid", new object[] { dataContractNs }), type);
            }
        }

        private static void CheckRootTypeInConfigIsGeneric(Type type, ref Type rootType, ref Type[] genArgs)
        {
            if (rootType.IsGenericType)
            {
                if (!rootType.ContainsGenericParameters)
                {
                    genArgs = rootType.GetGenericArguments();
                    rootType = rootType.GetGenericTypeDefinition();
                }
                else
                {
                    ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("TypeMustBeConcrete", new object[] { type }), type);
                }
            }
        }

        private static byte[] ComputeHash(byte[] namespaces)
        {
            int[] numArray = new int[] { 7, 12, 0x11, 0x16, 5, 9, 14, 20, 4, 11, 0x10, 0x17, 6, 10, 15, 0x15 };
            uint[] numArray2 = new uint[] { 
                0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee, 0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501, 0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be, 0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
                0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa, 0xd62f105d, 0x2441453, 0xd8a1e681, 0xe7d3fbc8, 0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed, 0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
                0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c, 0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70, 0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x4881d05, 0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
                0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039, 0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1, 0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1, 0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
            };
            int num = ((namespaces.Length + 8) / 0x40) + 1;
            uint num2 = 0x67452301;
            uint num3 = 0xefcdab89;
            uint num4 = 0x98badcfe;
            uint num5 = 0x10325476;
            for (int i = 0; i < num; i++)
            {
                byte[] buffer = namespaces;
                int num7 = i * 0x40;
                if ((num7 + 0x40) > namespaces.Length)
                {
                    buffer = new byte[0x40];
                    for (int k = num7; k < namespaces.Length; k++)
                    {
                        buffer[k - num7] = namespaces[k];
                    }
                    if (num7 <= namespaces.Length)
                    {
                        buffer[namespaces.Length - num7] = 0x80;
                    }
                    if (i == (num - 1))
                    {
                        buffer[0x38] = (byte) (namespaces.Length << 3);
                        buffer[0x39] = (byte) (namespaces.Length >> 5);
                        buffer[0x3a] = (byte) (namespaces.Length >> 13);
                        buffer[0x3b] = (byte) (namespaces.Length >> 0x15);
                    }
                    num7 = 0;
                }
                uint num9 = num2;
                uint num10 = num3;
                uint num11 = num4;
                uint num12 = num5;
                for (int j = 0; j < 0x40; j++)
                {
                    uint num13;
                    int num14;
                    if (j < 0x10)
                    {
                        num13 = (num10 & num11) | (~num10 & num12);
                        num14 = j;
                    }
                    else if (j < 0x20)
                    {
                        num13 = (num10 & num12) | (num11 & ~num12);
                        num14 = (5 * j) + 1;
                    }
                    else if (j < 0x30)
                    {
                        num13 = (num10 ^ num11) ^ num12;
                        num14 = (3 * j) + 5;
                    }
                    else
                    {
                        num13 = num11 ^ (num10 | ~num12);
                        num14 = 7 * j;
                    }
                    num14 = ((num14 & 15) * 4) + num7;
                    uint num16 = num12;
                    num12 = num11;
                    num11 = num10;
                    num10 = ((num9 + num13) + numArray2[j]) + ((uint) (((buffer[num14] + (buffer[num14 + 1] << 8)) + (buffer[num14 + 2] << 0x10)) + (buffer[num14 + 3] << 0x18)));
                    num10 = (num10 << numArray[(j & 3) | ((j >> 2) & -4)]) | (num10 >> (0x20 - numArray[(j & 3) | ((j >> 2) & -4)]));
                    num10 += num11;
                    num9 = num16;
                }
                num2 += num9;
                num3 += num10;
                if (i < (num - 1))
                {
                    num4 += num11;
                    num5 += num12;
                }
            }
            return new byte[] { ((byte) num2), ((byte) (num2 >> 8)), ((byte) (num2 >> 0x10)), ((byte) (num2 >> 0x18)), ((byte) num3), ((byte) (num3 >> 8)) };
        }

        internal static bool ConstructorRequiresMemberAccess(ConstructorInfo ctor) => 
            (((ctor != null) && !ctor.IsPublic) && !IsMemberVisibleInSerializationModule(ctor));

        internal static XmlQualifiedName CreateQualifiedName(string localName, string ns) => 
            new XmlQualifiedName(localName, GetNamespace(ns));

        internal static string EncodeLocalName(string localName)
        {
            if (IsAsciiLocalName(localName))
            {
                return localName;
            }
            if (IsValidNCName(localName))
            {
                return localName;
            }
            return XmlConvert.EncodeLocalName(localName);
        }

        public sealed override bool Equals(object other) => 
            ((this == other) || this.Equals(other, new Dictionary<DataContractPairKey, object>()));

        internal virtual bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
        {
            DataContract contract = other as DataContract;
            if (contract == null)
            {
                return false;
            }
            return (((this.StableName.Name == contract.StableName.Name) && (this.StableName.Namespace == contract.StableName.Namespace)) && (this.IsReference == contract.IsReference));
        }

        internal static string ExpandGenericParameters(string format, IGenericNameProvider genericNameProvider)
        {
            string namespacesDigest = null;
            StringBuilder builder = new StringBuilder();
            IList<int> nestedParameterCounts = genericNameProvider.GetNestedParameterCounts();
            for (int i = 0; i < format.Length; i++)
            {
                char ch = format[i];
                if (ch == '{')
                {
                    i++;
                    int startIndex = i;
                    while (i < format.Length)
                    {
                        if (format[i] == '}')
                        {
                            break;
                        }
                        i++;
                    }
                    if (i == format.Length)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("GenericNameBraceMismatch", new object[] { format, genericNameProvider.GetGenericTypeName() })));
                    }
                    if ((format[startIndex] == '#') && (i == (startIndex + 1)))
                    {
                        if ((nestedParameterCounts.Count > 1) || !genericNameProvider.ParametersFromBuiltInNamespaces)
                        {
                            if (namespacesDigest == null)
                            {
                                StringBuilder builder2 = new StringBuilder(genericNameProvider.GetNamespaces());
                                foreach (int num3 in nestedParameterCounts)
                                {
                                    builder2.Insert(0, num3).Insert(0, " ");
                                }
                                namespacesDigest = GetNamespacesDigest(builder2.ToString());
                            }
                            builder.Append(namespacesDigest);
                        }
                    }
                    else
                    {
                        int num4;
                        if ((!int.TryParse(format.Substring(startIndex, i - startIndex), out num4) || (num4 < 0)) || (num4 >= genericNameProvider.GetParameterCount()))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("GenericParameterNotValid", new object[] { format.Substring(startIndex, i - startIndex), genericNameProvider.GetGenericTypeName(), genericNameProvider.GetParameterCount() - 1 })));
                        }
                        builder.Append(genericNameProvider.GetParameterName(num4));
                    }
                    continue;
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private static string ExpandGenericParameters(string format, Type type)
        {
            GenericNameProvider genericNameProvider = new GenericNameProvider(type);
            return ExpandGenericParameters(format, genericNameProvider);
        }

        internal static bool FieldRequiresMemberAccess(FieldInfo field) => 
            (((field != null) && !field.IsPublic) && !IsMemberVisibleInSerializationModule(field));

        private static string GetArrayPrefix(ref Type itemType)
        {
            string str = string.Empty;
            while (itemType.IsArray)
            {
                if (GetBuiltInDataContract(itemType) != null)
                {
                    return str;
                }
                str = str + "ArrayOf";
                itemType = itemType.GetElementType();
            }
            return str;
        }

        internal XmlQualifiedName GetArrayTypeName(bool isNullable)
        {
            XmlQualifiedName expandedStableName;
            if (this.IsValueType && isNullable)
            {
                System.Runtime.Serialization.GenericInfo info = new System.Runtime.Serialization.GenericInfo(GetStableName(Globals.TypeOfNullable), Globals.TypeOfNullable.FullName);
                info.Add(new System.Runtime.Serialization.GenericInfo(this.StableName, null));
                info.AddToLevel(0, 1);
                expandedStableName = info.GetExpandedStableName();
            }
            else
            {
                expandedStableName = this.StableName;
            }
            return new XmlQualifiedName("ArrayOf" + expandedStableName.Name, GetCollectionNamespace(expandedStableName.Namespace));
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public static DataContract GetBuiltInDataContract(string typeName) => 
            DataContractCriticalHelper.GetBuiltInDataContract(typeName);

        [SecurityTreatAsSafe, SecurityCritical]
        public static DataContract GetBuiltInDataContract(Type type) => 
            DataContractCriticalHelper.GetBuiltInDataContract(type);

        [SecurityCritical, SecurityTreatAsSafe]
        public static DataContract GetBuiltInDataContract(string name, string ns) => 
            DataContractCriticalHelper.GetBuiltInDataContract(name, ns);

        internal static void GetClrNameAndNamespace(string fullTypeName, out string localName, out string ns)
        {
            int length = fullTypeName.LastIndexOf('.');
            if (length < 0)
            {
                ns = string.Empty;
                localName = fullTypeName.Replace('+', '.');
            }
            else
            {
                ns = fullTypeName.Substring(0, length);
                localName = fullTypeName.Substring(length + 1).Replace('+', '.');
            }
            int index = localName.IndexOf('[');
            if (index >= 0)
            {
                localName = localName.Substring(0, index);
            }
        }

        internal static string GetClrTypeFullName(Type type)
        {
            if (type.IsGenericTypeDefinition || !type.ContainsGenericParameters)
            {
                return type.FullName;
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { type.Namespace, type.Name });
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static XmlDictionaryString GetClrTypeString(string key) => 
            DataContractCriticalHelper.GetClrTypeString(key);

        internal static string GetCollectionNamespace(string elementNs)
        {
            if (!IsBuiltInNamespace(elementNs))
            {
                return elementNs;
            }
            return "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        }

        internal static XmlQualifiedName GetCollectionStableName(Type type, Type itemType, out CollectionDataContractAttribute collectionContractAttribute)
        {
            string defaultStableLocalName;
            string defaultDataContractNamespace;
            object[] customAttributes = type.GetCustomAttributes(Globals.TypeOfCollectionDataContractAttribute, false);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                collectionContractAttribute = (CollectionDataContractAttribute) customAttributes[0];
                if (collectionContractAttribute.IsNameSetExplicit)
                {
                    defaultStableLocalName = collectionContractAttribute.Name;
                    if ((defaultStableLocalName == null) || (defaultStableLocalName.Length == 0))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("InvalidCollectionContractName", new object[] { GetClrTypeFullName(type) })));
                    }
                    if (type.IsGenericType && !type.IsGenericTypeDefinition)
                    {
                        defaultStableLocalName = ExpandGenericParameters(defaultStableLocalName, type);
                    }
                    defaultStableLocalName = EncodeLocalName(defaultStableLocalName);
                }
                else
                {
                    defaultStableLocalName = GetDefaultStableLocalName(type);
                }
                if (collectionContractAttribute.IsNamespaceSetExplicit)
                {
                    defaultDataContractNamespace = collectionContractAttribute.Namespace;
                    if (defaultDataContractNamespace == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("InvalidCollectionContractNamespace", new object[] { GetClrTypeFullName(type) })));
                    }
                    CheckExplicitDataContractNamespaceUri(defaultDataContractNamespace, type);
                }
                else
                {
                    defaultDataContractNamespace = GetDefaultDataContractNamespace(type);
                }
            }
            else
            {
                collectionContractAttribute = null;
                string str3 = "ArrayOf" + GetArrayPrefix(ref itemType);
                XmlQualifiedName stableName = GetStableName(itemType);
                defaultStableLocalName = str3 + stableName.Name;
                defaultDataContractNamespace = GetCollectionNamespace(stableName.Namespace);
            }
            return CreateQualifiedName(defaultStableLocalName, defaultDataContractNamespace);
        }

        internal static DataContract GetDataContract(Type type) => 
            GetDataContract(type.TypeHandle, type);

        internal static DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type) => 
            GetDataContract(typeHandle, type, SerializationMode.SharedContract);

        internal static DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle, SerializationMode mode) => 
            GetDataContractSkipValidation(id, typeHandle, null).GetValidContract(mode);

        internal static DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type, SerializationMode mode) => 
            GetDataContractSkipValidation(GetId(typeHandle), typeHandle, null).GetValidContract(mode);

        [SecurityCritical, SecurityTreatAsSafe]
        internal static DataContract GetDataContractForInitialization(int id) => 
            DataContractCriticalHelper.GetDataContractForInitialization(id);

        internal static IList<int> GetDataContractNameForGenericName(string typeName, StringBuilder localName)
        {
            int num2;
            List<int> list = new List<int>();
            int startIndex = 0;
        Label_0008:
            num2 = typeName.IndexOf('`', startIndex);
            if (num2 < 0)
            {
                if (localName != null)
                {
                    localName.Append(typeName.Substring(startIndex));
                }
                list.Add(0);
            }
            else
            {
                if (localName != null)
                {
                    localName.Append(typeName.Substring(startIndex, num2 - startIndex));
                }
                while ((startIndex = typeName.IndexOf('.', startIndex + 1, (num2 - startIndex) - 1)) >= 0)
                {
                    list.Add(0);
                }
                startIndex = typeName.IndexOf('.', num2);
                if (startIndex < 0)
                {
                    list.Add(int.Parse(typeName.Substring(num2 + 1), CultureInfo.InvariantCulture));
                }
                else
                {
                    list.Add(int.Parse(typeName.Substring(num2 + 1, (startIndex - num2) - 1), CultureInfo.InvariantCulture));
                    goto Label_0008;
                }
            }
            if (localName != null)
            {
                localName.Append("Of");
            }
            return list;
        }

        internal static string GetDataContractNamespaceFromUri(string uriString)
        {
            if (!uriString.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal))
            {
                return uriString;
            }
            return uriString.Substring("http://schemas.datacontract.org/2004/07/".Length);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static DataContract GetDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type) => 
            DataContractCriticalHelper.GetDataContractSkipValidation(id, typeHandle, type);

        private static XmlQualifiedName GetDCTypeStableName(Type type, DataContractAttribute dataContractAttribute)
        {
            string format = null;
            string dataContractNs = null;
            if (dataContractAttribute.IsNameSetExplicit)
            {
                format = dataContractAttribute.Name;
                if ((format == null) || (format.Length == 0))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("InvalidDataContractName", new object[] { GetClrTypeFullName(type) })));
                }
                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    format = ExpandGenericParameters(format, type);
                }
                format = EncodeLocalName(format);
            }
            else
            {
                format = GetDefaultStableLocalName(type);
            }
            if (dataContractAttribute.IsNamespaceSetExplicit)
            {
                dataContractNs = dataContractAttribute.Namespace;
                if (dataContractNs == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("InvalidDataContractNamespace", new object[] { GetClrTypeFullName(type) })));
                }
                CheckExplicitDataContractNamespaceUri(dataContractNs, type);
            }
            else
            {
                dataContractNs = GetDefaultDataContractNamespace(type);
            }
            return CreateQualifiedName(format, dataContractNs);
        }

        private static string GetDefaultDataContractNamespace(Type type)
        {
            string clrNs = type.Namespace;
            if (clrNs == null)
            {
                clrNs = string.Empty;
            }
            string globalDataContractNamespace = GetGlobalDataContractNamespace(clrNs, type.Module);
            if (globalDataContractNamespace == null)
            {
                globalDataContractNamespace = GetGlobalDataContractNamespace(clrNs, type.Assembly);
            }
            if (globalDataContractNamespace == null)
            {
                return GetDefaultStableNamespace(type);
            }
            CheckExplicitDataContractNamespaceUri(globalDataContractNamespace, type);
            return globalDataContractNamespace;
        }

        private static string GetDefaultStableLocalName(Type type)
        {
            string str;
            if (type.IsGenericParameter)
            {
                return ("{" + type.GenericParameterPosition + "}");
            }
            string arrayPrefix = null;
            if (type.IsArray)
            {
                arrayPrefix = GetArrayPrefix(ref type);
            }
            if (type.DeclaringType == null)
            {
                str = type.Name;
            }
            else
            {
                int startIndex = (type.Namespace == null) ? 0 : type.Namespace.Length;
                if (startIndex > 0)
                {
                    startIndex++;
                }
                str = GetClrTypeFullName(type).Substring(startIndex).Replace('+', '.');
            }
            if (arrayPrefix != null)
            {
                str = arrayPrefix + str;
            }
            if (type.IsGenericType)
            {
                StringBuilder localName = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                bool flag = true;
                int index = str.IndexOf('[');
                if (index >= 0)
                {
                    str = str.Substring(0, index);
                }
                IList<int> dataContractNameForGenericName = GetDataContractNameForGenericName(str, localName);
                bool isGenericTypeDefinition = type.IsGenericTypeDefinition;
                Type[] genericArguments = type.GetGenericArguments();
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    Type type2 = genericArguments[i];
                    if (isGenericTypeDefinition)
                    {
                        localName.Append("{").Append(i).Append("}");
                    }
                    else
                    {
                        XmlQualifiedName stableName = GetStableName(type2);
                        localName.Append(stableName.Name);
                        builder2.Append(" ").Append(stableName.Namespace);
                        if (flag)
                        {
                            flag = IsBuiltInNamespace(stableName.Namespace);
                        }
                    }
                }
                if (isGenericTypeDefinition)
                {
                    localName.Append("{#}");
                }
                else if ((dataContractNameForGenericName.Count > 1) || !flag)
                {
                    foreach (int num4 in dataContractNameForGenericName)
                    {
                        builder2.Insert(0, num4).Insert(0, " ");
                    }
                    localName.Append(GetNamespacesDigest(builder2.ToString()));
                }
                str = localName.ToString();
            }
            return EncodeLocalName(str);
        }

        internal static XmlQualifiedName GetDefaultStableName(Type type) => 
            CreateQualifiedName(GetDefaultStableLocalName(type), GetDefaultStableNamespace(type));

        private static void GetDefaultStableName(CodeTypeReference typeReference, out string localName, out string ns)
        {
            string baseType = typeReference.BaseType;
            DataContract builtInDataContract = GetBuiltInDataContract(baseType);
            if (builtInDataContract != null)
            {
                localName = builtInDataContract.StableName.Name;
                ns = builtInDataContract.StableName.Namespace;
            }
            else
            {
                GetClrNameAndNamespace(baseType, out localName, out ns);
                if (typeReference.TypeArguments.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    StringBuilder builder2 = new StringBuilder();
                    bool flag = true;
                    IList<int> dataContractNameForGenericName = GetDataContractNameForGenericName(localName, builder);
                    foreach (CodeTypeReference reference in typeReference.TypeArguments)
                    {
                        string str2;
                        string str3;
                        GetDefaultStableName(reference, out str2, out str3);
                        builder.Append(str2);
                        builder2.Append(" ").Append(str3);
                        if (flag)
                        {
                            flag = IsBuiltInNamespace(str3);
                        }
                    }
                    if ((dataContractNameForGenericName.Count > 1) || !flag)
                    {
                        foreach (int num in dataContractNameForGenericName)
                        {
                            builder2.Insert(0, num).Insert(0, " ");
                        }
                        builder.Append(GetNamespacesDigest(builder2.ToString()));
                    }
                    localName = builder.ToString();
                }
                localName = EncodeLocalName(localName);
                ns = GetDefaultStableNamespace(ns);
            }
        }

        internal static void GetDefaultStableName(string fullTypeName, out string localName, out string ns)
        {
            CodeTypeReference typeReference = new CodeTypeReference(fullTypeName);
            GetDefaultStableName(typeReference, out localName, out ns);
        }

        internal static string GetDefaultStableNamespace(string clrNs)
        {
            if (clrNs == null)
            {
                clrNs = string.Empty;
            }
            return new Uri(Globals.DataContractXsdBaseNamespaceUri, clrNs).AbsoluteUri;
        }

        internal static string GetDefaultStableNamespace(Type type)
        {
            if (type.IsGenericParameter)
            {
                return "{ns}";
            }
            return GetDefaultStableNamespace(type.Namespace);
        }

        internal static DataContract GetGetOnlyCollectionDataContract(int id, RuntimeTypeHandle typeHandle, Type type, SerializationMode mode)
        {
            DataContract validContract = GetGetOnlyCollectionDataContractSkipValidation(id, typeHandle, type).GetValidContract(mode);
            if (validContract is ClassDataContract)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.Runtime.Serialization.SR.GetString("ErrorDeserializing", new object[] { System.Runtime.Serialization.SR.GetString("ErrorTypeInfo", new object[] { GetClrTypeFullName(validContract.UnderlyingType) }), System.Runtime.Serialization.SR.GetString("NoSetMethodForProperty", new object[] { string.Empty, string.Empty }) })));
            }
            return validContract;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static DataContract GetGetOnlyCollectionDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type) => 
            DataContractCriticalHelper.GetGetOnlyCollectionDataContractSkipValidation(id, typeHandle, type);

        private static string GetGlobalDataContractNamespace(string clrNs, ICustomAttributeProvider customAttribuetProvider)
        {
            object[] customAttributes = customAttribuetProvider.GetCustomAttributes(typeof(ContractNamespaceAttribute), false);
            string contractNamespace = null;
            for (int i = 0; i < customAttributes.Length; i++)
            {
                ContractNamespaceAttribute attribute = (ContractNamespaceAttribute) customAttributes[i];
                string clrNamespace = attribute.ClrNamespace;
                if (clrNamespace == null)
                {
                    clrNamespace = string.Empty;
                }
                if (clrNamespace == clrNs)
                {
                    if (attribute.ContractNamespace == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("InvalidGlobalDataContractNamespace", new object[] { clrNs })));
                    }
                    if (contractNamespace != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("DataContractNamespaceAlreadySet", new object[] { contractNamespace, attribute.ContractNamespace, clrNs })));
                    }
                    contractNamespace = attribute.ContractNamespace;
                }
            }
            return contractNamespace;
        }

        public override int GetHashCode() => 
            base.GetHashCode();

        [SecurityTreatAsSafe, SecurityCritical]
        internal static int GetId(RuntimeTypeHandle typeHandle) => 
            DataContractCriticalHelper.GetId(typeHandle);

        [SecurityCritical, SecurityTreatAsSafe]
        internal static int GetIdForInitialization(ClassDataContract classContract) => 
            DataContractCriticalHelper.GetIdForInitialization(classContract);

        [SecurityTreatAsSafe, SecurityCritical]
        internal static string GetNamespace(string key) => 
            DataContractCriticalHelper.GetNamespace(key);

        private static string GetNamespacesDigest(string namespaces)
        {
            byte[] inArray = ComputeHash(Encoding.UTF8.GetBytes(namespaces));
            char[] outArray = new char[0x18];
            int num = Convert.ToBase64CharArray(inArray, 0, 6, outArray, 0);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                char ch = outArray[i];
                switch (ch)
                {
                    case '+':
                        builder.Append("_P");
                        break;

                    case '/':
                        builder.Append("_S");
                        break;

                    case '=':
                        break;

                    default:
                        builder.Append(ch);
                        break;
                }
            }
            return builder.ToString();
        }

        private static XmlQualifiedName GetNonDCTypeStableName(Type type)
        {
            string localName = null;
            string ns = null;
            Type type2;
            if (CollectionDataContract.IsCollection(type, out type2))
            {
                CollectionDataContractAttribute attribute;
                return GetCollectionStableName(type, type2, out attribute);
            }
            localName = GetDefaultStableLocalName(type);
            if (ClassDataContract.IsNonAttributedTypeValidForSerialization(type))
            {
                ns = GetDefaultDataContractNamespace(type);
            }
            else
            {
                ns = GetDefaultStableNamespace(type);
            }
            return CreateQualifiedName(localName, ns);
        }

        internal static XmlQualifiedName GetStableName(Type type)
        {
            bool flag;
            return GetStableName(type, out flag);
        }

        internal static XmlQualifiedName GetStableName(Type type, out bool hasDataContract)
        {
            XmlQualifiedName nonDCTypeStableName;
            DataContractAttribute attribute;
            type = UnwrapRedundantNullableType(type);
            if (TryGetBuiltInXmlAndArrayTypeStableName(type, out nonDCTypeStableName))
            {
                hasDataContract = false;
                return nonDCTypeStableName;
            }
            if (TryGetDCAttribute(type, out attribute))
            {
                nonDCTypeStableName = GetDCTypeStableName(type, attribute);
                hasDataContract = true;
                return nonDCTypeStableName;
            }
            nonDCTypeStableName = GetNonDCTypeStableName(type);
            hasDataContract = false;
            return nonDCTypeStableName;
        }

        internal virtual DataContract GetValidContract() => 
            this;

        internal virtual DataContract GetValidContract(SerializationMode mode) => 
            this;

        internal static Dictionary<XmlQualifiedName, DataContract> ImportKnownTypeAttributes(Type type)
        {
            Dictionary<XmlQualifiedName, DataContract> knownDataContracts = null;
            Dictionary<Type, Type> typesChecked = new Dictionary<Type, Type>();
            ImportKnownTypeAttributes(type, typesChecked, ref knownDataContracts);
            return knownDataContracts;
        }

        private static void ImportKnownTypeAttributes(Type type, Dictionary<Type, Type> typesChecked, ref Dictionary<XmlQualifiedName, DataContract> knownDataContracts)
        {
            while ((type != null) && IsTypeSerializable(type))
            {
                if (typesChecked.ContainsKey(type))
                {
                    return;
                }
                typesChecked.Add(type, type);
                object[] customAttributes = type.GetCustomAttributes(Globals.TypeOfKnownTypeAttribute, false);
                if (customAttributes != null)
                {
                    bool flag = false;
                    bool flag2 = false;
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        KnownTypeAttribute attribute = (KnownTypeAttribute) customAttributes[i];
                        if (attribute.Type != null)
                        {
                            if (flag)
                            {
                                ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("KnownTypeAttributeOneScheme", new object[] { GetClrTypeFullName(type) }), type);
                            }
                            CheckAndAdd(attribute.Type, typesChecked, ref knownDataContracts);
                            flag2 = true;
                        }
                        else
                        {
                            if (flag || flag2)
                            {
                                ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("KnownTypeAttributeOneScheme", new object[] { GetClrTypeFullName(type) }), type);
                            }
                            string methodName = attribute.MethodName;
                            if (methodName == null)
                            {
                                ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("KnownTypeAttributeNoData", new object[] { GetClrTypeFullName(type) }), type);
                            }
                            if (methodName.Length == 0)
                            {
                                ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("KnownTypeAttributeEmptyString", new object[] { GetClrTypeFullName(type) }), type);
                            }
                            MethodInfo info = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, Globals.EmptyTypeArray, null);
                            if (info == null)
                            {
                                ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("KnownTypeAttributeUnknownMethod", new object[] { methodName, GetClrTypeFullName(type) }), type);
                            }
                            if (!Globals.TypeOfTypeEnumerable.IsAssignableFrom(info.ReturnType))
                            {
                                ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("KnownTypeAttributeReturnType", new object[] { GetClrTypeFullName(type), methodName }), type);
                            }
                            object obj2 = info.Invoke(null, Globals.EmptyObjectArray);
                            if (obj2 == null)
                            {
                                ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("KnownTypeAttributeMethodNull", new object[] { GetClrTypeFullName(type) }), type);
                            }
                            foreach (Type type2 in (IEnumerable<Type>) obj2)
                            {
                                if (type2 == null)
                                {
                                    ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("KnownTypeAttributeValidMethodTypes", new object[] { GetClrTypeFullName(type) }), type);
                                }
                                CheckAndAdd(type2, typesChecked, ref knownDataContracts);
                            }
                            flag = true;
                        }
                    }
                }
                LoadKnownTypesFromConfig(type, typesChecked, ref knownDataContracts);
                type = type.BaseType;
            }
        }

        private static bool IsAlpha(char ch) => 
            (((ch >= 'A') && (ch <= 'Z')) || ((ch >= 'a') && (ch <= 'z')));

        private static bool IsAsciiLocalName(string localName)
        {
            if (localName.Length == 0)
            {
                return false;
            }
            if (!IsAlpha(localName[0]))
            {
                return false;
            }
            for (int i = 1; i < localName.Length; i++)
            {
                char ch = localName[i];
                if (!IsAlpha(ch) && !IsDigit(ch))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsBuiltInNamespace(string ns)
        {
            if (ns != "http://www.w3.org/2001/XMLSchema")
            {
                return (ns == "http://schemas.microsoft.com/2003/10/Serialization/");
            }
            return true;
        }

        private static bool IsCollectionElementTypeEqualToRootType(string collectionElementTypeName, Type rootType)
        {
            if (collectionElementTypeName.StartsWith(GetClrTypeFullName(rootType), StringComparison.Ordinal))
            {
                Type t = Type.GetType(collectionElementTypeName, false);
                if (t != null)
                {
                    if (t.IsGenericType && !IsOpenGenericType(t))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("KnownTypeConfigClosedGenericDeclared", new object[] { collectionElementTypeName })));
                    }
                    if (rootType.Equals(t))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsDigit(char ch) => 
            ((ch >= '0') && (ch <= '9'));

        private static bool IsElemTypeNullOrNotEqualToRootType(string elemTypeName, Type rootType)
        {
            Type o = Type.GetType(elemTypeName, false);
            if ((o != null) && rootType.Equals(o))
            {
                return false;
            }
            return true;
        }

        internal bool IsEqualOrChecked(object other, Dictionary<DataContractPairKey, object> checkedContracts)
        {
            if (this == other)
            {
                return true;
            }
            if (checkedContracts != null)
            {
                DataContractPairKey key = new DataContractPairKey(this, other);
                if (checkedContracts.ContainsKey(key))
                {
                    return true;
                }
                checkedContracts.Add(key, null);
            }
            return false;
        }

        private static bool IsMemberVisibleInSerializationModule(MemberInfo member)
        {
            if (!IsTypeVisibleInSerializationModule(member.DeclaringType))
            {
                return false;
            }
            if (member is MethodInfo)
            {
                return ((MethodInfo) member).IsAssembly;
            }
            if (member is FieldInfo)
            {
                return ((FieldInfo) member).IsAssembly;
            }
            return ((member is ConstructorInfo) && ((ConstructorInfo) member).IsAssembly);
        }

        private static bool IsOpenGenericType(Type t)
        {
            Type[] genericArguments = t.GetGenericArguments();
            for (int i = 0; i < genericArguments.Length; i++)
            {
                if (!genericArguments[i].IsGenericParameter)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsTypeNullable(Type type) => 
            (!type.IsValueType || (type.IsGenericType && (type.GetGenericTypeDefinition() == Globals.TypeOfNullable)));

        internal static bool IsTypeSerializable(Type type)
        {
            Type type2;
            if ((((!type.IsSerializable && !type.IsDefined(Globals.TypeOfDataContractAttribute, false)) && (!type.IsInterface && !type.IsPointer)) && (!Globals.TypeOfIXmlSerializable.IsAssignableFrom(type) && (!CollectionDataContract.IsCollection(type, out type2) || !IsTypeSerializable(type2)))) && (GetBuiltInDataContract(type) == null))
            {
                return ClassDataContract.IsNonAttributedTypeValidForSerialization(type);
            }
            return true;
        }

        internal static bool IsTypeVisible(Type t)
        {
            if (!t.IsVisible && !IsTypeVisibleInSerializationModule(t))
            {
                return false;
            }
            foreach (Type type in t.GetGenericArguments())
            {
                if (!type.IsGenericParameter && !IsTypeVisible(type))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsTypeVisibleInSerializationModule(Type type) => 
            type.Module.Equals(typeof(CodeGenerator).Module);

        internal virtual bool IsValidContract(SerializationMode mode) => 
            true;

        internal static bool IsValidNCName(string name)
        {
            try
            {
                XmlConvert.VerifyNCName(name);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private static void LoadKnownTypesFromConfig(Type type, Dictionary<Type, Type> typesChecked, ref Dictionary<XmlQualifiedName, DataContract> knownDataContracts)
        {
            if (ConfigSection != null)
            {
                DeclaredTypeElementCollection declaredTypes = ConfigSection.DeclaredTypes;
                Type rootType = type;
                Type[] genArgs = null;
                CheckRootTypeInConfigIsGeneric(type, ref rootType, ref genArgs);
                DeclaredTypeElement element = declaredTypes[rootType.AssemblyQualifiedName];
                if ((element != null) && IsElemTypeNullOrNotEqualToRootType(element.Type, rootType))
                {
                    element = null;
                }
                if (element == null)
                {
                    for (int i = 0; i < declaredTypes.Count; i++)
                    {
                        if (IsCollectionElementTypeEqualToRootType(declaredTypes[i].Type, rootType))
                        {
                            element = declaredTypes[i];
                            break;
                        }
                    }
                }
                if (element != null)
                {
                    for (int j = 0; j < element.KnownTypes.Count; j++)
                    {
                        Type type3 = element.KnownTypes[j].GetType(element.Type, genArgs);
                        if (type3 != null)
                        {
                            CheckAndAdd(type3, typesChecked, ref knownDataContracts);
                        }
                    }
                }
            }
        }

        internal static bool MethodRequiresMemberAccess(MethodInfo method) => 
            (((method != null) && !method.IsPublic) && !IsMemberVisibleInSerializationModule(method));

        public virtual object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("UnexpectedContractType", new object[] { GetClrTypeFullName(base.GetType()), GetClrTypeFullName(this.UnderlyingType) })));
        }

        internal void ThrowInvalidDataContractException(string message)
        {
            ThrowInvalidDataContractException(message, this.UnderlyingType);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static void ThrowInvalidDataContractException(string message, Type type)
        {
            DataContractCriticalHelper.ThrowInvalidDataContractException(message, type);
        }

        public static void ThrowTypeNotSerializable(Type type)
        {
            ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("TypeNotSerializable", new object[] { type }), type);
        }

        private static bool TryGetBuiltInXmlAndArrayTypeStableName(Type type, out XmlQualifiedName stableName)
        {
            stableName = null;
            DataContract builtInDataContract = GetBuiltInDataContract(type);
            if (builtInDataContract != null)
            {
                stableName = builtInDataContract.StableName;
            }
            else if (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
            {
                bool flag;
                XmlSchemaType type2;
                XmlQualifiedName name;
                SchemaExporter.GetXmlTypeInfo(type, out name, out type2, out flag);
                stableName = name;
            }
            else if (type.IsArray)
            {
                CollectionDataContractAttribute attribute;
                stableName = GetCollectionStableName(type, type.GetElementType(), out attribute);
            }
            return (stableName != null);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static bool TryGetDCAttribute(Type type, out DataContractAttribute dataContractAttribute)
        {
            dataContractAttribute = null;
            object[] customAttributes = type.GetCustomAttributes(Globals.TypeOfDataContractAttribute, false);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                dataContractAttribute = (DataContractAttribute) customAttributes[0];
            }
            return (dataContractAttribute != null);
        }

        internal static Type UnwrapNullableType(Type type)
        {
            while (type.IsGenericType && (type.GetGenericTypeDefinition() == Globals.TypeOfNullable))
            {
                type = type.GetGenericArguments()[0];
            }
            return type;
        }

        internal static Type UnwrapRedundantNullableType(Type type)
        {
            Type type2 = type;
            while (type.IsGenericType && (type.GetGenericTypeDefinition() == Globals.TypeOfNullable))
            {
                type2 = type;
                type = type.GetGenericArguments()[0];
            }
            return type2;
        }

        internal virtual void WriteRootElement(XmlWriterDelegator writer, XmlDictionaryString name, XmlDictionaryString ns)
        {
            if (object.ReferenceEquals(ns, DictionaryGlobals.SerializationNamespace) && !this.IsPrimitive)
            {
                writer.WriteStartElement("z", name, ns);
            }
            else
            {
                writer.WriteStartElement(name, ns);
            }
        }

        public virtual void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("UnexpectedContractType", new object[] { GetClrTypeFullName(base.GetType()), GetClrTypeFullName(this.UnderlyingType) })));
        }

        internal virtual bool CanContainReferences =>
            true;

        private static DataContractSerializerSection ConfigSection
        {
            [SecurityCritical]
            get
            {
                if (configSection == null)
                {
                    configSection = DataContractSerializerSection.UnsafeGetSection();
                }
                return configSection;
            }
        }

        internal System.Runtime.Serialization.GenericInfo GenericInfo
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.GenericInfo;
            [SecurityCritical]
            set
            {
                this.helper.GenericInfo = value;
            }
        }

        internal virtual bool HasRoot
        {
            get => 
                true;
            set
            {
            }
        }

        protected DataContractCriticalHelper Helper =>
            this.helper;

        internal virtual bool IsBuiltInDataContract =>
            this.helper.IsBuiltInDataContract;

        internal virtual bool IsISerializable
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.IsISerializable;
            [SecurityCritical]
            set
            {
                this.helper.IsISerializable = value;
            }
        }

        internal virtual bool IsPrimitive =>
            false;

        internal bool IsReference
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.IsReference;
            [SecurityCritical]
            set
            {
                this.helper.IsReference = value;
            }
        }

        internal bool IsValueType
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.IsValueType;
            [SecurityCritical]
            set
            {
                this.helper.IsValueType = value;
            }
        }

        internal virtual Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.KnownDataContracts;
            [SecurityCritical]
            set
            {
                this.helper.KnownDataContracts = value;
            }
        }

        internal XmlDictionaryString Name =>
            this.name;

        public virtual XmlDictionaryString Namespace =>
            this.ns;

        internal XmlQualifiedName StableName
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.StableName;
            [SecurityCritical]
            set
            {
                this.helper.StableName = value;
            }
        }

        internal virtual XmlDictionaryString TopLevelElementName
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.TopLevelElementName;
            [SecurityCritical]
            set
            {
                this.helper.TopLevelElementName = value;
            }
        }

        internal virtual XmlDictionaryString TopLevelElementNamespace
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.TopLevelElementNamespace;
            [SecurityCritical]
            set
            {
                this.helper.TopLevelElementNamespace = value;
            }
        }

        internal Type TypeForInitialization =>
            this.helper.TypeForInitialization;

        internal Type UnderlyingType =>
            this.helper.UnderlyingType;

        [SecurityCritical(SecurityCriticalScope.Everything)]
        internal class DataContractCriticalHelper
        {
            private static object cacheLock = new object();
            private static Dictionary<string, XmlDictionaryString> clrTypeStrings;
            private static XmlDictionary clrTypeStringsDictionary;
            private static object clrTypeStringsLock = new object();
            private static object createDataContractLock = new object();
            private static DataContract[] dataContractCache = new DataContract[0x20];
            private static int dataContractID = 0;
            private System.Runtime.Serialization.GenericInfo genericInfo;
            private static object initBuiltInContractsLock = new object();
            private bool isReference;
            private bool isValueType;
            private XmlDictionaryString name;
            private static Dictionary<string, string> namespaces;
            private static object namespacesLock = new object();
            private static Dictionary<XmlQualifiedName, DataContract> nameToBuiltInContract;
            private XmlDictionaryString ns;
            private XmlQualifiedName stableName;
            private Type typeForInitialization;
            private static TypeHandleRef typeHandleRef = new TypeHandleRef();
            private static Dictionary<string, DataContract> typeNameToBuiltInContract;
            private static Dictionary<Type, DataContract> typeToBuiltInContract;
            private static Dictionary<TypeHandleRef, IntRef> typeToIDCache = new Dictionary<TypeHandleRef, IntRef>(new TypeHandleRefEqualityComparer());
            private readonly Type underlyingType;

            internal DataContractCriticalHelper()
            {
            }

            internal DataContractCriticalHelper(Type type)
            {
                this.underlyingType = type;
                this.SetTypeForInitialization(type);
                this.isValueType = type.IsValueType;
            }

            private static bool ContractMatches(DataContract contract, DataContract cachedContract) => 
                ((cachedContract != null) && (cachedContract.UnderlyingType == contract.UnderlyingType));

            private static DataContract CreateDataContract(int id, RuntimeTypeHandle typeHandle, Type type)
            {
                lock (createDataContractLock)
                {
                    DataContract dataContract = dataContractCache[id];
                    if (dataContract == null)
                    {
                        if (type == null)
                        {
                            type = Type.GetTypeFromHandle(typeHandle);
                        }
                        type = DataContract.UnwrapNullableType(type);
                        type = GetDataContractAdapterType(type);
                        dataContract = GetBuiltInDataContract(type);
                        if (dataContract == null)
                        {
                            if (type.IsArray)
                            {
                                dataContract = new CollectionDataContract(type);
                            }
                            else if (type.IsEnum)
                            {
                                dataContract = new EnumDataContract(type);
                            }
                            else if (type.IsGenericParameter)
                            {
                                dataContract = new GenericParameterDataContract(type);
                            }
                            else if (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
                            {
                                dataContract = new XmlDataContract(type);
                            }
                            else
                            {
                                if (type.IsPointer)
                                {
                                    type = Globals.TypeOfReflectionPointer;
                                }
                                if (!CollectionDataContract.TryCreate(type, out dataContract))
                                {
                                    if ((!type.IsSerializable && !type.IsDefined(Globals.TypeOfDataContractAttribute, false)) && !ClassDataContract.IsNonAttributedTypeValidForSerialization(type))
                                    {
                                        ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("TypeNotSerializable", new object[] { type }), type);
                                    }
                                    dataContract = new ClassDataContract(type);
                                }
                            }
                        }
                    }
                    return dataContract;
                }
            }

            private static DataContract CreateGetOnlyCollectionDataContract(int id, RuntimeTypeHandle typeHandle, Type type)
            {
                DataContract dataContract = null;
                lock (createDataContractLock)
                {
                    dataContract = dataContractCache[id];
                    if (dataContract != null)
                    {
                        return dataContract;
                    }
                    if (type == null)
                    {
                        type = Type.GetTypeFromHandle(typeHandle);
                    }
                    type = DataContract.UnwrapNullableType(type);
                    type = GetDataContractAdapterType(type);
                    CollectionDataContract.CreateGetOnlyCollectionDataContract(type, out dataContract);
                }
                return dataContract;
            }

            public static DataContract GetBuiltInDataContract(string typeName)
            {
                if (!typeName.StartsWith("System.", StringComparison.Ordinal))
                {
                    return null;
                }
                lock (initBuiltInContractsLock)
                {
                    if (typeNameToBuiltInContract == null)
                    {
                        typeNameToBuiltInContract = new Dictionary<string, DataContract>();
                    }
                    DataContract contract = null;
                    if (!typeNameToBuiltInContract.TryGetValue(typeName, out contract))
                    {
                        Type type = null;
                        switch (typeName.Substring(7))
                        {
                            case "Char":
                                type = typeof(char);
                                break;

                            case "Boolean":
                                type = typeof(bool);
                                break;

                            case "SByte":
                                type = typeof(sbyte);
                                break;

                            case "Byte":
                                type = typeof(byte);
                                break;

                            case "Int16":
                                type = typeof(short);
                                break;

                            case "UInt16":
                                type = typeof(ushort);
                                break;

                            case "Int32":
                                type = typeof(int);
                                break;

                            case "UInt32":
                                type = typeof(uint);
                                break;

                            case "Int64":
                                type = typeof(long);
                                break;

                            case "UInt64":
                                type = typeof(ulong);
                                break;

                            case "Single":
                                type = typeof(float);
                                break;

                            case "Double":
                                type = typeof(double);
                                break;

                            case "Decimal":
                                type = typeof(decimal);
                                break;

                            case "DateTime":
                                type = typeof(DateTime);
                                break;

                            case "String":
                                type = typeof(string);
                                break;

                            case "Byte[]":
                                type = typeof(byte[]);
                                break;

                            case "Object":
                                type = typeof(object);
                                break;

                            case "TimeSpan":
                                type = typeof(TimeSpan);
                                break;

                            case "Guid":
                                type = typeof(Guid);
                                break;

                            case "Uri":
                                type = typeof(Uri);
                                break;

                            case "Xml.XmlQualifiedName":
                                type = typeof(XmlQualifiedName);
                                break;

                            case "Enum":
                                type = typeof(Enum);
                                break;

                            case "ValueType":
                                type = typeof(ValueType);
                                break;

                            case "Array":
                                type = typeof(Array);
                                break;

                            case "Xml.XmlElement":
                                type = typeof(XmlElement);
                                break;

                            case "Xml.XmlNode[]":
                                type = typeof(System.Xml.XmlNode[]);
                                break;
                        }
                        if (type != null)
                        {
                            TryCreateBuiltInDataContract(type, out contract);
                        }
                        typeNameToBuiltInContract.Add(typeName, contract);
                    }
                    return contract;
                }
            }

            public static DataContract GetBuiltInDataContract(Type type)
            {
                if (type.IsInterface && !CollectionDataContract.IsCollectionInterface(type))
                {
                    type = Globals.TypeOfObject;
                }
                lock (initBuiltInContractsLock)
                {
                    if (typeToBuiltInContract == null)
                    {
                        typeToBuiltInContract = new Dictionary<Type, DataContract>();
                    }
                    DataContract contract = null;
                    if (!typeToBuiltInContract.TryGetValue(type, out contract))
                    {
                        TryCreateBuiltInDataContract(type, out contract);
                        typeToBuiltInContract.Add(type, contract);
                    }
                    return contract;
                }
            }

            public static DataContract GetBuiltInDataContract(string name, string ns)
            {
                lock (initBuiltInContractsLock)
                {
                    if (nameToBuiltInContract == null)
                    {
                        nameToBuiltInContract = new Dictionary<XmlQualifiedName, DataContract>();
                    }
                    DataContract contract = null;
                    XmlQualifiedName key = new XmlQualifiedName(name, ns);
                    if (!nameToBuiltInContract.TryGetValue(key, out contract) && TryCreateBuiltInDataContract(name, ns, out contract))
                    {
                        nameToBuiltInContract.Add(key, contract);
                    }
                    return contract;
                }
            }

            internal static XmlDictionaryString GetClrTypeString(string key)
            {
                lock (clrTypeStringsLock)
                {
                    XmlDictionaryString str;
                    if (clrTypeStrings == null)
                    {
                        clrTypeStringsDictionary = new XmlDictionary();
                        clrTypeStrings = new Dictionary<string, XmlDictionaryString>();
                        try
                        {
                            clrTypeStrings.Add(Globals.TypeOfInt.Assembly.FullName, clrTypeStringsDictionary.Add("0"));
                        }
                        catch (Exception exception)
                        {
                            if (DiagnosticUtility.IsFatal(exception))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception.Message, exception);
                        }
                    }
                    if (!clrTypeStrings.TryGetValue(key, out str))
                    {
                        str = clrTypeStringsDictionary.Add(key);
                        try
                        {
                            clrTypeStrings.Add(key, str);
                        }
                        catch (Exception exception2)
                        {
                            if (DiagnosticUtility.IsFatal(exception2))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception2.Message, exception2);
                        }
                    }
                    return str;
                }
            }

            private static Type GetDataContractAdapterType(Type type)
            {
                if (type == Globals.TypeOfDateTimeOffset)
                {
                    return Globals.TypeOfDateTimeOffsetAdapter;
                }
                return type;
            }

            private static RuntimeTypeHandle GetDataContractAdapterTypeHandle(RuntimeTypeHandle typeHandle)
            {
                if (Globals.TypeOfDateTimeOffset.TypeHandle.Equals(typeHandle))
                {
                    return Globals.TypeOfDateTimeOffsetAdapter.TypeHandle;
                }
                return typeHandle;
            }

            internal static DataContract GetDataContractForInitialization(int id)
            {
                DataContract contract = dataContractCache[id];
                if (contract == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.Runtime.Serialization.SR.GetString("DataContractCacheOverflow")));
                }
                return contract;
            }

            internal static DataContract GetDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
            {
                DataContract contract = dataContractCache[id];
                return contract?.GetValidContract();
            }

            internal static DataContract GetGetOnlyCollectionDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
            {
                DataContract contract = dataContractCache[id];
                if (contract == null)
                {
                    contract = CreateGetOnlyCollectionDataContract(id, typeHandle, type);
                    dataContractCache[id] = contract;
                }
                return contract;
            }

            internal static int GetId(RuntimeTypeHandle typeHandle)
            {
                lock (cacheLock)
                {
                    IntRef ref2;
                    typeHandle = GetDataContractAdapterTypeHandle(typeHandle);
                    typeHandleRef.Value = typeHandle;
                    if (!typeToIDCache.TryGetValue(typeHandleRef, out ref2))
                    {
                        int num = dataContractID++;
                        if (num >= dataContractCache.Length)
                        {
                            int newSize = (num < 0x3fffffff) ? (num * 2) : 0x7fffffff;
                            if (newSize <= num)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.Runtime.Serialization.SR.GetString("DataContractCacheOverflow")));
                            }
                            Array.Resize<DataContract>(ref dataContractCache, newSize);
                        }
                        ref2 = new IntRef(num);
                        try
                        {
                            typeToIDCache.Add(new TypeHandleRef(typeHandle), ref2);
                        }
                        catch (Exception exception)
                        {
                            if (DiagnosticUtility.IsFatal(exception))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception.Message, exception);
                        }
                    }
                    return ref2.Value;
                }
            }

            internal static int GetIdForInitialization(ClassDataContract classContract)
            {
                int id = DataContract.GetId(classContract.TypeForInitialization.TypeHandle);
                if ((id < dataContractCache.Length) && ContractMatches(classContract, dataContractCache[id]))
                {
                    return id;
                }
                for (int i = 0; i < dataContractID; i++)
                {
                    if (ContractMatches(classContract, dataContractCache[i]))
                    {
                        return i;
                    }
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.Runtime.Serialization.SR.GetString("DataContractCacheOverflow")));
            }

            internal static string GetNamespace(string key)
            {
                lock (namespacesLock)
                {
                    string str;
                    if (namespaces == null)
                    {
                        namespaces = new Dictionary<string, string>();
                    }
                    if (namespaces.TryGetValue(key, out str))
                    {
                        return str;
                    }
                    try
                    {
                        namespaces.Add(key, key);
                    }
                    catch (Exception exception)
                    {
                        if (DiagnosticUtility.IsFatal(exception))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception.Message, exception);
                    }
                    return key;
                }
            }

            internal void SetDataContractName(XmlQualifiedName stableName)
            {
                XmlDictionary dictionary = new XmlDictionary(2);
                this.Name = dictionary.Add(stableName.Name);
                this.Namespace = dictionary.Add(stableName.Namespace);
                this.StableName = stableName;
            }

            internal void SetDataContractName(XmlDictionaryString name, XmlDictionaryString ns)
            {
                this.Name = name;
                this.Namespace = ns;
                this.StableName = DataContract.CreateQualifiedName(name.Value, ns.Value);
            }

            [SecurityCritical, SecurityTreatAsSafe]
            private void SetTypeForInitialization(Type classType)
            {
                if (classType.IsSerializable || classType.IsDefined(Globals.TypeOfDataContractAttribute, false))
                {
                    this.typeForInitialization = classType;
                }
            }

            internal void ThrowInvalidDataContractException(string message)
            {
                ThrowInvalidDataContractException(message, this.UnderlyingType);
            }

            internal static void ThrowInvalidDataContractException(string message, Type type)
            {
                if (type != null)
                {
                    lock (cacheLock)
                    {
                        typeHandleRef.Value = GetDataContractAdapterTypeHandle(type.TypeHandle);
                        try
                        {
                            typeToIDCache.Remove(typeHandleRef);
                        }
                        catch (Exception exception)
                        {
                            if (DiagnosticUtility.IsFatal(exception))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception.Message, exception);
                        }
                    }
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(message));
            }

            public static bool TryCreateBuiltInDataContract(Type type, out DataContract dataContract)
            {
                if (type.IsEnum)
                {
                    dataContract = null;
                    return false;
                }
                dataContract = null;
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        dataContract = new BooleanDataContract();
                        break;

                    case TypeCode.Char:
                        dataContract = new CharDataContract();
                        break;

                    case TypeCode.SByte:
                        dataContract = new SignedByteDataContract();
                        break;

                    case TypeCode.Byte:
                        dataContract = new UnsignedByteDataContract();
                        break;

                    case TypeCode.Int16:
                        dataContract = new ShortDataContract();
                        break;

                    case TypeCode.UInt16:
                        dataContract = new UnsignedShortDataContract();
                        break;

                    case TypeCode.Int32:
                        dataContract = new IntDataContract();
                        break;

                    case TypeCode.UInt32:
                        dataContract = new UnsignedIntDataContract();
                        break;

                    case TypeCode.Int64:
                        dataContract = new LongDataContract();
                        break;

                    case TypeCode.UInt64:
                        dataContract = new UnsignedLongDataContract();
                        break;

                    case TypeCode.Single:
                        dataContract = new FloatDataContract();
                        break;

                    case TypeCode.Double:
                        dataContract = new DoubleDataContract();
                        break;

                    case TypeCode.Decimal:
                        dataContract = new DecimalDataContract();
                        break;

                    case TypeCode.DateTime:
                        dataContract = new DateTimeDataContract();
                        break;

                    case TypeCode.String:
                        dataContract = new StringDataContract();
                        break;

                    default:
                        if (type == typeof(byte[]))
                        {
                            dataContract = new ByteArrayDataContract();
                        }
                        else if (type == typeof(object))
                        {
                            dataContract = new ObjectDataContract();
                        }
                        else if (type == typeof(Uri))
                        {
                            dataContract = new UriDataContract();
                        }
                        else if (type == typeof(XmlQualifiedName))
                        {
                            dataContract = new QNameDataContract();
                        }
                        else if (type == typeof(TimeSpan))
                        {
                            dataContract = new TimeSpanDataContract();
                        }
                        else if (type == typeof(Guid))
                        {
                            dataContract = new GuidDataContract();
                        }
                        else if ((type == typeof(Enum)) || (type == typeof(ValueType)))
                        {
                            dataContract = new SpecialTypeDataContract(type, DictionaryGlobals.ObjectLocalName, DictionaryGlobals.SchemaNamespace);
                        }
                        else if (type == typeof(Array))
                        {
                            dataContract = new CollectionDataContract(type);
                        }
                        else if ((type == typeof(XmlElement)) || (type == typeof(System.Xml.XmlNode[])))
                        {
                            dataContract = new XmlDataContract(type);
                        }
                        break;
                }
                return (dataContract != null);
            }

            public static bool TryCreateBuiltInDataContract(string name, string ns, out DataContract dataContract)
            {
                dataContract = null;
                if (ns == DictionaryGlobals.SchemaNamespace.Value)
                {
                    if (DictionaryGlobals.BooleanLocalName.Value == name)
                    {
                        dataContract = new BooleanDataContract();
                    }
                    else if (DictionaryGlobals.SignedByteLocalName.Value == name)
                    {
                        dataContract = new SignedByteDataContract();
                    }
                    else if (DictionaryGlobals.UnsignedByteLocalName.Value == name)
                    {
                        dataContract = new UnsignedByteDataContract();
                    }
                    else if (DictionaryGlobals.ShortLocalName.Value == name)
                    {
                        dataContract = new ShortDataContract();
                    }
                    else if (DictionaryGlobals.UnsignedShortLocalName.Value == name)
                    {
                        dataContract = new UnsignedShortDataContract();
                    }
                    else if (DictionaryGlobals.IntLocalName.Value == name)
                    {
                        dataContract = new IntDataContract();
                    }
                    else if (DictionaryGlobals.UnsignedIntLocalName.Value == name)
                    {
                        dataContract = new UnsignedIntDataContract();
                    }
                    else if (DictionaryGlobals.LongLocalName.Value == name)
                    {
                        dataContract = new LongDataContract();
                    }
                    else if (DictionaryGlobals.integerLocalName.Value == name)
                    {
                        dataContract = new IntegerDataContract();
                    }
                    else if (DictionaryGlobals.positiveIntegerLocalName.Value == name)
                    {
                        dataContract = new PositiveIntegerDataContract();
                    }
                    else if (DictionaryGlobals.negativeIntegerLocalName.Value == name)
                    {
                        dataContract = new NegativeIntegerDataContract();
                    }
                    else if (DictionaryGlobals.nonPositiveIntegerLocalName.Value == name)
                    {
                        dataContract = new NonPositiveIntegerDataContract();
                    }
                    else if (DictionaryGlobals.nonNegativeIntegerLocalName.Value == name)
                    {
                        dataContract = new NonNegativeIntegerDataContract();
                    }
                    else if (DictionaryGlobals.UnsignedLongLocalName.Value == name)
                    {
                        dataContract = new UnsignedLongDataContract();
                    }
                    else if (DictionaryGlobals.FloatLocalName.Value == name)
                    {
                        dataContract = new FloatDataContract();
                    }
                    else if (DictionaryGlobals.DoubleLocalName.Value == name)
                    {
                        dataContract = new DoubleDataContract();
                    }
                    else if (DictionaryGlobals.DecimalLocalName.Value == name)
                    {
                        dataContract = new DecimalDataContract();
                    }
                    else if (DictionaryGlobals.DateTimeLocalName.Value == name)
                    {
                        dataContract = new DateTimeDataContract();
                    }
                    else if (DictionaryGlobals.StringLocalName.Value == name)
                    {
                        dataContract = new StringDataContract();
                    }
                    else if (DictionaryGlobals.timeLocalName.Value == name)
                    {
                        dataContract = new TimeDataContract();
                    }
                    else if (DictionaryGlobals.dateLocalName.Value == name)
                    {
                        dataContract = new DateDataContract();
                    }
                    else if (DictionaryGlobals.hexBinaryLocalName.Value == name)
                    {
                        dataContract = new HexBinaryDataContract();
                    }
                    else if (DictionaryGlobals.gYearMonthLocalName.Value == name)
                    {
                        dataContract = new GYearMonthDataContract();
                    }
                    else if (DictionaryGlobals.gYearLocalName.Value == name)
                    {
                        dataContract = new GYearDataContract();
                    }
                    else if (DictionaryGlobals.gMonthDayLocalName.Value == name)
                    {
                        dataContract = new GMonthDayDataContract();
                    }
                    else if (DictionaryGlobals.gDayLocalName.Value == name)
                    {
                        dataContract = new GDayDataContract();
                    }
                    else if (DictionaryGlobals.gMonthLocalName.Value == name)
                    {
                        dataContract = new GMonthDataContract();
                    }
                    else if (DictionaryGlobals.normalizedStringLocalName.Value == name)
                    {
                        dataContract = new NormalizedStringDataContract();
                    }
                    else if (DictionaryGlobals.tokenLocalName.Value == name)
                    {
                        dataContract = new TokenDataContract();
                    }
                    else if (DictionaryGlobals.languageLocalName.Value == name)
                    {
                        dataContract = new LanguageDataContract();
                    }
                    else if (DictionaryGlobals.NameLocalName.Value == name)
                    {
                        dataContract = new NameDataContract();
                    }
                    else if (DictionaryGlobals.NCNameLocalName.Value == name)
                    {
                        dataContract = new NCNameDataContract();
                    }
                    else if (DictionaryGlobals.XSDIDLocalName.Value == name)
                    {
                        dataContract = new IDDataContract();
                    }
                    else if (DictionaryGlobals.IDREFLocalName.Value == name)
                    {
                        dataContract = new IDREFDataContract();
                    }
                    else if (DictionaryGlobals.IDREFSLocalName.Value == name)
                    {
                        dataContract = new IDREFSDataContract();
                    }
                    else if (DictionaryGlobals.ENTITYLocalName.Value == name)
                    {
                        dataContract = new ENTITYDataContract();
                    }
                    else if (DictionaryGlobals.ENTITIESLocalName.Value == name)
                    {
                        dataContract = new ENTITIESDataContract();
                    }
                    else if (DictionaryGlobals.NMTOKENLocalName.Value == name)
                    {
                        dataContract = new NMTOKENDataContract();
                    }
                    else if (DictionaryGlobals.NMTOKENSLocalName.Value == name)
                    {
                        dataContract = new NMTOKENDataContract();
                    }
                    else if (DictionaryGlobals.ByteArrayLocalName.Value == name)
                    {
                        dataContract = new ByteArrayDataContract();
                    }
                    else if (DictionaryGlobals.ObjectLocalName.Value == name)
                    {
                        dataContract = new ObjectDataContract();
                    }
                    else if (DictionaryGlobals.TimeSpanLocalName.Value == name)
                    {
                        dataContract = new XsDurationDataContract();
                    }
                    else if (DictionaryGlobals.UriLocalName.Value == name)
                    {
                        dataContract = new UriDataContract();
                    }
                    else if (DictionaryGlobals.QNameLocalName.Value == name)
                    {
                        dataContract = new QNameDataContract();
                    }
                }
                else if (ns == DictionaryGlobals.SerializationNamespace.Value)
                {
                    if (DictionaryGlobals.TimeSpanLocalName.Value == name)
                    {
                        dataContract = new TimeSpanDataContract();
                    }
                    else if (DictionaryGlobals.GuidLocalName.Value == name)
                    {
                        dataContract = new GuidDataContract();
                    }
                    else if (DictionaryGlobals.CharLocalName.Value == name)
                    {
                        dataContract = new CharDataContract();
                    }
                    else if ("ArrayOfanyType" == name)
                    {
                        dataContract = new CollectionDataContract(typeof(Array));
                    }
                }
                else if (ns == DictionaryGlobals.AsmxTypesNamespace.Value)
                {
                    if (DictionaryGlobals.CharLocalName.Value == name)
                    {
                        dataContract = new AsmxCharDataContract();
                    }
                    else if (DictionaryGlobals.GuidLocalName.Value == name)
                    {
                        dataContract = new AsmxGuidDataContract();
                    }
                }
                else if (ns == "http://schemas.datacontract.org/2004/07/System.Xml")
                {
                    if (name == "XmlElement")
                    {
                        dataContract = new XmlDataContract(typeof(XmlElement));
                    }
                    else if (name == "ArrayOfXmlNode")
                    {
                        dataContract = new XmlDataContract(typeof(System.Xml.XmlNode[]));
                    }
                }
                return (dataContract != null);
            }

            internal virtual void WriteRootElement(XmlWriterDelegator writer, XmlDictionaryString name, XmlDictionaryString ns)
            {
                if (object.ReferenceEquals(ns, DictionaryGlobals.SerializationNamespace) && !this.IsPrimitive)
                {
                    writer.WriteStartElement("z", name, ns);
                }
                else
                {
                    writer.WriteStartElement(name, ns);
                }
            }

            internal virtual bool CanContainReferences =>
                true;

            internal System.Runtime.Serialization.GenericInfo GenericInfo
            {
                get => 
                    this.genericInfo;
                set
                {
                    this.genericInfo = value;
                }
            }

            internal virtual bool HasRoot
            {
                get => 
                    true;
                set
                {
                }
            }

            internal virtual bool IsBuiltInDataContract =>
                false;

            internal virtual bool IsISerializable
            {
                get => 
                    false;
                set
                {
                    this.ThrowInvalidDataContractException(System.Runtime.Serialization.SR.GetString("RequiresClassDataContractToSetIsISerializable"));
                }
            }

            internal virtual bool IsPrimitive =>
                false;

            internal bool IsReference
            {
                get => 
                    this.isReference;
                set
                {
                    this.isReference = value;
                }
            }

            internal bool IsValueType
            {
                get => 
                    this.isValueType;
                set
                {
                    this.isValueType = value;
                }
            }

            internal virtual Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
            {
                get => 
                    null;
                set
                {
                }
            }

            internal XmlDictionaryString Name
            {
                get => 
                    this.name;
                set
                {
                    this.name = value;
                }
            }

            public XmlDictionaryString Namespace
            {
                get => 
                    this.ns;
                set
                {
                    this.ns = value;
                }
            }

            internal XmlQualifiedName StableName
            {
                get => 
                    this.stableName;
                set
                {
                    this.stableName = value;
                }
            }

            internal virtual XmlDictionaryString TopLevelElementName
            {
                get => 
                    this.name;
                set
                {
                    this.name = value;
                }
            }

            internal virtual XmlDictionaryString TopLevelElementNamespace
            {
                get => 
                    this.ns;
                set
                {
                    this.ns = value;
                }
            }

            internal Type TypeForInitialization =>
                this.typeForInitialization;

            internal Type UnderlyingType =>
                this.underlyingType;
        }
    }
}


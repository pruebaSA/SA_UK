namespace System.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    internal static class Globals
    {
        public const string ActualTypeLocalName = "ActualType";
        public const string ActualTypeNameAttribute = "Name";
        public const string ActualTypeNamespaceAttribute = "Namespace";
        public const string AddMethodName = "Add";
        public const string AddValueMethodName = "AddValue";
        public const string AnyTypeLocalName = "anyType";
        public const string ArrayPrefix = "ArrayOf";
        public const string ArraySizeLocalName = "Size";
        public const string ClrAssemblyLocalName = "Assembly";
        public const string ClrNamespaceProperty = "ClrNamespace";
        public const string ClrTypeLocalName = "Type";
        public const string CollectionsNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
        public const string ContextFieldName = "context";
        public const string CurrentPropertyName = "Current";
        public const string DataContractXmlNamespace = "http://schemas.datacontract.org/2004/07/System.Xml";
        public const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";
        [SecurityCritical]
        private static Uri dataContractXsdBaseNamespaceUri;
        public const string DefaultClrNamespace = "GeneratedNamespace";
        public const bool DefaultEmitDefaultValue = true;
        public const string DefaultFieldSuffix = "Field";
        public const string DefaultGeneratedMember = "GeneratedMember";
        public const bool DefaultIsReference = false;
        public const bool DefaultIsRequired = false;
        public const string DefaultMemberSuffix = "Member";
        public const int DefaultOrder = 0;
        public const string DefaultPropertySuffix = "Property";
        public const string DefaultTypeName = "GeneratedType";
        public const string DefaultValueLocalName = "DefaultValue";
        public const string ElementPrefix = "q";
        public const string EmitDefaultValueAttribute = "EmitDefaultValue";
        public const string EmitDefaultValueProperty = "EmitDefaultValue";
        [SecurityCritical]
        private static object[] emptyObjectArray;
        [SecurityCritical]
        private static Type[] emptyTypeArray;
        public const string EnumerationValueLocalName = "EnumerationValue";
        public const string EnumeratorFieldName = "enumerator";
        public const string ExportSchemaMethod = "ExportSchema";
        public const string ExtensionDataObjectFieldName = "extensionDataField";
        public const string ExtensionDataObjectPropertyName = "ExtensionData";
        public const string ExtensionDataSetExplicitMethod = "System.Runtime.Serialization.IExtensibleDataObject.set_ExtensionData";
        public const string ExtensionDataSetMethod = "set_ExtensionData";
        public const string False = "false";
        public const string GenericNameAttribute = "Name";
        public const string GenericNamespaceAttribute = "Namespace";
        public const string GenericParameterLocalName = "GenericParameter";
        public const string GenericParameterNestedLevelAttribute = "NestedLevel";
        public const string GenericTypeLocalName = "GenericType";
        public const string GetCurrentMethodName = "get_Current";
        public const string GetEnumeratorMethodName = "GetEnumerator";
        public const string GetObjectDataMethodName = "GetObjectData";
        public const string IdLocalName = "Id";
        [SecurityCritical]
        private static XmlQualifiedName idQualifiedName;
        public const string IntLocalName = "int";
        public const string IsAnyProperty = "IsAny";
        public const string IsDictionaryLocalName = "IsDictionary";
        public const string ISerializableFactoryTypeLocalName = "FactoryType";
        public const string IsReferenceProperty = "IsReference";
        public const string IsRequiredProperty = "IsRequired";
        public const string IsValueTypeLocalName = "IsValueType";
        public const string ItemNameProperty = "ItemName";
        public const string KeyLocalName = "Key";
        public const string KeyNameProperty = "KeyName";
        [SecurityCritical]
        private static ReflectionPermission memberAccessPermission;
        public const string MoveNextMethodName = "MoveNext";
        public const string MscorlibAssemblyName = "0";
        public const string NameProperty = "Name";
        public const string NamespaceProperty = "Namespace";
        public static readonly string NewObjectId = string.Empty;
        public const string NodeArrayFieldName = "nodesField";
        public const string NodeArrayPropertyName = "Nodes";
        public const string NullObjectId = null;
        public const string OccursUnbounded = "unbounded";
        public const string OrderProperty = "Order";
        public const string RefLocalName = "Ref";
        [SecurityCritical]
        private static XmlQualifiedName refQualifiedName;
        internal const BindingFlags ScanAllMembers = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        public const string SchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";
        public const string SchemaLocalName = "schema";
        public const string SchemaNamespace = "http://www.w3.org/2001/XMLSchema";
        public const string SerializationEntryFieldName = "entry";
        [SecurityCritical]
        private static SecurityPermission serializationFormatterPermission;
        public const string SerializationInfoFieldName = "info";
        public const string SerializationInfoPropertyName = "SerializationInfo";
        public const string SerializationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";
        public const string SerializationSchema = "<?xml version='1.0' encoding='utf-8'?>\r\n<xs:schema elementFormDefault='qualified' attributeFormDefault='qualified' xmlns:tns='http://schemas.microsoft.com/2003/10/Serialization/' targetNamespace='http://schemas.microsoft.com/2003/10/Serialization/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>\r\n  <xs:element name='anyType' nillable='true' type='xs:anyType' />\r\n  <xs:element name='anyURI' nillable='true' type='xs:anyURI' />\r\n  <xs:element name='base64Binary' nillable='true' type='xs:base64Binary' />\r\n  <xs:element name='boolean' nillable='true' type='xs:boolean' />\r\n  <xs:element name='byte' nillable='true' type='xs:byte' />\r\n  <xs:element name='dateTime' nillable='true' type='xs:dateTime' />\r\n  <xs:element name='decimal' nillable='true' type='xs:decimal' />\r\n  <xs:element name='double' nillable='true' type='xs:double' />\r\n  <xs:element name='float' nillable='true' type='xs:float' />\r\n  <xs:element name='int' nillable='true' type='xs:int' />\r\n  <xs:element name='long' nillable='true' type='xs:long' />\r\n  <xs:element name='QName' nillable='true' type='xs:QName' />\r\n  <xs:element name='short' nillable='true' type='xs:short' />\r\n  <xs:element name='string' nillable='true' type='xs:string' />\r\n  <xs:element name='unsignedByte' nillable='true' type='xs:unsignedByte' />\r\n  <xs:element name='unsignedInt' nillable='true' type='xs:unsignedInt' />\r\n  <xs:element name='unsignedLong' nillable='true' type='xs:unsignedLong' />\r\n  <xs:element name='unsignedShort' nillable='true' type='xs:unsignedShort' />\r\n  <xs:element name='char' nillable='true' type='tns:char' />\r\n  <xs:simpleType name='char'>\r\n    <xs:restriction base='xs:int'/>\r\n  </xs:simpleType>  \r\n  <xs:element name='duration' nillable='true' type='tns:duration' />\r\n  <xs:simpleType name='duration'>\r\n    <xs:restriction base='xs:duration'>\r\n      <xs:pattern value='\\-?P(\\d*D)?(T(\\d*H)?(\\d*M)?(\\d*(\\.\\d*)?S)?)?' />\r\n      <xs:minInclusive value='-P10675199DT2H48M5.4775808S' />\r\n      <xs:maxInclusive value='P10675199DT2H48M5.4775807S' />\r\n    </xs:restriction>\r\n  </xs:simpleType>\r\n  <xs:element name='guid' nillable='true' type='tns:guid' />\r\n  <xs:simpleType name='guid'>\r\n    <xs:restriction base='xs:string'>\r\n      <xs:pattern value='[\\da-fA-F]{8}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{12}' />\r\n    </xs:restriction>\r\n  </xs:simpleType>\r\n  <xs:attribute name='FactoryType' type='xs:QName' />\r\n  <xs:attribute name='Id' type='xs:ID' />\r\n  <xs:attribute name='Ref' type='xs:IDREF' />\r\n</xs:schema>\r\n";
        public const string SerPrefix = "z";
        public const string SerPrefixForSchema = "ser";
        public const string Space = " ";
        public const string StringLocalName = "string";
        public const string SurrogateDataLocalName = "Surrogate";
        public const string TnsPrefix = "tns";
        public const string True = "true";
        [SecurityCritical]
        private static Type typeOfArray;
        [SecurityCritical]
        private static Type typeOfByteArray;
        [SecurityCritical]
        private static Type typeOfClassDataNode;
        [SecurityCritical]
        private static Type typeOfCollectionDataContractAttribute;
        [SecurityCritical]
        private static Type typeOfCollectionDataNode;
        [SecurityCritical]
        private static Type typeOfContractNamespaceAttribute;
        [SecurityCritical]
        private static Type typeOfDataContractAttribute;
        [SecurityCritical]
        private static Type typeOfDataMemberAttribute;
        [SecurityCritical]
        private static Type typeOfDateTimeOffset;
        [SecurityCritical]
        private static Type typeOfDateTimeOffsetAdapter;
        [SecurityCritical]
        private static Type typeOfDBNull;
        [SecurityCritical]
        private static Type typeOfDictionaryEnumerator;
        [SecurityCritical]
        private static Type typeOfDictionaryGeneric;
        [SecurityCritical]
        private static Type typeOfEnumMemberAttribute;
        [SecurityCritical]
        private static Type typeOfExtensionDataObject;
        [SecurityCritical]
        private static Type typeOfFlagsAttribute;
        [SecurityCritical]
        private static Type typeOfGenericDictionaryEnumerator;
        [SecurityCritical]
        private static Type typeOfGuid;
        [SecurityCritical]
        private static Type typeOfHashtable;
        [SecurityCritical]
        private static Type typeOfICollection;
        [SecurityCritical]
        private static Type typeOfICollectionGeneric;
        [SecurityCritical]
        private static Type typeOfIDeserializationCallback;
        [SecurityCritical]
        private static Type typeOfIDictionary;
        [SecurityCritical]
        private static Type typeOfIDictionaryEnumerator;
        [SecurityCritical]
        private static Type typeOfIDictionaryGeneric;
        [SecurityCritical]
        private static Type typeOfIEnumerable;
        [SecurityCritical]
        private static Type typeOfIEnumerableGeneric;
        [SecurityCritical]
        private static Type typeOfIEnumerator;
        [SecurityCritical]
        private static Type typeOfIEnumeratorGeneric;
        [SecurityCritical]
        private static Type typeOfIExtensibleDataObject;
        [SecurityCritical]
        private static Type typeOfIList;
        [SecurityCritical]
        private static Type typeOfIListGeneric;
        [SecurityCritical]
        private static Type typeOfInt;
        [SecurityCritical]
        private static Type typeOfIObjectReference;
        [SecurityCritical]
        private static Type typeOfIPropertyChange;
        [SecurityCritical]
        private static Type typeOfISerializable;
        [SecurityCritical]
        private static Type typeOfISerializableDataNode;
        [SecurityCritical]
        private static Type typeOfIXmlSerializable;
        [SecurityCritical]
        private static Type typeOfKeyValue;
        [SecurityCritical]
        private static Type typeOfKeyValuePair;
        [SecurityCritical]
        private static Type typeOfKnownTypeAttribute;
        [SecurityCritical]
        private static Type typeOfListGeneric;
        [SecurityCritical]
        private static Type typeOfNonSerializedAttribute;
        [SecurityCritical]
        private static Type typeOfNullable;
        [SecurityCritical]
        private static Type typeOfObject;
        [SecurityCritical]
        private static Type typeOfObjectArray;
        [SecurityCritical]
        private static Type typeOfOnDeserializedAttribute;
        [SecurityCritical]
        private static Type typeOfOnDeserializingAttribute;
        [SecurityCritical]
        private static Type typeOfOnSerializedAttribute;
        [SecurityCritical]
        private static Type typeOfOnSerializingAttribute;
        [SecurityCritical]
        private static Type typeOfOptionalFieldAttribute;
        [SecurityCritical]
        private static Type typeOfReflectionPointer;
        [SecurityCritical]
        private static Type typeOfSerializableAttribute;
        [SecurityCritical]
        private static Type typeOfSerializationEntry;
        [SecurityCritical]
        private static Type typeOfSerializationInfo;
        [SecurityCritical]
        private static Type typeOfSerializationInfoEnumerator;
        [SecurityCritical]
        private static Type typeOfStreamingContext;
        [SecurityCritical]
        private static Type typeOfString;
        [SecurityCritical]
        private static Type typeOfTimeSpan;
        [SecurityCritical]
        private static Type typeOfTypeEnumerable;
        [SecurityCritical]
        private static Type typeOfULong;
        [SecurityCritical]
        private static Type typeOfUri;
        [SecurityCritical]
        private static Type typeOfValueType;
        [SecurityCritical]
        private static Type typeOfVoid;
        [SecurityCritical]
        private static Type typeOfXmlDataNode;
        [SecurityCritical]
        private static Type typeOfXmlElement;
        [SecurityCritical]
        private static Type typeOfXmlFormatClassReaderDelegate;
        [SecurityCritical]
        private static Type typeOfXmlFormatClassWriterDelegate;
        [SecurityCritical]
        private static Type typeOfXmlFormatCollectionReaderDelegate;
        [SecurityCritical]
        private static Type typeOfXmlFormatCollectionWriterDelegate;
        [SecurityCritical]
        private static Type typeOfXmlFormatGetOnlyCollectionReaderDelegate;
        [SecurityCritical]
        private static Type typeOfXmlNodeArray;
        [SecurityCritical]
        private static Type typeOfXmlQualifiedName;
        [SecurityCritical]
        private static Type typeOfXmlRootAttribute;
        [SecurityCritical]
        private static Type typeOfXmlSchemaProviderAttribute;
        [SecurityCritical]
        private static Type typeOfXmlSchemaSet;
        [SecurityCritical]
        private static Type typeOfXmlSchemaType;
        [SecurityCritical]
        private static Type typeOfXmlSerializableServices;
        public const string ValueLocalName = "Value";
        public const string ValueNameProperty = "ValueName";
        public const string ValueProperty = "Value";
        public const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";
        public const string XmlnsPrefix = "xmlns";
        public const string XsdPrefix = "x";
        public const string XsiNilLocalName = "nil";
        public const string XsiPrefix = "i";
        public const string XsiTypeLocalName = "type";

        internal static Uri DataContractXsdBaseNamespaceUri
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (dataContractXsdBaseNamespaceUri == null)
                {
                    dataContractXsdBaseNamespaceUri = new Uri("http://schemas.datacontract.org/2004/07/");
                }
                return dataContractXsdBaseNamespaceUri;
            }
        }

        internal static object[] EmptyObjectArray
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (emptyObjectArray == null)
                {
                    emptyObjectArray = new object[0];
                }
                return emptyObjectArray;
            }
        }

        internal static Type[] EmptyTypeArray
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (emptyTypeArray == null)
                {
                    emptyTypeArray = new Type[0];
                }
                return emptyTypeArray;
            }
        }

        internal static XmlQualifiedName IdQualifiedName
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (idQualifiedName == null)
                {
                    idQualifiedName = new XmlQualifiedName("Id", "http://schemas.microsoft.com/2003/10/Serialization/");
                }
                return idQualifiedName;
            }
        }

        public static ReflectionPermission MemberAccessPermission
        {
            [SecurityCritical]
            get
            {
                if (memberAccessPermission == null)
                {
                    memberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
                }
                return memberAccessPermission;
            }
        }

        internal static XmlQualifiedName RefQualifiedName
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (refQualifiedName == null)
                {
                    refQualifiedName = new XmlQualifiedName("Ref", "http://schemas.microsoft.com/2003/10/Serialization/");
                }
                return refQualifiedName;
            }
        }

        public static SecurityPermission SerializationFormatterPermission
        {
            [SecurityCritical]
            get
            {
                if (serializationFormatterPermission == null)
                {
                    serializationFormatterPermission = new SecurityPermission(SecurityPermissionFlag.SerializationFormatter);
                }
                return serializationFormatterPermission;
            }
        }

        internal static Type TypeOfArray
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfArray == null)
                {
                    typeOfArray = typeof(Array);
                }
                return typeOfArray;
            }
        }

        internal static Type TypeOfByteArray
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfByteArray == null)
                {
                    typeOfByteArray = typeof(byte[]);
                }
                return typeOfByteArray;
            }
        }

        internal static Type TypeOfClassDataNode
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfClassDataNode == null)
                {
                    typeOfClassDataNode = typeof(ClassDataNode);
                }
                return typeOfClassDataNode;
            }
        }

        internal static Type TypeOfCollectionDataContractAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfCollectionDataContractAttribute == null)
                {
                    typeOfCollectionDataContractAttribute = typeof(CollectionDataContractAttribute);
                }
                return typeOfCollectionDataContractAttribute;
            }
        }

        internal static Type TypeOfCollectionDataNode
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfCollectionDataNode == null)
                {
                    typeOfCollectionDataNode = typeof(CollectionDataNode);
                }
                return typeOfCollectionDataNode;
            }
        }

        internal static Type TypeOfContractNamespaceAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfContractNamespaceAttribute == null)
                {
                    typeOfContractNamespaceAttribute = typeof(ContractNamespaceAttribute);
                }
                return typeOfContractNamespaceAttribute;
            }
        }

        internal static Type TypeOfDataContractAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfDataContractAttribute == null)
                {
                    typeOfDataContractAttribute = typeof(DataContractAttribute);
                }
                return typeOfDataContractAttribute;
            }
        }

        internal static Type TypeOfDataMemberAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfDataMemberAttribute == null)
                {
                    typeOfDataMemberAttribute = typeof(DataMemberAttribute);
                }
                return typeOfDataMemberAttribute;
            }
        }

        internal static Type TypeOfDateTimeOffset
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfDateTimeOffset == null)
                {
                    typeOfDateTimeOffset = typeof(DateTimeOffset);
                }
                return typeOfDateTimeOffset;
            }
        }

        internal static Type TypeOfDateTimeOffsetAdapter
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfDateTimeOffsetAdapter == null)
                {
                    typeOfDateTimeOffsetAdapter = typeof(DateTimeOffsetAdapter);
                }
                return typeOfDateTimeOffsetAdapter;
            }
        }

        internal static Type TypeOfDBNull
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfDBNull == null)
                {
                    typeOfDBNull = typeof(DBNull);
                }
                return typeOfDBNull;
            }
        }

        internal static Type TypeOfDictionaryEnumerator
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfDictionaryEnumerator == null)
                {
                    typeOfDictionaryEnumerator = typeof(CollectionDataContract.DictionaryEnumerator);
                }
                return typeOfDictionaryEnumerator;
            }
        }

        internal static Type TypeOfDictionaryGeneric
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfDictionaryGeneric == null)
                {
                    typeOfDictionaryGeneric = typeof(Dictionary<,>);
                }
                return typeOfDictionaryGeneric;
            }
        }

        internal static Type TypeOfEnumMemberAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfEnumMemberAttribute == null)
                {
                    typeOfEnumMemberAttribute = typeof(EnumMemberAttribute);
                }
                return typeOfEnumMemberAttribute;
            }
        }

        internal static Type TypeOfExtensionDataObject
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfExtensionDataObject == null)
                {
                    typeOfExtensionDataObject = typeof(ExtensionDataObject);
                }
                return typeOfExtensionDataObject;
            }
        }

        internal static Type TypeOfFlagsAttribute
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfFlagsAttribute == null)
                {
                    typeOfFlagsAttribute = typeof(FlagsAttribute);
                }
                return typeOfFlagsAttribute;
            }
        }

        internal static Type TypeOfGenericDictionaryEnumerator
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfGenericDictionaryEnumerator == null)
                {
                    typeOfGenericDictionaryEnumerator = typeof(CollectionDataContract.GenericDictionaryEnumerator<, >);
                }
                return typeOfGenericDictionaryEnumerator;
            }
        }

        internal static Type TypeOfGuid
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfGuid == null)
                {
                    typeOfGuid = typeof(Guid);
                }
                return typeOfGuid;
            }
        }

        internal static Type TypeOfHashtable
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfHashtable == null)
                {
                    typeOfHashtable = typeof(Hashtable);
                }
                return typeOfHashtable;
            }
        }

        internal static Type TypeOfICollection
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfICollection == null)
                {
                    typeOfICollection = typeof(ICollection);
                }
                return typeOfICollection;
            }
        }

        internal static Type TypeOfICollectionGeneric
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfICollectionGeneric == null)
                {
                    typeOfICollectionGeneric = typeof(ICollection<>);
                }
                return typeOfICollectionGeneric;
            }
        }

        internal static Type TypeOfIDeserializationCallback
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfIDeserializationCallback == null)
                {
                    typeOfIDeserializationCallback = typeof(IDeserializationCallback);
                }
                return typeOfIDeserializationCallback;
            }
        }

        internal static Type TypeOfIDictionary
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfIDictionary == null)
                {
                    typeOfIDictionary = typeof(IDictionary);
                }
                return typeOfIDictionary;
            }
        }

        internal static Type TypeOfIDictionaryEnumerator
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfIDictionaryEnumerator == null)
                {
                    typeOfIDictionaryEnumerator = typeof(IDictionaryEnumerator);
                }
                return typeOfIDictionaryEnumerator;
            }
        }

        internal static Type TypeOfIDictionaryGeneric
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfIDictionaryGeneric == null)
                {
                    typeOfIDictionaryGeneric = typeof(IDictionary<,>);
                }
                return typeOfIDictionaryGeneric;
            }
        }

        internal static Type TypeOfIEnumerable
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfIEnumerable == null)
                {
                    typeOfIEnumerable = typeof(IEnumerable);
                }
                return typeOfIEnumerable;
            }
        }

        internal static Type TypeOfIEnumerableGeneric
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfIEnumerableGeneric == null)
                {
                    typeOfIEnumerableGeneric = typeof(IEnumerable<>);
                }
                return typeOfIEnumerableGeneric;
            }
        }

        internal static Type TypeOfIEnumerator
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfIEnumerator == null)
                {
                    typeOfIEnumerator = typeof(IEnumerator);
                }
                return typeOfIEnumerator;
            }
        }

        internal static Type TypeOfIEnumeratorGeneric
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfIEnumeratorGeneric == null)
                {
                    typeOfIEnumeratorGeneric = typeof(IEnumerator<>);
                }
                return typeOfIEnumeratorGeneric;
            }
        }

        internal static Type TypeOfIExtensibleDataObject
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfIExtensibleDataObject == null)
                {
                    typeOfIExtensibleDataObject = typeof(IExtensibleDataObject);
                }
                return typeOfIExtensibleDataObject;
            }
        }

        internal static Type TypeOfIList
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfIList == null)
                {
                    typeOfIList = typeof(IList);
                }
                return typeOfIList;
            }
        }

        internal static Type TypeOfIListGeneric
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfIListGeneric == null)
                {
                    typeOfIListGeneric = typeof(IList<>);
                }
                return typeOfIListGeneric;
            }
        }

        internal static Type TypeOfInt
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfInt == null)
                {
                    typeOfInt = typeof(int);
                }
                return typeOfInt;
            }
        }

        internal static Type TypeOfIObjectReference
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfIObjectReference == null)
                {
                    typeOfIObjectReference = typeof(IObjectReference);
                }
                return typeOfIObjectReference;
            }
        }

        internal static Type TypeOfIPropertyChange
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfIPropertyChange == null)
                {
                    typeOfIPropertyChange = typeof(INotifyPropertyChanged);
                }
                return typeOfIPropertyChange;
            }
        }

        internal static Type TypeOfISerializable
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfISerializable == null)
                {
                    typeOfISerializable = typeof(ISerializable);
                }
                return typeOfISerializable;
            }
        }

        internal static Type TypeOfISerializableDataNode
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfISerializableDataNode == null)
                {
                    typeOfISerializableDataNode = typeof(ISerializableDataNode);
                }
                return typeOfISerializableDataNode;
            }
        }

        internal static Type TypeOfIXmlSerializable
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfIXmlSerializable == null)
                {
                    typeOfIXmlSerializable = typeof(IXmlSerializable);
                }
                return typeOfIXmlSerializable;
            }
        }

        internal static Type TypeOfKeyValue
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfKeyValue == null)
                {
                    typeOfKeyValue = typeof(KeyValue<,>);
                }
                return typeOfKeyValue;
            }
        }

        internal static Type TypeOfKeyValuePair
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfKeyValuePair == null)
                {
                    typeOfKeyValuePair = typeof(KeyValuePair<,>);
                }
                return typeOfKeyValuePair;
            }
        }

        internal static Type TypeOfKnownTypeAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfKnownTypeAttribute == null)
                {
                    typeOfKnownTypeAttribute = typeof(KnownTypeAttribute);
                }
                return typeOfKnownTypeAttribute;
            }
        }

        internal static Type TypeOfListGeneric
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfListGeneric == null)
                {
                    typeOfListGeneric = typeof(List<>);
                }
                return typeOfListGeneric;
            }
        }

        internal static Type TypeOfNonSerializedAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfNonSerializedAttribute == null)
                {
                    typeOfNonSerializedAttribute = typeof(NonSerializedAttribute);
                }
                return typeOfNonSerializedAttribute;
            }
        }

        internal static Type TypeOfNullable
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfNullable == null)
                {
                    typeOfNullable = typeof(Nullable<>);
                }
                return typeOfNullable;
            }
        }

        internal static Type TypeOfObject
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfObject == null)
                {
                    typeOfObject = typeof(object);
                }
                return typeOfObject;
            }
        }

        internal static Type TypeOfObjectArray
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfObjectArray == null)
                {
                    typeOfObjectArray = typeof(object[]);
                }
                return typeOfObjectArray;
            }
        }

        internal static Type TypeOfOnDeserializedAttribute
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfOnDeserializedAttribute == null)
                {
                    typeOfOnDeserializedAttribute = typeof(OnDeserializedAttribute);
                }
                return typeOfOnDeserializedAttribute;
            }
        }

        internal static Type TypeOfOnDeserializingAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfOnDeserializingAttribute == null)
                {
                    typeOfOnDeserializingAttribute = typeof(OnDeserializingAttribute);
                }
                return typeOfOnDeserializingAttribute;
            }
        }

        internal static Type TypeOfOnSerializedAttribute
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfOnSerializedAttribute == null)
                {
                    typeOfOnSerializedAttribute = typeof(OnSerializedAttribute);
                }
                return typeOfOnSerializedAttribute;
            }
        }

        internal static Type TypeOfOnSerializingAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfOnSerializingAttribute == null)
                {
                    typeOfOnSerializingAttribute = typeof(OnSerializingAttribute);
                }
                return typeOfOnSerializingAttribute;
            }
        }

        internal static Type TypeOfOptionalFieldAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfOptionalFieldAttribute == null)
                {
                    typeOfOptionalFieldAttribute = typeof(OptionalFieldAttribute);
                }
                return typeOfOptionalFieldAttribute;
            }
        }

        internal static Type TypeOfReflectionPointer
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfReflectionPointer == null)
                {
                    typeOfReflectionPointer = typeof(Pointer);
                }
                return typeOfReflectionPointer;
            }
        }

        internal static Type TypeOfSerializableAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfSerializableAttribute == null)
                {
                    typeOfSerializableAttribute = typeof(SerializableAttribute);
                }
                return typeOfSerializableAttribute;
            }
        }

        internal static Type TypeOfSerializationEntry
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfSerializationEntry == null)
                {
                    typeOfSerializationEntry = typeof(SerializationEntry);
                }
                return typeOfSerializationEntry;
            }
        }

        internal static Type TypeOfSerializationInfo
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfSerializationInfo == null)
                {
                    typeOfSerializationInfo = typeof(SerializationInfo);
                }
                return typeOfSerializationInfo;
            }
        }

        internal static Type TypeOfSerializationInfoEnumerator
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfSerializationInfoEnumerator == null)
                {
                    typeOfSerializationInfoEnumerator = typeof(SerializationInfoEnumerator);
                }
                return typeOfSerializationInfoEnumerator;
            }
        }

        internal static Type TypeOfStreamingContext
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfStreamingContext == null)
                {
                    typeOfStreamingContext = typeof(StreamingContext);
                }
                return typeOfStreamingContext;
            }
        }

        internal static Type TypeOfString
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfString == null)
                {
                    typeOfString = typeof(string);
                }
                return typeOfString;
            }
        }

        internal static Type TypeOfTimeSpan
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfTimeSpan == null)
                {
                    typeOfTimeSpan = typeof(TimeSpan);
                }
                return typeOfTimeSpan;
            }
        }

        internal static Type TypeOfTypeEnumerable
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfTypeEnumerable == null)
                {
                    typeOfTypeEnumerable = typeof(IEnumerable<Type>);
                }
                return typeOfTypeEnumerable;
            }
        }

        internal static Type TypeOfULong
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfULong == null)
                {
                    typeOfULong = typeof(ulong);
                }
                return typeOfULong;
            }
        }

        internal static Type TypeOfUri
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfUri == null)
                {
                    typeOfUri = typeof(Uri);
                }
                return typeOfUri;
            }
        }

        internal static Type TypeOfValueType
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfValueType == null)
                {
                    typeOfValueType = typeof(ValueType);
                }
                return typeOfValueType;
            }
        }

        internal static Type TypeOfVoid
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfVoid == null)
                {
                    typeOfVoid = typeof(void);
                }
                return typeOfVoid;
            }
        }

        internal static Type TypeOfXmlDataNode
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfXmlDataNode == null)
                {
                    typeOfXmlDataNode = typeof(XmlDataNode);
                }
                return typeOfXmlDataNode;
            }
        }

        internal static Type TypeOfXmlElement
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfXmlElement == null)
                {
                    typeOfXmlElement = typeof(XmlElement);
                }
                return typeOfXmlElement;
            }
        }

        internal static Type TypeOfXmlFormatClassReaderDelegate
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfXmlFormatClassReaderDelegate == null)
                {
                    typeOfXmlFormatClassReaderDelegate = typeof(XmlFormatClassReaderDelegate);
                }
                return typeOfXmlFormatClassReaderDelegate;
            }
        }

        internal static Type TypeOfXmlFormatClassWriterDelegate
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfXmlFormatClassWriterDelegate == null)
                {
                    typeOfXmlFormatClassWriterDelegate = typeof(XmlFormatClassWriterDelegate);
                }
                return typeOfXmlFormatClassWriterDelegate;
            }
        }

        internal static Type TypeOfXmlFormatCollectionReaderDelegate
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfXmlFormatCollectionReaderDelegate == null)
                {
                    typeOfXmlFormatCollectionReaderDelegate = typeof(XmlFormatCollectionReaderDelegate);
                }
                return typeOfXmlFormatCollectionReaderDelegate;
            }
        }

        internal static Type TypeOfXmlFormatCollectionWriterDelegate
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfXmlFormatCollectionWriterDelegate == null)
                {
                    typeOfXmlFormatCollectionWriterDelegate = typeof(XmlFormatCollectionWriterDelegate);
                }
                return typeOfXmlFormatCollectionWriterDelegate;
            }
        }

        internal static Type TypeOfXmlFormatGetOnlyCollectionReaderDelegate
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfXmlFormatGetOnlyCollectionReaderDelegate == null)
                {
                    typeOfXmlFormatGetOnlyCollectionReaderDelegate = typeof(XmlFormatGetOnlyCollectionReaderDelegate);
                }
                return typeOfXmlFormatGetOnlyCollectionReaderDelegate;
            }
        }

        internal static Type TypeOfXmlNodeArray
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfXmlNodeArray == null)
                {
                    typeOfXmlNodeArray = typeof(System.Xml.XmlNode[]);
                }
                return typeOfXmlNodeArray;
            }
        }

        internal static Type TypeOfXmlQualifiedName
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfXmlQualifiedName == null)
                {
                    typeOfXmlQualifiedName = typeof(XmlQualifiedName);
                }
                return typeOfXmlQualifiedName;
            }
        }

        internal static Type TypeOfXmlRootAttribute
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfXmlRootAttribute == null)
                {
                    typeOfXmlRootAttribute = typeof(XmlRootAttribute);
                }
                return typeOfXmlRootAttribute;
            }
        }

        internal static Type TypeOfXmlSchemaProviderAttribute
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfXmlSchemaProviderAttribute == null)
                {
                    typeOfXmlSchemaProviderAttribute = typeof(XmlSchemaProviderAttribute);
                }
                return typeOfXmlSchemaProviderAttribute;
            }
        }

        internal static Type TypeOfXmlSchemaSet
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (typeOfXmlSchemaSet == null)
                {
                    typeOfXmlSchemaSet = typeof(XmlSchemaSet);
                }
                return typeOfXmlSchemaSet;
            }
        }

        internal static Type TypeOfXmlSchemaType
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfXmlSchemaType == null)
                {
                    typeOfXmlSchemaType = typeof(XmlSchemaType);
                }
                return typeOfXmlSchemaType;
            }
        }

        internal static Type TypeOfXmlSerializableServices
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (typeOfXmlSerializableServices == null)
                {
                    typeOfXmlSerializableServices = typeof(XmlSerializableServices);
                }
                return typeOfXmlSerializableServices;
            }
        }
    }
}


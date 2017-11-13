namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml;

    public sealed class DataContractSerializer : XmlObjectSerializer
    {
        private IDataContractSurrogate dataContractSurrogate;
        private bool ignoreExtensionDataObject;
        internal Dictionary<XmlQualifiedName, DataContract> knownDataContracts;
        private ReadOnlyCollection<Type> knownTypeCollection;
        internal IList<Type> knownTypeList;
        private int maxItemsInObjectGraph;
        private bool needsContractNsAtRoot;
        private bool preserveObjectReferences;
        private DataContract rootContract;
        private XmlDictionaryString rootName;
        private XmlDictionaryString rootNamespace;
        private Type rootType;

        public DataContractSerializer(Type type) : this(type, null)
        {
        }

        public DataContractSerializer(Type type, IEnumerable<Type> knownTypes) : this(type, knownTypes, 0x7fffffff, false, false, null)
        {
        }

        public DataContractSerializer(Type type, string rootName, string rootNamespace) : this(type, rootName, rootNamespace, null)
        {
        }

        public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace) : this(type, rootName, rootNamespace, null)
        {
        }

        public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes) : this(type, rootName, rootNamespace, knownTypes, 0x7fffffff, false, false, null)
        {
        }

        public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes) : this(type, rootName, rootNamespace, knownTypes, 0x7fffffff, false, false, null)
        {
        }

        public DataContractSerializer(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
        {
            this.Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
        }

        public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
        {
            XmlDictionary dictionary = new XmlDictionary(2);
            this.Initialize(type, dictionary.Add(rootName), dictionary.Add(DataContract.GetNamespace(rootNamespace)), knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
        }

        public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
        {
            this.Initialize(type, rootName, rootNamespace, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
        }

        internal static DataContract GetDataContract(DataContract declaredTypeContract, Type declaredType, Type objectType)
        {
            if (declaredType.IsInterface && CollectionDataContract.IsCollectionInterface(declaredType))
            {
                return declaredTypeContract;
            }
            if (declaredType.IsArray)
            {
                return declaredTypeContract;
            }
            return DataContract.GetDataContract(objectType.TypeHandle, objectType, SerializationMode.SharedContract);
        }

        internal override Type GetDeserializeType() => 
            this.rootType;

        internal override Type GetSerializeType(object graph)
        {
            if (graph != null)
            {
                return graph.GetType();
            }
            return this.rootType;
        }

        internal static Type GetSurrogatedType(IDataContractSurrogate dataContractSurrogate, Type type) => 
            DataContractSurrogateCaller.GetDataContractType(dataContractSurrogate, DataContract.UnwrapNullableType(type));

        private void Initialize(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
        {
            XmlObjectSerializer.CheckNull(type, "type");
            this.rootType = type;
            if (knownTypes != null)
            {
                this.knownTypeList = new List<Type>();
                foreach (Type type2 in knownTypes)
                {
                    this.knownTypeList.Add(type2);
                }
            }
            if (maxItemsInObjectGraph < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxItemsInObjectGraph", System.Runtime.Serialization.SR.GetString("ValueMustBeNonNegative")));
            }
            this.maxItemsInObjectGraph = maxItemsInObjectGraph;
            this.ignoreExtensionDataObject = ignoreExtensionDataObject;
            this.preserveObjectReferences = preserveObjectReferences;
            this.dataContractSurrogate = dataContractSurrogate;
        }

        private void Initialize(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
        {
            this.Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
            this.rootName = rootName;
            this.rootNamespace = rootNamespace;
        }

        internal override bool InternalIsStartObject(XmlReaderDelegator reader) => 
            base.IsRootElement(reader, this.RootContract, this.rootName, this.rootNamespace);

        internal override object InternalReadObject(XmlReaderDelegator xmlReader, bool verifyObjectName)
        {
            if (this.MaxItemsInObjectGraph == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("ExceededMaxItemsQuota", new object[] { this.MaxItemsInObjectGraph })));
            }
            if (verifyObjectName)
            {
                if (!this.InternalIsStartObject(xmlReader))
                {
                    XmlDictionaryString topLevelElementName;
                    XmlDictionaryString topLevelElementNamespace;
                    if (this.rootName == null)
                    {
                        topLevelElementName = this.RootContract.TopLevelElementName;
                        topLevelElementNamespace = this.RootContract.TopLevelElementNamespace;
                    }
                    else
                    {
                        topLevelElementName = this.rootName;
                        topLevelElementNamespace = this.rootNamespace;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(System.Runtime.Serialization.SR.GetString("ExpectingElement", new object[] { topLevelElementNamespace, topLevelElementName }), xmlReader));
                }
            }
            else if (!base.IsStartElement(xmlReader))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(System.Runtime.Serialization.SR.GetString("ExpectingElementAtDeserialize", new object[] { XmlNodeType.Element }), xmlReader));
            }
            DataContract rootContract = this.RootContract;
            if (rootContract.IsPrimitive && object.ReferenceEquals(rootContract.UnderlyingType, this.rootType))
            {
                return rootContract.ReadXmlValue(xmlReader, null);
            }
            if (base.IsRootXmlAny(this.rootName, rootContract))
            {
                return XmlObjectSerializerReadContext.ReadRootIXmlSerializable(xmlReader, rootContract as XmlDataContract, false);
            }
            return XmlObjectSerializerReadContext.CreateContext(this, rootContract).InternalDeserialize(xmlReader, this.rootType, rootContract, null, null);
        }

        internal override void InternalWriteEndObject(XmlWriterDelegator writer)
        {
            if (!base.IsRootXmlAny(this.rootName, this.RootContract))
            {
                writer.WriteEndElement();
            }
        }

        internal override void InternalWriteObject(XmlWriterDelegator writer, object graph)
        {
            this.InternalWriteStartObject(writer, graph);
            this.InternalWriteObjectContent(writer, graph);
            this.InternalWriteEndObject(writer);
        }

        internal override void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
        {
            if (this.MaxItemsInObjectGraph == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("ExceededMaxItemsQuota", new object[] { this.MaxItemsInObjectGraph })));
            }
            DataContract rootContract = this.RootContract;
            Type underlyingType = rootContract.UnderlyingType;
            Type objType = (graph == null) ? underlyingType : graph.GetType();
            if (this.dataContractSurrogate != null)
            {
                graph = SurrogateToDataContractType(this.dataContractSurrogate, graph, underlyingType, ref objType);
            }
            if (graph == null)
            {
                if (base.IsRootXmlAny(this.rootName, rootContract))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("IsAnyCannotBeNull", new object[] { underlyingType })));
                }
                XmlObjectSerializer.WriteNull(writer);
            }
            else if (underlyingType == objType)
            {
                if (rootContract.CanContainReferences)
                {
                    XmlObjectSerializerWriteContext context = XmlObjectSerializerWriteContext.CreateContext(this, rootContract);
                    context.HandleGraphAtTopLevel(writer, graph, rootContract);
                    context.SerializeWithoutXsiType(rootContract, writer, graph, underlyingType.TypeHandle);
                }
                else
                {
                    rootContract.WriteXmlValue(writer, graph, null);
                }
            }
            else
            {
                XmlObjectSerializerWriteContext context2 = null;
                if (base.IsRootXmlAny(this.rootName, rootContract))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("IsAnyCannotBeSerializedAsDerivedType", new object[] { objType, rootContract.UnderlyingType })));
                }
                rootContract = GetDataContract(rootContract, underlyingType, objType);
                context2 = XmlObjectSerializerWriteContext.CreateContext(this, this.RootContract);
                if (rootContract.CanContainReferences)
                {
                    context2.HandleGraphAtTopLevel(writer, graph, rootContract);
                }
                context2.OnHandleIsReference(writer, rootContract, graph);
                context2.SerializeWithXsiTypeAtTopLevel(rootContract, writer, graph, underlyingType.TypeHandle);
            }
        }

        internal override void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
        {
            base.WriteRootElement(writer, this.RootContract, this.rootName, this.rootNamespace, this.needsContractNsAtRoot);
        }

        public override bool IsStartObject(XmlDictionaryReader reader) => 
            base.IsStartObjectHandleExceptions(new XmlReaderDelegator(reader));

        public override bool IsStartObject(XmlReader reader) => 
            base.IsStartObjectHandleExceptions(new XmlReaderDelegator(reader));

        public override object ReadObject(XmlReader reader) => 
            base.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), true);

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName) => 
            base.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), verifyObjectName);

        public override object ReadObject(XmlReader reader, bool verifyObjectName) => 
            base.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), verifyObjectName);

        internal static object SurrogateToDataContractType(IDataContractSurrogate dataContractSurrogate, object oldObj, Type surrogatedDeclaredType, ref Type objType)
        {
            object obj2 = DataContractSurrogateCaller.GetObjectToSerialize(dataContractSurrogate, oldObj, objType, surrogatedDeclaredType);
            if (obj2 != oldObj)
            {
                if (obj2 == null)
                {
                    objType = Globals.TypeOfObject;
                    return obj2;
                }
                objType = obj2.GetType();
            }
            return obj2;
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            base.WriteEndObjectHandleExceptions(new XmlWriterDelegator(writer));
        }

        public override void WriteEndObject(XmlWriter writer)
        {
            base.WriteEndObjectHandleExceptions(new XmlWriterDelegator(writer));
        }

        public override void WriteObject(XmlWriter writer, object graph)
        {
            base.WriteObjectHandleExceptions(new XmlWriterDelegator(writer), graph);
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            base.WriteObjectContentHandleExceptions(new XmlWriterDelegator(writer), graph);
        }

        public override void WriteObjectContent(XmlWriter writer, object graph)
        {
            base.WriteObjectContentHandleExceptions(new XmlWriterDelegator(writer), graph);
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            base.WriteStartObjectHandleExceptions(new XmlWriterDelegator(writer), graph);
        }

        public override void WriteStartObject(XmlWriter writer, object graph)
        {
            base.WriteStartObjectHandleExceptions(new XmlWriterDelegator(writer), graph);
        }

        public IDataContractSurrogate DataContractSurrogate =>
            this.dataContractSurrogate;

        public bool IgnoreExtensionDataObject =>
            this.ignoreExtensionDataObject;

        internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContractDictionary
        {
            get
            {
                if ((this.knownDataContracts == null) && (this.knownTypeList != null))
                {
                    this.knownDataContracts = XmlObjectSerializerContext.GetDataContractsForKnownTypes(this.knownTypeList);
                }
                return this.knownDataContracts;
            }
        }

        public ReadOnlyCollection<Type> KnownTypes
        {
            get
            {
                if (this.knownTypeCollection == null)
                {
                    if (this.knownTypeList != null)
                    {
                        this.knownTypeCollection = new ReadOnlyCollection<Type>(this.knownTypeList);
                    }
                    else
                    {
                        this.knownTypeCollection = new ReadOnlyCollection<Type>(Globals.EmptyTypeArray);
                    }
                }
                return this.knownTypeCollection;
            }
        }

        public int MaxItemsInObjectGraph =>
            this.maxItemsInObjectGraph;

        public bool PreserveObjectReferences =>
            this.preserveObjectReferences;

        private DataContract RootContract
        {
            get
            {
                if (this.rootContract == null)
                {
                    this.rootContract = DataContract.GetDataContract((this.dataContractSurrogate == null) ? this.rootType : GetSurrogatedType(this.dataContractSurrogate, this.rootType));
                    this.needsContractNsAtRoot = base.CheckIfNeedsContractNsAtRoot(this.rootName, this.rootNamespace, this.rootContract);
                }
                return this.rootContract;
            }
        }
    }
}


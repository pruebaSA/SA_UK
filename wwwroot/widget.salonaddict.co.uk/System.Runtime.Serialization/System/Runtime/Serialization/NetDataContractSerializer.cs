namespace System.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization.Formatters;
    using System.Xml;

    public sealed class NetDataContractSerializer : XmlObjectSerializer, IFormatter
    {
        private FormatterAssemblyStyle assemblyFormat;
        private SerializationBinder binder;
        private DataContract cachedDataContract;
        private StreamingContext context;
        private bool ignoreExtensionDataObject;
        private int maxItemsInObjectGraph;
        private XmlDictionaryString rootName;
        private XmlDictionaryString rootNamespace;
        private ISurrogateSelector surrogateSelector;

        public NetDataContractSerializer() : this(new StreamingContext(StreamingContextStates.All))
        {
        }

        public NetDataContractSerializer(StreamingContext context) : this(context, 0x7fffffff, false, FormatterAssemblyStyle.Full, null)
        {
        }

        public NetDataContractSerializer(string rootName, string rootNamespace) : this(rootName, rootNamespace, new StreamingContext(StreamingContextStates.All), 0x7fffffff, false, FormatterAssemblyStyle.Full, null)
        {
        }

        public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace) : this(rootName, rootNamespace, new StreamingContext(StreamingContextStates.All), 0x7fffffff, false, FormatterAssemblyStyle.Full, null)
        {
        }

        public NetDataContractSerializer(StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
        {
            this.Initialize(context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
        }

        public NetDataContractSerializer(string rootName, string rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
        {
            XmlDictionary dictionary = new XmlDictionary(2);
            this.Initialize(dictionary.Add(rootName), dictionary.Add(DataContract.GetNamespace(rootNamespace)), context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
        }

        public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
        {
            this.Initialize(rootName, rootNamespace, context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
        }

        public object Deserialize(Stream stream) => 
            base.ReadObject(stream);

        internal DataContract GetDataContract(object obj, ref Hashtable surrogateDataContracts) => 
            this.GetDataContract((obj == null) ? Globals.TypeOfObject : obj.GetType(), ref surrogateDataContracts);

        internal DataContract GetDataContract(Type type, ref Hashtable surrogateDataContracts) => 
            this.GetDataContract(type.TypeHandle, type, ref surrogateDataContracts);

        internal DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type, ref Hashtable surrogateDataContracts)
        {
            DataContract contract = GetDataContractFromSurrogateSelector(this.surrogateSelector, this.Context, typeHandle, type, ref surrogateDataContracts);
            if (contract != null)
            {
                return contract;
            }
            if (this.cachedDataContract == null)
            {
                contract = DataContract.GetDataContract(typeHandle, type, SerializationMode.SharedType);
                this.cachedDataContract = contract;
                return contract;
            }
            DataContract cachedDataContract = this.cachedDataContract;
            if (cachedDataContract.UnderlyingType.TypeHandle.Equals(typeHandle))
            {
                return cachedDataContract;
            }
            return DataContract.GetDataContract(typeHandle, type, SerializationMode.SharedType);
        }

        internal static DataContract GetDataContractFromSurrogateSelector(ISurrogateSelector surrogateSelector, StreamingContext context, RuntimeTypeHandle typeHandle, Type type, ref Hashtable surrogateDataContracts)
        {
            ISurrogateSelector selector;
            if (surrogateSelector == null)
            {
                return null;
            }
            if (type == null)
            {
                type = Type.GetTypeFromHandle(typeHandle);
            }
            DataContract builtInDataContract = DataContract.GetBuiltInDataContract(type);
            if (builtInDataContract != null)
            {
                return builtInDataContract;
            }
            if (surrogateDataContracts != null)
            {
                DataContract contract2 = (DataContract) surrogateDataContracts[type];
                if (contract2 != null)
                {
                    return contract2;
                }
            }
            DataContract contract3 = null;
            ISerializationSurrogate serializationSurrogate = surrogateSelector.GetSurrogate(type, context, out selector);
            if (serializationSurrogate != null)
            {
                contract3 = new SurrogateDataContract(type, serializationSurrogate);
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                DataContract itemContract = GetDataContractFromSurrogateSelector(surrogateSelector, context, elementType.TypeHandle, elementType, ref surrogateDataContracts);
                if (itemContract == null)
                {
                    itemContract = DataContract.GetDataContract(elementType.TypeHandle, elementType, SerializationMode.SharedType);
                }
                contract3 = new CollectionDataContract(type, itemContract);
            }
            if (contract3 == null)
            {
                return null;
            }
            if (surrogateDataContracts == null)
            {
                surrogateDataContracts = new Hashtable();
            }
            surrogateDataContracts.Add(type, contract3);
            return contract3;
        }

        private void Initialize(StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
        {
            this.context = context;
            if (maxItemsInObjectGraph < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxItemsInObjectGraph", System.Runtime.Serialization.SR.GetString("ValueMustBeNonNegative")));
            }
            this.maxItemsInObjectGraph = maxItemsInObjectGraph;
            this.ignoreExtensionDataObject = ignoreExtensionDataObject;
            this.surrogateSelector = surrogateSelector;
            this.AssemblyFormat = assemblyFormat;
        }

        private void Initialize(XmlDictionaryString rootName, XmlDictionaryString rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
        {
            this.Initialize(context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
            this.rootName = rootName;
            this.rootNamespace = rootNamespace;
        }

        internal override bool InternalIsStartObject(XmlReaderDelegator reader) => 
            base.IsStartElement(reader);

        internal override object InternalReadObject(XmlReaderDelegator xmlReader, bool verifyObjectName)
        {
            if (this.MaxItemsInObjectGraph == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("ExceededMaxItemsQuota", new object[] { this.MaxItemsInObjectGraph })));
            }
            if (!base.IsStartElement(xmlReader))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(System.Runtime.Serialization.SR.GetString("ExpectingElementAtDeserialize", new object[] { XmlNodeType.Element }), xmlReader));
            }
            return XmlObjectSerializerReadContext.CreateContext(this).InternalDeserialize(xmlReader, (Type) null, null, (string) null);
        }

        internal override void InternalWriteEndObject(XmlWriterDelegator writer)
        {
            writer.WriteEndElement();
        }

        internal override void InternalWriteObject(XmlWriterDelegator writer, object graph)
        {
            Hashtable surrogateDataContracts = null;
            DataContract dataContract = this.GetDataContract(graph, ref surrogateDataContracts);
            this.InternalWriteStartObject(writer, graph, dataContract);
            this.InternalWriteObjectContent(writer, graph, dataContract, surrogateDataContracts);
            this.InternalWriteEndObject(writer);
        }

        internal override void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
        {
            Hashtable surrogateDataContracts = null;
            DataContract dataContract = this.GetDataContract(graph, ref surrogateDataContracts);
            this.InternalWriteObjectContent(writer, graph, dataContract, surrogateDataContracts);
        }

        private void InternalWriteObjectContent(XmlWriterDelegator writer, object graph, DataContract contract, Hashtable surrogateDataContracts)
        {
            if (this.MaxItemsInObjectGraph == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("ExceededMaxItemsQuota", new object[] { this.MaxItemsInObjectGraph })));
            }
            if (base.IsRootXmlAny(this.rootName, contract))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("IsAnyNotSupportedByNetDataContractSerializer", new object[] { contract.UnderlyingType })));
            }
            if (graph == null)
            {
                XmlObjectSerializer.WriteNull(writer);
            }
            else
            {
                Type type = graph.GetType();
                if (contract.UnderlyingType != type)
                {
                    contract = this.GetDataContract(graph, ref surrogateDataContracts);
                }
                XmlObjectSerializerWriteContext context = null;
                if (contract.CanContainReferences)
                {
                    context = XmlObjectSerializerWriteContext.CreateContext(this, surrogateDataContracts);
                    context.HandleGraphAtTopLevel(writer, graph, contract);
                }
                WriteClrTypeInfo(writer, contract);
                contract.WriteXmlValue(writer, graph, context);
            }
        }

        internal override void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
        {
            Hashtable surrogateDataContracts = null;
            DataContract dataContract = this.GetDataContract(graph, ref surrogateDataContracts);
            this.InternalWriteStartObject(writer, graph, dataContract);
        }

        private void InternalWriteStartObject(XmlWriterDelegator writer, object graph, DataContract contract)
        {
            base.WriteRootElement(writer, contract, this.rootName, this.rootNamespace, base.CheckIfNeedsContractNsAtRoot(this.rootName, this.rootNamespace, contract));
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

        public void Serialize(Stream stream, object graph)
        {
            base.WriteObject(stream, graph);
        }

        internal static void WriteClrTypeInfo(XmlWriterDelegator writer, DataContract dataContract)
        {
            if (!dataContract.IsISerializable && !(dataContract is SurrogateDataContract))
            {
                if (dataContract.UnderlyingType == Globals.TypeOfDateTimeOffsetAdapter)
                {
                    WriteClrTypeInfo(writer, DataContract.GetClrTypeFullName(Globals.TypeOfDateTimeOffset), Globals.TypeOfDateTimeOffset.Assembly.FullName);
                }
                else
                {
                    WriteClrTypeInfo(writer, DataContract.GetClrTypeFullName(dataContract.UnderlyingType), dataContract.UnderlyingType.Assembly.FullName);
                }
            }
        }

        internal static void WriteClrTypeInfo(XmlWriterDelegator writer, string clrTypeName, string clrAssemblyName)
        {
            if (clrTypeName != null)
            {
                writer.WriteAttributeString("z", DictionaryGlobals.ClrTypeLocalName, DictionaryGlobals.SerializationNamespace, DataContract.GetClrTypeString(clrTypeName));
            }
            if (clrAssemblyName != null)
            {
                writer.WriteAttributeString("z", DictionaryGlobals.ClrAssemblyLocalName, DictionaryGlobals.SerializationNamespace, DataContract.GetClrTypeString(clrAssemblyName));
            }
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

        public FormatterAssemblyStyle AssemblyFormat
        {
            get => 
                this.assemblyFormat;
            set
            {
                if ((value != FormatterAssemblyStyle.Full) && (value != FormatterAssemblyStyle.Simple))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("InvalidAssemblyFormat", new object[] { value })));
                }
                this.assemblyFormat = value;
            }
        }

        public SerializationBinder Binder
        {
            get => 
                this.binder;
            set
            {
                this.binder = value;
            }
        }

        public StreamingContext Context
        {
            get => 
                this.context;
            set
            {
                this.context = value;
            }
        }

        public bool IgnoreExtensionDataObject =>
            this.ignoreExtensionDataObject;

        public int MaxItemsInObjectGraph =>
            this.maxItemsInObjectGraph;

        public ISurrogateSelector SurrogateSelector
        {
            get => 
                this.surrogateSelector;
            set
            {
                this.surrogateSelector = value;
            }
        }
    }
}


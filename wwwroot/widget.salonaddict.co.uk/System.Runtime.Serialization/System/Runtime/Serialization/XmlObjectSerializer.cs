namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.ServiceModel.Diagnostics;
    using System.Text;
    using System.Xml;

    public abstract class XmlObjectSerializer
    {
        [SecurityCritical]
        private static IFormatterConverter formatterConverter;

        protected XmlObjectSerializer()
        {
        }

        internal bool CheckIfNeedsContractNsAtRoot(XmlDictionaryString name, XmlDictionaryString ns, DataContract contract)
        {
            if (name == null)
            {
                return false;
            }
            if ((contract.IsBuiltInDataContract || !contract.CanContainReferences) || contract.IsISerializable)
            {
                return false;
            }
            string str = XmlDictionaryString.GetString(contract.Namespace);
            return (!string.IsNullOrEmpty(str) && (str != XmlDictionaryString.GetString(ns)));
        }

        internal static void CheckNull(object obj, string name)
        {
            if (obj == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(name));
            }
        }

        internal static SerializationException CreateSerializationException(string errorMessage) => 
            CreateSerializationException(errorMessage, null);

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static SerializationException CreateSerializationException(string errorMessage, Exception innerException) => 
            new SerializationException(errorMessage, innerException);

        internal static Exception CreateSerializationExceptionWithReaderDetails(string errorMessage, XmlReaderDelegator reader) => 
            CreateSerializationException(TryAddLineInfo(reader, System.Runtime.Serialization.SR.GetString("EncounteredWithNameNamespace", new object[] { errorMessage, reader.NodeType, reader.LocalName, reader.NamespaceURI })));

        internal virtual Type GetDeserializeType() => 
            null;

        internal virtual Type GetSerializeType(object graph)
        {
            if (graph != null)
            {
                return graph.GetType();
            }
            return null;
        }

        private static string GetTypeInfo(Type type)
        {
            if (type != null)
            {
                return DataContract.GetClrTypeFullName(type);
            }
            return string.Empty;
        }

        private static string GetTypeInfoError(string errorMessage, Type type, Exception innerException)
        {
            string str = (type == null) ? string.Empty : System.Runtime.Serialization.SR.GetString("ErrorTypeInfo", new object[] { DataContract.GetClrTypeFullName(type) });
            string str2 = (innerException == null) ? string.Empty : innerException.Message;
            return System.Runtime.Serialization.SR.GetString(errorMessage, new object[] { str, str2 });
        }

        internal virtual bool InternalIsStartObject(XmlReaderDelegator reader)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        internal virtual object InternalReadObject(XmlReaderDelegator reader, bool verifyObjectName) => 
            this.ReadObject(reader.UnderlyingReader, verifyObjectName);

        internal virtual void InternalWriteEndObject(XmlWriterDelegator writer)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        internal virtual void InternalWriteObject(XmlWriterDelegator writer, object graph)
        {
            this.WriteStartObject(writer.Writer, graph);
            this.WriteObjectContent(writer.Writer, graph);
            this.WriteEndObject(writer.Writer);
        }

        internal virtual void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        internal virtual void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        internal bool IsRootElement(XmlReaderDelegator reader, DataContract contract, XmlDictionaryString name, XmlDictionaryString ns)
        {
            reader.MoveToElement();
            if (name != null)
            {
                return reader.IsStartElement(name, ns);
            }
            if (!contract.HasRoot)
            {
                return reader.IsStartElement();
            }
            if (reader.IsStartElement(contract.TopLevelElementName, contract.TopLevelElementNamespace))
            {
                return true;
            }
            ClassDataContract baseContract = contract as ClassDataContract;
            if (baseContract != null)
            {
                baseContract = baseContract.BaseContract;
            }
            while (baseContract != null)
            {
                if (reader.IsStartElement(baseContract.TopLevelElementName, baseContract.TopLevelElementNamespace))
                {
                    return true;
                }
                baseContract = baseContract.BaseContract;
            }
            if (baseContract == null)
            {
                DataContract primitiveDataContract = PrimitiveDataContract.GetPrimitiveDataContract(Globals.TypeOfObject);
                if (reader.IsStartElement(primitiveDataContract.TopLevelElementName, primitiveDataContract.TopLevelElementNamespace))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsRootXmlAny(XmlDictionaryString rootName, DataContract contract) => 
            ((rootName == null) && !contract.HasRoot);

        internal bool IsStartElement(XmlReaderDelegator reader)
        {
            if (!reader.MoveToElement())
            {
                return reader.IsStartElement();
            }
            return true;
        }

        public abstract bool IsStartObject(XmlDictionaryReader reader);
        public virtual bool IsStartObject(XmlReader reader)
        {
            CheckNull(reader, "reader");
            return this.IsStartObject(XmlDictionaryReader.CreateDictionaryReader(reader));
        }

        internal bool IsStartObjectHandleExceptions(XmlReaderDelegator reader)
        {
            bool flag;
            try
            {
                CheckNull(reader, "reader");
                flag = this.InternalIsStartObject(reader);
            }
            catch (XmlException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorIsStartObject", this.GetDeserializeType(), exception), exception));
            }
            catch (FormatException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorIsStartObject", this.GetDeserializeType(), exception2), exception2));
            }
            return flag;
        }

        public virtual object ReadObject(Stream stream)
        {
            CheckNull(stream, "stream");
            return this.ReadObject(XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max));
        }

        public virtual object ReadObject(XmlDictionaryReader reader) => 
            this.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), true);

        public virtual object ReadObject(XmlReader reader)
        {
            CheckNull(reader, "reader");
            return this.ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader));
        }

        public abstract object ReadObject(XmlDictionaryReader reader, bool verifyObjectName);
        public virtual object ReadObject(XmlReader reader, bool verifyObjectName)
        {
            CheckNull(reader, "reader");
            return this.ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader), verifyObjectName);
        }

        internal object ReadObjectHandleExceptions(XmlReaderDelegator reader, bool verifyObjectName)
        {
            object obj3;
            try
            {
                CheckNull(reader, "reader");
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ReadObjectBegin, System.Runtime.Serialization.SR.GetString("TraceCodeReadObjectBegin"), new StringTraceRecord("Type", GetTypeInfo(this.GetDeserializeType())));
                    object obj2 = this.InternalReadObject(reader, verifyObjectName);
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ReadObjectEnd, System.Runtime.Serialization.SR.GetString("TraceCodeReadObjectEnd"), new StringTraceRecord("Type", GetTypeInfo(this.GetDeserializeType())));
                    return obj2;
                }
                obj3 = this.InternalReadObject(reader, verifyObjectName);
            }
            catch (XmlException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorDeserializing", this.GetDeserializeType(), exception), exception));
            }
            catch (FormatException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorDeserializing", this.GetDeserializeType(), exception2), exception2));
            }
            return obj3;
        }

        internal static string TryAddLineInfo(XmlReaderDelegator reader, string errorMessage)
        {
            if (reader.HasLineInfo())
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { System.Runtime.Serialization.SR.GetString("ErrorInLine", new object[] { reader.LineNumber, reader.LinePosition }), errorMessage });
            }
            return errorMessage;
        }

        public abstract void WriteEndObject(XmlDictionaryWriter writer);
        public virtual void WriteEndObject(XmlWriter writer)
        {
            CheckNull(writer, "writer");
            this.WriteEndObject(XmlDictionaryWriter.CreateDictionaryWriter(writer));
        }

        internal void WriteEndObjectHandleExceptions(XmlWriterDelegator writer)
        {
            try
            {
                CheckNull(writer, "writer");
                this.InternalWriteEndObject(writer);
            }
            catch (XmlException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorWriteEndObject", null, exception), exception));
            }
            catch (FormatException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorWriteEndObject", null, exception2), exception2));
            }
        }

        internal static void WriteNull(XmlWriterDelegator writer)
        {
            writer.WriteAttributeBool("i", DictionaryGlobals.XsiNilLocalName, DictionaryGlobals.SchemaInstanceNamespace, true);
        }

        public virtual void WriteObject(Stream stream, object graph)
        {
            CheckNull(stream, "stream");
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false);
            this.WriteObject(writer, graph);
            writer.Flush();
        }

        public virtual void WriteObject(XmlDictionaryWriter writer, object graph)
        {
            this.WriteObjectHandleExceptions(new XmlWriterDelegator(writer), graph);
        }

        public virtual void WriteObject(XmlWriter writer, object graph)
        {
            CheckNull(writer, "writer");
            this.WriteObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
        }

        public abstract void WriteObjectContent(XmlDictionaryWriter writer, object graph);
        public virtual void WriteObjectContent(XmlWriter writer, object graph)
        {
            CheckNull(writer, "writer");
            this.WriteObjectContent(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
        }

        internal void WriteObjectContentHandleExceptions(XmlWriterDelegator writer, object graph)
        {
            try
            {
                CheckNull(writer, "writer");
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.WriteObjectContentBegin, System.Runtime.Serialization.SR.GetString("TraceCodeWriteObjectContentBegin"), new StringTraceRecord("Type", GetTypeInfo(this.GetSerializeType(graph))));
                    if (writer.WriteState != WriteState.Element)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(System.Runtime.Serialization.SR.GetString("XmlWriterMustBeInElement", new object[] { writer.WriteState })));
                    }
                    this.InternalWriteObjectContent(writer, graph);
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.WriteObjectContentEnd, System.Runtime.Serialization.SR.GetString("TraceCodeWriteObjectContentEnd"), new StringTraceRecord("Type", GetTypeInfo(this.GetSerializeType(graph))));
                }
                else
                {
                    if (writer.WriteState != WriteState.Element)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(System.Runtime.Serialization.SR.GetString("XmlWriterMustBeInElement", new object[] { writer.WriteState })));
                    }
                    this.InternalWriteObjectContent(writer, graph);
                }
            }
            catch (XmlException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorSerializing", this.GetSerializeType(graph), exception), exception));
            }
            catch (FormatException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorSerializing", this.GetSerializeType(graph), exception2), exception2));
            }
        }

        internal void WriteObjectHandleExceptions(XmlWriterDelegator writer, object graph)
        {
            try
            {
                CheckNull(writer, "writer");
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.WriteObjectBegin, System.Runtime.Serialization.SR.GetString("TraceCodeWriteObjectBegin"), new StringTraceRecord("Type", GetTypeInfo(this.GetSerializeType(graph))));
                    this.InternalWriteObject(writer, graph);
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.WriteObjectEnd, System.Runtime.Serialization.SR.GetString("TraceCodeWriteObjectEnd"), new StringTraceRecord("Type", GetTypeInfo(this.GetSerializeType(graph))));
                }
                else
                {
                    this.InternalWriteObject(writer, graph);
                }
            }
            catch (XmlException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorSerializing", this.GetSerializeType(graph), exception), exception));
            }
            catch (FormatException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorSerializing", this.GetSerializeType(graph), exception2), exception2));
            }
        }

        internal void WriteRootElement(XmlWriterDelegator writer, DataContract contract, XmlDictionaryString name, XmlDictionaryString ns, bool needsContractNsAtRoot)
        {
            if (name == null)
            {
                if (contract.HasRoot)
                {
                    contract.WriteRootElement(writer, contract.TopLevelElementName, contract.TopLevelElementNamespace);
                }
            }
            else
            {
                contract.WriteRootElement(writer, name, ns);
                if (needsContractNsAtRoot)
                {
                    writer.WriteNamespaceDecl(contract.Namespace);
                }
            }
        }

        public abstract void WriteStartObject(XmlDictionaryWriter writer, object graph);
        public virtual void WriteStartObject(XmlWriter writer, object graph)
        {
            CheckNull(writer, "writer");
            this.WriteStartObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
        }

        internal void WriteStartObjectHandleExceptions(XmlWriterDelegator writer, object graph)
        {
            try
            {
                CheckNull(writer, "writer");
                this.InternalWriteStartObject(writer, graph);
            }
            catch (XmlException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorWriteStartObject", this.GetSerializeType(graph), exception), exception));
            }
            catch (FormatException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateSerializationException(GetTypeInfoError("ErrorWriteStartObject", this.GetSerializeType(graph), exception2), exception2));
            }
        }

        internal static bool WriteTypeInfo(XmlWriterDelegator writer, DataContract contract, DataContract declaredContract)
        {
            if ((!object.ReferenceEquals(contract.Name, declaredContract.Name) || !object.ReferenceEquals(contract.Namespace, declaredContract.Namespace)) && ((contract.Name.Value != declaredContract.Name.Value) || (contract.Namespace.Value != declaredContract.Namespace.Value)))
            {
                writer.WriteAttributeQualifiedName("i", DictionaryGlobals.XsiTypeLocalName, DictionaryGlobals.SchemaInstanceNamespace, contract.Name, contract.Namespace);
                return true;
            }
            return false;
        }

        internal static IFormatterConverter FormatterConverter
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (formatterConverter == null)
                {
                    formatterConverter = new System.Runtime.Serialization.FormatterConverter();
                }
                return formatterConverter;
            }
        }

        internal virtual Dictionary<XmlQualifiedName, DataContract> KnownDataContractDictionary =>
            null;
    }
}


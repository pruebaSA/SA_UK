﻿namespace System.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;

    internal class XmlObjectSerializerWriteContext : XmlObjectSerializerContext
    {
        private ObjectReferenceStack byValObjectsInScope;
        private const int depthToCheckCyclicReference = 0x200;
        private bool isGetOnlyCollection;
        protected bool preserveObjectReferences;
        private ObjectToIdCache serializedObjects;
        private XmlSerializableWriter xmlSerializableWriter;

        protected XmlObjectSerializerWriteContext(NetDataContractSerializer serializer) : base(serializer)
        {
            this.byValObjectsInScope = new ObjectReferenceStack();
        }

        protected XmlObjectSerializerWriteContext(DataContractSerializer serializer, DataContract rootTypeDataContract) : base(serializer, rootTypeDataContract)
        {
            this.byValObjectsInScope = new ObjectReferenceStack();
        }

        internal XmlObjectSerializerWriteContext(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject) : base(serializer, maxItemsInObjectGraph, streamingContext, ignoreExtensionDataObject)
        {
            this.byValObjectsInScope = new ObjectReferenceStack();
        }

        internal static XmlObjectSerializerWriteContext CreateContext(DataContractSerializer serializer, DataContract rootTypeDataContract)
        {
            if (!serializer.PreserveObjectReferences && (serializer.DataContractSurrogate == null))
            {
                return new XmlObjectSerializerWriteContext(serializer, rootTypeDataContract);
            }
            return new XmlObjectSerializerWriteContextComplex(serializer, rootTypeDataContract);
        }

        internal static XmlObjectSerializerWriteContext CreateContext(NetDataContractSerializer serializer, Hashtable surrogateDataContracts) => 
            new XmlObjectSerializerWriteContextComplex(serializer, surrogateDataContracts);

        internal static T GetDefaultValue<T>() => 
            default(T);

        internal static bool GetHasValue<T>(T? value) where T: struct => 
            value.HasValue;

        internal static T GetNullableValue<T>(T? value) where T: struct => 
            value.Value;

        internal void HandleGraphAtTopLevel(XmlWriterDelegator writer, object obj, DataContract contract)
        {
            writer.WriteXmlnsAttribute("i", DictionaryGlobals.SchemaInstanceNamespace);
            if (contract.IsISerializable)
            {
                writer.WriteXmlnsAttribute("x", DictionaryGlobals.SchemaNamespace);
            }
            this.OnHandleReference(writer, obj, true);
        }

        internal void IncrementArrayCount(XmlWriterDelegator xmlWriter, Array array)
        {
            this.IncrementCollectionCount(xmlWriter, array.GetLength(0));
        }

        internal void IncrementCollectionCount(XmlWriterDelegator xmlWriter, ICollection collection)
        {
            this.IncrementCollectionCount(xmlWriter, collection.Count);
        }

        private void IncrementCollectionCount(XmlWriterDelegator xmlWriter, int size)
        {
            base.IncrementItemCount(size);
            this.WriteArraySize(xmlWriter, size);
        }

        internal void IncrementCollectionCountGeneric<T>(XmlWriterDelegator xmlWriter, ICollection<T> collection)
        {
            this.IncrementCollectionCount(xmlWriter, collection.Count);
        }

        internal virtual void InternalSerialize(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
        {
            if (writeXsiType)
            {
                Type typeOfObject = Globals.TypeOfObject;
                this.SerializeWithXsiType(xmlWriter, obj, Type.GetTypeHandle(obj), null, -1, typeOfObject.TypeHandle, typeOfObject);
            }
            else if (isDeclaredType)
            {
                DataContract dataContract = this.GetDataContract(declaredTypeID, declaredTypeHandle);
                this.SerializeWithoutXsiType(dataContract, xmlWriter, obj, declaredTypeHandle);
            }
            else
            {
                RuntimeTypeHandle typeHandle = Type.GetTypeHandle(obj);
                if (declaredTypeHandle.Equals(typeHandle))
                {
                    DataContract contract2 = (declaredTypeID >= 0) ? this.GetDataContract(declaredTypeID, declaredTypeHandle) : this.GetDataContract(declaredTypeHandle, null);
                    this.SerializeWithoutXsiType(contract2, xmlWriter, obj, declaredTypeHandle);
                }
                else
                {
                    this.SerializeWithXsiType(xmlWriter, obj, typeHandle, null, declaredTypeID, declaredTypeHandle, Type.GetTypeFromHandle(declaredTypeHandle));
                }
            }
        }

        internal void InternalSerializeReference(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
        {
            if (!this.OnHandleReference(xmlWriter, obj, true))
            {
                this.InternalSerialize(xmlWriter, obj, isDeclaredType, writeXsiType, declaredTypeID, declaredTypeHandle);
            }
            this.OnEndHandleReference(xmlWriter, obj, true);
        }

        internal virtual void OnEndHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
        {
            if ((xmlWriter.depth >= 0x200) && canContainCyclicReference)
            {
                this.byValObjectsInScope.Pop(obj);
            }
        }

        internal bool OnHandleIsReference(XmlWriterDelegator xmlWriter, DataContract contract, object obj)
        {
            if ((this.preserveObjectReferences || !contract.IsReference) || this.isGetOnlyCollection)
            {
                return false;
            }
            bool newId = true;
            int id = this.SerializedObjects.GetId(obj, ref newId);
            this.byValObjectsInScope.EnsureSetAsIsReference(obj);
            if (newId)
            {
                xmlWriter.WriteAttributeString("z", DictionaryGlobals.IdLocalName, DictionaryGlobals.SerializationNamespace, string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[] { "i", id }));
                return false;
            }
            xmlWriter.WriteAttributeString("z", DictionaryGlobals.RefLocalName, DictionaryGlobals.SerializationNamespace, string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[] { "i", id }));
            return true;
        }

        internal virtual bool OnHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
        {
            if ((xmlWriter.depth >= 0x200) && canContainCyclicReference)
            {
                if ((this.byValObjectsInScope.Count == 0) && DiagnosticUtility.ShouldTraceWarning)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.ObjectWithLargeDepth, System.Runtime.Serialization.SR.GetString("TraceCodeObjectWithLargeDepth"));
                }
                if (this.byValObjectsInScope.Contains(obj))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("CannotSerializeObjectWithCycles", new object[] { DataContract.GetClrTypeFullName(obj.GetType()) })));
                }
                this.byValObjectsInScope.Push(obj);
            }
            return false;
        }

        protected void SerializeAndVerifyType(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, bool verifyKnownType, RuntimeTypeHandle declaredTypeHandle)
        {
            bool flag = false;
            if (dataContract.KnownDataContracts != null)
            {
                this.scopedKnownTypes.Push(dataContract.KnownDataContracts);
                flag = true;
            }
            if (verifyKnownType)
            {
                DataContract contract = base.ResolveDataContractFromKnownTypes(dataContract.StableName.Name, dataContract.StableName.Namespace, null);
                if ((contract == null) || (contract.UnderlyingType != dataContract.UnderlyingType))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("DcTypeNotFoundOnSerialize", new object[] { DataContract.GetClrTypeFullName(dataContract.UnderlyingType), dataContract.StableName.Name, dataContract.StableName.Namespace })));
                }
            }
            this.WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);
            if (flag)
            {
                this.scopedKnownTypes.Pop();
            }
        }

        internal void SerializeWithoutXsiType(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle declaredTypeHandle)
        {
            if (!this.OnHandleIsReference(xmlWriter, dataContract, obj))
            {
                if (dataContract.KnownDataContracts != null)
                {
                    this.scopedKnownTypes.Push(dataContract.KnownDataContracts);
                    this.WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);
                    this.scopedKnownTypes.Pop();
                }
                else
                {
                    this.WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);
                }
            }
        }

        protected virtual void SerializeWithXsiType(XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle objectTypeHandle, Type objectType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle, Type declaredType)
        {
            DataContract validContract;
            bool verifyKnownType = false;
            if (declaredType.IsInterface && CollectionDataContract.IsCollectionInterface(declaredType))
            {
                validContract = this.GetDataContractSkipValidation(DataContract.GetId(objectTypeHandle), objectTypeHandle, objectType);
                if (this.OnHandleIsReference(xmlWriter, validContract, obj))
                {
                    return;
                }
                if ((this.Mode == SerializationMode.SharedType) && validContract.IsValidContract(this.Mode))
                {
                    validContract = validContract.GetValidContract(this.Mode);
                }
                else
                {
                    validContract = this.GetDataContract(declaredTypeHandle, declaredType);
                }
                this.WriteClrTypeInfo(xmlWriter, validContract);
            }
            else if (declaredType.IsArray)
            {
                validContract = this.GetDataContract(objectTypeHandle, objectType);
                this.WriteClrTypeInfo(xmlWriter, validContract);
                validContract = this.GetDataContract(declaredTypeHandle, declaredType);
            }
            else
            {
                validContract = this.GetDataContract(objectTypeHandle, objectType);
                if (this.OnHandleIsReference(xmlWriter, validContract, obj))
                {
                    return;
                }
                if (!this.WriteClrTypeInfo(xmlWriter, validContract))
                {
                    DataContract declaredContract = (declaredTypeID >= 0) ? this.GetDataContract(declaredTypeID, declaredTypeHandle) : this.GetDataContract(declaredTypeHandle, declaredType);
                    verifyKnownType = this.WriteTypeInfo(xmlWriter, validContract, declaredContract);
                }
            }
            this.SerializeAndVerifyType(validContract, xmlWriter, obj, verifyKnownType, declaredTypeHandle);
        }

        internal virtual void SerializeWithXsiTypeAtTopLevel(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle originalDeclaredTypeHandle)
        {
            bool verifyKnownType = false;
            Type underlyingType = base.rootTypeDataContract.UnderlyingType;
            if ((!underlyingType.IsInterface || !CollectionDataContract.IsCollectionInterface(underlyingType)) && !underlyingType.IsArray)
            {
                verifyKnownType = this.WriteTypeInfo(xmlWriter, dataContract, base.rootTypeDataContract);
            }
            this.SerializeAndVerifyType(dataContract, xmlWriter, obj, verifyKnownType, originalDeclaredTypeHandle);
        }

        internal void StoreIsGetOnlyCollection()
        {
            this.isGetOnlyCollection = true;
        }

        internal static void ThrowRequiredMemberMustBeEmitted(string memberName, Type type)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.Runtime.Serialization.SR.GetString("RequiredMemberMustBeEmitted", new object[] { memberName, type.FullName })));
        }

        internal bool TryWriteDeserializedExtensionData(XmlWriterDelegator xmlWriter, IDataNode dataNode)
        {
            object obj2 = dataNode.Value;
            if (obj2 == null)
            {
                return false;
            }
            Type type = (dataNode.DataContractName == null) ? obj2.GetType() : Globals.TypeOfObject;
            this.InternalSerialize(xmlWriter, obj2, false, false, -1, type.TypeHandle);
            return true;
        }

        internal virtual void WriteAnyType(XmlWriterDelegator xmlWriter, object value)
        {
            xmlWriter.WriteAnyType(value);
        }

        internal virtual void WriteArraySize(XmlWriterDelegator xmlWriter, int size)
        {
        }

        internal virtual void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value)
        {
            xmlWriter.WriteBase64(value);
        }

        internal virtual void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value, XmlDictionaryString name, XmlDictionaryString ns)
        {
            if (value == null)
            {
                this.WriteNull(xmlWriter, typeof(byte[]), true, name, ns);
            }
            else
            {
                xmlWriter.WriteStartElementPrimitive(name, ns);
                xmlWriter.WriteBase64(value);
                xmlWriter.WriteEndElementPrimitive();
            }
        }

        internal virtual bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, DataContract dataContract) => 
            false;

        internal virtual bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, string clrTypeName, string clrAssemblyName) => 
            false;

        protected virtual void WriteDataContractValue(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle declaredTypeHandle)
        {
            dataContract.WriteXmlValue(xmlWriter, obj, this);
        }

        private void WriteExtensionClassData(XmlWriterDelegator xmlWriter, ClassDataNode dataNode)
        {
            if (!this.TryWriteDeserializedExtensionData(xmlWriter, dataNode))
            {
                this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
                IList<ExtensionDataMember> members = dataNode.Members;
                if (members != null)
                {
                    for (int i = 0; i < members.Count; i++)
                    {
                        this.WriteExtensionDataMember(xmlWriter, members[i]);
                    }
                }
            }
        }

        private void WriteExtensionCollectionData(XmlWriterDelegator xmlWriter, CollectionDataNode dataNode)
        {
            if (!this.TryWriteDeserializedExtensionData(xmlWriter, dataNode))
            {
                this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
                this.WriteArraySize(xmlWriter, dataNode.Size);
                IList<IDataNode> items = dataNode.Items;
                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        xmlWriter.WriteStartElement(dataNode.ItemName, dataNode.ItemNamespace);
                        this.WriteExtensionDataValue(xmlWriter, items[i]);
                        xmlWriter.WriteEndElement();
                    }
                }
            }
        }

        internal void WriteExtensionData(XmlWriterDelegator xmlWriter, ExtensionDataObject extensionData, int memberIndex)
        {
            if ((!base.IgnoreExtensionDataObject && (extensionData != null)) && (extensionData.Members != null))
            {
                for (int i = 0; i < extensionData.Members.Count; i++)
                {
                    ExtensionDataMember member = extensionData.Members[i];
                    if (member.MemberIndex == memberIndex)
                    {
                        this.WriteExtensionDataMember(xmlWriter, member);
                    }
                }
            }
        }

        private void WriteExtensionDataMember(XmlWriterDelegator xmlWriter, ExtensionDataMember member)
        {
            xmlWriter.WriteStartElement(member.Name, member.Namespace);
            IDataNode dataNode = member.Value;
            this.WriteExtensionDataValue(xmlWriter, dataNode);
            xmlWriter.WriteEndElement();
        }

        internal virtual void WriteExtensionDataTypeInfo(XmlWriterDelegator xmlWriter, IDataNode dataNode)
        {
            if (dataNode.DataContractName != null)
            {
                this.WriteTypeInfo(xmlWriter, dataNode.DataContractName, dataNode.DataContractNamespace);
            }
            this.WriteClrTypeInfo(xmlWriter, dataNode.ClrTypeName, dataNode.ClrAssemblyName);
        }

        internal void WriteExtensionDataValue(XmlWriterDelegator xmlWriter, IDataNode dataNode)
        {
            base.IncrementItemCount(1);
            if (dataNode == null)
            {
                this.WriteNull(xmlWriter);
            }
            else if (!dataNode.PreservesReferences || !this.OnHandleReference(xmlWriter, (dataNode.Value == null) ? dataNode : dataNode.Value, true))
            {
                Type dataType = dataNode.DataType;
                if (dataType == Globals.TypeOfClassDataNode)
                {
                    this.WriteExtensionClassData(xmlWriter, (ClassDataNode) dataNode);
                }
                else if (dataType == Globals.TypeOfCollectionDataNode)
                {
                    this.WriteExtensionCollectionData(xmlWriter, (CollectionDataNode) dataNode);
                }
                else if (dataType == Globals.TypeOfXmlDataNode)
                {
                    this.WriteExtensionXmlData(xmlWriter, (XmlDataNode) dataNode);
                }
                else if (dataType == Globals.TypeOfISerializableDataNode)
                {
                    this.WriteExtensionISerializableData(xmlWriter, (ISerializableDataNode) dataNode);
                }
                else
                {
                    this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
                    if (dataType == Globals.TypeOfObject)
                    {
                        object obj2 = dataNode.Value;
                        if (obj2 != null)
                        {
                            this.InternalSerialize(xmlWriter, obj2, false, false, -1, obj2.GetType().TypeHandle);
                        }
                    }
                    else
                    {
                        xmlWriter.WriteExtensionData(dataNode);
                    }
                }
                if (dataNode.PreservesReferences)
                {
                    this.OnEndHandleReference(xmlWriter, (dataNode.Value == null) ? dataNode : dataNode.Value, true);
                }
            }
        }

        private void WriteExtensionISerializableData(XmlWriterDelegator xmlWriter, ISerializableDataNode dataNode)
        {
            if (!this.TryWriteDeserializedExtensionData(xmlWriter, dataNode))
            {
                this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
                if (dataNode.FactoryTypeName != null)
                {
                    xmlWriter.WriteAttributeQualifiedName("z", DictionaryGlobals.ISerializableFactoryTypeLocalName, DictionaryGlobals.SerializationNamespace, dataNode.FactoryTypeName, dataNode.FactoryTypeNamespace);
                }
                IList<ISerializableDataMember> members = dataNode.Members;
                if (members != null)
                {
                    for (int i = 0; i < members.Count; i++)
                    {
                        ISerializableDataMember member = members[i];
                        xmlWriter.WriteStartElement(member.Name, string.Empty);
                        this.WriteExtensionDataValue(xmlWriter, member.Value);
                        xmlWriter.WriteEndElement();
                    }
                }
            }
        }

        private void WriteExtensionXmlData(XmlWriterDelegator xmlWriter, XmlDataNode dataNode)
        {
            if (!this.TryWriteDeserializedExtensionData(xmlWriter, dataNode))
            {
                IList<System.Xml.XmlAttribute> xmlAttributes = dataNode.XmlAttributes;
                if (xmlAttributes != null)
                {
                    foreach (System.Xml.XmlAttribute attribute in xmlAttributes)
                    {
                        attribute.WriteTo(xmlWriter.Writer);
                    }
                }
                this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
                IList<System.Xml.XmlNode> xmlChildNodes = dataNode.XmlChildNodes;
                if (xmlChildNodes != null)
                {
                    foreach (System.Xml.XmlNode node in xmlChildNodes)
                    {
                        node.WriteTo(xmlWriter.Writer);
                    }
                }
            }
        }

        internal void WriteISerializable(XmlWriterDelegator xmlWriter, ISerializable obj)
        {
            Type type = obj.GetType();
            SerializationInfo info = new SerializationInfo(type, XmlObjectSerializer.FormatterConverter);
            obj.GetObjectData(info, base.GetStreamingContext());
            this.WriteSerializationInfo(xmlWriter, type, info);
        }

        internal void WriteIXmlSerializable(XmlWriterDelegator xmlWriter, object obj)
        {
            if (this.xmlSerializableWriter == null)
            {
                this.xmlSerializableWriter = new XmlSerializableWriter();
            }
            WriteIXmlSerializable(xmlWriter, obj, this.xmlSerializableWriter);
        }

        private static void WriteIXmlSerializable(XmlWriterDelegator xmlWriter, object obj, XmlSerializableWriter xmlSerializableWriter)
        {
            xmlSerializableWriter.BeginWrite(xmlWriter.Writer, obj);
            IXmlSerializable serializable = obj as IXmlSerializable;
            if (serializable != null)
            {
                serializable.WriteXml(xmlSerializableWriter);
            }
            else
            {
                XmlElement element = obj as XmlElement;
                if (element != null)
                {
                    element.WriteTo(xmlSerializableWriter);
                }
                else
                {
                    System.Xml.XmlNode[] nodeArray = obj as System.Xml.XmlNode[];
                    if (nodeArray == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("UnknownXmlType", new object[] { DataContract.GetClrTypeFullName(obj.GetType()) })));
                    }
                    foreach (System.Xml.XmlNode node in nodeArray)
                    {
                        node.WriteTo(xmlSerializableWriter);
                    }
                }
            }
            xmlSerializableWriter.EndWrite();
        }

        protected virtual void WriteNull(XmlWriterDelegator xmlWriter)
        {
            XmlObjectSerializer.WriteNull(xmlWriter);
        }

        internal void WriteNull(XmlWriterDelegator xmlWriter, Type memberType, bool isMemberTypeSerializable)
        {
            this.CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
            this.WriteNull(xmlWriter);
        }

        internal void WriteNull(XmlWriterDelegator xmlWriter, Type memberType, bool isMemberTypeSerializable, XmlDictionaryString name, XmlDictionaryString ns)
        {
            xmlWriter.WriteStartElement(name, ns);
            this.WriteNull(xmlWriter, memberType, isMemberTypeSerializable);
            xmlWriter.WriteEndElement();
        }

        internal virtual void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value)
        {
            xmlWriter.WriteQName(value);
        }

        internal virtual void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value, XmlDictionaryString name, XmlDictionaryString ns)
        {
            if (value == null)
            {
                this.WriteNull(xmlWriter, typeof(XmlQualifiedName), true, name, ns);
            }
            else
            {
                if (((ns != null) && (ns.Value != null)) && (ns.Value.Length > 0))
                {
                    xmlWriter.WriteStartElement("q", name, ns);
                }
                else
                {
                    xmlWriter.WriteStartElement(name, ns);
                }
                xmlWriter.WriteQName(value);
                xmlWriter.WriteEndElement();
            }
        }

        internal static void WriteRootIXmlSerializable(XmlWriterDelegator xmlWriter, object obj)
        {
            WriteIXmlSerializable(xmlWriter, obj, new XmlSerializableWriter());
        }

        internal void WriteSerializationInfo(XmlWriterDelegator xmlWriter, Type objType, SerializationInfo serInfo)
        {
            if (DataContract.GetClrTypeFullName(objType) != serInfo.FullTypeName)
            {
                string str;
                string str2;
                DataContract.GetDefaultStableName(serInfo.FullTypeName, out str, out str2);
                xmlWriter.WriteAttributeQualifiedName("z", DictionaryGlobals.ISerializableFactoryTypeLocalName, DictionaryGlobals.SerializationNamespace, DataContract.GetClrTypeString(str), DataContract.GetClrTypeString(str2));
            }
            this.WriteClrTypeInfo(xmlWriter, serInfo.FullTypeName, serInfo.AssemblyName);
            base.IncrementItemCount(serInfo.MemberCount);
            SerializationInfoEnumerator enumerator = serInfo.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SerializationEntry current = enumerator.Current;
                XmlDictionaryString clrTypeString = DataContract.GetClrTypeString(DataContract.EncodeLocalName(current.Name));
                xmlWriter.WriteStartElement(clrTypeString, DictionaryGlobals.EmptyString);
                object obj2 = current.Value;
                if (obj2 == null)
                {
                    this.WriteNull(xmlWriter);
                }
                else
                {
                    this.InternalSerializeReference(xmlWriter, obj2, false, false, -1, Globals.TypeOfObject.TypeHandle);
                }
                xmlWriter.WriteEndElement();
            }
        }

        internal virtual void WriteString(XmlWriterDelegator xmlWriter, string value)
        {
            xmlWriter.WriteString(value);
        }

        internal virtual void WriteString(XmlWriterDelegator xmlWriter, string value, XmlDictionaryString name, XmlDictionaryString ns)
        {
            if (value == null)
            {
                this.WriteNull(xmlWriter, typeof(string), true, name, ns);
            }
            else
            {
                xmlWriter.WriteStartElementPrimitive(name, ns);
                xmlWriter.WriteString(value);
                xmlWriter.WriteEndElementPrimitive();
            }
        }

        protected virtual bool WriteTypeInfo(XmlWriterDelegator writer, DataContract contract, DataContract declaredContract) => 
            XmlObjectSerializer.WriteTypeInfo(writer, contract, declaredContract);

        protected virtual void WriteTypeInfo(XmlWriterDelegator writer, string dataContractName, string dataContractNamespace)
        {
            writer.WriteAttributeQualifiedName("i", DictionaryGlobals.XsiTypeLocalName, DictionaryGlobals.SchemaInstanceNamespace, dataContractName, dataContractNamespace);
        }

        internal virtual void WriteUri(XmlWriterDelegator xmlWriter, Uri value)
        {
            xmlWriter.WriteUri(value);
        }

        internal virtual void WriteUri(XmlWriterDelegator xmlWriter, Uri value, XmlDictionaryString name, XmlDictionaryString ns)
        {
            if (value == null)
            {
                this.WriteNull(xmlWriter, typeof(Uri), true, name, ns);
            }
            else
            {
                xmlWriter.WriteStartElementPrimitive(name, ns);
                xmlWriter.WriteUri(value);
                xmlWriter.WriteEndElementPrimitive();
            }
        }

        internal override bool IsGetOnlyCollection
        {
            get => 
                this.isGetOnlyCollection;
            set
            {
                this.isGetOnlyCollection = value;
            }
        }

        protected ObjectToIdCache SerializedObjects
        {
            get
            {
                if (this.serializedObjects == null)
                {
                    this.serializedObjects = new ObjectToIdCache();
                }
                return this.serializedObjects;
            }
        }
    }
}


namespace System.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters;

    internal class XmlObjectSerializerReadContextComplex : XmlObjectSerializerReadContext
    {
        private FormatterAssemblyStyle assemblyFormat;
        private SerializationBinder binder;
        protected IDataContractSurrogate dataContractSurrogate;
        private Hashtable dataContractTypeCache;
        private SerializationMode mode;
        private bool preserveObjectReferences;
        private Hashtable surrogateDataContracts;
        private ISurrogateSelector surrogateSelector;

        internal XmlObjectSerializerReadContextComplex(NetDataContractSerializer serializer) : base(serializer)
        {
            this.dataContractTypeCache = new Hashtable();
            this.mode = SerializationMode.SharedType;
            this.preserveObjectReferences = true;
            this.binder = serializer.Binder;
            this.surrogateSelector = serializer.SurrogateSelector;
            this.assemblyFormat = serializer.AssemblyFormat;
        }

        internal XmlObjectSerializerReadContextComplex(DataContractSerializer serializer, DataContract rootTypeDataContract) : base(serializer, rootTypeDataContract)
        {
            this.dataContractTypeCache = new Hashtable();
            this.mode = SerializationMode.SharedContract;
            this.preserveObjectReferences = serializer.PreserveObjectReferences;
            this.dataContractSurrogate = serializer.DataContractSurrogate;
        }

        internal XmlObjectSerializerReadContextComplex(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject) : base(serializer, maxItemsInObjectGraph, streamingContext, ignoreExtensionDataObject)
        {
            this.dataContractTypeCache = new Hashtable();
        }

        internal override void CheckIfTypeSerializable(Type memberType, bool isMemberTypeSerializable)
        {
            if (this.mode == SerializationMode.SharedType)
            {
                ISurrogateSelector selector;
                if ((this.surrogateSelector != null) && (this.surrogateSelector.GetSurrogate(memberType, base.GetStreamingContext(), out selector) != null))
                {
                    return;
                }
            }
            else if (this.dataContractSurrogate != null)
            {
                while (memberType.IsArray)
                {
                    memberType = memberType.GetElementType();
                }
                memberType = DataContractSurrogateCaller.GetDataContractType(this.dataContractSurrogate, memberType);
                if (!DataContract.IsTypeSerializable(memberType))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("TypeNotSerializable", new object[] { memberType })));
                }
                return;
            }
            base.CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
        }

        internal override int GetArraySize()
        {
            if (!this.preserveObjectReferences)
            {
                return -1;
            }
            return base.attributes.ArraySZSize;
        }

        internal override DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle)
        {
            DataContract contract = NetDataContractSerializer.GetDataContractFromSurrogateSelector(this.surrogateSelector, base.GetStreamingContext(), typeHandle, null, ref this.surrogateDataContracts);
            if (contract == null)
            {
                return base.GetDataContract(id, typeHandle);
            }
            if (this.IsGetOnlyCollection && (contract is SurrogateDataContract))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("SurrogatesWithGetOnlyCollectionsNotSupportedSerDeser", new object[] { DataContract.GetClrTypeFullName(contract.UnderlyingType) })));
            }
            return contract;
        }

        internal override DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
        {
            DataContract contract = NetDataContractSerializer.GetDataContractFromSurrogateSelector(this.surrogateSelector, base.GetStreamingContext(), typeHandle, type, ref this.surrogateDataContracts);
            if (contract == null)
            {
                return base.GetDataContract(typeHandle, type);
            }
            if (this.IsGetOnlyCollection && (contract is SurrogateDataContract))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("SurrogatesWithGetOnlyCollectionsNotSupportedSerDeser", new object[] { DataContract.GetClrTypeFullName(contract.UnderlyingType) })));
            }
            return contract;
        }

        internal override Type GetSurrogatedType(Type type)
        {
            if (this.dataContractSurrogate == null)
            {
                return base.GetSurrogatedType(type);
            }
            type = DataContract.UnwrapNullableType(type);
            Type surrogatedType = DataContractSerializer.GetSurrogatedType(this.dataContractSurrogate, type);
            if (this.IsGetOnlyCollection && (surrogatedType != type))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("SurrogatesWithGetOnlyCollectionsNotSupportedSerDeser", new object[] { DataContract.GetClrTypeFullName(type) })));
            }
            return surrogatedType;
        }

        internal override object InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, string name, string ns)
        {
            if (this.mode != SerializationMode.SharedContract)
            {
                return this.InternalDeserializeInSharedTypeMode(xmlReader, -1, declaredType, name, ns);
            }
            if (this.dataContractSurrogate == null)
            {
                return base.InternalDeserialize(xmlReader, declaredType, name, ns);
            }
            return this.InternalDeserializeWithSurrogate(xmlReader, declaredType, null, name, ns);
        }

        internal override object InternalDeserialize(XmlReaderDelegator xmlReader, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle, string name, string ns)
        {
            if (this.mode != SerializationMode.SharedContract)
            {
                return this.InternalDeserializeInSharedTypeMode(xmlReader, declaredTypeID, Type.GetTypeFromHandle(declaredTypeHandle), name, ns);
            }
            if (this.dataContractSurrogate == null)
            {
                return base.InternalDeserialize(xmlReader, declaredTypeID, declaredTypeHandle, name, ns);
            }
            return this.InternalDeserializeWithSurrogate(xmlReader, Type.GetTypeFromHandle(declaredTypeHandle), null, name, ns);
        }

        internal override object InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, DataContract dataContract, string name, string ns)
        {
            if (this.mode != SerializationMode.SharedContract)
            {
                return this.InternalDeserializeInSharedTypeMode(xmlReader, -1, declaredType, name, ns);
            }
            if (this.dataContractSurrogate == null)
            {
                return base.InternalDeserialize(xmlReader, declaredType, dataContract, name, ns);
            }
            return this.InternalDeserializeWithSurrogate(xmlReader, declaredType, dataContract, name, ns);
        }

        private object InternalDeserializeInSharedTypeMode(XmlReaderDelegator xmlReader, int declaredTypeID, Type declaredType, string name, string ns)
        {
            object retObj = null;
            DataContract contract;
            if (base.TryHandleNullOrRef(xmlReader, declaredType, name, ns, ref retObj))
            {
                return retObj;
            }
            string clrAssembly = base.attributes.ClrAssembly;
            string clrType = base.attributes.ClrType;
            if ((clrAssembly != null) && (clrType != null))
            {
                Assembly assembly;
                Type type;
                contract = this.ResolveDataContractInSharedTypeMode(clrAssembly, clrType, out assembly, out type);
                if (contract == null)
                {
                    if (assembly == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("AssemblyNotFound", new object[] { clrAssembly })));
                    }
                    if (type == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("ClrTypeNotFound", new object[] { assembly.FullName, clrType })));
                    }
                }
                if ((declaredType != null) && declaredType.IsArray)
                {
                    contract = (declaredTypeID < 0) ? base.GetDataContract(declaredType) : this.GetDataContract(declaredTypeID, declaredType.TypeHandle);
                }
            }
            else
            {
                if (clrAssembly != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(xmlReader, System.Runtime.Serialization.SR.GetString("AttributeNotFound", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/", "Type", xmlReader.NodeType, xmlReader.NamespaceURI, xmlReader.LocalName }))));
                }
                if (clrType != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(xmlReader, System.Runtime.Serialization.SR.GetString("AttributeNotFound", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/", "Assembly", xmlReader.NodeType, xmlReader.NamespaceURI, xmlReader.LocalName }))));
                }
                if (declaredType == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(xmlReader, System.Runtime.Serialization.SR.GetString("AttributeNotFound", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/", "Type", xmlReader.NodeType, xmlReader.NamespaceURI, xmlReader.LocalName }))));
                }
                contract = (declaredTypeID < 0) ? base.GetDataContract(declaredType) : this.GetDataContract(declaredTypeID, declaredType.TypeHandle);
            }
            return this.ReadDataContractValue(contract, xmlReader);
        }

        private object InternalDeserializeWithSurrogate(XmlReaderDelegator xmlReader, Type declaredType, DataContract surrogateDataContract, string name, string ns)
        {
            DataContract dataContract = surrogateDataContract ?? base.GetDataContract(DataContractSurrogateCaller.GetDataContractType(this.dataContractSurrogate, declaredType));
            if (this.IsGetOnlyCollection && (dataContract.UnderlyingType != declaredType))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("SurrogatesWithGetOnlyCollectionsNotSupportedSerDeser", new object[] { DataContract.GetClrTypeFullName(declaredType) })));
            }
            this.ReadAttributes(xmlReader);
            string objectId = base.GetObjectId();
            object obj2 = base.InternalDeserialize(xmlReader, name, ns, ref dataContract);
            object newObj = DataContractSurrogateCaller.GetDeserializedObject(this.dataContractSurrogate, obj2, dataContract.UnderlyingType, declaredType);
            base.ReplaceDeserializedObject(objectId, obj2, newObj);
            return newObj;
        }

        protected override DataContract ResolveDataContractFromTypeName()
        {
            if (this.mode == SerializationMode.SharedContract)
            {
                return base.ResolveDataContractFromTypeName();
            }
            if ((base.attributes.ClrAssembly != null) && (base.attributes.ClrType != null))
            {
                Assembly assembly;
                Type type;
                return this.ResolveDataContractInSharedTypeMode(base.attributes.ClrAssembly, base.attributes.ClrType, out assembly, out type);
            }
            return null;
        }

        private DataContract ResolveDataContractInSharedTypeMode(string assemblyName, string typeName, out Assembly assembly, out Type type)
        {
            type = this.ResolveDataContractTypeInSharedTypeMode(assemblyName, typeName, out assembly);
            if (type != null)
            {
                return base.GetDataContract(type);
            }
            return null;
        }

        private Type ResolveDataContractTypeInSharedTypeMode(string assemblyName, string typeName, out Assembly assembly)
        {
            assembly = null;
            Type type = null;
            if (this.binder != null)
            {
                type = this.binder.BindToType(assemblyName, typeName);
            }
            if (type != null)
            {
                return type;
            }
            XmlObjectDataContractTypeKey key = new XmlObjectDataContractTypeKey(assemblyName, typeName);
            XmlObjectDataContractTypeInfo info = (XmlObjectDataContractTypeInfo) this.dataContractTypeCache[key];
            if (info == null)
            {
                if (assemblyName == "0")
                {
                    assembly = Globals.TypeOfInt.Assembly;
                }
                else if (this.assemblyFormat == FormatterAssemblyStyle.Full)
                {
                    assembly = Assembly.Load(assemblyName);
                }
                else
                {
                    assembly = Assembly.LoadWithPartialName(assemblyName);
                    if (assembly == null)
                    {
                        AssemblyName name = new AssemblyName(assemblyName) {
                            Version = null
                        };
                        assembly = Assembly.LoadWithPartialName(name.FullName);
                    }
                }
                if (assembly != null)
                {
                    try
                    {
                        type = assembly.GetType(typeName);
                    }
                    catch (TypeLoadException)
                    {
                        if (this.assemblyFormat == FormatterAssemblyStyle.Full)
                        {
                            throw;
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        if (this.assemblyFormat == FormatterAssemblyStyle.Full)
                        {
                            throw;
                        }
                    }
                    catch (FileLoadException)
                    {
                        if (this.assemblyFormat == FormatterAssemblyStyle.Full)
                        {
                            throw;
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        if (this.assemblyFormat == FormatterAssemblyStyle.Full)
                        {
                            throw;
                        }
                    }
                }
                if (((type == null) && (assembly != null)) && (this.assemblyFormat == FormatterAssemblyStyle.Simple))
                {
                    type = System.Runtime.Serialization.TypeName.GetType(assembly, typeName);
                }
                if (type == null)
                {
                    return type;
                }
                info = new XmlObjectDataContractTypeInfo(assembly, type);
                lock (this.dataContractTypeCache)
                {
                    if (!this.dataContractTypeCache.ContainsKey(key))
                    {
                        this.dataContractTypeCache[key] = info;
                    }
                    return type;
                }
            }
            assembly = info.Assembly;
            return info.Type;
        }

        internal override SerializationMode Mode =>
            this.mode;

        private class XmlObjectDataContractTypeInfo
        {
            private System.Reflection.Assembly assembly;
            private System.Type type;

            public XmlObjectDataContractTypeInfo(System.Reflection.Assembly assembly, System.Type type)
            {
                this.assembly = assembly;
                this.type = type;
            }

            public System.Reflection.Assembly Assembly =>
                this.assembly;

            public System.Type Type =>
                this.type;
        }

        private class XmlObjectDataContractTypeKey
        {
            private string assemblyName;
            private string typeName;

            public XmlObjectDataContractTypeKey(string assemblyName, string typeName)
            {
                this.assemblyName = assemblyName;
                this.typeName = typeName;
            }

            public override bool Equals(object obj)
            {
                if (!object.ReferenceEquals(this, obj))
                {
                    XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeKey key = obj as XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeKey;
                    if (key == null)
                    {
                        return false;
                    }
                    if (this.assemblyName != key.assemblyName)
                    {
                        return false;
                    }
                    if (this.typeName != key.typeName)
                    {
                        return false;
                    }
                }
                return true;
            }

            public override int GetHashCode()
            {
                int hashCode = 0;
                if (this.assemblyName != null)
                {
                    hashCode = this.assemblyName.GetHashCode();
                }
                if (this.typeName != null)
                {
                    hashCode ^= this.typeName.GetHashCode();
                }
                return hashCode;
            }
        }
    }
}


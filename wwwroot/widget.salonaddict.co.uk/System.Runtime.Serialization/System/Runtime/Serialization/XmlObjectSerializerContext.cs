namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Xml;

    internal class XmlObjectSerializerContext
    {
        [SecurityCritical]
        private bool demandedMemberAccessPermission;
        [SecurityCritical]
        private bool demandedSerializationFormatterPermission;
        private bool ignoreExtensionDataObject;
        private bool isSerializerKnownDataContractsSetExplicit;
        private int itemCount;
        private int maxItemsInObjectGraph;
        protected DataContract rootTypeDataContract;
        internal ScopedKnownTypes scopedKnownTypes;
        protected XmlObjectSerializer serializer;
        protected Dictionary<XmlQualifiedName, DataContract> serializerKnownDataContracts;
        protected IList<Type> serializerKnownTypeList;
        private StreamingContext streamingContext;

        internal XmlObjectSerializerContext(NetDataContractSerializer serializer) : this(serializer, serializer.MaxItemsInObjectGraph, serializer.Context, serializer.IgnoreExtensionDataObject)
        {
        }

        internal XmlObjectSerializerContext(DataContractSerializer serializer, DataContract rootTypeDataContract) : this(serializer, serializer.MaxItemsInObjectGraph, new StreamingContext(StreamingContextStates.All), serializer.IgnoreExtensionDataObject)
        {
            this.rootTypeDataContract = rootTypeDataContract;
            this.serializerKnownTypeList = serializer.knownTypeList;
        }

        internal XmlObjectSerializerContext(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject)
        {
            this.scopedKnownTypes = new ScopedKnownTypes();
            this.serializer = serializer;
            this.itemCount = 1;
            this.maxItemsInObjectGraph = maxItemsInObjectGraph;
            this.streamingContext = streamingContext;
            this.ignoreExtensionDataObject = ignoreExtensionDataObject;
        }

        internal virtual void CheckIfTypeSerializable(Type memberType, bool isMemberTypeSerializable)
        {
            if (!isMemberTypeSerializable)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("TypeNotSerializable", new object[] { memberType })));
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public void DemandMemberAccessPermission()
        {
            if (!this.demandedMemberAccessPermission)
            {
                Globals.MemberAccessPermission.Demand();
                this.demandedMemberAccessPermission = true;
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public void DemandSerializationFormatterPermission()
        {
            if (!this.demandedSerializationFormatterPermission)
            {
                Globals.SerializationFormatterPermission.Demand();
                this.demandedSerializationFormatterPermission = true;
            }
        }

        internal DataContract GetDataContract(Type type) => 
            this.GetDataContract(type.TypeHandle, type);

        internal virtual DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle)
        {
            if (this.IsGetOnlyCollection)
            {
                return DataContract.GetGetOnlyCollectionDataContract(id, typeHandle, null, this.Mode);
            }
            return DataContract.GetDataContract(id, typeHandle, this.Mode);
        }

        internal virtual DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
        {
            if (this.IsGetOnlyCollection)
            {
                return DataContract.GetGetOnlyCollectionDataContract(DataContract.GetId(typeHandle), typeHandle, type, this.Mode);
            }
            return DataContract.GetDataContract(typeHandle, type, this.Mode);
        }

        private DataContract GetDataContractFromSerializerKnownTypes(XmlQualifiedName qname)
        {
            DataContract contract;
            Dictionary<XmlQualifiedName, DataContract> serializerKnownDataContracts = this.SerializerKnownDataContracts;
            if (serializerKnownDataContracts == null)
            {
                return null;
            }
            if (!serializerKnownDataContracts.TryGetValue(qname, out contract))
            {
                return null;
            }
            return contract;
        }

        internal static Dictionary<XmlQualifiedName, DataContract> GetDataContractsForKnownTypes(IList<Type> knownTypeList)
        {
            if (knownTypeList == null)
            {
                return null;
            }
            Dictionary<XmlQualifiedName, DataContract> nameToDataContractTable = new Dictionary<XmlQualifiedName, DataContract>();
            Dictionary<Type, Type> typesChecked = new Dictionary<Type, Type>();
            for (int i = 0; i < knownTypeList.Count; i++)
            {
                Type type = knownTypeList[i];
                if (type == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("NullKnownType", new object[] { "knownTypes" })));
                }
                DataContract.CheckAndAdd(type, typesChecked, ref nameToDataContractTable);
            }
            return nameToDataContractTable;
        }

        internal virtual DataContract GetDataContractSkipValidation(int typeId, RuntimeTypeHandle typeHandle, Type type)
        {
            if (this.IsGetOnlyCollection)
            {
                return DataContract.GetGetOnlyCollectionDataContractSkipValidation(typeId, typeHandle, type);
            }
            return DataContract.GetDataContractSkipValidation(typeId, typeHandle, type);
        }

        internal StreamingContext GetStreamingContext() => 
            this.streamingContext;

        internal virtual Type GetSurrogatedType(Type type) => 
            type;

        internal void IncrementItemCount(int count)
        {
            if (count > (this.maxItemsInObjectGraph - this.itemCount))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("ExceededMaxItemsQuota", new object[] { this.maxItemsInObjectGraph })));
            }
            this.itemCount += count;
        }

        protected DataContract ResolveDataContractFromKnownTypes(string typeName, string typeNs, DataContract memberTypeContract)
        {
            DataContract primitiveDataContract = PrimitiveDataContract.GetPrimitiveDataContract(typeName, typeNs);
            if (primitiveDataContract == null)
            {
                DataContract dataContract;
                XmlQualifiedName qname = new XmlQualifiedName(typeName, typeNs);
                primitiveDataContract = this.scopedKnownTypes.GetDataContract(qname);
                if (primitiveDataContract == null)
                {
                    primitiveDataContract = this.GetDataContractFromSerializerKnownTypes(qname);
                }
                if (((primitiveDataContract == null) && (memberTypeContract != null)) && (!memberTypeContract.UnderlyingType.IsInterface && (memberTypeContract.StableName == qname)))
                {
                    primitiveDataContract = memberTypeContract;
                }
                if ((primitiveDataContract != null) || (this.rootTypeDataContract == null))
                {
                    return primitiveDataContract;
                }
                if (this.rootTypeDataContract.StableName == qname)
                {
                    return this.rootTypeDataContract;
                }
                for (CollectionDataContract contract2 = this.rootTypeDataContract as CollectionDataContract; contract2 != null; contract2 = dataContract as CollectionDataContract)
                {
                    dataContract = this.GetDataContract(this.GetSurrogatedType(contract2.ItemType));
                    if (dataContract.StableName == qname)
                    {
                        return dataContract;
                    }
                }
            }
            return primitiveDataContract;
        }

        internal bool IgnoreExtensionDataObject =>
            this.ignoreExtensionDataObject;

        internal virtual bool IsGetOnlyCollection
        {
            get => 
                false;
            set
            {
            }
        }

        internal virtual SerializationMode Mode =>
            SerializationMode.SharedContract;

        internal int RemainingItemCount =>
            (this.maxItemsInObjectGraph - this.itemCount);

        private Dictionary<XmlQualifiedName, DataContract> SerializerKnownDataContracts
        {
            get
            {
                if ((this.serializerKnownDataContracts == null) && !this.isSerializerKnownDataContractsSetExplicit)
                {
                    this.serializerKnownDataContracts = this.serializer.KnownDataContractDictionary;
                    this.isSerializerKnownDataContractsSetExplicit = true;
                }
                return this.serializerKnownDataContracts;
            }
        }
    }
}


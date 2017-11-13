namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    internal class ClientOptions
    {
        private List<ReferencedCollectionType> m_CollectionMappingList;
        private bool m_EnableDataBinding;
        private List<ReferencedType> m_ExcludedTypeList;
        private bool m_GenerateAsynchronousMethods;
        private bool m_GenerateInternalTypes;
        private bool m_GenerateMessageContracts;
        private bool m_GenerateSerializableTypes;
        private bool m_ImportXmlTypes;
        private List<NamespaceMapping> m_NamespaceMappingList;
        private bool m_ReferenceAllAssemblies;
        private List<ReferencedAssembly> m_ReferencedAssemblyList;
        private List<ReferencedType> m_ReferencedDataContractTypeList;
        private ProxySerializerType m_Serializer;
        private List<ContractMapping> m_ServiceContractMappingList;
        private bool m_UseSerializerForFaults;
        private bool m_UseSerializerForFaultsSpecified;
        private bool m_Wrapped;
        private bool m_WrappedSpecified;

        [XmlArrayItem("CollectionMapping", typeof(ReferencedCollectionType)), XmlArray(ElementName="CollectionMappings")]
        public List<ReferencedCollectionType> CollectionMappingList
        {
            get
            {
                if (this.m_CollectionMappingList == null)
                {
                    this.m_CollectionMappingList = new List<ReferencedCollectionType>();
                }
                return this.m_CollectionMappingList;
            }
        }

        [XmlElement]
        public bool EnableDataBinding
        {
            get => 
                this.m_EnableDataBinding;
            set
            {
                this.m_EnableDataBinding = value;
            }
        }

        [XmlArrayItem("ExcludedType", typeof(ReferencedType)), XmlArray(ElementName="ExcludedTypes")]
        public List<ReferencedType> ExcludedTypeList
        {
            get
            {
                if (this.m_ExcludedTypeList == null)
                {
                    this.m_ExcludedTypeList = new List<ReferencedType>();
                }
                return this.m_ExcludedTypeList;
            }
        }

        [XmlElement]
        public bool GenerateAsynchronousMethods
        {
            get => 
                this.m_GenerateAsynchronousMethods;
            set
            {
                this.m_GenerateAsynchronousMethods = value;
            }
        }

        [XmlElement]
        public bool GenerateInternalTypes
        {
            get => 
                this.m_GenerateInternalTypes;
            set
            {
                this.m_GenerateInternalTypes = value;
            }
        }

        [XmlElement]
        public bool GenerateMessageContracts
        {
            get => 
                this.m_GenerateMessageContracts;
            set
            {
                this.m_GenerateMessageContracts = value;
            }
        }

        [XmlElement]
        public bool GenerateSerializableTypes
        {
            get => 
                this.m_GenerateSerializableTypes;
            set
            {
                this.m_GenerateSerializableTypes = value;
            }
        }

        [XmlElement]
        public bool ImportXmlTypes
        {
            get => 
                this.m_ImportXmlTypes;
            set
            {
                this.m_ImportXmlTypes = value;
            }
        }

        [XmlArrayItem("NamespaceMapping", typeof(NamespaceMapping)), XmlArray(ElementName="NamespaceMappings")]
        public List<NamespaceMapping> NamespaceMappingList
        {
            get
            {
                if (this.m_NamespaceMappingList == null)
                {
                    this.m_NamespaceMappingList = new List<NamespaceMapping>();
                }
                return this.m_NamespaceMappingList;
            }
        }

        [XmlElement]
        public bool ReferenceAllAssemblies
        {
            get => 
                this.m_ReferenceAllAssemblies;
            set
            {
                this.m_ReferenceAllAssemblies = value;
            }
        }

        [XmlArray(ElementName="ReferencedAssemblies"), XmlArrayItem("ReferencedAssembly", typeof(ReferencedAssembly))]
        public List<ReferencedAssembly> ReferencedAssemblyList
        {
            get
            {
                if (this.m_ReferencedAssemblyList == null)
                {
                    this.m_ReferencedAssemblyList = new List<ReferencedAssembly>();
                }
                return this.m_ReferencedAssemblyList;
            }
        }

        [XmlArray(ElementName="ReferencedDataContractTypes"), XmlArrayItem("ReferencedDataContractType", typeof(ReferencedType))]
        public List<ReferencedType> ReferencedDataContractTypeList
        {
            get
            {
                if (this.m_ReferencedDataContractTypeList == null)
                {
                    this.m_ReferencedDataContractTypeList = new List<ReferencedType>();
                }
                return this.m_ReferencedDataContractTypeList;
            }
        }

        [XmlElement]
        public ProxySerializerType Serializer
        {
            get => 
                this.m_Serializer;
            set
            {
                this.m_Serializer = value;
            }
        }

        [XmlArray(ElementName="ServiceContractMappings"), XmlArrayItem("ServiceContractMapping", typeof(ContractMapping))]
        public List<ContractMapping> ServiceContractMappingList
        {
            get
            {
                if (this.m_ServiceContractMappingList == null)
                {
                    this.m_ServiceContractMappingList = new List<ContractMapping>();
                }
                return this.m_ServiceContractMappingList;
            }
        }

        [XmlElement]
        public bool UseSerializerForFaults
        {
            get => 
                (this.m_UseSerializerForFaultsSpecified && this.m_UseSerializerForFaults);
            set
            {
                this.m_UseSerializerForFaultsSpecified = true;
                this.m_UseSerializerForFaults = value;
            }
        }

        [XmlIgnore]
        public bool UseSerializerForFaultsSpecified =>
            this.m_UseSerializerForFaultsSpecified;

        [XmlElement]
        public bool Wrapped
        {
            get => 
                (this.m_WrappedSpecified && this.m_Wrapped);
            set
            {
                this.m_WrappedSpecified = true;
                this.m_Wrapped = value;
            }
        }

        [XmlIgnore]
        public bool WrappedSpecified =>
            this.m_WrappedSpecified;

        public enum ProxySerializerType
        {
            [XmlEnum(Name="Auto")]
            Auto = 0,
            [XmlEnum(Name="DataContractSerializer")]
            DataContractSerializer = 1,
            [XmlEnum(Name="XmlSerializer")]
            XmlSerializer = 2
        }
    }
}


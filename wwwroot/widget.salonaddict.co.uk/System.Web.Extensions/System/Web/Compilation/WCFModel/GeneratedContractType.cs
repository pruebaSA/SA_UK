namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Xml.Serialization;

    internal class GeneratedContractType
    {
        private string m_ConfigurationName;
        private string m_ContractType;
        private string m_Name;
        private string m_TargetNamespace;

        public GeneratedContractType()
        {
        }

        public GeneratedContractType(string targetNamespace, string portName, string contractType, string configurationName)
        {
            this.m_TargetNamespace = targetNamespace;
            this.m_Name = portName;
            this.m_ContractType = contractType;
            this.m_ConfigurationName = configurationName;
        }

        [XmlAttribute]
        public string ConfigurationName
        {
            get => 
                this.m_ConfigurationName;
            set
            {
                this.m_ConfigurationName = value;
            }
        }

        [XmlAttribute]
        public string ContractType
        {
            get => 
                this.m_ContractType;
            set
            {
                this.m_ContractType = value;
            }
        }

        [XmlAttribute]
        public string Name
        {
            get => 
                this.m_Name;
            set
            {
                this.m_Name = value;
            }
        }

        [XmlAttribute]
        public string TargetNamespace
        {
            get => 
                this.m_TargetNamespace;
            set
            {
                this.m_TargetNamespace = value;
            }
        }
    }
}


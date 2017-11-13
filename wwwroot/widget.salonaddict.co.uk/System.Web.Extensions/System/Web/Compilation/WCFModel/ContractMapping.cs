namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Xml.Serialization;

    internal class ContractMapping
    {
        private string m_Name;
        private string m_TargetNamespace;
        private string m_TypeName;

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

        [XmlAttribute]
        public string TypeName
        {
            get => 
                this.m_TypeName;
            set
            {
                this.m_TypeName = value;
            }
        }
    }
}


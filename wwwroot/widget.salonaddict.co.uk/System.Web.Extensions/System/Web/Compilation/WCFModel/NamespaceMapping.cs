namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Xml.Serialization;

    internal class NamespaceMapping
    {
        private string m_ClrNamespace;
        private string m_TargetNamespace;

        [XmlAttribute]
        public string ClrNamespace
        {
            get => 
                this.m_ClrNamespace;
            set
            {
                this.m_ClrNamespace = value;
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


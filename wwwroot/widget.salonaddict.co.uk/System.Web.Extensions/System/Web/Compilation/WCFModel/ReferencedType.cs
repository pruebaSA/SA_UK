namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Xml.Serialization;

    internal class ReferencedType
    {
        private string m_TypeName;

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

